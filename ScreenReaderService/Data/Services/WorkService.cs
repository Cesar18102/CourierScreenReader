using System.IO;

using Android.OS;

namespace ScreenReaderService.Data.Services
{
    public class WorkService : ObjectFileMappingService<WorkInfo>
    {
        private const string WORK_INFO_FILE_NAME = "work.json";

        private static readonly string WORK_INFO_PATH = Path.Combine(
            Environment.ExternalStorageDirectory.AbsolutePath, "CourierBot"
        );

        public WorkService() : base(WORK_INFO_PATH, WORK_INFO_FILE_NAME) { }

        public WorkInfo WorkInfo
        {
            get => Data;
            set => Data = value;
        }
    }
}