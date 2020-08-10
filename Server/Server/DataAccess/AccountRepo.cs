using System.Threading.Tasks;
using Server.DataAccess.Entities;

namespace Server.DataAccess
{
    public class AccountRepo : RepoBase<AccountEntity>
    {
        public AccountRepo(string connectionString) : base(connectionString) { }

        public async Task<AccountEntity> GetById(int id)
        {
            return await FirstOrDefault(account => account.Id, id);
        }

        public async Task<AccountEntity> GetByLogin(string login)
        {
            return await FirstOrDefault(account => account.Login, login);
        }
    }
}