using System.IO;

using Android.OS;

namespace ScreenReaderService.Data.Services
{
    public class ConstraintsConfigService : ObjectFileMappingService<ConstraintsInfo>
    {
        private const string CONSTRAINTS_FILE_NAME = "constraints.json";

        private static readonly string CONSTRAINTS_CONFIG_PATH = Path.Combine(
            Environment.ExternalStorageDirectory.AbsolutePath, "CourierBot"
        );

        public ConstraintsConfigService() : base(CONSTRAINTS_CONFIG_PATH, CONSTRAINTS_FILE_NAME) { }

        public ConstraintsInfo Constraints
        {
            get => Data;
            set => Data = value;
        }
    }
}
