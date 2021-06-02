using Terminal.Gui;
using System;
using ClassLib;
using System.Collections.Generic;
namespace ManageData
{
    public class ShowProfileDialog : Dialog
    {
        private TextField username;
        private TextField fullname;
        private DateField date;
        private TimeField time;
        private TextField role;
        private ListView reviews;
        public ShowProfileDialog()
        {
            this.Title = "My profile";
            Button ok = new Button("OK");
            ok.Clicked += DialogCanceled;
            this.AddButton(ok);

            Label usernameLabel = new Label(2, 2, "Username:");
            username = new TextField("")
            {
                X = 20, Y = Pos.Top(usernameLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(usernameLabel, username);

            Label fullnameLabel = new Label(2, 4, "Fullname:");
            fullname = new TextField("")
            {
                X = 20, Y = Pos.Top(fullnameLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(fullnameLabel, fullname);

            Label dateLabel = new Label(2, 6, "Reg date:");
            date = new DateField(DateTime.MinValue.Date)
            {
                X = 20, Y = Pos.Top(dateLabel), Width = Dim.Percent(30),
                IsShortFormat = true
            };
            time = new TimeField(DateTime.MinValue.TimeOfDay)
            {
                X = 30, Y = Pos.Top(dateLabel), Width = Dim.Percent(30),
                IsShortFormat = true
            };
            this.Add(dateLabel, date, time);

            Label roleLabel = new Label(2, 8, "My role:");
            role = new TextField("")
            {
                X = 20, Y = Pos.Top(roleLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(roleLabel,role);

            Label reviewsLabel = new Label(2, 10, "My reviews");
            reviews = new ListView()
            {
                Width = Dim.Fill(), Height = Dim.Fill()
            };
            reviews.OpenSelectedItem += OnOpenReview;

            FrameView frameView = new FrameView("")
            {
                X = 20, Y = Pos.Top(reviewsLabel),
                Width = Dim.Percent(50),
                Height = 5
            };
            frameView.Add(reviews);
            this.Add(reviewsLabel, frameView);
        }
        public void SetUser(User user, List<Review> reviews)
        {
            this.username.Text = user.username;
            this.fullname.Text = user.fullname;
            this.date.Date = user.registrationDate.Date;
            this.date.ReadOnly = true;
            this.time.Time = user.registrationDate.TimeOfDay;
            this.time.ReadOnly = true;
            if(user.isModerator == 0)
            {
                this.role.Text = "user";
            }
            else
            {
                this.role.Text = "moderator";
            }
            if(reviews.Count == 0)
            {
                List<string> emptyText = new List<string>();
                emptyText.Add("There are no reviews.");
                this.reviews.SetSource(emptyText);
            }
            else
            {
                this.reviews.SetSource(reviews);
            }
        }
        private void OnOpenReview(ListViewItemEventArgs args)
        {
            Review currentReview = (Review)args.Value;
            ShowReviewDialog dialog = new ShowReviewDialog();
            dialog.SetReview(currentReview);
            Application.Run(dialog);
        }
        private void DialogCanceled()
        {
            Application.RequestStop();
        }
    }
}