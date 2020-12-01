using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
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
	public partial class LoginPage : ContentPage
	{
		
		/*
		
			The LoginPage is the first things the user is greeted with upon startup.
			
			Here they can input their account username and password in order to access the rest of the application.
			
		*/
		
        public bool BBT = true;

        public LoginPage ()
		{
			InitializeComponent ();
            BindingContext = this;
            
            var properties = App.Current.Properties;
            if (properties.ContainsKey("username"))
            {
                UserLogin.Text = (string)properties["username"];
            }

            if (properties.ContainsKey("password"))
            {
                UserPassword.Text = (string)properties["password"];
            }
        }

        void LoginCheck()
        {
            App.LoggedinUser = new UserTable();
            
            var Test = App.database.GetUserStats(1);
            
			if(Test != null)
            {
                App.Online = true;
            }

            App.LoggedinUser = null;
        }
		
		//When user pressed the login button, their input is checked for matches on the UserTable. If the input matches, the application starts up by initializing the loadingScreen, and moving into the MainPage and updating its children. If the user is new, then the tutorial and setup is prepared for them before they enter the mainpages.

        async void Login(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(App.LoadingScreen);
            Button button = (Button)sender;
            await button.ScaleTo(0.8f, 100, Easing.BounceOut);
            await button.ScaleTo(1f, 100, Easing.BounceOut);

            LoginCheck();
			
            if (App.Online && UserLogin.Text != null && UserPassword.Text != null)
            {
                UserTable User = new UserTable
                {
                    Username = UserLogin.Text,
                    Email = UserLogin.Text,
                    Password = UserPassword.Text
                };

                App.database.Login(User);
                if (App.LoggedinUser != null)
                {

                    if (App.LoggedinUser.TutorialProgress == 0)
                    {
                        App.TutorialSafety = false;
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            App.Startpage.Detail = new IntroWalkthrough() { };
                        });
                        
                    }
                    else
                    {
                        await System.Threading.Tasks.Task.Run(async () =>
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                App.LS.loadingAnimation.Play();

                                App.Startpage.Detail = App.LS;

                                App.LS.LoadingText.Text = "Appen laddas in.";
                            });
                            StartApp();
                            await System.Threading.Tasks.Task.Delay(10);
                            Device.BeginInvokeOnMainThread( () =>
                            {
                                Console.WriteLine("Initiering Klar");
                                
                                App.Startpage.Detail = new NavigationPage(App.Mainpage) { BarBackgroundColor = App.MC, BarTextColor = Color.FromHex("#FFFFFF"), };
                                App.Mainpage.CurrentPage = App.Mainpage.Children[0];
                                App.SideMenu.UpdatingSideMenu();
                                App.database.UpdateTutorialProgress(App.LoggedinUser);
                                var MP = App.Mainpage;
                                var PP = (ProfilePage)MP.Children[2];
                                PP.ChangeIntroStep(App.LoggedinUser.TutorialProgress);
                            
							});

                        });
                        
                        App.SideMenu.SetTags();
						
                        StylePage.ColorFunction(Color.FromHex(App.LoggedinUser.Style));

                    }

                    var properties = App.Current.Properties;
                    if (!properties.ContainsKey("username"))
                    {
                        properties.Add("username", UserLogin.Text);
                    }
                    else
                    {
                        properties["username"] = UserLogin.Text;
                    }

                    if (!properties.ContainsKey("password"))
                    {
                        properties.Add("password", UserPassword.Text);
                    }
                    else
                    {
                        properties["password"] = UserPassword.Text;
                    }
                    if (!properties.ContainsKey("showingress"))
                    {
                        properties.Add("showingress", true);
                    }
                    if (!properties.ContainsKey("avatarbodyBig"))
                    {
                        properties.Add("avatarbodyBig", true);
                    }

                    //If the following two lines are added, the tutorial will be reset to 0 on log in.
                    //App.LoggedinUser.TutorialProgress = 0; 
                    //App.database.UpdateTutorialProgress(App.LoggedinUser);
                }
                else
                {
                    await DisplayAlert("Failed Login", "Incorrect Username/Password", "OK");
                }
            }
            else
            {
                await DisplayAlert("Offline", "The Server is currently Offline. Please try again later.", "OK");
            }
        }
		
		//If the registration button is pressed, the RegistrationPage is opened for the user.
        async void Register(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegistrationPage());
        }

        void Grafiker(object sender, EventArgs e)
        {
            App.Startpage.Detail = new NavigationPage(new GSampleTabbedPage());
        }
		
		//This function contains the necessary steps to initialize the mainPage after logging in, and also runs the loading screen.
        public void StartApp()
        {
            var CustomNewsGridPage = (CustomNewsFeed)App.Mainpage.Children[0];
            var NG = (NewsGridPage)App.Mainpage.Children[1];
            Device.BeginInvokeOnMainThread(() =>
            {
                App.LS.LoadingText.Text = "Laddar in Dina Val";
            });
            
            NG.CreateFeed(0);
            Device.BeginInvokeOnMainThread(() =>
            {

            });

            CustomNewsGridPage.CreateFeed();
            Device.BeginInvokeOnMainThread(() =>
            {
                App.LS.LoadingText.Text = "Laddar in det samlade Nyhetsflödet";
            });

            var x = (ProfilePage)App.Mainpage.Children[2];
            x.Login(App.LoggedinUser);

            Device.BeginInvokeOnMainThread(() =>
            {
                App.LS.LoadingText.Text = "Updaterar Profilsidan";
            });
        }
    }
}