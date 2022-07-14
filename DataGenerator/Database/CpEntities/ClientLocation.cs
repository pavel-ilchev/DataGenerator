using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataGenerator.Database.CpEntities
{
    [Table("ClientLocation")]
    public partial class ClientLocation
    {
        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        [StringLength(2)]
        public string? Country { get; set; }
        [StringLength(256)]
        public string? Address { get; set; }
        [StringLength(50)]
        public string? Phone { get; set; }
        [StringLength(256)]
        public string? City { get; set; }
        [StringLength(50)]
        public string? Fax { get; set; }
        [StringLength(256)]
        public string? State { get; set; }
        [StringLength(256)]
        public string? Email { get; set; }
        [StringLength(256)]
        public string? Zip { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }

        [ForeignKey("ClientId")]
        [InverseProperty("ClientLocations")]
        public virtual Client Client { get; set; } = null!;
    }
}
