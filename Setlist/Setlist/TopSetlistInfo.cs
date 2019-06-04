using System;
namespace Setlist
{
    public class TopSetlistInfo
    {
        public string bandName, location, link;

        public TopSetlistInfo(string bandName, string location, string link)
        {
            this.bandName = bandName;
            this.location = location;
            this.link = link;
        }

        public void printInfo()
        {
            Console.WriteLine($"Band: {bandName}, Location: {location}, Link: {link}");
        }

        public string[] GetFields()
        {
            string[] data = { bandName, location };
            return data;
        }
    }
}
