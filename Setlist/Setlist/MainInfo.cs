using System;

namespace Setlist
{
    public class MainInfo
    {
        /* Class describes main page objects
         * such as Popular Setlists, Upcoming Events
         * and Recent Edits. It keeps info about
         * artist/band name, location of event with
         * the date, all date fiealds (month ,day, year)
         * and a link on the setlist.
         * 
         * ParseMainPage() function get all data from
         * main page and make a list of this class objects.
         * There always must be 30 objects (10 Popular Setlists,
         * 10 Upcoming Events and 10 Recent Edits) in one list.
         *         
         * this.GetFields() function returns all printable
         * data in a string array.
         */

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


