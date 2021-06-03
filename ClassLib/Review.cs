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
    }
}