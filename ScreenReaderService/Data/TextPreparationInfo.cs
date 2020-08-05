using System.Collections.Generic;

namespace ScreenReaderService.Data
{
    public class TextPreparationInfo
    {
        public ICollection<ReplacementInfo> Replacements { get; private set; } = new List<ReplacementInfo>();
    }
}