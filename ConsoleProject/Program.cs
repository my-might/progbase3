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
                1. Generate films
                2. Generate actors
                3. Generate reviews
                4. Generate users
                5. Exit");
                string command = Console.ReadLine();
                string generatorPath = "/home/valeria/Desktop/progbase3/data/generator/";
                if(command == "1")
                {
                    FilmRepository repo = new FilmRepository(connection);
                    ProcessGenerateFilms(generatorPath, repo);
                }
                else if(command == "2")
                {
                    ActorRepository repo = new ActorRepository(connection);
                    ProcessGenerateActors(generatorPath, repo);
                }
                else if(command == "3")
                {
                    ReviewRepository repo = new ReviewRepository(connection);
                    ProcessGenerateReviews(generatorPath, repo);
                }
                else if(command == "4")
                {
                    UserRepository repo = new UserRepository(connection);
                    ProcessGenerateUsers(generatorPath, repo);
                }
                else if(command == "5")
                {
                    Console.WriteLine("End.");
                    break;
                }
                else
                {
                    Console.Error.WriteLine("Error: Unavailable command. Try again.");
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
        static void ProcessGenerateFilms(string generatorPath, FilmRepository repo)
        {
            int number = GetNumberOfEntities();
            int start = 0;
            int end = 0;
            while(true)
            {
                Console.Write("Please, enter start of range of release year to generate: ");
                string inputStart = Console.ReadLine();
                if(!int.TryParse(inputStart, out start) || start <= 0)
                {
                    Console.WriteLine("Error: Year must be positive integer. Try again.");
                    continue;
                }
                Console.Write("Please, enter end of range of release year to generate: ");
                string inputEnd = Console.ReadLine();
                if(!int.TryParse(inputEnd, out end) || end <= 0 || end > 2021)
                {
                    Console.WriteLine("Error: Year must be positive integer and not bigger than 2021. Try again.");
                    continue;
                }
                if(start >= end)
                {
                    Console.WriteLine("Error: Start must be less than end. Try again.");
                    continue;
                }
                break;
            }
            Console.WriteLine("Data is generating...");
            for(int i = 0; i<number; i++)
            {
                Film film = new Film();
                film.title = GenerateFromFile(generatorPath + "titles.txt");
                film.genre = GenerateFromFile(generatorPath + "genres.txt");
                film.description = GenerateFromFile(generatorPath + "descriptions.txt");
                Random ran = new Random();
                film.releaseYear = ran.Next(start, end+1);
                repo.Insert(film);
            }
            Console.WriteLine("Done!");
        }
        static void ProcessGenerateActors(string generatorPath, ActorRepository repo)
        {
            int number = GetNumberOfEntities();
            DateTime start;
            DateTime end;
            while(true)
            {
                Console.Write("Please, enter start of range of birth date to generate: ");
                string inputStart = Console.ReadLine();
                if(!DateTime.TryParse(inputStart, out start))
                {
                    Console.WriteLine("Error: Birth date must be in date format. Try again.");
                    continue;
                }
                Console.Write("Please, enter end of range of birth date to generate: ");
                string inputEnd = Console.ReadLine();
                if(!DateTime.TryParse(inputEnd, out end))
                {
                    Console.WriteLine("Error: Birth date must be in date format. Try again.");
                    continue;
                }
                if(end > DateTime.Now)
                {
                    Console.WriteLine("Error: End mustn`t be bigger than datetime now. Try again.");
                    continue;
                }
                if(start >= end)
                {
                    Console.WriteLine("Error: Start must be less than end. Try again.");
                    continue;
                }
                break;
            }
            int range = (end-start).Days;
            Console.WriteLine("Data is generating...");
            for(int i = 0; i<number; i++)
            {
                Actor actor = new Actor();
                actor.fullname = GenerateFromFile(generatorPath + "fullnames.txt");
                actor.country = GenerateFromFile(generatorPath + "countries.txt");
                Random ran = new Random();
                actor.birthDate = start.AddDays(ran.Next(range));
                repo.Insert(actor);
            }
            Console.WriteLine("Done!");
        }
        static void ProcessGenerateReviews(string generatorPath, ReviewRepository repo)
        {
            int number = GetNumberOfEntities();
            DateTime start;
            DateTime end;
            while(true)
            {
                Console.Write("Please, enter start of range of post datetime to generate: ");
                string inputStart = Console.ReadLine();
                if(!DateTime.TryParse(inputStart, out start))
                {
                    Console.WriteLine("Error: Post datetime must be in date format. Try again.");
                    continue;
                }
                Console.Write("Please, enter end of range of post datetime to generate: ");
                string inputEnd = Console.ReadLine();
                if(!DateTime.TryParse(inputEnd, out end))
                {
                    Console.WriteLine("Error: Post datetime must be in date format. Try again.");
                    continue;
                }
                if(end > DateTime.Now)
                {
                    Console.WriteLine("Error: End mustn`t be bigger than datetime now. Try again.");
                    continue;
                }
                if(start >= end)
                {
                    Console.WriteLine("Error: Start must be less than end. Try again.");
                    continue;
                }
                break;
            }
            TimeSpan range = end-start;
            Console.WriteLine("Data is generating...");
            for(int i = 0; i<number; i++)
            {
                Review review = new Review();
                review.opinion = GenerateFromFile(generatorPath + "opinions.txt");
                Random ran = new Random();
                review.rating = ran.Next(1, 11);
                TimeSpan ts = new TimeSpan((long)(ran.NextDouble() * range.Ticks));
                review.postedAt = start + ts;
                repo.Insert(review);
            }
            Console.WriteLine("Done!");
        }
        static void ProcessGenerateUsers(string generatorPath, UserRepository repo)
        {
            int number = GetNumberOfEntities();
            DateTime start;
            DateTime end;
            while(true)
            {
                Console.Write("Please, enter start of range of registration datetime to generate: ");
                string inputStart = Console.ReadLine();
                if(!DateTime.TryParse(inputStart, out start))
                {
                    Console.WriteLine("Error: Registration datetime must be in date format. Try again.");
                    continue;
                }
                Console.Write("Please, enter end of range of registration datetime to generate: ");
                string inputEnd = Console.ReadLine();
                if(!DateTime.TryParse(inputEnd, out end))
                {
                    Console.WriteLine("Error: Registration datetime must be in date format. Try again.");
                    continue;
                }
                if(end > DateTime.Now)
                {
                    Console.WriteLine("Error: End mustn`t be bigger than datetime now. Try again.");
                    continue;
                }
                if(start >= end)
                {
                    Console.WriteLine("Error: Start must be less than end. Try again.");
                    continue;
                }
                break;
            }
            TimeSpan range = end-start;
            Console.WriteLine("Data is generating...");
            for(int i = 0; i<number; i++)
            {
                User user = new User();
                user.username = GenerateFromFile(generatorPath + "usernames.txt");
                user.fullname = GenerateFromFile(generatorPath + "fullnames.txt");
                Random ran = new Random();
                user.password = ran.Next(10000000, 99999999).ToString();
                TimeSpan ts = new TimeSpan((long)(ran.NextDouble() * range.Ticks));
                user.registrationDate = start + ts;
                repo.Insert(user);
            }
            Console.WriteLine("Done!");
        }
        static int GetNumberOfEntities()
        {
            int number = 0;
            while(true)
            {
                Console.Write("How many entities you want to generate? ");
                string numberOfEntities = Console.ReadLine();
                if(!int.TryParse(numberOfEntities, out number) || number <= 0)
                {
                    Console.WriteLine("Error: Number of entities must be positive integer. Try again.");
                    continue;
                }
                break;
            }
            return number;
        }
    }
}
