using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ООО_Лабиринт.Classes;

namespace ООО_Лабиринт.Views
{
    /// <summary>
    /// Логика взаимодействия для MyOrdersWindow.xaml
    /// </summary>
    public partial class MyOrdersWindow : Window
    {
        public MyOrdersWindow()
        {
            InitializeComponent();
        }

        /// При загрузке окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowOrders();
        }

        /// Метод отображает информацию о заказах с дополнительными свойствами
        /// </summary>
        private void ShowOrders()
        {

            /*listOrders = Helper.DB.Order.ToList();	//Получить все заказы
            int totalCount = listOrders.Count;		//Их общее количество
            listProductsInOrders = Helper.DB.OrderProduct.ToList(); //Получить все заказанные товары
            //Создание списка заказанных товаров с расширенными свойствами
            listExtendedOrders = new List<OrderExtended>();
            foreach (var order in listOrders)
            {
                Classes.OrderExtended orderExtended = new OrderExtended();
                orderExtended.Order = order;		//Заполнить данные о заказе из БД
                //Вызов методов класса через объект для вычисления дополнительных свойств
                orderExtended.TotalSumma = orderExtended.CalcTotalSummma(listProductsInOrders);
                orderExtended.TotalDiscount = orderExtended.CalcTotalDiscount(listProductsInOrders);
                listExtendedOrders.Add(orderExtended);
            }*/
            List<OrderExtended> orders = App.DB.Order.Where(x => x.OrderClient == App.User.UserID).ToList().ConvertAll(item => new OrderExtended(item));
            listViewOrders.ItemsSource = null;
            listViewOrders.ItemsSource = orders;
        }

        /// Выбор заказа в списке заказов
        private void listViewOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (listViewOrders.SelectedItem == null)
                return;

            var selectedOrder = listViewOrders.SelectedItem as OrderExtended;

            //Отображение товаров выбранного заказа
            listViewOrder.ItemsSource = selectedOrder.OrderBookDatas;
            /*selectedOrder.OrderBookDatas.First().Book.BookName;
            selectedOrder.OrderBookDatas.First().Book.BookPrice;
            selectedOrder.OrderBookDatas.First().Book.BookDiscount;
            selectedOrder.OrderBookDatas.First().PriceWithDiscount;
            selectedOrder.*/
                

        }

        private void buttonNavigation_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
