namespace AspNetMVCUSDAInteractiveMap.Models
{
    public class FunctionHelper
    {

        public static string GetCountyJsonFileName(string year, string type)
        {
            if (type == "pres")
                return year + "CountyPresLayer.json";
            else
                return year + "CountyUsSecLayer.json";
        }

        public static string GetStateJsonFileName(string year, string type)
        {
            if (type == "pres")
                return year + "StatePresLayer.json";
            else
                return year + "StateUsSecLayer.json";
        }


        public static string GetRadarJsonFileName(string type)
        {
            if (type == "pres")
                return "PresRadarChartData.json";
            else
                return "UsSecRadarChartData.json";
        }



    }
}
