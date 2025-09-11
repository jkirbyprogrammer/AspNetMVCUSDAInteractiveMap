using CsvHelper;
using GeoJSON.Net.Feature;
using Newtonsoft.Json;
using System.Globalization;

namespace AspNetMVCUSDAInteractiveMap.Models
{
    public class USDAGeoJsonModel(string year, string filepath)
    {
        private readonly string _year = year;
        private readonly string _filePath = filepath;

        public string GetGeoJson()
        {
            using StreamReader countySr = new(_filePath);
            string jsonCountyResults = countySr.ReadToEnd();
            return jsonCountyResults;
        }


        public string GetRadarJsonData(int startYear = 2018)
        {
            List<RadarHelper> rh = [];
            string csvPath = _filePath;
            using (StreamReader csvReader = new(csvPath))
            using (CsvReader csv = new(csvReader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<CountyEmergencyDecs>().ToList();
                foreach (var dec in records.Select(x => x.DisasterDescription).Distinct().ToList())
                {
                    for (int i = startYear; i < DateTime.Now.Year; i++)
                    {
                        int totalDecs = records.Where(x => x.DisasterDescription == dec && x.DisasterYear == i.ToString())
                            .Select(x => x.DeclarationNumber).Distinct().Count();
                        if (totalDecs > 0)
                            rh.Add(new RadarHelper { Type = dec, Total = totalDecs, Year = i });
                    }
                }
            }

            var groupedList = GetGroupedTotals(rh).Where(x => x.Type != "drought, excessive heat" && x.Type != "other");

            string jsonString = JsonConvert.SerializeObject(groupedList, Formatting.Indented);
            return jsonString;
        }

        private static void SetRadarHelperDefaults(List<RadarHelper> rh, int startYear = 2018)
        {
            for (int i = startYear; i < DateTime.Now.Year; i++)
            {
                rh.Add(new RadarHelper { Type = "frost, freeze, snow, ice", Total = 0, Year = i });
                rh.Add(new RadarHelper { Type = "tornadoes, hail, flooding, storm, landslides", Total = 0, Year = i });
                rh.Add(new RadarHelper { Type = "hurricane", Total = 0, Year = i });
                rh.Add(new RadarHelper { Type = "wildfires", Total = 0, Year = i });
                rh.Add(new RadarHelper { Type = "drought, excessive heat", Total = 0, Year = i });
                rh.Add(new RadarHelper { Type = "other", Total = 0, Year = i });
            }
        }


        private static List<RadarHelper> GetGroupedTotals(List<RadarHelper> orginalList)
        {
            List<RadarHelper> rhGrouped = [];
            SetRadarHelperDefaults(rhGrouped);

            for (int i = 0; i < orginalList.Count; i++)
            {
                RadarHelper? r = orginalList[i];
                if (r.Type.Contains("frost", StringComparison.CurrentCultureIgnoreCase) || r.Type.Contains("freeze", StringComparison.CurrentCultureIgnoreCase)
                    || r.Type.Contains("snow", StringComparison.CurrentCultureIgnoreCase) || r.Type.Contains("ice", StringComparison.CurrentCultureIgnoreCase)
                    || r.Type.Contains("winter", StringComparison.CurrentCultureIgnoreCase) || r.Type.Contains("freezing", StringComparison.CurrentCultureIgnoreCase)
                    || r.Type.Contains("cold", StringComparison.CurrentCultureIgnoreCase))
                {
                    var rhToUpdate = rhGrouped.Where(x => x.Type.Equals("frost, freeze, snow, ice", StringComparison.CurrentCultureIgnoreCase) && x.Year == r.Year).FirstOrDefault();
                    if (rhToUpdate != null)
                        rhToUpdate.Total += r.Total;
                }
                else if (r.Type.Contains("hurricane", StringComparison.CurrentCultureIgnoreCase) || r.Type.Contains("cyclone", StringComparison.CurrentCultureIgnoreCase)
                    || r.Type.Contains("remnants of ida", StringComparison.CurrentCultureIgnoreCase) || r.Type.Contains("typhoon", StringComparison.CurrentCultureIgnoreCase))
                {
                    var rhToUpdate = rhGrouped.Where(x => x.Type.Equals("hurricane", StringComparison.CurrentCultureIgnoreCase) && x.Year == r.Year).FirstOrDefault();
                    if (rhToUpdate != null)
                        rhToUpdate.Total += r.Total;
                }
                else if (r.Type.Contains("fire", StringComparison.CurrentCultureIgnoreCase))
                {
                    var rhToUpdate = rhGrouped.Where(x => x.Type.Equals("wildfires", StringComparison.CurrentCultureIgnoreCase) && x.Year == r.Year).FirstOrDefault();
                    if (rhToUpdate != null)
                        rhToUpdate.Total += r.Total;
                }
                else if (r.Type.Contains("tornado", StringComparison.CurrentCultureIgnoreCase) || r.Type.Contains("hail", StringComparison.CurrentCultureIgnoreCase)
                    || r.Type.Contains("flooding", StringComparison.CurrentCultureIgnoreCase) || r.Type.Contains("storm", StringComparison.CurrentCultureIgnoreCase)
                    || r.Type.Contains("landslides", StringComparison.CurrentCultureIgnoreCase) || r.Type.Contains("precipitation", StringComparison.CurrentCultureIgnoreCase)
                    || r.Type.Contains("wind", StringComparison.CurrentCultureIgnoreCase) || r.Type.Contains("rain", StringComparison.CurrentCultureIgnoreCase)
                    || r.Type.Contains("flood", StringComparison.CurrentCultureIgnoreCase) || r.Type.Contains("wet", StringComparison.CurrentCultureIgnoreCase)
                    || r.Type.Contains("derecho", StringComparison.CurrentCultureIgnoreCase) || r.Type.Contains("moisture", StringComparison.CurrentCultureIgnoreCase)
                    || r.Type.Contains("landslide", StringComparison.CurrentCultureIgnoreCase) || r.Type.Contains("earthquake", StringComparison.CurrentCultureIgnoreCase))
                {
                    var rhToUpdate = rhGrouped.Where(x => x.Year == r.Year && x.Type.ToLower().Equals("tornadoes, hail, flooding, storm, landslides"
                        ,StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    if (rhToUpdate != null)
                        rhToUpdate.Total += r.Total;
                }
                else if (r.Type.Contains("drought", StringComparison.CurrentCultureIgnoreCase) || r.Type.Contains("heat", StringComparison.CurrentCultureIgnoreCase)
                    || r.Type.Contains("high temperature", StringComparison.CurrentCultureIgnoreCase))
                {
                    var rhToUpdate = rhGrouped.Where(x => x.Year == r.Year && x.Type.Equals("drought, excessive heat"
                        , StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    if (rhToUpdate != null)
                        rhToUpdate.Total += r.Total;
                }
                else
                {
                    var rhToUpdate = rhGrouped.Where(x => x.Year == r.Year && x.Type.Equals("other"
                        , StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    if (rhToUpdate != null)
                        rhToUpdate.Total += r.Total;
                }
            }
            return [.. rhGrouped.OrderBy(x => x.Type).ThenBy(x => x.Year)];

        }



        public string GetLineChartJsonData(string type, int startYear = 2018)
        {
            List<RadarHelper> rh = [];
            using (StreamReader csvReader = new(_filePath))
            using (CsvReader csv = new(csvReader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<CountyEmergencyDecs>().ToList();
                for (int i = startYear; i < DateTime.Now.Year; i++)
                {
                    int totalDecs = records.Where(x => x.DisasterYear == i.ToString()).Select(x => x.DeclarationNumber).Distinct().Count();
                    rh.Add(new RadarHelper { Type = type, Total = totalDecs, Year = i });
                }
            }

            string jsonString = JsonConvert.SerializeObject(rh, Formatting.Indented);
            return jsonString;
        }

        public string GetStateAppendGeoJson(string type)
        {
            string jsonResults = "";
            FeatureCollection statefeatureCollection = new();
            var stateFilePath = _filePath + "/lib/JsonData/StatesLayer.json";
            string csvPath = _filePath + ((type == "pres") ? "/lib/CSVFiles/CountyStatePresidentEmergencyDecs.csv"
                : "/lib/CSVFiles/CountyStateUsSecOfAgEmergency.csv");

            using (StreamReader r = new(stateFilePath))
            {
                using StreamReader csvReader = new(csvPath);
                using CsvReader csv = new(csvReader, CultureInfo.InvariantCulture);
                jsonResults = r.ReadToEnd();
                statefeatureCollection = JsonConvert.DeserializeObject<FeatureCollection>(jsonResults ?? "") ?? new();
                List<Feature> features = [.. statefeatureCollection.Features];
                var records = csv.GetRecords<CountyEmergencyDecs>().Where(x => x.DisasterYear == _year).ToList();

                foreach (var f in features)
                {
                    var countyCode = f;
                    var stateName = f.Properties.Where(x => x.Key == "name").Select(x => x.Value).FirstOrDefault();
                    var stateCode = f.Id;
                    int totalDisasters = records.Where(x => x.Fips[..2] == stateCode).Select(x => x.DeclarationNumber)
                        .Distinct().Count();
                    string disasterList = "";
                    if (totalDisasters > 0)
                    {
                        disasterList = String.Join(" | ", records.Where(x => x.Fips[..2] == stateCode)
                            .Select(x => x.DisasterDescription.Trim()).Distinct().ToList());
                    }

                    f.Properties.Add("TotalPresDecs", totalDisasters);
                    f.Properties.Add("DecsWithCrops", 0);
                    f.Properties.Add("ListOfDisasters", disasterList);
                }
            }

            string featureCollectionJson = JsonConvert.SerializeObject(statefeatureCollection);

            return featureCollectionJson;
        }

        public string GetCountyAppendGeoJsonAllCropData(string type)
        {
            string jsonCountyResults = "";
            string jsonStateResults = "";
            var stateFilePath = _filePath + "/lib/JsonData/StatesLayer.json";
            FeatureCollection stateseatureCollection = new();
            var countyFilePath = _filePath + "/lib/JsonData/CountyData.json";
            FeatureCollection countyfeatureCollection = new();
            string csvDecPath = _filePath + ((type == "pres") ? "/lib/CSVFiles/CountyStatePresidentEmergencyDecs.csv"
                : "/lib/CSVFiles/CountyStateUsSecOfAgEmergency.csv");
            string csvCropPath = _filePath + "/lib/CSVFiles/2022CountyCropData.csv";

            using (StreamReader cr = new(countyFilePath))
            {
                using StreamReader sr = new(stateFilePath);
                using StreamReader csvReaderDecs = new(csvDecPath);
                using var csvDecs = new CsvReader(csvReaderDecs, CultureInfo.InvariantCulture);
                using var csvReaderCrops = new StreamReader(csvCropPath);
                using var csvCrops = new CsvReader(csvReaderCrops, CultureInfo.InvariantCulture);
                jsonCountyResults = cr.ReadToEnd();
                jsonStateResults = sr.ReadToEnd();
                countyfeatureCollection = JsonConvert.DeserializeObject<FeatureCollection>(jsonCountyResults ?? "") ?? new();
                stateseatureCollection = JsonConvert.DeserializeObject<FeatureCollection>(jsonStateResults ?? "") ?? new();
                List<Feature> countyFeatures = countyfeatureCollection.Features;
                List<Feature> stateFeatures = stateseatureCollection.Features;
                var decRecords = csvDecs.GetRecords<CountyEmergencyDecs>().Where(x => x.DisasterYear == _year).ToList();
                var cropRecords = csvCrops.GetRecords<CountyEmergencyDecsCrops>().ToList();

                foreach (var cf in countyFeatures)
                {
                    var featureStateFips = cf.Properties.FirstOrDefault(x => x.Key == "STATEFP").Value.ToString();
                    var featureCountyFips = cf.Properties.FirstOrDefault(x => x.Key == "COUNTYFP").Value.ToString();
                    var featureFipsCode = featureStateFips + featureCountyFips;
                    int totalDisasters = decRecords.Where(x => x.Fips == featureFipsCode).Select(x => x.DeclarationNumber).Distinct().Count();
                    int totalCrops = cropRecords.Where(x => x.Fips == featureFipsCode).Select(x => x.DataItem).Distinct().Count();
                    string disasterList = "";
                    string cropDetailList = "";

                    var stateName = stateFeatures?.FirstOrDefault(x => x.Id == featureStateFips)?.Properties
                        .FirstOrDefault(x => x.Key == "name").Value
                        .ToString();

                    if (totalDisasters > 0)
                    {
                        disasterList = String.Join(" | ", decRecords.Where(x => x.Fips == featureFipsCode)
                            .Select(x => x.DisasterDescription.Trim()).Distinct().ToList());
                    }

                    if (totalCrops > 0)
                    {
                        cropDetailList = String.Join(" | ", cropRecords.Where(x => x.Fips == featureFipsCode)
                            .Select(x => x.DataItem + ": " + Convert.ToInt32(x.Value).ToString("N0")).Distinct().ToList());
                    }

                    string countyPlaceName = cf.Properties.FirstOrDefault(x => x.Key == "NAME").Value.ToString() ?? "";

                    cf.Properties.Add("name", countyPlaceName + (!String.IsNullOrEmpty(stateName) ? ", " + stateName : ""));
                    cf.Properties.Add("TotalPresDecs", totalDisasters);
                    cf.Properties.Add("DecsWithCrops", totalCrops);
                    cf.Properties.Add("ListOfDisasters", disasterList);
                    cf.Properties.Add("CropDetailList", cropDetailList);
                }

            }

            string featureCollectionJson = JsonConvert.SerializeObject(countyfeatureCollection);

            return featureCollectionJson;
        }

    }
}
