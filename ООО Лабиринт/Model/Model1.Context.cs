﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DBLabyrinth : DbContext
    {
        public DBLabyrinth()
            : base("name=DBLabyrinth")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Book> Book { get; set; }
        public virtual DbSet<BookData> BookData { get; set; }
        public virtual DbSet<Genre> Genre { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderBook> OrderBook { get; set; }
        public virtual DbSet<PublishingHouse> PublishingHouse { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<User> User { get; set; }
    }
}
