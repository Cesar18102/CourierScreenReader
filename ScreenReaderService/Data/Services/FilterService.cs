using System.IO;
using System.Linq;

using Android.OS;

using Autofac;

namespace ScreenReaderService.Data.Services
{
    public class FilterService : ObjectFileMappingService<FilterInfo>
    {
        private const string FILTER_INFO_FILE_NAME = "filters.json";

        private static readonly string FILTER_INFO_PATH = Path.Combine(
            Environment.ExternalStorageDirectory.AbsolutePath, "CourierBot"
        );

        public FilterService() : base(FILTER_INFO_PATH, FILTER_INFO_FILE_NAME) { }

        public FilterInfo Filters
        {
            get => Data;
            set => Data = value;
        }

        private TextPreparationService TextPreparationService = DependencyHolder.Dependencies.Resolve<TextPreparationService>();

        public bool Assert(Order order)
        {
            string orderFrom = Prepare(order.From);
            bool from = Filters.From.Any(start => Assert(Prepare(start), orderFrom));

            if (!from)
                return false;

            string orderTo = Prepare(order.To);
            bool to = Filters.To.Any(end => Assert(Prepare(end), orderTo));

            if (!to)
                return false;

            bool mods = Filters.MustHaveModifiers.All(
                mustHaveMod => order.Modifiers.Any(
                    mod => Assert(mustHaveMod.ToLower(), mod.ToLower())
                )
            );

            return from & to & mods;
        }

        private string Prepare(string source)
        {
            string result = source.ToLower();

            foreach (ReplacementInfo replacement in TextPreparationService.PreparationInfo.Replacements)
                foreach (string find in replacement.Find)
                {
                    string search = find.ToLower();
                    if (result.Contains(search))
                    {
                        result = result.Replace(search, replacement.Replace);
                        break;
                    }
                }

            return result.ToLower().Trim();
        }

        private const float ACCEPTED_WORD_MISTAKE_PERCENT = 0.1f;
        private const float ACCEPTED_PHRASE_MISTAKE_PERCENT = 0.1f;

        private bool Assert(string filter, string sample)
        {
            return sample.Contains(filter);

            if (sample.Contains(filter))
                return true;

            string[] filterWords = filter.Split(' ');
            string[] sampleWords = sample.Split(' ');

            bool[] usedFilter = new bool[filterWords.Length];
            bool[] usedSamples = new bool[sampleWords.Length];

            int totalDiffer = 0;

            for (int i = 0; i < sampleWords.Length; ++i)
            {
                int firstUnusedFilter = 0;
                while (firstUnusedFilter < filterWords.Length && usedFilter[firstUnusedFilter])
                    ++firstUnusedFilter;

                if (firstUnusedFilter == filterWords.Length)
                    break;

                int bestFilter = firstUnusedFilter;
                int best = Lewenstein(sampleWords[i], filterWords[firstUnusedFilter]);

                for(int j = firstUnusedFilter + 1; j < filterWords.Length; ++j)
                {
                    if (usedFilter[j])
                        continue;

                    int differ = Lewenstein(sampleWords[i], filterWords[j]);

                    if(differ < best)
                    {
                        bestFilter = j;
                        best = differ;
                    }
                }

                float wordDifferPercent = (float)best / sampleWords[i].Length;

                if (wordDifferPercent <= ACCEPTED_WORD_MISTAKE_PERCENT)
                {
                    totalDiffer += best;
                    usedFilter[bestFilter] = true;
                    usedSamples[i] = true;
                }
            }

            for (int i = 0; i < usedFilter.Length; ++i)
                totalDiffer += usedFilter[i] ? 0 : filterWords[i].Length;

            for (int i = 0; i < usedSamples.Length; ++i)
                totalDiffer += usedSamples[i] ? 0 : sampleWords[i].Length;

            float phraseDifferPercent = (float)totalDiffer / sample.Length;
            return phraseDifferPercent <= ACCEPTED_PHRASE_MISTAKE_PERCENT;
        }

        private int Lewenstein(string s1, string s2)
        {
            int?[,] cache = new int?[s1.Length, s2.Length];
            return Lewenstein(s1, s2, s1.Length - 1, s2.Length - 1, cache);
        }

        private int Lewenstein(string s1, string s2, int i, int j, int?[,] cache)
        {
            if (i == -1 && j == -1)
                return 0;

            if (i >= 0 && j == -1)
                return i;

            if (i == -1 && j >= 0)
                return j;

            if (cache[i, j].HasValue)
                return cache[i, j].Value;

            int[] reccurent = new int[]
            {
                Lewenstein(s1, s2, i, j - 1, cache) + 1,
                Lewenstein(s1, s2, i - 1, j, cache) + 1,
                Lewenstein(s1, s2, i - 1 , j - 1, cache) + (s1[i] == s2[j] ? 0 : 1)
            };

            int result = reccurent.Min();
            cache[i, j] = result;
            return result;
        }
    }
}
