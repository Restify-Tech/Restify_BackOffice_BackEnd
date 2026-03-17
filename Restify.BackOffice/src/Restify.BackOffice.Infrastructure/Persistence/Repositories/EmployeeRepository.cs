using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly BackOfficeDbContext _context;

    public EmployeeRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Where(e => e.IsActive && e.Status == EmployeeStatus.Active)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetByIdentificationAsync(string identificationNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.IdentificationNumber == identificationNumber, cancellationToken);
    }

    public async Task<Employee> AddAsync(Employee entity, CancellationToken cancellationToken = default)
    {
        await _context.Employees.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Employee entity, CancellationToken cancellationToken = default)
    {
        _context.Employees.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Employee entity, CancellationToken cancellationToken = default)
    {
        _context.Employees.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
