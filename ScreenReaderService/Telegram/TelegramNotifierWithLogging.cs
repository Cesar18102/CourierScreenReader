using System.Threading.Tasks;

using Autofac;

using ScreenReaderService.Data.Services;

namespace ScreenReaderService.Telegram
{
    public class TelegramNotifierWithLogging : TelegramNotifier
    {
        private const string LOG_CHAT_ID = "-1001477295571";
        private CredentialsConfigService CredentialsService = DependencyHolder.Dependencies.Resolve<CredentialsConfigService>();

        private const string TRASH_PREFIX_SCANNING_TIME = "Scanning took";
        private const string TRASH_PREFIX_SCANNING_SKIP = "Scanning skipped due to";

        private bool IsImortantMessage(string message)
        {
            return !message.StartsWith(TRASH_PREFIX_SCANNING_SKIP) && !message.StartsWith(TRASH_PREFIX_SCANNING_TIME);
        }

        public override async Task NotifyMessage(string message)
        {
            await base.NotifyMessage(message);

            if (IsImortantMessage(message))
                await NotifySplitMessage($"FROM {CredentialsService.Credentials.TelegramUsername}\n\n*********\n\n{message}", LOG_CHAT_ID);
        }
    }
}