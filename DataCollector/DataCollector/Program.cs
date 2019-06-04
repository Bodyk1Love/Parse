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
            //List<TopSetlistInfo> Top = ParseTopSetlistsPage();
            //foreach(var i in Top)
            //{
            //    i.printInfo();
            //}


            MainPageToFile("MainPage.txt");
            OneSetlistInfo("SetlistInfo.txt");
            TopSetlist("TopSetlists.txt");
        }






        public static List<string> GetLinksOnArtists()
        {
            /* Get all links on artists [A-Z] from page
             * https://www.setlist.fm/artists.
             * 
             * 
             * 
             * Output: string list with links
             * for each artist. (27 links)
             */

            List<string> links = new List<string>();
            HtmlWeb webDoc = new HtmlWeb();
            HtmlDocument doc = webDoc.Load("https://www.setlist.fm/artists");
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");

            if (nodes.Count == 0)
            {
                links.Add("Problem occured while loading all artists!");
                return links;
            }

            foreach (var tag in nodes)
            {
                if (tag.Attributes["href"] != null &&
                    tag.Attributes["href"].Value.Contains("artist") &&
                    tag.Attributes["href"].Value.EndsWith("1.html", StringComparison.Ordinal))
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




        public static string[] GenerateAllLinks(string link)
        {
            /* Get all links on all pages for
             * first artist letter.
             * 
             * Input: link on first page with
             * all artists. (From list that generate
             * GetLinksOnArtists() function  
             * for exsample: https://www.setlist.fm/artist/browse/a/1.html
             * 
             * Output: string array with all links
             * with artists on first letter.
             */


            int last = 0;
            HtmlWeb webDoc = new HtmlWeb();
            HtmlDocument doc = webDoc.Load("https://www.setlist.fm/" + link);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");

            if (nodes.Count == 0)
            {
                return "Problem occured while loading artist link! |".Split('|');
            }

            string category = link.Split('/')[2];

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
            for (int i = 0; i < last; i++)
            {
                AllLinks[i] = "artist/browse/" + category + '/' + (i + 1).ToString() + ".html";
            }
            return AllLinks;
        }



        public static Dictionary<string, string> GetArtistLink(string linkToGo)
        {
            /* Get a dictionary with a link on
             * one artist from page with all artists.
             * 
             * Input: a link on a page with artists
             * (From array that is generated by
             * GenerateAllLinks() function)
             * 
             * Output: a dictionary with
             * link : artist_name pair.
             */

            Dictionary<string, string> ArtistLink = new Dictionary<string, string>();
            HtmlWeb webDoc = new HtmlWeb();
            HtmlDocument doc = webDoc.Load("https://www.setlist.fm/" + linkToGo);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");

            foreach (var tag in nodes)
            {
                if (tag.Attributes.Contains("title") &&
                    tag.Attributes["title"].Value.StartsWith("More", StringComparison.Ordinal))
                {
                    var link = tag.Attributes["href"].Value.Replace("../", "");
                    var name = clean(String.Format("\"{0}\"", tag.InnerHtml.Substring(6, tag.InnerHtml.Length - 13)));
                    ArtistLink.Add(link, name);
                }
            }
            return ArtistLink;
        }


        public static string clean(string s)
        {
            /*To correct special chars in data
             * 
             * Input: parsed string
             * 
             * Output: cleaned string         
             */
            StringBuilder sb = new StringBuilder(s);
            sb.Replace("&#039;", "\'");
            sb.Replace("&#039", "\'");
            sb.Replace("&quot;", "\'");
            sb.Replace("&amp;", "&");
            sb.Replace("&nbsp;", " ");
            return sb.ToString();
        }

        //-------------------------------------------------------------------
        public static List<MainInfo> ParseMainPage()
        {
            /* Get main data from the main page
             * https://www.setlist.fm/
             * 
             * 
             * Output: list of MainInfo objects
             * (see info in MainInfo.cs)
             */
            HtmlWeb webDoc = new HtmlWeb();
            HtmlDocument doc = webDoc.Load("https://www.setlist.fm/");
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");
            List<MainInfo> MainPage = new List<MainInfo>();
            foreach (var tag in nodes)
            {
                if (tag.Attributes.Contains("title") &&
                    tag.Attributes["title"].Value.StartsWith("View this", StringComparison.Ordinal))

                {
                    string observeLink = tag.Attributes["href"].Value;
                    string[] txt = tag.InnerText.Split(
                                    new[] { Environment.NewLine },
                                    StringSplitOptions.None
                    );
                    txt = txt.Where(c => c != "").ToArray();
                    MainInfo obj = new MainInfo(clean(txt[0]), clean(txt[1]), txt[2], txt[3], txt[4], observeLink);
                    MainPage.Add(obj);
                }
            }
            return MainPage;
        }


        public static List<TopSetlistInfo> ParseTopSetlistsPage()
        {
            /* Get top setlists from setlit page 
             * https://www.setlist.fm/setlists
             * 
             * 
             * Output: list of TopSetlistInfo objects
             * (see info in TopSetlistInfo.cs)
             */
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
                    TopSetlistInfo obj = new TopSetlistInfo(clean(txt[0]), clean(txt[1]), observeLink);
                    TopSetlists.Add(obj);
                }
            }
            return TopSetlists;
        }

        public static SetlistInfo ParseSetlistPage(string link, string bandName)
        {
            /* Get main info from setlist page
             * of one artist.
             * 
             * Input: link on setlist and name of 
             * setlist artist.
             * 
             * Output: SetlistInfo object
             * (see info in SetlistInfo.cs)          
             */
            List<string> songs = new List<string>();
            string artistLink = "";
            HtmlWeb webDoc = new HtmlWeb();
            HtmlDocument doc = webDoc.Load("https://www.setlist.fm/" + link);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");

            foreach (var tag in nodes)
            {

                if ((tag.Attributes.Contains("class") &&
                    tag.Attributes["class"].Value == "summary url") ||
                    (tag.Attributes.Contains("title") &&
                    tag.Attributes["title"].Value.StartsWith("Statistics for", StringComparison.Ordinal)) &&
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
            if (songs.Count == 0)
            {
                songs.Add("Nothing");
            }
            SetlistInfo obj = new SetlistInfo(bandName, artistLink, songs);
            return obj;
        }


        public static ArtistInfo ParseArtistPage(SetlistInfo parent)
        {
            /* Get main info from artist page.
            *
            * 
            * Input: SetlistInfo object of one artist.
            * 
            * Output: ArtistInfo object
            * (see info in ArtistInfo.cs)          
            */
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
                  tag.Attributes["title"].Value.StartsWith("Show song statistics of the tour", StringComparison.Ordinal))
                {
                    var name = clean(tag.InnerText.Trim());
                    tours.Add(name, ParseTourPage(tag.Attributes["href"].Value));
                }
                if (tag.Attributes.Contains("title") &&
                  tag.Attributes["title"].Value.EndsWith("albums", StringComparison.Ordinal))
                {
                    albumsLink = tag.Attributes["href"].Value.Replace("../", "");
                }
            }
            albums = ParseAlbumPage(albumsLink);

            ArtistInfo artist = new ArtistInfo(parent.bandName, songs, tours, albums);
            return artist;
        }


        public static List<string> ParseAlbumPage(string link)
        {
            /* Get list of albums of an artist.
             *
             * Input: string link on artist's album.
             * 
             * Output: string list with artist albums.        
             */
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


        public static List<string> ParseTourPage(string link)
        {
            /* Get list of songs from a particular
             * tour.
             *
             * Input: string link on a tour.
             * 
             * Output: string list with songs from
             * the tour.        
             */
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


        public static void MainPageToFile(string path)
        {
            Globals.MainPageInfo = ParseMainPage();
            string str = "";
            for (var i = 0; i < 10; i++)
            {
                str += String.Join("\n", Globals.MainPageInfo[i].GetFields());
                str += "\n";
            }
            str += "----------------------\n";
            for (var i = 10; i < 20; i++)
            {
                str += String.Join("\n", Globals.MainPageInfo[i].GetFields());
                str += "\n";
            }
            str += "----------------------\n";
            for (var i = 20; i < 30; i++)
            {
                str += String.Join("\n", Globals.MainPageInfo[i].GetFields());
                str += "\n";
            }
            str += "----------------------\n";
            Console.WriteLine(str);
            File.WriteAllText(path, str);
        }

        public static void TopSetlist(string path)
        {
            Globals.TopSetlists = ParseTopSetlistsPage();
            string str = "";
            for (var i = 0; i < 10; i++)
            {
                str += String.Join("\n", Globals.TopSetlists[i].GetFields());
                str += "\n";
            }
            Console.WriteLine(str);
            File.WriteAllText(path, str);
        }

        public static void OneSetlistInfo(string path)
        {
            SetlistInfo check = ParseSetlistPage(Globals.MainPageInfo[0].link, Globals.MainPageInfo[0].bandName);
            ArtistInfo artist = ParseArtistPage(check);
            string str = $"Artist name: {artist.bandName}\n\n";
            str += "------------------------------";
            str += "All songs\n";
            foreach (var song in artist.songs)
            {
                str += song;
                str += "\n";
            }
            str += "------------------------------";
            str += "Tours\n";
            foreach (KeyValuePair<string, List<string>> keyValue in artist.tours)
            {
                str += keyValue.Key;
                str += "\n";
                foreach (var song in keyValue.Value)
                {
                    str += $"    {song}\n";
                }
                str += "\n";
            }
            str += "------------------------------";
            str += "Albums\n";
            foreach (var album in artist.albums)
            {
                str += album;
                str += "\n";
            }
            str += "------------------------------";
            str += "Setlist songs\n";
            foreach (var song in check.songs)
            {
                str += song;
                str += "\n";
            }
            File.WriteAllText(path, str);
        }

    }
}