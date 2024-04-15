using EasyCaptcha.Wpf;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ООО_Лабиринт
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int loginAttempt = 1;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                App.DB = new Model.DBLabyrinth(); // Связь с БД
            }
            catch
            {
                MessageBox.Show("Отсутствует БД");
                return;
            }
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonGuest_Click(object sender, RoutedEventArgs e)
        {
            App.User = null;
            ShowCatalog();
        }

        bool isUserChange = true;
        private void tbPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUserChange)
            {
                pbPassword.Password = tbPassword.Text;
            }
        }

        private void pbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            isUserChange = false;
            tbPassword.Text = pbPassword.Password;
            isUserChange = true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            pbPassword.Visibility = Visibility.Collapsed;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            pbPassword.Visibility = Visibility.Visible;
        }

        private async void buttonLogIn_Click(object sender, RoutedEventArgs e)
        {
            string logIn = tbLogin.Text;
            string password = pbPassword.Password;

            // Проверки на ввод логина и пароля
            string message = "";
            if (string.IsNullOrEmpty(logIn))
                message += "Вы забыли ввести логин\n";
            if (string.IsNullOrEmpty(password))
                message += "Вы забыли ввести пароль\n";
            if (message.Length > 0)
            {
                MessageBox.Show(message);
                return;
            }

            // Проверка на бота, если требуется
            if (loginAttempt > 1 && !captcha.CaptchaText.Equals(tbCaptcha.Text))
            {
                MessageBox.Show("Упс! Кажется Вы бот");
                PrepareCaptcha();
                return;
            }

            // Попытаться авторизовать пользователя
            bool authorizationSuccess = Authorize(logIn, password);
            if (authorizationSuccess)
            {
                loginAttempt = 1;
                ShowCaptcha = false;
                ClearCredentialFields();
                ShowCatalog();
                return;
            }

            MessageBox.Show("Пользователь не найден");

            // Блокировка на 10 секунд
            if (loginAttempt >= 2)
            {
                buttonLogIn.IsEnabled = false;
                MessageBox.Show("Слишком много попыток. Придется немного подождать");

                string buttonContent = buttonLogIn.Content.ToString();
                buttonLogIn.Foreground = new SolidColorBrush(Colors.Black);

                for (int secondsLeft = 10; secondsLeft > 0; secondsLeft--)
                {
                    buttonLogIn.Content = secondsLeft;
                    await Task.Delay(1000);
                }

                buttonLogIn.Foreground = new SolidColorBrush(Colors.White);
                buttonLogIn.Content = buttonContent;

                buttonLogIn.IsEnabled = true;
            }

            PrepareCaptcha();
            ShowCaptcha = true;

            loginAttempt++;
        }

        private bool Authorize(string logIn, string password)
        {
            App.User = App.DB.User.Where(x => x.UserLogin.Equals(logIn) && x.UserPassword.Equals(password)).FirstOrDefault();
            return App.User != null;
        }

        private void ClearCredentialFields()
        {
            tbLogin.Clear();
            pbPassword.Clear();
        }

        private bool ShowCaptcha
        {
            set
            {
                if (value)
                    captchaGrid.Height = GridLength.Auto;
                else captchaGrid.Height = new GridLength(0);
            }
        }

        private void PrepareCaptcha(Captcha.LetterOption letterOption = Captcha.LetterOption.Alphanumeric, int numberOfLetters = 4)
        {
            tbCaptcha.Clear();
            captcha.CreateCaptcha(letterOption, numberOfLetters);
        }

        private void ShowCatalog()
        {
            Views.Catalog catalog = new Views.Catalog();
            this.Hide();
            catalog.Closed += (s, args) => this.Show(); // Показываем текущее окно после закрытия дочернего
            catalog.ShowDialog();
        }
    }
}
