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
	public partial class Sidemenu : ContentPage
	{
		/*
		
			The Sidemenu is a page that is always available.
			
			It contains a list of the user's seleced categories (filter), tags, and authors. Selecting an object on the list updates the NewsGridPage to show articles of the selected object.
			
		*/
		
        public string Filter = "";
        public string Author = "";
        public string Tag = "";

        public Sidemenu ()
		{
			InitializeComponent ();
            UserSettingsB.IsEnabled = false;
            LogoutB.IsEnabled = false;
        }

        public void ChangeColor(Color color)
        {
            SideMenuBox.Color = color;
        }
		
        public void SetKategori(object sender, EventArgs e)
        {
            var Sender = (Button)sender;
            Filter = Sender.ClassId;
        }
		
        public void SetTag(object sender, EventArgs e)
        {
            var Sender = (Button)sender;
            Tag = Sender.ClassId;
        }
		
        public void SetAuthor(object sender, EventArgs e)
        {
            var Sender = (Button)sender;
            Author = Sender.ClassId;
        }

        public void Clear(object sender, EventArgs e)
        {
            Filter = "";
            Author = "";
            Tag = "";
        }

		//The following function is used to update the NewsGridPage with the selected Filter, Author, and/or Tag.
        public void PrintNews(object sender, EventArgs e)
        {
            ButtonLock();
            NewsGridPage Page = (NewsGridPage)App.Mainpage.Children[1];
            App.database.LocalExecute("DELETE FROM NF");

            Page.PREV = 0;
            Page.CURR = NewsGridPage.DBLN;
            Page.NEXT = NewsGridPage.DBLN * 2;
            Page.Loadnr = 1;
            Page.Filter = Filter;
            Page.Author = Author;
            Page.Tag = Tag;

            Page.ArticleList.Clear();
            Page.LoadLocalDB();
            Page.AddNews(0);
            ButtonLock();
        }
		
		//The following functions are used to keep the user from creating a button click before all database requests are finished.
        public void ButtonLock()
        {
            Nyheter.IsEnabled = !Nyheter.IsEnabled;
            Sport.IsEnabled = !Sport.IsEnabled;
            Ekonomi.IsEnabled = !Ekonomi.IsEnabled;
            NöjeochKultur.IsEnabled = !NöjeochKultur.IsEnabled; 
            Åsikter.IsEnabled = !Åsikter.IsEnabled;
            Familj.IsEnabled = !Familj.IsEnabled;
            UserSettingsB.IsEnabled = !UserSettingsB.IsEnabled;
            AboutB.IsEnabled = !AboutB.IsEnabled;
            LogoutB.IsEnabled = !LogoutB.IsEnabled;

            Tibro.IsEnabled = !Tibro.IsEnabled;
            Skövde.IsEnabled = !Skövde.IsEnabled;
            Falkköping.IsEnabled = !Falkköping.IsEnabled;
            Karlsborg.IsEnabled = !Karlsborg.IsEnabled;
            Rensa.IsEnabled = !Rensa.IsEnabled;
            Verkställ.IsEnabled = !Verkställ.IsEnabled;
        }

        public void LockButtons()
        {
            UserSettingsB.IsEnabled = false;
            LogoutB.IsEnabled = false;
        }
		
        public void UnlockButtons()
        {
            UserSettingsB.IsEnabled = true;
            LogoutB.IsEnabled = true;
        }

		//When pressing the logout button a request goes out to the server, and the user is sent to the loginPage.
        async void Logout(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            ButtonLock();
            ProfilePage page = (ProfilePage)App.Mainpage.Children[2];
            page.Logout();            
            await button.RotateTo(-5, 80, Easing.BounceOut);
            await button.RotateTo(5, 120, Easing.BounceOut);
            await button.RotateTo(0, 80, Easing.BounceOut);
            ButtonLock();
            UserSettingsB.IsEnabled = false;
            LogoutB.IsEnabled = false;
        }
		
		//When pressing the About button the AboutPage opens.
        async void About(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            ButtonLock();
            await button.RotateTo(-5, 80, Easing.BounceOut);
            await button.RotateTo(5, 120, Easing.BounceOut);
            await button.RotateTo(0, 80, Easing.BounceOut);
            await Navigation.PushAsync(new AboutPage());
            ButtonLock();
        }
		
		//When pressing the UserSettings button the UserSettingsPage opens.
        async void UserSettings(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            ButtonLock();
            await button.RotateTo(-5, 80, Easing.BounceOut);
            await button.RotateTo(5, 120, Easing.BounceOut);
            await button.RotateTo(0, 80, Easing.BounceOut);
            await Navigation.PushAsync(new UserSettingsPage());
            ButtonLock();
        }
    }
}