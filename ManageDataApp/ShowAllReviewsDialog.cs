using Terminal.Gui;
using System.Collections.Generic;
using ClassLib;
using RPCLib;
namespace ManageDataApp
{
    public class ShowAllReviewsDialog : Dialog
    {
        private RemoteService repo;
        private int page = 1;
        private Label totalPagesLabel;
        private Label currentPageLabel;
        private TextField searchOpinion;
        private string searchValue = "";
        private ListView allReviews;
        private User user;
        public ShowAllReviewsDialog()
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
            Button prevPage = new Button("Prev")
            {
                X = Pos.Center() - 28, Y = 2
            };
            prevPage.Clicked += ClickPrevPage;
            Button nextPage = new Button("Next")
            {
                X = Pos.Center() + 20, Y = 2
            };
            nextPage.Clicked += ClickNextPage;
            currentPageLabel = new Label("?")
            {
                X = Pos.Right(prevPage) + 4, Y = 2,
                Width = 5
            };
            totalPagesLabel = new Label("?")
            {
                X = Pos.Left(nextPage) - 6, Y = 2,
                Width = 5
            };
            FrameView frameView = new FrameView("")
            {
                X = 3, Y = 3,
                Width = Dim.Fill() - 4,
                Height = 12
            };
            frameView.Add(allReviews);

            Label opinionLabel = new Label("Search by opinion:")
            {
                X = 5, Y = Pos.Bottom(frameView)
            };
            searchOpinion = new TextField("")
            {
                X = Pos.Right(opinionLabel) + 1, Y = Pos.Y(opinionLabel), Width = Dim.Percent(50)
            };
            searchOpinion.KeyPress += OnSearchOpinionEnter;

            this.Add(title, prevPage, nextPage, currentPageLabel, totalPagesLabel, frameView, opinionLabel, searchOpinion);
        }
        private void OnSearchOpinionEnter(KeyEventEventArgs args)
        {
            if(args.KeyEvent.Key == Key.Enter)
            {
                page = 1;
                this.searchValue = this.searchOpinion.Text.ToString();
                ShowCurrentPage();
            }
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
            if(page > repo.reviewRepository.GetSearchPagesCount(searchValue) && page > 1)
            {
                page--;
            }
            if(dialog.updated)
            {
                repo.reviewRepository.Update((long)review.id, dialog.GetReview());
            }
            ShowCurrentPage();
        }
        public void SetRepository(RemoteService repo)
        {
            this.repo = repo;
            ShowCurrentPage();
        }
        public void SetUser(User user)
        {
            this.user = user;
        }
        private void ClickPrevPage()
        {
            if(page == 1)
            {
                return;
            }
            this.page--;
            ShowCurrentPage();
        }
        private void ClickNextPage()
        {
            int totalPages = repo.reviewRepository.GetSearchPagesCount(searchValue);
            if(page == totalPages)
            {
                return;
            }
            this.page++;
            ShowCurrentPage();
        }
        private void ShowCurrentPage()
        {
            int totalPages = repo.reviewRepository.GetSearchPagesCount(searchOpinion.Text.ToString());
            if(page > totalPages && page > 1)
            {
                page = totalPages;
            }
            if(totalPages != 0 && page == 0)
            {
                page = 1;
            }
            if(totalPages == 0)
            {
                this.page = 0;
                this.currentPageLabel.Text = this.page.ToString();
                this.totalPagesLabel.Text = totalPages.ToString();
                List<string> emptyText = new List<string>();
                emptyText.Add("There are no reviews in the database.");
                this.allReviews.SetSource(emptyText);
            }
            else
            {
                this.currentPageLabel.Text = this.page.ToString();
                this.totalPagesLabel.Text = totalPages.ToString();
                this.allReviews.SetSource(repo.reviewRepository.GetSearchPage(searchOpinion.Text.ToString(), page));
            }
        }
        private void DialogCanceled()
        {
            Application.RequestStop();
        }
    }
}