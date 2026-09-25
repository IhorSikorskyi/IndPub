using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.Repositories.Interfaces;

namespace IndPubBack.Infrastructure.Implementations;

public class EntityValidationService(
    IUserRepository userRepository,
    IBookRepository bookRepository,
    IReviewRepository reviewRepository) : IEntityValidationService
{
    public async Task<bool> IsUserExistsAsync(Guid userId)
    {
        if (!await userRepository.IsExistAsync(userId))
        {
            return false;
        }

        return true;
    }

    public async Task<bool> IsBookExistsAsync(Guid bookId)
    {
        if (!await bookRepository.IsExistAsync(bookId))
        {
            return false;
        }

        return true;
    }

    public async Task<bool> IsReviewExistsAsync(Guid reviewId)
    {
        if (!await reviewRepository.IsExistAsync(reviewId))
        {
            return false;
        }

        return true;
    }
}