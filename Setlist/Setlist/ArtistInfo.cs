using System;
using System.Collections.Generic;

namespace Setlist
{
    public class ArtistInfo
    {
        /* Class describes artist page object.
         * It keeps info about artist/band name,
         * list of all it's songs, list with it's
         * albums a a dictionary with a 
         * tourName(string) : tourSongs(List<string>)        
         * pair.
         * 
         * ParseArtistPage() function get all data from
         * artist page and make an object of this class.
         */


        public string bandName;
        public List<string> songs = new List<string>();
        public Dictionary<string, List<string>> tours = new Dictionary<string, List<string>>();
        public List<string> albums = new List<string>();

        public ArtistInfo(string bandName, List<string> songs,
               Dictionary<string, List<string>> tours, List<string> albums)
        {
            this.bandName = bandName;
            this.songs = songs;
            this.tours = tours;
            this.albums = albums;
        }
    }
}
