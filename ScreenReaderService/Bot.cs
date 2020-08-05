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
        private const int UPDATE_DELAY = 500;
        private TelegramNotifier Notifier = DependencyHolder.Dependencies.Resolve<TelegramNotifier>();

        private BotInfo BotInfo = DependencyHolder.Dependencies.Resolve<BotInfo>();

        public async void Start()
        {
            await DependencyHolder.Dependencies.Resolve<AuthService>().LogIn(BotInfo.Login, BotInfo.Password);
            await StartHandlingTelegramUpdates();
        }

        private async Task StartHandlingTelegramUpdates()
        {
            while (true)
            {
                await Task.Delay(UPDATE_DELAY);

                IEnumerable<Command<string>> commands = await Notifier.GetUpdates(BotInfo.Username);

                foreach (Command<string> command in commands)
                    await ProcessCommand(command);
            }
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
                await Notifier.NotifyMessage(
                    $"@{BotInfo.Username}, order with ID = {id} not found;"
                );
            else
            {
                BotInfo.ActiveOrders.Remove(delivered);
                BotInfo.DoneOrders.Add(delivered);
            }
        }
    }
}