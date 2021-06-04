using ScottPlot;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System;
using System.Xml;
using System.Xml.Linq;
using RPCLib;
using ClassLib;
using ScottPlot.Plottable;
namespace DataProcessLib
{
    public static class GenerateObjects
    {
        private static Actor actor;
        private static string directoryPath;
        private static RemoteService repo;
        private static void GenerateImage()
        {
            Plot plt = new Plot(600, 400);
            List<Film> films = repo.roleRepository.GetForImage(actor.id);
            double[] values = ValuesForImage(films);
            double[] offsets = OffsetsForImage(films);

            BarPlot bar = plt.AddBar(values);
            bar.ValueOffsets = offsets;
            plt.XTicks(TitlesForImage(films));
            plt.Title("Waterfall Bar Graph");
            bar.FillColorNegative = Color.Red;
            bar.FillColor = Color.Green;

            plt.SaveFig("./../data/sample/word/media/image1.png");
        }
        public static void GenerateReport()
        {
            string examplePath = "./../data/Sample.docx";
            string extractPath = "./../data/sample";
            ZipFile.ExtractToDirectory(examplePath, extractPath);

            XElement root = XElement.Load("./../data/sample/word/document.xml");
            FindAndReplace(root);
            root.Save("./../data/sample/word/document.xml");
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
        public static void SetInfo(Actor actor1, string directoryPath1, RemoteService repo1)
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
        private static double[] ValuesForImage(List<Film> films)
        {
            double[] values = new double[films.Count];
            double sum = 0;
            for(int i = 0; i<films.Count; i++)
            {
                values[i] = repo.reviewRepository.GetAverageFilmRating(films[i].id) - sum;
                sum += repo.reviewRepository.GetAverageFilmRating(films[i].id) - sum;
            }
            return values;
        }
        private static double[] OffsetsForImage(List<Film> films)
        {
            double[] offsets = new double[films.Count];
            double[] values = ValuesForImage(films);
            double sum = 0;
            for(int i = 0; i<films.Count; i++)
            {
                offsets[i] = sum;
                sum += values[i];
                // if(i == 0)
                // {
                //     offsets[i] = 0;
                // }
                // else
                // {
                //     offsets[i] = repo.reviewRepository.GetAverageFilmRating(films[i-1].id);
                // }
            }
            return offsets;
        }
    }
}
