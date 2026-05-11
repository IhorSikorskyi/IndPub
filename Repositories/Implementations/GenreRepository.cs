using IndPubBack.Models;
using IndPubBack.Repositories.Interfaces;

namespace IndPubBack.Repositories.Implementations;

public class GenreRepository(Connected dbContext) : Repository<Genre>(dbContext), IGenreRepository
{

}