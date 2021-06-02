using Terminal.Gui;
using System.Collections.Generic;
using ClassLib;
namespace ManageData
{
    public class ShowFilmDialog : Dialog
    {
        private TextField idField;
        private TextField titleField;
        private TextField genreField;
        private TextView descriptionField;
        private TextField yearField;
        private ListView rolesView;
        private List<int> updatedRoles;
        public bool deleted;
        public bool updated;
        public Film filmToShow;
        private Service repo;

        private bool user;
        
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
            FrameView frameView = new FrameView("")
            {
                X = 20, Y = Pos.Top(rolesLabel),
                Width = Dim.Percent(50),
                Height = 4
            };
            frameView.Add(rolesView);
            this.Add(rolesLabel, frameView);

            Button delete = new Button(2, 14, "Delete");
            delete.Clicked += OnDeleteFilm;
            Button edit = new Button("Edit")
            {
                X = delete.X, Y = 15
            };
            edit.Clicked += OnEditFilm;
            Button showReviews = new Button(2, 15, "Show reviews");
            showReviews.Clicked += OnShowReviews;
            this.Add(delete, edit, showReviews);
            delete.Visible = (user = false);
            edit.Visible = (user = false);
            showReviews.Visible = (user = true);
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
        private void OnShowReviews()
        {
            ShowFilmReviews dialog = new ShowFilmReviews();
            dialog.SetRepository(repo, true, filmToShow.id);
            Application.Run(dialog);
        }
        private void OnEditFilm()
        {
            EditFilmDialog dialog = new EditFilmDialog();
            dialog.SetFilm(this.filmToShow);
            dialog.SetRepository(this.repo.actorRepository);
            Application.Run(dialog);

            if(!dialog.canceled)
            {
                Film updated = dialog.GetFilm();
                updated.id = filmToShow.id;
                this.updatedRoles = dialog.GetActorIds();
                this.updated = true;
                SetFilm(updated);
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
            if(updatedRoles != null)
            {
                this.rolesView.SetSource(ListIntToActor());
            }
            else
            {
                List<Actor> actors = ArrayToList(film.actors);
                this.rolesView.SetSource(actors);
            }
        }
        private List<Actor> ListIntToActor()
        {
            List<Actor> actors = new List<Actor>();
            foreach(int id in updatedRoles)
            {
                actors.Add(repo.actorRepository.GetById(id));
            }
            return actors;
        }
        public void SetService(Service repo, bool user)
        {
            this.repo = repo;
            this.user = user;
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