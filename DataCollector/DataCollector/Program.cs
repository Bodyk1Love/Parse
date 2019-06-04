using System;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using CsvHelper;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;

namespace DataCollector
{
    class MainClass
    {

        public static void Main(string[] args)
        {

            //StringBuilder csv = new StringBuilder();
            //string path = "xyz.csv";
            ////string path1 = "a-z.txt";
            ////string path = "allArtists.txt";


            //Console.WriteLine("Get artists links...");
            //List<string> links = GetLinkOnArtist();
            //Console.WriteLine("Done!");
            //Dictionary<string, string> Artists = new Dictionary<string, string>();
            //string[] AllArtistsOnLetter = GenerateAllLinks(links[0]);
            //for (int i = 0; i < 5; i++)
            //{
            //    var page = GetArtistLink(AllArtistsOnLetter[i]);
            //    foreach (KeyValuePair<string, string> keyValue in page)
            //    {
            //        Artists.Add(keyValue.Key, keyValue.Value);
            //    }
            //}
            //foreach (var name in Artists.Keys)
            //{
            //    Console.WriteLine(name);
            //}
            //List<string> AllArtists = new List<string>();
            //foreach (var link in links)
            //{
            //    AllArtists.AddRange(GenerateAllLinks(link));
            //}
            ////foreach (var link in AllArtists)
            ////{
            ////    File.AppendAllText(path, link + Environment.NewLine);
            ////}
            //foreach (var link in AllArtists)
            //{
            //    Console.WriteLine(link);
            //    Dictionary<string, string> Artists = GetArtistLink(link);
            //    foreach (KeyValuePair<string, string> keyValue in Artists)
            //    {
            //        //File.AppendAllText(path, keyValue.Key + '|' + keyValue.Value + Environment.NewLine);
            //        Console.WriteLine("DONE...");
            //    }
            //}
            //Console.WriteLine("Make dictionary of every artist and link...");
            //Dictionary<string, string> Artist = GetArtistLink(links[0]);
            //Console.WriteLine("Done!");

            //foreach (KeyValuePair<string, string> keyValue in Artist)
            //{
            //    Console.WriteLine("Get link on statistics...");
            //    string statistics = GetArtistStatistics(keyValue.Key);
            //    Console.WriteLine("Done...");

            //    Console.WriteLine("Get songs...");
            //    List<string> songs = GetSongs(statistics);
            //    Console.WriteLine("Done...");

            //    //Console.WriteLine("Add to file...");
            //    //foreach (var song in songs)
            //    //{
            //    //    csv.AppendLine($"{keyValue.Value},{song}");
            //    //    File.WriteAllText(path, csv.ToString());
            //    //}

            //    Console.WriteLine("Done...");
            //}
            //List<MainInfo> MainPageInfo = ParseMainPage();
            //Console.WriteLine(MainPageInfo.Count);
            //SetlistInfo check = ParseSetlistPage(MainPageInfo[11]);
            //foreach(string s in check.songs)
            //{
            //    Console.WriteLine(s);
            //}
            //ArtistInfo artist = ParseArtistPage(check);
            //artist.printInfo();
            List<TopSetlistInfo> Top = ParseTopSetlistsPage();
            foreach(var i in Top)
            {
                i.printInfo();
            }
        }




        static List<string> GetSongs(string link)
        {
            // Get all name of songs in the page
            List<string> songs = new List<string>();
            HtmlWeb webDoc = new HtmlWeb();
            HtmlDocument doc = webDoc.Load("https://www.setlist.fm/" + link);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");

            foreach (var tag in nodes)
            {

                if (tag.Attributes.Contains("class") && 
                    (tag.Attributes["class"].Value == "songLabel" ||
                    tag.Attributes["class"].Value == "songName") &&
                    !songs.Contains(tag.InnerText))
                {
                    var name = clean(String.Format("\"{0}\"", tag.InnerText.Trim()));
                    songs.Add(name);
                }
            }
            return songs;
        }


        static List<string> GetLinkOnArtist()
        {
            // Get all links on artists [A-Z]

            List<string> links = new List<string>();
            HtmlWeb webDoc = new HtmlWeb();
            HtmlDocument doc = webDoc.Load("https://www.setlist.fm/artists");
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");

            foreach (var tag in nodes)
            {
                //Console.WriteLine(tag.Attributes[0].Value);
                if (tag.Attributes["href"] != null && 
                    tag.Attributes["href"].Value.Contains("artist") &&
                    tag.Attributes["href"].Value.EndsWith("1.html"))
                {
                    if (!links.Contains(tag.Attributes["href"].Value))
                    {
                        var link = tag.Attributes["href"].Value;
                        links.Add(link);
                    }
                }
            }
            return links;
        }

