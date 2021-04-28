using System;
namespace ConsoleProject
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
    }
}