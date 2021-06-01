using Terminal.Gui;
using System.Collections.Generic;
using ClassLib;
namespace ManageData
{
    public class ShowAllActorsDialog : Dialog
    {
        private Service repo;
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
            List<Film> roles = repo.roleRepository.GetAllFilms(actor.id);
            actor.films = new Film[roles.Count];
            roles.CopyTo(actor.films);
            ShowActorDialog dialog = new ShowActorDialog();
            dialog.SetActor(actor);
            dialog.SetRepository(repo.filmRepository);
            Application.Run(dialog);
            if(dialog.deleted)
            {
                repo.actorRepository.DeleteById(actor.id);
                repo.roleRepository.DeleteByActorId(actor.id);
            }
            if(page > repo.actorRepository.GetTotalPages() && page > 1)
            {
                page--;
            }
            if(dialog.updated)
            {
                repo.actorRepository.Update((long)actor.id, dialog.GetActor());
                List<int> updatedRoles = dialog.GetUpdatedRoles();
                    foreach(int filmId in updatedRoles)
                    {
                        if(!repo.roleRepository.IsExist(filmId, actor.id))
                        {
                            Role currentRole = new Role(){actorId = actor.id, filmId = filmId};
                            repo.roleRepository.Insert(currentRole);
                        }
                    }
                    foreach(Film film in roles)
                    {
                        bool isExist = false;
                        if(updatedRoles.Count != 0)
                        {
                            for(int i = 0; i<updatedRoles.Count; i++)
                            {
                                if(film.id == updatedRoles[i])
                                {
                                    isExist = true;
                                    break;
                                }
                            }
                        }
                        if(!isExist)
                        {
                            repo.roleRepository.Delete(actor.id, film.id);
                        }
                    }
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
            else if(toPage < 1 || toPage > repo.actorRepository.GetTotalPages())
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
        public void SetRepository(Service repo)
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
            int totalPages = repo.actorRepository.GetTotalPages();
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
            this.totalPagesLabel.Text = repo.actorRepository.GetTotalPages().ToString();
            this.allActors.SetSource(repo.actorRepository.GetPage(page));
        }
        private void DialogCanceled()
        {
            Application.RequestStop();
        }
    }
}