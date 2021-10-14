﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterBook.Models;

namespace TwitterBook.DTO.V1.Requests
{
    public class CreatePostRequestDTO
    {
        public string Name { get; set; }

        public List<string> Tags { get; set; }

    }
}
