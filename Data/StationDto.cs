using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DmiDataLib.Data
{
    public class StationDto
    {
        public class Feature
        {
            public Geometry geometry { get; set; }
            public string id { get; set; }
            public string type { get; set; }
            public Properties properties { get; set; }
        }

        public class Geometry
        {
            public List<double> coordinates { get; set; }
            public string type { get; set; }
        }

        public class Link
        {
            public string href { get; set; }
            public string rel { get; set; }
            public string type { get; set; }
            public string title { get; set; }
        }

        public class Properties
        {
            public double? barometerHeight { get; set; }
            public string country { get; set; }
            public DateTime created { get; set; }
            public string name { get; set; }
            public DateTime operationFrom { get; set; }
            public DateTime? operationTo { get; set; }
            public string owner { get; set; }
            public List<string> parameterId { get; set; }
            public string regionId { get; set; }
            public double? stationHeight { get; set; }
            public string stationId { get; set; }
            public string status { get; set; }
            public string type { get; set; }
            public object updated { get; set; }
            public DateTime validFrom { get; set; }
            public DateTime? validTo { get; set; }
            public string wmoCountryCode { get; set; }
            public string wmoStationId { get; set; }
        }

        public class Root
        {
            public string type { get; set; }
            public List<Feature> features { get; set; }
            public DateTime timeStamp { get; set; }
            public int numberReturned { get; set; }
            public List<Link> links { get; set; }
        }
    }
}
