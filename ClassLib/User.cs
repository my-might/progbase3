using System;
using System.Collections.Generic;
namespace ClassLib
{
    public class User
    {
        public int id;
        public string username;
        public string fullname;
        public string password;
        public int isModerator;
        public DateTime registrationDate;
        public List<Review> reviews;
        public User()
        {
            this.id = 0;
            this.username = null;
            this.fullname = null;
            this.password = null;
            this.isModerator = 0;
            this.registrationDate = DateTime.MinValue;
        }
        public User(int id, string username, string fullname, string password, int isModerator, DateTime registrationDate)
        {
            this.id = 0;
            this.username = username;
            this.fullname = fullname;
            this.password = password;
            this.isModerator = isModerator;
            this.registrationDate = registrationDate;
        }
    }
}