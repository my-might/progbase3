using Terminal.Gui;
using Microsoft.Data.Sqlite;
namespace ConsoleProject
{
    public class UserInterface
    {
        static Service repo;
        static TextField idToShow;
        public static void MainWindow()
        {
            string dataBaseFile = "/home/valeria/Desktop/progbase3/data/data.db";
            SqliteConnection connection = new SqliteConnection($"Data Source = {dataBaseFile}");
            repo = new Service(connection);
            Application.Init();
            Toplevel top = Application.Top;

            MenuBar menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_Export Films", "", Export),
                    new MenuItem ("_Import Films", "", null),
                    new MenuItem ("_Exit", "", OnQuit)
                }),
                new MenuBarItem ("_Help", new MenuItem [] {
                    new MenuItem ("Help!", "", null)
                })
            });
            Window win = new Window("e-database of films and actors")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            Button addEntity = new Button(2, 4, "Create new entity");
            addEntity.Clicked += ProcessClickCreate;
            Button showAllEntity = new Button(2, 6, "View all entities");
            showAllEntity.Clicked += ProcessClickViewAll;
            Button showEntity = new Button(2, 8, "View entity");
            showEntity.Clicked += ProcessClickShow;
            Button editEntity = new Button(2, 10, "Edit entity");
            editEntity.Clicked += ProcessClickEdit;

            win.Add(addEntity, showAllEntity, showEntity, editEntity);
            top.Add(win, menu);
            Application.Run();
        }
        static void OnQuit()
        {
            Application.RequestStop();
        }
        static void Export()
        {
            Export dialog = new Export();
            dialog.SetRepository(repo.actorRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Xml xml = new Xml(repo.filmRepository);
                xml.ExportData(repo.roleRepository.GetAllFilms(int.Parse(dialog.actorIdField.Text.ToString())), dialog.directory.Text.ToString());
            }
        }
        static void ProcessClickCreate()
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
        static void ProcessClickViewAll()
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
        static void ClickCreateFilm()
        {
            CreateFilmDialog dialog = new CreateFilmDialog();
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Film film = dialog.GetFilm();
                repo.filmRepository.Insert(film);
            }
        }
        static void ClickCreateActor()
        {
            CreateActorDialog dialog = new CreateActorDialog();
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Actor actor = dialog.GetActor();
                repo.actorRepository.Insert(actor);
            }
        }
        static void ClickCreateReview()
        {
            CreateReviewDialog dialog = new CreateReviewDialog();
            dialog.SetRepository(repo.filmRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Review review = dialog.GetReview();
                repo.reviewRepository.Insert(review);
            }
        }
        static void ClickShowAllFilms()
        {
            ShowAllFilmsDialog dialog = new ShowAllFilmsDialog();
            dialog.SetRepository(repo.filmRepository);
            Application.Run(dialog);
        }
        static void ClickShowAllActors()
        {
            ShowAllActorsDialog dialog = new ShowAllActorsDialog();
            dialog.SetRepository(repo.actorRepository);
            Application.Run(dialog);
        }
        static void ClickShowAllReviews()
        {
            ShowAllReviewsDialog dialog = new ShowAllReviewsDialog();
            dialog.SetRepository(repo.reviewRepository);
            Application.Run(dialog);
        }
        private static void ProcessClickShow()
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
            showReviews.Clicked += ClickShowAllReviews;
            Button cancel = new Button(4, 9, "Cancel");
            cancel.Clicked += OnQuit;

            win.Add(idLabel, idToShow, showFilms, showActors, showReviews, cancel);
            Application.Run(win);
        }
        static void ClickShowFilm()
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
                dialog.SetFilm(repo.filmRepository.GetById(id));
                Application.Run(dialog);
                if(dialog.deleted)
                {
                    repo.filmRepository.DeleteById(id);
                }
                if(dialog.updated)
                {
                    repo.filmRepository.Update((long) id, dialog.GetFilm());
                }
            }
        }
        static void ClickShowActor()
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
                dialog.SetActor(repo.actorRepository.GetById(id));
                Application.Run(dialog);
                if(dialog.deleted)
                {
                    repo.actorRepository.DeleteById(id);
                }
                if(dialog.updated)
                {
                    repo.actorRepository.Update((long) id, dialog.GetActor());
                }
            }
        }
        static void ClickShowReview()
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
        static void ClickEditFilm()
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
                dialog.SetFilm(repo.filmRepository.GetById(id));
                Application.Run(dialog);
                if(!dialog.canceled)
                {
                    repo.filmRepository.Update((long)id, dialog.GetFilm());
                }
            }
        }
        static void ClickEditActor()
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
                dialog.SetActor(repo.actorRepository.GetById(id));
                Application.Run(dialog);
                if(!dialog.canceled)
                {
                    repo.actorRepository.Update((long) id, dialog.GetActor());
                }
            }
        }
        static void ClickEditReview()
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
                dialog.SetReview(repo.reviewRepository.GetById(id));
                Application.Run(dialog);
                if(!dialog.canceled)
                {
                    repo.reviewRepository.Update((long) id, dialog.GetReview());
                }
            }
        }
    }
}