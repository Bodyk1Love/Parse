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
        label1.Text = "Done1";
        for (var i = 10; i < 20; i++)
        {
            combobox2.AppendText(String.Join("\n", Globals.MainPageInfo[i].GetFields()));
        }
        label1.Text = "Done2";
        for (var i = 20; i < 30; i++)
        {
            combobox3.AppendText(String.Join("\n", Globals.MainPageInfo[i].GetFields()));
        }
        label1.Text = "Done3";

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
        SetlistInfo obj = new SetlistInfo(parent.GetFields(), artistLink, songs);
        return obj;
    }

    static ArtistInfo ParseArtistPage(SetlistInfo parent)
    {
        List<string> songs = new List<string>();
        Globals.tours = new Dictionary<string, List<string>>();
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
                Globals.tours.Add(name, ParseTourPage(tag.Attributes["href"].Value));
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
        foreach (var tour in artist.tours.Keys)
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
        foreach (var tour in artist.tours.Keys)
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
        foreach (var tour in artist.tours.Keys)
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
            List<string> tourSongs = Globals.tours.Values.ElementAt(id);
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
}
