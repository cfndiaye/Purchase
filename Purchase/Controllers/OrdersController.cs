using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PurchaseShared.Models;
using Purchase.Services.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Purchase.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly VendorService _vendorService;
        private readonly ILogger _logger;

        public OrdersController(OrderService orderService, ILogger<OrdersController> logger, VendorService vendorService)
        {
            _orderService = orderService;
            _logger = logger;
            _vendorService = vendorService;
        }
        // GET: api/<OrdersController>
        [HttpGet]
        public async Task<IEnumerable<Order>> Get()
        {
            return (await _orderService.GetOrdersAsync());
        }

        // GET api/<OrdersController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order is null) return NotFound();
            order.Vendor = await _vendorService.GetVendorByIdAsync(order.VendorId);
            return Ok(order);
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _orderService.AddOrderAsync(order);
                    _logger.LogInformation($"order ID: {order.Id} ajoute avec succes");
                    await _vendorService.UpdateVendorOrdersAsync(order.VendorId, order.Id);
                    return Ok(order);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
            return StatusCode(500, order);
        }

        // PUT api/<OrdersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Order orderUpdated)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order is null) return NotFound();
            orderUpdated.Id = order.Id;
            try
            {
                await _orderService.UpdateOrderAsync(id, orderUpdated);
                _logger.LogInformation($"Order Id: {orderUpdated.Id} modifie avec succes");
                return Ok(orderUpdated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return StatusCode(500, orderUpdated);
        }

        // DELETE api/<OrdersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order is null) return NotFound();
            try
            {
                await _orderService.DeleteOrderAsync(id);
                _logger.LogInformation($"order Id: {id} supprime avec succes.");
                await _vendorService.RemoveOrderOnVendor(id, order.Vendor);
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return StatusCode(500, order);
        }

        //Bulk import of Orders
        [HttpPost]
        public async Task<IActionResult> BulkImport([FromBody] BulkOder bulkOrder)
        {
            int count = 0;
            int errorCount = 0;

            foreach (var order in bulkOrder.Orders)
            {
                try
                {
                    await _orderService.AddOrderAsync(order);
                    //await _vendorService.UpdateVendorOrdersAsync(order.VendorId, order.Id);
                    count++;
                }
                catch (Exception e)
                {
                    errorCount++;
                    return StatusCode(500, e.Message);

                }

            }
            return Ok(new { Count = count, ErreurCount = errorCount });
        }
    }
}
