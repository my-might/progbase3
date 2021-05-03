using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
namespace ConsoleProject
{
    public class ReviewRepository
    {
        private SqliteConnection connection;
        public ReviewRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }
        public long Insert(Review review)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO reviews (opinion,rating,postedAt,userId,filmId)
                                    VALUES ($opinion,$rating,$postedAt,$userId,$filmId);
                                    SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$opinion", review.opinion);
            command.Parameters.AddWithValue("$rating", review.rating);
            command.Parameters.AddWithValue("$postedAt", review.postedAt.ToString("o"));
            command.Parameters.AddWithValue("$userId", review.userId);
            command.Parameters.AddWithValue("$filmId", review.filmId);
            long newId = (long)command.ExecuteScalar();
            connection.Close();
            return newId;
        }
        public int DeleteById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM reviews WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }
        public int Update(long id, Review review)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE reviews SET opinion = $opinion, rating = $rating, postedAt = $postedAt WHERE id = $id";
            command.Parameters.AddWithValue("$opinion", review.opinion);
            command.Parameters.AddWithValue("$rating", review.rating);
            command.Parameters.AddWithValue("$postedAt", review.postedAt);
            command.Parameters.AddWithValue("$id", id);
            int result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }
        public Review GetById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            Review review = new Review();
            if(reader.Read())
            {
                review = GetReview(reader);
            }
            else
            {
                review = null;
            }
            reader.Close();
            connection.Close();
            return review;
        }
        public List<Review> GetAll()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews";
            SqliteDataReader reader = command.ExecuteReader();
            List<Review> reviews = new List<Review>();
            while(reader.Read())
            {
                Review currentReview = GetReview(reader);
                reviews.Add(currentReview);
            }
            reader.Close();
            connection.Close();
            return reviews;
        }
        public List<Review> GetAllFilmReviews(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE filmId = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            List<Review> reviews = new List<Review>();
            while(reader.Read())
            {
                Review currentReview = GetReview(reader);
                reviews.Add(currentReview);
            }
            reader.Close();
            connection.Close();
            return reviews;
        }
        public List<Review> GetAllUserReviews(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE userId = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            List<Review> reviews = new List<Review>();
            while(reader.Read())
            {
                Review currentReview = GetReview(reader);
                reviews.Add(currentReview);
            }
            reader.Close();
            connection.Close();
            return reviews;
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