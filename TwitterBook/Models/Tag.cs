using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterBook.Models
{
    public class Tag
    {
        [Key]
        public Guid Id { get; set; }

        public string TagString { get; set; }

        public Post Post { get; set; }

    }
}
