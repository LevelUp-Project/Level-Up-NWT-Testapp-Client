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
	public partial class AboutPage : ContentPage
	{
		/*
		
			This page displays information about the application's developers, as well as some contact information, and where the user is able to update the application.
			
			There is also a online form linked here that can be used to send relevant information, feedback, or bug reports to the developers.
		
			The links in on this page have been removed as of the official Creative Commons release. They have been replaced with links to google.com.
			
		*/
		
        public AboutPage()
		{
			InitializeComponent ();
        }
        private void Button_Clicked(object sender, EventArgs e)
        {

            Uri siteUri = new Uri("https://google.com");

            Device.OpenUri(siteUri);
        }

        private void Button_Clicked2(object sender, EventArgs e)
        {
            Uri siteUri = new Uri("https://google.com");

            Device.OpenUri(siteUri);
        
        }
    }
}