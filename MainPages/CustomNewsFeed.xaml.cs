using FFImageLoading.Forms;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NWT
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomNewsFeed : ContentPage
    {
		
		/*
		
			The CustomNewsFeed generates a list of Article objects based on your selections in the sidemenu.
			If the user has a tag, an author, and a category in their list in the SideMenu, then only articles containing those will be shown in this list.
			
			These objects lead to newsPage, where in each objects content will be shown to the user.
			
		*/
		
        public int Loadnr = 1;
        public static int DBLN = 20;
        public int Rownr = 1;
        public static TapGestureRecognizer TGR;
        public List<Article> ArticleList = new List<Article>();
        public List<Article> ArticlePrintList = new List<Article>();
        public static string Defaultimage = "https://cdn.production.nwt.se/static-assets/logos/nwt/main.svg";
        public static Random rnd = new Random();
        public List<string> Filter = new List<string>();
        public List<string> Author = new List<string>();
        public List<string> Tag = new List<string>();
        public ListView ArticleListView = new ListView();
        public int PREV = 0;
        public int CURR = DBLN;
        public int NEXT = DBLN * 2;
        public bool ScrollLock = false;
        public bool TagsModified = false;
        public bool First = true;
        public int argc = 0;
        public int interval = 0;

		// The Article object defined and holds all data required to load in a full article. This is used in the news feeds, as well as the NewsPage in order to display articles to the user.
        public class Article
        {
            public long ID { get; set; }
            public string Source { get; set; }
            public Color CategoryColor { get; set; }
            public string FirstTag { get; set; }
            public string Tag { get; set; }
            public bool TagVisible { get; set; }
            public int TagLength { get; set; }
            public string Header { get; set; }
            public string Ingress { get; set; }
            public bool HasIngress { get; set; }
            public DateTime DatePublished { get; set; }
            public string datePub { get; set; }
            public int DateLength { get; set; }
            public string IMGSource { get; set; }
            public int HeaderLength { get; set; }
            public string ArticleReactions { get; set; }
            public string ReactionSum { get; set; }
            public int ReactionNR { get; set; }
            public string ReactionSrc { get; set; }
            public string ReactionSrcOthers { get; set; }
            public bool Plus { get; set; }
            public bool Full { get; set; }
            public bool CategoryBig { get; set; }
            public bool CategorySmall { get; set; }
            public bool AdVisibility { get; set; }
            public string AdText { get; set; }
            public string AdSource { get; set; }

            public int IHR { get; set; }
            public int BHR { get; set; }
            public int CBWR { get; set; }
            public int CBHR { get; set; }
            public LayoutOptions CBHO { get; set; }
            public LayoutOptions CBVO { get; set; }
            public int INGHR { get; set; }

            public Article(NewsfeedTable NF, int interval)
            {
                ReactionNR = 9999;

                if (NF.Category.Contains("Sport"))
                {
                    CategoryColor = Color.FromHex("#6fb110");
                }
                else if (NF.Category.Contains("Ekonomi"))
                {
                    CategoryColor = Color.FromHex("#3b5e6a");
                }
                else if (NF.Category.Contains("Åsikter"))
                {
                    CategoryColor = Color.FromHex("#57bcbf");
                }
                else if(NF.Category.Contains("Nöje/Kultur"))
                {
                    CategoryColor = Color.FromHex("#bb0066");
                }
                else if (NF.Category.Contains("Familj"))
                {
                    CategoryColor = Color.FromHex("#e0d8b3");
                }
                else
                {
                    CategoryColor = Color.FromHex("#3b5e6a");
                }

                Tag = "";
                FirstTag = "";
                if (App.LoggedinUser != null)
                {
                    var TagList = JsonConvert.DeserializeObject<List<List<string>>>(App.LoggedinUser.TaggString);
                    var UserTags = TagList[1];
                    var Firsttag = true;

                    foreach (var Tag_ in UserTags)
                    {
                        if (NF.Tag.Contains(Tag_))
                        {
                            if (Firsttag)
                            {
                                FirstTag = Tag_;
                                Firsttag = false;
                            }
                            else
                            {
                                Tag += ", ";
                            }
                            Tag += Tag_;
                            
                        }

                    }
                    if (Firsttag)
                    {
                        TagVisible = false;
                    }
                    else
                    {
                        TagVisible = true;
                    }
                }
                DatePublished = NF.DatePosted;
                datePub = DatePublished.Day + "/" + DatePublished.Month + "/" + DatePublished.Year;
                DateLength = (datePub.Length * 7);
                TagLength = (FirstTag.Length*7);

                Console.WriteLine("Ping");
                if (!NF.Ingress.Any())
                {
                    HasIngress = false;
                    Ingress = NF.Ingress;
                }
                else
                {
                    var properties = App.Current.Properties;
                    if (properties.ContainsKey("showingress"))
                    {
                        HasIngress = (bool)properties["showingress"];
                    }
                    else
                    {
                        HasIngress = true;
                    }
					
                    Ingress = NF.Ingress;

                    int IL = 40;
                    int IH = 17;

                    if (Ingress.Length < IL)
                    {
                        INGHR = IH;
                    }
                    else if (Ingress.Length < IL * 2)
                    {
                        INGHR = IH * 2;
                    }
                    else 
                    {
                        INGHR = IH * 3;
                    }
                }

                Console.WriteLine("Pong");
                ID = NF.Article;
                Header = NF.Header.Replace("*", "-").Replace("&quot;", "'"); ;
                IMGSource = NF.Image;
                Full = true;
                CategoryBig = true;
                CategorySmall = false;


                Plus = Convert.ToBoolean(NF.Plus);

                int BL = 100;
                int BH = 60;

                if (Header.Length < BL)
                {
                    HeaderLength = BH;
                }
                else if (Header.Length < BL * 2)
                {
                    HeaderLength = BH * 2;
                }
                else if (Header.Length < BL * 3)
                {
                    HeaderLength = BH * 3;
                }

                if (IMGSource.ToString() == Defaultimage)
                {
                    IMGSource = "";
                    IHR = 1;
                    CBWR = 7;
                    CBHR = HeaderLength;
                    BHR = HeaderLength;
                    CBHO = LayoutOptions.Start;
                    CBVO = LayoutOptions.FillAndExpand;
                    Full = false;
                    CategoryBig = false;
                    CategorySmall = true;
                }
                else
                {
                    IHR = 220;
                    BHR = 200 + HeaderLength;
                    CBWR = 200;
                    CBHR = 7;
                    CBHO = LayoutOptions.Fill;
                    CBVO = LayoutOptions.StartAndExpand;
                }


                if (interval % 5 == 0 && Full)
                {
                    AdVisibility = true;

                    //Randomizing the content of mockup ads and social media posts

                    int content = rnd.Next(1, 20);  // creates a number between 1 and 20

                    if (content == 1)
                    {
                        //a 1 in 3 chance to become an ad

                        AdText = "Reklam";
                        AdSource = "Commercial_Hamburger.jpg";

                    }
                    else if (content == 2)
                    {
                        //a 1 in 3 chance to become a social media post

                        AdText = "Användarbilder";
                        AdSource = "Commercial_Social.png";

                    }
                    else if (content == 3)
                    {
                        //a 1 in 3 chance to become an event

                        AdText = "Användarbilder";
                        AdSource = "reklamtest6.jpg";

                    }
                    else if (content == 4)
                    {
                        //a 1 in 3 chance to become an event

                        AdText = "Användarbilder";
                        AdSource = "reklamtest.jpg";

                    }
                    else if (content == 5)
                    {
                        //a 1 in 3 chance to become an event

                        AdText = "Reklam";
                        AdSource = "Reklam_2.jpg";

                    }
                    else if (content == 6)
                    {
                        //a 1 in 3 chance to become an event

                        AdText = "Användarbilder";
                        AdSource = "reklamtest2.jpg";

                    }
                    else if (content == 7)
                    {
                        //a 1 in 3 chance to become an event

                        AdText = "Användarbilder";
                        AdSource = "reklamtest3.jpg";

                    }
                    else if (content == 8)
                    {
                        //a 1 in 3 chance to become an event

                        AdText = "Användarbilder";
                        AdSource = "reklamtest4.jpg";

                    }
                    else if (content == 9)
                    {
                        //a 1 in 3 chance to become an event

                        AdText = "Användarbilder";
                        AdSource = "reklamtest5.jpg";

                    }
                    else if (content == 10)
                    {
                        //a 1 in 3 chance to become an event

                        AdText = "Reklam";
                        AdSource = "Reklam_1.jpg";

                    }
                    else if (content == 11)
                    {
                        //a 1 in 3 chance to become an event

                        AdText = "Reklam";
                        AdSource = "Reklam_3.jpg";

                    }
                    else if (content == 12)
                    {
                        //a 1 in 3 chance to become an event

                        AdText = "Reklam";
                        AdSource = "Reklam_4.jpg";

                    }
                    else if (content == 13)
                    {
                        //a 1 in 3 chance to become an event

                        AdText = "Reklam";
                        AdSource = "Reklam_5.jpg";

                    }
                    else if (content == 14)
                    {
                        //a 1 in 3 chance to become an event

                        AdText = "Reklam";
                        AdSource = "Reklam_6.jpg";

                    }
                    else
                    {
                        //else there is no extra post
                        AdVisibility = false;
                    }

                } else
                {
                    AdVisibility = false;
                }

                ArticleReactions = NF.ArtikelReactions;
                ReactionSum = NF.ReactionSum;

                var ReactionList = JsonConvert.DeserializeObject<List<ReactionTable>>(ReactionSum);

                foreach (ReactionTable Reaction in ReactionList)
                {
                    if (Reaction.User == App.LoggedinUser.ID)
                    {
                        //How you reacted
                        ReactionSrc = "reactions_" + Reaction.Reaktion + ".png";

                        //How other reacted (currently set to default)
                        ReactionSrcOthers = "reactions_0.png";
                        break;
                    }
                    else
                    {
                        ReactionSrc = "reactions_gray.png";
                        ReactionSrcOthers = "reactions_0.png";
                    }

                }
                int MaxValue = 0;
                int MostPicked = 0;

                for (int i = 0; i < 8; i++)
                {
                    int CurValue = 0;
                    foreach (ReactionTable Reaction in ReactionList)
                    {
                        if (Reaction.Reaktion == i)
                        {
                            CurValue++;
                        }
                    }
                    if (CurValue > MaxValue)
                    {
                        MaxValue = CurValue;
                        MostPicked = i;
                    }
                }
                ReactionSrcOthers = "reactions_" + MostPicked + ".png";
                ReactionNR = MaxValue;
                Console.WriteLine("Artikel Klar");
            }
        }

		// Updates the feed if the tags have been modified.
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (App.LoggedinUser != null)
            {
                Console.WriteLine("OnAppearing");
                if (TagsModified)
                {
                    App.Startpage.IsGestureEnabled = false;
                    Console.WriteLine("Tags modifing, reloading feed.");
                    TagUpdate();
                    TagsModified = false;
                    App.Startpage.IsGestureEnabled = true;
                }
            }           
        }

		// Initializing the feed by loading in the article objects.
        public CustomNewsFeed()
        {
            InitializeComponent();
            TGR = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            Console.WriteLine("NewsGrid");
            TGR.Tapped += (s, e) => {
                IsEnabled = false;
                LoadNews(s, e);
                IsEnabled = true;
            };
        }

        public void CreateFeed()
        {
            LoadLocalDB();
            AddNews();
        }

		// Load in a NewsPage when an article is clicked on in the feed.
        public void ArtikelClicked(object sender, EventArgs e)
        {
            var Header = (View)sender;
            Console.WriteLine(Header.ClassId);
            Console.WriteLine(App.Mainpage.Children[0].Navigation.NavigationStack[1].GetType());
            App.Mainpage.Children[0].Navigation.NavigationStack[1].ClassId = Header.ClassId;
            Navigation.PopAsync();
        }

        async void ReactionButtonClicked(object sender, System.EventArgs e)
        {
            Button b = (Button)sender;
            string ID = b.ClassId;
            ReactionPopUp rp = new ReactionPopUp(0, false, null, null);
            await PopupNavigation.Instance.PushAsync(rp);
        }

		// Updates the feed based on changes to the user's selected tags, categories, and authors. 
        public async void TagUpdate()
        {
            App.database.LocalExecute("DELETE FROM CNF");

            PREV = 0;
            CURR = NewsGridPage.DBLN;
            NEXT = NewsGridPage.DBLN * 2;
            Loadnr = 1;
            var TagList = JsonConvert.DeserializeObject<List<List<string>>>(App.LoggedinUser.TaggString);
            Console.WriteLine("CNF Taggupdate, Deserializing Taglist.");

            if(TagList != null)
            {
                Filter = TagList[0];
                Tag = TagList[1];
                Author = TagList[2];
                ArticleList.Clear();
                //Console.WriteLine("CNF Taggupdate, Starting Task. Filter: " + Filter.First());
                await System.Threading.Tasks.Task.Run(async () =>
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        //ArticleListView.IsRefreshing = true;
                        App.LS.loadingAnimation.Play();
                        await Navigation.PushAsync(App.LS);

                        App.LS.LoadingText.Text = "Uppdaterar Nyhetsflödet för nya taggar.";

                    });

                    Console.WriteLine("CNF Taggupdate, Popup Made.");
                    LoadLocalDB();
                    Console.WriteLine("CNF Taggupdate, Database Loaded.");
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        AddNews();

                        Console.WriteLine("CNF Taggupdate, Listview Skapad.");
                        ArticleListView.ItemsSource = null;
                        ArticleListView.ItemsSource = ArticleList;

                        Console.WriteLine("CNF Taggupdate, ArticleList Refreshad.");

                    });


                    GC.Collect();


                    await System.Threading.Tasks.Task.Delay(10);
                    Console.WriteLine("CNF Taggupdate, Uhhhhh.");
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        Console.WriteLine("Initiering Klar");
                        await Navigation.PopAsync();
                    });

                });
            }
        }

		// Fetches the RSS feed of news from the database and uses it to load in a news page.
        async void LoadNews(object sender, EventArgs e)
        {
            var Header = (Button)sender;
            Header.IsEnabled = false;
            var id = Int32.Parse(Header.ClassId);
            var RSSTable = App.database.GetServerRSS(id);
            if (RSSTable != null)
            {
                RSSTable RSS = RSSTable.First();
                await Navigation.PushAsync(new NewsPage(RSS, argc));
            }
            else
            {
                await DisplayAlert("Article Load Failure", "Artikeln misslyckades att laddas in, vänligen försök igen.", "OK");
            }
            Header.IsEnabled = true;
        }


		// Loads in the database in batches if possible.
        public void LoadLocalDB()
        {
            var AL = App.database.BatchLoadCNF(Loadnr, (Loadnr + DBLN), Filter, Author, Tag);
			
            if(AL == 0)
            {
            AL = App.database.LoadCNF(Loadnr, (Loadnr + DBLN), Filter, Author, Tag);
            }
			
            Loadnr += DBLN;
        }

		// Creates the feed by creating a clickable element for each article. How many are loaded in is restricted by the user's settings.
        public void PrintNews()
        {
            int Start = 0;
            int End = 0;
            Rownr = 1;
            Start = PREV;
            End = CURR;
            NewsGrid.Children.Clear();
            Console.WriteLine("Grid Rensat.");
			
            ArticleListView = new ListView
            {
                // Source of data items.

                ItemsSource = ArticlePrintList,
                HasUnevenRows = true,
                SeparatorVisibility = SeparatorVisibility.None,

                // Define template for displaying each item.
                // (Argument of DataTemplate constructor is called for each item; it must return a Cell derivative.)
                ItemTemplate = new DataTemplate(() =>
                {
                    // Create views with bindings for displaying each property.
                    int IMGXC = 200;

                    Label Label = new Label
                    {
                        HorizontalTextAlignment = TextAlignment.Start,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontSize = 22,
                        FontAttributes = FontAttributes.Bold,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        TextColor = Color.Black,
                        InputTransparent = true,
                        Margin = new Thickness(15, 5, 15, 0),
                    };

                    Label.SetBinding(HeightRequestProperty, "HeaderLength");

                    var Image = new CachedImage()
                    {
                        WidthRequest = IMGXC,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        Aspect = Aspect.AspectFill,
                        InputTransparent = true,
                        CacheDuration = TimeSpan.FromDays(14),
                        DownsampleToViewSize = true,
                        RetryCount = 1,
                        RetryDelay = 250,
                        BitmapOptimizations = false,
                        LoadingPlaceholder = "",
                        ErrorPlaceholder = "failed_load.png",
                    };

                    Image.SetBinding(HeightRequestProperty, "IHR");

                    Button Box = new Button
                    {
                        BackgroundColor = Color.White,
                        WidthRequest = IMGXC,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        BorderColor = Color.FromHex("#f0f0f0"),
                    };

                    Box.SetBinding(HeightRequestProperty, "BHR");

                    if (argc == 1)
                    {
                        Box.Clicked += ArtikelClicked;
                    }
                    else
                    {
                        Box.Clicked += LoadNews;
                    }


                    BoxView ArticleMargin = new BoxView
                    {
                        Color = Color.FromHex("#f2f2f2"),
                        WidthRequest = IMGXC,
                        HeightRequest = 10,
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill,
                        InputTransparent = true,
                    };

                    BoxView CategoryBox = new BoxView
                    {
                        BackgroundColor = App.MC,
                        InputTransparent = true,
                    };
                    BoxView CategoryBoxSmall = new BoxView
                    {
                        BackgroundColor = App.MC,
                        InputTransparent = true,
                    };

                    Image Shadow = new Image
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Fill,
                        InputTransparent = true,
                        Source = "shadow.png"
                    };
                    Image WhiteShadow = new Image
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.End,
                        InputTransparent = true,
                        Source = "whiteShadow.png"
                    };
                    Image AdShadow = new Image
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Fill,
                        InputTransparent = true,
                        Source = "shadow.png"
                    };
                    BoxView AdArticleMargin = new BoxView
                    {
                        Color = Color.FromHex("#f2f2f2"),
                        WidthRequest = IMGXC,
                        HeightRequest = 10,
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill,
                        InputTransparent = true,
                    };

                    Label Tag = new Label
                    {
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontSize = 12,
                        FontAttributes = FontAttributes.Bold,
                        VerticalOptions = LayoutOptions.End,
                        HorizontalOptions = LayoutOptions.Start,
                        TextColor = Color.White,
                        InputTransparent = true,
                        Margin = new Thickness(15, 5, 15, 5),
                    };

                    Button TagBox = new Button
                    {
                        BackgroundColor = App.MC,
                        VerticalOptions = LayoutOptions.End,
                        HorizontalOptions = LayoutOptions.Start,
                        HeightRequest = 16,
                        Margin = new Thickness(13, 5, 13, 5),
                    };

                    TagBox.Clicked += TagPopup;
					
                    Label Date = new Label
                    {
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontSize = 12,
                        FontAttributes = FontAttributes.Bold,
                        VerticalOptions = LayoutOptions.End,
                        HorizontalOptions = LayoutOptions.End,
                        TextColor = Color.Black,
                        InputTransparent = true,
                        Margin = new Thickness(15, 5, 15, 5),
                    };

                    BoxView DateBox = new BoxView
                    {
                        BackgroundColor = App.MC,
                        VerticalOptions = LayoutOptions.End,
                        HorizontalOptions = LayoutOptions.End,
                        HeightRequest = 16,
                        Margin = new Thickness(13, 5, 10, 5),
                    };

                    Label IngressLabel = new Label
                    {
                        HorizontalTextAlignment = TextAlignment.Start,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontSize = 14,
                        FontAttributes = FontAttributes.Bold,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        HeightRequest = 54,
                        TextColor = Color.Gray,
                        InputTransparent = true,
                        Margin = new Thickness(15, 5, 15, 0),
                    };


                    var AdImage = new CachedImage
                    {
                        BackgroundColor = Color.White,
                        WidthRequest = IMGXC,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Fill,
                        Aspect = Aspect.AspectFill,
                        InputTransparent = true,
                        Margin = 30,
                        CacheDuration = TimeSpan.FromDays(14),
                        DownsampleToViewSize = true,
                        RetryCount = 1,
                        RetryDelay = 250,
                        BitmapOptimizations = false,
                        LoadingPlaceholder = "",
                        ErrorPlaceholder = "failed_load.png",
                    };
                    BoxView AdImageOutline = new BoxView
                    {
                        BackgroundColor = Color.White,
                        WidthRequest = IMGXC,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Fill,
                        Margin = 35,

                    };
                    AdImage.SetBinding(HeightRequestProperty, "IHR");

                    BoxView AdBox = new BoxView
                    {
                        BackgroundColor = Color.FromHex("#fffef2"),
                        WidthRequest = IMGXC,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Fill,
                        Margin = 0

                    };

                    Label AdLabel = new Label
                    {
                        Text = "Reklam",
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontSize = 12,
                        FontAttributes = FontAttributes.Bold,
                        VerticalOptions = LayoutOptions.Start,
                        HorizontalOptions = LayoutOptions.End,

                        TextColor = Color.LightGray,
                        InputTransparent = true,
                        Margin = new Thickness(5, 5, 5, 5),
                    };

                    //Reaction stuff
                    var ReactionButton = new Button
                    {
                        CornerRadius = 0,
                        BorderWidth = 0,
                        Margin = 0,
                        BackgroundColor = Color.Transparent,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        WidthRequest = 60,
                        HeightRequest = 60,
                        TextColor = Color.LightGray,
                        ClassId = "ArticleID",

                    };
                    ReactionButton.Clicked += ReactionButtonClicked;
                    var ReactionImage = new CachedImage
                    {
                        Margin = 15,
                        BackgroundColor = Color.Transparent,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        WidthRequest = 45,
                        HeightRequest = 45,
                        Source = "reactions_gray",

                    };
                    var ReactionButtonText = new Label
                    {
                        Text = "Reagera",
                        Margin = 0,
                        FontSize = 18,
                        TextColor = Color.DimGray,
                        BackgroundColor = Color.Transparent,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                    };
                    var ShareButton = new Image
                    {
                        Margin = 20,
                        BackgroundColor = Color.Transparent,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        WidthRequest = 40,
                        HeightRequest = 40,
                        Source = "icon_Share",

                    };
                    var ShareButtonText = new Label
                    {
                        Text = "Dela",
                        Margin = 0,
                        FontSize = 18,
                        TextColor = Color.DimGray,
                        BackgroundColor = Color.Transparent,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                    };
                    var ReactionBar1 = new BoxView
                    {
                        CornerRadius = 1,
                        Margin = 10,
                        BackgroundColor = Color.Red,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        VerticalOptions = LayoutOptions.End,
                        WidthRequest = Application.Current.MainPage.Width / 2 * 1f,
                        HeightRequest = 5,
                    };
                    var ReactionBar2 = new BoxView
                    {
                        CornerRadius = 1,
                        Margin = 10,
                        BackgroundColor = Color.Blue,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        VerticalOptions = LayoutOptions.End,
                        WidthRequest = Application.Current.MainPage.Width / 2 * 0.4f,
                        HeightRequest = 5,
                    };
                    var ReactionBar3 = new BoxView
                    {
                        CornerRadius = 1,
                        Margin = 10,
                        BackgroundColor = Color.Yellow,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        VerticalOptions = LayoutOptions.End,
                        WidthRequest = Application.Current.MainPage.Width / 2 * 0.1f,
                        HeightRequest = 5,
                    };
                    var ReactionsOthers1 = new CachedImage
                    {
                        Source = "reactions_0",
                        Margin = 10,
                        BackgroundColor = Color.Transparent,
                        HorizontalOptions = LayoutOptions.End,
                        VerticalOptions = LayoutOptions.Center,
                        WidthRequest = 30,
                        HeightRequest = 30,
                    };
                    var ReactionsOthers2 = new Image
                    {
                        Source = "reactions_7",
                        Margin = 10,
                        BackgroundColor = Color.Transparent,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        WidthRequest = 30,
                        HeightRequest = 30,
                    };
                    var ReactionsOthers3 = new Image
                    {
                        Source = "reactions_4",
                        Margin = 10,
                        BackgroundColor = Color.Transparent,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        WidthRequest = 30,
                        HeightRequest = 30,
                    };
                    var ReactionsOthersText = new Label
                    {
                        Text = "X",
                        Margin = 10,
                        FontSize = 20,
                        TextColor = Color.DimGray,
                        BackgroundColor = Color.Transparent,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.End,
                        WidthRequest = 30,
                        HeightRequest = 30,
                    };

                    AdBox.SetBinding(HeightRequestProperty, "BHR");

                    CategoryBox.SetBinding(IsVisibleProperty, "CategoryBig");
                    CategoryBox.SetBinding(HeightRequestProperty, "CBHR");
                    CategoryBox.SetBinding(WidthRequestProperty, "CBWR");
                    CategoryBox.SetBinding(BoxView.VerticalOptionsProperty, "CBVO");
                    CategoryBox.SetBinding(BoxView.HorizontalOptionsProperty, "CBHO");
                    CategoryBox.SetBinding(BoxView.BackgroundColorProperty, "CategoryColor");

                    CategoryBoxSmall.SetBinding(IsVisibleProperty, "CategorySmall");
                    CategoryBoxSmall.SetBinding(HeightRequestProperty, "CBHR");
                    CategoryBoxSmall.SetBinding(WidthRequestProperty, "CBWR");
                    CategoryBoxSmall.SetBinding(BoxView.VerticalOptionsProperty, "CBVO");
                    CategoryBoxSmall.SetBinding(BoxView.HorizontalOptionsProperty, "CBHO");
                    CategoryBoxSmall.SetBinding(BoxView.BackgroundColorProperty, "CategoryColor");

                    Label.SetBinding(Label.TextProperty, "Header");
                    Image.SetBinding(CachedImage.SourceProperty, "IMGSource");

                    Tag.SetBinding(Label.TextProperty, "FirstTag");
                    Tag.SetBinding(Label.IsVisibleProperty, "TagVisible");

                    TagBox.SetBinding(Button.IsVisibleProperty, "TagVisible");
                    TagBox.SetBinding(Button.WidthRequestProperty, "TagLength");
                    TagBox.SetBinding(Button.ClassIdProperty, "Tag");

                    Date.SetBinding(Label.TextProperty, "datePub");

                    DateBox.SetBinding(BoxView.WidthRequestProperty, "DateLength");
                    
                    IngressLabel.SetBinding(Label.TextProperty, "Ingress");
                    IngressLabel.SetBinding(Label.IsVisibleProperty, "HasIngress");
                    IngressLabel.SetBinding(HeightRequestProperty, "INGHR");

                    Label.SetBinding(Label.ClassIdProperty, "ID");
                    Image.SetBinding(CachedImage.ClassIdProperty, "ID");
                    Box.SetBinding(Button.ClassIdProperty, "ID");
                    ArticleMargin.SetBinding(BoxView.ClassIdProperty, "ID");

                    AdArticleMargin.SetBinding(BoxView.IsVisibleProperty, "AdVisibility");
                    AdShadow.SetBinding(BoxView.IsVisibleProperty, "AdVisibility");
                    AdBox.SetBinding(BoxView.IsVisibleProperty, "AdVisibility");
                    AdImageOutline.SetBinding(BoxView.IsVisibleProperty, "AdVisibility");
                    AdImage.SetBinding(CachedImage.IsVisibleProperty, "AdVisibility");
                    AdLabel.SetBinding(Label.IsVisibleProperty, "AdVisibility");
                    AdLabel.SetBinding(Label.TextProperty, "AdText");
                    AdImage.SetBinding(CachedImage.SourceProperty, "AdSource");

                    ReactionButton.SetBinding(ClassIdProperty, "ID");
                    ReactionsOthersText.SetBinding(Label.TextProperty, "ReactionNR");
                    ReactionImage.SetBinding(CachedImage.SourceProperty, "ReactionSrc");
                    ReactionsOthers1.SetBinding(CachedImage.SourceProperty, "ReactionSrcOthers");

                    var Grid = new Grid
                    {
                        RowDefinitions = {
                    new RowDefinition { Height = 0 },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = 10 },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = 10 },
                    },
                        ColumnDefinitions = {
                    new ColumnDefinition { Width = 0 },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 0 },
                    },
                        RowSpacing = 0,
                        ColumnSpacing = 0,
                        BackgroundColor = Color.FromHex("#f2f2f2")
                    };
					
                    Label.WidthRequest = Label.Width - 25;

                    Grid.Children.Add(ArticleMargin, 1, 18, 0, 1); //Boxview
                    Grid.Children.Add(Box, 1, 18, 1, 5); //Boxview
                    Grid.Children.Add(Image, 1, 18, 1, 2); //Image   
                    Grid.Children.Add(CategoryBox, 1, 18, 2, 3); //Label
                    Grid.Children.Add(CategoryBoxSmall, 1, 18, 1, 5); //Label
                    Grid.Children.Add(Label, 1, 18, 2, 3); //Label
                    Grid.Children.Add(IngressLabel, 1, 18, 3, 4); //Label
                    Grid.Children.Add(Date, 1, 18, 4, 5); //Tag 
                    Grid.Children.Add(Shadow, 1, 18, 5, 6);
                    Grid.Children.Add(TagBox, 1, 18, 1, 2); //Tag
                    Grid.Children.Add(Tag, 1, 18, 1, 2); //Tag   

                    Grid.Children.Add(ReactionButton, 1, 7, 4, 5); //Reaction   
                    Grid.Children.Add(ReactionButtonText, 3, 8, 4, 5); //Reaction   
                    Grid.Children.Add(ReactionImage, 1, 5, 4, 5); //Reaction   
                    Grid.Children.Add(ShareButton, 8, 12, 4, 5); //Reaction   
                    Grid.Children.Add(ShareButtonText, 9, 15, 4, 5); //Reaction  
                    Grid.Children.Add(ReactionsOthers1, 10, 18, 4, 5); //Reaction   
                    Grid.Children.Add(ReactionsOthersText, 13, 18, 4, 5); //Reaction  
                    Console.WriteLine("Utdata: " + Label.Text);

                    // Return an assembled ViewCell.
                    return new ViewCell
                    {
                        View = Grid
                    };
                })
            };
            NewsGrid.Children.Add(ArticleListView, 0, 3, 1, 2);
            NewsGrid.Children.Add(Down, 0, 3, 2, 3);
            First = false;
        }
		
		// A popup showing which tags, categories and/or authors allows the article to be shown in the feed.
        async void TagPopup(object sender, EventArgs e)
        {
            var Header = (Button)sender;
            Header.IsEnabled = false;
            String tagstring = Header.ClassId;
            await DisplayAlert("Tags", "Tags in this Article that you Follow: \n" + tagstring, "Okay");
            Header.IsEnabled = true;
        }

        private void Box_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

		// Adding the boxes that contains the article information, are can be clicked to open that article's news page.
        public void AddNews()
        {

            List<NewsfeedTable> Rss = App.database.GetCNF(Loadnr);
            
            Console.WriteLine(Rss.Count);
            Console.WriteLine("Inladdning Klar");
            int i = 1;

            foreach (NewsfeedTable NF in Rss)
            {
                bool Exists = false;
                App.Online = true;
                foreach (var Article in ArticleList)
                {
                    if (Article.ID == NF.Article)
                    {
                        Exists = true;
                    }
                }
                Console.WriteLine("Jämnförelse ned ArtikelLista klar");
                if (!Exists)
                {
                    var Box = new Article(NF, interval);

                    interval++;

                    Console.WriteLine("ArtikelObjekt Skapat");
                    ArticleList.Add(Box);
                }
                i++;
            }


            ArticlePrintList.Clear();
            for (int j = PREV; j < CURR; j++)
            {
                ArticlePrintList.Add(ArticleList[j]);
            }
            if(First && App.Online)
            {
                PrintNews();
            }
            
        }

		// When the user expands the feed beyond the initial load in.
        public async void ListViewScroll(object sender, EventArgs e)
        {
            Down.IsEnabled = false;
            ArticleListView.IsEnabled = false;
            NewsGrid.IsEnabled = false;
            await System.Threading.Tasks.Task.Run(async () =>
            {
                if (argc == 0)
                {
                    PREV = 0;
                    CURR = NEXT;
                    NEXT += DBLN;

                    Console.WriteLine("PREV: " + PREV + " CURR: " + CURR + " NEXT: " + NEXT);
                    double height = NewsSV.ContentSize.Height - 10;
                    LoadLocalDB();
                    AddNews();
                }
                Device.BeginInvokeOnMainThread( () =>
                {
                    ArticleListView.ItemsSource = null;
                    ArticleListView.ItemsSource = ArticleList;
                });

                GC.Collect();

                Device.BeginInvokeOnMainThread(async () =>
                {
                    Console.WriteLine("Initiering Klar");

                    await Navigation.PopAsync();
                    await NewsSV.ScrollToAsync(0, ArticleListView.Height - 10, false);
                    App.Startpage.IsGestureEnabled = true;
                });
                await System.Threading.Tasks.Task.Delay(5);
            });
            ArticleListView.IsEnabled = true;
            Down.IsEnabled = true;
            NewsGrid.IsEnabled = true;
        }
		
		//To make sure the user does not turn off the app accidentally, the return button is inactive on the four main view pages.
		
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}