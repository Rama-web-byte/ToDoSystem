using System.Collections.Generic;
using System.Threading.Tasks;
using SeamlessDigital.ToDoSystem.Models;
using SeamlessDigital.ToDoSystem.ViewModels;

namespace SeamlessDigital.ToDoSystem.Services.Contracts
{
    public interface ITodoRepo
    {
        Task<IEnumerable<TaskItemViewModel>> GetAllTasksAsync();

        Task<TaskItemViewModel> GetTaskByIdAsync(int id);

        Task<CreateTaskViewModel> AddTaskAsync(CreateTaskViewModel newtask);

        Task<bool> UpdateTaskAsync(int id, CreateTaskViewModel updatetask);

        Task<bool> DeleteTaskAsync(int id);

        Task<IEnumerable<TaskItemViewModel>> SearchTasksAsync(string? title, int? priority, DateTime? dueDate);

    }
}
