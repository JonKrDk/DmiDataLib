namespace DmiDataLib.Data
{
    public class Station
    {
        public string Id { get; set; }
        public GpsLocation Location { get; set; }
        public double? BarometerHeight { get; set; }
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public DateTime OperationFrom { get; set; }
        public DateTime? OperationTo { get; set; }
        public string Owner { get; set; }
        public List<string> ParameterId { get; set; }
        public string RegionId { get; set; }
        public double? StationHeight { get; set; }
        public string StationId { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string WmoCountryCode { get; set; }
        public string WmoStationId { get; set; }
    }
}
