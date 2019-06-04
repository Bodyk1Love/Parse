using System;
using System.Collections.Generic;
using Gtk;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using System.Reflection;
using System.IO;
using System.Text;
using Setlist;


public partial class MainWindow : Gtk.Window
{
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnButton1Clicked(object sender, EventArgs e)
    {
        Globals.MainPageInfo = ParseMainPage();
        for(var i=0; i<10; i++)
        {
            combobox1.AppendText(String.Join("\n", Globals.MainPageInfo[i].GetFields()));
        }
        for (var i = 10; i < 20; i++)
        {
            combobox2.AppendText(String.Join("\n", Globals.MainPageInfo[i].GetFields()));
        }
        for (var i = 20; i < 30; i++)
        {
            combobox3.AppendText(String.Join("\n", Globals.MainPageInfo[i].GetFields()));
        }

    }


    static List<MainInfo> ParseMainPage()
    {
        HtmlWeb webDoc = new HtmlWeb();
        HtmlDocument doc = webDoc.Load("https://www.setlist.fm/");
        HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");
        List<MainInfo> MainPage = new List<MainInfo>();
        foreach (var tag in nodes)
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
        if(songs.Count == 0)
        {
            songs.Add("Nothing");
        }
        SetlistInfo obj = new SetlistInfo(parent.GetFields(), artistLink, songs);
        return obj;
    }

    static SetlistInfo ParseSetlistPage(string link, string bandName)
    {
        // Get all name of songs in the page
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
                tag.Attributes["title"].Value.StartsWith("Statistics for")) &&
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

