using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
namespace ConsoleProject
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
        public int Update(long id, Actor actor)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE actors SET fullname = $fullname, country = $country, birthDate = $birthDate WHERE id = $id";
            command.Parameters.AddWithValue("$fullname", actor.fullname);
            command.Parameters.AddWithValue("$country", actor.country);
            command.Parameters.AddWithValue("$birthDate", actor.birthDate);
            command.Parameters.AddWithValue("$id", id);
            int result = command.ExecuteNonQuery();
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
        public LinkedList<Actor> GetAll()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM actors";
            SqliteDataReader reader = command.ExecuteReader();
            LinkedList<Actor> actors = new LinkedList<Actor>();
            while(reader.Read())
            {
                Actor currentActor = GetActor(reader);
                actors.AddLast(currentActor);
            }
            reader.Close();
            connection.Close();
            return actors;
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