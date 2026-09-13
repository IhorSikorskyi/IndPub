using IndPubBack.Data;
using IndPubBack.Entities;
using IndPubBack.Repositories.Interfaces;

namespace IndPubBack.Repositories.Implementations;

public class GenreRepository(IndPubDbContext dbContext) : Repository<Genre>(dbContext), IGenreRepository
{

}