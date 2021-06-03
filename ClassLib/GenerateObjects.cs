using ScottPlot;
using System.Collections.Generic;
namespace ClassLib
{
    public static class GenerateObjects
    {
        private static Actor actor;
        private static string filePath;
        private static Service repo;
        public static void GenerateImage()
        {
            List<Film> films = repo.roleRepository.GetForImage(actor.id);
            double[][] values = repo.roleRepository.ValuesForImage(films);
            string[] titles = repo.roleRepository.TitlesForImage(films);
            ScottPlot.Plot plt = new ScottPlot.Plot(600, 450);
            OHLC[] ohlcs = new OHLC[values.GetLength(0)];
            for(int i = 0; i<values.GetLength(0); i++)
            {
                ohlcs[i] = new OHLC(values[i][0], values[i][1], values[i][2], values[i][3], i, 1);
            }
            plt.AddCandlesticks(ohlcs);
            string[] tickLabels = titles;
            plt.XTicks(tickLabels);
            plt.YLabel("Rating");
            plt.XLabel("Film ids");
            plt.Title($"Film reviews changes for actor witn id {actor.id}");

            plt.SaveFig(filePath);
        }
        public static void GenerateReport()
        {

        }
        public static void SetInfo(Actor actor1, string filePath1, Service repo1)
        {
            actor = actor1;
            filePath = filePath1;
            repo = repo1;
        }
    }
}