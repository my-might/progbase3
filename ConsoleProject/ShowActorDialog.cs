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
        private int[] updatedRoles;
        public bool deleted;
        public bool updated;
        public Actor actorToshow;
        
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

            Label rolesLabel = new Label(2, 8, "Film ids:");
            rolesView = new ListView()
            {
                X = 20, Y = Pos.Top(rolesLabel), Width = Dim.Percent(50)
            };
            this.Add(rolesLabel, rolesView);

            Button delete = new Button(2, 12, "Delete");
            delete.Clicked += OnDeleteActor;
            Button edit = new Button("Edit")
            {
                X = Pos.Right(delete) + 3, Y = 12
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
            Application.Run(dialog);

            if(!dialog.canceled)
            {
                Actor updated = dialog.GetActor();
                updated.id = actorToshow.id;
                this.actorToshow = updated;
                this.updatedRoles = dialog.GetFilmIds();
                this.updated = true;
            }
            Application.RequestStop();
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
            this.birthdateField.Date = actor.birthDate;
            this.birthdateField.ReadOnly = true;
            this.rolesView.SetSource(ArrayToList(actor.films));
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
        public int[] GetUpdatedRoles()
        {
            return this.updatedRoles;
        }
        private void DialogCanceled()
        {
            Application.RequestStop();
        }
    }
}