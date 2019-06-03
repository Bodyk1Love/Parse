using System;
using System.Collections.Generic;
namespace DataCollector
{
    public class ArtistInfo
    {
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

        public void printInfo()
        {
            Console.WriteLine($"Band: {bandName}");
            Console.WriteLine("----------------------------------");
            songs.ForEach(Console.WriteLine);
            Console.WriteLine("----------------------------------");
            foreach (KeyValuePair<string, List<string>> kvp in tours)
            {
                Console.Write("Key = {0}, Value ", kvp.Key);
                kvp.Value.ForEach(Console.WriteLine);
            }
            Console.WriteLine("----------------------------------");
            albums.ForEach(Console.WriteLine);
        }
    }
}
