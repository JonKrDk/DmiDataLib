namespace DmiDataLib.Data
{
    /// <summary>
    /// An Observation contains a single measurement at a specific timestamp
    /// </summary>
    public class Observation
    {
        public DateTime Created { get; set; }
        public DateTime Observed { get; set; }
        public double Value { get; set; }
    }
}
