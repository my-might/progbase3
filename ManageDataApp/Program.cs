using Microsoft.Data.Sqlite;
using ClassLib;
using System.IO;
using System;
using RPCLib;
namespace ManageDataApp
{
    public class Program
    {
        static void Main(string[] args)
        {
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
