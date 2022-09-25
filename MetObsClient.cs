using DmiDataLib.Data;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace DmiDataLib
{
    /// <summary>
    /// Read data from DMI
    /// For more information see:
    /// https://confluence.govcloud.dk/display/FDAPI/Danish+Meteorological+Institute+-+Open+Data
    /// </summary>
    public class MetObsClient
    {
        private readonly string _apikey;
        private const string _baseUrl = "https://dmigw.govcloud.dk/v2/metObs";
        private bool _connected;
        private HttpClient _httpClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="apikey">API Key from DMI</param>
        public MetObsClient(string apikey)
        {
            this._apikey = apikey;
            this._connected = false;
        }

        /// <summary>
        /// Establish connection to the server
        /// </summary>
        private void Connect()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _connected = true;
        }

        /// <summary>
        /// Get all Stations
        /// </summary>
        /// <returns>A list of Stations</returns>
        public List<Station> GetStations()
        {
            if (!_connected) { Connect(); }

            string request = $"{_baseUrl}/collections/station/items?api-key={_apikey}";
            var httpResult = _httpClient.GetStringAsync(request).Result;
            StationDto.Root root= JsonConvert.DeserializeObject<StationDto.Root>(httpResult);

            List<Station> result = new List<Station>();
            foreach (StationDto.Feature feature in root.features)
            {
                result.Add(new Station()
                {
                    Id = feature.id,
                    Location = new GpsLocation()
                    {
                        Latitude =  feature.geometry.coordinates[0],
                        Longitude = feature.geometry.coordinates[1]
                    },
                    BarometerHeight = feature.properties.barometerHeight,
                    Country = feature.properties.country,
                    Created = feature.properties.created.ToLocalTime(),
                    Name = feature.properties.name,
                    OperationFrom = feature.properties.operationFrom,
                    OperationTo = feature.properties.operationTo,
                    Owner = feature.properties.owner,
                    ParameterId = feature.properties.parameterId,
                    RegionId = feature.properties.regionId,
                    StationHeight = feature.properties.stationHeight,
                    StationId = feature.properties.stationId,
                    Status = feature.properties.status,
                    Type = feature.properties.type,
                    ValidFrom = feature.properties.validFrom,
                    ValidTo = feature.properties.validTo,
                    WmoCountryCode = feature.properties.wmoCountryCode,
                    WmoStationId = feature.properties.wmoStationId
                });
            }

            return result;
        }

        /// <summary>
        /// Requests observations satisfying the search parameters
        /// </summary>
        /// <param name="limit">Specify a maximum number of observations you want to be returned, default is 1000</param>
        /// <param name="offset">Specify the number of observations that should be skipped before returning matching observations.</param>
        /// <param name="stationId">Narrow the search to a specific station ID</param>
        /// <param name="fromDateTime">Narrow the search to a date range or a specific date.</param>
        /// <param name="toDateTime">Narrow the search to a date range or a specific date.</param>
        /// <param name="period">Narrow the search to a predetermined date range.</param>
        /// <param name="parameterId">Narrow the search to a specific parameter id</param>
        /// <param name="status">Narrow the search to only allow stations having a specific status</param>
        /// <param name="stationType">Narrow the search to only allow stations having a specific type</param>
        /// <returns>A sorted list of Stations, each having a sorted list of parameters, and each parameter have a timebase sorted list of values</returns>
        public SortedList<string, StationData> GetObservations(
            int? limit = null, 
            int? offset = null, 
            string stationId = null, 
            DateTime? fromDateTime = null, 
            DateTime? toDateTime = null, 
            PeriodEnum? period = null, 
            string parameterId = null,
            string status = null,
            string stationType = null)
        {
            if (!_connected) { Connect(); }

            string request = $"{_baseUrl}/collections/observation/items?api-key={_apikey}";

            if (limit != null) { request += $"&limit={limit}"; }
            if (offset != null) { request += $"&offset={offset}"; }
            if (stationId != null) { request += $"&stationId={stationId}"; }
            string fromDateTimeStr = fromDateTime == null ? ".." : ((DateTime)fromDateTime).ToString("yyyy-MM-ddTHH:mm:sszzz").Replace("+", "%2B");
            string toDateTimeStr = toDateTime == null ? ".." : ((DateTime)toDateTime).ToString("yyyy-MM-ddTHH:mm:sszzz").Replace("+", "%2B");
            if (fromDateTime != null || toDateTime != null) { request += $"&datetime={fromDateTimeStr}/{toDateTimeStr}"; }

            switch (period)
            {
                case PeriodEnum.Latest:
                    request += $"&period=latest";
                    break;
                case PeriodEnum.Latest10Minutes:
                    request += $"&period=latest-10-minutes";
                    break;
                case PeriodEnum.LatestHour:
                    request += $"&period=latest-hour";
                    break;
                case PeriodEnum.LatestDay:
                    request += $"&period=latest-day";
                    break;
                case PeriodEnum.LatestWeek:
                    request += $"&period=latest-week";
                    break;
                case PeriodEnum.LatestMonth:
                    request += $"&period=latest-month";
                    break;
            }

            if (parameterId != null) { request += $"&parameterId={parameterId}"; }
            if (status != null) { request += $"&status={status}"; }
            if (stationType!= null) { request += $"&type={stationType}"; }

            var response = _httpClient.GetStringAsync(request).Result;
            ObservationDto.Root root = JsonConvert.DeserializeObject<ObservationDto.Root>(response);

            SortedList<string, StationData> stationDataList = new SortedList<string, StationData>();

            foreach (ObservationDto.Feature feature in root.features)
            {
                StationData stationData;
                if (stationDataList.ContainsKey(feature.properties.stationId))
                {
                    stationData = stationDataList[feature.properties.stationId];
                }
                else
                {
                    stationData = new StationData();
                    stationData.StationId = feature.properties.stationId;
                    stationDataList.Add(feature.properties.stationId, stationData);
                }

                ParameterData parameterData;
                if (stationData.Parameters.ContainsKey(feature.properties.parameterId))
                {
                    parameterData = stationData.Parameters[feature.properties.parameterId];
                }
                else
                {
                    parameterData = new ParameterData();
                    parameterData.Name = feature.properties.parameterId;
                    parameterData.Location = new GpsLocation()
                    {
                        Latitude = feature.geometry.coordinates[0],
                        Longitude = feature.geometry.coordinates[1]
                    };
                    stationData.Parameters.Add(feature.properties.parameterId, parameterData);
                }

                Observation observation;
                if (parameterData.ObservationData.ContainsKey(feature.properties.observed.ToLocalTime()))
                {
                    observation = parameterData.ObservationData[feature.properties.observed.ToLocalTime()];
                    Debug.WriteLine($"Warning : Duplicate data for Station=[{stationData.StationId}], Parameter=[{parameterData.Name}], Time=[{observation.Observed}], Value=[{observation.Value}]");
                }
                else
                {
                    observation = new Observation();
                    observation.Created = feature.properties.created.ToLocalTime();
                    observation.Observed = feature.properties.observed.ToLocalTime();
                    observation.Value = feature.properties.value;
                    parameterData.ObservationData.Add(feature.properties.observed.ToLocalTime(), observation);
                }
            }

            return stationDataList;
        }
    }
}
