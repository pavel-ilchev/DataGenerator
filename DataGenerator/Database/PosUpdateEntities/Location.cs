using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataGenerator.Database.PosUpdateEntities
{
    public partial class Location
    {
        public int RowId { get; set; }
        [Column("CP")]
        public int Cp { get; set; }
        [Key]
        public int LocationId { get; set; }
        public int? ClientId { get; set; }
        [StringLength(256)]
        public string? Name { get; set; }
        public Guid? Guid { get; set; }
        [Column("POSId")]
        public int? Posid { get; set; }
        public int TimeZoneId { get; set; }
        [StringLength(50)]
        public string? CurrentVersionDll { get; set; }
        [StringLength(50)]
        public string? CurrentVersionExe { get; set; }
        public string? SettingsXml { get; set; }
        [StringLength(3)]
        public string? PhoneAreaCode { get; set; }
        [Column("ConnectionID")]
        [StringLength(50)]
        public string? ConnectionId { get; set; }
        [Column("APIKey")]
        [StringLength(50)]
        public string? Apikey { get; set; }
        [Column("BaseURL")]
        [StringLength(500)]
        public string? BaseUrl { get; set; }
        [Column("DBName")]
        [StringLength(50)]
        public string? Dbname { get; set; }
        public int? ExternalId { get; set; }
        [StringLength(50)]
        public string? Username { get; set; }
        [StringLength(50)]
        public string? Password { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateLimit { get; set; }
        public bool? PerformFirstUpdate { get; set; }
        [Column("MACAddress")]
        [StringLength(255)]
        public string? Macaddress { get; set; }
        [StringLength(50)]
        public string? LocationConnectorKey { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ConnectorLastInstallDate { get; set; }
        [StringLength(512)]
        public string? DeleteHashOn { get; set; }
        [StringLength(50)]
        public string? Country { get; set; }
        public int? ConnectionTypeId { get; set; }
        public int? PosSubVersionId { get; set; }
        [StringLength(512)]
        public string? ConnectionStringComponents { get; set; }
        public TimeSpan? FailoverServiceUpdateTime { get; set; }
        [StringLength(512)]
        public string? DeleteOldHashOn { get; set; }

        [ForeignKey("ClientId")]
        [InverseProperty("Locations")]
        public virtual Client? Client { get; set; }
    }
}
