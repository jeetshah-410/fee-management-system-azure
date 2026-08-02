using System.Collections.Generic;
using System.Threading.Tasks;
using FeeManagement.Domain.Entities;

namespace FeeManagement.Domain.Interfaces;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int studentId);
    Task<IReadOnlyList<Student>> GetOverdueStudentsAsync();
    Task UpdateAsync(Student student);
}
