using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
namespace ConsoleProject
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
        public int DeleteById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM roles WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int result = command.ExecuteNonQuery();
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
    }
}