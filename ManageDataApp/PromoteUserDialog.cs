using Terminal.Gui;
using ClassLib;
using RPCLib;
namespace ManageDataApp
{
    public class PromoteUserDialog : Dialog
    {
        public bool canceled;
        private RemoteService repo;
        private TextField usernameField;
        public PromoteUserDialog()
        {
            this.Title = "Promote user";
            Button ok = new Button("OK");
            ok.Clicked += DialogSubmit;
            Button cancel = new Button("Cancel"); 
            cancel.Clicked += DialogCanceled;
            this.AddButton(ok);
            this.AddButton(cancel);

            Label info = new Label("Here you can promote user to moderator by username:")
            {
                X = Pos.Center(), Y = 5
            };

            Label usernameLabel = new Label("Username")
            {
                X = Pos.Center(), Y = Pos.Bottom(info) + 3
            };
            usernameField = new TextField("")
            {
                X = Pos.Center(), Y = Pos.Bottom(usernameLabel), Width = Dim.Percent(40)
            };
            this.Add(info, usernameLabel, usernameField);
        }
        public void SetService(RemoteService repo)
        {
            this.repo = repo;
        }
        public string GetUsername()
        {
            return this.usernameField.Text.ToString();
        }
        private void DialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void DialogSubmit()
        {
            string errorText = "";
            string usernameInput = this.usernameField.Text.ToString();
            if(usernameInput == "")
            {
                errorText = "Username field mustn`t be empty";
            }
            else if(repo.userRepository.GetByUsername(usernameInput) == null)
            {
                errorText = "User with entered username doesn`t exist";
            }
            else if(repo.userRepository.GetByUsername(usernameInput).isModerator == 1)
            {
                errorText = "User is already a moderator";
            }
            if(errorText != "")
            {
                MessageBox.ErrorQuery("Promoting", errorText, "OK");
            }
            else
            {
                MessageBox.Query("Promoting", "User was promoted successfully!", "OK");
                this.canceled = false;
                Application.RequestStop();
            }
        }
    }
}