using Terminal.Gui;
using System.Collections.Generic;
namespace ConsoleProject
{
    public class ShowAllActorsDialog : Dialog
    {
        private ActorRepository repo;
        private int page = 1;
        private Label totalPagesLabel;
        private Label currentPageLabel;
        private TextField searchPage;
        private ListView allActors;
        public ShowAllActorsDialog()
        {
            Button ok = new Button("OK");
            ok.Clicked += DialogCanceled;
            this.AddButton(ok);
            allActors = new ListView(new List<Actor>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            allActors.OpenSelectedItem += OnOpenActor;
            Label title = new Label("Actors list")
            {
                X = Pos.Center(), Y = 0
            };
            Button prevPage = new Button("Prev")
            {
                X = Pos.Center() - 28, Y = 2
            };
            prevPage.Clicked += ClickPrevPage;
            Button nextPage = new Button("Next")
            {
                X = Pos.Center() + 20, Y = 2
            };
            nextPage.Clicked += ClickNextPage;
            currentPageLabel = new Label("?")
            {
                X = Pos.Right(prevPage) + 4, Y = 2,
                Width = 5
            };
            totalPagesLabel = new Label("?")
            {
                X = Pos.Left(nextPage) - 6, Y = 2,
                Width = 5
            };
            FrameView frameView = new FrameView("")
            {
                X = 3, Y = 3,
                Width = Dim.Fill() - 4,
                Height = 12
            };
            frameView.Add(allActors);
            searchPage = new TextField("")
            {
                X = Pos.Center(), Y = 2,
                Width = Dim.Percent(10)
            };
            searchPage.KeyPress += OnSearchEnter;
            this.Add(title, prevPage, nextPage, currentPageLabel, totalPagesLabel, frameView, searchPage);
        }
        private void OnOpenActor(ListViewItemEventArgs args)
        {
            Actor actor = (Actor)args.Value;
            ShowActorDialog dialog = new ShowActorDialog();
            dialog.SetActor(actor);
            Application.Run(dialog);
            if(dialog.deleted)
            {
                repo.DeleteById(actor.id);
            }
            if(page > repo.GetTotalPages() && page > 1)
            {
                page--;
            }
            if(dialog.updated)
            {
                repo.Update((long)actor.id, dialog.GetActor());
            }
            ShowCurrentPage();
        }
        private void OnSearchEnter(KeyEventEventArgs args)
        {
            string errorText = "";
            int toPage = 0;
            if(args.KeyEvent.Key != Key.Enter)
            {
                return;
            }
            if(searchPage.Text.ToString() == "")
            {
                errorText = "Field is empty.";
            }
            else if(!int.TryParse(searchPage.Text.ToString(), out toPage))
            {
                errorText = "Page must be integer.";
            }
            else if(toPage < 1 || toPage > repo.GetTotalPages())
            {
                errorText = "Page is out of range.";
            }
            if(errorText != "")
            {
                MessageBox.ErrorQuery("Error", errorText, "OK");
            }
            else 
            {
                this.page = toPage;
                ShowCurrentPage();
            }
        }
        public void SetRepository(ActorRepository repo)
        {
            this.repo = repo;
            ShowCurrentPage();
        }
        private void ClickPrevPage()
        {
            if(page == 1)
            {
                return;
            }
            this.page--;
            ShowCurrentPage();
        }
        private void ClickNextPage()
        {
            int totalPages = repo.GetTotalPages();
            if(page == totalPages)
            {
                return;
            }
            this.page++;
            ShowCurrentPage();
        }
        private void ShowCurrentPage()
        {
            this.currentPageLabel.Text = this.page.ToString();
            this.totalPagesLabel.Text = repo.GetTotalPages().ToString();
            this.allActors.SetSource(repo.GetPage(page));
        }
        private void DialogCanceled()
        {
            Application.RequestStop();
        }
    }
}