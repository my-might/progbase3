using Terminal.Gui;
using System.Collections.Generic;
using ClassLib;
namespace ManageData
{
    public class MainWindowUser : Window
    {
        private Service repo;
        private TextField idToShow;
        private User currentUser;
        private Label loggedUser;
        private bool isModerator;
        public MainWindowUser()
        {
            this.Title = "e-base of films and actors";
            this.X = 0;
            this.Y = 1;

            Label logged = new Label("You are successfully logged in as user.")
            {
                X = Pos.Center(), Y = 5
            };
            Label available = new Label("There are available options for you:")
            {
                X = Pos.Center(), Y = 6
            };
            this.Add(logged, available);

            Button profile = new Button("My profile")
            {
                X = Pos.Center() - 20, Y = 9
            };
            profile.Clicked += ClickShowProfile;
            Button viewFilms = new Button("View all films")
            {
                X = Pos.Center() + 5, Y = 9
            };
            viewFilms.Clicked += ClickShowFilms;
            Button viewActors = new Button("View all actors")
            {
                X = Pos.Center() + 5, Y = 12
            };
            viewActors.Clicked += ClickShowActors;
            Button createReview = new Button("Write review")
            {
                X = Pos.Center() - 20, Y = 12
            };
            createReview.Clicked += ClickCreateReview;
            this.Add(profile, viewFilms, viewActors, createReview);
            Label idLabel = new Label("ID:")
            {
                X = Pos.Center() - 7, Y = 14
            };
            idToShow = new TextField("")
            {
                X = Pos.Center(), Y = 14, Width = Dim.Percent(5)
            };
            Button showFilms = new Button("Find film")
            {
                X = Pos.Center(), Y = 15
            };
            showFilms.Clicked += ClickFindFilm;
            Button showActors = new Button("Find actor")
            {
                X = Pos.Center(), Y = 16
            };
            showActors.Clicked += ClickFindActor;
            this.Add(idLabel, idToShow, showFilms, showActors);

            loggedUser = new Label(" ")
            {
                X = Pos.Percent(2), Y = Pos.Percent(2),
                Width = Dim.Fill() - 20
            };
            
            Button logOut = new Button("Log out")
            {
                X = Pos.Percent(85), Y = Pos.Y(loggedUser)
            };
            logOut.Clicked += OnLogOut;
            this.Add(loggedUser, logOut);

        }
        public void SetUser(User user)
        {
            this.currentUser = user;
            this.loggedUser.Text = $"Logged user: {user.username}";
            if(user.isModerator == 0)
            {
                isModerator = false;
            }
            else
            {
                isModerator = true;
            }
        }
        private void OnLogOut()
        {
            Application.Top.RemoveAll();
            UserInterface.ProcessRegistration();
        }
        private void ClickCreateReview()
        {
            CreateReviewDialog dialog = new CreateReviewDialog();
            dialog.SetService(repo);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Review review = dialog.GetReview();
                review.userId = currentUser.id;
                repo.reviewRepository.Insert(review);
            }
        }
        private void ClickShowProfile()
        {
            List<Review> userReviews = repo.reviewRepository.GetAllUserReviews(currentUser.id);
            ShowProfileDialog dialog = new ShowProfileDialog();
            dialog.SetUser(currentUser, userReviews);
            Application.Run(dialog);
        }
        private void ClickFindFilm()
        {
            string errorText = "";
            int id = 0;
            if(idToShow.Text.ToString() == "")
            {
                errorText = "ID field mustn`t be empty.";
            }
            else if(!int.TryParse(idToShow.Text.ToString(), out id) || id < 1)
            {
                errorText = "ID must be positive integer.";
            }
            else if(repo.filmRepository.GetById(id) == null)
            {
                errorText = $"Film with id [{id}] doesn`t exist.";
            }
            if(errorText != "")
            {
                MessageBox.ErrorQuery("Error", errorText, "OK");
                return;
            }
            else
            {
                ShowFilmDialog dialog = new ShowFilmDialog();
                Film filmToSet = repo.filmRepository.GetById(id);
                List<Actor> roles = repo.roleRepository.GetAllActors(id);
                filmToSet.actors = new Actor[roles.Count];
                roles.CopyTo(filmToSet.actors);
                dialog.SetFilm(filmToSet);
                dialog.SetService(repo, true);
                Application.Run(dialog);
            }
        }
        private void ClickFindActor()
        {
            string errorText = "";
            int id = 0;
            if(idToShow.Text.ToString() == "")
            {
                errorText = "ID field mustn`t be empty.";
            }
            else if(!int.TryParse(idToShow.Text.ToString(), out id) || id < 1)
            {
                errorText = "ID must be positive integer.";
            }
            else if(repo.actorRepository.GetById(id) == null)
            {
                errorText = $"Actor with id [{id}] doesn`t exist.";
            }
            if(errorText != "")
            {
                MessageBox.ErrorQuery("Error", errorText, "OK");
                return;
            }
            else
            {
                ShowActorDialog dialog = new ShowActorDialog();
                Actor actorToSet = repo.actorRepository.GetById(id);
                List<Film> roles = repo.roleRepository.GetAllFilms(id);
                actorToSet.films = new Film[roles.Count];
                roles.CopyTo(actorToSet.films);
                dialog.SetActor(actorToSet);
                dialog.SetRepository(repo.filmRepository);
                Application.Run(dialog);
            }
        }
        private void ClickShowFilms()
        {
            ShowAllFilmsDialog dialog = new ShowAllFilmsDialog();
            dialog.SetRepository(repo);
            Application.Run(dialog);
        }
        private void ClickShowActors()
        {
            ShowAllActorsDialog dialog = new ShowAllActorsDialog();
            dialog.SetRepository(repo);
            Application.Run(dialog);
        }
        public void SetService(Service repo)
        {
            this.repo = repo;
        }
    }
}