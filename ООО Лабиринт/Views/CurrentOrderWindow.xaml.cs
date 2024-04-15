using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
using ООО_Лабиринт.Classes;
using ООО_Лабиринт.Model;
using static ООО_Лабиринт.Constants;

namespace ООО_Лабиринт.Views
{
    /// <summary>
    /// Логика взаимодействия для CurrentOrderWindow.xaml
    /// </summary>
    public partial class CurrentOrderWindow : Window
    {
        private HashSet<OrderItem> _order;

        public CurrentOrderWindow()
        {
            InitializeComponent();
        }

        public CurrentOrderWindow(HashSet<OrderItem> currentOrder)
        {
            InitializeComponent();

            this._order = currentOrder;
            listBoxBooksInOrder.ItemsSource = _order;

            ShowInfo();
        }

        private void ShowInfo()
        {
            listBoxBooksInOrder.ItemsSource = null;
            listBoxBooksInOrder.ItemsSource = _order;
            CalcOrder();
        }

        private void CalcOrder()
        {
            double total = 0;
            foreach (var book in _order)
                total += (double)book.BookExtended.PriceWithDiscount * book.Amount;
            tbTotal.Text = "Сумма заказа: " + total;
        }

        private void buttonNavigation_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        Random random = new Random();
        private void butCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_order.Count <= 0)
            {
                MessageBox.Show("Сохранять нечего");
                return;
            }
            
            Model.User user = App.DB.User.FirstOrDefault(x => cbFullName.Text == x.UserFullName);
            if(user == null)
            {
                MessageBox.Show("Клиент не найден");
                return;
            }

            Model.Order order = new Model.Order();

            string randomCode;
            do { randomCode = random.Next(1000).ToString().PadLeft(3, '0'); } 
                while (App.DB.Order.Count(x => randomCode == x.OrderID) > 0);
            order.OrderID = randomCode;

            order.OrderClient = user.UserID;
            order.OrderManager = App.User.UserID;
            order.OrderDate = DateTime.Now.AddDays(3);
            order.OrderStatus = OrderStatusIDs.FULLY_PAID;

            App.DB.Order.Add(order);
            try
            {
                App.DB.SaveChanges();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"К сожалению, создать новый заказ не удалось\n{ex.Message}");
                return;
            }


            foreach(var book in _order)
            {
                var booksInStock = App.DB.Book.Where(x => x.BookDataID == book.BookExtended.Book.BookID && x.BookInOrder == false).Take(book.Amount);
                foreach(var bookInStock in booksInStock)
                {
                    Model.OrderBook orderBook = new Model.OrderBook();
                    orderBook.OrderID = order.OrderID;
                    orderBook.BookArticle = bookInStock.BookArticle;
                    bookInStock.BookInOrder = true;

                    App.DB.OrderBook.Add(orderBook);
                }
            }
            try
            {
                App.DB.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"К сожалению, добавить книги в заказ не удалось\n{ex.Message}");
                return;
            }
            _order.Clear();

            MessageBox.Show("Запись в базу произошла успешно!");

            this.Close();
        }

        private void butDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var book = (sender as Button).DataContext as OrderItem;
            deleteBookFromOrder(book);
        }

        private bool deleteBookFromOrder(OrderItem book)
        {
            var res = MessageBox.Show($"Вы уверены, что хотите удалить из заказа все книги \"{book.BookExtended.Book.BookName}\"?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res == MessageBoxResult.No)
                return false;

            _order.Remove(book);
            
            ShowInfo();

            return true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string newAmountStr = (sender as TextBox).Text;
            if (string.IsNullOrEmpty(newAmountStr))
                return;

            var book = (sender as TextBox).DataContext as OrderItem;

            int newAmount;
            if(!int.TryParse(newAmountStr, out newAmount))
            {
                MessageBox.Show("Ошибка, вы ввели что-то непонятное");
                (sender as TextBox).Text = book.Amount.ToString();
                return;
            }

            if (newAmount == 0)
            {
                bool isDeleted = deleteBookFromOrder(book);
                if(!isDeleted)
                    (sender as TextBox).Text = book.Amount.ToString();
                return;
            }

            try
            {
                book.Amount = newAmount;
            }
            catch
            {
                MessageBox.Show("Введенное количество превышает количество на складе");
                (sender as TextBox).Text = book.Amount.ToString();
            }

            ShowInfo();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var users = App.DB.User.ToList().ConvertAll(us => us.UserFullName);
            cbFullName.ItemsSource = users;
            cbFullName.SelectedIndex = 0;

            tbFIO.Text = App.User != null ? App.User.UserFullName : GUEST;
        }
    }
}
