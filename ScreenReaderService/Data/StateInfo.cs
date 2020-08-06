namespace ScreenReaderService.Data
{
    public class StateInfo
    {
        public bool Paused { get; set; }
        public int IdCounter { get; set; }
        public Order DiscoveredOrder { get; set; }
    }
}