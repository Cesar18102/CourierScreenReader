using System.Collections.Generic;

namespace ScreenReaderService.Data
{
    public class FilterInfo
    {
        public ICollection<string> From { get; private set; } = new List<string>();
        public ICollection<string> To { get; private set; } = new List<string>();
        public ICollection<string> MustHaveModifiers { get; private set; } = new List<string>();
    }
}
