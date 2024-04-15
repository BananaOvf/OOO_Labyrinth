using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ООО_Лабиринт.Model;

namespace ООО_Лабиринт.Classes
{
    public class BookExtended
    {
        public BookData Book {  get; set; } 

        public BookExtended(BookData book) 
        { 
            Book = book;
        }

        public string PicturePath {
            get 
            {
                var basePath = Environment.CurrentDirectory + "/images/" + Book.BookPicture; 
                return File.Exists(basePath) ? basePath : "/Resources/logo.png";
            } 
        }

        public bool InOrder
        {
            get {
                var book = App.DB.Book.Where(bk => bk.BookDataID == Book.BookID).FirstOrDefault();
                if (book == null || book.BookInOrder == false)
                    return false;
                return true;
            }
        }

        public double PriceWithDiscount { get => Book.BookPrice * (1 - Book.BookDiscount / 100.0); }
        public bool IsDiscountNotZero { get => Book.BookDiscount != 0; }

    }
}
