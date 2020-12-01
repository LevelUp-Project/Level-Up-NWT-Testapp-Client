using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;

namespace NWT
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsPage : ContentPage
    {
		/*
		
			This page displays the content of an article selected in the CustomNewsFeed or NewsGridPage.
		
		*/

        public int red = 255;
        public int green = 255;
        public int blue = 255;
        public string timerText = "0%";
        public System.Timers.Timer Timer;
        public static int ArticleNR;
        public int CC = 8;
        public bool Read = false;
        public int Row = 8;
        public bool Topimg = true;
        public bool Favorited = false;
        public ListView CommentListView;
        public List<CommentTable> CommentTableList = new List<CommentTable>();
        public List<Comment> CommentList = new List<Comment>();
        public int LWH = 500;
        public bool CommentsLoaded = false;
        public bool Reacted = false;
        public ReactionTable CR;
        public RSSTable rssTable;
        public Image ReactionImage;
        public Image ReactionsOthers1;
        public Label ReactionsOthersText;

		// Users can write and send in comments to be displayed underneath articles.
        public class Comment
        {
            public UserTable User { get; set; }
            public string UserName { get; set; }
            public List<string> UserAvatar { get; set; }
            public string CommentText { get; set; }
            public int CommentNR { get; set; }           
            public CommentTable CB { get; set; }

            public Comment(CommentTable s, UserTable User)
            {
                CB = s;
                
                UserName = User.Name;
                UserAvatar = JsonConvert.DeserializeObject<List<string>>(User.Avatar);
                CommentText = s.Comment;
                CommentNR = s.CommentNR;
            }
        }

		//The visuals are set up. If the user is logged in, interactions such as coin collecting, history and favorite checks are performed. 
        public NewsPage(RSSTable RSS, int argc)
        {
            InitializeComponent();

            rssTable = RSS; 
            BackGroundReactionTimerFavorites.BackgroundColor = App.MC;
            red = 255 + (int)App.MC.R * 2;
            green = 255 + (int)App.MC.G * 2;
            blue = 255 + (int)App.MC.B * 2;
            FavIcon.BackgroundColor = App.MC;
			
            if (App.LoggedinUser != null && argc == 0) // If the User is logged in and article is opened normally
            {
                bool read = false;
                var History = App.database.GetAllHistory(App.LoggedinUser.ID); //Fetch history from server to check if article is read.

                if(History == null)
                {
                    read = true;
                }
                else
                {
                    foreach (HistoryTable HT in History)
                    {
                        if (RSS.ID == HT.Article)
                        {
                            read = true;
                            break;
                        }
                    }
                }
                          
                if (read)
                {                   
                    TimerButton.IsEnabled = false;
                    TimerIcon.Source = "Icon_Coin.png";
                    TimerButton.Text = "";
                    TimerButton.IsVisible = false;
                    Read = true;
                    NewsPageView.BackgroundColor = App.MC;
                    TimerButton.BackgroundColor = App.MC;
                    TimerButton.TextColor = Color.White;
                                     
                }
                else
                {
                    NewsPageView.BackgroundColor = Color.White;
                    TimerButton.TextColor = Color.White;
                    Timer = new System.Timers.Timer
                    {
                        Interval = 20
                    };
                    Timer.Elapsed += OnTimedEvent;
                    Timer.Enabled = true;
                }
            }
            else
            {
                NewsPageView.BackgroundColor = Color.FromRgb(248, 248, 248);
            }

            if(argc == 1) // If article is loaded through History
            {
                TimerButton.IsVisible = false;
                TimerIcon.IsVisible = false;
                FavIcon.IsVisible = false;
            }
            
            if (argc == 2) // If article is loaded through Favorites
            {
                TimerButton.IsVisible = false;
                TimerIcon.IsVisible = false;
                Favorited = true;
            }
            LoadNews(RSS);
            FavButtonCheck(RSS);
            ReactionCheck();
        }

		//The user can interact with the list of categories, tags, and authors underneath the article.
        void TagSelected(object sender, EventArgs e)
        {
            CommentEntry.IsVisible = false;
            CommentButton.IsVisible = false;
            CommentListView.IsVisible = false;
            TagGrid.IsVisible = true;
        }
		
		//The user can interact with the comments underneath the article.
        void CommentSelected(object sender, EventArgs e)
        {
            CommentEntry.IsVisible = true;
            CommentButton.IsVisible = true;
            CommentListView.IsVisible = true;
            TagGrid.IsVisible = false;
        }
		
		//The article is loaded in, and the article text, date, author, tags, reactions, and comment sections are defined, modified, and added to the page.
        void LoadNews(RSSTable RSS)
        {
            ArticleNR = RSS.ID;
            Rubrik.Text = RSS.Title.Replace("*", "-").Replace("&quot;", "'");
            Ingress.Text = RSS.Description.Replace("*", "-").Replace("&quot;", "'");
            string datePub = RSS.PubDate.ToString();

            for (int i = 0; i < 3; i++)
            {
                datePub = datePub.Remove(datePub.Length - 1);
            }
            Top.Text = datePub + "   "+RSS.Source;
            if (RSS.Author == "Ingen Byline")
            {
                Author.Text = "";
            } else
            {
                Author.TextColor = Color.FromHex("#649FD4");
                Author.Text = "Av: " + RSS.Author;
            }

            Location.TextColor = Color.FromHex("#649FD4");
            switch (RSS.Tag)
            {
                case "Skövde":
                    Location.Text = "Plats: Skövde";
                    break;
                case "Tibro":
                    Location.Text = "Plats: Tibro";
                    break;
                case "Falköping":
                    Location.Text = "Plats: Falköping";
                    break;
                case "Karlsborg":
                    Location.Text = "Plats: Karlsborg";
                    break;
            }
            switch (RSS.Category)
            {
                case "Skövde":
                    Location.Text = "Plats: Skövde";
                    break;
                case "Tibro":
                    Location.Text = "Plats: Tibro";
                    break;
                case "Falköping":
                    Location.Text = "Plats: Falköping";
                    break;
                case "Karlsborg":
                    Location.Text = "Plats: Karlsborg";
                    break;
            }

            Title = RSS.Category;

            if (RSS.Tag.Length > 20)
            {
                //Tags.Text = "";
            }
            else if(RSS.Tag != "")
            {
                //Tags.Text = "Tags: " + RSS.Tag + "  ";
            } else
            {
                //Tags.Text = "";
            }

            var TGR = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            TGR.Tapped += (s, e) => {
                IsEnabled = false;
                IsEnabled = true;
            };
            Author.GestureRecognizers.Add(TGR);

            ArticleImage.Source = RSS.ImgSource;


            var Order = JsonConvert.DeserializeObject<List<int>>(RSS.Ordning);
            var Text = JsonConvert.DeserializeObject<List<string>>(RSS.Text);
            var Images = JsonConvert.DeserializeObject<List<string>>(RSS.Images);
            var ImageText = JsonConvert.DeserializeObject<List<string>>(RSS.Imagetext);

            int Count = 0;
            int TextCount = 0;
            int ImageCount = 0;

            var Reactions = App.database.GetReactionsFromArticle(RSS.ID);
            foreach (ReactionTable Reaction in Reactions)
            {
                if (Reaction.User == App.LoggedinUser.ID)
                {
                    Reacted = true;
                    CR = Reaction;
                    break;
                }
            }

            var ReactionBackground = new BoxView
            {
                CornerRadius = 0,
                Margin = 0,
                BackgroundColor = App.MC,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            //Reaction buttons
            var ReactionButton = new Button
            {
                CornerRadius = 0,
                BorderWidth = 0,
                Margin = 0,
                BackgroundColor = Color.Transparent,
                BorderColor = Color.LightGray,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Text = "",
                FontSize = 20,
                TextColor = Color.LightGray,
            };
            ReactionButton.Clicked += ReactionButtonClicked;
            ReactionImage = new Image
            {
                Source = "reactions_gray",
                Margin = 0,
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 50,
                HeightRequest = 50,

            };
            var ReactionBar1 = new BoxView
            {
                CornerRadius = 1,
                Margin = 5,
                BackgroundColor = Color.Red,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.End,
                WidthRequest = Application.Current.MainPage.Width / 2 * 1f,
                HeightRequest = 5,
            };
            var ReactionBar2 = new BoxView
            {
                CornerRadius = 1,
                Margin = 5,
                BackgroundColor = Color.Blue,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.End,
                WidthRequest = Application.Current.MainPage.Width / 2 * 0.4f,
                HeightRequest = 5,
            };
            var ReactionBar3 = new BoxView
            {
                CornerRadius = 1,
                Margin = 0,
                BackgroundColor = Color.Yellow,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.End,
                WidthRequest = Application.Current.MainPage.Width / 2 * 0.1f,
                HeightRequest = 5,
            };
            ReactionsOthers1 = new Image
            {
                Source = "reactions_4",
                Margin = 10,
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 30,
                HeightRequest = 30,
            };
            var ReactionsOthers2 = new Image
            {
                Source = "reactions_2",
                Margin = 10,
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 30,
                HeightRequest = 30,
            };
            var ReactionsOthers3 = new Image
            {
                Source = "reactions_0",
                Margin = 10,
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 30,
                HeightRequest = 30,
            };
            ReactionsOthersText = new Label
            {
                Text = Reactions.Count.ToString(),
                TextColor = Color.Black,
                FontSize = 20,
                Margin = 0,
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };

            foreach (int Type in Order) //For loop that decided which order the images and the body text will be arranged in
            {
                if (Type == 0) //If Text
                {                   
                        ArticleGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        
                        
                        
                        var Label = new Label
                        {
                            Text = Text[TextCount].Replace("*", "-").Replace("&quot;","'"),
                            HorizontalTextAlignment = TextAlignment.Start,
                            VerticalTextAlignment = TextAlignment.Start,
                            FontSize = 16,
                            TextColor = Color.Black,                        
                            Margin = new Thickness(0, 0, 5, 10)
                        };
                        ArticleGrid.Children.Add(Label, 1, 4, Row, Row + 1);
                        Row++;
                        Count++;
                        TextCount++;
                }
                else if (Type == 1) //If Image
                {
                        var IMGText = new Label
                        {
                            Text = ImageText[ImageCount].Replace("*", "-"),
                            HorizontalOptions = LayoutOptions.Start ,
                            VerticalOptions = LayoutOptions.Start,
                            HorizontalTextAlignment = TextAlignment.Start,
                            VerticalTextAlignment = TextAlignment.Start,
                            FontSize = 14,
                            TextColor = Color.Gray,
                            Margin = new Thickness(0, 10, 0, 20)
                        };
                        
                        if (Topimg == true)
                        {
                            Image img = new Image { Source = Images[ImageCount] };
                            

                            ArticleImage.Source = Images[ImageCount];
                            ArticleImage.HeightRequest = img.Height;
                            ArticleImage.WidthRequest = 300;
                            ArticleGrid.Children.Add(IMGText, 1, 4, 5, 6);
                            Topimg = false;
                        }
                        else
                        {

                            var Image = new Image
                            {
                                WidthRequest = 200,
                                HeightRequest = 300,
                                Aspect = Aspect.AspectFill,
                                Margin = 5,
                            };

                            ArticleGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                            ArticleGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        ArticleGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        Image.Source = Images[ImageCount];
                            ArticleGrid.Children.Add(Image, 0, 5, Row, Row + 1);
                            ArticleGrid.Children.Add(IMGText, 1, 4, Row+1, Row + 2);
                            Row++;
                            Row++;
                            Row++;
                    }
                        Count++;
                        ImageCount++;
                }
            }
            string[] Categories = RSS.Category.Split(new[] { ", " }, StringSplitOptions.None);
            string[] Tags = RSS.Tag.Split(new[] { ", " }, StringSplitOptions.None);

            int TagRow = 0;

            foreach (String Category in Categories) //Display all the articles categories
            {
                var Box = new Button
                {
                    CornerRadius = 10,
                    Margin = 2,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    ClassId = Category,
                    
                };
                Box.Clicked += CategoryButtonClicked;

                var EmptyAddedBox = new Image
                {
                    Source = "Icon_Heart_red",
                    Margin = 10,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    ClassId = Category,
                    WidthRequest = 30,
                    HeightRequest = 30,
                };

                var AddedBox = new Image
                {
                    Source = "Icon_Heart_Full",
                    Margin = 10,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    ClassId = Category,
                    WidthRequest = 30,
                    HeightRequest = 30,
                };
                var Line = new Button
                {
                    Margin = 0,
                    BackgroundColor = App.MC,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Start,
                    ClassId = Category,
                    HeightRequest = 1,
                };

                var Comment = new Label
                {
                    Text = Category,
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    TextColor = Color.Black,
                    FontSize = 16,
                    WidthRequest = 290,
                    Margin = 20,
                    InputTransparent = true,
                };
                TagGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                TagGrid.Children.Add(EmptyAddedBox, 0, 6, TagRow, TagRow + 1);
                TagGrid.Children.Add(Line, 0, 6, TagRow, TagRow + 1);
                TagGrid.Children.Add(Box, 0, 6, TagRow, TagRow + 1);

                if (App.SideMenu.Categories.Contains(Category))
                {
                    Box.IsEnabled = false;
                    TagGrid.Children.Add(AddedBox, 0, 6, TagRow, TagRow + 1);
                    AddedBox.IsEnabled = false;
                }
                TagGrid.Children.Add(Comment, 0, 6, TagRow, TagRow + 1);
                TagRow++;
            }

            foreach (String Tag in Tags) //Display all the articles tags
            {
                var Box = new Button
                {
                    CornerRadius = 10,
                    Margin = 2,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    TextColor = Color.Transparent,
                    ClassId = Tag
                };
                Box.Clicked += TagButtonClicked;
                var EmptyAddedBox = new Image
                {
                    Source = "Icon_Heart_red",
                    Margin = 10,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    ClassId = Tag,
                    WidthRequest = 30,
                    HeightRequest = 30,
                };
                var Line = new Button
                {
                    Margin = 0,
                    BackgroundColor = App.MC,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Start,
                    ClassId = Tag,
                    HeightRequest = 1,
                };

                var AddedBox = new Image
                {
                    Source= "Icon_Heart_Full",
                    Margin = 10,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    ClassId = Tag,
                    WidthRequest = 30,
                    HeightRequest = 30,
                };
                var Comment = new Label
                {
                    Text = Tag,
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    TextColor = Color.Black,
                    FontSize = 16,
                    WidthRequest = 290,
                    Margin = 20,
                    InputTransparent = true,
                };
                TagGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                TagGrid.Children.Add(EmptyAddedBox, 0, 6, TagRow, TagRow + 1);
                TagGrid.Children.Add(Line, 0, 6, TagRow, TagRow + 1);
                TagGrid.Children.Add(Box, 0, 6, TagRow, TagRow + 1);

                if (App.SideMenu.Tags.Contains(Tag))
                {
                    Box.IsEnabled = false;
                    TagGrid.Children.Add(AddedBox, 0, 6, TagRow, TagRow + 1);
                    AddedBox.IsEnabled = false;
                }
                TagGrid.Children.Add(Comment, 0, 6, TagRow, TagRow + 1);
                TagRow++;
            }


            ArticleGrid.Children.Add(Author, 1, 2, Row, Row + 1);
            ArticleGrid.Children.Add(Location, 2, 4, Row, Row + 1);
            ArticleGrid.Children.Add(BG, 0, 5, 0, Row + 2);
            ArticleGrid.Children.Add(BackGround, 0, 5, Row + 2, Row + 4);
            ArticleGrid.Children.Add(BackGroundReactionTimerFavorites, 0, 5, Row + 2, Row + 4);
            ArticleGrid.Children.Add(TimerIcon, 2, 3, Row + 2, Row + 4);
            ArticleGrid.Children.Add(TimerButton, 2, 3, Row + 2, Row + 4);
            TimerButton.TextColor = Color.White;
            ArticleGrid.Children.Add(FavIcon, 3, 4, Row + 2, Row + 4);

            ArticleGrid.Children.Add(ReactionImage, 1, 2, Row + 2, Row + 4); //Reaction   
            ArticleGrid.Children.Add(ReactionButton, 1, 2, Row + 2, Row + 4); //Reaction   
            ArticleGrid.Children.Add(ReactionsOthers1, 1, 2, Row + 1, Row + 2); //Reaction   
            ArticleGrid.Children.Add(ReactionsOthersText, 1, 2, Row + 1, Row + 2); //Reaction  
			
            if (App.Online)
            {
                LoadComments();
            }
			
            ArticleGrid.Children.Add(TagGrid, 0, 5, Row + 4, Row + 5);
            TagGrid.BackgroundColor = Color.White;
            CommentListView.InputTransparent = true;
            TagGrid.IsVisible = true;
        }

		// A popup to ask the user's feedback on modifying their tags, categories, and authors.
        async void CategoryButtonClicked(object sender, System.EventArgs e)
        {
            var Button = (Button)sender;
            bool answer = await DisplayAlert("", "Vill du lägga till " + Button.ClassId + " i din lista?", "Nej", "Ja");
            if (!answer)
            {
                App.SideMenu.Categories.Add(Button.ClassId);
                App.SideMenu.TaglistUpdate = true;
                Button.IsEnabled = false;
                Button.BackgroundColor = Color.Transparent;
            }
            else
            {
                Button.IsEnabled = true;
                Button.BackgroundColor = Color.Transparent;
            }
            Button.IsEnabled = false;
        }
        void RemoveCategoryButtonClicked(object sender, System.EventArgs e)
        {

            var Button = (Button)sender;
            App.SideMenu.Categories.Remove(Button.ClassId);
            App.SideMenu.TaglistUpdate = true;
            Button.IsEnabled = false;
        }
        async void TagButtonClicked(object sender, System.EventArgs e)
        {
            var Button = (Button)sender;
            bool answer = await DisplayAlert("", "Vill du lägga till " + Button.ClassId + " i din lista?" , "Nej", "Ja");
            if (!answer)
            {
                App.SideMenu.Tags.Add(Button.ClassId);
                App.SideMenu.TaglistUpdate = true;
                Button.IsEnabled = false;
                Button.BackgroundColor = Color.Transparent;
            } else
            {
                Button.IsEnabled = true;
                Button.BackgroundColor = Color.Transparent;
            }
                
        }
        void RemoveTagButtonClicked(object sender, System.EventArgs e)
        {

            var Button = (Button)sender;
            App.SideMenu.Tags.Remove(Button.ClassId);
            App.SideMenu.TaglistUpdate = true;
            Button.IsEnabled = false;
        }
        
		// A popup to react to the user's input on pressing a reaction.
        async void ReactionButtonClicked(object sender, System.EventArgs e)
        {
            var Button = (Button)sender;
            ReactionPopUp rp = new ReactionPopUp(ArticleNR, Reacted, CR, this);
            await PopupNavigation.Instance.PushAsync(rp);
        }

        public void ReactionCheck() //Checks if the user has already reacted on the article, and sets the reaction if so.
        {
            if (App.LoggedinUser != null)
            {

                var Reactions = App.database.GetReactionsFromArticle(rssTable.ID);

                foreach (ReactionTable Reaction in Reactions)
                {

                    if (Reaction.User == App.LoggedinUser.ID)
                    {
                        Reacted = true;
                        CR = Reaction;

                        //How the user reacted.
                        ReactionImage.Source = "reactions_" + Reaction.Reaktion + ".png";
                        break;
                    }
                    else
                    {
                        ReactionImage.Source = "reactions_gray.png";
                    }
                }
                int MaxValue = 0;
                int MostPicked = 0;
                for (int i = 0; i < 8; i++)
                {
                    int CurValue = 0;
                    foreach (ReactionTable Reaction in Reactions)
                    {
                        if(Reaction.Reaktion == i)
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
                ReactionsOthers1.Source = "reactions_"+MostPicked+".png";
                ReactionsOthersText.Text = MaxValue.ToString();
            }
        }

        public void FavButtonCheck(RSSTable RSS) //Checks if the user has already favorited the article, and sets the favoritesbutton if so.
        {
            if (App.LoggedinUser != null)
            {
                int j = 0;
                var FAL = App.database.GetFavorites(App.LoggedinUser.ID);

                if (FAL != null)
                {
                    Console.WriteLine("Favorites Gotten: " + FAL.Count());
                    foreach (var FA in FAL)
                    {
                        if (RSS.ID == FA.ID)
                        {
                            Favorited = true;
                        }
                    }
                }
                if (Favorited)
                {
                    FavIcon.Source = "Icon_Heart_Full";
                }
                else
                {
                    FavIcon.Source = "Icon_Heart_red";
                }
            }
        }

        async void AnimateButton(Button button, Image image, BoxView box) //Button Animation.
        {
            if (box != null)
            {
                await box.ScaleTo(0.8f, 100, Easing.BounceOut);
                await box.ScaleTo(1f, 100, Easing.BounceOut);
            }
            else if (image != null)
            {
                await image.ScaleTo(0.8f, 100, Easing.BounceOut);
                await image.ScaleTo(1f, 100, Easing.BounceOut);
            }
            else if (button != null)
            {
                await button.ScaleTo(0.8f, 100, Easing.BounceOut);
                await button.ScaleTo(1f, 100, Easing.BounceOut);
            }
        }

        async void FavButtonClicked(object sender, System.EventArgs e) // When the favorite button is clicked, a favorite object for that user is sent to the server to be stored.
        {
            if (App.LoggedinUser != null)
            {
                if (Favorited)
                {
                    App.database.Execute("DELETE FROM Favorites WHERE User = "+ App.LoggedinUser.ID +" AND Article = "+ ArticleNR);
                    await DisplayAlert("Favorite", "Article Unfavorited", "Ok");
                    Favorited = false;
                    FavIcon.Source = "Icon_Heart_red";
                }
                else
                {
                    var fav = new FavoritesTable
                    {
                        User = App.LoggedinUser.ID,
                        Article = ArticleNR,
                        Image = ArticleImage.Source.ToString(),
                        Header = Rubrik.Text
                    };
                    App.database.InsertFavorite(fav);
                    
                    Image image = FavIcon;

                    await image.ScaleTo(0.8f, 100, Easing.BounceOut);
                    await image.ScaleTo(1f, 100, Easing.BounceOut);

                    await DisplayAlert("Favorit", "Artikel tillagd i favoriter.", "Ok");
                    Favorited = true;
                    FavIcon.Source = "Icon_Heart_Full";
                }
            }
            else
            {
                await DisplayAlert("Favorite", "Please Log in in order to Favorite Articles", "Ok");
            }
        }
        async void TimerButtonClicked(object sender, System.EventArgs e) // When the Timer button is clicked, a history object for that user is sent to the server to be stored.
        {
            TimerButton.IsEnabled = false;
            Button button = (Button)sender;
            await button.ScaleTo(0.8f, 80, Easing.BounceOut);
            await button.ScaleTo(1, 80, Easing.BounceOut);

            if (TimerButton.BackgroundColor == App.MC && Read == false)
            {
                var HT = new HistoryTable
                {
                    User = App.LoggedinUser.ID,
                    Article = ArticleNR,
                    Readat = DateTime.Now,
                    Header = Rubrik.Text,
                    Image = ArticleImage.Source.ToString()
                };
                App.database.InsertHistory(HT);
                TimerIcon.Source = "Icon_Coin.png";
                TimerButton.Text = "";
                TimerButton.IsVisible = false;
                TimerButton.TextColor = Color.White;
                var NG = (NewsGridPage)App.Mainpage.Children[1];
                foreach (NewsGridPage.Article A in NG.ArticleList)
                {
                    if (A.ID == ArticleNR)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
							//Article is Locked.
                        });

                    }
                }
                App.database.Plustoken(App.LoggedinUser, 1);
                Read = true;
                TimerButton.IsEnabled = false;
                var variable = (ProfilePage)App.Mainpage.Children[2];
                variable.TokenNumber.Text = App.LoggedinUser.Plustokens.ToString();
            }
            else
            {
                TimerButton.IsEnabled = true;
            }
        }
        async void TimerDone(object sender)
        {
            Label label = (Label)sender;
            TimerButton.TextColor = Color.White;
        }
		
		// A timer to make sure the user cannot collect coins before a certain time has passed. The background color and timer text updates dynamically to show how much time remains.
        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (red > (App.MC.R*255))
            {
                red = red - 1;
            }
            if (green > (App.MC.G * 255))
            {
                green = green - 1;
            }
            if (blue > (App.MC.B * 255))
            {
                blue = blue - 1;
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                TimerButton.BackgroundColor = Color.FromRgb(red, green, blue);
                string newText = Math.Round((((App.MC.R * 255) + (App.MC.G * 255) + (App.MC.B * 255)) / (blue + green + red)) * 100).ToString() + "%";
                if (TimerButton.Text != newText)
                    TimerButton.Text = newText;
                if (TimerButton.Text == "100%")
                    TimerButton.Text = "Samla mynt!";
                TimerButton.TextColor = Color.White;
            });

            if (App.MC == Color.FromRgb(red, green, blue))
            {
                Timer.Stop();
                Timer.Close();
                Timer.Dispose();
                TimerDone(Rubrik);
                TimerDone(Ingress);
            }
            else
            {
                Timer.Start();
            }
        }
        
		// Sending in a comment to the database for display.
        async void SubmitComment(object sender, EventArgs e)
        {
            CommentButton.IsEnabled = false;
            if (App.Online)
            {
                var CNR = App.database.CommentCount(ArticleNR);
                if (App.database.TokenCheck() && CommentEntry.Text != null && CommentEntry.Text != "" && CommentEntry.Text.Length > 0 && CNR != -1)
                {
                    var SC = new CommentTable
                    {
                        Article = ArticleNR,
                        CommentNR = CNR,
                        UserSubmitted = 0,
                        User = App.LoggedinUser.ID,
                        Replynr = -1,
                        Replylvl = 0,
                        Comment = CommentEntry.Text,
                        Point = 0
                    };
                    CommentEntry.Text = "";
                    Console.WriteLine("Inserting Comment");
                    App.database.InsertComment(SC);
                    Console.WriteLine("Loading Comments");
                    LoadComments();
                    Console.WriteLine("Refreshing Comments");
                    CommentListView.ItemsSource = null;
                    CommentListView.ItemsSource = CommentList;
                    CommentListView.HeightRequest = LWH;
                }
            }
            else
            {
                await DisplayAlert("Offline", "The Server is currently Offline. Please try again later.", "OK");
            }
            CommentButton.IsEnabled = true;
        }
		
		// When the user wants to load in and display all comments on an article.
        async void LoadComments()
        {
            Console.WriteLine("Nr of Comments (Pre Load): " + CommentTableList.Count);
            CommentTableList.Clear();
            CommentTableList = App.database.GetComments(ArticleNR,0,-1);
            if (CommentTableList != null)
            {
                CommentList.Clear();
                foreach (var CommentTable in CommentTableList)
                {
                    MakeComment(CommentTable);
                }
                if (7 < CommentList.Count)
                {
                    LWH = 70 * CommentList.Count;
                }
                else
                {
                    LWH = 70 * CommentList.Count;
                }
                if (!CommentsLoaded)
                {
                    CreateCommentListView();
                    CommentsLoaded = true;
                }
            }
            else
            {
                await DisplayAlert("Offline", "Comments could not be loaded. Please try again later.", "OK");
            }
            Console.WriteLine("Nr of Comments (Post Load): " + CommentTableList.Count);
        }
        public void MakeComment(CommentTable s)
        {
            var User = App.database.GetUser(s.User);
            if(User != null)
            {
                var C = new Comment(s, User.First());
                CommentList.Add(C);
            }
                      
        }
        public void CreateCommentListView() //Creates the Listview that displays the comments.
        {
            CommentListView = new ListView
            {
                // Source of data items.
                ItemsSource = CommentList,
                HasUnevenRows = true,
                SeparatorVisibility = SeparatorVisibility.None,
                HeightRequest = LWH,

                // Define template for displaying each item.
                // (Argument of DataTemplate constructor is called for each item; it must return a Cell derivative.)
                ItemTemplate = new DataTemplate(() =>
                {
                    var Box = new BoxView
                    {
                        CornerRadius = 10,
                        Margin = 5,
                        HeightRequest = 50,
                        BackgroundColor = Color.White,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                    };
                    var Comment = new Label
                    {
                        HorizontalTextAlignment = TextAlignment.Start,
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalOptions = LayoutOptions.Start,
                        TextColor = Color.Black,
                        FontSize = 16,
                        
                        Margin = 20,
                    };
                    var Username = new Label
                    {
                        HorizontalTextAlignment = TextAlignment.Start,
                        VerticalTextAlignment = TextAlignment.Start,
                        TextColor = Color.Black,
                        FontSize = 16,

                    };
                    var AvatarFace = new Image
                    {
                        WidthRequest = 50,
                        HeightRequest = 50,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        BackgroundColor = App.MC,
						
                        Margin = 5
                    };
                    var AvatarHair = new Image
                    {
                        WidthRequest = 50,
                        HeightRequest = 50,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,

                        Margin = 5

                    };
                    var AvatarBody = new Image
                    {
                        WidthRequest = 50,
                        HeightRequest = 50,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,

                        Margin = 5

                    };
                    var AvatarExpr = new Image
                    {
                        WidthRequest = 50,
                        HeightRequest = 50,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,

                        Margin = 5

                    };
                    var AvatarBeard = new Image
                    {
                        WidthRequest = 50,
                        HeightRequest = 50,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,

                        Margin = 5
                    };
                    var Elispses = new Button()
                    {
                        BackgroundColor = Color.Transparent,
                        WidthRequest = 60,
                        HeightRequest = 30,
                        HorizontalOptions = LayoutOptions.End,
                        VerticalOptions = LayoutOptions.End,
						
                        Margin = 20,
                        ImageSource = "elipses.png",
                    };

                    var CommentGrid = new Grid
                    {
                        RowDefinitions = {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                    },
                        ColumnDefinitions = {
                    new ColumnDefinition { Width = 10 },
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
                        
                        IsClippedToBounds = true
                    };

                    Comment.SetBinding(Label.TextProperty, "CommentText");
                    Username.SetBinding(Label.TextProperty, "UserName");
                    AvatarFace.SetBinding(Image.SourceProperty, "UserAvatar[0]");                    
                    AvatarHair.SetBinding(Image.SourceProperty, "UserAvatar[1]");
                    AvatarBody.SetBinding(Image.SourceProperty, "UserAvatar[2]");
                    AvatarExpr.SetBinding(Image.SourceProperty, "UserAvatar[3]");
                    AvatarBeard.SetBinding(Image.SourceProperty, "UserAvatar[4]");

                    // Return an assembled ViewCell.
                    return new ViewCell
                    {
                        View = CommentGrid
                    };
                })
            };
        }
		
        protected override void OnDisappearing()
        {
            Console.WriteLine("Memory Cleanup");
            GC.Collect();
        }
    }
}
