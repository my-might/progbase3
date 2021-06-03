using ScottPlot;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System;
using System.Xml;
using System.Xml.Linq;
namespace ClassLib
{
    public static class GenerateObjects
    {
        private static Actor actor;
        private static string directoryPath;
        private static Service repo;
        private static void GenerateImage()
        {
            ScottPlot.Plot plt = new ScottPlot.Plot(600, 450);
            List<Film> films = repo.roleRepository.GetForImage(actor.id);
            if(films.Count != 0)
            {
                double[][] values = ValuesForImage(films);
                string[] titles = TitlesForImage(films);
                OHLC[] ohlcs = new OHLC[values.GetLength(0)];
                for(int i = 0; i<values.GetLength(0); i++)
                {
                    ohlcs[i] = new OHLC(values[i][0], values[i][1], values[i][2], values[i][3], i, 1);
                }
                plt.AddCandlesticks(ohlcs);
                string[] tickLabels = titles;
                plt.XTicks(tickLabels);
            }
            plt.YLabel("Rating");
            plt.XLabel("Film ids");
            plt.Title($"Film reviews changes for actor witn id {actor.id}");

            plt.SaveFig("./../data/sample/word/media/image1.png");
        }
        public static void GenerateReport()
        {
            string examplePath = "./../data/Sample.docx";
            string extractPath = "./../data/sample";
            ZipFile.ExtractToDirectory(examplePath, extractPath);

            XElement root = XElement.Load("./../data/sample/word/document.xml");
            FindAndReplace(root);
            root.Save("./../data//sample/word/document.xml");
            File.Delete("./../data/sample/word/media/image1.png");
            GenerateImage();
            ZipFile.CreateFromDirectory(extractPath, directoryPath + $"/Report_{DateTime.Now.ToString().Replace("/", ".")}.docx");
            Directory.Delete("./../data/sample", true);
        }
        private static void FindAndReplace(XElement node)
        {
            if (node.FirstNode != null && node.FirstNode.NodeType == XmlNodeType.Text)
            {
                switch (node.Value)
                {
                    case "{{": node.Value = ""; break;
                    case "}}": node.Value = ""; break;
                    case "fullname": node.Value = actor.fullname; break;
                    case "filmsNumber": node.Value = repo.roleRepository.GetAllFilms(actor.id).Count.ToString(); break;
                    case "averageRating": 
                        if(GetAverageActorRating() == -1)
                        {
                            node.Value = "no films for actor";
                        }
                        else
                        {
                            node.Value = GetAverageActorRating().ToString(); 
                        }
                        break;
                    case "titleHigh": 
                        if(GetFilmHighestRating() == null)
                        {
                            node.Value = "x";
                        }
                        else
                        {
                            node.Value = GetFilmHighestRating().title;
                        }
                        break;
                    case "genreHigh":
                        if(GetFilmHighestRating() == null)
                        {
                            node.Value = "x";
                        }
                        else
                        {
                            node.Value = GetFilmHighestRating().genre;
                        }
                        break;
                    case "descHigh": 
                        if(GetFilmHighestRating() == null)
                        {
                            node.Value = "x";
                        }
                        else
                        {
                            node.Value = GetFilmHighestRating().description;
                        }
                        break;
                    case "yearHigh": 
                        if(GetFilmHighestRating() == null)
                        {
                            node.Value = "x";
                        }
                        else
                        {
                            node.Value = GetFilmHighestRating().releaseYear.ToString();
                        }
                        break;
                    case "ratingHigh": 
                        if(GetFilmHighestRating() == null)
                        {
                            node.Value = "x";
                        }
                        else
                        {
                            node.Value = repo.reviewRepository.GetAverageFilmRating(GetFilmHighestRating().id).ToString();
                        }
                        break;
                    case "titleLow": 
                        if(GetFilmLowestRating() == null)
                        {
                            node.Value = "x";
                        }
                        else
                        {
                            node.Value = GetFilmLowestRating().title;
                        }
                        break;
                    case "genreLow": 
                        if(GetFilmLowestRating() == null)
                        {
                            node.Value = "x";
                        }
                        else
                        {
                            node.Value = GetFilmLowestRating().genre;
                        }
                        break;
                    case "descLow": 
                        if(GetFilmLowestRating() == null)
                        {
                            node.Value = "x";
                        }
                        else
                        {
                            node.Value = GetFilmLowestRating().description;
                        }
                        break;
                    case "yearLow": 
                        if(GetFilmLowestRating() == null)
                        {
                            node.Value = "x";
                        }
                        else
                        {
                            node.Value = GetFilmLowestRating().releaseYear.ToString();
                        }
                        break;
                    case "ratingLow": 
                        if(GetFilmLowestRating() == null)
                        {
                            node.Value = "x";
                        }
                        else
                        {
                            node.Value = repo.reviewRepository.GetAverageFilmRating(GetFilmLowestRating().id).ToString();
                        }
                        break;
                }
            }
        
            foreach (XElement el in node.Elements())
            {
                FindAndReplace(el);
            }
        }
        private static double GetAverageActorRating()
        {
            List<Film> films = repo.roleRepository.GetAllFilms(actor.id);
            if(films.Count == 0)
            {
                return -1;
            }
            double sum = 0;
            foreach(Film film in films)
            {
                if(repo.reviewRepository.GetAverageFilmRating(film.id) != 0)
                {
                    sum += repo.reviewRepository.GetAverageFilmRating(film.id);
                }
            }
            double result = Math.Round((double)sum/films.Count, 2);
            return result;
        }
        private static Film GetFilmHighestRating()
        {
            List<Film> films = repo.roleRepository.GetAllFilms(actor.id);
            if(films.Count == 0)
            {
                return null;
            }
            Film result = null;
            double rating = 0;
            foreach(Film film in films)
            {
                if(repo.reviewRepository.GetAverageFilmRating(film.id) > rating)
                {
                    rating = repo.reviewRepository.GetAverageFilmRating(film.id);
                    result = film;
                }
            }
            return result;
        }
        private static Film GetFilmLowestRating()
        {
            List<Film> films = repo.roleRepository.GetAllFilms(actor.id);
            if(films.Count == 0)
            {
                return null;
            }
            Film result = null;
            double rating = 11;
            foreach(Film film in films)
            {
                if(repo.reviewRepository.GetAverageFilmRating(film.id) < rating 
                    && repo.reviewRepository.GetAverageFilmRating(film.id) != 0)
                {
                    rating = repo.reviewRepository.GetAverageFilmRating(film.id);
                    result = film;
                }
            }
            return result;
        }
        public static void SetInfo(Actor actor1, string directoryPath1, Service repo1)
        {
            actor = actor1;
            directoryPath = directoryPath1;
            repo = repo1;
        }
        public static string[] TitlesForImage(List<Film> films)
        {
            string[] result = new string[films.Count];
            for(int i = 0; i<films.Count; i++)
            {
                result[i] = films[i].id.ToString();
            }
            return result;
        }
        private static double[][] ValuesForImage(List<Film> films)
        {
            double[][] result = new double[films.Count][];
            for(int i = 0; i<films.Count; i++)
            {
                result[i] = new double[4];
                if(films[i].reviews.Count != 0)
                {
                    result[i][0] = films[i].reviews[0].rating;
                    result[i][3] = films[i].reviews[films[i].reviews.Count-1].rating;
                    double min = films[i].reviews[0].rating;
                    double max = films[i].reviews[0].rating;
                    foreach(Review review in films[i].reviews)
                    {
                        if(review.rating > max)
                        {
                            max = review.rating;
                        }
                        else if(review.rating < min)
                        {
                            min = review.rating;
                        }
                    }
                    result[i][1] = max;
                    result[i][2] = min;
                }
                else
                {
                    result[i][0] = 0;
                    result[i][1] = 0;
                    result[i][2] = 0;
                    result[i][3] = 0;
                }
            }
            return result;
        }
    }
}
