using ScottPlot.ArrowShapes;
using ScottPlot.TickGenerators.TimeUnits;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO.Packaging;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static OpenTK.Graphics.OpenGL.GL;
using static System.Convert;

namespace EcoRegion
{
    public class Jids 
    {
        public int ID { get; set; }
        public string Region { get; set; }
        public double ActualExpense { get; set; } //текущие затраты
        public double InvCap { get; set; }//инвестеционный капитал направленный на охрану окружающей среды
        public double ExpCapRemont { get; set; }//затраты на капитальный ремонт окружающей среды
        public double SumOfNatExp { get; set; }//суммарные расходы на природоохранные
    }

    public partial class MainWindow : Window
    {   
        ECO_capitalismEntities eco = new ECO_capitalismEntities();
        public static List<Jids> Listok = new List<Jids>();
        public static List<Регион_> regions = new List<Регион_>();
        public static List<Лета> years = new List<Лета>();
        public MainWindow()
        {
            InitializeComponent();
            years = eco.Лета.ToList();
            cbRegions.ItemsSource = eco.Регион_.Select(u => u.Наименование).ToList();
            regions = eco.Регион_.ToList();
            cbYear.ItemsSource = eco.Лета.Select(u => u.Год).ToList();
            AllIn();
        }
        public void AllIn()//метад
        {
            lblOut.Content = null;
            lblOut2.Content = null;
            Egypt.ItemsSource = null;
            Listok.Clear();
            foreach (Регион_ stats in regions)
            {
                Jids jidik = new Jids();
                jidik.ID = stats.Код;
                jidik.Region = stats.Наименование;
                Listok.Add(jidik);

                jidik.ActualExpense = 0;
                jidik.InvCap = 0;
                jidik.ExpCapRemont = 0;
                jidik.SumOfNatExp = 0;
            }

            foreach (Jids lis in Listok)
            {
                foreach (Лета yr in years)
                {
                    //заполняем листок, суммируя все наши года, таблица будет выводить общее число деняк средии всех лет
                    lis.ActualExpense += eco.Статистика_.FirstOrDefault(j => (double)j.Регион == lis.ID && j.Статья == 1 && j.Год == yr.Код_год).СуммаЗатрат;
                    lis.InvCap += eco.Статистика_.FirstOrDefault(j => (double)j.Регион == lis.ID && j.Статья == 2 && j.Год == yr.Код_год).СуммаЗатрат;
                    lis.ExpCapRemont += eco.Статистика_.FirstOrDefault(j => (double)j.Регион == lis.ID && j.Статья == 3 && j.Год == yr.Код_год).СуммаЗатрат;
                    lis.SumOfNatExp += lis.ExpCapRemont + lis.InvCap + lis.ActualExpense;
                }
            }
            Egypt.ItemsSource = Listok;
            //MessageBox.Show(" " + Listok.Count() + " " + regions.Count());
        }

        double SumActualYear = 0;//суммма за актуальный год
        double SumUnctualYear = 0;//суммма за предактуальный год
        public static int yearcode = 0; //выбранный код года
        int actualyear = 0; //выбранный год
        public static string actualReg; //выбранный регион
        private void cbYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Egypt.ItemsSource = null;
            if (cbYear.SelectedItem != null) 
            {
                //чтоб найти и подставть по годам суммы, надо сначала код года найти, патамушта в комбобоксе не код года а год 
                actualyear = ToInt32(cbYear.SelectedItem);
                int unctualyear = 0;
                int tyearcode = 0;
                int yearcode = eco.Лета.FirstOrDefault(y => y.Год == actualyear).Код_год;
                if (actualyear != years[0].Год)
                {
                    unctualyear = actualyear - 1;
                    tyearcode = eco.Лета.FirstOrDefault(y => y.Год == unctualyear).Код_год;
                }

                foreach (Jids lis in Listok)
                {
                    if (unctualyear != 0)
                    {
                    //общие затраты за предыдущий выбранному год
                    SumUnctualYear += eco.Статистика_.FirstOrDefault(j => (double)j.Регион == lis.ID && j.Статья == 3
                    && j.Год == tyearcode).СуммаЗатрат + eco.Статистика_.FirstOrDefault(j => (double)j.Регион == lis.ID
                    && j.Статья == 2 && j.Год == tyearcode).СуммаЗатрат + eco.Статистика_.FirstOrDefault(j => (double)j.Регион == lis.ID
                    && j.Статья == 2 && j.Год == tyearcode).СуммаЗатрат + eco.Статистика_.FirstOrDefault(j => (double)j.Регион == lis.ID &&
                    j.Статья == 1 && j.Год == tyearcode).СуммаЗатрат;
                    }
                 
                //сумма за выбранный год для вывода
                lis.ActualExpense = eco.Статистика_.FirstOrDefault(j => (double)j.Регион == lis.ID &&
                j.Статья == 1 && j.Год == yearcode).СуммаЗатрат; //сумма затрат это столбец
                lis.InvCap = eco.Статистика_.FirstOrDefault(j => (double)j.Регион == lis.ID && j.Статья == 2 && j.Год == yearcode).СуммаЗатрат;
                lis.ExpCapRemont = eco.Статистика_.FirstOrDefault(j => (double)j.Регион == lis.ID && j.Статья == 3 && j.Год == yearcode).СуммаЗатрат;
                    lis.SumOfNatExp = lis.ExpCapRemont + lis.InvCap + lis.ActualExpense;


                SumActualYear += lis.SumOfNatExp; //сумма за выбранный год для вывода
                }
              
                if (cbRegions.SelectedItem != null)
                {
                    actualReg = cbRegions.SelectedItem.ToString();
                    Egypt.ItemsSource = Listok.Where(u => u.Region == actualReg);
                }
                else
                    Egypt.ItemsSource = Listok;

                chet();
            }
        }

        private void btShowAllRegions_Click(object sender, RoutedEventArgs e)
        {
            cbYear.SelectedItem = null;
            cbRegions.SelectedItem = null;
            AllIn();
        }

        private void cbRegions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            actualyear = ToInt32(cbYear.SelectedItem);
            if (cbRegions.SelectedItem != null)
            {
                
                Egypt.ItemsSource = null;
                
                string actualReg = cbRegions.SelectedItem.ToString(); 
                Egypt.ItemsSource = Listok.Where(u => u.Region == actualReg);
            }
            chet();
        }
        private void chet()
        {
            double sum = 0;
            if (actualyear != years[0].Год) { 

                
                if (SumUnctualYear - SumActualYear > SumUnctualYear - SumActualYear)
                    sum = SumUnctualYear - SumActualYear;
                else
                    sum = SumActualYear - SumUnctualYear;

                lblOut.Content = $"Изменение суммарных природоохранных затрат с " +
                     $"{cbYear.SelectedItem} г. к {actualyear - 1} г. {Math.Round(SumActualYear / SumUnctualYear * 100, 2)}%";
                lblOut2.Content = $"Изменение суммарных природоохранных затрат с " +
                $"{cbYear.SelectedItem} г. к {actualyear - 1} г. в рублях: {sum} руб";
            }
            else
            {
                lblOut.Content = "этот год не сравнить с предыдущим";
                lblOut2.Content = "этот год не сравнить с предыдущим";
            }
        }


        private void btDiagramm_Click(object sender, RoutedEventArgs e)
        {
            if (cbRegions.SelectedItem != null && cbYear.SelectedItem != null)
            {
                gramms grams = new gramms(cbRegions.SelectedItem.ToString(), cbYear.SelectedItem.ToString());
                grams.Show();
            }      
        }
    }
}
