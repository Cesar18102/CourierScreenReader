using System.Collections.Generic;

namespace ScreenReaderService.Data
{
    public class ReplacementInfo
    {
        public ICollection<string> Find { get; private set; } = new List<string>();
        public string Replace { get; private set; }

        public ReplacementInfo(ICollection<string> find, string replace)
        {
            Find = find;
            Replace = replace;
        }
    }
}