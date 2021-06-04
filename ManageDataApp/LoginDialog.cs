using Terminal.Gui;
using ClassLib;
using System;
using RPCLib;
using DataProcessLib;
namespace ManageDataApp
{
    public class LoginDialog : Dialog
    {
        public bool canceled;
        private User loggedUser;
        private TextField username;
        private TextField password;
        private RemoteUserRepository repo;
        public LoginDialog()
        {
            this.Title = "Log in";
            this.Height = 23;
            this.Width = 50;
            Button ok = new Button("OK");
            ok.Clicked += DialogSubmit;
            Button cancel = new Button("Cancel"); 
            cancel.Clicked += DialogCanceled;
            this.AddButton(ok);
            this.AddButton(cancel);

            Label info = new Label("Log in")
            {
                X = Pos.Center(), Y = 1
            };
            this.Add(info);

            Label usernameLabel = new Label("Username:")
            {
                X = Pos.Center(), Y = 5
            };
            username = new TextField("")
            {
                X = Pos.Center(), Y = Pos.Bottom(usernameLabel), Width = Dim.Percent(50)
            };
            this.Add(usernameLabel, username);

            Label passwordLabel = new Label("Password:")
            {
                X = Pos.Center(), Y = 9
            };
            password = new TextField()
            {
                X = Pos.Center(), Y = Pos.Bottom(passwordLabel), Width = Dim.Percent(50),
                Secret = true
            };
            this.Add(passwordLabel, password);
        }
        private void DialogSubmit()
        {
            string errorText = "";
            if(username.Text.ToString() == "")
            {
                errorText = "Username field mustn`t be empty";
            }
            else if(password.Text.ToString() == "")
            {
                errorText = "Password field mustn`t be empty";
            }
            if(errorText != "")
            {
                MessageBox.ErrorQuery("Error", errorText, "OK");
            }
            else
            {
                loggedUser = new User();
                try
                {
                    Authentication.SetRepository(repo);
                    loggedUser = Authentication.Login(username.Text.ToString(), password.Text.ToString());
                    MessageBox.Query("Log in", "You have logged in successfully!", "OK");
                    this.canceled = false;
                    Application.RequestStop();
                }
                catch(Exception ex)
                {
                    MessageBox.ErrorQuery("Error", ex.Message,  "OK");
                }
            }
        }
        public User GetUser()
        {
            return this.loggedUser;
        }
        private void DialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        public void SetRepository(RemoteUserRepository repo)
        {
            this.repo = repo;
        }
    }
}