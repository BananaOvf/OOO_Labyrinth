using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ООО_Лабиринт.Classes;
using static ООО_Лабиринт.Constants;
//using static System.Linq.IQueryable<ООО_Лабиринт.Classes.BookExtended>;

namespace ООО_Лабиринт.Views
{
    /// <summary>
    /// Логика взаимодействия для Catalog.xaml
    /// </summary>
    public partial class Catalog : Window
    {
        private HashSet<OrderItem> currentOrder = new HashSet<OrderItem>();

        public Catalog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Catalog_Loaded(object sender, RoutedEventArgs e)
        {
            buttonAddBook.Visibility = Visibility.Collapsed;
            buttonMyOrders.Visibility = Visibility.Collapsed;
            buttonCurrentOrder.Visibility = Visibility.Collapsed;
            buttonMyClientsOrders.Visibility = Visibility.Collapsed;

            listBoxContextMenu.Visibility = Visibility.Collapsed;

            listBoxBooks.MouseDoubleClick -= ListBoxBooks_MouseDoubleClick;

            //Информация о пользователе
            if (App.User != null)
            {
                tbFIO.Text = App.User.UserFullName;
                switch(App.User.Role.RoleID)
                {
                    case UserRoleIDs.CLIENT:
                        buttonMyOrders.Visibility = Visibility.Visible;
                        break;
                    case UserRoleIDs.MANAGER:
                        listBoxContextMenu.Visibility = Visibility.Visible;
                        buttonMyClientsOrders.Visibility = Visibility.Visible;
                        break;
                    case UserRoleIDs.ADMINISTRATOR:
                        buttonAddBook.Visibility = Visibility.Visible;
                        listBoxBooks.MouseDoubleClick += ListBoxBooks_MouseDoubleClick;
                        break;

                }
            }
            else tbFIO.Text = GUEST;

            cbGenreFilter.ItemsSource = App.DB.Genre.ToList().ConvertAll(x => x.GenreName).Prepend(ALL_GENRES);
            cbGenreFilter.SelectedIndex = 0;

            string[] sortStates = { SortDirections.DEFAULT, SortDirections.ASCENDING, SortDirections.DESCENDING };
            cbPriceSort.ItemsSource = sortStates;
            cbPriceSort.SelectedIndex = 0;

            ShowBooks();
        }

        private void ListBoxBooks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BookExtended book = listBoxBooks.SelectedItem as BookExtended;
            if (book == null)
                return;
            if(book.InOrder)
            {
                MessageBox.Show("Книга находится в заказе, вы не можете ее редактировать");
                return;
            }

            var editBookWindow = new EditBookWindow(book);
            this.Hide();
            editBookWindow.Closed += (s, args) =>
            {
                this.ShowBooks();
                this.Show();
            };
            editBookWindow.ShowDialog();
        }

        private void ShowBooks()
        {
            List<Classes.BookExtended> books = App.DB.BookData.ToList().ConvertAll(x => new BookExtended(x));
            productsDisplayedNumber.Text = books.Count().ToString();

            books = books.Where(x => x.Book.BookName.ToLower().Contains(tbSearch.Text.Trim(' ').ToLower())).ToList();

            if (cbGenreFilter.SelectedItem != null && !cbGenreFilter.SelectedItem.Equals(ALL_GENRES))
                books = books.Where(x => x.Book.Genre.GenreID == cbGenreFilter.SelectedIndex).ToList();

            switch (cbPriceSort.SelectedItem)
            {
                case SortDirections.ASCENDING:
                    books = books.OrderBy(x => x.PriceWithDiscount).ToList();
                    break;
                case SortDirections.DESCENDING:
                    books = books.OrderByDescending(x => x.PriceWithDiscount).ToList();
                    break;
            }

            listBoxBooks.ItemsSource = books;
            productsDisplayedNumber.Text = books.Count + " / " + productsDisplayedNumber.Text;
        }

        private void List_Update(object sender, SelectionChangedEventArgs e)
        {
            ShowBooks();
        }

        private void List_Update(object sender, TextChangedEventArgs e)
        {
            ShowBooks();
        }

        /// <summary>
        /// Добавить в заказ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*private void miAddInOrder_Click(object sender, RoutedEventArgs e)
        {
            butViewOrder.Visibility = Visibility.Visible;

            Classes.ProductExtended productSelected = listBoxProducts.SelectedItem as Classes.ProductExtended;

            string art = productSelected.Product.ProductArticle;
            Classes.ProductInOrder productSearch = productsInOrder.FirstOrDefault(pr => pr.ProductExtendedInOrder.Product.ProductArticle == art);

            if (productSearch != null)
                productSearch.CountProductInOrder++;
            else
            {
                Classes.ProductInOrder productNew = new Classes.ProductInOrder(productSelected);
                productsInOrder.Add(productNew);
            }

        }*/

