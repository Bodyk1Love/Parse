using System;
namespace Setlist
{
    public class TopSetlistInfo
    {
        /* Class describes top setlists page object.
         * It keeps info about artist/band name,
         * location of an event and a link on a
         * setlist.        
         * 
         * ParseTopSetlistsPage() function get 
         * all data from top setlists page and make 
         * an object of this class.
         * 
         * this.GetFields() function returns all printable
         * data in a string array.
         */

        public string bandName, location, link;

        public TopSetlistInfo(string bandName, string location, string link)
        {
            this.bandName = bandName;
            this.location = location;
            this.link = link;
        }

        public string[] GetFields()
        {
            string[] data = { bandName, location };
            return data;
        }
    }
}
