using System;
using System.Collections.Generic;
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
            var userAgentHeaders = HttpContext.Request.Headers["User-Agent"];
            var userAgent = Convert.ToString(userAgentHeaders[0]);
            Random random = new Random();
            List<string> orderStages = new List<string>
            {
                "CreateOrder",
                "UpdateOrder",
                "PaymentAttempted",
                "PaymentFailed",
                "PaymentSuccessful",
                "UpdateDeliveryOption",
                "ConfirmOrder",
                "OrderConfirmationFailed",
                "COMPLETE"
            };

            while (true)
            {
                int orderStagesLength;
                bool isComplete = random.NextDouble() >= 0.5;
                if (isComplete)
                {
                    orderStagesLength = orderStages.Count;
                }
                else
                {
                    orderStagesLength = random.Next(2, orderStages.Count); // Needs to be at least 2 events to avoid order-summary bug
                }

                var correlationId = Guid.NewGuid().ToString();
                var orderNumber = Guid.NewGuid().ToString();
                for (int i = 0; i < orderStagesLength; i++)
                {
                    eventName = orderStages[i];
                    var euros = random.Next(50, 201);
                    var cents = random.Next(0, 100);
                    var amount = euros.ToString() + "." + cents.ToString();
                    var order = new Order
                    {
                        Number = orderNumber,
                        Value = decimal.Parse(amount)
                    };
                    await _domainServiceLayer.SaveOrder(
                        order,
                        eventName,
                        userAgent,
                        HttpContext.Request.QueryString.Value,
                        correlationId);

                    int delay = random.Next(1, 6) * 1000;
                    await Task.Delay(delay);
                }
                await Task.Delay(500);
            }
            return _domainServiceLayer.CorrelationId;
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
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