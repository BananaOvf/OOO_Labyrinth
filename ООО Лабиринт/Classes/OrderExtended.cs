using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ООО_Лабиринт.Classes
{
    public class OrderExtended
    {
        public Model.Order Order { get; set; }

        public OrderExtended(Model.Order order)
        {
            this.Order = order;
        }

        public double OrderSum
        {
            get => OrderBookDatas.Sum(x => x.PriceWithDiscount);
        }

        public List<BookExtended> OrderBookDatas
        {
            get => App.DB.OrderBook.Where(x => x.OrderID == this.Order.OrderID).ToList()
                    .ConvertAll(item => new BookExtended(item.Book.BookData));
        }

    }
}
