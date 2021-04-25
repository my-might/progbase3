using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
namespace ConsoleProject
{
    public class UserRepository
    {
        private SqliteConnection connection;
        public UserRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }
        public long Insert(User user)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO users (username,fullname,password,isModerator,registrationDate)
                                    VALUES ($username,$fullname,$password,$isModerator,$registrationDate);
                                    SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$username", user.username);
            command.Parameters.AddWithValue("$fullname", user.fullname);
            command.Parameters.AddWithValue("$password", user.password);
            command.Parameters.AddWithValue("$isModerator", user.isModerator);/////
            command.Parameters.AddWithValue("$registrationDate", user.registrationDate.ToString("o"));
            long newId = (long)command.ExecuteScalar();
            connection.Close();
            return newId;
        }
        public int DeleteById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }
        public int Update(long id, User user)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE users SET username = $username, fullname = $fullname, password = $password, 
                                    isModerator = $isModerator, registrationDate = $registrationDate WHERE id = $id";
            command.Parameters.AddWithValue("$username", user.username);
            command.Parameters.AddWithValue("$fullname", user.fullname);
            command.Parameters.AddWithValue("$password", user.password);
            command.Parameters.AddWithValue("$isModerator", user.isModerator);
            command.Parameters.AddWithValue("$registrationDate", user.registrationDate.ToString("o"));
            command.Parameters.AddWithValue("$id", id);
            int result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }
        public User GetById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            User user = new User();
            if(reader.Read())
            {
                user = GetUser(reader);
            }
            else
            {
                user = null;
            }
            reader.Close();
            connection.Close();
            return user;
        }
        public LinkedList<User> GetAll()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users";
            SqliteDataReader reader = command.ExecuteReader();
            LinkedList<User> users = new LinkedList<User>();
            while(reader.Read())
            {
                User currentUser = GetUser(reader);
                users.AddLast(currentUser);
            }
            reader.Close();
            connection.Close();
            return users;
        }
        private static User GetUser(SqliteDataReader reader)
        {
            User user = new User();
            user.id = int.Parse(reader.GetString(0));
            user.username = reader.GetString(1);
            user.fullname = reader.GetString(2);
            user.password = reader.GetString(3);
            user.isModerator = int.Parse(reader.GetString(4));
            user.registrationDate = DateTime.Parse(reader.GetString(5));
            return user;
        }
    }
}