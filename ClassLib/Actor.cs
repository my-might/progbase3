using System;
using System.Collections.Generic;
namespace ClassLib
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
        public string ActorConnection()
        {
            string separator = "#$&";
            string connection = id+separator+fullname+separator+country+separator+birthDate;
            return connection;
        }
        public static Actor ActorParser(string connection)
        {
            string separator = "#$&";
            string[] parameters = connection.Split(separator);
            Actor actor = new Actor();
            actor.id = int.Parse(parameters[0]);
            actor.fullname = parameters[1];
            actor.country = parameters[2];
            actor.birthDate = DateTime.Parse(parameters[3]);
            return actor;
        }
    }
}