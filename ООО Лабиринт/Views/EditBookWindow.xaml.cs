using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ООО_Лабиринт.Classes;
using ООО_Лабиринт.Model;

namespace ООО_Лабиринт.Views
{
    /// <summary>
    /// Логика взаимодействия для EditBookWindow.xaml
    /// </summary>
    public partial class EditBookWindow : Window
    {
        private BookExtended CurrentBook;

        OpenFileDialog dlg = new OpenFileDialog();
        bool isPhoto = false;		//Наличие фото
        string filePath;			//Путь к фото из диалога
        //Путь к папке с фотографиями
        string pathPhoto = System.IO.Directory.GetCurrentDirectory() + @"\images\";

        public EditBookWindow(BookExtended book)
        {
            InitializeComponent();
            this.CurrentBook = book;
        }

        private void butDeletefromDB_Click(object sender, RoutedEventArgs e)
        {

            App.DB.Book.RemoveRange(App.DB.Book.Where(book => book.BookDataID == CurrentBook.Book.BookID));
            App.DB.BookData.Remove(CurrentBook.Book);
            try
            {
                App.DB.SaveChanges();		//Обновить БД и при добавлении/редактировании
                MessageBox.Show("БД успешно обновлена");
            }
            catch
            {
                MessageBox.Show("При обновлении БД возникли проблемы");
            }
            this.Close();
        }

        private void butSaveInDB_Click(object sender, RoutedEventArgs e)
        {

            StringBuilder sb = new StringBuilder();	//Строка с сообщениями

            //Проверка непустых значений для обязательных полей
            if (String.IsNullOrEmpty(tbName.Text))
            { sb.Append("Вы не ввели название." + Environment.NewLine); }
            if (String.IsNullOrEmpty(tbAuthor.Text))
            { sb.Append("Вы не ввели автора." + Environment.NewLine); }
            if (String.IsNullOrEmpty(tbYear.Text))
            { sb.Append("Вы не ввели год издания." + Environment.NewLine); }
            if (String.IsNullOrEmpty(tbCost.Text))
            { sb.Append("Вы не ввели цену." + Environment.NewLine); }
            if (String.IsNullOrEmpty(tbDiscount.Text))
            { sb.Append("Вы не ввели скидку." + Environment.NewLine); }
            if (sb.Length > 0)			//Есть сообщения об ошибках
            {
                MessageBox.Show(sb.ToString());
                return;
            }

            //Есть фото
            if (isPhoto)
            {
                CurrentBook.Book.BookPicture = CurrentBook.Book.BookID + ".jpg";	 //Для записи в БД
                string newPath = pathPhoto + CurrentBook.Book.BookPicture;	//Полное имя файла цели
                try
                {
                    if (System.IO.File.Exists(newPath))
                        System.IO.File.Delete(newPath);

                    System.IO.File.Copy(filePath, newPath, true); //Копирование из диалога в целевую
                } catch(Exception ex) { MessageBox.Show("Не удалось скопировать файл. " + ex.Message); return; }
            }

            //Получаем значения для всех остальных полей при добавлении/редактировании
            CurrentBook.Book.BookName = tbName.Text;
            CurrentBook.Book.BookAuthor = tbAuthor.Text;
            CurrentBook.Book.BookPublishingHouse = (int)cbPublishingHouse.SelectedValue;
            CurrentBook.Book.BookYear = Convert.ToInt32(tbYear.Text);
            CurrentBook.Book.BookGenre = (int)cbGenre.SelectedValue;
            CurrentBook.Book.BookPrice = Convert.ToDouble(tbCost.Text);
            CurrentBook.Book.BookDiscount = Convert.ToInt32(tbDiscount.Text);

            try
            {
                App.DB.SaveChanges();		//Обновить БД и при добавлении/редактировании
                MessageBox.Show("БД успешно обновлена");
            }
            catch
            {
                MessageBox.Show("При обновлении БД возникли проблемы");
            }
            this.Close();
        }

        private void butSelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            if(dlg.ShowDialog() == true)
            {
                filePath = dlg.FileName;
                //Отобразить фото в компоненте
                imagePhoto.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
                isPhoto = true;		 //Есть фото
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Настройка диалога
            dlg.InitialDirectory = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Downloads";
            dlg.Filter = "Image files (*.jpg)|*.jpg";

            cbPublishingHouse.DisplayMemberPath = "PublishingHouseName";
            cbPublishingHouse.SelectedValuePath = "PublishingHouseID";
            cbPublishingHouse.ItemsSource = App.DB.PublishingHouse.ToList();
            cbPublishingHouse.SelectedIndex = 0;

            cbGenre.DisplayMemberPath = "GenreName";
            cbGenre.SelectedValuePath = "GenreID";
            cbGenre.ItemsSource = App.DB.Genre.ToList();
            cbGenre.SelectedIndex = 0;

            tbName.Text = CurrentBook.Book.BookName;
            tbAuthor.Text = CurrentBook.Book.BookAuthor;
            cbGenre.Text = CurrentBook.Book.Genre.GenreName;
            cbPublishingHouse.Text = CurrentBook.Book.PublishingHouse.PublishingHouseName;
            tbYear.Text = CurrentBook.Book.BookYear.ToString();
            tbCost.Text = CurrentBook.Book.BookPrice.ToString();
            tbDiscount.Text = CurrentBook.Book.BookDiscount.ToString();
            imagePhoto.Source = new BitmapImage(new Uri(CurrentBook.PicturePath, UriKind.Absolute));
        }

        private void buttonNavigation_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
