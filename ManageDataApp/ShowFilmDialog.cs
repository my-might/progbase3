using Terminal.Gui;
using System.Collections.Generic;
using ClassLib;
using RPCLib;
namespace ManageDataApp
{
    public class ShowFilmDialog : Dialog
    {
        private TextField idField;
        private TextField titleField;
        private TextField genreField;
        private TextView descriptionField;
        private TextField yearField;
        private ListView rolesView;
        private TextField ratingField;
        private List<int> updatedRoles;
        public bool deleted;
        public bool updated;
        public Film filmToShow;
        private RemoteService repo;
        private User user;
        private Button delete;
        private Button edit;
        public ShowFilmDialog()
        {
            this.Title = "Show film";
            Button ok = new Button("OK");
            ok.Clicked += DialogCanceled;
            this.AddButton(ok);
            Label idLabel = new Label(2, 1, "ID:");
            idField = new TextField("")
            {
                X = 20, Y = Pos.Top(idLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(idLabel, idField);

            Label titleLabel = new Label(2, 3, "Title:");
            titleField = new TextField("")
            {
                X = 20, Y = Pos.Top(titleLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(titleLabel, titleField);

            Label genreLabel = new Label(2, 5, "Genre:");
            genreField = new TextField("")
            {
                X = 20, Y = Pos.Top(genreLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(genreLabel, genreField);

            Label descriptionLabel = new Label(2, 7, "Description:");
            descriptionField = new TextView()
            {
                X = 20, Y = Pos.Top(descriptionLabel), Width = Dim.Percent(50),
                Height = 3, ReadOnly = true
            };
            this.Add(descriptionLabel, descriptionField);

            Label yearLabel = new Label(2, 11, "Release year:");
            yearField = new TextField("")
            {
                X = 20, Y = Pos.Top(yearLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(yearLabel, yearField);

            Label rolesLabel = new Label(2, 13, "Starring actors:");
            rolesView = new ListView(new List<Actor>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            Label ratingLabel = new Label(2, 17, "Average rating:");
            ratingField = new TextField("")
            {
                X = 20, Y = Pos.Top(ratingLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(ratingLabel, ratingField);

            FrameView frameView = new FrameView("")
            {
                X = 20, Y = Pos.Top(rolesLabel),
                Width = Dim.Percent(50),
                Height = 3
            };
            frameView.Add(rolesView);
            this.Add(rolesLabel, frameView);

            delete = new Button(2, 19, "Delete");
            delete.Clicked += OnDeleteFilm;
            edit = new Button("Edit")
            {
                X = delete.X, Y = 20
            };
            edit.Clicked += OnEditFilm;
            Button showReviews = new Button(2, 21, "Show reviews");
            showReviews.Clicked += OnShowReviews;
            Button createReview = new Button(2, 22, "Write review");
            createReview.Clicked += OnWriteReview;
            this.Add(delete, edit, showReviews, createReview);
        }
        private void OnDeleteFilm()
        {
            int index = MessageBox.Query("Delete film", "Are you sure?", "No", "Yes");
            if(index == 1)
            {
                this.deleted = true;
                Application.RequestStop();
            }
        }
        private void OnWriteReview()
        {
            CreateReviewDialog dialog = new CreateReviewDialog();
            dialog.SetService(repo);
            dialog.CreateForFilm(filmToShow.id);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Review review = dialog.GetReview();
                review.userId = user.id;
                repo.reviewRepository.Insert(review);
            }
            SetFilm(filmToShow);
        }
        private void OnShowReviews()
        {
            ShowFilmReviews dialog = new ShowFilmReviews();
            dialog.SetRepository(repo, user, filmToShow.id);
            Application.Run(dialog);
            SetFilm(filmToShow);
        }
        private void OnEditFilm()
        {
            EditFilmDialog dialog = new EditFilmDialog();
            dialog.SetFilm(this.filmToShow);
            dialog.SetRepository(this.repo.actorRepository);
            Application.Run(dialog);

            if(!dialog.canceled)
            {
                MessageBox.Query("Edit", "Film was updated!", "OK");
                Film updatedFilm = dialog.GetFilm();
                updatedFilm.id = filmToShow.id;
                this.updatedRoles = dialog.GetActorIds();
                updatedFilm.actors = ListToArray(ListIntToActor(updatedRoles));
                this.updated = true;
                SetFilm(updatedFilm);
            }
        }
        public Film GetFilm()
        {
            return this.filmToShow;
        }
        public void SetFilm(Film film)
        {
            this.filmToShow = film;
            this.idField.Text = film.id.ToString();
            this.titleField.Text = film.title;
            this.genreField.Text = film.genre;
            this.descriptionField.Text = film.description;
            this.yearField.Text = film.releaseYear.ToString();
            if(film.actors != null)
            {
                List<Actor> actors = ArrayToList(film.actors);
                this.rolesView.SetSource(actors);
            }
            else
            {
                List<string> emptyText = new List<string>();
                emptyText.Add("There are no actors.");
                this.rolesView.SetSource(emptyText);
            }
            double rating = repo.reviewRepository.GetAverageFilmRating(film.id);
            if(repo.reviewRepository.GetAllFilmReviews(film.id).Count != 0)
            {
                this.ratingField.Text = repo.reviewRepository.GetAverageFilmRating(film.id).ToString();
            }
            else
            {
                this.ratingField.Text = "no reviews";
            }
        }
        private List<Actor> ListIntToActor(List<int> input)
        {
            List<Actor> actors = new List<Actor>();
            foreach(int id in input)
            {
                actors.Add(repo.actorRepository.GetById(id));
            }
            return actors;
        }
        private Actor[] ListToArray(List<Actor> actors)
        {
            Actor[] result = null;
            if(actors.Count != 0)
            {
                result = new Actor[actors.Count];
                for(int i = 0; i<actors.Count; i++)
                {
                    result[i] = actors[i];
                }
            }
            return result;
        }
        public void SetService(RemoteService repo)
        {
            this.repo = repo;
        }
        public void SetUser(User user)
        {
            this.user = user;
            if(user.isModerator == 0)
            {
                delete.Visible = false;
                edit.Visible = false;
            }
        }
        private List<Actor> ArrayToList(Actor[] actors)
        {
            List<Actor> result = new List<Actor>();
            foreach(Actor actor in actors)
            {
                result.Add(actor);
            }
            return result;
        }
        public List<int> GetUpdatedRoles()
        {
            return this.updatedRoles;
        }
        private void DialogCanceled()
        {
            Application.RequestStop();
        }
    }
}