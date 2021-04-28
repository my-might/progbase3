using System;
using System.Collections.Generic;
namespace ConsoleProject
{
    public class Actor
    {
        public int id;
        public string fullname;
        public string country;
        public DateTime birthDate;
        public Film[] films;
        public Actor()
        {
            this.id = 0;
            this.fullname = null;
            this.country = null;
            this.birthDate = DateTime.MinValue;
        }
        public Actor(int id, string fullname, string country, DateTime birthDate)
        {
            this.id = id;
            this.fullname = fullname;
            this.country = country;
            this.birthDate = birthDate;
        }
        public override string ToString()
        {
            return string.Format($"[{this.id}] {this.fullname}, {this.country}, {this.birthDate.ToShortDateString()}");
        }
    }
}