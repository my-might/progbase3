using System;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Collections.Generic;
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
                Service repo = new Service(connection);
                if(command == "1")
                {
                    ProcessGenerateFilms(generatorPath, repo);
                }
                else if(command == "2")
                {
                    ProcessGenerateActors(generatorPath, repo);
                }
                else if(command == "3")
                {
                    ProcessGenerateReviews(generatorPath, repo);
                }
                else if(command == "4")
                {
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
        static void ProcessGenerateFilms(string generatorPath, Service repo)
        {
            int[] actorsId = repo.actorRepository.GetAllIds();
            if(actorsId.Length == 0)
            {
                Console.WriteLine("Error: Cannot generate films, when database doesn`t contain any actors.");
                while(true)
                {
                    Console.WriteLine("Do you want to generate actors too? (y/n)");
                    string answer = Console.ReadLine().ToLower();
                    if(answer == "n")
                    {
                        return;
                    }
                    else if(answer == "y")
                    {
                        Console.WriteLine("Process generating actors:");
                        ProcessGenerateActors(generatorPath, repo);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Error: Unavailable option. Try again.");
                        continue;
                    }
                }
            }
            Console.WriteLine("Process generating films:");
            int number = GetNumberOfEntities();
            actorsId = repo.actorRepository.GetAllIds();
            int start = 0;
            int end = 0;
            int minActors = 0;
            int maxActors = 0;
            while(true)
            {
                Console.Write("Please, enter start of range of release year to generate: ");
                string inputStart = Console.ReadLine();
                if(!int.TryParse(inputStart, out start) || start < 1895) //1895-first film
                {
                    Console.WriteLine("Error: Year must be positive integer and more than 1894. Try again.");
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
            while(true)
            {
                Console.Write("Please, enter min number of actors to generate for each film: ");
                string inputStart = Console.ReadLine();
                if(!int.TryParse(inputStart, out minActors) || start > actorsId.Length || start <= 0)
                {
                    Console.WriteLine("Error: Number must be positive integer and available for existing number of actors. Try again.");
                    continue;
                }
                Console.Write("Please, enter max number of actors to generate for each film: ");
                string inputEnd = Console.ReadLine();
                if(!int.TryParse(inputEnd, out maxActors) || end <= 0 || end > actorsId.Length)
                {
                    Console.WriteLine("Error: Number must be positive integer and available for existing number of actors. Try again.");
                    continue;
                }
                if(start > end)
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
                long filmId = repo.filmRepository.Insert(film);
                int numberOfActors = ran.Next(minActors, maxActors+1);
                for(int j = 0; j<numberOfActors; j++)
                {
                    int currentActorId = ran.Next(0, actorsId.Length);
                    if(repo.roleRepository.IsExist(filmId, currentActorId))
                    {
                        j--;
                        continue;
                    }
                    Role role = new Role();
                    role.filmId = (int)filmId;
                    role.actorId = currentActorId;
                    repo.roleRepository.Insert(role);
                }
            }
            Console.WriteLine("Done!");
        }
        static void ProcessGenerateActors(string generatorPath, Service repo)
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
                    Console.WriteLine("Error: Birth date mustn`t be bigger than datetime now. Try again.");
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
                repo.actorRepository.Insert(actor);
            }
            Console.WriteLine("Done!");
        }
        static void ProcessGenerateReviews(string generatorPath, Service repo)
        {
            int[] filmsId = repo.filmRepository.GetAllIds();
            if(filmsId.Length == 0)
            {
                Console.Error.WriteLine("Error: Cannot generate reviews, when database doesn`t contain any films.");
                return;
            }
            int[] usersId = repo.userRepository.GetAllIds();
            if(usersId.Length == 0)
            {
                Console.Error.WriteLine("Error: Cannot generate reviews, when database doesn`t contain any users.");
                return;
            }
            int minYear = repo.filmRepository.GetMinReleaseYear();
            DateTime minReg = repo.userRepository.GetMinRegistrationDate();
            int number = GetNumberOfEntities();
            DateTime start;
            DateTime end;
            while(true)
            {
                Console.Write("Please, enter start of range of post datetime to generate: ");
                string inputStart = Console.ReadLine();
                if(!DateTime.TryParse(inputStart, out start) || start.Year < minYear || start < minReg)
                {
                    Console.WriteLine("Error: Post datetime must be in date format and available for films and users. Try again.");
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
                    Console.WriteLine("Error: Post datetime mustn`t be bigger than datetime now. Try again.");
                    continue;
                }
                if(start > end)
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
                int index = ran.Next(0, filmsId.Length);
                for(int j = index; j<=filmsId.Length; j++)
                {
                    if(j == filmsId.Length)
                    {
                        j = -1;
                    }
                    else if(repo.filmRepository.GetById(filmsId[j]).releaseYear < review.postedAt.Year)
                    {
                        review.filmId = filmsId[j];
                        break;
                    }
                }
                int userIndex = ran.Next(0, usersId.Length);
                for(int j = userIndex; j<=usersId.Length; j++)
                {
                    if(j == usersId.Length)
                    {
                        j = -1;
                    }
                    else if(repo.userRepository.GetById(usersId[j]).registrationDate < review.postedAt)
                    {
                        review.userId = usersId[j];
                        break;
                    }
                }
                repo.reviewRepository.Insert(review);
            }
            Console.WriteLine("Done!");
        }
        static void ProcessGenerateUsers(string generatorPath, Service repo)
        {
            int number = GetNumberOfEntities();
            DateTime start;
            DateTime end;
            while(true)
            {
                Console.Write("Please, enter start of range of registration datetime to generate: ");
                string inputStart = Console.ReadLine();
                if(!DateTime.TryParse(inputStart, out start) || start.Year < 2000)
                {
                    Console.WriteLine("Error: Registration datetime must be in date format and not less than 2000 year. Try again.");
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
                    Console.WriteLine("Error: Registration datetime mustn`t be bigger than datetime now. Try again.");
                    continue;
                }
                if(start > end)
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
                repo.userRepository.Insert(user);
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
