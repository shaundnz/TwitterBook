﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TwitterBook.Data;
using TwitterBook.Models;
using TwitterBook.Options;

namespace TwitterBook.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DataContext _context;

        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters, DataContext dataContext)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _context = dataContext;
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AuthenticationResult { Errors = new[] { "User does not exist" } };
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!userHasValidPassword) return new AuthenticationResult { Errors = new[] { "User/Password combination is not valid" } };

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var principals = GetPrincipalFromToken(token);
            if (principals == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Invalid Token" }
                };
            }

            var expiryDateUnix = long.Parse(principals.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUTC = DateTime.UnixEpoch.AddSeconds(expiryDateUnix);

            if (expiryDateTimeUTC > DateTime.Now) return new AuthenticationResult { Errors = new[] { "This token hasn't expired yet." } };

            var jti = principals.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(refToken => refToken.Token.ToString() == refreshToken);

            if (storedRefreshToken == null) return new AuthenticationResult { Errors = new[] { "This refresh token does not exist" } };

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate) return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };

            if (storedRefreshToken.Invalidated) return new AuthenticationResult { Errors = new[] { "This refresh token has been invalidated" } };

            if (storedRefreshToken.Used) return new AuthenticationResult { Errors = new[] { "This refresh token has been used" } };

            if (storedRefreshToken.JwtId != jti) return new AuthenticationResult { Errors = new[] { "This refresh token not valid for this access token" } };

            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(principals.Claims.Single(claim => claim.Type == "id").Value);
            return await GenerateAuthenticationResultForUserAsync(user);

        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                // Slightly modified _tokenValidationParameters, Validate lifetime set to false to allow new access token to be given even if access token is expired
                var tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }
                return principal;
            } catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return new AuthenticationResult { Errors = new[] { "User with this email address already exists" } };
            }

            var newUser = new IdentityUser
            {
                Email = email,
                UserName = email
            };

            var createdUser = await _userManager.CreateAsync(newUser, password);
            
            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }
            
            await _userManager.AddClaimAsync(newUser, new Claim("tags.view", "true"));
            
            return await GenerateAuthenticationResultForUserAsync(newUser);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id)
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            
            claims.AddRange(userClaims);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthenticationResult
            {
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token.ToString(),
                Success = true
            };
        }
    }
}
