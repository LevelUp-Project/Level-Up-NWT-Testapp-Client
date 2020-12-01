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
	public partial class UserSubmissionPage : ContentPage
	{
		/*
		
			This page allows the user to submit their own article, by using text input and then sending it to the database.
			
		*/
		
		public UserSubmissionPage ()
		{
			InitializeComponent ();
            ClassId = "-1";
		}

        public void Submit(object sender, EventArgs e)
        {
            if(App.LoggedinUser != null && Rubrik.Text != "" && Ingress.Text != "" && Brodtext.Text != "")
            {
				//Submit to database.
			}
        }
        async public void GetReference(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewsGridPage(1));
        }        
	}
}