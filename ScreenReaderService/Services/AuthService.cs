using System;
using System.Threading.Tasks;

using Autofac;
using Newtonsoft.Json;
using RestSharp;

using ScreenReaderService.Dto;

namespace ScreenReaderService.Services
{
    public class AuthService
    {
        private SaltService SaltService = DependencyHolder.Dependencies.Resolve<SaltService>();
        private HashingService HashingService = DependencyHolder.Dependencies.Resolve<HashingService>();

        private const string LOG_IN_ENDPOINT = "http://courierbot2020.somee.com/api/auth/login";
        public async Task LogIn(string login, string password)
        {
            LogInDto dto = new LogInDto();

            dto.Login = login;
            dto.Salt = SaltService.GetRandomSalt();

            string hashedPwd = HashingService.GetHash(password);
            dto.PasswordSalted = SaltService.GetSaltedHash(hashedPwd, dto.Salt);

            RestClient client = new RestClient(LOG_IN_ENDPOINT);
            RestRequest request = new RestRequest(Method.POST);

            request.AddJsonBody(JsonConvert.SerializeObject(dto));

            IRestResponse response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                throw new UnauthorizedAccessException();
        }
    }
}