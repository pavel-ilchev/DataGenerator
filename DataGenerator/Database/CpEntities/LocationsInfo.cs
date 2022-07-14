using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataGenerator.Database.CpEntities
{
    [Table("LocationsInfo")]
    [Index("LocationId", Name = "UQ__Location__E7FEA496133C5605", IsUnique = true)]
    public partial class LocationsInfo
    {
        [Key]
        public int Id { get; set; }
        public int LocationId { get; set; }
        public int ClientId { get; set; }
        [StringLength(50)]
        public string? TrackingPhone { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        [Column(TypeName = "decimal(16, 2)")]
        public decimal? AdWordsBudget { get; set; }
        [StringLength(100)]
        public string? LeadEmail { get; set; }
        [StringLength(100)]
        public string? CustomLocationIdentifier { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateAdded { get; set; }

        [ForeignKey("ClientId")]
        [InverseProperty("LocationsInfos")]
        public virtual Client Client { get; set; } = null!;
        [ForeignKey("LocationId")]
        [InverseProperty("LocationsInfo")]
        public virtual Location Location { get; set; } = null!;
    }
}
