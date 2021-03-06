using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
namespace ClassLib
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
        public long DeleteByFilmId(int filmId)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM reviews WHERE filmId = $filmId";
            command.Parameters.AddWithValue("$filmId", filmId);
            long result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }
        public long Update(long id, Review review)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE reviews SET opinion = $opinion, rating = $rating WHERE id = $id";
            command.Parameters.AddWithValue("$opinion", review.opinion);
            command.Parameters.AddWithValue("$rating", review.rating);
            command.Parameters.AddWithValue("$id", id);
            long result = command.ExecuteNonQuery();
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
        public int GetSearchPagesCount(string searchOpinion)
        {
            const int pageSize = 10;
            connection.Open();
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM reviews WHERE opinion LIKE '%' || $value || '%'";
            command.Parameters.AddWithValue("$value", searchOpinion);
            long result = (long)command.ExecuteScalar();
            connection.Close();
            return (int)Math.Ceiling((int)result / (double)pageSize);
        }
        public List<Review> GetSearchPage(string searchOpinion, int page)
        {
            const int pageSize = 10;
            connection.Open();
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE opinion LIKE '%' || $value || '%' LIMIT $pageSize OFFSET $offset";
            command.Parameters.AddWithValue("$pageSize",pageSize);
            command.Parameters.AddWithValue("offset",pageSize*(page-1));
            command.Parameters.AddWithValue("$value", searchOpinion);
            List<Review> reviews = new List<Review>();
            SqliteDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                Review review = GetReview(reader);
                reviews.Add(review);
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
        public double GetAverageFilmRating(int filmId)
        {
            List<Review> reviews = GetAllFilmReviews(filmId);
            if(reviews.Count == 0)
            {
                return 0;
            }
            int sum = 0;
            foreach(Review review in reviews)
            {
                sum += review.rating;
            }
            double result = Math.Round((double)sum/reviews.Count, 2);
            return result;
        }
        private static Review GetReview(SqliteDataReader reader)
        {
            Review review = new Review();
            review.id = int.Parse(reader.GetString(0));
            review.opinion = reader.GetString(1);
            review.rating = int.Parse(reader.GetString(2));
            review.postedAt = DateTime.Parse(reader.GetString(3));
            review.userId = int.Parse(reader.GetString(4));
            review.filmId = int.Parse(reader.GetString(5));
            return review;
        }
        
    }
}