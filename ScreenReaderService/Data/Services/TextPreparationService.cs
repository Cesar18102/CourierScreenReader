using System.IO;

using Android.OS;

namespace ScreenReaderService.Data.Services
{
    public class TextPreparationService : ObjectFileMappingService<TextPreparationInfo>
    {
        private const string TEXT_PREPARATION_INFO_FILE_NAME = "text_preparation.json";

        private static readonly string TEXT_PREPARATION_INFO_PATH = Path.Combine(
            Environment.ExternalStorageDirectory.AbsolutePath, "CourierBot"
        );

        public TextPreparationService() : base(TEXT_PREPARATION_INFO_PATH, TEXT_PREPARATION_INFO_FILE_NAME) { }

        public TextPreparationInfo PreparationInfo
        {
            get => Data;
            set => Data = value;
        }
    }
}