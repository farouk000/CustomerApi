using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerApi.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        public string BackOfficeUrl { get; set; }
        [Required]
        public string BackOfficeSite { get; set; }
        [Required]
        public int BackOfficePort { get; set; }
        public string BackOfficeProtocol { get; set; }
        [Required]
        public string FrontOfficeUrl { get; set; }
        [Required]
        public string FrontOfficeSite { get; set; }
        [Required]
        public int FrontOfficePort { get; set; }
        public string FrontOfficeProtocol { get; set; }
        [Required]
        public string DBName { get; set; }
        [Required]
        public string BackupFile { get; set; }
        [Required]
        public int VersionId { get; set; }
        public string VersionNumber { get; set; }
    }
}
