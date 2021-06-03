using Microsoft.Data.Sqlite;
using ClassLib;
using System.IO;
using System;
namespace ManageData
{
    public class Program
    {
        static void Main(string[] args)
        {
            string dataBaseFile = "./../data/data.db";
            if(!File.Exists(dataBaseFile))
            {
                Console.WriteLine("Database doesn`t exist.");
                Environment.Exit(1);
            }
            SqliteConnection connection = new SqliteConnection($"Data Source = {dataBaseFile}");
            Service repo = new Service(connection);
            UserInterface.SetService(repo);
            UserInterface.ProcessApplication();
        }
    }
}
