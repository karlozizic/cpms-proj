using System;
using System.Collections.Generic;

#nullable disable

namespace OCPP.Core.Database
{
    public partial class ChargeLocation
    {
        public ChargeLocation()
        {
            ChargePoints = new HashSet<ChargePoint>();
        }

        public int LocationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double MaxPowerLimit { get; set; }
        public double CurrentPower { get; set; }
        public virtual ICollection<ChargePoint> ChargePoints { get; set; }
    }
}