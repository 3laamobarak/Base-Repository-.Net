using Company.Project.Domain.Models;

namespace Company.Project.Domain.Interfaces
{
    public interface IExampleClassRepository : IBaseRepository<ExampleClass>
    {
        Task<IEnumerable<ExampleClass>> GetAllAsync(int skip, int take);
    }
}
