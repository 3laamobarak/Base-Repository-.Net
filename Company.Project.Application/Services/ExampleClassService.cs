using Company.Project.Application.Contracts;
using Company.Project.Domain.Interfaces;
using Company.Project.Domain.Models;
using Company.Project.Application.DTO;

namespace Company.Project.Application.Services
{
    public class ExampleClassService : IExampleClassService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExampleClassService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<ExampleClass>> GetAllAsync(int skip, int take)
        {
            // return await _unitOfWork.ExampleClassRepository.GetAllAsync(skip, take);
            var exampleClasses = new List<ExampleClass>
            {
                new ExampleClass { Id = 1, Name = "Example 1" },
                new ExampleClass { Id = 2, Name = "Example 2" }
            };
            return exampleClasses;
            
        }
        public async Task<ExampleClass> GetByIdAsync(int id)
        {
            // return await _unitOfWork.ExampleClassRepository.GetByIdAsync(id);
            var exampleClass = new ExampleClass
            {
                Id = id,
                Name = "Example Name"
            };
            return exampleClass;
        }
        
        public async Task<ExampleClass> CreateAsync (CreateExampleClassDto createDto)
        {
            var exampleClass = new ExampleClass
            {
                Name = createDto.Name
            };
            // await _unitOfWork.ExampleClassRepository.AddAsync(exampleClass);
            await _unitOfWork.Completeasync();

            return exampleClass;
        }

        public async Task<ExampleClass> UpdateAsync(UpdateExampleClassDto updateDto)
        {
            // var existingItem = await _unitOfWork.ExampleClassRepository.GetByIdAsync(updateDto.Id);
            // if (existingItem == null)
            {
                throw new KeyNotFoundException("Item not found.");
            }

            // existingItem.Name = updateDto.Name;
            // await _unitOfWork.ExampleClassRepository.UpdateAsync(existingItem);
            await _unitOfWork.Completeasync();

            // return existingItem;
        }
        public async Task DeleteAsync(int id)
        {
            // var existingItem = await _unitOfWork.ExampleClassRepository.GetByIdAsync(id);
            // if (existingItem == null)
            {
                throw new KeyNotFoundException("Item not found.");
            }

            // await _unitOfWork.ExampleClassRepository.DeleteAsync(existingItem);
            // await _unitOfWork.Completeasync();
        }
    }
}
