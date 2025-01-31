using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace OCPP.Core.Management.Controllers;

public partial class ApiController : BaseController
{
    [HttpPost]
    [Route("api/SmartLoadBalancing")]
    public IActionResult SmartLoadBalancing(int locationId)
    {
        var location = DbContext.ChargeLocations.FirstOrDefault(l => l.LocationId == locationId);
        if (location == null) return BadRequest("Location not found.");

        var chargePoints = DbContext.ChargePoints
            .Where(cp => cp.LocationId == locationId && cp.CurrentPower > 0)
            .OrderBy(cp => cp.CurrentPower)
            .ToList();

        double totalPowerUsed = chargePoints.Sum(cp => cp.CurrentPower ?? 0);
        double remainingPower = location.MaxPowerLimit - totalPowerUsed;

        if (remainingPower < 0)
        {
            double reductionPerCharger = Math.Abs(remainingPower) / chargePoints.Count;
            foreach (var cp in chargePoints)
            {
                if (cp.CurrentPower.HasValue)
                {
                    cp.CurrentPower = Math.Max(1, cp.CurrentPower.Value - reductionPerCharger);
                }
            }
        }
        else
        {
            foreach (var cp in chargePoints)
            {
                if (cp.CurrentPower.HasValue)
                {
                    double maxPossible = Math.Min(cp.MaxPower ?? 22, location.MaxPowerLimit - totalPowerUsed);
                    cp.CurrentPower = Math.Min(maxPossible, cp.CurrentPower.Value + 2);
                }
            }
        }

        location.CurrentPower = chargePoints.Sum(cp => cp.CurrentPower ?? 0);
        DbContext.SaveChanges();
        return Ok($"Power balanced successfully for location {location.Name}.");
    }
}