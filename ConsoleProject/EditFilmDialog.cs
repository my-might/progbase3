namespace ConsoleProject
{
    public class EditFilmDialog : CreateFilmDialog
    {
        public EditFilmDialog()
        {
            this.Title = "Edit film";
        }
        public void SetFilm(Film film)
        {
            this.inputTitle.Text = film.title;
            this.inputGenre.Text = film.genre;
            this.inputDescription.Text = film.description;
            this.inputRelease.Text = film.releaseYear.ToString();
        }
    }
}