using Terminal.Gui;
using System.Collections.Generic;
using ClassLib;

namespace ManageData
{
    public class ShowFilmReviews : Dialog
    {
        private Service repo;
        private ListView allReviews;
        private User user;
        private int filmId;
        public ShowFilmReviews()
        {
            Button ok = new Button("OK");
            ok.Clicked += DialogCanceled;
            this.AddButton(ok);
            allReviews = new ListView(new List<Review>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            allReviews.OpenSelectedItem += OnOpenReview;
            Label title = new Label("Reviews list")
            {
                X = Pos.Center(), Y = 0
            };
            FrameView frameView = new FrameView("")
            {
                X = 3, Y = 3,
                Width = Dim.Fill() - 4,
                Height = 12
            };
            frameView.Add(allReviews);
            this.Add(title, frameView);
        }
        private void OnOpenReview(ListViewItemEventArgs args)
        {
            Review review = new Review();
            try
            {
                review = (Review)args.Value; 
            }
            catch
            {
                return;
            }
            ShowReviewDialog dialog = new ShowReviewDialog();
            dialog.SetService(repo);
            dialog.SetReview(review);
            dialog.SetUser(user);
            Application.Run(dialog);
            if(dialog.deleted)
            {
                repo.reviewRepository.DeleteById(review.id);
            }
            if(dialog.updated)
            {
                repo.reviewRepository.Update((long)review.id, dialog.GetReview());
            }
            ShowCurrentPage();
        }
        public void SetRepository(Service repo, User user, int filmId)
        {
            this.repo = repo;
            this.user = user;
            this.filmId = filmId;
            ShowCurrentPage();
        }
        private void ShowCurrentPage()
        {
            List<Review> reviews = repo.reviewRepository.GetAllFilmReviews(filmId);
            if(reviews.Count != 0)
            {
                this.allReviews.SetSource(reviews);
            }
            else
            {
                List<string> emptyText = new List<string>();
                emptyText.Add("There are no reviews for this film.");
                this.allReviews.SetSource(emptyText);
            }
        }
        private void DialogCanceled()
        {
            Application.RequestStop();
        }
    }
}