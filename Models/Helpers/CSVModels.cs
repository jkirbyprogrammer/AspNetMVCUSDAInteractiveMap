
namespace AspNetMVCUSDAInteractiveMap.Models
{
    public class CountyEmergencyDecs
    {

        public string Fips { get; set; } = "";
        public string County { get; set; } = "";
        public string State { get; set; } = "";
        public string DesignationCode { get; set; } = "";
        public string DeclarationNumber { get; set; } = "";
        public string DisasterDescription { get; set; } = "";
        public string DisasterYear { get; set; } = "";

    }

    public class CountyEmergencyDecsCrops
    {

        public string Year { get; set; } = "";
        public string State { get; set; } = "";
        public string AgDistrict { get; set; } = "";
        public string AgDistrictCode { get; set; } = "";
        public string Fips { get; set; } = "";
        public string Commodity { get; set; } = "";
        public string DataItem { get; set; } = "";
        public string Value { get; set; } = "";
        public string CVPercent { get; set; } = "";

    }


    public class RadarHelper
    {
        public string Type { get; set; } = "";
        public int Year { get; set; } = 0;
        public int Total { get; set; } = 0;
    }


}
