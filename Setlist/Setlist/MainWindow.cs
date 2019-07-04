using System;
using System.Collections.Generic;
using Gtk;
using System.Linq;
using HtmlAgilityPack;
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
                string[] txt = tag.InnerText.Split(
                                new[] { "\n" },
                                StringSplitOptions.None
                );
                txt = txt.Where(c => c != "").ToArray();
                string observeLink = tag.Attributes["href"].Value;
                MainInfo obj = new MainInfo(clean(txt[0]), clean(txt[1]), txt[2], txt[3], txt[4], observeLink);
                MainPage.Add(obj);
            }
        }
        return MainPage;
    }

    public static SearchingInfo ParseSearchingPage(string link)
    {
        /* Get list of setlists from
         * the page of artist that user
         * is searching for from         
         * https://www.setlist.fm/search?query=
         * link        
         * 
         * 
         * Output: list of MainInfo objects
         * (see info in MainInfo.cs)
         */
        HtmlWeb webDoc = new HtmlWeb();
        HtmlDocument doc = webDoc.Load("https://www.setlist.fm/search?query=" + link);
        HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a");
        Dictionary<string, string> Setlists = new Dictionary<string, string>();
        string artistLink = "";
        foreach (var tag in nodes)
        {
            if (tag.Attributes.Contains("title") &&
               tag.Attributes["title"].Value == "View song statistics of all setlists")
            {
                artistLink = tag.Attributes["href"].Value.Replace("../", "");
            }
            if (!tag.Attributes.Contains("class") &&
                tag.Attributes.Contains("title") &&
                tag.Attributes["title"].Value.StartsWith("View this", StringComparison.Ordinal))

            {
                string observeLink = tag.Attributes["href"].Value.Replace("../", "");
                string txt = tag.InnerText;
                Setlists.Add(observeLink, txt);
            }
        }
        SearchingInfo SearchingPage = new SearchingInfo(link, Setlists, artistLink);
        return SearchingPage;
    }


    public static string clean(string s)
    {
        StringBuilder sb = new StringBuilder(s);
        sb.Replace("&#039;", "\'");
        sb.Replace("&#039", "\'");
        sb.Replace("&quot;", "\'");
        sb.Replace("&amp;", "&");
        sb.Replace("&nbsp;", " ");
        return sb.ToString();
    }



    public static SetlistInfo ParseSetlistPage(MainInfo parent)
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
        if (songs.Count == 0)
        {
            songs.Add("Nothing");
        }
        SetlistInfo obj = new SetlistInfo(parent.GetFields(), artistLink, songs);
        return obj;
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


    public static ArtistInfo ParseArtistPage(string link, string bandName)
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
        Dictionary<string, string> tours = new Dictionary<string, string>();
        List<string> albums = new List<string>();
        string albumsLink = "";

        HtmlWeb webDoc = new HtmlWeb();
        HtmlDocument doc = webDoc.Load("https://www.setlist.fm/" + link);
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
                tours.Add(tag.Attributes["href"].Value.Replace("../", ""), name);
            }
            if (tag.Attributes.Contains("title") &&
              tag.Attributes["title"].Value.EndsWith("albums", StringComparison.Ordinal))
            {
                albumsLink = tag.Attributes["href"].Value.Replace("../", "");
            }
        }
        albums = ParseAlbumPage(albumsLink);

        ArtistInfo artist = new ArtistInfo(bandName, songs, tours, albums);
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
                                new[] { "\n" },
                                StringSplitOptions.None
                );
                txt = txt.Where(c => c != "").ToArray();
                TopSetlistInfo obj = new TopSetlistInfo(clean(txt[0]), clean(txt[1]), observeLink);
                TopSetlists.Add(obj);
            }
        }
        return TopSetlists;
    }




    protected void OnLoadMainPageClicked(object sender, EventArgs e)
    {
        MainPageObjectsOn();
        Globals.MainPageInfo = ParseMainPage();
        for (var i = 0; i < 10; i++)
        {
            PopularSetlistsMenu.AppendText(String.Join("\n", Globals.MainPageInfo[i].GetFields()));
        }
        for (var i = 10; i < 20; i++)
        {
            UpcomingEventsMenu.AppendText(String.Join("\n", Globals.MainPageInfo[i].GetFields()));
        }
        for (var i = 20; i < 30; i++)
        {
            RecentEditsMenu.AppendText(String.Join("\n", Globals.MainPageInfo[i].GetFields()));
        }
    }


    protected void OnLoadArtistsButtonClicked(object sender, EventArgs e)
    {
        label14.Visible = true;
        ArtistByFirstLetterMenu.Visible = true;
        Globals.artists = GetLinksOnArtists();
    }


    protected void OnLoadTopSetlistsButtonClicked(object sender, EventArgs e)
    {
        label12.Visible = true;
        TopSetlistsMenu.Visible = true;
        Globals.TopSetlists = ParseTopSetlistsPage();
        for (var i = 0; i < 10; i++)
        {
            TopSetlistsMenu.AppendText(String.Join("\n", Globals.TopSetlists[i].GetFields()));
        }
    }


    protected void OnFindArtisButtonClicked(object sender, EventArgs e)
    {
        ArtistInfoObjectsOn();
        label11.Visible = true;
        ArtistTopSetlists.Visible = true;
        for (int i = 0; i < 50; i++)
        {
            ArtistTopSetlists.RemoveText(0);
        }
        clear();
        string SearchingData = entry1.Text;
        string[] SeachingTokens = SearchingData.ToLower().Split();
        string SearchingString = String.Join("+", SeachingTokens);
        Globals.SearchingPageInfo = ParseSearchingPage(SearchingString);
        for (int i = 0, j = Globals.SearchingPageInfo.Setlists.Count; i < j; i++)
        {
            ArtistTopSetlists.AppendText(Globals.SearchingPageInfo.Setlists.Values.ElementAt(i));
        }
        Globals.CurrentArtist = ParseArtistPage(Globals.SearchingPageInfo.artistLink, Globals.SearchingPageInfo.bandName);
        Globals.Tours = new Dictionary<List<string>, string>();
        Globals.CurrentArtistToursLinks = Globals.CurrentArtist.tours.Keys.ToList();
        label1.Text = Globals.CurrentArtist.bandName;
        foreach (var song in Globals.CurrentArtist.songs)
        {
            AllArtistSongsMenu.AppendText(song);
        }
        foreach (var tour in Globals.CurrentArtist.tours.Values)
        {
            ArtistToursMenu.AppendText(tour);
        }
        foreach (var album in Globals.CurrentArtist.albums)
        {
            ArtistAlbumsMenu.AppendText(album);
        }
    }

    protected void OnPopularSetlistsMenuChanged(object sender, EventArgs e)
    {
        clear();
        ArtistInfoObjectsOn();
        var id = PopularSetlistsMenu.Active;
        SetlistInfo check = ParseSetlistPage(Globals.MainPageInfo[id]);
        Globals.CurrentArtist = ParseArtistPage(check.artistLink, check.bandName);
        label1.Text = Globals.CurrentArtist.bandName;
        Globals.CurrentArtistToursLinks = Globals.CurrentArtist.tours.Keys.ToList();
        foreach (var song in Globals.CurrentArtist.songs)
        {
            AllArtistSongsMenu.AppendText(song);
        }
        foreach (var tour in Globals.CurrentArtist.tours.Values)
        {
            ArtistToursMenu.AppendText(tour);
        }
        foreach (var album in Globals.CurrentArtist.albums)
        {
            ArtistAlbumsMenu.AppendText(album);
        }
        foreach (var song in check.songs)
        {
            SongsInSetlistMenu.AppendText(song);
        }
    }

    protected void OnUpcomingEventsMenuChanged(object sender, EventArgs e)
    {
        clear();
        ArtistInfoObjectsOn();
        var id = UpcomingEventsMenu.Active + 10;
        SetlistInfo check = ParseSetlistPage(Globals.MainPageInfo[id]);
        Globals.CurrentArtist = ParseArtistPage(check.artistLink, check.bandName);
        Globals.CurrentArtistToursLinks = Globals.CurrentArtist.tours.Keys.ToList();
        label1.Text = Globals.CurrentArtist.bandName;
        foreach (var song in Globals.CurrentArtist.songs)
        {
            AllArtistSongsMenu.AppendText(song);
        }
        foreach (var tour in Globals.CurrentArtist.tours.Values)
        {
            ArtistToursMenu.AppendText(tour);
        }
        foreach (var album in Globals.CurrentArtist.albums)
        {
            ArtistAlbumsMenu.AppendText(album);
        }
        foreach (var song in check.songs)
        {
            SongsInSetlistMenu.AppendText(song);
        }
    }

    protected void OnRecentEditsMenuChanged(object sender, EventArgs e)
    {
        clear();
        ArtistInfoObjectsOn();
        var id = RecentEditsMenu.Active + 20;
        SetlistInfo check = ParseSetlistPage(Globals.MainPageInfo[id]);
        Globals.CurrentArtist = ParseArtistPage(check.artistLink, check.bandName);
        Globals.CurrentArtistToursLinks = Globals.CurrentArtist.tours.Keys.ToList();
        label1.Text = Globals.CurrentArtist.bandName;
        foreach (var song in Globals.CurrentArtist.songs)
        {
            AllArtistSongsMenu.AppendText(song);
        }
        foreach (var tour in Globals.CurrentArtist.tours.Values)
        {
            ArtistToursMenu.AppendText(tour);
        }
        foreach (var album in Globals.CurrentArtist.albums)
        {
            ArtistAlbumsMenu.AppendText(album);
        }
        foreach (var song in check.songs)
        {
            SongsInSetlistMenu.AppendText(song);
        }
    }


    protected void OnArtistByFirstLetterMenuChanged(object sender, EventArgs e)
    {
        var id = ArtistByFirstLetterMenu.Active;
        ListOfArtistsOnChosenLetter.Visible = true;
        try
        {
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
                ListOfArtistsOnChosenLetter.AppendText(name);
            }
            Globals.Artists = Artists;
        }
        catch (Exception)
        {
            ListOfArtistsOnChosenLetter.AppendText("Load Artists first");
        }

    }


    protected void OnArtistToursMenuChanged(object sender, EventArgs e)
    {
        SongsInTourMenu.Visible = true;
        label10.Visible = true;
        for (int i = 0; i < 200; i++)
        {
            SongsInTourMenu.RemoveText(0);
        }
        int id = ArtistToursMenu.Active;
        if (id >= 0)
        {
            try
            {
                List<string> tourSongs = ParseTourPage(Globals.CurrentArtistToursLinks[id]);
                foreach (var song in tourSongs)
                {
                    SongsInTourMenu.AppendText(song);
                }
            }
            catch (Exception ex)
            {
                SongsInTourMenu.AppendText("Nothing" + ex);
            }
        }
    }


    protected void OnListOfArtistsOnChosenLetterChanged(object sender, EventArgs e)
    {
        clear();
        ArtistInfoObjectsOn();
        var id = ListOfArtistsOnChosenLetter.Active;
        SetlistInfo check = ParseSetlistPage(Globals.Artists.Keys.ElementAt(id), ListOfArtistsOnChosenLetter.ActiveText);
        Globals.CurrentArtist = ParseArtistPage(check.artistLink, check.bandName);
        Globals.CurrentArtistToursLinks = Globals.CurrentArtist.tours.Keys.ToList();
        label1.Text = Globals.CurrentArtist.bandName;
        foreach (var song in Globals.CurrentArtist.songs)
        {
            AllArtistSongsMenu.AppendText(song);
        }
        foreach (var tour in Globals.CurrentArtist.tours.Values)
        {
            ArtistToursMenu.AppendText(tour);
        }
        foreach (var album in Globals.CurrentArtist.albums)
        {
            ArtistAlbumsMenu.AppendText(album);
        }
        foreach (var song in check.songs)
        {
            SongsInSetlistMenu.AppendText(song);
        }
    }


    protected void OnTopSetlistsMenuChanged(object sender, EventArgs e)
    {
        clear();
        ArtistInfoObjectsOn();
        var id = TopSetlistsMenu.Active;
        SetlistInfo check = ParseSetlistPage(Globals.TopSetlists[id].link, Globals.TopSetlists[id].bandName);
        Globals.CurrentArtist = ParseArtistPage(check.artistLink, check.bandName);
        Globals.CurrentArtistToursLinks = Globals.CurrentArtist.tours.Keys.ToList();
        label1.Text = Globals.CurrentArtist.bandName;
        foreach (var song in Globals.CurrentArtist.songs)
        {
            AllArtistSongsMenu.AppendText(song);
        }
        foreach (var tour in Globals.CurrentArtist.tours.Values)
        {
            ArtistToursMenu.AppendText(tour);
        }
        foreach (var album in Globals.CurrentArtist.albums)
        {
            ArtistAlbumsMenu.AppendText(album);
        }
        foreach (var song in check.songs)
        {
            SongsInSetlistMenu.AppendText(song);
        }
    }

    protected void OnArtistTopSetlistsChanged(object sender, EventArgs e)
    {
        ArtistInfoObjectsOn();
        var id = ArtistTopSetlists.Active;
        for (int i = 0; i < 100; i++)
        {
            SongsInSetlistMenu.RemoveText(0);
        }
        try
        {
            SetlistInfo check = ParseSetlistPage(Globals.SearchingPageInfo.Setlists.Keys.ElementAt(id), Globals.SearchingPageInfo.bandName);
            foreach (var song in check.songs)
            {
                SongsInSetlistMenu.AppendText(song);
            }
        }
        catch (Exception ex)
        {
            label1.Text = ex.ToString();
        }
    }


    public void clear()
    {
        for (int i = 0; i < 200; i++)
        {
            SongsInSetlistMenu.RemoveText(0);
            AllArtistSongsMenu.RemoveText(0);
            ArtistToursMenu.RemoveText(0);
            ArtistAlbumsMenu.RemoveText(0);
        }
    }

    public void MainPageObjectsOn()
    {
        label2.Visible = true;
        label3.Visible = true;
        label4.Visible = true;
        PopularSetlistsMenu.Visible = true;
        UpcomingEventsMenu.Visible = true;
        RecentEditsMenu.Visible = true;
    }

    public void ArtistInfoObjectsOn()
    {
        label5.Visible = true;
        label6.Visible = true;
        label7.Visible = true;
        label8.Visible = true;
        AllArtistSongsMenu.Visible = true;
        ArtistAlbumsMenu.Visible = true;
        ArtistToursMenu.Visible = true;
        SongsInSetlistMenu.Visible = true;
    }


}
