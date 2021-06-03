using Terminal.Gui;
namespace ManageData
{
    public class Import : Dialog
    {
        public bool canceled;
        public TextField filePath;
        public Import()
        {
            this.Title = "Import";
            Button ok = new Button("OK");
            ok.Clicked += DialogSubmit;
            this.AddButton(ok);
            Button cancel = new Button("Cancel"); 
            cancel.Clicked += DialogCanceled;
            this.AddButton(cancel);
            
            Label filePathLabel = new Label("Select directory:")
            {
                X = Pos.Center(), Y = 5
            };
            Button openDirectory = new Button("Open directory")
            {
                X = Pos.Center(), Y = Pos.Bottom(filePathLabel)
            };
            openDirectory.Clicked += SelectDirectory;
            filePath = new TextField("not selected")
            {
                X = Pos.Center(), Y = Pos.Bottom(openDirectory) + 1, Width = Dim.Fill() - 4
            };
            this.Add(filePathLabel, openDirectory, filePath);
        }
        private void SelectDirectory()
        {
            OpenDialog dialog = new OpenDialog("Open XML file", "Open?");
            Application.Run(dialog);
        
            if (!dialog.Canceled)
            {
                NStack.ustring filePath1 = dialog.FilePath;
                filePath.Text = filePath1;
            }
            else
            {
                filePath.Text = "not selected.";
            }
        }
        
        private void DialogSubmit()
        {
            string errorText = "";
            if(filePath.Text.ToString() == "not selected")
            {
                errorText = "You have to choose file to import from";
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