using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using ООО_Лабиринт;
using ООО_Лабиринт.Classes;
using ООО_Лабиринт.Model;
using static System.Net.Mime.MediaTypeNames;

namespace ООО_Лабиринт_Tests
{
    [TestClass]
    public class UnitTestsClasses
    {
        private const string TestImageFileName = "test_image.jpg";

        [TestMethod]
        public void TestPicturePath_Exists()
        {
            // Arrange
            var bookData = new BookData { BookPicture = TestImageFileName };
            var bookExtended = new BookExtended(bookData);

            // Act
            var picturePath = bookExtended.PicturePath;

            // Assert
            Assert.IsTrue(File.Exists(picturePath));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestPicturePath_Default()
        {
            // Arrange
            var bookData = new BookData { BookPicture = "non_existing_image.jpg" };
            var bookExtended = new BookExtended(bookData);

            // Act
            var picturePath = bookExtended.PicturePath;

            // Assert
            Assert.AreEqual("/Resources/logo.png", picturePath);
        }

        [TestMethod]
        public void TestInOrder_BookInOrder()
        {
            // Arrange
            var bookData = new BookData { BookID = 1 };
            var bookExtended = new BookExtended(bookData);
            var booksInOrder = new List<Book> { new Book { BookDataID = 1, BookInOrder = true } };
            var mockDbContext = new MockDbContext(booksInOrder);

            // Act
            var isInOrder = bookExtended.InOrder;

            // Assert
            Assert.IsTrue(isInOrder);
        }

        [TestMethod]
        public void TestOrderItem_Amount_Valid()
        {
            // Arrange
            var bookData = new BookData { BookID = 1 };
            var book = new BookExtended(bookData);
            var orderItem = new OrderItem(book);

            // Act
            orderItem.Amount = 1;

            // Assert
            Assert.AreEqual(1, orderItem.Amount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestOrderItem_Amount_Invalid()
        {
            // Arrange
            var bookData = new BookData { BookID = 1 };
            var book = new BookExtended(bookData);
            var orderItem = new OrderItem(book);

            // Act
            orderItem.Amount = -1;

            // Assert
            // Ждем что произойдет исключение ArgumentException
        }








        // Mock DbContext for testing
        public class MockDbContext : DbContext
        {
            public MockDbContext(List<Book> books)
            {
                Book = new MockDbSet<Book>(books);
            }

            public DbSet<Book> Book { get; set; }
        }

        public class MockDbSet<TEntity> : DbSet<TEntity>, IQueryable, IEnumerable<TEntity> where TEntity : class
        {
            private readonly List<TEntity> _data;

            public MockDbSet(IEnumerable<TEntity> data)
            {
                _data = new List<TEntity>(data);
            }

            public override TEntity Find(params object[] keyValues)
            {
                throw new NotImplementedException("Override this method for your test needs.");
            }
/*
            public override IEnumerator<TEntity> GetEnumerator()
            {
                return _data.GetEnumerator();
            }*/

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _data.GetEnumerator();
            }
        }
    }
}