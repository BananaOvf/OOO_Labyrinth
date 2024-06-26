//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ООО_Лабиринт.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class BookData
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BookData()
        {
            this.Book = new HashSet<Book>();
        }
    
        public int BookID { get; set; }
        public string BookName { get; set; }
        public string BookAuthor { get; set; }
        public double BookPrice { get; set; }
        public int BookDiscount { get; set; }
        public int BookPublishingHouse { get; set; }
        public string BookPicture { get; set; }
        public string BookDescription { get; set; }
        public int BookYear { get; set; }
        public int BookGenre { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Book> Book { get; set; }
        public virtual Genre Genre { get; set; }
        public virtual PublishingHouse PublishingHouse { get; set; }
    }
}
