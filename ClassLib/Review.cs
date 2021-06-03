using System;
namespace ClassLib
{
    public class Review
    {
        public int id;
        public string opinion;
        public int rating;
        public DateTime postedAt;
        public int userId;
        public int filmId;
         public Review()
        {
            this.id = 0;
            this.opinion = null;
            this.rating = 0;
            this.postedAt = DateTime.MinValue;
        }
        public Review(int id, string opinion, int rating, DateTime postedAt)
        {
            this.id = id;
            this.opinion = opinion;
            this.rating = rating;
            this.postedAt = postedAt;
        }
        public override string ToString()
        {
            return string.Format($"[{this.id}] {this.opinion}, {this.rating}");
        }
        public string ReviewConnection()
        {
            string separator = "#$&";
            string connection = id+separator+opinion+separator+rating+separator+postedAt+separator+userId+separator+filmId;
            return connection;
        }
        public Review ReviewParser(string connection)
        {
            string separator = "#$&";
            string[] parameters = connection.Split(separator);
            Review review = new Review();
            review.id = int.Parse(parameters[0]);
            review.opinion = parameters[1];
            review.rating = int.Parse(parameters[2]);
            review.postedAt = DateTime.Parse(parameters[3]);
            review.userId = int.Parse(parameters[4]);
            review.filmId = int.Parse(parameters[5]);
            return review;
        }
    }
}