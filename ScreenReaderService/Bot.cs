using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Autofac;

using ScreenReaderService.Data;
using ScreenReaderService.Telegram;
using ScreenReaderService.Services;
using ScreenReaderService.Data.Services;

namespace ScreenReaderService
{
    public class Bot 
    {
        private const int DEFAULT_DELAY = 1000;

        private const int ITERATIONS_FOR_TELEGRAM_UPDATE = 1;
        private const int ITERATIONS_FOR_AUTH_CHECK = 60 * 30;

        private TelegramNotifier Notifier = DependencyHolder.Dependencies.Resolve<TelegramNotifier>();

        private BotInfo BotInfo = DependencyHolder.Dependencies.Resolve<BotInfo>();
        private ConstraintsConfigService ConstraintsConfigService = DependencyHolder.Dependencies.Resolve<ConstraintsConfigService>();
        private CredentialsConfigService CredentialsConfigService = DependencyHolder.Dependencies.Resolve<CredentialsConfigService>();

        public async void Start()
        {
            for (int i = 0; ; ++i)
            {
                await Task.Delay(DEFAULT_DELAY);

                if (i % ITERATIONS_FOR_TELEGRAM_UPDATE == 0)
                    await HandleTelegramUpdates();

                if (i % ITERATIONS_FOR_AUTH_CHECK == 0)
                    await CheckAuth();
            }
        }

        private async Task CheckAuth()
        {
            await DependencyHolder.Dependencies.Resolve<AuthService>().LogIn(
                CredentialsConfigService.Credentials.Login,
                CredentialsConfigService.Credentials.Password
            );
        }

        private async Task HandleTelegramUpdates()
        {
            IEnumerable<Command<string>> commands = await Notifier.GetUpdates(
                    CredentialsConfigService.Credentials.TelegramUsername
                );

            foreach (Command<string> command in commands)
                await ProcessCommand(command);
        }

        private async Task ProcessCommand(Command<string> command)
        {
            switch (command.CommandType)
            {
                case CommandType.PAUSE: BotInfo.Paused = true; break;
                case CommandType.RESUME: BotInfo.Paused = false; break;
                case CommandType.DELIVERED: await MarkDelivered(command.Data); break;
            }
        }

        private async Task MarkDelivered(string id)
        {
            Order delivered = BotInfo.ActiveOrders.FirstOrDefault(order => order.Id.ToString() == id);

            if (delivered == null)
            {
                await Notifier.NotifyMessage(
                    $"@{CredentialsConfigService.Credentials.TelegramUsername}, order with ID = {id} not found;"
                );
            }
            else
            {
                BotInfo.ActiveOrders.Remove(delivered);
                BotInfo.DoneOrders.Add(delivered);

                await Notifier.NotifyMessage(
                    $"@{CredentialsConfigService.Credentials.TelegramUsername}, order with ID = {id} marked as delivered.\n" +
                    $"Now you've {BotInfo.ActiveOrders.Count}/{ConstraintsConfigService.Constraints.MaxActiveOrdersAtTime} active orders.\n" +
                    $"Today you've delivered {BotInfo.DoneOrders.Count}/{ConstraintsConfigService.Constraints.MaxOrdersPerDay} orders."
                );
            }
        }
    }
}