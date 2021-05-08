using Terminal.Gui;
using System;
using Microsoft.Data.Sqlite;
namespace ConsoleProject
{
    public class CreateReviewDialog : Dialog
    {
        public bool canceled;
        private TextField inputOpinion;
        private TextField inputRating;
        private TextField inputFilmId;
        private Service repo;
        public CreateReviewDialog(Service repo)
        {
            this.repo = repo;
            this.Title = "Create review";
            Button ok = new Button("OK");
            ok.Clicked += DialogSubmit;
            Button cancel = new Button("Cancel"); 
            cancel.Clicked += DialogCanceled;
            this.AddButton(ok);
            this.AddButton(cancel);

            Label reviewOpinion = new Label(2, 2, "Opinion:");
            inputOpinion = new TextField("")
            {
                X = 20, Y = Pos.Top(reviewOpinion), Width = Dim.Percent(50)
            };
            this.Add(reviewOpinion, inputOpinion);

            Label reviewRating = new Label(2, 4, "Rating:");
            inputRating = new TextField("")
            {
                X = 20, Y = Pos.Top(reviewRating), Width = Dim.Percent(50)
            };
            this.Add(reviewRating, inputRating);

            Label reviewFilmId = new Label(2, 6, "Film id:");
            inputFilmId = new TextField("")
            {
                X = 20, Y = Pos.Top(reviewFilmId), Width = Dim.Percent(50)
            };
            this.Add(reviewFilmId, inputFilmId);
        }
        public Review GetReview()
        {
            return new Review(){opinion = inputOpinion.Text.ToString(),
                            rating = int.Parse(inputRating.Text.ToString()),
                            postedAt = DateTime.Now,
                            userId = 1,
                            filmId = int.Parse(inputFilmId.Text.ToString())};
        }
        private void DialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void DialogSubmit()
        {
            string errorText = "";
            if(inputOpinion.Text.ToString() == "")
            {
                errorText = "Opinion field mustn`t be empty.";
            }
            else if(inputRating.Text.ToString() == "")
            {
                errorText = "Rating field mustn`t be empty.";
            }
            else if(!int.TryParse(inputRating.Text.ToString(), out int rating) || rating > 10 || rating < 1)
            {
                errorText = "Rating must be integer from 1 to 10.";
            }
            else if(inputFilmId.Text.ToString() == "")
            {
                errorText = "Film id field mustn`t be empty.";
            }
            else if(!int.TryParse(inputFilmId.Text.ToString(), out int id) )
            {
                errorText = "Film id must be integer.";
            }
            else if(repo.filmRepository.GetById(id) == null)
            {
                errorText = "Entered film id does not exist in the database.";
            }
            if(errorText != "")
            {
                MessageBox.ErrorQuery("Error", errorText, "OK");
            }
            else
            {
                this.canceled = false;
                Application.RequestStop();
            }
        }
    }
}