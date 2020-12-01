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
	public partial class UserSettingsPage : ContentPage
	{
		/*
		
			This page displays settings that the user can configure.
			These settings have to do with the user experience in the client specifically.
		
		*/
		
        public UserSettingsPage()
		{
			InitializeComponent ();
            UpdateInfoButton.BackgroundColor = App.MC;
        }
    }
}