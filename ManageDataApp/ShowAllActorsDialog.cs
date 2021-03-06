using Terminal.Gui;
using System.Collections.Generic;
using ClassLib;
using RPCLib;
namespace ManageDataApp
{
    public class ShowAllActorsDialog : Dialog
    {
        private RemoteService repo;
        private int page = 1;
        private Label totalPagesLabel;
        private Label currentPageLabel;
        private TextField searchFullname;
        private string searchValue = "";
        private ListView allActors;
        private User user;
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

            Label fullnameLabel = new Label("Search by fullname:")
            {
                X = 5, Y = Pos.Bottom(frameView)
            };
            searchFullname = new TextField("")
            {
                X = Pos.Right(fullnameLabel) + 1, Y = Pos.Y(fullnameLabel), Width = Dim.Percent(50)
            };
            searchFullname.KeyPress += OnSearchFullnameEnter;
            this.Add(title, prevPage, nextPage, currentPageLabel, totalPagesLabel, 
                    frameView, fullnameLabel, searchFullname);
        }
        private void OnSearchFullnameEnter(KeyEventEventArgs args)
        {
            if(args.KeyEvent.Key == Key.Enter)
            {
                page = 1;
                this.searchValue = this.searchFullname.Text.ToString();
                ShowCurrentPage();
            }
        }
        private void OnOpenActor(ListViewItemEventArgs args)
        {
            Actor actor = new Actor();
            try
            {
                actor = (Actor)args.Value;
            }
            catch
            {
                return;
            }
            List<Film> roles = repo.roleRepository.GetAllFilms(actor.id);
            if(roles.Count != 0)
            {
                actor.films = new Film[roles.Count];
                roles.CopyTo(actor.films);
            }
            else
            {
                actor.films = null;
            }
            ShowActorDialog dialog = new ShowActorDialog();
            dialog.SetActor(actor);
            dialog.SetRepository(repo.filmRepository);
            dialog.SetUser(user);
            Application.Run(dialog);
            if(dialog.deleted)
            {
                repo.actorRepository.DeleteById(actor.id);
                repo.roleRepository.DeleteByActorId(actor.id);
            }
            if(page > repo.actorRepository.GetSearchPagesCount(searchValue) && page > 1)
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
        public void SetRepository(RemoteService repo)
        {
            this.repo = repo;
            ShowCurrentPage();
        }
        public void SetUser(User user)
        {
            this.user = user;
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
            int totalPages = repo.actorRepository.GetSearchPagesCount(searchValue);
            if(page == totalPages)
            {
                return;
            }
            this.page++;
            ShowCurrentPage();
        }
        private void ShowCurrentPage()
        {
            int totalPages = repo.actorRepository.GetSearchPagesCount(searchValue);
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
                emptyText.Add("There are no actors in the database.");
                this.allActors.SetSource(emptyText);
            }
            else
            {
                this.currentPageLabel.Text = this.page.ToString();
                this.totalPagesLabel.Text = totalPages.ToString();
                this.allActors.SetSource(repo.actorRepository.GetSearchPage(searchValue, page));
            }
        }
        private void DialogCanceled()
        {
            Application.RequestStop();
        }
    }
}