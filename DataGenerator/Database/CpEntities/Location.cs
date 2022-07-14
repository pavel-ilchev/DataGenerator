using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataGenerator.Database.CpEntities
{
    public partial class Location
    {
        [Key]
        public int Id { get; set; }
        public int? ClientId { get; set; }
        [StringLength(256)]
        public string? Name { get; set; }
        public Guid? Guid { get; set; }
        [Column("POSId")]
        public int? Posid { get; set; }
        public int TimeZoneId { get; set; }
        public bool? AppointmentIntegration { get; set; }
        [StringLength(50)]
        public string? CurrentVersionDll { get; set; }
        [StringLength(50)]
        public string? CurrentVersionExe { get; set; }
        public string? SettingsXml { get; set; }
        public string? TaskScheduler { get; set; }
        public bool? GuidPublished { get; set; }
        [StringLength(3)]
        public string? PhoneAreaCode { get; set; }
        public Guid? ApiGuid { get; set; }
        [Column("ConnectorScheduledTIme", TypeName = "datetime")]
        public DateTime? ConnectorScheduledTime { get; set; }
        [Column("MACAddress")]
        [StringLength(255)]
        public string? Macaddress { get; set; }
        [Column("apikeyProfitBoost")]
        [StringLength(50)]
        public string? ApikeyProfitBoost { get; set; }
        [Column("dbnameProfitBoost")]
        [StringLength(50)]
        public string? DbnameProfitBoost { get; set; }
        [StringLength(50)]
        public string? LocationConnectorKey { get; set; }
        [StringLength(50)]
        public string? MindBodyUserName { get; set; }
        [StringLength(50)]
        public string? MindBodyUserPass { get; set; }
        public int? MindBodySiteId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ConnectorLastInstallDate { get; set; }
        public bool? CreateRemoteConnection { get; set; }
        [Column("APIKey")]
        [StringLength(50)]
        public string? Apikey { get; set; }
        [StringLength(50)]
        public string? UserName { get; set; }
        [StringLength(50)]
        public string? Password { get; set; }
        [StringLength(50)]
        public string? ExternalId { get; set; }
        [StringLength(250)]
        public string? Other { get; set; }
        public bool? OneHourUpdate { get; set; }
        public int? CallClassifierCutoff { get; set; }
        public bool? CallClassifierEnabled { get; set; }
        [StringLength(256)]
        public string? LocAddress { get; set; }
        [StringLength(128)]
        public string? LocCity { get; set; }
        [StringLength(64)]
        public string? LocRegion { get; set; }
        [StringLength(30)]
        public string? LocPostCode { get; set; }
        [StringLength(10)]
        public string? LocCountryAbbreviation { get; set; }

        [ForeignKey("ClientId")]
        [InverseProperty("Locations")]
        public virtual Client? Client { get; set; }
        [InverseProperty("Location")]
        public virtual LocationsInfo LocationsInfo { get; set; } = null!;
    }
}