        static Dictionary<string, string> GetArtistLink(string linkToGo)
        {
            // Get all links on one artist name

            Dictionary<string, string> ArtistLink = new Dictionary<string, string>();
            HtmlWeb webDoc = new HtmlWeb();
            HtmlDocument doc = webDoc.Load("https://www.setlist.fm/" + linkToGo);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");

            foreach (var tag in nodes)
            {
                if (tag.Attributes.Contains("title") &&
                    tag.Attributes["title"].Value.StartsWith("More"))
                {
                    var link = tag.Attributes["href"].Value.Replace("../", "");
                    var name = clean(String.Format("\"{0}\"", tag.InnerHtml.Substring(6, tag.InnerHtml.Length - 13)));
                    ArtistLink.Add(link, name);
                }
            }
            return ArtistLink;
        }


        static void GetTourLinks()
        {
            // Get all links on one artist name
            HtmlWeb webDoc = new HtmlWeb();
            HtmlDocument doc = webDoc.Load("https://www.setlist.fm/setlists/g-13d4ade5.html");
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");

            foreach (var tag in nodes)
            {
                if (tag.Attributes.Contains("class") &&
                    tag.Attributes["class"].Value == "summary url")
                {
                    Console.WriteLine(tag.Attributes["href"].Value);
                }
            }
        }






        static string GetArtistStatistics(string link)
        {
            // Get all links on one artist name
            string statLink = null;
            HtmlWeb webDoc = new HtmlWeb();
            HtmlDocument doc = webDoc.Load("https://www.setlist.fm/" + link);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");

            foreach (var tag in nodes)
            {
                if (tag.Attributes.Contains("title") &&
                    tag.Attributes["title"].Value == "View song statistics of all setlists")

                {
                    statLink = tag.Attributes["href"].Value.Replace("../", "");
                }
            }
            return statLink;
        }


        static string[] GenerateAllLinks(string link)
        {
            // Get all links on one artist name

            int last = 0;
            HtmlWeb webDoc = new HtmlWeb();
            HtmlDocument doc = webDoc.Load("https://www.setlist.fm/" + link);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");

            string category = link.Split('/')[2];
            Console.WriteLine(category);

            foreach (var tag in nodes)
            {
                if (tag.Attributes.Contains("title") &&
                    tag.Attributes["title"].Value == "Go to last page")

                {
                    last = Convert.ToInt32(tag.InnerText);
                    break;
                }
            }
            string[] AllLinks = new string[last];
            for (int i=0; i<last; i++)
            {
                AllLinks[i] = "artist/browse/" + category + '/' + (i+1).ToString() + ".html";
            }
            return AllLinks;
        }



        static string clean(string s)
        {
            StringBuilder sb = new StringBuilder(s);
            sb.Replace("&#039;", "\'");
            sb.Replace("&#039", "\'");
            sb.Replace("&quot;", "\'");
            sb.Replace("&amp;", "&");
            sb.Replace("&nbsp;", " ");
            return sb.ToString();
        }

        //-------------------------------------------------------------------
        static List<MainInfo> ParseMainPage()
        {
            HtmlWeb webDoc = new HtmlWeb();
            HtmlDocument doc = webDoc.Load("https://www.setlist.fm/");
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");
            List<MainInfo> MainPage = new List<MainInfo>();
            foreach(var tag in nodes)
            {
                if (tag.Attributes.Contains("title") &&
                    tag.Attributes["title"].Value.StartsWith("View this"))

                {
                    string observeLink = tag.Attributes["href"].Value;
                    string[] txt = tag.InnerText.Split(
                                    new[] { Environment.NewLine },
                                    StringSplitOptions.None
                    );
                    txt = txt.Where(c => c != "").ToArray();
                    MainInfo obj = new MainInfo(clean(txt[0]), txt[1], txt[2], txt[3], txt[4], observeLink);
                    MainPage.Add(obj);
                }
            }
            return MainPage;
        }

