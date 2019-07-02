using System;
using System.Collections.Generic;

namespace Setlist
{
    public class SearchingInfo
    {
        /* Class describes searching page objects
        * such as Setlists and link on artist stats.
        * It keeps info about artist/band name, and 
        * a dictionry with a link: SetlistName pair.
        * 
        * ParseSearchingPage() function get all data from
        * searching page and make an object of this class.
        */

        public string bandName, artistLink;
        public Dictionary<string, string> Setlists;

        public SearchingInfo(string bandName, Dictionary<string, string> Setlists, string artistLink)
        {
            this.bandName = bandName;
            this.Setlists = Setlists;
            this.artistLink = artistLink;
        }
    }
}
