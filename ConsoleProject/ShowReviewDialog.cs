using Terminal.Gui;
namespace ConsoleProject
{
    public class ShowReviewDialog : Dialog
    {
        private TextField idField;
        private TextField opinionField;
        private TextField ratingField;
        private DateField postedAtDateField;
        private TimeField postedAtTimeField;
        private TextField filmIdField;
        public bool deleted;
        public bool updated;
        public Review reviewToShow;
        private Service repo;
        public ShowReviewDialog()
        {
            this.Title = "Show review";
            Button ok = new Button("OK");
            ok.Clicked += DialogCanceled;
            this.AddButton(ok);
            Label idLabel = new Label(2, 2, "ID:");
            idField = new TextField("")
            {
                X = 20, Y = Pos.Top(idLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(idLabel, idField);

            Label opinionLabel = new Label(2, 4, "Opinion:");
            opinionField = new TextField("")
            {
                X = 20, Y = Pos.Top(opinionLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(opinionLabel, opinionField);

            Label ratingLable = new Label(2, 6, "Rating:");
            ratingField = new TextField("")
            {
                X = 20, Y = Pos.Top(ratingLable), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(ratingLable, ratingField);

            Label postedAtLable = new Label(2, 8, "Posted at:");
            postedAtDateField = new DateField()
            {
                X = 20, Y = Pos.Top(postedAtLable), Width = Dim.Percent(30),
                IsShortFormat = true
            };
            postedAtTimeField = new TimeField()
            {
                X = 30, Y = Pos.Top(postedAtLable), Width = Dim.Percent(30),
                IsShortFormat = true
            };
            this.Add(postedAtLable, postedAtDateField, postedAtTimeField);

            Label filmIdLabel = new Label(2, 10, "Film id:");
            filmIdField = new TextField("")
            {
                X = 20, Y = Pos.Top(filmIdLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(filmIdLabel, filmIdField);
            Button delete = new Button(2, 12, "Delete");
            delete.Clicked += OnDeleteReview;
            Button edit = new Button("Edit")
            {
                X = delete.X, Y = 13
            };
            edit.Clicked += OnEditReview;
            this.Add(delete, edit);
        }
        private void OnDeleteReview()
        {
            int index = MessageBox.Query("Delete review", "Are you sure?", "No", "Yes");
            if(index == 1)
            {
                this.deleted = true;
                Application.RequestStop();
            }
        }
        private void OnEditReview()
        {
            EditReviewDialog dialog = new EditReviewDialog();
            dialog.SetReview(this.reviewToShow);
            dialog.SetService(repo);
            Application.Run(dialog);

            if(!dialog.canceled)
            {
                Review updated = dialog.GetReviewEdit();
                updated.id = reviewToShow.id;
                updated.postedAt = reviewToShow.postedAt;
                updated.userId = reviewToShow.userId;
                this.updated = true;
                this.SetReview(updated);
            }
        }
        public Review GetReview()
        {
            return this.reviewToShow;
        }
        public void SetService(Service repo)
        {
            this.repo = repo;
        }

        public void SetReview(Review review)
        {
            this.reviewToShow = review;
            this.idField.Text = review.id.ToString();
            this.opinionField.Text = review.opinion;
            this.ratingField.Text = review.rating.ToString();
            this.postedAtDateField.Date = review.postedAt.Date;
            this.postedAtDateField.ReadOnly = true;
            this.postedAtTimeField.Time = review.postedAt.TimeOfDay;
            this.postedAtTimeField.ReadOnly = true;
            this.filmIdField.Text = review.filmId.ToString();
        }
        private void DialogCanceled()
        {
            Application.RequestStop();
        }
    }
}