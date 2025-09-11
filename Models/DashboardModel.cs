namespace AspNetMVCUSDAInteractiveMap.Models
{
    public class DashboardModel
    {
        public string Year { get; set; }
        public string Type { get; set; }
        public int Interval { get; set; }

        public DashboardModel(string year, string type)
        {
            Year = year;
            Type = type;
            Interval = GetInterval();
        }

        private int GetInterval()
        {
            if (Year == "2025")
                if (Type == "pres")
                    return 1;
                else
                    return 4;
            else if (Year == "2024")
                if (Type == "pres")
                    return 3;
                else
                    return 8;           
            else if (Year == "2023")
                if (Type == "pres")
                    return 3;
                else
                    return 6;
            else
                if (Type == "pres")
                    return 2;
                else
                    return 6;

        }

    }
}
