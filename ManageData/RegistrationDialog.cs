using Terminal.Gui;
using ClassLib;
using System;
namespace ManageData
{
    public class RegistrationDialog : Dialog
    {
        public bool canceled;
        private TextField username;
        private TextField fullname;
        private TextField password;
        private TextField confirm;
        private User user;
        private UserRepository repo;
        public RegistrationDialog()
        {
            this.Title = "Registration";
            this.Height = 23;
            this.Width = 50;
            Button ok = new Button("OK");
            ok.Clicked += DialogSubmit;
            Button cancel = new Button("Exit"); 
            cancel.Clicked += DialogCanceled;
            this.AddButton(ok);
            this.AddButton(cancel);

            Label info = new Label("Log in or register first")
            {
                X = Pos.Center(), Y = 1
            };
            this.Add(info);

            Label usernameLabel = new Label("Username:")
            {
                X = Pos.Center(), Y = 3
            };
            username = new TextField("")
            {
                X = Pos.Center(), Y = Pos.Bottom(usernameLabel), Width = Dim.Percent(50)
            };
            this.Add(usernameLabel, username);

            Label fullnameLabel = new Label("Fullname:")
            {
                X = Pos.Center(), Y = 6
            };
            fullname = new TextField("")
            {
                X = Pos.Center(), Y = Pos.Bottom(fullnameLabel), Width = Dim.Percent(50)
            };
            this.Add(fullnameLabel, fullname);

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

            Label confirmLabel = new Label("Confirm password:")
            {
                X = Pos.Center(), Y = 12
            };
            confirm = new TextField()
            {
                X = Pos.Center(), Y = Pos.Bottom(confirmLabel), Width = Dim.Percent(50),
                Secret = true
            };
            this.Add(confirmLabel, confirm);

            Label logLabel = new Label("If you`re already registered:")
            {
                X = Pos.Center() - 20, Y = 18
            };
            Button logIn = new Button("Log in")
            {
                X = Pos.Right(logLabel) + 1, Y = Pos.Y(logLabel)
            };
            logIn.Clicked += OnLogIn;
            this.Add(logLabel, logIn);
        }
        private void OnLogIn()
        {
            LoginDialog dialog = new LoginDialog();
            dialog.SetRepository(repo);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                this.user = dialog.GetUser();
                this.canceled = false;
                Application.RequestStop();
            }
        }
        private void DialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void DialogSubmit()
        {
            string errorText = "";
            if(username.Text.ToString() == "")
            {
                errorText = "Username field mustn`t be empty";
            }
            else if(!CheckString(username.Text.ToString()))
            {
                errorText = "Unavailable username format";
            }
            else if(fullname.Text.ToString() == "")
            {
                errorText = "Fullname field mustn`t be empty";
            }
            else if(!CheckFullname())
            {
                errorText = "Unavailable fullname format";
            }
            else if(password.Text.ToString() == "")
            {
                errorText = "Password field mustn`t be empty";
            }
            else if(password.Text.ToString().Length < 8)
            {
                errorText = "Password must contain at least 8 symbols";
            }
            else if(!CheckString(password.Text.ToString()))
            {
                errorText = "Unavailable password format";
            }
            else if(password.Text.ToString() != confirm.Text.ToString())
            {
                errorText = "Enter password more attentively";
            }
            if(errorText != "")
            {
                MessageBox.ErrorQuery("Error", errorText, "OK");
            }
            else
            {
                user = new User();
                user.username = username.Text.ToString();
                user.fullname = fullname.Text.ToString();
                user.password = password.Text.ToString();
                try
                {
                    Authentication.SetRepository(repo);
                    Authentication.Registration(user);
                    MessageBox.Query("Registration", "You have registered successfully!", "OK");
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
            return this.user;
        }
        public void SetRepository(UserRepository repo)
        {
            this.repo = repo;
        }
        private bool CheckString(string toCheck)
        {
            foreach(char c in toCheck)
            {
                if(!char.IsLetterOrDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
        private bool CheckFullname()
        {
            string toCheck = fullname.Text.ToString();
            int letters = 0;
            foreach(char c in toCheck)
            {
                if(char.IsLetter(c))
                {
                    letters++;
                }
                if(!char.IsLetter(c) && !char.IsWhiteSpace(c))
                {
                    return false;
                }
            }
            if(letters == 0)
            {
                return false;
            }
            return true;
        }
    }
}