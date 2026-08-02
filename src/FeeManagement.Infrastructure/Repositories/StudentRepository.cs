using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeeManagement.Domain.Entities;
using FeeManagement.Domain.Interfaces;
using FeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FeeManagement.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly FeeDbContext _context;

    public StudentRepository(FeeDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(int studentId)
    {
        return await _context.Students.FindAsync(studentId);
    }

    public async Task<IReadOnlyList<Student>> GetOverdueStudentsAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Students
            .Where(s => s.PaidAmount < s.TotalFee && now > s.DueDate)
            .ToListAsync();
    }

    public async Task UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
    }
}
