using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DnsClient.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PurchaseShared.Models;
using Purchase.Services.Contract;
using Purchase.Services.Implementation;
using ILogger = Microsoft.Extensions.Logging.ILogger;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Purchase.Controllers
{
  [Route("api/[controller]/[action]")]
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
    public async Task<IActionResult> Get()
    {
      var vendors = await _vendorService.GetVendorsAsync();
      var vendorsOrdered = vendors.OrderBy(v => v.Name);
      return Ok(vendorsOrdered);
    }

    // GET api/values/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Vendor>> Get(string id)
    {
      var vendor = await _vendorService.GetVendorByIdAsync(id);
      if (vendor is null) return NotFound();
      return vendor;
    }

    // GET api/values/5
    [HttpGet("{vendorname}")]
    public async Task<IEnumerable<Vendor>> GetByName(string vendorname)
    {
      var vendor = await _vendorService.GetVendorsAsync(vendorname);
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
    public async Task<IActionResult> Put(string id, [FromBody] Vendor updatedVendor)
    {
      var vendor = await _vendorService.GetVendorByIdAsync(id);
      if (vendor is null) return NotFound();
      //updatedVendor.Id = vendor.Id;

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
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
      }

      return Ok(id);
    }

    //upload vendor POST api/values
    [HttpPost]
    public async Task<IActionResult> UploadVendors(IFormFile file)
    {
      if (file is null) 
        return StatusCode(500, "file not found");
      var vendors = new List<Vendor>();

      using (var streamReader = new StreamReader((file.OpenReadStream())))
      {
        while (!streamReader.EndOfStream)
        {
          var line = await streamReader.ReadLineAsync();
          string[] vendorLigne = line.Split(';');
          var vendor = new Vendor
          {
            Name = vendorLigne[0],
            EmailId = vendorLigne[1],
            Telephone = vendorLigne[3],
            Comments = vendorLigne[5]
          };
          vendors.Add(vendor);
        }
      }
      if (vendors.Count > 0)
        foreach (var vendor in vendors)
        {
          try
          {
            await _vendorService.AddVendorAsync(vendor);
          }
          catch (Exception ex)
          {
            _logger.LogError(ex.Message);

            return StatusCode(500, vendor);

          }

        }


      return Ok(vendors);
    }
  }
}