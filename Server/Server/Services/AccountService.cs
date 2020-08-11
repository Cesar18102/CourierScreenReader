using System;
using System.Threading.Tasks;

using Autofac;

using Server.Dto;

using Server.Models;
using Server.Models.Exceptions;

using Server.DataAccess;
using Server.DataAccess.Entities;

namespace Server.Services
{
    public class AccountService
    {
        private static readonly AccountRepo AccountRepo = DependencyHolder.RepoDependencies.Resolve<AccountRepo>();

        private static readonly PayService PayService = DependencyHolder.ServiceDependencies.Resolve<PayService>();
        private static readonly HashingService Hasher = DependencyHolder.ServiceDependencies.Resolve<HashingService>();
        private static readonly SessionService SessionService = DependencyHolder.ServiceDependencies.Resolve<SessionService>();

        public async Task<Session> LogIn(LogInDto logInDto)
        {
            AccountEntity account = await AccountRepo.GetByLogin(logInDto.Login);

            if (account == null)
                throw new NotFoundException("User with such login");

            string originalPasswordSalted = Hasher.GetHash(account.Password + logInDto.Salt);
            if (originalPasswordSalted.ToUpper() != logInDto.PasswordSalted.ToUpper())
                throw new UnauthorizedAccessException("Wrong password");

            if (SessionService.IsActive(account.Id))
                return SessionService.GetByAccountId(account.Id);

            //await PayService.CheckPaymentRequired(account.Login);

            return SessionService.CreateSessionFor(account.Id);
        }

        public Session CheckSession(SessionDto sessionDto)
        {
            SessionService.CheckSession(sessionDto);
            return SessionService.GetByAccountId(sessionDto.UserId.Value);
        }
    }
}