using Terminal.Gui;
using System;
using System.Collections.Generic;
namespace ConsoleProject
{
    public class ShowActorDialog : Dialog
    {
        private TextField idField;
        private TextField fullnameField;
        private TextField countryField;
        private DateField birthdateField;
        private ListView rolesView;
        private List<int> updatedRoles;
        public bool deleted;
        public bool updated;
        public Actor actorToshow;
        private FilmRepository repo;
        
        public ShowActorDialog()
        {
            this.Title = "Show actor";
            Button ok = new Button("OK");
            ok.Clicked += DialogCanceled;
            this.AddButton(ok);
            Label idLabel = new Label(2, 2, "ID:");
            idField = new TextField("")
            {
                X = 20, Y = Pos.Top(idLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(idLabel, idField);

            Label fullnameLabel = new Label(2, 4, "Fullname:");
            fullnameField = new TextField("")
            {
                X = 20, Y = Pos.Top(fullnameLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(fullnameLabel, fullnameField);

            Label countryLabel = new Label(2, 6, "Country:");
            countryField = new TextField("")
            {
                X = 20, Y = Pos.Top(countryLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(countryLabel, countryField);

            Label birthdateLabel = new Label(2, 8, "Birth date:");
            birthdateField = new DateField()
            {
                X = 20, Y = Pos.Top(birthdateLabel), Width = Dim.Percent(50)
            };
            this.Add(birthdateLabel, birthdateField);

            Label rolesLabel = new Label(2, 10, "Starred in:");
            rolesView = new ListView(new List<Film>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            FrameView frameView = new FrameView("")
            {
                X = 20, Y = Pos.Top(rolesLabel),
                Width = Dim.Percent(50),
                Height = 5
            };
            frameView.Add(rolesView);
            this.Add(rolesLabel, frameView);

            Button delete = new Button(2, 13, "Delete");
            delete.Clicked += OnDeleteActor;
            Button edit = new Button("Edit")
            {
                X = delete.X, Y = 15
            };
            edit.Clicked += OnEditActor;
            this.Add(delete, edit);
        }
        private void OnDeleteActor()
        {
            int index = MessageBox.Query("Delete actor", "Are you sure?", "No", "Yes");
            if(index == 1)
            {
                this.deleted = true;
                Application.RequestStop();
            }
        }
        private void OnEditActor()
        {
            EditActorDialog dialog = new EditActorDialog();
            dialog.SetActor(this.actorToshow);
            dialog.SetRepository(this.repo);
            Application.Run(dialog);

            if(!dialog.canceled)
            {
                Actor updated = dialog.GetActor();
                updated.id = actorToshow.id;
                this.updatedRoles = dialog.GetFilmIds();
                this.updated = true;
                SetActor(updated);
            }
        }
        public Actor GetActor()
        {
            return this.actorToshow;
        }
        public void SetActor(Actor actor)
        {
            this.actorToshow = actor;
            this.idField.Text = actor.id.ToString();
            this.fullnameField.Text = actor.fullname;
            this.countryField.Text = actor.country;
            this.birthdateField.ReadOnly = false;
            this.birthdateField.Date = actor.birthDate;
            this.birthdateField.ReadOnly = true;
            if(updatedRoles != null)
            {
                this.rolesView.SetSource(ListIntToFilm());
            }
            else
            {
                List<Film> films = ArrayToList(actor.films);
                this.rolesView.SetSource(films);
            }
        }
        public void SetRepository(FilmRepository repo)
        {
            this.repo = repo;
        }
        private List<Film> ListIntToFilm()
        {
            List<Film> films = new List<Film>();
            foreach(int id in updatedRoles)
            {
                films.Add(repo.GetById(id));
            }
            return films;
        }
        private List<Film> ArrayToList(Film[] films)
        {
            List<Film> result = new List<Film>();
            foreach(Film film in films)
            {
                result.Add(film);
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