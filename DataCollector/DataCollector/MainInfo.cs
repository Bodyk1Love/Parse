using System;
using System.Collections.Generic;

namespace DataCollector
{
    public class MainInfo
    {
        public string bandName, location, month, day, year, link;

        public MainInfo(string bandName, string location, string month,
                                    string day, string year, string link)
        {
            this.bandName = bandName;
            this.location = location;
            this.month = month;
            this.day = day;
            this.year = year;
            this.link = link;
        }

        public string[] GetFields()
        {
            string[] data = { bandName, location, month, day, year };
            return data;
        }
    }
}

