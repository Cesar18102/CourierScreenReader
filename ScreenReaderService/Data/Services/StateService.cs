using System.IO;

using Android.OS;

namespace ScreenReaderService.Data.Services
{
    public class StateService : ObjectFileMappingService<StateInfo>
    {
        private const string STATE_INFO_FILE_NAME = "state.json";

        private static readonly string STATE_INFO_PATH = Path.Combine(
            Environment.ExternalStorageDirectory.AbsolutePath, "CourierBot"
        );

        public StateService() : base(STATE_INFO_PATH, STATE_INFO_FILE_NAME) { }

        public StateInfo StateInfo
        {
            get => Data;
            set => Data = value;
        }
    }
}