using Terminal.Gui;
using ClassLib;
using System.Collections.Generic;
using RPCLib;
namespace ManageDataApp
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
        private RemoteFilmRepository repo;
        private User user;
        private Button delete;
        private Button edit;
        
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

            delete = new Button(2, 13, "Delete");
            delete.Clicked += OnDeleteActor;
            edit = new Button("Edit")
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
                MessageBox.Query("Edit", "Actor was updated!", "OK");
                Actor updatedActor = dialog.GetActor();
                updatedActor.id = actorToshow.id;
                this.updatedRoles = dialog.GetFilmIds();
                updatedActor.films = ListToArray(ListIntToFilm(updatedRoles));
                this.updated = true;
                SetActor(updatedActor);
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
            if(actor.films != null)
            {
                List<Film> films = ArrayToList(actor.films);
                this.rolesView.SetSource(films);
            }
            else
            {
                List<string> emptyText = new List<string>();
                emptyText.Add("There are no films.");
                this.rolesView.SetSource(emptyText);
            }
        }
        public void SetRepository(RemoteFilmRepository repo)
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
        private Film[] ListToArray(List<Film> films)
        {
            Film[] result = null;
            if(films.Count != 0)
            {
                result = new Film[films.Count];
                for(int i = 0; i<films.Count; i++)
                {
                    result[i] = films[i];
                }
            }
            return result;
        }
        private List<Film> ListIntToFilm(List<int> input)
        {
            List<Film> films = new List<Film>();
            foreach(int id in input)
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