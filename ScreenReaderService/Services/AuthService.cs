using System.Threading.Tasks;

using Autofac;
using RestSharp;
using Newtonsoft.Json;

using ScreenReaderService.Data;
using ScreenReaderService.Dto;
using ScreenReaderService.Util;
using ScreenReaderService.Data.Services;

namespace ScreenReaderService.Services
{
    public class AuthService
    {
        private SaltService SaltService = DependencyHolder.Dependencies.Resolve<SaltService>();
        private HashingService HashingService = DependencyHolder.Dependencies.Resolve<HashingService>();
        private StatusCodeHandlerService StatusCodeHandlerService = DependencyHolder.Dependencies.Resolve<StatusCodeHandlerService>();

        private SessionService SessionService = DependencyHolder.Dependencies.Resolve<SessionService>();

        private const string LOG_IN_ENDPOINT = "auth/login";
        public async Task LogIn(string login, string password)
        {
            LogInDto dto = new LogInDto();

            dto.Login = login;
            dto.Salt = SaltService.GetRandomSalt();

            string hashedPwd = HashingService.GetHash(password);
            dto.PasswordSalted = SaltService.GetSaltedHash(hashedPwd, dto.Salt);
            
            RestClient client = new RestClient(Constants.BASE_SERVER_ADDRESS + LOG_IN_ENDPOINT);
            RestRequest request = new RestRequest(Method.POST);

            request.AddJsonBody(JsonConvert.SerializeObject(dto));

            IRestResponse response = await client.ExecuteAsync(request);

            StatusCodeHandlerService.HandleResponseStatus(response.StatusCode, response.Content);
            SessionService.Session = JsonConvert.DeserializeObject<Session>(response.Content);
        }
    }
}