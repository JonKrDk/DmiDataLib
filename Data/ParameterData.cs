namespace DmiDataLib.Data
{
    public class ParameterData
    {
        public string Name { get; set; }
        public GpsLocation Location { get; set; }
        public SortedList<DateTime, Observation> ObservationData { get; set; } = new SortedList<DateTime, Observation>();
    }
}
