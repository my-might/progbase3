using Terminal.Gui;
using System.Collections.Generic;
using ClassLib;
namespace ManageData
{
    public static class UserInterface
    {
        private static Service repo;
        
        private static TextField idToShow;
        private static Toplevel top;
        
        public static void SetService(Service repo1)
        {
            repo = repo1;
        }
        public static void ProcessApplication()
        {
            Application.Init();
            top = Application.Top;
            ProcessRegistration();
        }
        private static void OnQuit()
        {
            Application.RequestStop();
        }
        public static void ProcessRegistration()
        {
            RegistrationDialog dialog = new RegistrationDialog();
            dialog.SetRepository(repo.userRepository);
            top.Add(dialog);
            Application.Run();
            if(!dialog.canceled)
            {
                User registered = dialog.GetUser();
                MenuBar menu = new MenuBar(new MenuBarItem[] {
                    new MenuBarItem ("_File", new MenuItem [] {
                        new MenuItem ("_Export Films", "", Export),
                        new MenuItem ("_Import Films", "", Import),
                        new MenuItem ("_Generate report", "", Report),
                        new MenuItem ("_Exit", "", OnQuit)
                    }),
                    new MenuBarItem ("_Help", new MenuItem [] {
                        new MenuItem ("_About!", "", Help)
                    })
                });
                MainWindowUser userWin = new MainWindowUser();
                userWin.SetService(repo);
                userWin.SetUser(registered);
                top.Add(userWin, menu);
                Application.Run();
            }
        }
        public static void Help()
        {
            HelpDialog dialog = new HelpDialog();
            Application.Run(dialog);
        }
        public static void Report()
        {
            GenerateReportDialog dialog = new GenerateReportDialog();
            dialog.SetRepository(repo.actorRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Actor actor = repo.actorRepository.GetById(int.Parse(dialog.actorIdField.Text.ToString()));
                GenerateObjects.SetInfo(actor, dialog.directory.Text.ToString(), repo);
                GenerateObjects.GenerateReport();
                MessageBox.Query("Report", "Report was generated successfully!", "OK");
            }
        }
        public static void Export()
        {
            Export dialog = new Export();
            dialog.SetRepository(repo.actorRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Xml.SetRepository(repo.filmRepository);
                Xml.ExportData(repo.roleRepository.GetAllFilms(int.Parse(dialog.actorIdField.Text.ToString())), dialog.directory.Text.ToString());
                MessageBox.Query("Export", "Data was exported successfully!", "OK");
            }
        }
        public static void Import()
        {
            Import dialog = new Import();
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                try
                {
                    Xml.SetRepository(repo.filmRepository);
                    Xml.ImportData(dialog.filePath.Text.ToString());
                    MessageBox.Query("Export", "Data was imported successfully!", "OK");
                }
                catch
                {
                    MessageBox.ErrorQuery("Error", "Cannot import data from chosen file", "OK");
                }
            }
        }
    }
}