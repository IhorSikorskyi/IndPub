using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;

namespace IndPubBack.Data;

public class IndPubDbContext(DbContextOptions<IndPubDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();
    public DbSet<BookTag> BookTags => Set<BookTag>();
    public DbSet<BookLike> BookLikes => Set<BookLike>();
    public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Chapter> Chapters => Set<Chapter>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<LibraryEntry> Libraries => Set<LibraryEntry>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<CommentLike> CommentLikes => Set<CommentLike>();
    public DbSet<ReviewLike> ReviewLikes => Set<ReviewLike>();
    public DbSet<BookView> BookViews => Set<BookView>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Subcategory> Subcategories => Set<Subcategory>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IndPubDbContext).Assembly);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditInfo();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyAuditInfo()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added && entry.Entity is BaseEntity baseEntity)
            {
                baseEntity.CreatedAt = now;
            }

            if (entry.State != EntityState.Added && entry.State != EntityState.Modified)
            {
                continue;
            }

            switch (entry.Entity)
            {
                case Book book:
                    book.UpdatedDate = now;
                    break;

                case Subscription subscription when entry.State == EntityState.Added:
                    subscription.SubscribedAt = now;
                    break;

                case LibraryEntry libraryEntry when entry.State == EntityState.Added:
                    libraryEntry.DateAdded = now;
                    break;

                case BookView bookView:
                    bookView.ViewedAt = now;
                    break;

                case BookLike bookLike when entry.State == EntityState.Added:
                    bookLike.LikedAt = now;
                    break;

                case ReviewLike reviewLike when entry.State == EntityState.Added:
                    reviewLike.LikedAt = now;
                    break;
            }
        }
    }
}