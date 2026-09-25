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
        services.AddScoped<IBlobService, BlobService>();
        services.AddScoped<IEntityValidationService, EntityValidationService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IPasswordValidationService, PasswordValidationService>();
        services.AddScoped<IAccessValidationService, AccessValidationService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IReviewInteractionService, ReviewInteractionService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<ITokenRevocationStore, TokenRevocationStore>();

        return services;
    }
}