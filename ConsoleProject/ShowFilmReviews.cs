using Terminal.Gui;
using System.Collections.Generic;

namespace ConsoleProject
{
    public class ShowFilmReviews : Dialog
    {
        private Service repo;
        private ListView allReviews;
        private bool user;
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
            Review review = (Review)args.Value;
            ShowReviewDialog dialog = new ShowReviewDialog();
            dialog.SetService(repo);
            dialog.SetReview(review);
            Application.Run(dialog);
            if(!user)
            {
                if(dialog.deleted)
                {
                    repo.reviewRepository.DeleteById(review.id);
                }
                if(dialog.updated)
                {
                    repo.reviewRepository.Update((long)review.id, dialog.GetReview());
                }
            }
            ShowCurrentPage();
        }
        public void SetRepository(Service repo, bool user, int filmId)
        {
            this.repo = repo;
            this.user = user;
            this.filmId = filmId;
            ShowCurrentPage();
        }
        private void ShowCurrentPage()
        {
            this.allReviews.SetSource(repo.reviewRepository.GetAllFilmReviews(filmId));
        }
        private void DialogCanceled()
        {
            Application.RequestStop();
        }
    }
}