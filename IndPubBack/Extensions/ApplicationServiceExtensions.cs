using IndPubBack.Infrastructure.Implementations;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Services.Implementations;
using IndPubBack.Services.Interfaces;

namespace IndPubBack.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<ILibraryService, LibraryService>();
        services.AddScoped<IBookInteractionService, BookInteractionService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<IChapterService, ChapterService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<INotificationService, NotificationService>();

        // TODO: Review this services and consider to delete them if they are not best practices to use in the project or maybe replace them with better alternatives
        // DI Container registrations for infrastructure services
        services.AddScoped<IBlobService, BlobService>();
        services.AddScoped<IEntityValidationService, EntityValidationService>();
        services.AddScoped<IImageValidationService, ImageValidationService>();
        services.AddScoped<IPasswordValidationService, PasswordValidationService>();
        services.AddScoped<IAccessValidationService, AccessValidationService>();

        return services;
    }
}