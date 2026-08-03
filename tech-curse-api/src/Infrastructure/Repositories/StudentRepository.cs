using Microsoft.EntityFrameworkCore;
using tech_curse_api.src.Application.DTOs;
using tech_curse_api.src.Application.Interfaces;
using tech_curse_api.src.Domain.Entities;
using tech_curse_api.src.Infrastructure.Data;

namespace tech_curse_api.src.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly TechCurseContext _context;

    public StudentRepository(TechCurseContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Student> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto searchParams)
    {
        var query = _context.Students.AsQueryable().AsNoTracking();

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((searchParams.PageNumber - 1) * searchParams.PageSize)
            .Take(searchParams.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
        => await _context.Students.ToListAsync();

    public async Task<Student?> GetByIdAsync(int id)
        => await _context.Students.FindAsync(id);

    public async Task<Student?> GetByEmailAsync(string email)
        => await _context.Students.FirstOrDefaultAsync(s => s.Email == email);

    public async Task AddAsync(Student student)
    {
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Student student)
    {
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Students.AnyAsync(s => s.Email == email);
    }
}