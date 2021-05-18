using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;
namespace ConsoleProject
{
    public class FilmRepository
    {
        private SqliteConnection connection;
        public FilmRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }
        public long Insert(Film film)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO films (title,genre,description,releaseYear)
                                    VALUES ($title,$genre,$description,$releaseYear);
                                    SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$title", film.title);
            command.Parameters.AddWithValue("$genre", film.genre);
            command.Parameters.AddWithValue("$description", film.description);
            command.Parameters.AddWithValue("$releaseYear", film.releaseYear);
            long newId = (long)command.ExecuteScalar();
            connection.Close();
            return newId;
        }
        public long InsertImported(Film film)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO films (id,title,genre,description,releaseYear,isImported)
                                    VALUES ($id,$title,$genre,$description,$releaseYear,$isImported);
                                    SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$id", film.id);
            command.Parameters.AddWithValue("$title", film.title);
            command.Parameters.AddWithValue("$genre", film.genre);
            command.Parameters.AddWithValue("$description", film.description);
            command.Parameters.AddWithValue("$releaseYear", film.releaseYear);
            command.Parameters.AddWithValue("$isImported", "imported");
            long newId = (long)command.ExecuteScalar();
            connection.Close();
            return newId;
        }
        public int DeleteById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM films WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }
        public long Update(long id, Film film)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE films SET title = $title, genre = $genre, description = $description, releaseYear = $releaseYear WHERE id = $id";
            command.Parameters.AddWithValue("$title", film.title);
            command.Parameters.AddWithValue("$genre", film.genre);
            command.Parameters.AddWithValue("$description", film.description);
            command.Parameters.AddWithValue("$releaseYear", film.releaseYear);
            command.Parameters.AddWithValue("$id", id);
            long result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }
        public Film GetById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM films WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            Film film = new Film();
            if(reader.Read())
            {
                film = GetFilm(reader);
            }
            else
            {
                film = null;
            }
            reader.Close();
            connection.Close();
            return film;
        }
        public List<Film> GetAll()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM films";
            SqliteDataReader reader = command.ExecuteReader();
            List<Film> films = new List<Film>();
            while(reader.Read())
            {
                Film currentFilm = GetFilm(reader);
                films.Add(currentFilm);
            }
            reader.Close();
            connection.Close();
            return films;
        }
        public long GetCount()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM films";
            long result = (long)command.ExecuteScalar();
            connection.Close();
            return result;
        }
        public int GetTotalPages()
        {
            const int pageSize = 10;
            return (int)Math.Ceiling(GetCount() / (double)pageSize);
        }
        public List<Film> GetPage(int p)
        {
            connection.Open();
            const int pageSize = 10;
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM films LIMIT $pageSize OFFSET $offset";
            command.Parameters.AddWithValue("$pageSize",pageSize);
            command.Parameters.AddWithValue("offset",pageSize*(p-1));
            List<Film> films = new List<Film>();
            SqliteDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                Film film = GetFilm(reader);
                films.Add(film);
            }
            reader.Close();
            connection.Close();
            return films;
        }
        public int[] GetAllIds()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT id FROM films";
            SqliteDataReader reader = command.ExecuteReader();
            List<int> ids = new List<int>();
            while(reader.Read())
            {
                int currentId = int.Parse(reader.GetString(0));
                ids.Add(currentId);
            }
            reader.Close();
            connection.Close();
            int[] result = new int[ids.Count];
            ids.CopyTo(result);
            return result;
        }
        public int GetMinReleaseYear()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT releaseYear FROM films";
            SqliteDataReader reader = command.ExecuteReader();
            int min = 2021;
            while(reader.Read())
            {
                int currentYear = int.Parse(reader.GetString(0));
                if(currentYear < min)
                {
                    min = currentYear;
                }
            }
            reader.Close();
            connection.Close();
            return min;
        }
        private static Film GetFilm(SqliteDataReader reader)
        {
            Film film = new Film();
            film.id = int.Parse(reader.GetString(0));
            film.title = reader.GetString(1);
            film.genre = reader.GetString(2);
            film.description = reader.GetString(3);
            film.releaseYear = int.Parse(reader.GetString(4));
            return film;
        }
    }
}