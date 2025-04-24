using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Schema;

namespace EcoRegion
{
    public partial class gramms : Window
    {
        //public static List<Polutions> stat;
        public gramms(string actualReg, string year)
        {
            ECO_capitalismEntities eco = new ECO_capitalismEntities();
            InitializeComponent();
            MessageBox.Show(" ? " + actualReg + " код года  - " + year);
            int year2 = Convert.ToInt32(year);
            int god_code = eco.Лета.FirstOrDefault(k => k.Год == year2).Код_год;
            

            int region = eco.Регион_.FirstOrDefault(i => i.Наименование == actualReg).Код;
            List <Статистика_> selected = eco.Статистика_.Where(u => u.Регион == region && u.Год == god_code).ToList();

            var article = eco.Статья_.ToList();

            var mybar1 = Graphics.Plot.Add.Bar(position: 1, value: selected[0].СуммаЗатрат);
            mybar1.LegendText = "Текущие затраты";

            var mybar2 = Graphics.Plot.Add.Bar(position: 2, value: selected[1].СуммаЗатрат);
            mybar2.LegendText = "Инвестиционные капиталы";

            var mybar3 = Graphics.Plot.Add.Bar(position: 3, value: selected[2].СуммаЗатрат);
            mybar3.LegendText = "На природоохранный ремонт";

            //Выводим Легенду и заголовок
            Graphics.Plot.ShowLegend(Edge.Right);

            //Убираем подписи для OX
            Graphics.Plot.Axes.Bottom.TickLabelStyle.IsVisible = false;
            //Формируем график вплотную к OX и OY
            Graphics.Plot.Axes.Margins(bottom: 0, left: 0);
            //Подпись для OX
            Graphics.Plot.XLabel($"Диаграмма за {year2} по {actualReg}");
            //Обновляем график в соответствии с новыми данными
            Graphics.Refresh();


        }
    }
}
