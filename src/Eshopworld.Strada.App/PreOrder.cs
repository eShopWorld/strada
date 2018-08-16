using System;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;

namespace Eshopworld.Strada.App
{
    /// <summary>
    ///     PreOrder is a simple, simulated PreOrder.
    /// </summary>
    internal class PreOrder
    {
        public string Number { get; set; }
        public decimal Value { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<AddressDetails> Addresses { get; set; }
    }

    internal class AddressDetails
    {
        public string Status { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }
}