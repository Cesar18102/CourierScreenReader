namespace ScreenReaderService.Telegram
{
    public enum CommandType
    {
        PAUSE,
        RESUME,
        DELIVERED,
        GET_FILTERS_INFO,
        GET_CONSTRAINTS_INFO,
        GET_ACTIVE_LIST,
        GET_DAY_HISTORY,
        CLEAR_DAY_HISTORY
    }
}
