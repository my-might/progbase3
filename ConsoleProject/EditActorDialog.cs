namespace ConsoleProject
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
        }
    }
}