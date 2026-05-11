using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Models;

public class Connected(DbContextOptions<Connected> options) : DbContext(options)
{
    #region DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }
    public DbSet<BookTag> BookTags { get; set; }
    public DbSet<BookLike> BookLikes { get; set; }
    public DbSet<BookAuthor> BookAuthors { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<LibraryEntry> Libraries { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentLike> CommentLikes { get; set; }
    public DbSet<ReviewLike> ReviewLikes { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Users

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Login)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasMany(u => u.BookLikes)
            .WithOne(bl => bl.User)
            .HasForeignKey(bl => bl.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Comments)
            .WithOne(bc => bc.User)
            .HasForeignKey(bc => bc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(b => b.BookAuthors)
            .WithOne(ba => ba.User)
            .HasForeignKey(ba => ba.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Reviews)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.ReviewLikes)
            .WithOne(rl => rl.User)
            .HasForeignKey(rl => rl.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.CommentLikes)
            .WithOne(cl => cl.User)
            .HasForeignKey(cl => cl.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Bookmarks)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Notifications)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Entries)
            .WithOne(le => le.User)
            .HasForeignKey(le => le.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>()
            .HasDefaultValue(Roles.User);

        #endregion

        #region Books

        modelBuilder.Entity<Book>()
            .HasMany(b => b.BookAuthors)
            .WithOne(ba => ba.Book)
            .HasForeignKey(ba => ba.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Book>()
            .Property(b => b.Status)
            .HasConversion<string>()
            .HasDefaultValue(Status.Ongoing);

        modelBuilder.Entity<Book>()
            .HasMany(b => b.BookLikes)
            .WithOne(bl => bl.Book)
            .HasForeignKey(bl => bl.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Book>()
            .HasMany(b => b.Reviews)
            .WithOne(r => r.Book)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Book>()
            .HasMany(b => b.Chapters)
            .WithOne(c => c.Book)
            .HasForeignKey(c => c.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Book>()
            .HasMany(b => b.Notifications)
            .WithOne(n => n.Book)
            .HasForeignKey(n => n.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Book>()
            .HasMany(b => b.BookTags)
            .WithOne(bt => bt.Book)
            .HasForeignKey(bt => bt.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Book>()
            .HasOne(b => b.Genre)
            .WithMany(g => g.Books)
            .HasForeignKey(b => b.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Book>()
            .HasMany(b => b.LibraryEntries)
            .WithOne(le => le.Book)
            .HasForeignKey(le => le.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region BookAuthors

        modelBuilder.Entity<BookAuthor>()
            .HasKey(ba => new { ba.BookId, ba.UserId });

        modelBuilder.Entity<BookAuthor>()
            .HasOne(ba => ba.Book)
            .WithMany(b => b.BookAuthors)
            .HasForeignKey(ba => ba.BookId);

        modelBuilder.Entity<BookAuthor>()
            .HasOne(ba => ba.User)
            .WithMany(u => u.BookAuthors)
            .HasForeignKey(ba => ba.UserId);

        #endregion

        #region BookLikes

        modelBuilder.Entity<BookLike>()
            .HasKey(bl => new { bl.BookId, bl.UserId });

        modelBuilder.Entity<BookLike>()
            .HasOne(bl => bl.Book)
            .WithMany(b => b.BookLikes)
            .HasForeignKey(bl => bl.BookId);

        modelBuilder.Entity<BookLike>()
            .HasOne(bl => bl.User)
            .WithMany(u => u.BookLikes)
            .HasForeignKey(bl => bl.UserId);

        #endregion

        #region Comments

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasMany(c => c.CommentLikes)
            .WithOne(cl => cl.Comment)
            .HasForeignKey(cl => cl.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Chapter)
            .WithMany(ch => ch.Comments)
            .HasForeignKey(c => c.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Review)
            .WithMany(r => r.Comments)
            .HasForeignKey(c => c.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion

        #region CommentLikes

        modelBuilder.Entity<CommentLike>()
            .HasKey(cl => new { cl.CommentId, cl.UserId });

        modelBuilder.Entity<CommentLike>()
            .HasOne(cl => cl.Comment)
            .WithMany(c => c.CommentLikes)
            .HasForeignKey(cl => cl.CommentId);

        modelBuilder.Entity<CommentLike>()
            .HasOne(cl => cl.User)
            .WithMany(u => u.CommentLikes)
            .HasForeignKey(cl => cl.UserId);

        #endregion

        #region Reviews

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Book)
            .WithMany(b => b.Reviews)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<Review>()
            .HasMany(r => r.ReviewLikes)
            .WithOne(rl => rl.Review)
            .HasForeignKey(rl => rl.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasMany(r => r.Comments)
            .WithOne(c => c.Review)
            .HasForeignKey(c => c.ReviewId)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion

        #region ReviewLikes

        modelBuilder.Entity<ReviewLike>()
            .HasKey(rl => new { rl.ReviewId, rl.UserId });

        modelBuilder.Entity<ReviewLike>()
            .HasOne(rl => rl.Review)
            .WithMany(r => r.ReviewLikes)
            .HasForeignKey(rl => rl.ReviewId);

        modelBuilder.Entity<ReviewLike>()
            .HasOne(rl => rl.User)
            .WithMany(u => u.ReviewLikes)
            .HasForeignKey(rl => rl.UserId);

        #endregion

        #region Bookmarks

        modelBuilder.Entity<Bookmark>()
            .HasKey(b => new { b.UserId, b.ChapterId });

        modelBuilder.Entity<Bookmark>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bookmarks)
            .HasForeignKey(b => b.UserId);

        modelBuilder.Entity<Bookmark>()
            .HasOne(b => b.Chapter)
            .WithMany(c => c.Bookmarks)
            .HasForeignKey(b => b.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region Chapters

        modelBuilder.Entity<Chapter>()
            .HasOne(c => c.Book)
            .WithMany(b => b.Chapters)
            .HasForeignKey(c => c.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Chapter>()
            .HasMany(c => c.Bookmarks)
            .WithOne(b => b.Chapter)
            .HasForeignKey(b => b.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Chapter>()
            .HasMany(c => c.Comments)
            .WithOne(c => c.Chapter)
            .HasForeignKey(c => c.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region Subscriptions

        modelBuilder.Entity<Subscription>()
            .HasKey(s => new { s.UserId, s.AuthorId });

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.User)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.Author)
            .WithMany(u => u.Subscribers)
            .HasForeignKey(s => s.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion

        #region Notifications

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Book)
            .WithMany(b => b.Notifications)
            .HasForeignKey(n => n.BookId);

        #endregion

        #region Tags

        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();

        modelBuilder.Entity<Tag>()
            .HasMany(t => t.BookTags)
            .WithOne(bt => bt.Tag)
            .HasForeignKey(bt => bt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region BookTags

        modelBuilder.Entity<BookTag>()
            .HasKey(bt => new { bt.BookId, bt.TagId });

        modelBuilder.Entity<BookTag>()
            .HasOne(bt => bt.Book)
            .WithMany(b => b.BookTags)
            .HasForeignKey(bt => bt.BookId);

        modelBuilder.Entity<BookTag>()
            .HasOne(bt => bt.Tag)
            .WithMany(t => t.BookTags)
            .HasForeignKey(bt => bt.TagId);

        #endregion

        #region Genres

        modelBuilder.Entity<Genre>()
            .HasIndex(g => g.Name)
            .IsUnique();

        modelBuilder.Entity<Genre>()
            .HasMany(g => g.Books)
            .WithOne(b => b.Genre)
            .HasForeignKey(b => b.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion

        #region LibraryEntries

        modelBuilder.Entity<LibraryEntry>()
            .HasKey(le => new { le.UserId, le.BookId });

        modelBuilder.Entity<LibraryEntry>()
            .HasOne(le => le.User)
            .WithMany(u => u.Entries)
            .HasForeignKey(le => le.UserId);

        modelBuilder.Entity<LibraryEntry>()
            .HasOne(le => le.Book)
            .WithMany(b => b.LibraryEntries)
            .HasForeignKey(le => le.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion
    }

    public override int SaveChanges()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                switch (entry.Entity)
                {
                    case User user when entry.State == EntityState.Added:
                        user.JoiningDate = now;
                        break;

                    case Book book:
                        if (entry.State == EntityState.Added)
                            book.PublishedDate = now;

                        book.UpdatedDate = now;
                        break;

                    case Review review when entry.State == EntityState.Added:
                        review.CreatedAt = now;
                        break;

                    case Comment comment when entry.State == EntityState.Added:
                        comment.CreatedAt = now;
                        break;

                    case Notification notification when entry.State == EntityState.Added:
                        notification.CreatedAt = now;
                        break;

                    case Subscription subscription when entry.State == EntityState.Added:
                        subscription.SubscribedAt = now;
                        break;

                    case Chapter chapter when entry.State == EntityState.Added:
                        chapter.CreatedAt = now;
                        break;

                    case LibraryEntry libraryEntry when entry.State == EntityState.Added:
                        libraryEntry.DateAdded = now;
                        break;
                }
            }
        }

        return base.SaveChanges();
    }
}