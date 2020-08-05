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

        public string Username { get; private set; } = "Vasian"; // "Cesar18102";
        public string Login { get; private set; } = "vasian"; // "cesar18102";
        public string Password { get; private set; } = "vasian"; // "123456";
    }
}