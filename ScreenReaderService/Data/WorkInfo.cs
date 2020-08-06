using System.Collections.Generic;

namespace ScreenReaderService.Data
{
    public class WorkInfo 
    {
        public ICollection<Order> ActiveOrders { get; private set; } = new List<Order>();
        public ICollection<Order> DoneOrders { get; private set; } = new List<Order>();
    }
}