using FFImageLoading.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NWT
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StylePage : ContentPage
    {
        
		/*
		
			This page shows a list of 'styles' or simple colors that the user can select in order to change the 'style' or color of the app.
		
		*/
		
        public Label Label = new Label { };
        public Image Image = new Image { };
        public Button Button = new Button { };
        public List<int> StylesInventory = new List<int>();
        public string ActiveStyle;
        public ListView StyleListView;
        public List<AvatarItemsTable> StylesList;
        public List<UserStyle> UserStylesList = new List<UserStyle>();
        public class UserStyle
		{
            public AvatarItemsTable Style { get; set; }
            public bool Owns { get; set; }
            public string ImagePath { get; set; }
            public int ID  { get; set; }

            public UserStyle(AvatarItemsTable Style_, bool Owns_)
            {
                Style = Style_;
                Owns = Owns_;
                ImagePath = Style.ImagePath;
                ID = Style.ID;
            }
        }

        public StylePage()
        {

            InitializeComponent();

            if (App.LoggedinUser.TutorialProgress == 3)
            {
                App.LoggedinUser.TutorialProgress = 4;
                App.database.UpdateTutorialProgress(App.LoggedinUser);
            }
            StylesInventory = JsonConvert.DeserializeObject<List<int>>(App.LoggedinUser.Inventory);
            StylesList = App.database.GetItemFromType("Style");
            int i = 0;
			
            foreach (var Style in StylesList)
            {
                i++;
                bool Owns = false;
                Console.WriteLine(StylesInventory.Count);

                foreach (var StyleID in StylesInventory)
                {
                    if(Style.ID == StyleID || i < 5)
                    {
                        Owns = true;
                        break;
                    }
                    else
                    {
                        Owns = false;
                    }
                }
                UserStylesList.Add(new UserStyle(Style, !Owns));
                Console.WriteLine(Style.ID);
            }
            CreateListView();
            StyleGrid.Children.Add(StyleListView, 0, 3, 1, 2);
        }

        public void CreateListView()
        {
            StyleListView = new ListView
            {
                // Source of data items.
                ItemsSource = UserStylesList,
                HasUnevenRows = false,
                SeparatorVisibility = SeparatorVisibility.None,
                BackgroundColor = Color.FromHex("#FFFFFF"),

                ItemTemplate = new DataTemplate(() =>
                {
                    Button Box = new Button
                    {
                        HeightRequest = 30,
                        BorderColor = Color.FromHex("#f0f0f0"),
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill,
                        Margin = 5,
                    };
					
                    Box.Clicked += SelectStyle;

                    var Image = new CachedImage()
                    {
                        HeightRequest = 20,
                        
                        Source = "Icon_Hub_white.png",
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(10,0,0,0),
                        CacheDuration = TimeSpan.FromDays(14),
                        DownsampleToViewSize = false,
                        RetryCount = 5,
                        RetryDelay = 100,
                        BitmapOptimizations = false,
                        LoadingPlaceholder = "",
                        ErrorPlaceholder = "",
                    };

                    Button Lock = new Button
                    {
                        HeightRequest = 30,
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill,
                        Margin = 5,
                    };

                    Lock.Clicked += UnlockComponent;

                    var LockImage = new CachedImage()
                    {
                        HeightRequest = 20,
                        Source = "icon_keyhole.png",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(0, 0, 0, 0),
                        CacheDuration = TimeSpan.FromDays(14),
                        DownsampleToViewSize = true,
                        RetryCount = 5,
                        RetryDelay = 100,
                        BitmapOptimizations = false,
                        LoadingPlaceholder = "",
                        ErrorPlaceholder = "",
                        InputTransparent = true
                    };

                    Box.SetBinding(Button.ClassIdProperty, "ImagePath");
                    Box.SetBinding(Button.BackgroundColorProperty, "ImagePath");
                    Image.SetBinding(CachedImage.ClassIdProperty, "ID");
                    Lock.SetBinding(Button.ClassIdProperty, "ID");
                    LockImage.SetBinding(CachedImage.ClassIdProperty, "ID");
                    Lock.SetBinding(Button.IsVisibleProperty, "Owns");
                    LockImage.SetBinding(CachedImage.IsVisibleProperty, "Owns");

                    var Grid = new Grid
                    {
                        RowDefinitions = {
                    new RowDefinition { Height = 0 },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = 10 },
                    },
                        ColumnDefinitions = {
                    new ColumnDefinition { Width = 0 },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 0 },
                    },
                        RowSpacing = 0,
                        ColumnSpacing = 0,
                        BackgroundColor = Color.FromHex("#f2f2f2")
                    };

                    Grid.Children.Add(Box, 0, 5, 1, 2); //Boxview
                    Grid.Children.Add(Image, 1, 2, 1, 2); //Image   
                    Grid.Children.Add(Lock, 0, 5, 1, 2); //Boxview 

                    return new ViewCell
                    {
                        View = Grid
                    };
                })
            };
        }

        async void UnlockComponent(object sender, EventArgs e)
        {
            var Button = (Button)sender;
            Console.WriteLine(Button.ClassId);
            var id = Convert.ToInt32(Button.ClassId);
            var Style = App.database.GetItemFromID(id).First();
            int tokenNumber = Style.Price;
            Console.WriteLine("Unlocking item.");
            bool answer = await DisplayAlert("", "Vill du låsa upp " + Style.Descriptions + " för " + tokenNumber + " mynt?", "Nej", "Ja");
            if (!answer)
            {
                if (App.LoggedinUser.Plustokens >= tokenNumber)
                {
                    //Unlocks item
                    Button.IsEnabled = false;
                    Button.IsVisible = false;
                    App.database.Plustoken(App.LoggedinUser, -tokenNumber);
                    StylesInventory.Add(Convert.ToInt32(id));
                    App.LoggedinUser.Inventory = JsonConvert.SerializeObject(StylesInventory);
                    App.database.UpdateAvatarItems(App.LoggedinUser);
                }
                else
                {
                    await DisplayAlert("", "Inte tillräckligt mynt. Du har bara " + App.LoggedinUser.Plustokens + ". Du behöver " + (tokenNumber - App.LoggedinUser.Plustokens) + " mynt till.", "Okej.");
                }
            }
        }

        public void SelectStyle(object sender, EventArgs e)
        {
            var Sender = (Button)sender;
            var Style = Sender.ClassId;
            if(Style == "Blue")
            {
                ColorFunction(Color.FromHex("#649FD4"));
            }
            else if (Style == "Green")
            {
                ColorFunction(Color.FromHex("#6fb110"));
            }
            else if (Style == "LightBlue")
            {
                ColorFunction(Color.FromHex("#649FD4"));
            }
            else if (Style == "Purple")
            {
                ColorFunction(Color.FromHex("#bb0066"));
            }
            else if (Style == "White")
            {
                ColorFunction(Color.FromHex("#e0d8b3"));
            }
            else
            {
                ColorFunction(Sender.BackgroundColor);
            }
            App.LoggedinUser.Style = Style;
            App.database.UpdateAvatarItems(App.LoggedinUser);
        }

        public static void ColorFunction(Color BGC)
        {
            var BC = BGC;
            var CM = App.SideMenu;
            var MP = App.Mainpage;
            var SP = App.Startpage;
            var CNF = (CustomNewsFeed)MP.Children[0];
            var NF = (NewsGridPage)MP.Children[1];
            var PP = (ProfilePage)MP.Children[2];
            var HP = (HubbPage)MP.Children[3];

            CM.DownButton.BackgroundColor = BC;
            CM.DownButton.Color = BC;
            MP.BackgroundColor = BC;
            MP.BarBackgroundColor = BC;
            SP.Detail.BackgroundColor = BC;
            CNF.Down.BackgroundColor = BC;
            NF.Down.BackgroundColor = BC;
            SP.Master.BackgroundColor = BC;
            SP.Detail.BackgroundColor = BC;
            PP.PE1.BackgroundColor = BC;
            PP.PE1.Color = BC;
            PP.FavoritesButton.BackgroundColor = BC;
            PP.HistoryButton.BackgroundColor = BC;
            PP.StyleButton.BackgroundColor = BC;
            PP.AchievementsButton.BackgroundColor = BC;
            PP.ProfileSettingsButton.BackgroundColor = BC;
            PP.m1bx.BackgroundColor = BC;
            PP.m2bx.BackgroundColor = BC;
            PP.m3bx.BackgroundColor = BC;
            HP.BackgroundColor = BC;

            App.MC = BC;

            NF.PrintNews();
            App.Startpage.Detail = new NavigationPage(App.Mainpage) { BarBackgroundColor = BC, BarTextColor = Color.FromHex("#FFFFFF"), };
        }

        async void OnDismissButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync();
        }

        protected override void OnDisappearing()
        {
            Console.WriteLine("Memory Cleanup");
            GC.Collect();
        }
    }
}