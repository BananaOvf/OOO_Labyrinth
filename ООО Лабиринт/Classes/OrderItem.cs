using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ООО_Лабиринт.Model;

namespace ООО_Лабиринт.Classes
{
    public class OrderItem
    {
        public BookExtended BookExtended { get; private set; }

        private int _amount = 0;
        public int Amount {
            get => _amount;
            set
            {
                if (value > App.DB.Book.Count(x => BookExtended.Book.BookID == x.BookDataID && x.BookInOrder == false) || value < 0)
                    throw new ArgumentException("Неверное количество");
                _amount = value;
            }
        }

        public OrderItem(BookExtended book)
        {
            BookExtended = book;
        }

        public override int GetHashCode()
        {
            return BookExtended.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            OrderItem other = (OrderItem)obj;
            return BookExtended.Equals(other.BookExtended);
        }

    }
}
