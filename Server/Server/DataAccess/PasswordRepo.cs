using System.Threading.Tasks;
using System.Collections.Generic;

using Server.DataAccess.Entities;

namespace Server.DataAccess
{
    public class PasswordRepo : RepoBase<PasswordEntity>
    {
        public PasswordRepo(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<PasswordEntity>> GetByUserId(int userId)
        {
            return await Get(pwd => pwd.AccountId, userId);
        }
    }
}