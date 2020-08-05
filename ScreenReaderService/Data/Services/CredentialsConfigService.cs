using System.IO;

using Android.OS;

namespace ScreenReaderService.Data.Services
{
    public class CredentialsConfigService : ObjectFileMappingService<CredentialsInfo>
    {
        private const string CREDENTIALS_CONFIG_FILE_NAME = "credentials.json";

        private static readonly string CREDENTIALS_CONFIG_PATH = Path.Combine(
            Environment.ExternalStorageDirectory.AbsolutePath, "CourierBot"
        );

        public CredentialsConfigService() : base(CREDENTIALS_CONFIG_PATH, CREDENTIALS_CONFIG_FILE_NAME) { }

        public CredentialsInfo Credentials
        {
            get => Data;
            set => Data = value;
        }
    }
}
