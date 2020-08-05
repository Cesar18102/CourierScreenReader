using System.Collections.Generic;

namespace ScreenReaderService.Data
{
    public class Order
    {
        public int Id { get; private set; }

        public string From { get; private set; }
        public string To { get; private set; }

        public ICollection<string> Modifiers { get; private set; } = new List<string>();

        public Order(int id, string from, string to)
        {
            Id = id;

            From = from;
            To = to;
        }

        public override string ToString()
        {
            return $"ID: {Id}; A: {From}; B: {To}";
        }
    }
}
