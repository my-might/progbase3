using Terminal.Gui;
namespace ManageData
{
    public class Import : Dialog
    {
        public bool canceled;
        public Label directory;
        public Import()
        {
            this.Title = "Import";
            Button ok = new Button("OK");
            ok.Clicked += DialogSubmit;
            this.AddButton(ok);
            Button cancel = new Button("Cancel"); 
            cancel.Clicked += DialogCanceled;
            this.AddButton(cancel);
            Label filePathLabel = new Label("Select file:")
            {
                X = 4, Y = Pos.Center()
            };
            Button openDirectory = new Button("Open file")
            {
                X = 20, Y = Pos.Top(filePathLabel)
            };
            openDirectory.Clicked += SelectDirectory;
            directory = new Label("not selected")
            {
                X = 20, Y = Pos.Top(filePathLabel) + 1, Width = Dim.Fill()
            };
            this.Add(filePathLabel, openDirectory, directory);
        }
        private void SelectDirectory()
        {
            OpenDialog dialog = new OpenDialog("Open file", "Open?");
            dialog.CanChooseDirectories = false;
            dialog.CanChooseFiles = true;
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
        
        private void DialogSubmit()
        {
            string errorText = "";
            if(directory.Text.ToString() == "")
            {
                errorText = "You have to choose file to import from.";
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