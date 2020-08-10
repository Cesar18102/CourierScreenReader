using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Autofac;

using Server.Dto;
using Server.Models;

using Server.DataAccess;
using Server.DataAccess.Entities;

namespace Server.Services
{
    public class PasswordService
    {
        private static readonly PasswordRepo PasswordRepo = DependencyHolder.RepoDependencies.Resolve<PasswordRepo>();
        private static readonly SessionService SessionService = DependencyHolder.ServiceDependencies.Resolve<SessionService>();

        public async Task<LogicResult> ChangePassword(ChangePasswordDto dto)
        {
            SessionService.CheckSession(dto.Session);
            int userId = dto.Session.UserId.GetValueOrDefault();

            IEnumerable<PasswordEntity> passwords = await PasswordRepo.GetByUserId(userId);
            PasswordEntity lastPassword = passwords.OrderByDescending(pwd => pwd.ChangeDateTime).FirstOrDefault();

            if(lastPassword == null || lastPassword.Value != dto.Password)
            {
                PasswordEntity newPassword = new PasswordEntity()
                {
                    AccountId = userId,
                    ChangeDateTime = DateTime.Now,
                    Value = dto.Password
                };

                await PasswordRepo.Insert(newPassword);
                return LogicResult.TRUE_RESULT;
            }

            return LogicResult.FALSE_RESULT;
        }
    }
}