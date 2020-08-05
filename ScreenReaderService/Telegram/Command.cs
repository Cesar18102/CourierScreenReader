namespace ScreenReaderService.Telegram
{
    public class Command<TData>
    {
        public CommandType CommandType { get; private set; }
        public TData Data { get; private set; }

        public Command(CommandType type, TData data)
        {
            CommandType = type;
            Data = data;
        }
    }
}
