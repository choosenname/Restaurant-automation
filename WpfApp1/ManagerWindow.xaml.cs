using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.Models;
using WpfApp1.Models.Database;
using Window = System.Windows.Window;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        DatabaseContext db = new DatabaseContext();
        public ManagerWindow()
        {
            InitializeComponent();

            GetReport();
        }

        private void GetReport()
        {
            decimal nal = db.Kassa.First(x => x.Id == 1).Nalichny, card = db.Kassa.First(x => x.Id == 1).Card, ret = db.Kassa.First(x => x.Id == 1).Return;
            string example = $"================================\n" 
                + $"Ресторан\n\n" 
                + $"Дата и время: {DateTime.Now.ToString("dd.MM.yyyy HH:mm")} PM\n"
                + $"Z-отчет\n" + $"--------------------------------\n" 
                + $"Кол-во наличных: {nal}\n" 
                + $"Кол-во по карте: {card}\n" 
                + $"Кол-во отмен: {ret}\n"
                + $"Итог: {nal + card - ret}\n"
                + "================================";

            textBox.Text = example;
        }

        private void End_Click(object sender, RoutedEventArgs e)
        {
            Kassa kassa = db.Kassa.First(x => x.Id == 1);
            kassa.Nalichny = 0;
            kassa.Card = 0;
            kassa.Return = 0;
            db.SaveChanges();
            MessageBox.Show("Z-отчет выписан");
            this.Close();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Visible = true;
            Document doc = wordApp.Documents.Add();
            doc.Content.Text = textBox.Text;

            doc.SaveAs2("report.docx");

            MessageBox.Show("Отчет сохранен");
        }
    }
}
