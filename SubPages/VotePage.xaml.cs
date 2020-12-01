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
	public partial class VotePage : ContentPage
	{
		/*
		
			This page contains a page where voting forms found on the database can be displayed.
			The user can then vote on the answers available, which the database will then recieve.
		
		*/
		
		public VotePage ()
		{
			InitializeComponent ();

            Header.TextColor = App.MC;
            SubmitN.BackgroundColor = App.MC;
            VoteN.BackgroundColor = App.MC;
            ResultN.BackgroundColor = App.MC;
        }
        async void Submit(object sender, EventArgs e)
        {
            if (App.Online)
            {
                await Navigation.PushAsync(new VoteSubmissionPage());
            }
            else
            {
                await DisplayAlert("Offline", "The Server is currently Offline. Please try again later.", "OK");
            }

        }
        async void Vote(object sender, EventArgs e)
        {
            if (App.Online)
            {
                await Navigation.PushAsync(new VoteListPage(1));
            }
            else
            {
                await DisplayAlert("Offline", "The Server is currently Offline. Please try again later.", "OK");
            }

        }
        async void Results(object sender, EventArgs e)
        {
            if (App.Online)
            {
                await Navigation.PushAsync(new VoteListPage(2));
            }
            else
            {
                await DisplayAlert("Offline", "The Server is currently Offline. Please try again later.", "OK");
            }

        }
        async void Archive(object sender, EventArgs e)
        {
            if (App.Online)
            {
                await Navigation.PushAsync(new VoteListPage(3));
            }
            else
            {
                await DisplayAlert("Offline", "The Server is currently Offline. Please try again later.", "OK");
            }

        }
    }
}