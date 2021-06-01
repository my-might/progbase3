using Terminal.Gui;
using ClassLib;
namespace ManageData
{
    public class Export : Dialog
    {
        public bool canceled;
        public Label directory;
        public TextField actorIdField;
        private ActorRepository repo;
        public Export()
        {
            this.Title = "Export";
            Button ok = new Button("OK");
            ok.Clicked += DialogSubmit;
            this.AddButton(ok);
            Button cancel = new Button("Cancel"); 
            cancel.Clicked += DialogCanceled;
            this.AddButton(cancel);
            Label actorIdLabel = new Label(2, 2, "Actor id:");
            actorIdField = new TextField("")
            {
                X = 20, Y = Pos.Top(actorIdLabel), Width = Dim.Percent(50)
            };
            Label filePathLabel = new Label(2, 4, "Select directory:");
            Button openDirectory = new Button("Open directory")
            {
                X = 20, Y = Pos.Top(filePathLabel)
            };
            openDirectory.Clicked += SelectDirectory;
            directory = new Label("not selected")
            {
                X = 20, Y = Pos.Top(filePathLabel) + 1, Width = Dim.Fill()
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
        public void SetRepository(ActorRepository repo)
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
            else if(directory.Text.ToString() == "")
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