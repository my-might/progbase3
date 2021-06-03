using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
namespace ClassLib
{
    public class ActorRepository
    {
        private SqliteConnection connection;
        public ActorRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }
        public long Insert(Actor actor)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO actors (fullname,country,birthDate)
                                    VALUES ($fullname,$country,$birthDate);
                                    SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$fullname", actor.fullname);
            command.Parameters.AddWithValue("$country", actor.country);
            command.Parameters.AddWithValue("$birthDate", actor.birthDate.ToShortDateString());
            long newId = (long)command.ExecuteScalar();
            connection.Close();
            return newId;
        }
        public int DeleteById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM actors WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }
        public long Update(long id, Actor actor)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE actors SET fullname = $fullname, country = $country, birthDate = $birthDate WHERE id = $id";
            command.Parameters.AddWithValue("$fullname", actor.fullname);
            command.Parameters.AddWithValue("$country", actor.country);
            command.Parameters.AddWithValue("$birthDate", actor.birthDate.ToShortDateString());
            command.Parameters.AddWithValue("$id", id);
            long result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }
        public Actor GetById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM actors WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            Actor actor = new Actor();
            if(reader.Read())
            {
                actor = GetActor(reader);
            }
            else
            {
                actor = null;
            }
            reader.Close();
            connection.Close();
            return actor;
        }
        public List<Actor> GetAll()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM actors";
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
        public long GetCount()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM actors";
            long result = (long)command.ExecuteScalar();
            connection.Close();
            return result;
        }
        public int GetTotalPages()
        {
            const int pageSize = 10;
            return (int)Math.Ceiling(GetCount() / (double)pageSize);
        }
        public List<Actor> GetPage(int p)
        {
            connection.Open();
            const int pageSize = 10;
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM actors LIMIT $pageSize OFFSET $offset";
            command.Parameters.AddWithValue("$pageSize",pageSize);
            command.Parameters.AddWithValue("offset",pageSize*(p-1));
            List<Actor> actors = new List<Actor>();
            SqliteDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                Actor actor = GetActor(reader);
                actors.Add(actor);
            }
            reader.Close();
            connection.Close();
            return actors;
        }
        public int GetSearchPagesCount(string searchFullname)
        {
            const int pageSize = 10;
            connection.Open();
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM actors WHERE fullname LIKE '%' || $value || '%'";
            command.Parameters.AddWithValue("$value", searchFullname);
            long result = (long)command.ExecuteScalar();
            connection.Close();
            return (int)Math.Ceiling((int)result / (double)pageSize);
        }
        public List<Actor> GetSearchPage(string searchFullname, int page)
        {
            const int pageSize = 10;
            connection.Open();
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM actors WHERE fullname LIKE '%' || $value || '%' LIMIT $pageSize OFFSET $offset";
            command.Parameters.AddWithValue("$pageSize",pageSize);
            command.Parameters.AddWithValue("offset",pageSize*(page-1));
            command.Parameters.AddWithValue("$value", searchFullname);
            List<Actor> actors = new List<Actor>();
            SqliteDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                Actor actor = GetActor(reader);
                actors.Add(actor);
            }
            reader.Close();
            connection.Close();
            return actors;
        }
        public int[] GetAllIds()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT id FROM actors";
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