        static List<TopSetlistInfo> ParseTopSetlistsPage()
        {
            HtmlWeb webDoc = new HtmlWeb();
            HtmlDocument doc = webDoc.Load("https://www.setlist.fm/setlists");
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");
            List<TopSetlistInfo> TopSetlists = new List<TopSetlistInfo>();
            foreach (var tag in nodes)
            {
                if (tag.Attributes.Contains("class") &&
                    tag.Attributes["class"].Value == "twoLineLink")

                {
                    string observeLink = tag.Attributes["href"].Value;
                    string[] txt = tag.InnerText.Split(
                                    new[] { Environment.NewLine },
                                    StringSplitOptions.None
                    );
                    txt = txt.Where(c => c != "").ToArray();
                    TopSetlistInfo obj = new TopSetlistInfo(clean(txt[0]), txt[1], observeLink);
                    TopSetlists.Add(obj);
                }
            }
            return TopSetlists;
        }

        static SetlistInfo ParseSetlistPage(MainInfo parent)
        {
            // Get all name of songs in the page
            List<string> songs = new List<string>();
            string artistLink = "";
            HtmlWeb webDoc = new HtmlWeb();
            HtmlDocument doc = webDoc.Load("https://www.setlist.fm/" + parent.link);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");

            foreach (var tag in nodes)
            {

                if (tag.Attributes.Contains("class") &&
                    tag.Attributes["class"].Value == "songLabel" &&
                    !songs.Contains(tag.InnerText))
                {
                    var name = clean(tag.InnerText.Trim());
                    songs.Add(name);
                }
                if (tag.Attributes.Contains("title") &&
                   tag.Attributes["title"].Value == "View song statistics of all setlists")
                {
                    artistLink = tag.Attributes["href"].Value.Replace("../", "");
                }
            }
            SetlistInfo obj = new SetlistInfo(parent.GetFields(), artistLink, songs);
            return obj;
        }


        static ArtistInfo ParseArtistPage(SetlistInfo parent)
        {
            List<string> songs = new List<string>();
            Dictionary<string, List<string>> tours = new Dictionary<string, List<string>>();
            List<string> albums = new List<string>();
            string albumsLink = "";

            HtmlWeb webDoc = new HtmlWeb();
            HtmlDocument doc = webDoc.Load("https://www.setlist.fm/" + parent.artistLink);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");
            foreach (var tag in nodes)
            {
                if (tag.Attributes.Contains("class") &&
                   tag.Attributes["class"].Value == "songName" &&
                   !songs.Contains(tag.InnerText))
                {
                    var name = clean(tag.InnerText.Trim());
                    songs.Add(name);
                }

                if (tag.Attributes.Contains("title") &&
                  tag.Attributes["title"].Value.StartsWith("Show song statistics of the tour"))
                {
                    var name = clean(tag.InnerText.Trim());
                    tours.Add(name, ParseTourPage(tag.Attributes["href"].Value));
                }
                if (tag.Attributes.Contains("title") &&
                  tag.Attributes["title"].Value.EndsWith("albums"))
                {
                    Console.WriteLine(tag.Attributes["href"].Value);
                    albumsLink = tag.Attributes["href"].Value.Replace("../", "");
                }
            }
            Console.WriteLine(albumsLink);

            albums = ParseAlbumPage(albumsLink);

            ArtistInfo artist = new ArtistInfo(parent.bandName, songs, tours, albums);
            return artist;
        }


        static List<string> ParseAlbumPage(string link)
        {
            // Get all name of songs in the page
            List<string> albums = new List<string>();
            HtmlWeb webDoc = new HtmlWeb();
            HtmlDocument doc = webDoc.Load("https://www.setlist.fm/" + link);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//td");
            try
            {
                foreach (var tag in nodes)
                {
                    if (tag.Attributes.Contains("class") &&
                       tag.Attributes["class"].Value == "songName")
                    {
                        if (tag.InnerText != "")
                        {
                            albums.Add(clean(tag.InnerText.Trim()));
                        }
                    }
                }
            }
            catch (Exception)
            {
                albums.Add("No albums");
                return albums;
            }
            return albums;
        }


        static List<string> ParseTourPage(string link)
        {
            // Get all name of songs in the page
            List<string> songs = new List<string>();
            HtmlWeb webDoc = new HtmlWeb();
            HtmlDocument doc = webDoc.Load("https://www.setlist.fm/" + link);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");

            foreach (var tag in nodes)
            {
                if (tag.Attributes.Contains("class") &&
                   tag.Attributes["class"].Value == "songName")
                {
                    songs.Add(clean(tag.InnerText.Trim()));
                }
            }
            return songs;
        }



    }
}