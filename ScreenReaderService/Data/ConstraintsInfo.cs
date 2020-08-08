namespace ScreenReaderService.Data
{
    public class ConstraintsInfo
    {
        public int MaxOrdersPerDay { get; set; }
        public int MaxActiveOrdersAtTime { get; set; }
        public int MaxOrderPrice { get; set; }
        public int MaxTotalSpent { get; set; }

        public override string ToString()
        {
            return $"Max active orders: {MaxActiveOrdersAtTime}\n" +
                   $"Max orders per day: {MaxOrdersPerDay}";
        }
    }
}
