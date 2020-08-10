using Server.DataAccess.Attributes;
using System;

namespace Server.DataAccess.Entities
{
    [Table("orders")]
    public class OrderEntity : IEntity
    {
        public int Id { get; set; }

        [Column("account_id")]
        public int AccountId { get; set; }

        [Column("from_a")]
        public string FromA { get; set; }

        [Column("to_b")]
        public string ToB { get; set; }

        [Column("init_date_time")]
        public DateTime TakeDateTime { get; set; }

        public float Gain { get; set; }
        public bool Paid { get; set; }
    }
}