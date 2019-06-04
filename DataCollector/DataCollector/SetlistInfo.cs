
using System.Collections.Generic;

namespace DataCollector
{
    public class SetlistInfo
    {
        /* Class describes setlist page object.
         * It keeps info about artist/band name,
         * link on an artist's info and list of 
         * song in this setlist.
         * 
         * ParseSetlistPage() function get all data from
         * setlist page and make an  object of this class.
         * 
         * ParseArtistPage() function uses an object of
         * this class as an argument, to get a link
         * on an artist info page.
         */


        public string bandName, artistLink;
        public List<string> songs = new List<string>();


        public SetlistInfo(string bandName, string artistLink, List<string> songs)
        {
            this.bandName = bandName;
            this.artistLink = artistLink;
            this.songs = songs;
        }
    }
}
