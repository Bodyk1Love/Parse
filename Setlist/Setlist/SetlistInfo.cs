using System;
using System.Collections.Generic;

namespace Setlist
{
    public class SetlistInfo
    {
        public string bandName, location, month, day, year, artistLink;
        public List<string> songs = new List<string>();

        public SetlistInfo(string[] data, string artistLink, List<string> songs)
        {
            this.bandName = data[0];
            this.location = data[1];
            this.month = data[2];
            this.day = data[3];
            this.year = data[4];
            this.artistLink = artistLink;
            this.songs = songs;
        }
    }
}
