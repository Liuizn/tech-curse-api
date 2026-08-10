using Microsoft.EntityFrameworkCore;
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Domain.Entities;
using tech_curse_api.src.Infrastructure.Data;

namespace tech_curse_api.src.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly TechCurseContext _context;

    public PaymentRepository(TechCurseContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(int id)
    {
        return await _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PaymentId == id);
    }

    public async Task<(IEnumerable<Payment> Items, int TotalCount)> GetByStudentIdAsync(int studentId, PaginationParamsDto searchParams)
    {
        var query = _context.Payments
            .AsQueryable()
            .AsNoTracking()
            .Where(p => p.StudentId == studentId);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((searchParams.PageNumber - 1) * searchParams.PageSize)
            .Take(searchParams.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<Payment>> GetByEnrollmentIdAsync(int enrollmentId)
    {
        var query = _context.Payments
            .AsQueryable()
            .AsNoTracking()
            .Where(p => p.EnrollmentId == enrollmentId);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return items;
    }

    public async Task<Payment?> GetActiveByEnrollmentAsync(int enrollmentId)
    {
        throw new NotImplementedException();
    }

    public async Task<(IEnumerable<Payment> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto searchParams)
    {
        var query = _context.Payments.AsQueryable().AsNoTracking();

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((searchParams.PageNumber - 1) * searchParams.PageSize)
            .Take(searchParams.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(Payment payment)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Payment payment)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Payment payment)
    {
        throw new NotImplementedException();
    }
}
