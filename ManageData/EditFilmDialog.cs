using ClassLib;
namespace ManageData
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
            string ids = "";
            if(film.actors != null)
            {
                for(int i = 0; i<film.actors.Length; i++)
                {
                    ids += $"{film.actors[i].id}";
                    if(i != film.actors.Length - 1)
                    {
                        ids += ",";
                    }
                }
            }
            this.inputRoles.Text = ids;
        }
    }
}