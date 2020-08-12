using System.Collections.Generic;

namespace ScreenReaderService.Data.Services
{
    public class BadOrdersService
    {
        public ICollection<Order> OrdersBlackList = new List<Order>();
    }
}