    static ArtistInfo ParseArtistPage(SetlistInfo parent)
    {
        List<string> songs = new List<string>();
        Globals.tours = new Dictionary<List<string>, string>();
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
                Globals.tours.Add(ParseTourPage(tag.Attributes["href"].Value), name);
            }
            if (tag.Attributes.Contains("title") &&
              tag.Attributes["title"].Value.EndsWith("albums"))
            {
                albumsLink = tag.Attributes["href"].Value.Replace("../", "");
            }
        }

        albums = ParseAlbumPage(albumsLink);

        ArtistInfo artist = new ArtistInfo(parent.bandName, songs, Globals.tours, albums);
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
        for (int i = 0; i < last; i++)
        {
            AllLinks[i] = "artist/browse/" + category + '/' + (i + 1).ToString() + ".html";
        }
        return AllLinks;
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


    protected void OnCombobox1Changed(object sender, EventArgs e)
    {
        clear();
        var id = combobox1.Active;
        label1.Text = $"Got id {id.ToString()}";
        SetlistInfo check = ParseSetlistPage(Globals.MainPageInfo[id]);
        label1.Text = "Setlist parsed!!";
        ArtistInfo artist = ParseArtistPage(check);
        label1.Text = "Artist got!";
        label1.Text = artist.bandName;
        foreach (var song in artist.songs)
        {
            combobox6.AppendText(song);
        }
        foreach (var tour in artist.tours.Values)
        {
            combobox7.AppendText(tour);
        }
        foreach (var album in artist.albums)
        {
            combobox8.AppendText(album);
        }
        foreach (var song in check.songs)
        {
            combobox5.AppendText(song);
        }
    }

    protected void OnCombobox2Changed(object sender, EventArgs e)
    {
        clear();
        var id = combobox2.Active + 10;
        SetlistInfo check = ParseSetlistPage(Globals.MainPageInfo[id]);
        ArtistInfo artist = ParseArtistPage(check);
        label1.Text = artist.bandName;
        foreach (var song in artist.songs)
        {
            combobox6.AppendText(song);
        }
        foreach (var tour in artist.tours.Values)
        {
            combobox7.AppendText(tour);
        }
        foreach (var album in artist.albums)
        {
            combobox8.AppendText(album);
        }
        foreach (var song in check.songs)
        {
            combobox5.AppendText(song);
        }
    }

    protected void OnCombobox3Changed(object sender, EventArgs e)
    {
        clear();
        var id = combobox3.Active + 20;
        SetlistInfo check = ParseSetlistPage(Globals.MainPageInfo[id]);
        ArtistInfo artist = ParseArtistPage(check);
        label1.Text = artist.bandName;
        foreach (var song in artist.songs)
        {
            combobox6.AppendText(song);
        }
        foreach (var tour in artist.tours.Values)
        {
            combobox7.AppendText(tour);
        }
        foreach (var album in artist.albums)
        {
            combobox8.AppendText(album);
        }
        foreach (var song in check.songs)
        {
            combobox5.AppendText(song);
        }
    }

    public void clear()
    {
        for (int i=0; i<200; i++)
        {
            combobox5.RemoveText(0);
            combobox6.RemoveText(0);
            combobox7.RemoveText(0);
            combobox8.RemoveText(0);
        }
    }

    protected void OnCombobox7Changed(object sender, EventArgs e)
    {
        for (int i = 0; i < 200; i++)
        {
            combobox5.RemoveText(0);
        }
        int id = combobox7.Active;
        try
        {
            List<string> tourSongs = Globals.tours.Keys.ElementAt(id);
            foreach (var song in tourSongs)
            {
                combobox5.AppendText(song);
            }
        }
        catch (Exception)
        {
            combobox5.AppendText("Nothing");
        }
    }

    protected void OnButton2Clicked(object sender, EventArgs e)
    {
        Globals.artists = GetLinkOnArtist();
    }

    protected void OnCombobox4Changed(object sender, EventArgs e)
    {
        var id = combobox4.Active;
        string[] AllArtistsOnLetter = GenerateAllLinks(Globals.artists[id]);
        Dictionary<string, string> Artists = new Dictionary<string, string>();
        for (int i = 0; i < 5; i++)
        {
            var page = GetArtistLink(AllArtistsOnLetter[i]);
            foreach (KeyValuePair<string, string> keyValue in page)
            {
                Artists.Add(keyValue.Key, keyValue.Value);
            } 
        }
        foreach (var name in Artists.Values)
        {
            combobox9.AppendText(name);
        }
        Globals.Artists = Artists;

    }

    protected void OnCombobox9Changed(object sender, EventArgs e)
    {
        clear();
        var id = combobox9.Active;
        SetlistInfo check = ParseSetlistPage(Globals.Artists.Keys.ElementAt(id), combobox9.ActiveText);
        ArtistInfo artist = ParseArtistPage(check);
        label1.Text = artist.bandName;
        foreach (var song in artist.songs)
        {
            combobox6.AppendText(song);
        }
        foreach (var tour in artist.tours.Values)
        {
            combobox7.AppendText(tour);
        }
        foreach (var album in artist.albums)
        {
            combobox8.AppendText(album);
        }
        foreach (var song in check.songs)
        {
            combobox5.AppendText(song);
        }
    }

    protected void OnButton3Clicked(object sender, EventArgs e)
    {
        Globals.TopSetlists = ParseTopSetlistsPage();
        for (var i = 0; i < 10; i++)
        {
            combobox10.AppendText(String.Join("\n", Globals.TopSetlists[i].GetFields()));
        }
    }

    protected void OnCombobox10Changed(object sender, EventArgs e)
    {
        clear();
        var id = combobox10.Active;
        SetlistInfo check = ParseSetlistPage(Globals.TopSetlists[id].link, Globals.TopSetlists[id].bandName);
        ArtistInfo artist = ParseArtistPage(check);
        label1.Text = artist.bandName;
        foreach (var song in artist.songs)
        {
            combobox6.AppendText(song);
        }
        foreach (var tour in artist.tours.Values)
        {
            combobox7.AppendText(tour);
        }
        foreach (var album in artist.albums)
        {
            combobox8.AppendText(album);
        }
        foreach (var song in check.songs)
        {
            combobox5.AppendText(song);
        }
    }
}
