using Terminal.Gui;
using System.Collections.Generic;
using ClassLib;
using RPCLib;
namespace ManageDataApp
{
    public class MainWindowUser : Window
    {
        private RemoteService repo;
        private Button viewReviews;
        private Button createActor;
        private Button createFilm;
        private Button promoteUser;
        private User currentUser;
        private Label loggedUser;
        private Label logged;
        
        private bool isModerator;
        public MainWindowUser()
        {
            this.Title = "e-base of films and actors";
            this.X = 0;
            this.Y = 1;

            logged = new Label(" ")
            {
                X = Pos.Center(), Y = 5, Width = 40
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
            viewReviews = new Button("View all reviews")
            {
                X = Pos.Center() + 5, Y = 15
            };
            viewReviews.Clicked += ClickShowReviews;
            Button createReview = new Button("Write review")
            {
                X = Pos.Center() - 20, Y = 12
            };
            createReview.Clicked += ClickCreateReview;

            createActor = new Button("Create actor")
            {
                X = Pos.Center() - 20, Y = 15
            };
            createActor.Clicked += ClickCreateActor;

            createFilm = new Button("Create film")
            {
                X = Pos.Center() - 20, Y = 18
            };
            createFilm.Clicked += ClickCreateFilm;

            promoteUser = new Button("Promote user")
            {
                X = Pos.Center() + 5, Y = 18
            };
            promoteUser.Clicked += ClickPromoteUser;

            this.Add(profile, viewFilms, viewActors, viewReviews, createReview, createFilm, createActor, promoteUser);

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
                logged.Text = "You are successfully logged as user.";
                isModerator = false;
                createActor.Visible = false;
                createFilm.Visible = false;
                promoteUser.Visible = false;
            }
            else
            {
                logged.Text = "You are successfully logged as moderator.";
                isModerator = true;
            }
        }
        private void OnLogOut()
        {
            Application.Top.RemoveAll();
            UserInterface.ProcessRegistration();
        }
        private void ClickPromoteUser()
        {
            PromoteUserDialog dialog = new PromoteUserDialog();
            dialog.SetService(repo);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                string username = dialog.GetUsername();
                User user = repo.userRepository.GetByUsername(username);
                user.isModerator = 1;
                repo.userRepository.Update(user.id, user);
            }
        }
        private void ClickCreateFilm()
        {
            CreateFilmDialog dialog = new CreateFilmDialog();
            dialog.SetRepository(repo.actorRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Film film = dialog.GetFilm();
                int insertedId = (int)repo.filmRepository.Insert(film);
                film.id = insertedId;
                List<int> actorIds = dialog.GetActorIds();
                foreach(int id in actorIds)
                {
                    Role currentRole = new Role(){actorId = id, filmId = insertedId};
                    repo.roleRepository.Insert(currentRole);
                }
                List<Actor> roles = repo.roleRepository.GetAllActors(film.id);
                if(roles.Count != 0)
                {
                    film.actors = new Actor[roles.Count];
                    roles.CopyTo(film.actors);
                }
                else
                {
                    film.actors = null;
                }
                ShowFilmDialog filmDialog = new ShowFilmDialog();
                filmDialog.SetService(repo);
                filmDialog.SetFilm(film);
                filmDialog.SetUser(currentUser);
                Application.Run(filmDialog);
                if(filmDialog.deleted)
                {
                    repo.filmRepository.DeleteById(film.id);
                    repo.roleRepository.DeleteByFilmId(film.id);
                    repo.reviewRepository.DeleteByFilmId(film.id);
                }
                if(filmDialog.updated)
                {
                    repo.filmRepository.Update((long)film.id, dialog.GetFilm());
                    List<int> updatedRoles = filmDialog.GetUpdatedRoles();
                        foreach(int actorId in updatedRoles)
                        {
                            if(!repo.roleRepository.IsExist(film.id, actorId))
                            {
                                Role currentRole = new Role(){actorId = actorId, filmId = film.id};
                                repo.roleRepository.Insert(currentRole);
                            }
                        }
                        foreach(Actor actor in roles)
                        {
                            bool isExist = false;
                            if(updatedRoles.Count != 0)
                            {
                                for(int i = 0; i<updatedRoles.Count; i++)
                                {
                                    if(actor.id == updatedRoles[i])
                                    {
                                        isExist = true;
                                        break;
                                    }
                                }
                            }
                            if(!isExist)
                            {
                                repo.roleRepository.Delete(actor.id, film.id);
                            }
                        }
                }
            }
        }
        private void ClickCreateActor()
        {
            CreateActorDialog dialog = new CreateActorDialog();
            dialog.SetRepository(repo.filmRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Actor actor = dialog.GetActor();
                int insertedId = (int)repo.actorRepository.Insert(actor);
                actor.id = insertedId;
                List<int> filmIds = dialog.GetFilmIds();
                foreach(int id in filmIds)
                {
                    Role currentRole = new Role(){actorId = insertedId, filmId = id};
                    repo.roleRepository.Insert(currentRole);
                }
                List<Film> roles = repo.roleRepository.GetAllFilms(actor.id);
                if(roles.Count != 0)
                {
                    actor.films = new Film[roles.Count];
                    roles.CopyTo(actor.films);
                }
                else
                {
                    actor.films = null;
                }
                ShowActorDialog actorDialog = new ShowActorDialog();
                actorDialog.SetActor(actor);
                actorDialog.SetRepository(repo.filmRepository);
                actorDialog.SetUser(currentUser);
                Application.Run(actorDialog);
                if(actorDialog.deleted)
                {
                    repo.actorRepository.DeleteById(actor.id);
                    repo.roleRepository.DeleteByActorId(actor.id);
                }
                if(actorDialog.updated)
                {
                    repo.actorRepository.Update((long)actor.id, actorDialog.GetActor());
                    List<int> updatedRoles = actorDialog.GetUpdatedRoles();
                        foreach(int filmId in updatedRoles)
                        {
                            if(!repo.roleRepository.IsExist(filmId, actor.id))
                            {
                                Role currentRole = new Role(){actorId = actor.id, filmId = filmId};
                                repo.roleRepository.Insert(currentRole);
                            }
                        }
                        foreach(Film film in roles)
                        {
                            bool isExist = false;
                            if(updatedRoles.Count != 0)
                            {
                                for(int i = 0; i<updatedRoles.Count; i++)
                                {
                                    if(film.id == updatedRoles[i])
                                    {
                                        isExist = true;
                                        break;
                                    }
                                }
                            }
                            if(!isExist)
                            {
                                repo.roleRepository.Delete(actor.id, film.id);
                            }
                        }
                }
            }
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
                review.id = (int)repo.reviewRepository.Insert(review);
                ShowReviewDialog showDialog = new ShowReviewDialog();
                showDialog.SetService(repo);
                showDialog.SetReview(review);
                showDialog.SetUser(currentUser);
                Application.Run(showDialog);
                if(showDialog.deleted)
                {
                    repo.reviewRepository.DeleteById(review.id);
                }
                if(showDialog.updated)
                {
                    repo.reviewRepository.Update((long)review.id, dialog.GetReview());
                }
            }
        }
        private void ClickShowProfile()
        {
            ShowProfileDialog dialog = new ShowProfileDialog();
            dialog.SetService(repo);
            dialog.SetUser(currentUser);
            dialog.SetReviews();
            Application.Run(dialog);
        }
        private void ClickShowFilms()
        {
            ShowAllFilmsDialog dialog = new ShowAllFilmsDialog();
            dialog.SetRepository(repo);
            dialog.SetUser(currentUser);
            Application.Run(dialog);
        }
        private void ClickShowActors()
        {
            ShowAllActorsDialog dialog = new ShowAllActorsDialog();
            dialog.SetRepository(repo);
            dialog.SetUser(currentUser);
            Application.Run(dialog);
        }
        private void ClickShowReviews()
        {
            ShowAllReviewsDialog dialog = new ShowAllReviewsDialog();
            dialog.SetRepository(repo);
            dialog.SetUser(currentUser);
            Application.Run(dialog);
        }
        public void SetService(RemoteService repo)
        {
            this.repo = repo;
        }
    }
}