using System.Web.Http;
using System.Net.Http;
using System.Threading.Tasks;

using Autofac;

using Server.Services;
using Server.API.LiqPay.Dto;

namespace Server.Controllers
{
    public class PayController : ControllerBase
    {
        private static readonly PayService PayService = DependencyHolder.ServiceDependencies.Resolve<PayService>();

        [HttpGet]
        public async Task<HttpResponseMessage> GetPaymentForm(int userId)
        {
            return await Execute(async () => await PayService.GetPaymentForm(userId));
        }

        public async Task<HttpResponseMessage> HandlePaymentCallback(PaymentConfirmDto dto)
        {
            return await Execute(async () => await PayService.HandlePaymentCallback(dto));
        }
    }
}
