using Terminal.Gui;
using ClassLib;
namespace ManageData
{
    public class ShowReviewDialog : Dialog
    {
        private TextField idField;
        private TextView opinionField;
        private TextField ratingField;
        private DateField postedAtDateField;
        private TimeField postedAtTimeField;
        private TextField filmField;
        private TextField userField;
        public bool deleted;
        public bool updated;
        private Review reviewToShow;
        private Service repo;
        private User user;
        private Button delete;
        private Button edit;
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
            opinionField = new TextView()
            {
                X = 20, Y = Pos.Top(opinionLabel), Width = Dim.Percent(50),
                Height = 3, ReadOnly = true
            };
            this.Add(opinionLabel, opinionField);

            Label ratingLable = new Label(2, 8, "Rating:");
            ratingField = new TextField("")
            {
                X = 20, Y = Pos.Top(ratingLable), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(ratingLable, ratingField);

            Label postedAtLable = new Label(2, 10, "Posted at:");
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

            Label filmLabel = new Label(2, 12, "For film:");
            filmField = new TextField("")
            {
                X = 20, Y = Pos.Top(filmLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(filmLabel, filmField);

            Label userLabel = new Label(2, 14, "By user:");
            userField = new TextField("")
            {
                X = 20, Y = Pos.Top(userLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(userLabel, userField);

            delete = new Button(2, 16, "Delete");
            delete.Clicked += OnDeleteReview;
            edit = new Button("Edit")
            {
                X = delete.X, Y = 17
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
                MessageBox.Query("Edit", "Review was updated!", "OK");
                Review updated = dialog.GetReviewEdit();
                updated.id = reviewToShow.id;
                updated.postedAt = reviewToShow.postedAt;
                updated.userId = reviewToShow.userId;
                updated.filmId = reviewToShow.filmId;
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
        public void SetUser(User user)
        {
            this.user = user;
            if(user.id == reviewToShow.userId)
            {
                delete.Visible = true;
                edit.Visible = true;
            }
            else if(user.isModerator == 1)
            {
                delete.Visible = true;
                edit.Visible = false;
            }
            else
            {
                delete.Visible = false;
                edit.Visible = false;
            }
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
            this.filmField.Text = repo.filmRepository.GetById(review.filmId).ToString();
            this.userField.Text = repo.userRepository.GetById(review.userId).username.ToString();
        }
        private void DialogCanceled()
        {
            Application.RequestStop();
        }
    }
}