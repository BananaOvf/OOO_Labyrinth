using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
using ООО_Лабиринт.Model;

namespace ООО_Лабиринт.Views
{
    /// <summary>
    /// Логика взаимодействия для NewBookWindow.xaml
    /// </summary>
    public partial class NewBookWindow : Window
    {
        OpenFileDialog dlg = new OpenFileDialog();
        bool isPhoto = false;		//Наличие фото
        string filePath;			//Путь к фото из диалога
        //Путь к папке с фотографиями
        string pathPhoto = System.IO.Directory.GetCurrentDirectory() + @"\images\";
        Model.Book book;	//Товар, с которым сейчас работают

        /// <summary>
        /// Конструктор окна без параметров - при добавлении нового товара
        /// </summary>
        public NewBookWindow()
        {
            InitializeComponent();
            tbTitle.Text = "Добавление товара";
        }

        /// Подготовительные действия
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Заполнение и настройка списков из БД

            cbPublishingHouse.DisplayMemberPath = "PublishingHouseName";
            cbPublishingHouse.SelectedValuePath = "PublishingHouseID";
            cbPublishingHouse.ItemsSource = App.DB.PublishingHouse.ToList();
            cbPublishingHouse.SelectedIndex = 0;

            cbGenre.DisplayMemberPath = "GenreName";
            cbGenre.SelectedValuePath = "GenreID";
            cbGenre.ItemsSource = App.DB.Genre.ToList();
            cbGenre.SelectedIndex = 0;

            //Настройка диалога
            dlg.InitialDirectory = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Downloads";
            dlg.Filter = "Image files (*.jpg)|*.jpg";
        }

        /// Выбрать фото
        private void butSelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            if (dlg.ShowDialog() == true)
            {
                filePath = dlg.FileName;
                //Отобразить фото в компоненте
                imagePhoto.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
                isPhoto = true;		 //Есть фото
            }

        }

        /// Сохранить в БД
        private void butSaveInDB_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();	//Строка с сообщениями
            sb.Clear();
            //Проверка пустоты всех полей
            if (String.IsNullOrEmpty(tbArticle.Text))
            {
                sb.Append("Вы не ввели артикул." + Environment.NewLine);
            }
            else
            {
                //Проверка отсутствия повторения артикля
                Model.Book book = App.DB.Book.FirstOrDefault(x => x.BookArticle == tbArticle.Text);
                if (book != null && book.BookData.BookName != tbName.Text)
                { sb.Append("Такая книга уже существует. Придется добавлять идентичные данные" + Environment.NewLine); }
            }

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

            book = new Model.Book();		//Создаем добавляемый объект
            book.BookArticle = tbArticle.Text;		//Внимание на артикль и фото

            var publishingHouse = (int)cbPublishingHouse.SelectedValue;
            var year = Convert.ToInt32(tbYear.Text);
            var genre = (int)cbGenre.SelectedValue;
            var cost = Convert.ToInt32(tbCost.Text);
            var discount = Convert.ToInt32(tbDiscount.Text);
            var sameBook = App.DB.BookData.Where(x =>
                x.BookName == tbName.Text &&
                x.BookAuthor == tbAuthor.Text &&
                x.BookPublishingHouse == publishingHouse &&
                x.BookYear == year &&
                x.BookGenre == genre &&
                x.BookPrice == cost &&
                x.BookDiscount == discount &&
                x.BookDescription == tbDescription.Text
            ).FirstOrDefault();

            if (sameBook == null)
            {
                book.BookData = new Model.BookData();
                book.BookInOrder = false;

                var lastBookData = App.DB.BookData.OrderByDescending(bd => bd.BookID).FirstOrDefault();
                int newBookID = (lastBookData != null) ? lastBookData.BookID + 1 : 1;
                book.BookDataID = book.BookData.BookID = newBookID;

                //Есть фото
                if (isPhoto)
                {
                    book.BookData.BookPicture = book.BookData.BookID + ".jpg";	 //Для записи в БД
                    string newPath = pathPhoto + book.BookData.BookPicture;	//Полное имя файла цели
                    try
                    {
                        System.IO.File.Copy(filePath, newPath, true); //Копирование из диалога в целевую
                    }
                    catch (Exception ex) { MessageBox.Show("Не удалось скопировать файл. " + ex.Message); return; }
                }

                //Получаем значения для всех остальных полей при добавлении/редактировании
                book.BookData.BookName = tbName.Text;
                book.BookData.BookAuthor = tbAuthor.Text;
                book.BookData.BookPublishingHouse = (int)cbPublishingHouse.SelectedValue;
                book.BookData.BookYear = Convert.ToInt32(tbYear.Text);
                book.BookData.BookGenre = (int)cbGenre.SelectedValue;
                book.BookData.BookPrice = Convert.ToDouble(tbCost.Text);
                book.BookData.BookDiscount = Convert.ToInt32(tbDiscount.Text);
                book.BookData.BookDescription = tbDescription.Text;
            }
            else book.BookDataID = sameBook.BookID;

            App.DB.Book.Add(book);		//Добавить в кэш новую запись
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

        private void buttonNavigation_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
