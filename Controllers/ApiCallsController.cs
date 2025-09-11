using AspNetMVCUSDAInteractiveMap.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetMVCUSDAInteractiveMap.Controllers
{
    public class ApiCallsController(IWebHostEnvironment hostingEnvironment) : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment = hostingEnvironment;
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("/api/GetCountyGeoJson")]
        public IActionResult GetCountyGeoJson(string year = "2025", string type = "pres")
        {
            var countyFilePath = _hostingEnvironment.WebRootPath + "/lib/JsonData/" + FunctionHelper.GetCountyJsonFileName(year, type);
            USDAGeoJsonModel uSDAGeoJsonModel = new(year, countyFilePath);
            string jsonCountyResults = uSDAGeoJsonModel.GetGeoJson();

            return Ok(jsonCountyResults);
        }

        [HttpGet]
        [Route("/api/GetStateGeoJson")]
        public IActionResult GetStateGeoJson(string year = "2025", string type = "pres")
        {
            var stateFilePath = _hostingEnvironment.WebRootPath + "/lib/JsonData/" + FunctionHelper.GetStateJsonFileName(year, type);
            USDAGeoJsonModel uSDAGeoJsonModel = new(year, stateFilePath);
            string jsonStateResults = uSDAGeoJsonModel.GetGeoJson();

            return Ok(jsonStateResults);
        }


        [HttpGet]
        [Route("/api/GetRadarData")]
        public IActionResult GetRadarData(string type = "pres")
        {
            var stateFilePath = _hostingEnvironment.WebRootPath + "/lib/JsonData/" + FunctionHelper.GetRadarJsonFileName(type);
            USDAGeoJsonModel uSDAGeoJsonModel = new("", stateFilePath);
            string jsonStateResults = uSDAGeoJsonModel.GetGeoJson();

            return Ok(jsonStateResults);
        }


        [HttpGet]
        [Route("/api/GetLineChartData")]
        public IActionResult GetLineChartData()
        {
            var stateFilePath = _hostingEnvironment.WebRootPath + "/lib/JsonData/LineChartData.json";
            USDAGeoJsonModel uSDAGeoJsonModel = new("", stateFilePath);
            string jsonStateResults = uSDAGeoJsonModel.GetGeoJson();

            return Ok(jsonStateResults);
        }

        [HttpGet]
        [Route("/api/GetRadarDataFromFiles")]
        public IActionResult GetRadarDataFromFiles(string type = "pres")
        {
            string csvPath = _hostingEnvironment.WebRootPath + ((type == "pres") ? "/lib/CSVFiles/CountyStatePresidentEmergencyDecs.csv"
                : "/lib/CSVFiles/CountyStateUsSecOfAgEmergency.csv");
            USDAGeoJsonModel uSDAGeoJsonModel = new("", csvPath);
            string jsonRadarData = uSDAGeoJsonModel.GetRadarJsonData();

            return Ok(jsonRadarData);
        }

        [HttpGet]
        [Route("/api/GetLineChartDataFromFiles")]
        public IActionResult GetLineChartDataFromFiles(string type = "pres")
        {
            string csvPath = _hostingEnvironment.WebRootPath + ((type == "pres") ? "/lib/CSVFiles/CountyStatePresidentEmergencyDecs.csv"
                : "/lib/CSVFiles/CountyStateUsSecOfAgEmergency.csv");
            USDAGeoJsonModel uSDAGeoJsonModel = new("", csvPath);
            var lineChartJsonString = uSDAGeoJsonModel.GetLineChartJsonData(type);

            return Ok(lineChartJsonString);
        }

        [HttpGet]
        [Route("/api/GetStateAppendGeoJsonFromFiles")]
        public IActionResult GetStateAppendGeoJsonFromFiles(string year = "2025", string type = "pres")
        {
            USDAGeoJsonModel uSDAGeoJsonModel = new(year, _hostingEnvironment.WebRootPath);
            string featureCollectionJson = uSDAGeoJsonModel.GetStateAppendGeoJson(type);

            return Ok(featureCollectionJson);
        }

        [HttpGet]
        [Route("/api/GetCountyAppendGeoJsonFromFiles")]
        public IActionResult GetCountyAppendGeoJsonFromFiles(string year, string type)
        {
            USDAGeoJsonModel uSDAGeoJsonModel = new(year, _hostingEnvironment.WebRootPath);
            string featureCollectionJson = uSDAGeoJsonModel.GetCountyAppendGeoJsonAllCropData(type);

            return Ok(featureCollectionJson);
        }

    }
}
