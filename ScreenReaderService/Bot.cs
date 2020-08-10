using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Autofac;

using ScreenReaderService.Data;
using ScreenReaderService.Telegram;
using ScreenReaderService.Services;

namespace ScreenReaderService
{
    public class Bot 
    {
        private const int DEFAULT_DELAY = 1000;

        private const int ITERATIONS_FOR_TELEGRAM_UPDATE = 1;
        private const int ITERATIONS_FOR_AUTH_CHECK = 60 * 30;

        private BotService BotService = DependencyHolder.Dependencies.Resolve<BotService>();
        private TelegramNotifier Notifier = DependencyHolder.Dependencies.Resolve<TelegramNotifier>();

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

        public async Task CheckAuth()
        {
            await DependencyHolder.Dependencies.Resolve<AuthService>().LogIn(
                BotService.CredentialsService.Credentials.Login,
                BotService.CredentialsService.Credentials.Password
            );
        }

        private async Task HandleTelegramUpdates()
        {
            IEnumerable<Command<string>> commands = await Notifier.GetUpdates(
                BotService.CredentialsService.Credentials.TelegramUsername
            );

            foreach (Command<string> command in commands)
                await ProcessCommand(command);
        }

        private async Task ProcessCommand(Command<string> command)
        {
            switch (command.CommandType)
            {
                case CommandType.PAUSE: 
                    BotService.StateService.StateInfo.Paused = true;
                    BotService.StateService.Save();
                    break;
                case CommandType.RESUME:
                    BotService.StateService.StateInfo.Paused = false;
                    BotService.StateService.Save();
                    break;

                case CommandType.GET_FILTERS_INFO: 
                    await Notifier.NotifyMessage(BotService.FilterService.Filters.ToString()); 
                    break;
                case CommandType.GET_CONSTRAINTS_INFO: 
                    await Notifier.NotifyMessage(BotService.ConstraintsService.Constraints.ToString()); 
                    break;

                case CommandType.GET_ACTIVE_LIST:
                    string activeOrders = string.Join('\n', BotService.WorkService.WorkInfo.ActiveOrders);
                    await Notifier.NotifyMessage(string.IsNullOrEmpty(activeOrders) ? "No active orders" : activeOrders);
                    break;
                case CommandType.GET_DAY_HISTORY:
                    string doneOrders = string.Join('\n', BotService.WorkService.WorkInfo.DoneOrders);
                    await Notifier.NotifyMessage(string.IsNullOrEmpty(doneOrders) ? "No done orders" : doneOrders);
                    break;

                case CommandType.CLEAR_DAY_HISTORY:
                    if (BotService.WorkService.WorkInfo.DoneOrders.Count == 0)
                        await Notifier.NotifyMessage("No history to be cleared");
                    else
                    {
                        BotService.WorkService.WorkInfo.DoneOrders.Clear();
                        BotService.WorkService.Save();
                        await Notifier.NotifyMessage("History cleared");
                    }
                    break;

                case CommandType.DELIVERED: await MarkDelivered(command.Data); break;
            }
        }

        private async Task MarkDelivered(string id)
        {
            Order delivered = BotService.WorkService.WorkInfo.ActiveOrders.FirstOrDefault(order => order.Id.ToString() == id);

            if (delivered == null)
            {
                await Notifier.NotifyMessage(
                    $"@{BotService.CredentialsService.Credentials.TelegramUsername}, order with ID = {id} not found;"
                );
            }
            else
            {
                BotService.WorkService.WorkInfo.ActiveOrders.Remove(delivered);
                BotService.WorkService.WorkInfo.DoneOrders.Add(delivered);

                BotService.WorkService.Save();

                await Notifier.NotifyMessage(
                    $"@{BotService.CredentialsService.Credentials.TelegramUsername}, order with ID = {id} marked as delivered.\n" +
                    $"Now you've {BotService.WorkService.WorkInfo.ActiveOrders.Count}/{BotService.ConstraintsService.Constraints.MaxActiveOrdersAtTime} active orders.\n" +
                    $"Today you've delivered {BotService.WorkService.WorkInfo.DoneOrders.Count}/{BotService.ConstraintsService.Constraints.MaxOrdersPerDay} orders."
                );
            }
        }
    }
}