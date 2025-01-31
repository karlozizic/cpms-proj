using System.ComponentModel.DataAnnotations;
using OCPP.Core.Database;
using System.Collections.Generic;

namespace OCPP.Core.Management.Models
{
    public class ChargeLocationViewModel
    {
        public List<ChargeLocation> ChargeLocations { get; set; } = new List<ChargeLocation>();

        public int LocationId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double MaxPowerLimit { get; set; }
        public double CurrentPower { get; set; }
    }
}
