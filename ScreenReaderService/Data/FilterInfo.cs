using System.Collections.Generic;

namespace ScreenReaderService.Data
{
    public class FilterInfo
    {
        public ICollection<string> From { get; private set; } = new List<string>();
        public ICollection<string> To { get; private set; } = new List<string>();
        public ICollection<string> MustHaveModifiers { get; private set; } = new List<string>();

        public override string ToString()
        {
            string froms = string.Join('\n', From);
            string tos = string.Join('\n', To);
            string mods = string.Join('\n', MustHaveModifiers);

            return $"A-points: \n{froms}\n\n" +
                   $"B-points: \n{tos}\n\n" +
                   $"Modifiers: \n{mods}";
        }
    }
}
