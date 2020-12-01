using FFImageLoading.Forms;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NWT
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsGridPage : ContentPage
    {
		
		/*
		
			The NewsGridPage generates a list of Article objects based on your selections in the sidemenu.
			If the user selects a certain tag, then only articles containing that tag will be shown.
			
			These objects lead to newsPage, where in each objects content will be shown to the user.
			
		*/
		
        public int Loadnr = 1;
        public static int DBLN = 20; //Database Load Nr
        public int Rownr = 1;
        public static TapGestureRecognizer TGR;
        public List<Article> ArticleList = new List<Article>();
        public List<Article> ArticlePrintList = new List<Article>();
        public static string Defaultimage = "http://media2.hitzfm.nu/2016/11/Nyheter_3472x1074.jpg";
        public static Random rnd = new Random();
        public string Filter = "All";
        public string Author = "";
        public string Tag = "";
        public ListView ArticleListView;
        public int PREV = 0;
        public int CURR = DBLN;
        public int NEXT = DBLN * 2;
        public bool ScrollLock = false;
        public string EmptyList = "";

        public bool First = true;
        public int argc = 0;

		// The Article object defined and holds all data required to load in a full article. This is used in the news feeds, as well as the NewsPage in order to display articles to the user.
        public class Article
        {
            public long ID { get; set; }
            public string Source { get; set; }
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
            public bool Plus { get; set; }
            public string ArticleReactions { get; set; }
            public string ReactionSum { get; set; }
            public bool Full { get; set; }
            public bool CategoryBig { get; set; }
            public bool CategorySmall { get; set; }
            public int IHR { get; set; }          
            public int BHR { get; set; }
            public int CBWR { get; set; }
            public int CBHR { get; set; }
            public LayoutOptions CBHO { get; set; }
            public LayoutOptions CBVO { get; set; }
            public int INGHR { get; set; }
            public bool IsNormalfeed { get; set; }
            public int ReactionNR { get; set; }
            public string ReactionSrc { get; set; }
            public string ReactionSrcOthers { get; set; }

            public Article(NewsfeedTable NF, int argc)
            {
                ReactionNR = 1337;

                Tag = NF.Category.Split(new[] { ", " }, StringSplitOptions.None)[0];
                ID = NF.Article;
                Header = NF.Header.Replace("*", "-").Replace("&quot;", "'");
                IMGSource = NF.Image;
                if(!NF.Ingress.Any()) // If the ingress is empty, skip construction otherwise make a window whose size is based on the length of the Ingress.
                {
                    HasIngress = false;
                    Ingress = NF.Ingress;
                }
                else
                {
                    
                    Ingress = NF.Ingress;
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
                    else if (Ingress.Length < IL * 3)
                    {
                        INGHR = IH * 3;
                    }

                    Ingress = "● " + Ingress + "...";
                }

                DatePublished = NF.DatePosted;
                
                datePub = DatePublished.Day + "/" + DatePublished.Month + "/" + DatePublished.Year;

                DateLength = (datePub.Length * 7);

                Full = true;
                CategoryBig = true;
                CategorySmall = false;

                if (NF.Category == "")
                {
                   
                }
                TagLength = (Tag.Length * 7);

                Plus = Convert.ToBoolean(NF.Plus);

                int BL = 100;
                int BH = 60;

                if(Header.Length < BL)
                {
                    HeaderLength = BH;
                }
                else if (Header.Length < BL*2)
                {
                    HeaderLength = BH*2;
                }
                else if (Header.Length < BL*3)
                {
                    HeaderLength = BH*3;
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
                    INGHR = 36;
                }
                else
                {
                    IHR = 200;
                    BHR = 200 + HeaderLength;
                    CBWR = 200;
                    CBHR = 7;
                    CBHO = LayoutOptions.FillAndExpand;
                    CBVO = LayoutOptions.Start;
                    INGHR = 54;
                }

                if (argc != 0)
                {
                    IsNormalfeed = false;
                    HasIngress = false;
                    TagVisible = false;
                }
                else
                {
                    IsNormalfeed = true;
                }

                ArticleReactions = NF.ArtikelReactions;
                ReactionSum = NF.ReactionSum;

                var ReactionList = JsonConvert.DeserializeObject<List<ReactionTable>>(ReactionSum);

                foreach (ReactionTable Reaction in ReactionList)
                {
                    if (Reaction.User == App.LoggedinUser.ID)
                    {
                        ReactionSrc = "reactions_"+Reaction.Reaktion+".png";
                        break;
                    }
                    else
                    {
                        ReactionSrc = "reactions_gray.png";
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

		// Initializes the page by setting the title.
        public NewsGridPage(int Argc)
        {
            InitializeComponent();

            NewsSV.ClassId = null;

            argc = Argc;
            
            TGR = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };

            if (argc == 0)
            {
                Title = "Mina Val";
            }
            else 
            {
                Title = "Samlade Artiklar";
                CreateFeed(argc);
            }
        }

		// Create the list of element containing article information that open up news pages if clicked on.
        public void CreateListView()
        {
            ArticleListView = new ListView
            {
                // Source of data items.
                
                ItemsSource = ArticleList,
                HasUnevenRows = true,
                SeparatorVisibility = SeparatorVisibility.None,
                BackgroundColor = Color.FromHex("#FFFFFF"),
                Footer = Down,

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
                        RetryCount = 5,
                        RetryDelay = 250,
                        BitmapOptimizations = false,
                        LoadingPlaceholder = "",
                        ErrorPlaceholder = "failed_load.png",
                    };

                    Image.SetBinding(HeightRequestProperty, "IHR");

                    Button Box = new Button
                    {
                        BackgroundColor = Color.Transparent,
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

                    BoxView TagBox = new BoxView
                    {

                        BackgroundColor = App.MC,
                        VerticalOptions = LayoutOptions.End,
                        HorizontalOptions = LayoutOptions.Start,
                        HeightRequest = 16,
                        Margin = new Thickness(10, 5, 13, 5),

                    };

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
                        Text = "RE",
                        TextColor = Color.LightGray,
                        ClassId = "ArticleID",

                    };
                    ReactionButton.Clicked += ReactionButtonClicked;
                    var ReactionImage = new Image
                    {
                        Margin = 15,
                        BackgroundColor = Color.Transparent,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        WidthRequest = 45,
                        HeightRequest = 45,
                        Source = "reactions_gray",

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
                        TextColor = Color.Black,
                        BackgroundColor = Color.Transparent,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        WidthRequest = 30,
                        HeightRequest = 30,
                    };

                    CategoryBox.SetBinding(IsVisibleProperty, "CategoryBig");
                    CategoryBox.SetBinding(HeightRequestProperty, "CBHR");
                    CategoryBox.SetBinding(WidthRequestProperty, "CBWR");
                    CategoryBox.SetBinding(BoxView.VerticalOptionsProperty, "CBVO");
                    CategoryBox.SetBinding(BoxView.HorizontalOptionsProperty, "CBHO");

                    CategoryBoxSmall.SetBinding(IsVisibleProperty, "CategorySmall");
                    CategoryBoxSmall.SetBinding(HeightRequestProperty, "CBHR");
                    CategoryBoxSmall.SetBinding(WidthRequestProperty, "CBWR");
                    CategoryBoxSmall.SetBinding(BoxView.VerticalOptionsProperty, "CBVO");
                    CategoryBoxSmall.SetBinding(BoxView.HorizontalOptionsProperty, "CBHO");

                    Label.SetBinding(Label.TextProperty, "Header");
                    Image.SetBinding(CachedImage.SourceProperty, "IMGSource");

                    Label.SetBinding(Label.ClassIdProperty, "ID");
                    Image.SetBinding(CachedImage.ClassIdProperty, "ID");

                    Box.SetBinding(Button.ClassIdProperty, "ID");

                    ArticleMargin.SetBinding(BoxView.ClassIdProperty, "ID");
                    Tag.SetBinding(Label.TextProperty, "Tag");
                    Tag.SetBinding(Label.IsVisibleProperty, "TagVisible");

                    TagBox.SetBinding(Button.IsVisibleProperty, "TagVisible");
                    TagBox.SetBinding(BoxView.WidthRequestProperty, "TagLength");

                    Date.SetBinding(Label.TextProperty, "datePub");
                    Date.SetBinding(Label.IsVisibleProperty, "IsNormalfeed");
                    DateBox.SetBinding(BoxView.IsVisibleProperty, "IsNormalfeed");
                    DateBox.SetBinding(BoxView.WidthRequestProperty, "DateLength");

                    IngressLabel.SetBinding(Label.TextProperty, "Ingress");
                    IngressLabel.SetBinding(Label.IsVisibleProperty, "HasIngress");
                    IngressLabel.SetBinding(HeightRequestProperty, "INGHR");

                    ReactionButton.SetBinding(ClassIdProperty, "ID");
                    ReactionsOthersText.SetBinding(Label.TextProperty, "ReactionNR");
                    ReactionsOthers1.SetBinding(CachedImage.SourceProperty, "ReactionSrc");

                    var Grid = new Grid
                    {

                        RowDefinitions = {
                    new RowDefinition { Height = 0 },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
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

                    Grid.Children.Add(ReactionButton, 1, 5, 4, 5); //Reaction   
                    Grid.Children.Add(ReactionImage, 1, 5, 4, 5); //Reaction   
                    Grid.Children.Add(ShareButton, 7, 12, 4, 5); //Reaction   
                    Grid.Children.Add(ReactionsOthers1, 10, 18, 4, 5); //Reaction   
                    Grid.Children.Add(ReactionsOthersText, 10, 18, 4, 5); //Reaction  
                    Console.WriteLine("Utdata: " + Label.Text);

                    // Return an assembled ViewCell.
                    return new ViewCell
                    {
                        View = Grid
                    };
                })

            };

        }


		// Initializes the generation of the newsfeed. Can generate different variants, depending on how the page is loaded.
        public void CreateFeed(int Argc)
        {
            if (Argc == 0) // If the NewsGrid is loaded normally after logging in
            {
                Console.WriteLine("NewsGrid");
                TGR.Tapped += (s, e) => {
                    IsEnabled = false;
                    LoadNews(s, e);
                    IsEnabled = true;
                };
                Device.BeginInvokeOnMainThread(() =>
                {
                    App.LS.LoadingText.Text = "Laddar in Dina Val, Hämtar nyheter ifrån servern";
                });



                LoadLocalDB();


                Device.BeginInvokeOnMainThread(() =>
                {
                    App.LS.LoadingText.Text = "Laddar in Dina Val, Skapar Nyhetslistan";
                });

                EmptyList = "Hej! Klicka på ett ämne i sidomenyn. Då kommer det valda ämnet att visas på denna sida!";

                AddNews(0);

            }
            else if (Argc == 1) //If the newsfeed is loaded through the user created articles creator.
            {
                
                Console.WriteLine("ReferatSida");
                TGR.Tapped += (s, e) => {
                    IsEnabled = false;
                    var Header = (View)s;
                    Console.WriteLine(Header.ClassId);
                    Console.WriteLine(App.Mainpage.Children[0].Navigation.NavigationStack[1].GetType());
                    App.Mainpage.Children[0].Navigation.NavigationStack[1].ClassId = Header.ClassId;

                    Navigation.PopAsync();
                };
                AddNews(0);
            }
            else if (Argc == 2) // If the newsfeed is loaded through the users historypage
            {
                Title = "Historik";
                Console.WriteLine("HistorikSida");
                TGR.Tapped += (s, e) => {
                    IsEnabled = false;
                    LoadNews(s, e);
                    IsEnabled = true;
                };
                EmptyList = "Den här sidan ska visa upp dom senaste artiklar som du har läst. För att läsa en artikel, tryck på knappen 'Samla Token' efter slutet av artikelns brödtext.";
                LoadHistory();
            }
            else if (Argc == 3) // If the newsfeed is loaded through the users favoritespage
            {
                Title = "Favoriter";
                Console.WriteLine("FavoritSida");
                TGR.Tapped += (s, e) => {
                    IsEnabled = false;
                    LoadNews(s, e);
                    IsEnabled = true;
                };

                EmptyList = "Den här sidan ska visa upp dom artiklar du favoritmarkerat. För att favoritmarkera en artikel, tryck på den hjärtformade knappen efter slutet av artikelns brödtext.";
                LoadFavorites();
            }

            if (!ArticleList.Any())
            {
                EmptyText.Text = EmptyList;
                NewsGrid.Children.Add(EmptyText, 1, 1);
            }
            else
            {
                EmptyText.IsVisible = false;
            }
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
        
        async void ReactionButtonClicked(object sender, System.EventArgs e) //Sends an reaction object to the server on this article based on what the user pressed on.
        {
            Button b = (Button)sender;

            var ID = Convert.ToInt32(b.ClassId);

            var UserReactions = App.database.GetReactionsFromUser(App.LoggedinUser.ID);
            bool Reacted = false;
            ReactionTable CR = null;

            foreach (ReactionTable Reaction in UserReactions)
            {
                if (Reaction.Article == ID)
                {
                    Reacted = true;
                    CR = Reaction;
                    break;
                }
            }
            ReactionPopUp rp = new ReactionPopUp(ID,Reacted,CR, null);
            await PopupNavigation.Instance.PushAsync(rp);
        }

		// Fetches the RSS object of an article from the database and uses it to load in a news page.
        async void LoadNews(object sender, EventArgs e)
        {
            bool LoadFailure = false;
            var Header = (Button)sender;
            Header.IsEnabled = false;
            var id = Int32.Parse(Header.ClassId);

            await System.Threading.Tasks.Task.Run(async () =>
            {
                var NP = new ContentPage();
                
                bool OK = false;
                
                Device.BeginInvokeOnMainThread(async () =>
                {
                    //IsBusy = true;
                    
                    LoadingPopUp x = new LoadingPopUp();
                    x.loadingAnimation.Play();
                    await Navigation.PushAsync(x);
                    
                    
                });
				
                var RSSTable = App.database.GetServerRSS(id);
                if (RSSTable != null)
                {
                    RSSTable RSS = RSSTable.First();
                    Device.BeginInvokeOnMainThread( () =>
                    {
                        NP = new NewsPage(RSS, argc);
                        OK = true;
                    });
                }
                else
                {
                    LoadFailure = true;
                }
				
                await System.Threading.Tasks.Task.Delay(10);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    Console.WriteLine("Initiering Klar");                   
                    
                    await Navigation.PopAsync();
                    if(OK)
                    {
                        await Navigation.PushAsync(NP);
                    }
                });

            });
			
            Header.IsEnabled = true;
            if (LoadFailure)
            {
                await DisplayAlert("Article Load Failure", "Artikeln misslyckades att laddas in, vänligen försök igen.", "OK");
            }
        }

        public void LoadHistory()
        {
            AddNews(1);
        }

        public void LoadFavorites()
        {
            AddNews(2);
        }

        public void LoadLocalDB()//Fetches newsfeed objects from the server from Loadnr to Loadnr+DBLN taken in order of newest submitted having ID = 0
        {
            var AL = App.database.BatchLoadNF(Loadnr, (Loadnr + DBLN),Filter, Author, Tag);
            if (AL == 0)
            {
                AL = App.database.LoadNF(Loadnr, (Loadnr + DBLN), Filter, Author, Tag);
            }
            Loadnr += DBLN;
        }

		// Creates the feed by creating a clickable element for each article. How many are loaded in is restricted by the user's settings.
        public void PrintNews()
        {
            Rownr = 1;
            NewsGrid.Children.Clear();
            CreateListView();

            NewsGrid.Children.Add(ArticleListView, 0, 3, 1, 2);
            First = false;
        }

        private void Box_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

		// Adding the boxes that contains the article information, are can be clicked to open that article's news page.
        public void AddNews(int Argc)
        {
            List<NewsfeedTable> Rss = new List<NewsfeedTable>();
            if (Argc == 0)// If loaded normally
            {
                Rss = App.database.GetNF(Loadnr);
            }
            else if (Argc == 1)// If loaded from historypage
            {
                Down.IsVisible = false;
                int j = 0;
                var RAL = App.database.GetHistory(App.LoggedinUser.ID);
                Console.WriteLine("History Gotten: " + RAL.Count());

                if(RAL != null)
                {
                    foreach (var RA in RAL)
                    {
                        RA.Image = RA.Image.Substring(4, RA.Image.Length-4);
                        var NF = new NewsfeedTable
                        {
                            ID = 0,
                            NewsScore = 5,
                            Image = RA.Image,
                            Article = RA.Article,
                            Category = "N/A",
                            Header = RA.Header,
                            Ingress = "",
                            DatePosted = DateTime.Now,
                            Plus = 0
                        };
                        Rss.Add(NF);
                        j++;
                        Console.WriteLine("Artikel Inlagd");
                    }
                    CURR = j;
                }
            }
            else if (Argc == 2) //If Loaded from favorites page
            {
                Down.IsVisible = false;
                int j = 0;
                var FAL = App.database.GetFavorites(App.LoggedinUser.ID);

                if(FAL != null)
                {
                    Console.WriteLine("Favorites Gotten: " + FAL.Count());
                    foreach (var FA in FAL)
                    {
                        FA.Image = FA.Image.Substring(4, FA.Image.Length-4);
                        var NF = new NewsfeedTable
                        {
                            ID = 0,
                            NewsScore = 5,
                            Image = FA.Image,
                            Article = FA.Article,
                            Category = "N/A",
                            Header = FA.Header,
                            Ingress = "",
                            DatePosted = DateTime.Now,
                            Plus = 0
                        };
                        Rss.Add(NF);
                        j++;
                        Console.WriteLine("Artikel Inlagd");
                    }
                    CURR = j;
                }
            }
            else
            {
                Rss = App.database.GetNF(Loadnr);
            }
			
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
                    var Box = new Article(NF,argc);

                    Console.WriteLine("ArtikelObjekt Skapat");
                    ArticleList.Add(Box);
                }
                i++;
                Device.BeginInvokeOnMainThread(() =>
                {
                    App.LS.LoadingText.Text = "Skapar Nyhetslistan. " + i + " Artiklar Skapade.";
                });
                
            }
			
            if (First && App.Online)
            {
                Console.WriteLine("News Added, Creating Listview");
                PrintNews();
                Console.WriteLine("Listview Created");
            }
        }

        public void ChangeName(string newName)
        {
            Title = newName;
        }

		// When the user fetches more articles from the server, to be displayed in the Listview.
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
                    AddNews(argc);
                }
                Device.BeginInvokeOnMainThread(() =>  
                {
                    ArticleListView.ItemsSource = null;
                    ArticleListView.ItemsSource = ArticleList;
                });

                GC.Collect();

                Device.BeginInvokeOnMainThread(async () =>
                {
                    Console.WriteLine("Initiering Klar");
                    await NewsSV.ScrollToAsync(0, ArticleListView.Height - 10, false);
                });              
                await System.Threading.Tasks.Task.Delay(5);
            });
            ArticleListView.IsEnabled = true;
            Down.IsEnabled = true;
            NewsGrid.IsEnabled = true;
        }
		
		//To make sure the user does not turn off the app accidentally, the return button is inactive on the four main view pages. If the page is of favorites or history, then they are allowed to go back.

        protected override bool OnBackButtonPressed()
        {
            if (Title == "Favoriter" || Title == "Historik")
            {
                base.OnBackButtonPressed();
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}