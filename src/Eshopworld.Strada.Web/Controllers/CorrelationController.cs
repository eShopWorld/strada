﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Eshopworld.Strada.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CorrelationController : ControllerBase
    {
        private readonly DomainServiceLayer _domainServiceLayer;

        public CorrelationController(DomainServiceLayer domainServiceLayer)
        {
            _domainServiceLayer = domainServiceLayer;
        }

        /// <summary>
        ///     Saves an <see cref="Order" /> instance to DB, transmits to data analytics systems, and returns the correlation-id
        ///     pertaining to the HTTP context.
        /// </summary>
        [HttpGet]
        public async Task<string> Get(string eventName)
        {
            while (true)
            {
                var order = new Order
                {
                    Number = Guid.NewGuid().ToString(),
                    Value = 10.00m,
                    EmailAddress = "somebody@somewhere.com"
                };
                await _domainServiceLayer.SaveOrder(order, eventName);
                await Task.Delay(350);
            }

            return _domainServiceLayer.CorrelationId;
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}