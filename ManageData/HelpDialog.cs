using Terminal.Gui;
namespace ManageData
{
    public class HelpDialog : Dialog
    {
        public HelpDialog()
        {
            this.Title = "About program";
            Rect frame1 = new Rect(20, 2, 41, 7);
            Label programInfo = new Label(2, 2, "Program info:");
            TextView info = new TextView(frame1)
            {
                X = 20, Y = Pos.Top(programInfo), 
                Width = Dim.Fill(), Height = Dim.Fill(),
                Text = "This program is a e-base of films and\nactors. Here you can view actors, films,\nreviews and info about them, " + 
                        "also view\nall entities by pages.",
                ReadOnly = true
            };
            Rect frame2 = new Rect(20, 11, 41, 5);
            Label authorInfo = new Label(2, 11, "Program author:");
            TextView author = new TextView(frame2)
            {
                X = 20, Y = Pos.Top(authorInfo), 
                Width = Dim.Fill(),
                Text = "Krivosheeva Valeria, student of KPI.\nAll questions you can ask by e-mail.",
                ReadOnly = true
            };
            this.Add(programInfo, info, authorInfo, author);
            Button ok = new Button("OK");
            ok.Clicked += DialogCanceled;
            this.AddButton(ok);
        }
        private void DialogCanceled()
        {
            Application.RequestStop();
        }
    }
}