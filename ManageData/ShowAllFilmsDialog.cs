using Terminal.Gui;
using System.Collections.Generic;
using ClassLib;
namespace ManageData
{
    public class ShowAllFilmsDialog : Dialog
    {
        private Service repo;
        private int page = 1;
        private Label totalPagesLabel;
        private Label currentPageLabel;
        private TextField searchPage;
        private TextField searchTitle;
        private string searchValue = "";
        private ListView allFilms;
        private User currentUser;
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
            // searchPage = new TextField("")
            // {
            //     X = Pos.Center(), Y = 2,
            //     Width = Dim.Percent(10)
            // };
            // searchPage.KeyPress += OnSearchPageAlt;

            Label titleLabel = new Label("Search by title:")
            {
                X = 5, Y = Pos.Bottom(frameView)
            };
            searchTitle = new TextField("")
            {
                X = Pos.Right(titleLabel) + 1, Y = Pos.Y(titleLabel), Width = Dim.Percent(50)
            };
            searchTitle.KeyPress += OnSearchTitleEnter;

            this.Add(title, prevPage, nextPage, currentPageLabel, totalPagesLabel, 
                    frameView, titleLabel, searchTitle);
        }
        private void OnSearchTitleEnter(KeyEventEventArgs args)
        {
            if(args.KeyEvent.Key == Key.Enter)
            {
                page = 1;
                this.searchValue = this.searchTitle.Text.ToString();
                ShowCurrentPage();
            }
        }
        private void OnOpenFilm(ListViewItemEventArgs args)
        {
            Film film = new Film();
            try
            {
                film = (Film)args.Value;
            }
            catch
            {
                return;
            }
            List<Actor> roles = repo.roleRepository.GetAllActors(film.id);
            if(roles.Count != 0)
            {
                film.actors = new Actor[roles.Count];
                roles.CopyTo(film.actors);
            }
            else
            {
                film.actors = null;
            }
            ShowFilmDialog dialog = new ShowFilmDialog();
            dialog.SetService(repo);
            dialog.SetFilm(film);
            dialog.SetUser(currentUser);
            Application.Run(dialog);
            if(dialog.deleted)
            {
                repo.filmRepository.DeleteById(film.id);
                repo.roleRepository.DeleteByFilmId(film.id);
                repo.reviewRepository.DeleteByFilmId(film.id);
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
        private void OnSearchPageAlt(KeyEventEventArgs args)
        {
            string errorText = "";
            int toPage = 0;
            if(args.KeyEvent.Key != Key.AltMask)
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
        public void SetUser(User user)
        {
            this.currentUser = user;
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
            int totalPages = repo.filmRepository.GetSearchPagesCount(searchTitle.Text.ToString());
            if(page > totalPages && page > 1)
            {
                page = totalPages;
            }
            if(totalPages != 0 && page == 0)
            {
                page = 1;
            }
            if(totalPages == 0)
            {
                this.page = 0;
                this.currentPageLabel.Text = this.page.ToString();
                this.totalPagesLabel.Text = totalPages.ToString();
                List<string> emptyText = new List<string>();
                emptyText.Add("There are no films in the database.");
                this.allFilms.SetSource(emptyText);
            }
            else
            {
                this.currentPageLabel.Text = this.page.ToString();
                this.totalPagesLabel.Text = totalPages.ToString();
                this.allFilms.SetSource(repo.filmRepository.GetSearchPage(searchTitle.Text.ToString(), page));
            }
        }
        private void DialogCanceled()
        {
            Application.RequestStop();
        }
    }
}