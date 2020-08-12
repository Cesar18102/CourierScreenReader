using System;
using System.Collections.Generic;

namespace ScreenReaderService.Data
{
    public class Order : IEquatable<Order>
    {
        public int Id { get; private set; }

        public string From { get; private set; }
        public string To { get; private set; }

        public int Gain { get; private set; }
        public int BuyPrice { get; set; }

        public DateTime? MinTakeTime { get; set; }
        public DateTime? MaxTakeTime { get; set; }

        public DateTime? MinDeliverTime { get; set; }
        public DateTime? MaxDeliverTime { get; set; }

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
            return $"ID: {Id}\n" +
                   $"A: {From}\n" +
                   $"B: {To}\n" +
                   $"Buy price: {BuyPrice}\n" +
                   $"Gain: {Gain}\n" +
                   $"Take time: {MinTakeTime.ToString()} - {MaxTakeTime.ToString()}\n" +
                   $"Deliver time: {MinDeliverTime.ToString()} - {MaxDeliverTime.ToString()}";
        }

        public bool Equals(Order other)
        {
            return other.From == From && other.To == To && other.Gain == Gain &&
                   other.MinTakeTime == MinTakeTime && other.MaxTakeTime == MaxTakeTime &&
                   other.MinDeliverTime == MinDeliverTime && other.MaxDeliverTime == MaxDeliverTime;
        }
    }
}
