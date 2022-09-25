namespace DmiDataLib.Data
{
    public class StationData
    {
        public string StationId { get; set; }
        public SortedList<string, ParameterData> Parameters { get; set; } = new SortedList<string, ParameterData>();
    }
}
