using System.Threading.Tasks;

using Autofac;
using RestSharp;
using Newtonsoft.Json;

using ScreenReaderService.Dto;
using ScreenReaderService.Util;

using ScreenReaderService.Data;
using ScreenReaderService.Data.Services;

namespace ScreenReaderService.Services
{
    public class OrderService
    {   
        private SessionService SessionService = DependencyHolder.Dependencies.Resolve<SessionService>();

        private const string TAKE_ORDER_ENDPOINT = "Order/RegisterOrderTake";
        public async Task TakeOrder(Order order)
        {
            TakeOrderDto dto = new TakeOrderDto();

            dto.Session = SessionService.GetRandomSessionDto();
            dto.Order = new OrderDto()
            {
                FromA = order.From,
                ToB = order.To,
                Gain = order.Gain
            };

            RestClient client = new RestClient(Constants.BASE_SERVER_ADDRESS + TAKE_ORDER_ENDPOINT);
            RestRequest request = new RestRequest(Method.POST);

            request.AddJsonBody(JsonConvert.SerializeObject(dto));

            IRestResponse response = await client.ExecuteAsync(request);
        }
    }
}