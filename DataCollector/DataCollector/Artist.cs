using System;
using System.Collections.Generic;

namespace DataCollector
{
    public class Artist
    {
        protected string bandName;
        public List<string> songs = new List<string>();

        public Artist(string name)
        {
            bandName = name;
        }
    }
}
