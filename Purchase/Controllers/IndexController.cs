using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Purchase.Infrastructure.Models;
using PurchaseShared.Models;

namespace Purchase.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        public IndexController(IOptions<PurchaseStoreDatabaseSettings> options)
        {
                _vendor =   new Vendor { Id = "Test", 
                    Name = "Vendor test", 
                    Telephone = "775422420", 
                    EmailId = "cfndiaye@gmail.com",
                    Comments = options.Value.ConnectionString
                };
        }

        protected Vendor _vendor;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await Task.Run(() => Ok(_vendor));
        }

        [HttpGet]
        public async Task<IActionResult> IsAvailable()
        {
            return await Task.Run(() => Ok(true));
        }
    }
}