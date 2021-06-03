using System;
namespace ClassLib
{
    public class Role
    {
        public int id;
        public int actorId;
        public int filmId;

        public override string ToString()
        {
            return string.Format($"[{this.id}] actor:{this.actorId}, film:{this.filmId}");
        }
        public string RoleConnection()
        {
            string separator = "#$&";
            string connection = id+separator+actorId+separator+filmId;
            return connection;
        }
        public Role RoleParser(string connection)
        {
            string separator = "#$&";
            string[] parameters = connection.Split(separator);
            Role role = new Role();
            role.id = int.Parse(parameters[0]);
            role.actorId = int.Parse(parameters[1]);
            role.filmId = int.Parse(parameters[2]);
            return role;
        }
    }
}