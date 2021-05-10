using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Data.Sqlite;
namespace ConsoleProject
{
    public class Xml
    {
        private FilmRepository repo;
        public Xml(FilmRepository repo)
        {
            this.repo = repo;
        }
        public void ExportData(List<Film> films, string filePath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Film>));
            StreamWriter sw = new System.IO.StreamWriter(filePath);
            ser.Serialize(sw, films);
            sw.Close();
        }

        public void ImportData(string filePath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Film>));
            string xmlData = File.ReadAllText(filePath);
            StringReader reader = new StringReader(xmlData);
            List<Film> films = (List<Film>)ser.Deserialize(reader);
            reader.Close();
            foreach(Film film in films)
            {
                if(repo.GetById(film.id) == null)
                {
                    repo.InsertImported(film);
                }
            }
        }
    }
}