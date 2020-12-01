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
	public partial class HubbPage : ContentPage
	{

		/*
		
			The HubbPage contains four buttons, each leading to a different subPage.
			
		*/
       
		public HubbPage()
		{
			InitializeComponent ();

            BackgroundColor = App.MC;
		}

        public void ButtonLock()
        {
           /*EvenemangsButton.IsEnabled = !EvenemangsButton.IsEnabled;
           VoteButton.IsEnabled = !VoteButton.IsEnabled;
           PlayButton.IsEnabled = !PlayButton.IsEnabled;*/
        }

		//These five functions each open a different subPage.
		//First one opens UserNewsGridPage.
		//Second one opens UserSubmissionPage.
		//Third one opens VotePage.
		//Fourth one opens PlayPage.

        void Insandare(object sender, EventArgs e)
        {
            //Button button = (Button)sender;
            ButtonLock();
            /*await button.RotateTo(-5, 80, Easing.BounceOut);
            await button.RotateTo(5, 120, Easing.BounceOut);
            await button.RotateTo(0, 80, Easing.BounceOut);*/
            //await Navigation.PushAsync(new UserNewsGridPage());
            ButtonLock();
        }
        async void MakeInsandare(object sender, EventArgs e)
        {
            //Button button = (Button)sender;
            ButtonLock();
            /*await button.RotateTo(-5, 80, Easing.BounceOut);
            await button.RotateTo(5, 120, Easing.BounceOut);
            await button.RotateTo(0, 80, Easing.BounceOut);*/
            await Navigation.PushAsync(new UserSubmissionPage());
            ButtonLock();
        }

        async void VotePage(object sender, EventArgs e)
        {
            //Button button = (Button)sender;
            ButtonLock();
            /*await button.RotateTo(-5, 80, Easing.BounceOut);
            await button.RotateTo(5, 120, Easing.BounceOut);
            await button.RotateTo(0, 80, Easing.BounceOut);*/
            await Navigation.PushAsync(new VotePage());
            ButtonLock();
        }

        async void GamePage(object sender, EventArgs e)
        {
            //Button button = (Button)sender;
            ButtonLock();
            /*await button.RotateTo(-5, 80, Easing.BounceOut);
            await button.RotateTo(5, 120, Easing.BounceOut);
            await button.RotateTo(0, 80, Easing.BounceOut);*/
            await Navigation.PushAsync(new PlayPage());
            ButtonLock();
        }
		
        async void WIP(object sender, EventArgs e)
        {
            await DisplayAlert("WIP", "Inte ännu färdig.", "Okej");
        }
		
		//To make sure the user does not turn off the app accidentally, the return button is inactive on the four main view pages.
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

    }
}