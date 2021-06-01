using Terminal.Gui;
namespace ManageData
{
    public class RegistrationDialog : Dialog
    {
        public bool canceled;
        private TextField username;
        private TextField fullname;
        private TextField password;
        public RegistrationDialog()
        {
            this.Title = "Registration";
            Button ok = new Button("OK");
            ok.Clicked += DialogSubmit;
            Button cancel = new Button("Cancel"); 
            cancel.Clicked += DialogCanceled;
            this.AddButton(ok);
            this.AddButton(cancel);

            Label usernameLabel = new Label("Username:")
            {
                X = Pos.Center(), Y = 4
            };
            username = new TextField("")
            {
                X = Pos.Center(), Y = Pos.Bottom(usernameLabel) + 1, Width = Dim.Percent(50)
            };
            this.Add(usernameLabel, username);

            Label fullnameLabel = new Label("Fullname:")
            {
                X = Pos.Center(), Y = 7
            };
            fullname = new TextField("")
            {
                X = Pos.Center(), Y = Pos.Bottom(fullnameLabel) + 1, Width = Dim.Percent(50)
            };
            this.Add(fullnameLabel, fullname);

            Label passwordLabel = new Label("Password:")
            {
                X = Pos.Center(), Y = 10
            };
            password = new TextField()
            {
                X = Pos.Center(), Y = Pos.Bottom(passwordLabel) + 1, Width = Dim.Percent(50)
            };
            this.Add(passwordLabel, password);
        }
        private void DialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void DialogSubmit()
        {
            Application.RequestStop();
        }
    }
}