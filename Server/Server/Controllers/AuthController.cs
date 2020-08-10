using System.Threading.Tasks;

using System.Net.Http;
using System.Web.Http;

using Autofac;

using Server.Dto;
using Server.Services;

namespace Server.Controllers
{
    public class AuthController : ControllerBase
    {
        private static readonly AccountService AccountService = DependencyHolder.ServiceDependencies.Resolve<AccountService>();

        [HttpPost]
        public async Task<HttpResponseMessage> LogIn([FromBody]LogInDto logInDto)
        {
            return await Execute(
                async () => await AccountService.LogIn(logInDto)
            );
        }

        [HttpPost]
        public HttpResponseMessage CheckSession([FromBody] SessionDto session)
        {
            return Execute(() => AccountService.CheckSession(session));
        }
    }
}
