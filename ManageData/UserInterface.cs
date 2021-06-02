using Terminal.Gui;
using System.Collections.Generic;
using ClassLib;
namespace ManageData
{
    public static class UserInterface
    {
        private static Service repo;
        
        private static TextField idToShow;
        private static Toplevel top;
        
        public static void SetService(Service repo1)
        {
            repo = repo1;
        }
        public static void ProcessApplication()
        {
            Application.Init();
            top = Application.Top;
            ProcessRegistration();
        }
        private static void OnQuit()
        {
            Application.RequestStop();
        }
        public static void ProcessRegistration()
        {
            RegistrationDialog dialog = new RegistrationDialog();
            dialog.SetRepository(repo.userRepository);
            top.Add(dialog);
            Application.Run();
            if(!dialog.canceled)
            {
                User registered = dialog.GetUser();
                MenuBar menu = new MenuBar(new MenuBarItem[] {
                    new MenuBarItem ("_File", new MenuItem [] {
                        new MenuItem ("_Export Films", "", Export),
                        new MenuItem ("_Import Films", "", Import),
                        new MenuItem ("_Exit", "", OnQuit)
                    }),
                    new MenuBarItem ("_Help", new MenuItem [] {
                        new MenuItem ("Help!", "", null)
                    })
                });
                MainWindowUser userWin = new MainWindowUser();
                userWin.SetService(repo);
                userWin.SetUser(registered);
                top.Add(userWin, menu);
                Application.Run();
            }
        }
        public static void Export()
        {
            Export dialog = new Export();
            dialog.SetRepository(repo.actorRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Xml.SetRepository(repo.filmRepository);
                Xml.ExportData(repo.roleRepository.GetAllFilms(int.Parse(dialog.actorIdField.Text.ToString())), dialog.directory.Text.ToString());
            }
        }
        public static void Import()
        {
            Import dialog = new Import();
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                try
                {
                    Xml.SetRepository(repo.filmRepository);
                    Xml.ImportData(dialog.directory.Text.ToString());
                }
                catch
                {
                    MessageBox.ErrorQuery("Error", "Cannot import data", "OK");
                }
            }
        }
        public static void ProcessClickCreate()
        {
            Window win = new Window("Select entity")
            {
                X = 30,
                Y = 6,
                Width = 20,
                Height = 10
            };
            Button addFilm = new Button(4, 1, "Film");
            addFilm.Clicked += ClickCreateFilm;
            Button addActor = new Button(4, 2, "Actor");
            addActor.Clicked += ClickCreateActor;
            Button addReview = new Button(4, 3, "Review");
            addReview.Clicked += ClickCreateReview;
            Button cancel = new Button(4, 7, "Cancel");
            cancel.Clicked += OnQuit;

            win.Add(addFilm, addActor, addReview, cancel);
            Application.Run(win);
        }
        public static void ProcessClickViewAll()
        {
            Window win = new Window("Select entity")
            {
                X = 30,
                Y = 6,
                Width = 20,
                Height = 10
            };
            Button showFilms = new Button(4, 1, "Films");
            showFilms.Clicked += ClickShowAllFilms;
            Button showActors = new Button(4, 2, "Actors");
            showActors.Clicked += ClickShowAllActors;
            Button showReviews = new Button(4, 3, "Reviews");
            showReviews.Clicked += ClickShowAllReviews;
            Button cancel = new Button(4, 7, "Cancel");
            cancel.Clicked += OnQuit;

            win.Add(showFilms, showActors, showReviews, cancel);
            Application.Run(win);
        }
        public static void ClickCreateFilm()
        {
            CreateFilmDialog dialog = new CreateFilmDialog();
            dialog.SetRepository(repo.actorRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Film film = dialog.GetFilm();
                int insertedId = (int)repo.filmRepository.Insert(film);
                List<int> actorIds = dialog.GetActorIds();
                foreach(int id in actorIds)
                {
                    Role currentRole = new Role(){actorId = id, filmId = insertedId};
                    repo.roleRepository.Insert(currentRole);
                }
            }
        }
        public static void ClickCreateActor()
        {
            CreateActorDialog dialog = new CreateActorDialog();
            dialog.SetRepository(repo.filmRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Actor actor = dialog.GetActor();
                int insertedId = (int)repo.actorRepository.Insert(actor);
                List<int> filmIds = dialog.GetFilmIds();
                foreach(int id in filmIds)
                {
                    Role currentRole = new Role(){actorId = insertedId, filmId = id};
                    repo.roleRepository.Insert(currentRole);
                }
            }
        }
        public static void ClickCreateReview()
        {
            CreateReviewDialog dialog = new CreateReviewDialog();
            dialog.SetService(repo);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Review review = dialog.GetReview();
                repo.reviewRepository.Insert(review);
            }
        }
        public static void ClickShowAllFilms()
        {
            ShowAllFilmsDialog dialog = new ShowAllFilmsDialog();
            dialog.SetRepository(repo);
            Application.Run(dialog);
        }
        public static void ClickShowAllActors()
        {
            ShowAllActorsDialog dialog = new ShowAllActorsDialog();
            dialog.SetRepository(repo);
            Application.Run(dialog);
        }
        public static void ClickShowAllReviews()
        {
            ShowAllReviewsDialog dialog = new ShowAllReviewsDialog();
            dialog.SetRepository(repo);
            Application.Run(dialog);
        }
        public static void ProcessClickShow()
        {
            Window win = new Window("Select entity")
            {
                X = 30,
                Y = 6,
                Width = Dim.Percent(30),
                Height = 12
            };
            Label idLabel = new Label(4, 2, "ID:");
            idToShow = new TextField("")
            {
                X = 8, Y = 2, Width = Dim.Percent(40)
            };
            Button showFilms = new Button(4, 3, "Films");
            showFilms.Clicked += ClickShowFilm;
            Button showActors = new Button(4, 4, "Actors");
            showActors.Clicked += ClickShowActor;
            Button showReviews = new Button(4, 5, "Reviews");
            showReviews.Clicked += ClickShowReview;
            Button cancel = new Button(4, 9, "Cancel");
            cancel.Clicked += OnQuit;

            win.Add(idLabel, idToShow, showFilms, showActors, showReviews, cancel);
            Application.Run(win);
        }
        public static void ClickShowFilm()
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
                dialog.SetService(repo, false);
                Application.Run(dialog);
                if(dialog.deleted)
                {
                    repo.filmRepository.DeleteById(id);
                    repo.roleRepository.DeleteByFilmId(id);
                }
                if(dialog.updated)
                {
                    repo.filmRepository.Update((long) id, dialog.GetFilm());
                    List<int> updatedRoles = dialog.GetUpdatedRoles();
                    foreach(int actorId in updatedRoles)
                    {
                        if(!repo.roleRepository.IsExist(filmToSet.id, actorId))
                        {
                            Role currentRole = new Role(){actorId = actorId, filmId = filmToSet.id};
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
                            repo.roleRepository.Delete(actor.id, filmToSet.id);
                        }
                    }
                }
            }
        }
        public static void ClickShowActor()
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
                if(dialog.deleted)
                {
                    repo.actorRepository.DeleteById(id);
                    repo.roleRepository.DeleteByActorId(id);
                }
                if(dialog.updated)
                {
                    repo.actorRepository.Update((long) id, dialog.GetActor());
                    List<int> updatedRoles = dialog.GetUpdatedRoles();
                    foreach(int filmId in updatedRoles)
                    {
                        if(!repo.roleRepository.IsExist(filmId, actorToSet.id))
                        {
                            Role currentRole = new Role(){actorId = actorToSet.id, filmId = filmId};
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
                            repo.roleRepository.Delete(actorToSet.id, film.id);
                        }
                    }
                }
            }
        }
        public static void ClickShowReview()
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
            else if(repo.reviewRepository.GetById(id) == null)
            {
                errorText = $"Review with id [{id}] doesn`t exist.";
            }
            if(errorText != "")
            {
                MessageBox.ErrorQuery("Error", errorText, "OK");
                return;
            }
            else
            {
                ShowReviewDialog dialog = new ShowReviewDialog();
                dialog.SetService(repo);
                dialog.SetReview(repo.reviewRepository.GetById(id));
                Application.Run(dialog);
                if(dialog.deleted)
                {
                    repo.reviewRepository.DeleteById(id);
                }
                if(dialog.updated)
                {
                    repo.reviewRepository.Update((long) id, dialog.GetReview());
                }
            }
        }
        private static void ProcessClickEdit()
        {
            Window win = new Window("Select entity")
            {
                X = 30,
                Y = 6,
                Width = Dim.Percent(30),
                Height = 12
            };
            Label idLabel = new Label(4, 2, "ID:");
            idToShow = new TextField("")
            {
                X = 8, Y = 2, Width = Dim.Percent(40)
            };
            Button editFilm = new Button(4, 3, "Film");
            editFilm.Clicked += ClickEditFilm;
            Button editActor = new Button(4, 4, "Actor");
            editActor.Clicked += ClickEditActor;
            Button editReview = new Button(4, 5, "Review");
            editReview.Clicked += ClickEditReview;
            Button cancel = new Button(4, 9, "Cancel");
            cancel.Clicked += OnQuit;

            win.Add(idLabel, idToShow, editFilm, editActor, editReview, cancel);
            Application.Run(win);
        }
        private static void ClickEditFilm()
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
                EditFilmDialog dialog = new EditFilmDialog();
                Film filmToSet = repo.filmRepository.GetById(id);
                List<Actor> roles = repo.roleRepository.GetAllActors(id);
                filmToSet.actors = new Actor[roles.Count];
                roles.CopyTo(filmToSet.actors);
                dialog.SetFilm(filmToSet);
                dialog.SetRepository(repo.actorRepository);
                Application.Run(dialog);
                if(!dialog.canceled)
                {
                    repo.filmRepository.Update((long) id, dialog.GetFilm());
                    List<int> updatedRoles = dialog.GetActorIds();
                    foreach(int actorId in updatedRoles)
                    {
                        if(!repo.roleRepository.IsExist(filmToSet.id, actorId))
                        {
                            Role currentRole = new Role(){actorId = actorId, filmId = filmToSet.id};
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
                            repo.roleRepository.Delete(actor.id, filmToSet.id);
                        }
                    }
                }
            }
        }
        private static void ClickEditActor()
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
                EditActorDialog dialog = new EditActorDialog();
                Actor actorToSet = repo.actorRepository.GetById(id);
                List<Film> roles = repo.roleRepository.GetAllFilms(id);
                actorToSet.films = new Film[roles.Count];
                roles.CopyTo(actorToSet.films);
                dialog.SetActor(actorToSet);
                dialog.SetRepository(repo.filmRepository);
                Application.Run(dialog);
                if(!dialog.canceled)
                {
                    repo.actorRepository.Update((long) id, dialog.GetActor());
                    List<int> updatedRoles = dialog.GetFilmIds();
                    foreach(int filmId in updatedRoles)
                    {
                        if(!repo.roleRepository.IsExist(filmId, actorToSet.id))
                        {
                            Role currentRole = new Role(){actorId = actorToSet.id, filmId = filmId};
                            repo.roleRepository.Insert(currentRole);
                        }
                    }
                    foreach(Film film in roles)
                    {
                        bool isExist = false;
                        if(updatedRoles.Count == 0)
                        {
                            isExist = false;
                        }
                        else
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
                            repo.roleRepository.Delete(actorToSet.id, film.id);
                        }
                    }
                }
            }
        }
        private static void ClickEditReview()
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
            else if(repo.reviewRepository.GetById(id) == null)
            {
                errorText = $"Review with id [{id}] doesn`t exist.";
            }
            if(errorText != "")
            {
                MessageBox.ErrorQuery("Error", errorText, "OK");
                return;
            }
            else
            {
                EditReviewDialog dialog = new EditReviewDialog();
                Review toEdit = repo.reviewRepository.GetById(id);
                dialog.SetReview(toEdit);
                dialog.SetService(repo);
                Application.Run(dialog);
                if(!dialog.canceled)
                {
                    repo.reviewRepository.Update((long) id, dialog.GetReview());
                }
            }
        }
    }
}