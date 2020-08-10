using System;

using Server.DataAccess.Attributes;

namespace Server.DataAccess.Entities
{
    [Table("pwd_changes")]
    public class PasswordEntity : IEntity
    {
        public int Id { get; set; }

        [Column("account_id")]
        public int AccountId { get; set; }

        [Column("change_date_time")]
        public DateTime ChangeDateTime { get; set; }

        [Column("new_value")]
        public string Value { get; set; }
    }
}