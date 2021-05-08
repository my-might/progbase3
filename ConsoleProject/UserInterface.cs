using Terminal.Gui;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
namespace ConsoleProject
{
    public class UserInterface
    {
        static Service repo;
        public static void Process()
        {
           string dataBaseFile = "/home/valeria/Desktop/progbase3/data/data.db";
            SqliteConnection connection = new SqliteConnection($"Data Source = {dataBaseFile}");
            repo = new Service(connection);
            Application.Init();
            Toplevel top = Application.Top;

            MenuBar menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_Export Films", "", null),
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
            showAllEntity.Clicked += ProcessClickShowAll;
            Button showEntity = new Button(2, 8, "View entity");
            showEntity.Clicked += ProcessClickShow;
            Button removeEntity = new Button(2, 10, "Remove entity");
            removeEntity.Clicked += ProcessClickRemove;
            Button editEntity = new Button(2, 12, "Edit entity");
            editEntity.Clicked += ProcessClickEdit;

            win.Add(addEntity, showAllEntity, showEntity, removeEntity, editEntity);
            top.Add(win, menu);
            Application.Run();
        }
        private static void OnQuit()
        {
            Application.RequestStop();
        }
        private static void ProcessClickCreate()
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
            CreateReviewDialog dialog = new CreateReviewDialog(repo);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Review review = dialog.GetReview();
                repo.reviewRepository.Insert(review);
            }
        }
        private static void ProcessClickShowAll()
        {
            Window win = new Window("Select entity to view")
            {
                X = 30,
                Y = 6,
                Width = 20,
                Height = 10
            };
            Application.Top.Add(win);
            Application.Run();
        }
        private static void ProcessClickShow()
        {

        }
        private static void ProcessClickRemove()
        {

        }
        private static void ProcessClickEdit()
        {

        }
    }
}