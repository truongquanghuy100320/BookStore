using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Models;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostCategory> PostCategories { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<TransactionStatus> TransactionStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=ADMIN\\SQLEXPRESS02;Database=BookStore;user id=sa;password=12345678;trusted_connection=true;encrypt=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("Author");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Image)
                .HasMaxLength(555)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Publish)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Story).HasColumnType("text");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("Book");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.IllustrationsNote).HasColumnType("text");
            entity.Property(e => e.Image)
                .HasMaxLength(555)
                .IsUnicode(false);
            entity.Property(e => e.Isbn10)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Isbn13)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Language)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NameAuthor)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NameCate)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.PublicationDate).HasColumnType("datetime");
            entity.Property(e => e.Publisher)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK_Book_Author");

            entity.HasOne(d => d.Category).WithMany(p => p.Books)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Book_Category");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.ToTable("Contact");

            entity.Property(e => e.Address)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Message).HasColumnType("text");
            entity.Property(e => e.Phone)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.Property(e => e.Address)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Salt)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.Address)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.DesPayment)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.DesTransactionStatus)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Note).HasColumnType("text");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.ShipDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(250)
                .IsUnicode(false);

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Order_Customer");

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK_Order_Payment");

            entity.HasOne(d => d.TransactStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TransactStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_TransactionStatus");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.ToTable("OrderDetail");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Books).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.BooksId)
                .HasConstraintName("FK_OrderDetail_Book");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderDetail_Order");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payment");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.PaymentDescription)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.PaymentInfo)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Updated).HasColumnType("datetime");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("Post");

            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.Image).HasMaxLength(250);
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.NameAuthor).HasMaxLength(250);
            entity.Property(e => e.Title).HasMaxLength(250);

            entity.HasOne(d => d.Category).WithMany(p => p.Posts)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Post_PostCategory");

            entity.HasOne(d => d.Customer).WithMany(p => p.Posts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Post_Customer");
        });

        modelBuilder.Entity<PostCategory>(entity =>
        {
            entity.ToTable("PostCategory");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("Sale");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TransactionStatus>(entity =>
        {
            entity.HasKey(e => e.TransactStatusId);

            entity.ToTable("TransactionStatus");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Status)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Updated).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
