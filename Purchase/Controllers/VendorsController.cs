using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnsClient.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Purchase.Models;
using Purchase.Services.Contract;
using Purchase.Services.Implementation;
using ILogger = Microsoft.Extensions.Logging.ILogger;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Purchase.Controllers
{
    [Route("api/[controller]")]
    public class VendorsController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        private readonly ILogger _logger;
        public VendorsController(VendorService vendorService, ILogger<VendorsController> logger)
        {
            _vendorService = vendorService;
            _logger = logger;
        }
        // GET: api/values
        [HttpGet]
        public async Task<IEnumerable<Vendor>> Get()
        {
            var vendors = await _vendorService.GetVendorsAsync();
            return vendors;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vendor>> Get(string id)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if (vendor is null) return NotFound();
            return vendor;
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                   await _vendorService.AddVendorAsync(vendor);
                    _logger.LogInformation($"{vendor.Name} ajouté avec succés.");
                    return Ok(vendor);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return StatusCode(500, vendor);
                }
            }

            return StatusCode(500, vendor);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, Vendor updatedVendor)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if (vendor is null) return NotFound();
            updatedVendor.Id = vendor.Id;

            try
            {
               await _vendorService.UpdateVendorAsync(id, updatedVendor);
                _logger.LogInformation($"{updatedVendor.Id} modifié avec succés.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, updatedVendor);
            }

            return Ok(updatedVendor);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if (vendor is null) return NotFound();
            try
            {
                await _vendorService.DeleteVendorAsync(id);
                _logger.LogInformation($" vendor id: {id} supprimé avec succés.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return Ok(id);
        }
    }
}
