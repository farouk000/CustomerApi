using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerApi.Models
{
    public class Version
    {
        public int Id { get; set; }
        [Required]
        public string VersionNumber { get; set; }
        [Required]
        public string Date { get; set; }
        public string Description { get; set; }
        public int IsDeleted { get; set; }
    }
}
