using System;

namespace ScreenReaderService.Data
{
    public class ConstraintsInfo
    {
        public int MaxOrdersPerDay { get; set; }
        public int MaxActiveOrdersAtTime { get; set; }

        public int MaxOrderPrice { get; set; }
        public int MaxTotalSpent { get; set; }

        public DateTime? MinTakeTime { get; set; }
        public DateTime? MaxTakeTime { get; set; }

        public DateTime? MinDeliverTime { get; set; }
        public DateTime? MaxDeliverTime { get; set; }

        public override string ToString()
        {
            return $"Max active orders: {MaxActiveOrdersAtTime}\n" +
                   $"Max orders per day: {MaxOrdersPerDay}\n" +
                   $"Max order price: {MaxOrderPrice}\n" +
                   $"Max total spent: {MaxTotalSpent}\n" +
                   $"Min take time: {(MinTakeTime.HasValue ? MinTakeTime.Value.ToString() : "undefined")}\n" +
                   $"Max take time: {(MaxTakeTime.HasValue ? MaxTakeTime.Value.ToString() : "undefined")}\n" +
                   $"Min deliver time: {(MinDeliverTime.HasValue ? MinDeliverTime.Value.ToString() : "undefined")}\n" +
                   $"Max deliver time: {(MaxDeliverTime.HasValue ? MaxDeliverTime.Value.ToString() : "undefined")}\n";
        }
    }
}
