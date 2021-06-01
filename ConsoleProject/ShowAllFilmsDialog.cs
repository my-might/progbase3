using Terminal.Gui;
using System.Collections.Generic;
namespace ConsoleProject
{
    public class ShowAllFilmsDialog : Dialog
    {
        private Service repo;
        private int page = 1;
        private Label totalPagesLabel;
        private Label currentPageLabel;
        private TextField searchPage;
        private ListView allFilms;
        public ShowAllFilmsDialog()
        {
            Button ok = new Button("OK");
            ok.Clicked += DialogCanceled;
            this.AddButton(ok);
            allFilms = new ListView(new List<Film>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            allFilms.OpenSelectedItem += OnOpenFilm;
            Label title = new Label("Films list")
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
            frameView.Add(allFilms);
            searchPage = new TextField("")
            {
                X = Pos.Center(), Y = 2,
                Width = Dim.Percent(10)
            };
            searchPage.KeyPress += OnSearchEnter;
            this.Add(title, prevPage, nextPage, currentPageLabel, totalPagesLabel, frameView, searchPage);
        }
        private void OnOpenFilm(ListViewItemEventArgs args)
        {
            Film film = (Film)args.Value;
            List<Actor> roles = repo.roleRepository.GetAllActors(film.id);
            film.actors = new Actor[roles.Count];
            roles.CopyTo(film.actors);
            ShowFilmDialog dialog = new ShowFilmDialog();
            dialog.SetFilm(film);
            dialog.SetService(repo, true);
            Application.Run(dialog);
            if(dialog.deleted)
            {
                repo.filmRepository.DeleteById(film.id);
                repo.roleRepository.DeleteByFilmId(film.id);
            }
            if(page > repo.filmRepository.GetTotalPages() && page > 1)
            {
                page--;
            }
            if(dialog.updated)
            {
                repo.filmRepository.Update((long)film.id, dialog.GetFilm());
                List<int> updatedRoles = dialog.GetUpdatedRoles();
                    foreach(int actorId in updatedRoles)
                    {
                        if(!repo.roleRepository.IsExist(film.id, actorId))
                        {
                            Role currentRole = new Role(){actorId = actorId, filmId = film.id};
                            repo.roleRepository.Insert(currentRole);
                        }
                    }
                    foreach(Actor actor in roles)
                    {
                        bool isExist = false;
                        if(updatedRoles.Count != 0)
                        {
                            for(int i = 0; i<updatedRoles.Count; i++)
                            {
                                if(actor.id == updatedRoles[i])
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
            else if(toPage < 1 || toPage > repo.filmRepository.GetTotalPages())
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
            int totalPages = repo.filmRepository.GetTotalPages();
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
            this.totalPagesLabel.Text = repo.filmRepository.GetTotalPages().ToString();
            this.allFilms.SetSource(repo.filmRepository.GetPage(page));
        }
        private void DialogCanceled()
        {
            Application.RequestStop();
        }
    }
}