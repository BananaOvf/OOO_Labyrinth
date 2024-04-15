using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ООО_Лабиринт.Model;

namespace ООО_Лабиринт
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Доступное свойство связи с БД
        public static DBLabyrinth DB { get; set; }

        // Доступное свойство текущего пользователя
        public static User User { get; set; }
    }

    public static class Constants
    {
        public const string ALL_GENRES = "Все жанры";
        public const string GUEST = "Гость";

        public static class SortDirections
        {
            public const string DEFAULT = "По умолчанию";
            public const string ASCENDING = "По возрастанию";
            public const string DESCENDING = "По убыванию";
        }

        public static class UserRoleIDs
        {
            public const int CLIENT = 1;
            public const int MANAGER = 2;
            public const int ADMINISTRATOR = 3;
        }

        public static class OrderStatusIDs
        {
            public const int POSTPONED = 1;
            public const int ON_CREDIT = 2;
            public const int PARTIALLY_PAID = 3;
            public const int FULLY_PAID = 4;
            public const int RETURNED = 5;
        }

    }
}
