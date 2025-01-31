using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OCPP.Core.Database;
using OCPP.Core.Management.Models;

namespace OCPP.Core.Management.Controllers
{
    public partial class HomeController : BaseController
    {
        [Authorize]
        public IActionResult ChargeLocation()
        {
            try
            {
                Logger.LogInformation("ChargeLocation: Loading charge locations...");
                var chargeLocations = DbContext.ChargeLocations.ToList();
                Logger.LogInformation("ChargeLocation: Found {0} charge locations", chargeLocations.Count);

                if (!chargeLocations.Any())
                {
                    Logger.LogWarning("ChargeLocation: No locations found in database!");
                }

                return View(new ChargeLocationViewModel { ChargeLocations = chargeLocations });
            }
            catch (Exception exp)
            {
                Logger.LogError(exp, "ChargeLocation: Error loading charge locations from database");
                TempData["ErrMessage"] = exp.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult ChargeLocationDetails(int id)
        {
            try
            {
                Logger.LogTrace("ChargeLocation: Loading details for location {0}", id);
                var location = DbContext.ChargeLocations.Find(id);
                if (location == null)
                {
                    Logger.LogWarning("ChargeLocation: Location {0} not found", id);
                    return NotFound();
                }
                return View(location);
            }
            catch (Exception exp)
            {
                Logger.LogError(exp, "ChargeLocation: Error loading location details");
                TempData["ErrMessage"] = exp.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult CreateChargeLocation()
        {
            return View(new ChargeLocation());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateChargeLocation(ChargeLocation location)
        {
            if (!ModelState.IsValid)
                return View(location);

            try
            {
                Logger.LogTrace("ChargeLocation: Creating new location {0}", location.Name);
                DbContext.ChargeLocations.Add(location);
                DbContext.SaveChanges();
                Logger.LogInformation("ChargeLocation: Location {0} created successfully", location.Name);
                return RedirectToAction("ChargeLocation");
            }
            catch (Exception exp)
            {
                Logger.LogError(exp, "ChargeLocation: Error creating location");
                TempData["ErrMessage"] = exp.Message;
                return View(location);
            }
        }

        [HttpGet]
        public IActionResult EditChargeLocation(int id)
        {
            var location = DbContext.ChargeLocations.Find(id);
            return location == null ? NotFound() : View(location);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditChargeLocation(int id, ChargeLocation location)
        {
            if (id != location.LocationId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(location);

            try
            {
                DbContext.Update(location);
                DbContext.SaveChanges();
                return RedirectToAction("ChargeLocation");
            }
            catch (Exception exp)
            {
                Logger.LogError(exp, "ChargeLocation: Error updating location");
                TempData["ErrMessage"] = exp.Message;
                return View(location);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteChargeLocation(int id)
        {
            try
            {
                var location = DbContext.ChargeLocations.Find(id);
                if (location == null)
                    return NotFound();

                DbContext.ChargeLocations.Remove(location);
                DbContext.SaveChanges();
                return RedirectToAction("ChargeLocation");
            }
            catch (Exception exp)
            {
                Logger.LogError(exp, "ChargeLocation: Error deleting location");
                TempData["ErrMessage"] = exp.Message;
                return RedirectToAction("ChargeLocation");
            }
        }
    }
}
