using ClassLib;
namespace ManageData
{
    public class EditActorDialog : CreateActorDialog
    {
        public EditActorDialog()
        {
            this.Title = "Edit actor";
        }
        public void SetActor(Actor actor)
        {
            this.inputFullname.Text = actor.fullname;
            this.inputCountry.Text = actor.country;
            this.inputBirthDate.Date = actor.birthDate; 
            string ids = "";
            if(actor.films != null)
            {
                for(int i = 0; i<actor.films.Length; i++)
                {
                    ids += $"{actor.films[i].id}";
                    if(i != actor.films.Length - 1)
                    {
                        ids += ",";
                    }
                }
            }
            this.inputRoles.Text = ids;
        }
    }
}