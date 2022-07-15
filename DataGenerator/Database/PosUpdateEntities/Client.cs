using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataGenerator.Database.PosUpdateEntities
{
    [Table("Client")]
    public partial class Client
    {
        public Client()
        {
            Locations = new HashSet<Location>();
        }

        public int RowId { get; set; }
        [Column("CP")]
        public int Cp { get; set; }
        [Key]
        public int ClientId { get; set; }
        public int? IndustryId { get; set; }
        [Column("ASPNETID")]
        public Guid Aspnetid { get; set; }
        [Column("AdWordsID")]
        [StringLength(256)]
        public string? AdWordsId { get; set; }
        [StringLength(256)]
        public string? Name { get; set; }
        [StringLength(50)]
        public string? Phone { get; set; }
        public string? ClientPath { get; set; }
        [Column("URL")]
        [StringLength(256)]
        public string? Url { get; set; }
        [StringLength(256)]
        public string? Package { get; set; }
        public bool? IsLive { get; set; }
        [Column(TypeName = "money")]
        public decimal? MonthlyFee { get; set; }
        public string? Notes { get; set; }
        [Column("TestURL")]
        [StringLength(256)]
        public string? TestUrl { get; set; }
        public bool? DailyUpdateFeatures { get; set; }
        [Column("SecondURL")]
        [StringLength(256)]
        public string? SecondUrl { get; set; }
        [Column("NCQFile")]
        [StringLength(512)]
        public string? Ncqfile { get; set; }
        [StringLength(50)]
        public string? DashboardOrder { get; set; }
        [StringLength(10)]
        public string? Units { get; set; }
        [StringLength(256)]
        public string? ContractId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DomainExpirationDate { get; set; }
        public int PlatformVersion { get; set; }

        [InverseProperty("Client")]
        public virtual ICollection<Location> Locations { get; set; }
    }
}
