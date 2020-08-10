using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

using Autofac;

using Server.Dto;
using Server.Services;

namespace Server.Controllers
{
    public class OrderController : ControllerBase
    {
        private static readonly OrderService OrderService = DependencyHolder.ServiceDependencies.Resolve<OrderService>();

        public async Task<HttpResponseMessage> RegisterOrderTake([FromBody] TakeOrderDto takeOrderDto)
        {
            return await Execute(async () => await OrderService.TakeOrder(takeOrderDto));
        }
    }
}
