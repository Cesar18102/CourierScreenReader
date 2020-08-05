using System.Collections.Generic;

namespace ScreenReaderService.Data
{
    public class BotInfo 
    {
        public ICollection<Order> ActiveOrders { get; private set; } = new List<Order>();
        public ICollection<Order> DoneOrders { get; private set; } = new List<Order>();

        public int IdCounter { get; set; }
        public Order DiscoveredOrder { get; set; }

        public bool Paused { get; set; }
    }
}