using System;
using System.Collections.Generic;
namespace ClassLib
{
    public class Film
    {
        public int id;
        public string title;
        public string genre;
        public string description;
        public int releaseYear;
        public List<Review> reviews;
        public Actor[] actors;
         public Film()
        {
            this.id = 0;
            this.title = null;
            this.genre = null;
            this.releaseYear = 0;
        }
        public Film(int id, string title, string genre, int releaseYear)
        {
            this.id = id;
            this.title = title;
            this.genre = genre;
            this.releaseYear = releaseYear;
        }
        public override string ToString()
        {
            return string.Format($"[{this.id}] {this.title}, {this.genre}, {this.releaseYear}");
        }
        public string FilmConnection()
        {
            string separator = "#$&";
            string connection = id+separator+title+separator+genre+separator+description+separator+releaseYear;
            return connection;
        }
        public Film FilmParser(string connection)
        {
            string separator = "#$&";
            string[] parameters = connection.Split(separator);
            Film film = new Film();
            film.id = int.Parse(parameters[0]);
            film.title = parameters[1];
            film.genre = parameters[2];
            film.description = parameters[3];
            film.releaseYear = int.Parse(parameters[4]);
            return film;
        }
    }
}