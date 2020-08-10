using Android.OS;
using System.IO;

namespace ScreenReaderService.Data.Services
{
    public class SessionService : ObjectFileMappingService<Session>
    {
        private const string SESSION_INFO_FILE_NAME = "session.json";

        private static readonly string SESSION_INFO_PATH = Path.Combine(
            Environment.ExternalStorageDirectory.AbsolutePath, "CourierBot"
        );

        public SessionService() : base(SESSION_INFO_PATH, SESSION_INFO_FILE_NAME) { }

        public Session Session
        {
            get => Data;
            set => Data = value;
        }
    }
}