using System;
using ClassLib;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string dbFilePath = "./../data/data.db";
            if(!File.Exists(dbFilePath))
            {
                Console.WriteLine("Database doesn`t exist.");
                Environment.Exit(1);
            }
            SqliteConnection connection = new SqliteConnection($"Data Source = {dbFilePath}");
            Service service = new Service(connection);
            ServerProcess server = new ServerProcess(service);
            server.RunServer();
        }
    }
}
