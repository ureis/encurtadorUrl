using EncurtadorURL.Domain.Entities;

namespace EncurtadorURL.Domain.Interfaces
{
    public interface IUrlRepository
    {
        Task<UrlRecord?> GetByShortCodeAsync(string shortCode);
        Task<UrlRecord?> GetByAliasAsync(string alias);
        Task<IReadOnlyList<UrlRecord>> GetAllAsync();
        Task AddAsync(UrlRecord urlRecord);
        Task SaveChangesAsync();
    }
}
