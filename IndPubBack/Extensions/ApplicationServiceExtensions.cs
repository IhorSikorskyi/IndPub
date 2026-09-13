using IndPubBack.Entities;
using IndPubBack.Services.Interfaces;
using IndPubBack.Services.Implementations;
using Microsoft.AspNetCore.Identity;

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
        return services;
    }
}