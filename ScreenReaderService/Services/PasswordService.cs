using System.Threading.Tasks;

using Autofac;
using RestSharp;
using Newtonsoft.Json;

using ScreenReaderService.Dto;
using ScreenReaderService.Util;
using ScreenReaderService.Data.Services;

namespace ScreenReaderService.Services
{
    public class PasswordService
    {
        private SessionService SessionService = DependencyHolder.Dependencies.Resolve<SessionService>();

        private const string CHANGE_PASSWORD_ENDPOINT = "Password/ChangePassword";
        public async Task UpdatePassword(string password)
        {
            ChangePasswordDto dto = new ChangePasswordDto();

            dto.Session = SessionService.GetRandomSessionDto();
            dto.Password = password;

            RestClient client = new RestClient(Constants.BASE_SERVER_ADDRESS + CHANGE_PASSWORD_ENDPOINT);
            RestRequest request = new RestRequest(Method.POST);

            request.AddJsonBody(JsonConvert.SerializeObject(dto));

            IRestResponse response = await client.ExecuteAsync(request);
        }
    }
}