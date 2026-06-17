using EncurtadorURL.Domain.Entities;
using EncurtadorURL.Domain.Interfaces;
using EncurtadorURL.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncurtadorURL.Infrastructure.Repositories
{
    public class UrlRepository : IUrlRepository
    {
        private readonly AppDbContext _context;

        public UrlRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UrlRecord?> GetByShortCodeAsync(string shortCode)
        {
            return await _context.Urls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
        }

        public async Task<UrlRecord?> GetByAliasAsync(string alias)
        {
            return await _context.Urls.FirstOrDefaultAsync(u => u.ShortCode == alias);
        }

        public async Task<IReadOnlyList<UrlRecord>> GetAllAsync()
        {
            return await _context.Urls
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(UrlRecord urlRecord)
        {
            await _context.Urls.AddAsync(urlRecord);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
