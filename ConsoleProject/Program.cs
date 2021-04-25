using System;
using Microsoft.Data.Sqlite;
using System.IO;
namespace ConsoleProject
{
    class Program
    {
        static void Main(string[] args)
        {
            string dataBaseFile = "/home/valeria/Desktop/progbase3/data/data.db";
            SqliteConnection connection = new SqliteConnection($"Data Source = {dataBaseFile}");
            while(true)
            {
                Console.WriteLine(@"Available commands:
                1. Generate information
                2. Exit");
                string command = Console.ReadLine();
                if(command == "1")
                {
                    string generatorPath = "/home/valeria/Desktop/progbase3/data/generator/";
                    Console.Write("Type of entity to generate: ");
                    string entity = Console.ReadLine();
                    Console.Write("How many entities you want to generate? ");
                    string numberOfEntities = Console.ReadLine();
                    if(!int.TryParse(numberOfEntities, out int number) || number <= 0)
                    {
                        Console.WriteLine("Error: Number of entities must be positive integer.");
                        continue;
                    }
                    if(entity == "film")
                    {
                        FilmRepository repo = new FilmRepository(connection);
                        Console.Write("Please, enter start of range of release years to generate: ");
                        string inputStart = Console.ReadLine();
                        if(!int.TryParse(inputStart, out int start) || start <= 0)
                        {
                            Console.WriteLine("Error: Year must be positive integer.");
                            continue;
                        }
                        Console.Write("Please, enter end of range of release years to generate: ");
                        string inputEnd = Console.ReadLine();
                        if(!int.TryParse(inputEnd, out int end) || end <= 0 || end > 2021)
                        {
                            Console.WriteLine("Error: Year must be positive integer and not bigger than 2021.");
                            continue;
                        }
                        if(start >= end)
                        {
                            Console.WriteLine("Error: start must be less than end.");
                            continue;
                        }
                        Console.WriteLine("Data is generating...");
                        for(int i = 0; i<number; i++)
                        {
                            string title = GenerateFromFile(generatorPath + "titles.txt");
                            string genre = GenerateFromFile(generatorPath + "genres.txt");
                            Random ran = new Random();
                            int releaseYear = ran.Next(start, end+1);
                            Film film = new Film();
                            film.title = title;
                            film.genre = genre;
                            film.releaseYear = releaseYear;
                            repo.Insert(film);
                        }
                        Console.WriteLine("Done!");
                    }
                    else if(entity == "actor")
                    {
                        ActorRepository repo = new ActorRepository(connection);
                        Console.Write("Please, enter start of range of birth dates to generate: ");
                        string inputStart = Console.ReadLine();
                        if(!DateTime.TryParse(inputStart, out DateTime start))
                        {
                            Console.WriteLine("Error: Birth date must be in date format.");
                            continue;
                        }
                        Console.Write("Please, enter end of range of birth dates to generate: ");
                        string inputEnd = Console.ReadLine();
                        if(!DateTime.TryParse(inputEnd, out DateTime end))
                        {
                            Console.WriteLine("Error: Birth date must be in date format.");
                            continue;
                        }
                        if(start >= end)
                        {
                            Console.WriteLine("Error: start must be less than end.");
                            continue;
                        }
                        int range = (end-start).Days;
                        Console.WriteLine("Data is generating...");
                        for(int i = 0; i<number; i++)
                        {
                            string fullname = GenerateFromFile(generatorPath + "fullnames.txt");
                            string country = GenerateFromFile(generatorPath + "countries.txt");
                            Random ran = new Random();
                            DateTime birthDate = start.AddDays(ran.Next(range));
                            Actor actor = new Actor();
                            actor.fullname = fullname;
                            actor.country = country;
                            actor.birthDate = birthDate;
                            repo.Insert(actor);
                        }
                        Console.WriteLine("Done!");
                    }
                    else if(entity == "review")
                    {
                        ReviewRepository repo = new ReviewRepository(connection);
                        Console.Write("Please, enter start of range of post datetime to generate: ");
                        string inputStart = Console.ReadLine();
                        if(!DateTime.TryParse(inputStart, out DateTime start))
                        {
                            Console.WriteLine("Error: Post datetime must be in date format.");
                            continue;
                        }
                        Console.Write("Please, enter end of range of post datetime to generate: ");
                        string inputEnd = Console.ReadLine();
                        if(!DateTime.TryParse(inputEnd, out DateTime end))
                        {
                            Console.WriteLine("Error: Post datetime must be in date format.");
                            continue;
                        }
                        if(start >= end)
                        {
                            Console.WriteLine("Error: start must be less than end.");
                            continue;
                        }
                        TimeSpan range = end-start;
                        Console.WriteLine("Data is generating...");
                        for(int i = 0; i<number; i++)
                        {
                            string opinion = GenerateFromFile(generatorPath + "opinions.txt");
                            Random ran = new Random();
                            int rating = ran.Next(1, 11);
                            TimeSpan ts = new TimeSpan((long)(ran.NextDouble() * range.Ticks));
                            DateTime postedAt = start + ts;
                            Review review = new Review();
                            review.opinion = opinion;
                            review.rating = rating;
                            review.postedAt = postedAt;
                            repo.Insert(review);
                        }
                        Console.WriteLine("Done!");
                    }
                    else
                    {
                        Console.Error.WriteLine("Error: Unavailable entity.");
                        continue;
                    }
                }
                else if(command == "2")
                {
                    Console.WriteLine("End.");
                    break;
                }
                else
                {
                    Console.Error.WriteLine("Error: Unavailable command.");
                    continue;
                }
            }
        }
        static string GenerateFromFile(string filePath)
        {
            int numberOfLines = File.ReadAllLines(filePath).Length;
            Random ran = new Random();
            int ranNumber = ran.Next(1, numberOfLines+1);
            string result = "";
            StreamReader sr = new StreamReader(filePath);
            int counter = 0;
            while(counter != ranNumber)
            {
                result = sr.ReadLine();
                counter++;
            }
            return result;
        }
    }
}
