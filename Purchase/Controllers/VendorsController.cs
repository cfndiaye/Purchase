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
using Microsoft.AspNetCore.Authorization;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Purchase.Controllers
{
  [Route("api/[controller]/[action]")]
  [ApiController]
  [Authorize]
  public class VendorsController : ControllerBase
  {
    private readonly ILogger _logger;
    private readonly VendorService _vendorService;

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
    [HttpGet("{id}/{include}")]
    public async Task<ActionResult<Vendor>> Get(string id, bool include)
    {
      var vendor = await _vendorService.GetVendorByIdAsync(id, include);
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

    [HttpGet("{top}/{type}")]
    public async Task<List<VendorStat>> GetTopVendorsAsync(int top, string type)
    {
      var vendors = await _vendorService.GetVendorsWithOrdersAsync();

      try
      {
        var vendorStats = new List<VendorStat>();

        var vendorsWithOrders = vendors.Where(v => v.OrderList is not null && v.OrderList.Any())
                                       .Where(v => v.Type == type)
                                       .ToList();

        if (vendorsWithOrders.Any())
        {
          foreach (var vo in vendorsWithOrders)
          {
            var amount = (decimal)vo.OrderList.Sum(o => o.Amount);
            var vendorStat = new VendorStat(vo.Id, vo.Name, amount, vo.Type);
            vendorStats.Add(vendorStat);
          }
        }
        return vendorStats.OrderByDescending(vs => vs.TotalAmounts)
                          .Take(top)
                          .ToList();
      }
      catch (Exception ex)
      {
        _logger.LogCritical(ex.Message);
      }
      return null;
    }

    [HttpGet("{top}/{year}")]
    public async Task<List<VendorStat>> GetAllTopVendorsAsync(int top, int year)
    {
      var vendors = await _vendorService.GetVendorsWithOrdersAsync();

      try
      {
        var vendorStats = new List<VendorStat>();
        var vendorsWithOrders = vendors.Where(v => v.OrderList is not null && v.OrderList.Any())
                                       .ToList();

        if (vendorsWithOrders.Any())
        {
          foreach (var vo in vendorsWithOrders)
          {
            if (vo.OrderList.Where(o => o.DatePo.Value.Year == year).Count() > 0)
            {
              decimal amount = (decimal)vo.OrderList.Where(o => o.DatePo.Value.Year == year)
                                                    .Sum(o => o.Amount);

               //verify the currency and convert to dollars us
               decimal amountToConvert = amount;
              switch (vo.Type)
              {

                case "Locale":
                  //amount /= 630;
                  amountToConvert = Devise.ConvertCfaToUsd(amount);
                  break;
                case "Europe":
                  amountToConvert = Devise.ConvertEuroToUsd(amount) ;
                  break;
                case "Asie":
                  break;
                case "Usa":
                  break;

              }
              var vendorStat = new VendorStat(vo.Id, vo.Name, amountToConvert, vo.Type);
              vendorStats.Add(vendorStat);
            }
          }
        }
        return vendorStats.OrderByDescending(vs => vs.TotalAmounts).Take(top).ToList();
      }
      catch (Exception ex)
      {
        _logger.LogCritical(ex.Message);
      }
      return null;
    }
  }

}
