using Server.DataAccess.Attributes;

namespace Server.DataAccess.Entities
{
    [Table("account")]
    public class AccountEntity : IEntity
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}