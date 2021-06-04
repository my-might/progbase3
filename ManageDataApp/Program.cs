using System;
using RPCLib;
using System.IO;
namespace ManageDataApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            string dbFile = "./../data/data.db";
            if(!File.Exists(dbFile))
            {
                Console.WriteLine("Cannot connect to server.");
                Environment.Exit(1);
            }
            RemoteService repo = new RemoteService();
            bool result = repo.TryConnect();
            if(result == false)
            {
                Console.WriteLine("Cannot connect to server.");
                Environment.Exit(1);
            }
            UserInterface.SetService(repo);
            UserInterface.ProcessApplication();
        }
    }
}
