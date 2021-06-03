using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
namespace ClassLib
{
    public class RoleRepository
    {
        private SqliteConnection connection;
        public RoleRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }
        public long Insert(Role role)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO roles (actorId,filmId)
                                    VALUES ($actorId,$filmId);
                                    SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$actorId", role.actorId);
            command.Parameters.AddWithValue("$filmId", role.filmId);
            long newId = (long)command.ExecuteScalar();
            connection.Close();
            return newId;
        }
        public long Delete(int actorId, int filmId)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM roles WHERE actorId = $actorId AND filmId = $filmId";
            command.Parameters.AddWithValue("$actorId", actorId);
            command.Parameters.AddWithValue("$filmId", filmId);
            long result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }
        public long DeleteByActorId(int actorId)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM roles WHERE actorId = $actorId";
            command.Parameters.AddWithValue("$actorId", actorId);
            long result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }
        public long DeleteByFilmId(int filmId)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM roles WHERE filmId = $filmId";
            command.Parameters.AddWithValue("$filmId", filmId);
            long result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }
        public Role GetById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM roles WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            Role role = new Role();
            if(reader.Read())
            {
                role = GetRole(reader);
            }
            else
            {
                role = null;
            }
            reader.Close();
            connection.Close();
            return role;
        }
        public int CountByActorId(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM roles WHERE actorId = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            int result = 0;
            while(reader.Read())
            {
                result++;
            }
            reader.Close();
            connection.Close();
            return result;
        }
        public int CountByFilmId(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM roles WHERE filmId = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            int result = 0;
            while(reader.Read())
            {
                result++;
            }
            reader.Close();
            connection.Close();
            return result;
        }
        public bool IsExist(long filmId, long actorId)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM roles WHERE filmId = $filmId AND actorId = $actorId";
            command.Parameters.AddWithValue("$filmId", filmId);
            command.Parameters.AddWithValue("$actorId", actorId);
            SqliteDataReader reader = command.ExecuteReader();
            bool result;
            if(reader.Read())
            {
                result = true;
            }
            else
            {
                result = false;
            }
            reader.Close();
            connection.Close();
            return result;
        }
        public List<Film> GetAllFilms(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT films.id, title, genre, description, releaseYear 
                                    FROM films, roles WHERE roles.actorId = $id AND roles.filmId = films.id";
            command.Parameters.AddWithValue("$id", id);
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
        public List<Actor> GetAllActors(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT actors.id, fullname, country, birthDate 
                                    FROM actors, roles WHERE roles.filmId = $id AND roles.actorId = actors.id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            List<Actor> actors = new List<Actor>();
            while(reader.Read())
            {
                Actor currentActor = GetActor(reader);
                actors.Add(currentActor);
            }
            reader.Close();
            connection.Close();
            return actors;
        }
        public List<Film> GetForImage(int id)
        {
            List<Film> films = GetAllFilms(id);
            foreach(Film film in films)
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM reviews WHERE filmId = $id ORDER BY postedAt";
                command.Parameters.AddWithValue("$id", film.id);
                SqliteDataReader reader = command.ExecuteReader();
                film.reviews = new List<Review>();
                while(reader.Read())
                {
                    Review currentReview = GetReview(reader);
                    film.reviews.Add(currentReview);
                }
                reader.Close();
                connection.Close();
            }
            return films;
        }
        public double[][] ValuesForImage(List<Film> films)
        {
            double[][] result = new double[films.Count][];
            for(int i = 0; i<films.Count; i++)
            {
                result[i] = new double[4];
                if(films[i].reviews.Count != 0)
                {
                    result[i][0] = films[i].reviews[0].rating;
                    result[i][3] = films[i].reviews[films[i].reviews.Count-1].rating;
                    double min = films[i].reviews[0].rating;
                    double max = films[i].reviews[0].rating;
                    foreach(Review review in films[i].reviews)
                    {
                        if(review.rating > max)
                        {
                            max = review.rating;
                        }
                        else if(review.rating < min)
                        {
                            min = review.rating;
                        }
                    }
                    result[i][1] = max;
                    result[i][2] = min;
                }
                else
                {
                    result[i][0] = 0;
                    result[i][1] = 0;
                    result[i][2] = 0;
                    result[i][3] = 0;
                }
            }
            return result;
        }
        public string[] TitlesForImage(List<Film> films)
        {
            string[] result = new string[films.Count];
            for(int i = 0; i<films.Count; i++)
            {
                result[i] = films[i].id.ToString();
            }
            return result;
        }
        private static Role GetRole(SqliteDataReader reader)
        {
            Role role = new Role();
            role.id = int.Parse(reader.GetString(0));
            role.actorId = int.Parse(reader.GetString(1));
            role.filmId = int.Parse(reader.GetString(2));
            return role;
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
        private static Actor GetActor(SqliteDataReader reader)
        {
            Actor actor = new Actor();
            actor.id = int.Parse(reader.GetString(0));
            actor.fullname = reader.GetString(1);
            actor.country = reader.GetString(2);
            actor.birthDate = DateTime.Parse(reader.GetString(3));
            return actor;
        }
        private static Review GetReview(SqliteDataReader reader)
        {
            Review review = new Review();
            review.id = int.Parse(reader.GetString(0));
            review.opinion = reader.GetString(1);
            review.rating = int.Parse(reader.GetString(2));
            review.postedAt = DateTime.Parse(reader.GetString(3));
            return review;
        }
    }
}