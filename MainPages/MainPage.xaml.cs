using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace NWT
{
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {

		/*
		
			The MainPage contains four children, each showed as tabs in the main view.
			
			The initialization adds a search button to the toolbar, changes the main color, and creates the four pages of the main view as children.
			
		*/

        public MainPage()
        {
            InitializeComponent();

            UnselectedTabColor = Color.FromHex("FFFFFF");
            SelectedTabColor = Color.FromHex("FFFFFF");

            ToolbarItems.Add(new ToolbarItem("Search", "Icon_Search_white.png", async () => { var page = new ContentPage(); var result = await page.DisplayAlert("Title", "Message", "Accept", "Cancel");
            Debug.WriteLine("success: {0}", result); }));

            Children[3] = new HubbPage();
            Children[2] = new ProfilePage();
            Children[1] = new NewsGridPage(0);
            Children[0] = new CustomNewsFeed();
		}
    }
}
