using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System;
using RPCLib;
using ClassLib;
namespace DataProcessLib
{
    public static class XmlSerialization
    {
        private static RemoteFilmRepository repo;
        public static void SetRepository(RemoteFilmRepository repo1)
        {
            repo = repo1;
        }
        public static void ExportData(List<Film> films, string directoryPath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Film>));
            StreamWriter sw = new System.IO.StreamWriter(directoryPath + $"/export_{DateTime.Now.ToString().Replace("/", ".")}.xml");
            ser.Serialize(sw, films);
            sw.Close();
        }

        public static void ImportData(string filePath)
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