using Microsoft.Data.Sqlite;
using ClassLib;
namespace ManageData
{
    public class Program
    {
        static void Main(string[] args)
        {
            string dataBaseFile = "/home/valeria/Desktop/progbase3/data/data.db";
            SqliteConnection connection = new SqliteConnection($"Data Source = {dataBaseFile}");
            Service repo = new Service(connection);
            UserInterface.SetService(repo);
            UserInterface.ProcessApplication();
        }
    }
}
