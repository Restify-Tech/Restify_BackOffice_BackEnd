using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly BackOfficeDbContext _context;

    public CustomerRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(c => c.Orders)
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer> AddAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        await _context.Customers.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        _context.Customers.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        _context.Customers.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AnyAsync(c => c.Email == email && (excludeId == null || c.Id != excludeId), cancellationToken);
    }
}
