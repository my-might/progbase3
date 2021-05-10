using Terminal.Gui;
using System;
namespace ConsoleProject
{
    public class CreateActorDialog : Dialog
    {
        public bool canceled;
        protected TextField inputFullname;
        protected TextField inputCountry;
        protected DateField inputBirthDate;
        public CreateActorDialog()
        {
            this.Title = "Create actor";
            Button ok = new Button("OK");
            ok.Clicked += DialogSubmit;
            Button cancel = new Button("Cancel"); 
            cancel.Clicked += DialogCanceled;
            this.AddButton(ok);
            this.AddButton(cancel);

            Label actorFullname = new Label(2, 2, "Fullname:");
            inputFullname = new TextField("")
            {
                X = 20, Y = Pos.Top(actorFullname), Width = Dim.Percent(50)
            };
            this.Add(actorFullname, inputFullname);

            Label actorCountry = new Label(2, 4, "Country:");
            inputCountry = new TextField("")
            {
                X = 20, Y = Pos.Top(actorCountry), Width = Dim.Percent(50)
            };
            this.Add(actorCountry, inputCountry);

            Label actorBirthdate = new Label(2, 6, "Birth date:");
            inputBirthDate = new DateField()
            {
                X = 20, Y = Pos.Top(actorBirthdate), Width = Dim.Percent(50), IsShortFormat = true
            };
            this.Add(actorBirthdate, inputBirthDate);
        }
        public Actor GetActor()
        {
            return new Actor(){fullname = inputFullname.Text.ToString(), 
                            country = inputCountry.Text.ToString(),
                            birthDate = DateTime.Parse(inputBirthDate.Text.ToString())};
        }
        private void DialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void DialogSubmit()
        {
            string errorText = "";
            if(inputFullname.Text.ToString() == "")
            {
                errorText = "Fullname field mustn`t be empty.";
            }
            else if(inputCountry.Text.ToString() == "")
            {
                errorText = "Country field mustn`t be empty.";
            }
            else if(inputBirthDate.Text.ToString() == "")
            {
                errorText = "Birth date field mustn`t be empty.";
            }
            else if(!DateTime.TryParse(inputBirthDate.Text.ToString(), out DateTime birthDate) || birthDate > DateTime.Now)
            {
                errorText = $"Birth date must be less than {DateTime.Now.ToString("o")}.";
            }
            if(errorText != "")
            {
                MessageBox.ErrorQuery("Error", errorText, "OK");
            }
            else
            {
                this.canceled = false;
                Application.RequestStop();
            }
        }
    }
}