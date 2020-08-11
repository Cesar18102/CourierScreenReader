using System;
using System.Collections.Generic;

namespace ScreenReaderService.Data
{
    public class Order
    {
        public int Id { get; private set; }

        public string From { get; private set; }
        public string To { get; private set; }

        public int Gain { get; private set; }
        public int BuyPrice { get; set; }

        public DateTime? TakeTime { get; set; }
        public DateTime? DeliverTime { get; set; }

        public ICollection<string> Modifiers { get; private set; } = new List<string>();

        public Order(int id, string from, string to, int gain)
        {
            Id = id;

            From = from;
            To = to;

            Gain = gain;
        }

        public override string ToString()
        {
            return $"ID: {Id}; A: {From}; B: {To}; Buy price {BuyPrice}";
        }
    }
}
