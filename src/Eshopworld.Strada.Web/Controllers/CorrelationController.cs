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
            var random = new Random();
            var orderStages = new List<string>
            {
                "CreateOrder",
                "UpdateOrder",
                "PaymentAttempted",
                "PaymentFailed",
                "PaymentSuccessful",
                "UpdateDeliveryOption",
                "ConfirmOrder",
                "OrderConfirmationFailed",
                "Processing",
                "COMPLETE"
            };

            var countries = new List<string>
            {
                "Ireland",
                "UK",
                "United States",
                "Canada",
                "Australia"
            };

            var dateTime = DateTime.Parse("2018-10-01 08:00:00").ToUniversalTime();
            DateTime endDate = DateTime.MinValue;

            while (endDate < DateTime.UtcNow)
            {
                try
                {
                    dateTime = dateTime.AddMinutes(random.Next(1, 11));
                    var isComplete = random.NextDouble() >= 0.5;
                    var orderStagesLength = isComplete ? orderStages.Count : random.Next(2, orderStages.Count - 1);
                    var country = countries[random.Next(0, countries.Count)];
                    var unitsPerOrder = random.Next(1, 8);

                    var correlationId = Guid.NewGuid().ToString();
                    var orderNumber = Guid.NewGuid().ToString();
                    DateTime startDate = dateTime.AddSeconds(random.Next(1, 10));

                    endDate = startDate.AddMinutes(random.Next(1, 6));
                    var euros = random.Next(50, 201);
                    var cents = random.Next(0, 100);
                    var amount = euros + "." + cents;

                    OrderSummary orderSummary = new OrderSummary
                    {
                        MinTimeDelay = random.Next(1000, 6000)
                    };
                    orderSummary.MaxTimeDelay = random.Next((int)orderSummary.MinTimeDelay, (int)orderSummary.MinTimeDelay + 500);
                    orderSummary.AvgTimeDelay = (orderSummary.MinTimeDelay + orderSummary.MaxTimeDelay) / 2;
                    var minFirstEventNameIndex = random.Next(0, orderStagesLength);
                    orderSummary.MinFirstEventName = orderStages[minFirstEventNameIndex];
                    var minSecondEventNameIndex = minFirstEventNameIndex < orderStagesLength - 2 ? minFirstEventNameIndex + 1 : minFirstEventNameIndex;
                    orderSummary.MinSecondEventName = orderStages[minSecondEventNameIndex];
                    var maxFirstEventNameIndex = random.Next(0, orderStagesLength);
                    orderSummary.MaxFirstEventName = orderStages[maxFirstEventNameIndex];
                    var maxSecondEventNameIndex = maxFirstEventNameIndex < orderStagesLength - 2 ? maxFirstEventNameIndex + 1 : maxFirstEventNameIndex;
                    orderSummary.MaxSecondEventName = orderStages[maxSecondEventNameIndex];
                    orderSummary.LastEventName = orderStages[orderStagesLength - 1];
                    orderSummary.Complete = isComplete;
                    orderSummary.OrderNumber = orderNumber;
                    orderSummary.StartDate = new DateTimeOffset(startDate).ToUnixTimeMilliseconds();
                    orderSummary.EndDate = new DateTimeOffset(endDate).ToUnixTimeMilliseconds();
                    orderSummary.OrderValue = float.Parse(amount);
                    orderSummary.TotalTime = (long)(endDate - startDate).TotalMilliseconds;
                    orderSummary.Country = country;
                    orderSummary.UnitsPerOrder = unitsPerOrder;
                    eventName = "EVENT";
                    //var order = new Order
                    //{
                    //    Number = orderNumber,
                    //    Value = decimal.Parse(amount),
                    //    Country = country,
                    //    UnitsPerOrder = unitsPerOrder
                    //};
                    await _domainServiceLayer.SaveOrder(
                        orderSummary,
                        eventName,
                        userAgent,
                        HttpContext.Request.QueryString.Value,
                        correlationId, dateTime);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //throw;
                }

                //var delay = random.Next(1, 6) * 1000;
                //await Task.Delay(delay);
            }

            return null;
            //await Task.Delay(500);
        }

        //return _domainServiceLayer.CorrelationId;


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