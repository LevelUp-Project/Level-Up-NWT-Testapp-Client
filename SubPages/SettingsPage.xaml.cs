﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NWT
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
		/*
		
			This page allows the user to edit account information. 
		
		*/
		
        public SettingsPage()
		{
			InitializeComponent ();
            Fullname.Text = App.LoggedinUser.Name;
            UserEmail.Text = App.LoggedinUser.Email;
            UserCity.Text = App.LoggedinUser.City;
            UserAge.Text = App.LoggedinUser.Age.ToString();

            var properties = App.Current.Properties;
            if (properties.ContainsKey("showingress"))
            {
                IngressSwitch.IsToggled = (bool)properties["showingress"];
            }

            UpdateInfoButton.BackgroundColor = App.MC;
            ChangePasswordButton.BackgroundColor = App.MC;
        }
		
        async void ChangePassword(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            await button.RotateTo(-5, 80, Easing.BounceOut);
            await button.RotateTo(5, 120, Easing.BounceOut);
            await button.RotateTo(0, 80, Easing.BounceOut);
            await Navigation.PushAsync(new PasswordPage());
        }

        public async void UpdateInfo(object sender, EventArgs e)
        {
            if (App.database.TokenCheck())
            {
                bool Change = false;

                if(App.LoggedinUser.Name != Fullname.Text)
                {
                    App.LoggedinUser.Name = Fullname.Text;
                    Change = true;
                }
                    
                if(App.LoggedinUser.Email != UserEmail.Text)
                {
                    App.LoggedinUser.Email = UserEmail.Text;
                    Change = true;
                }
                    
                if(App.LoggedinUser.City != UserCity.Text)
                {
                    App.LoggedinUser.City = UserCity.Text;
                    Change = true;
                }
                    
                int j;
                if (Int32.TryParse(UserAge.Text, out j))
                {
                    if(j != App.LoggedinUser.Age)
                    {
                        App.LoggedinUser.Age = j;
                        Change = true;
                    }
                }
                else
                {
                    Change = false;
                }


                if (Change)
                {
                    App.database.UpdateInfo(App.LoggedinUser);
                    await DisplayAlert("Ändrat", "Din information är ändrad.", "Okej");
                }
                else
                    await DisplayAlert("Det blev fel", "Var god skriv in godkänd information i samtliga rutor ovanför.", "Okej");
            }
        }

        public void IngressSwitchToggled(object sender, EventArgs e)
        {
            var properties = App.Current.Properties;
            if (properties.ContainsKey("showingress"))
            {
                properties["showingress"] = IngressSwitch.IsToggled;
            }
        }

        protected override void OnDisappearing()
        {
            Console.WriteLine("Memory Cleanup");
            GC.Collect();
        }
    }
}