        /*private void butViewOrder_Click(object sender, RoutedEventArgs e)
        {
            Order order = new Order(productsInOrder);
            this.Hide();
            order.ShowDialog();
            this.ShowDialog();
        }*/

        //удалить товар 
        /*private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Classes.ProductExtended productSelected = null;
            if (listBoxProducts.SelectedItem == null)	//Проверка, что есть выбранный товар в списке
            {
                MessageBox.Show("Выберите удаляемый товар");
                return;
            }
            //Выбранный товар в каталоге
            productSelected = listBoxProducts.SelectedItem as Classes.ProductExtended;
            if (MessageBox.Show("Вы действительно хотите удалить этот товар?",
                        "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                //Выбранный товар
                Model.Product product = productSelected.Product;
                //Поиск его среди заказанных товаров для правильного удаления
                Model.OrderProduct orderProduct = App.DB.OrderProduct.
                                                  FirstOrDefault(pr => pr.ProductArticle == product.ProductArticle);
                if (orderProduct == null)				//Товар еще не заказывали
                {
                    App.DB.Product.Remove(product); 	//Можно удалять
                    try
                    {
                        App.DB.SaveChanges(); 		//Фиксация изменений в БД
                        MessageBox.Show("Товар успешно удален");
                        ShowProducts();
                    }
                    catch
                    {
                        MessageBox.Show("При удалении возникли проблемы");
                        return;
                    }
                }
                else 					//Товар присутствует в заказах - удалять нельзя
                {
                    MessageBox.Show("Удалить нельзя, т.к. товар есть в заказах");
                    return;
                }
            }
        }*/

        /*private void butAddProduct_Click(object sender, RoutedEventArgs e)
        {
            //Вызов конструктора без параметров
            View.EditCatalog editCatalog = new View.EditCatalog();
            this.Hide();
            editCatalog.ShowDialog();
            this.ShowDialog();
        }*/

        /*private void butWorkOrder_Click(object sender, RoutedEventArgs e)
        {
            WorkOrder workOrder = new WorkOrder();
            this.Hide();
            workOrder.ShowDialog();
            this.ShowDialog();
        }*/

        /*private void listBoxProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (App.User.UserRole == 3) 		//Только для роли администратора 
            {
                //Выбранный товар в каталоге
                Classes.ProductExtended productSelected =
                                                     listBoxProducts.SelectedItem as Classes.ProductExtended;
                //Вызов конструктора с параметром - выбранный товар для редактирования
                View.EditCatalog editCatalog = new View.EditCatalog(productSelected);
                this.Hide();
                editCatalog.ShowDialog();
                this.ShowDialog();
            }
        }*/

        private void buttonAddBook_Click(object sender, RoutedEventArgs e)
        {
            var newBookWindow = new NewBookWindow();
            this.Hide();
            newBookWindow.Closed += (s, args) => this.Show(); // Показываем текущее окно после закрытия дочернего
            newBookWindow.ShowDialog();
        }

        private void buttonMyOrders_Click(object sender, RoutedEventArgs e)
        {
            var myOrdersWindow = new MyOrdersWindow();
            this.Hide();
            myOrdersWindow.Closed += (s, args) => this.Show(); // Показываем текущее окно после закрытия дочернего
            myOrdersWindow.ShowDialog();
        }

        private void buttonMyClientsOrders_Click(object sender, RoutedEventArgs e)
        {
            //ToDo Работа с заказами клиентов
        }

        private void miAddToOrder_Click(object sender, RoutedEventArgs e)
        {
            BookExtended selectedBook = listBoxBooks.SelectedItem as BookExtended;
            OrderItem bookInOrder = currentOrder.FirstOrDefault(item => item.BookExtended == selectedBook);
            if (bookInOrder == null)
            {
                var newItem = new OrderItem(selectedBook);
                if(TryToAddBook(newItem))
                    currentOrder.Add(newItem);
            }
            else TryToAddBook(bookInOrder);

            buttonCurrentOrder.Visibility = currentOrder.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool TryToAddBook(OrderItem orderItem)
        {
            try { orderItem.Amount++; }
            catch
            {
                MessageBox.Show("Такая книга уже закончилась");
                return false;
            }
            return true;
        }

        private void buttonCurrentOrder_Click(object sender, RoutedEventArgs e)
        {
            var currentOrderWindow = new CurrentOrderWindow(currentOrder);
            this.Hide();
            currentOrderWindow.Closed += (s, args) => this.Show(); // Показываем текущее окно после закрытия дочернего
            currentOrderWindow.ShowDialog();
        }
    }
}
