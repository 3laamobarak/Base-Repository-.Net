using Company.Project.Application.DTO;
using Company.Project.Domain.Models;

namespace Company.Project.Application.Contracts
{
    public interface IExampleClassService 
    {
        Task<IEnumerable<ExampleClass>> GetAllAsync(int skip, int take);
        Task<ExampleClass> GetByIdAsync(int id);
        Task<ExampleClass> CreateAsync(CreateExampleClassDto createDto);
        Task<ExampleClass> UpdateAsync(UpdateExampleClassDto updateDto);
        Task DeleteAsync(int id);
    }
}
