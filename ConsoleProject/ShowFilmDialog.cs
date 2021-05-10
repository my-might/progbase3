using Terminal.Gui;
namespace ConsoleProject
{
    public class ShowFilmDialog : Dialog
    {
        private TextField idField;
        private TextField titleField;
        private TextField genreField;
        private TextField descriptionField;
        private TextField yearField;
        public bool deleted;
        public bool updated;
        public Film filmToShow;

        
        public ShowFilmDialog()
        {
            this.Title = "Show film";
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

            Label titleLabel = new Label(2, 4, "Title:");
            titleField = new TextField("")
            {
                X = 20, Y = Pos.Top(titleLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(titleLabel, titleField);

            Label genreLabel = new Label(2, 6, "Genre:");
            genreField = new TextField("")
            {
                X = 20, Y = Pos.Top(genreLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(genreLabel, genreField);

            Label descriptionLabel = new Label(2, 8, "Description:");
            descriptionField = new TextField()
            {
                X = 20, Y = Pos.Top(descriptionLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(descriptionLabel, descriptionField);

            Label yearLabel = new Label(2, 10, "Release year:");
            yearField = new TextField("")
            {
                X = 20, Y = Pos.Top(yearLabel), Width = Dim.Percent(50),
                ReadOnly = true
            };
            this.Add(yearLabel, yearField);
            Button delete = new Button(2, 12, "Delete");
            delete.Clicked += OnDeleteFilm;
            Button edit = new Button("Edit")
            {
                X = Pos.Right(delete) + 3, Y = 12
            };
            edit.Clicked += OnEditFilm;
            this.Add(delete, edit);
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
        private void OnEditFilm()
        {
            EditFilmDialog dialog = new EditFilmDialog();
            dialog.SetFilm(this.filmToShow);
            Application.Run(dialog);

            if(!dialog.canceled)
            {
                Film updated = dialog.GetFilm();
                updated.id = filmToShow.id;
                this.SetFilm(updated);
                this.updated = true;
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
        }
        private void DialogCanceled()
        {
            Application.RequestStop();
        }
    }
}