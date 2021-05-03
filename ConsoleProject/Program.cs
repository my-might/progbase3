using System;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Collections.Generic;
namespace ConsoleProject
{
    public class Range<T>
    {
        public T start;
        public T end;
    }
    public class Program
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
            int number = GetNumberOfEntities();
            int[] actorIds = repo.actorRepository.GetAllIds();
            Range<int> rangeOfFilms = new Range<int>();
            bool actorRelation = false;
            if(actorIds.Length == 0)
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
                        rangeOfFilms = GenerateActorsForFilms(generatorPath, repo, number);
                        actorIds = repo.actorRepository.GetAllIds();
                        actorRelation = true;
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
            Range<int> rangeOfRelease = GetRangeOfYears();
            Range<int> rangeOfActors = GetRangeOfActors(actorIds.Length);
            int[] filmIds = new int[number];
            Console.WriteLine("Data is generating...");
            for(int i = 0; i<number; i++)
            {
                Film film = new Film();
                film.title = GenerateFromFile(generatorPath + "titles.txt");
                film.genre = GenerateFromFile(generatorPath + "genres.txt");
                film.description = GenerateFromFile(generatorPath + "descriptions.txt");
                Random ran = new Random();
                film.releaseYear = ran.Next(rangeOfRelease.start, rangeOfRelease.end+1);
                long filmId = repo.filmRepository.Insert(film);
                filmIds[i] = (int)filmId;
            }
            CreateRelationsFilmActor(rangeOfActors, filmIds, actorIds, repo);
            if(actorRelation)
            {
                CreateRelationsActorFilm(rangeOfFilms, actorIds, filmIds, repo);
            }
            Console.WriteLine("Done!");
        }
        static void ProcessGenerateActors(string generatorPath, Service repo)
        {
            int number = GetNumberOfEntities();
            int[] filmIds = repo.filmRepository.GetAllIds();
            Range<int> rangeOfActors = new Range<int>();
            bool filmRelation = false;
            if(filmIds.Length == 0)
            {
                Console.WriteLine("Error: Cannot generate actors, when database doesn`t contain any films.");
                while(true)
                {
                    Console.WriteLine("Do you want to generate films too? (y/n)");
                    string answer = Console.ReadLine().ToLower();
                    if(answer == "n")
                    {
                        return;
                    }
                    else if(answer == "y")
                    {
                        Console.WriteLine("Process generating films:");
                        rangeOfActors = GenerateFilmsForActors(generatorPath, repo, number);
                        filmIds = repo.filmRepository.GetAllIds();
                        filmRelation = true;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Error: Unavailable option. Try again.");
                        continue;
                    }
                }
            }
            Console.WriteLine("Process generating actors:");
            Range<DateTime> rangeOfBirth = GetRangeOfBirthDate();
            Range<int> rangeOfFilms = GetRangeOfActors(filmIds.Length);
            int range = (rangeOfBirth.end-rangeOfBirth.start).Days;
            int[] actorIds = new int[number];
            Console.WriteLine("Data is generating...");
            for(int i = 0; i<number; i++)
            {
                Actor actor = new Actor();
                actor.fullname = GenerateFromFile(generatorPath + "fullnames.txt");
                actor.country = GenerateFromFile(generatorPath + "countries.txt");
                Random ran = new Random();
                actor.birthDate = rangeOfBirth.start.AddDays(ran.Next(range));
                long actorId = repo.actorRepository.Insert(actor);
                actorIds[i] = (int)actorId;
            }
            CreateRelationsActorFilm(rangeOfFilms, actorIds, filmIds, repo);
            if(filmRelation)
            {
                CreateRelationsFilmActor(rangeOfActors, filmIds, actorIds, repo);
            }
            Console.WriteLine("Done!");
        }
        
        static void CreateRelationsFilmActor(Range<int> rangeOfActors, int[] filmIds, int[] actorIds, Service repo)
        {
            Random ran = new Random();
            for(int i = 0; i<filmIds.Length; i++)
            {
                int numberOfActors = ran.Next(rangeOfActors.start, rangeOfActors.end+1);
                for(int j = 0; j<numberOfActors; j++)
                {
                    int actorIndex = ran.Next(0, actorIds.Length);
                    if(repo.roleRepository.IsExist(filmIds[i], actorIds[actorIndex]))
                    {
                        j--;
                        continue;
                    }
                    else
                    {
                        Role role = new Role();
                        role.filmId = filmIds[i];
                        role.actorId = actorIds[actorIndex];
                        repo.roleRepository.Insert(role);
                    }
                }
            }
        }
        static void CreateRelationsActorFilm(Range<int> rangeOfFilms, int[] actorIds, int[] filmIds, Service repo)
        {
            Random ran = new Random();
            for(int i = 0; i<actorIds.Length; i++)
            {
                int numberOfFilms = ran.Next(rangeOfFilms.start, rangeOfFilms.end+1);
                for(int j = 0; j<numberOfFilms; j++)
                {
                    int filmIndex = ran.Next(0, filmIds.Length);
                    if(repo.roleRepository.IsExist(filmIds[filmIndex], actorIds[i]))
                    {
                        j--;
                        continue;
                    }
                    else
                    {
                        Role role = new Role();
                        role.filmId = filmIds[filmIndex];
                        role.actorId = actorIds[i];
                        repo.roleRepository.Insert(role);
                    }
                }
            }
        }
        
        static Range<int> GenerateActorsForFilms(string generatorPath, Service repo, int numberOfFilms)
        {
            int numberOfActors = GetNumberOfEntities();
            Range<DateTime> rangeOfBirth = GetRangeOfBirthDate();
            Range<int> rangeOfFilms = GetRangeOfFilms(numberOfFilms);
            int range = (rangeOfBirth.end-rangeOfBirth.start).Days;
            Console.WriteLine("Data is generating...");
            for(int i = 0; i<numberOfActors; i++)
            {
                Actor actor = new Actor();
                actor.fullname = GenerateFromFile(generatorPath + "fullnames.txt");
                actor.country = GenerateFromFile(generatorPath + "countries.txt");
                Random ran = new Random();
                actor.birthDate = rangeOfBirth.start.AddDays(ran.Next(range));
                repo.actorRepository.Insert(actor);
            }
            Console.WriteLine("Done!");
            return rangeOfFilms;
        }
        static Range<int> GenerateFilmsForActors(string generatorPath, Service repo, int numberOfActors)
        {
            int numberOfFilms = GetNumberOfEntities();
            Range<int> rangeOfRelease = GetRangeOfYears();
            Range<int> rangeOfActors = GetRangeOfActors(numberOfActors);
            Console.WriteLine("Data is generating...");
            for(int i = 0; i<numberOfFilms; i++)
            {
                Film film = new Film();
                film.title = GenerateFromFile(generatorPath + "titles.txt");
                film.genre = GenerateFromFile(generatorPath + "genres.txt");
                film.description = GenerateFromFile(generatorPath + "descriptions.txt");
                Random ran = new Random();
                film.releaseYear = ran.Next(rangeOfRelease.start, rangeOfRelease.end+1);
                long filmId = repo.filmRepository.Insert(film);
            }
            Console.WriteLine("Done!");
            return rangeOfActors;
        }
        static Range<int> GetRangeOfFilms(int numberOfFilms)
        {
            Range<int> range = new Range<int>();
            while(true)
            {
                Console.WriteLine($"Please, enter range of number of films to generate for each actor.(from 1 to {numberOfFilms})");
                Console.Write("Min: ");
                string inputStart = Console.ReadLine();
                if(!int.TryParse(inputStart, out range.start) || range.start > numberOfFilms || range.start <= 0)
                {
                    Console.WriteLine("Error: Number must be positive integer and available for borders. Try again.");
                    continue;
                }
                Console.Write("Max: ");
                string inputEnd = Console.ReadLine();
                if(!int.TryParse(inputEnd, out range.end) || range.end <= 0 || range.end > numberOfFilms)
                {
                    Console.WriteLine("Error: Number must be positive integer and available for borders. Try again.");
                    continue;
                }
                if(range.start > range.end)
                {
                    Console.WriteLine("Error: Start must be less than end. Try again.");
                    continue;
                }
                break;
            }
            return range;
        }
        static Range<int> GetRangeOfYears()
        {
            Range<int> rangeOfRelease = new Range<int>();
            while(true)
            {
                Console.WriteLine($"Please, enter range of release year to generate.(from 1895 to {DateTime.Now.Year})");
                Console.Write("Start: ");
                string inputStart = Console.ReadLine();
                if(!int.TryParse(inputStart, out rangeOfRelease.start) || rangeOfRelease.start < 1895) //1895-first film
                {
                    Console.WriteLine("Error: Year must be positive integer and available for borders. Try again.");
                    continue;
                }
                Console.Write("End: ");
                string inputEnd = Console.ReadLine();
                if(!int.TryParse(inputEnd, out rangeOfRelease.end) || rangeOfRelease.end <= 0 || rangeOfRelease.end > 2021)
                {
                    Console.WriteLine("Error: Year must be positive integer and available for borders. Try again.");
                    continue;
                }
                if(rangeOfRelease.start > rangeOfRelease.end)
                {
                    Console.WriteLine("Error: Start must be less than end. Try again.");
                    continue;
                }
                break;
            }
            return rangeOfRelease;
        }
        static Range<int> GetRangeOfActors(int numberOfActors)
        {
            Range<int> rangeOfActors = new Range<int>();
            while(true)
            {
                Console.WriteLine($"Please, enter range of number of actors to generate for each film.(from 1 to {numberOfActors})");
                Console.Write("Min: ");
                string inputStart = Console.ReadLine();
                if(!int.TryParse(inputStart, out rangeOfActors.start) || rangeOfActors.start > numberOfActors || rangeOfActors.start <= 0)
                {
                    Console.WriteLine("Error: Number must be positive integer and available for borders. Try again.");
                    continue;
                }
                Console.Write("Max: ");
                string inputEnd = Console.ReadLine();
                if(!int.TryParse(inputEnd, out rangeOfActors.end) || rangeOfActors.end <= 0 || rangeOfActors.end > numberOfActors)
                {
                    Console.WriteLine("Error: Number must be positive integer and available for borders. Try again.");
                    continue;
                }
                if(rangeOfActors.start > rangeOfActors.end)
                {
                    Console.WriteLine("Error: Start must be less than end. Try again.");
                    continue;
                }
                break;
            }
            return rangeOfActors;
        }
        static Range<DateTime> GetRangeOfBirthDate()
        {
            Range<DateTime> range = new Range<DateTime>();
            while(true)
            {
                Console.WriteLine($"Please, enter range of birth date to generate.");
                Console.Write("Start: ");
                string inputStart = Console.ReadLine();
                if(!DateTime.TryParse(inputStart, out range.start))
                {
                    Console.WriteLine("Error: Birth date must be in date format. Try again.");
                    continue;
                }
                Console.Write("End: ");
                string inputEnd = Console.ReadLine();
                if(!DateTime.TryParse(inputEnd, out range.end) || range.end > DateTime.Now)
                {
                    Console.WriteLine("Error: Birth date must be in date format. Try again.");
                    continue;
                }
                if(range.start > range.end)
                {
                    Console.WriteLine("Error: Start must be less than end. Try again.");
                    continue;
                }
                break;
            }
            return range; 
        }
        static void ProcessGenerateReviews(string generatorPath, Service repo)
        {
            int[] filmIds = repo.filmRepository.GetAllIds();
            if(filmIds.Length == 0)
            {
                Console.Error.WriteLine("Error: Cannot generate reviews, when database doesn`t contain any films.");
                return;
            }
            int[] userIds = repo.userRepository.GetAllIds();
            if(userIds.Length == 0)
            {
                Console.Error.WriteLine("Error: Cannot generate reviews, when database doesn`t contain any users.");
                return;
            }
            DateTime min = GetMin(repo);
            int numberOfEntities = GetNumberOfEntities();
            DateTime start;
            DateTime end;
            while(true)
            {
                Console.WriteLine($"Please, enter range of post datetime to generate.(from {min.ToString("o")} to {DateTime.Now.ToString("o")})");
                Console.Write("Start: ");
                string inputStart = Console.ReadLine();
                if(!DateTime.TryParse(inputStart, out start) || start < min)
                {
                    Console.Error.WriteLine("Error: Post datetime must be in date format and available for borders. Try again.");
                    continue;
                }
                Console.Write("End: ");
                string inputEnd = Console.ReadLine();
                if(!DateTime.TryParse(inputEnd, out end) || end > DateTime.Now)
                {
                    Console.Error.WriteLine("Error: Post datetime must be in date format and available for borders. Try again.");
                    continue;
                }
                if(start > end)
                {
                    Console.Error.WriteLine("Error: Start must be less than end. Try again.");
                    continue;
                }
                break;
            }
            TimeSpan range = end-start;
            Console.WriteLine("Data is generating...");
            for(int i = 0; i<numberOfEntities; i++)
            {
                Review review = new Review();
                review.opinion = GenerateFromFile(generatorPath + "opinions.txt");
                Random ran = new Random();
                review.rating = ran.Next(1, 11);
                TimeSpan ts = new TimeSpan((long)(ran.NextDouble() * range.Ticks));
                review.postedAt = start + ts;
                review.filmId = GetFilmId(repo, filmIds, review);
                review.userId = GetUserId(repo, userIds, review);
                repo.reviewRepository.Insert(review);
            }
            Console.WriteLine("Done!");
        }
        static DateTime GetMin(Service repo)
        {
            int minYear = repo.filmRepository.GetMinReleaseYear();
            DateTime minRegistration = repo.userRepository.GetMinRegistrationDate();
            if(minYear > minRegistration.Year)
            {
                return new DateTime(minYear, 1, 1);
            }
            else
            {
                return minRegistration;
            }
        }
        static int GetFilmId(Service repo, int[] filmIds, Review review)
        {
            Random ran = new Random();
            int index = ran.Next(0, filmIds.Length);
            for(int j = index; j<=filmIds.Length; j++)
            {
                if(j == filmIds.Length)
                {
                    j = -1;
                }
                else if(repo.filmRepository.GetById(filmIds[j]).releaseYear < review.postedAt.Year)
                {
                    return filmIds[j];
                }
            }
            return 0;
        }
        static int GetUserId(Service repo, int[] userIds, Review review)
        {
            Random ran = new Random();
            int index = ran.Next(0, userIds.Length);
            for(int j = index; j<=userIds.Length; j++)
            {
                if(j == userIds.Length)
                {
                    j = -1;
                }
                else if(repo.userRepository.GetById(userIds[j]).registrationDate < review.postedAt)
                {
                    return userIds[j];
                }
            }
            return 0;
        }
        static void ProcessGenerateUsers(string generatorPath, Service repo)
        {
            int number = GetNumberOfEntities();
            DateTime start;
            DateTime end;
            while(true)
            {
                Console.WriteLine($"Please, enter range of registration datetime to generate.(from {new DateTime(2000, 1, 1).ToString("o")} to {DateTime.Now.ToString("o")}) ");
                Console.Write("Start: ");
                string inputStart = Console.ReadLine();
                if(!DateTime.TryParse(inputStart, out start) || start.Year < 2000)
                {
                    Console.WriteLine("Error: Registration datetime must be in date format and available for borders. Try again.");
                    continue;
                }
                Console.Write("End: ");
                string inputEnd = Console.ReadLine();
                if(!DateTime.TryParse(inputEnd, out end) || end > DateTime.Now)
                {
                    Console.WriteLine("Error: Registration datetime must be in date format and available for borders. Try again.");
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
