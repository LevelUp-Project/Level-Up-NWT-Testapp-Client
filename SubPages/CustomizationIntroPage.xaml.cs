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
	public partial class CustomizationIntroPage : ContentPage
	{
		/*
		
			This page displays a list of tags, categories, and authors that the user can add to their customizable list.
			This customizable list will affect the articles appearing in the user's news feed.
		
		*/

		public CustomizationIntroPage()
		{
			InitializeComponent();
            
		}

        async void CustomizationPage(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            await Navigation.PushAsync(new PlayPage());
        }
    }
}