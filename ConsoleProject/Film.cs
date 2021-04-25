using System;
namespace ConsoleProject
{
    public class Film
    {
        public int id;
        public string title;
        public string genre;
        public int releaseYear;
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
    }
}