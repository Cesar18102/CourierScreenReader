using System.Web.Http;
using System.Net.Http;
using System.Threading.Tasks;

using Autofac;

using Server.Dto;
using Server.Services;

namespace Server.Controllers
{
    public class PasswordController : ControllerBase
    {
        private static readonly PasswordService PasswordService = DependencyHolder.ServiceDependencies.Resolve<PasswordService>();

        [HttpPost]
        public async Task<HttpResponseMessage> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            return await Execute(async () => await PasswordService.ChangePassword(dto));
        }
    }
}
