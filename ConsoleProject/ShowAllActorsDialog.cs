using Terminal.Gui;
using System.Collections.Generic;
namespace ConsoleProject
{
    public class ShowAllActorsDialog : Dialog
    {
        private Service repo;
        private int page = 1;
        private Label totalPagesLabel;
        private Label currentPageLabel;
        private ListView allActors;
        public ShowAllActorsDialog(Service repo)
        {
            this.repo = repo;
            Button ok = new Button("OK");
            ok.Clicked += DialogCanceled;
            allActors = new ListView(new List<Actor>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            //allActors.OpenSelectedItem += ;
            Label title = new Label("Actors list")
            {
                X = Pos.Center(), Y = 0
            };
            Button prevPage = new Button("Prev")
            {
                X = Pos.Center() - 20, Y = 2
            };
            prevPage.Clicked += ClickPrevPage;
            Button nextPage = new Button("Next")
            {
                X = Pos.Center() + 15, Y = 2
            };
            nextPage.Clicked += ClickNextPage;
            currentPageLabel = new Label("")
            {
                X = Pos.Right(prevPage) + 3, Y = 2
            };
            totalPagesLabel = new Label("")
            {
                X = Pos.Left(nextPage) - 3, Y = 2
            };
            FrameView frameView = new FrameView("")
            {
                X = 3, Y = 3,
                Width = Dim.Fill() - 4,
                Height = 12
            };
            ShowCurrentPage();
            frameView.Add(allActors);
            this.Add(ok, title, prevPage, nextPage, currentPageLabel, totalPagesLabel, frameView);
        }
        private void ClickPrevPage()
        {
            if(page == 1)
            {
                return;
            }
            page--;
            ShowCurrentPage();
        }
        private void ClickNextPage()
        {
            int totalPages = repo.actorRepository.GetTotalPages();
            if(page == totalPages)
            {
                return;
            }
            page++;
            ShowCurrentPage();
        }
        private void ShowCurrentPage()
        {
            this.currentPageLabel.Text = this.page.ToString();
            this.totalPagesLabel.Text = repo.actorRepository.GetTotalPages().ToString();
            this.allActors.SetSource(repo.actorRepository.GetPage(page));
        }
        private void DialogCanceled()
        {
            Application.RequestStop();
        }
    }
}