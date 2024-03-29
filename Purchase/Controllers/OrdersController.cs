﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PurchaseShared.Models;
using Purchase.Services.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Purchase.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
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
        //Ce code permet d'obtenir une liste d'objets Order triés par date d'approbation décroissante. 
        //Il fait appel à la méthode GetOrdersAsync() de l'objet _orderService pour obtenir une liste d'objets Order. 
        //La variable includeVendor est utilisée pour indiquer si les fournisseurs doivent être inclus ou non. 
        //Une fois la liste obtenue, elle est triée par date d'approbation décroissante et retournée.
        ///

        [HttpGet]
        public async Task<IEnumerable<Order>> Get()
        {
            var includeVendor = true;
            List<Order> orders = (await _orderService.GetOrdersAsync(includeVendor)).ToList();
            return orders.OrderByDescending(o => o.DatePo);
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

        //GET api/<OrdersController>/status
        [HttpGet("{status}")]
        public async Task<IActionResult> GetOrdersByStatus(string status)
        {
            var orders = await _orderService.GetOrdersByStatusAsync(status);
            return Ok(orders);
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

        // POST api/orders
        [HttpPost]
        public async Task<IActionResult> PostPending([FromBody] Order order)
        {

            if (ModelState.IsValid)
            {
                var existingOrder = await _orderService.GetOrderByPrAsync(order.ReqNumber);
                if (existingOrder == null)
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
                else
                {
                    _logger.LogInformation($"order ID: {existingOrder.Id} exist déja!");
                    return StatusCode(400, "Order already exists");
                }
            }
            return StatusCode(500, "Order model non valide");
        }

        // PUT api/<OrdersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Order orderUpdated)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order is null) return NotFound();

            orderUpdated.Id = order.Id;
            if (orderUpdated.VendorId != order.VendorId)
            {

                try
                {

                    var vendor = await _vendorService.GetVendorByIdAsync(order.VendorId);
                    await _vendorService.RemoveOrderOnVendor(order.Id, vendor);
                    _logger.LogInformation("commande enlevée de vendor");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return StatusCode(500, "Erreur lors du suppression de la commande dans vendor");
                }
            }
            try
            {
                await _orderService.UpdateOrderAsync(id, orderUpdated);
                _logger.LogInformation($"Order Id: {orderUpdated.Id} modifie avec succes");
                await _vendorService.UpdateVendorOrdersAsync(orderUpdated.VendorId, orderUpdated.Id);
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

        //GET total cost  orders by currency
        [HttpGet("{devise}")]
        public async Task<double> GetTotalCostByCurrency(string devise)
        {
            var cost = await _orderService.GetTotalCostAsync(devise);

            return cost;
        }

        // GET total cost orders by Vendor type
        [HttpGet("{type}")]
        public async Task<double> GetTotalCostByType(string type)
        {
            var cost = await _orderService.GetTotalCostByVendorType(type);

            return cost;
        }

        // GET total cost orders by Vendor type by year
        [HttpGet("{type}/{year}")]
        public async Task<double> GetTotalCostByType(string type, int year)
        {
            var cost = await _orderService.GetTotalCostByVendorType(type, year);

            return cost;
        }

        /// <summary>
        /// Get total count orders by vendor type and year
        /// </summary>
        /// <param name="type"></param>
        /// <param name="year"></param>
        /// <returns>int</returns>
        [HttpGet("{type}/{year}")]
        public async Task<int> GetTotalCounterByVendorType(string type, int year)
        {
            var counter = await _orderService.GetTotalCounterByVendorType(type, year);
            return counter;
        }

        /// <summary>
        /// Get total count orders by vendor type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>int</returns>
        [HttpGet("{type}")]
        public async Task<int> GetTotalCounterByVendorType(string type)
        {
            var counter = await _orderService.GetTotalCounterByVendorType(type);
            return counter;
        }

        /// <summary>
        /// Get All orders by vendor Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<List<Order>> GetOrdersByVendorIdAsync(string id)
        {
            return (await _orderService.GetOrdersAsync()).Where(o => o.VendorId == id).ToList();
        }

        /// <summary>
        /// Get All orders by vendor Id by year
        /// </summary>
        /// <param name="id"></param>
        /// <param name="year"></param>
        /// <returns></returns>  
        [HttpGet("{id}/{year}")]
        public async Task<List<Order>> GetOrdersByVendorIdAsync(string id, int year)
        {
            return (await _orderService.GetOrdersAsync()).Where(o => o.VendorId == id && o.DatePo.Value.Year == year).ToList();
        }

        [HttpGet("{status}")]
        public async Task<int> GetOrderCounter(string status)
        {
            return (await _orderService.GetOrdersByStatusAsync(status)).Count();
        }



    }
}
