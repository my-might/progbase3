using Terminal.Gui;
using System;
using System.Collections.Generic;
using ClassLib;
namespace ManageData
{
    public class CreateActorDialog : Dialog
    {
        public bool canceled;
        protected FilmRepository repo;
        protected TextField inputFullname;
        protected TextField inputCountry;
        protected DateField inputBirthDate;
        protected TextField inputRoles;
        private List<int> filmIds;
        public CreateActorDialog()
        {
            this.Title = "Create actor";
            Button ok = new Button("OK");
            ok.Clicked += DialogSubmit;
            Button cancel = new Button("Cancel"); 
            cancel.Clicked += DialogCanceled;
            this.AddButton(ok);
            this.AddButton(cancel);

            Label actorFullname = new Label(2, 2, "Fullname:");
            inputFullname = new TextField("")
            {
                X = 20, Y = Pos.Top(actorFullname), Width = Dim.Percent(50)
            };
            this.Add(actorFullname, inputFullname);

            Label actorCountry = new Label(2, 4, "Country:");
            inputCountry = new TextField("")
            {
                X = 20, Y = Pos.Top(actorCountry), Width = Dim.Percent(50)
            };
            this.Add(actorCountry, inputCountry);

            Label actorBirthdate = new Label(2, 6, "Birth date:");
            inputBirthDate = new DateField()
            {
                X = 20, Y = Pos.Top(actorBirthdate), Width = Dim.Percent(50), IsShortFormat = true
            };
            this.Add(actorBirthdate, inputBirthDate);

            Label actorRoles = new Label(2, 8, "Starred in:");
            inputRoles = new TextField("")
            {
                X = 20, Y = Pos.Top(actorRoles), Width = Dim.Percent(50)
            };
            this.Add(actorRoles, inputRoles);
        }
        public Actor GetActor()
        {
            return new Actor(){fullname = inputFullname.Text.ToString(), 
                            country = inputCountry.Text.ToString(),
                            birthDate = DateTime.Parse(inputBirthDate.Text.ToString())
                            };
        }
        public List<int> GetFilmIds()
        {
            return this.filmIds;
        }
        public void SetRepository(FilmRepository repo)
        {
            this.repo = repo;
        }
        private void DialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void DialogSubmit()
        {
            string errorText = "";
            if(inputFullname.Text.ToString() == "")
            {
                errorText = "Fullname field mustn`t be empty.";
            }
            else if(inputCountry.Text.ToString() == "")
            {
                errorText = "Country field mustn`t be empty.";
            }
            else if(inputBirthDate.Text.ToString() == "")
            {
                errorText = "Birth date field mustn`t be empty.";
            }
            else if(!DateTime.TryParse(inputBirthDate.Text.ToString(), out DateTime birthDate) || birthDate > DateTime.Now)
            {
                errorText = $"Birth date must be less than {DateTime.Now.ToString()}.";
            }
            else
            {
                filmIds = new List<int>();
                if(inputRoles.Text.ToString() == "")
                {
                    errorText = "";
                }
                else
                {
                    string[] ids = inputRoles.Text.ToString().Split(",");
                    for(int i = 0; i<ids.Length; i++)
                    {
                        if(!int.TryParse(ids[i], out int id))
                        {
                            errorText = $"Unavailable id: {ids[i]}.";
                            break;
                        }
                        else if(repo.GetById(id) == null)
                        {
                            errorText = $"Film with id '{id}' does not exist in the database.";
                            break;
                        }
                        else if(filmIds.Contains(id))
                        {
                            errorText = "Ids mustn`t contain equal values.";
                            break;
                        }
                        filmIds.Add(id);
                    }
                }
            }
            if(errorText != "")
            {
                MessageBox.ErrorQuery("Error", errorText, "OK");
            }
            else
            {
                this.canceled = false;
                MessageBox.Query("Write review", "Actor was created!", "OK");
                Application.RequestStop();
            }
        }
    }
}