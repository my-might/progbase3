using Terminal.Gui;
using ClassLib;
using RPCLib;
namespace ManageDataApp
{
    public class Export : Dialog
    {
        public bool canceled;
        public TextField directory;
        public TextField actorIdField;
        private RemoteActorRepository repo;
        public Export()
        {
            this.Title = "Export";
            Button ok = new Button("OK");
            ok.Clicked += DialogSubmit;
            this.AddButton(ok);
            Button cancel = new Button("Cancel"); 
            cancel.Clicked += DialogCanceled;
            this.AddButton(cancel);
            Label actorIdLabel = new Label("Actor id:")
            {
                X = Pos.Center(), Y = 4
            };
            actorIdField = new TextField("")
            {
                X = Pos.Center(), Y = Pos.Bottom(actorIdLabel), Width = Dim.Percent(30)
            };

            Label filePathLabel = new Label("Select directory:")
            {
                X = Pos.Center(), Y = Pos.Bottom(actorIdField) + 1
            };
            Button openDirectory = new Button("Open directory")
            {
                X = Pos.Center(), Y = Pos.Bottom(filePathLabel)
            };
            openDirectory.Clicked += SelectDirectory;
            directory = new TextField("not selected")
            {
                X = Pos.Center(), Y = Pos.Bottom(openDirectory) + 1, Width = Dim.Fill() - 4
            };
            this.Add(actorIdLabel, actorIdField, filePathLabel, openDirectory, directory);
        }
        private void SelectDirectory()
        {
            OpenDialog dialog = new OpenDialog("Open directory", "Open?");
            dialog.CanChooseDirectories = true;
            dialog.CanChooseFiles = false;
            Application.Run(dialog);
        
            if (!dialog.Canceled)
            {
                NStack.ustring filePath = dialog.FilePath;
                directory.Text = filePath;
            }
            else
            {
                directory.Text = "not selected.";
            }
        }
        public void SetRepository(RemoteActorRepository repo)
        {
            this.repo = repo;
        }
        private void DialogSubmit()
        {
            string errorText = "";
            if(actorIdField.Text.ToString() == "")
            {
                errorText = "Actor id field mustn`t be empty.";
            }
            else if(!int.TryParse(actorIdField.Text.ToString(), out int actorId) ||  actorId < 1)
            {
                errorText = "Actor id must be positive integer.";
            }
            else if(repo.GetById(actorId) == null)
            {
                errorText = "Entered actor id does not exist in the database.";
            }
            else if(directory.Text.ToString() == "not selected")
            {
                errorText = "You have to choose directory to save export.";
            }
            if(errorText != "")
            {
                MessageBox.ErrorQuery("Error", errorText, "OK");
                return;
            }
            else
            {
                this.canceled = false;
                Application.RequestStop();
            }
        }
        private void DialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
    }
}