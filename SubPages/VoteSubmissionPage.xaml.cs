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
	public partial class VoteSubmissionPage : ContentPage
	{
		/*
		
			This page contains a form which the user can fill in and send to the database.
			One question, and up to four answers, with a minimum of two answers.
			This form can then be used by the database to allow other users to vote on the filled in answers.
		
		*/
		
		public VoteSubmissionPage ()
		{
			InitializeComponent ();

            Submit.BackgroundColor = App.MC;

        }
        public async void SubmitQ(object sender, EventArgs e)
        {
            if (Question.Text != "" && Op1.Text != "" && Op2.Text != "" && Question.Text != null && Op1.Text != null && Op2.Text != null && Op3.Text != null && Op4.Text != null)
            {
                var VQ = new VoteQuestionTable();
                
                VQ.Question = Question.Text;
                VQ.Option1 = Op1.Text;
                VQ.Option2 = Op2.Text;
                VQ.Option3 = Op3.Text;
                VQ.Option4 = Op4.Text;
                VQ.Posted = DateTime.Now;
                VQ.Stage = 1;
                VQ.TotalVotes1 = 0;
                VQ.TotalVotes2 = 0;
                VQ.TotalVotes3 = 0;
                VQ.TotalVotes4 = 0;
                VQ.Winner = 0;
                App.database.InsertVoteQuestion(VQ);
                await DisplayAlert("Snyggt!", "Din fråga har skickats in.", "Okej");
            }
            else
            {
                await DisplayAlert("Något gick fel", "Kolla om någon textruta är tom!", "Okej");
            }
        }
    }
}