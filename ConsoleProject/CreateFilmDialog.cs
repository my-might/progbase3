using Terminal.Gui;
using System;
namespace ConsoleProject
{
    public class CreateFilmDialog : Dialog
    {
        public bool canceled;
        protected TextField inputTitle;
        protected TextField inputGenre;
        protected TextField inputDescription;
        protected TextField inputRelease;
        public CreateFilmDialog()
        {
            this.Title = "Create film";
            Button ok = new Button("OK");
            ok.Clicked += DialogSubmit;
            Button cancel = new Button("Cancel"); 
            cancel.Clicked += DialogCanceled;
            this.AddButton(ok);
            this.AddButton(cancel);

            Label filmTitle = new Label(2, 2, "Title:");
            inputTitle = new TextField("")
            {
                X = 20, Y = Pos.Top(filmTitle), Width = Dim.Percent(50)
            };
            this.Add(filmTitle, inputTitle);

            Label filmGenre = new Label(2, 4, "Genre:");
            inputGenre = new TextField("")
            {
                X = 20, Y = Pos.Top(filmGenre), Width = Dim.Percent(50)
            };
            this.Add(filmGenre, inputGenre);

            Label filmDescription = new Label(2, 6, "Description:");
            inputDescription = new TextField("")
            {
                X = 20, Y = Pos.Top(filmDescription), Width = Dim.Percent(50)
            };
            this.Add(filmDescription, inputDescription);

            Label filmRelease = new Label(2, 8, "Release year:");
            inputRelease = new TextField("")
            {
                X = 20, Y = Pos.Top(filmRelease), Width = Dim.Percent(50)
            };
            this.Add(filmRelease, inputRelease);
        }
        public Film GetFilm()
        {
            return new Film(){title = inputTitle.Text.ToString(),
                            genre = inputGenre.Text.ToString(),
                            description = inputDescription.Text.ToString(),
                            releaseYear = int.Parse(inputRelease.Text.ToString())};
        }
        private void DialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void DialogSubmit()
        {
            string errorText = "";
            if(inputTitle.Text.ToString() == "")
            {
                errorText = "Title field mustn`t be empty.";
            }
            else if(inputGenre.Text.ToString() == "")
            {
                errorText = "Genre field mustn`t be empty.";
            }
            else if(inputDescription.Text.ToString() == "")
            {
                errorText = "Description field mustn`t be empty.";
            }
            else if(inputRelease.Text.ToString() == "")
            {
                errorText = "Release year field mustn`t be empty.";
            }
            else if(!int.TryParse(inputRelease.Text.ToString(), out int year) || year > DateTime.Now.Year || year < 1895)
            {
                errorText = $"Release year must be integer from 1895 to {DateTime.Now.Year}.";
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