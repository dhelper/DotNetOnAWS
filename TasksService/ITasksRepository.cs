using TasksService.Controllers;

namespace TasksService;

public interface ITasksRepository
{
    Task<IEnumerable<TodoWithId>> GetAllTasks();
    Task<Todo> GetTaskById(string id);
    Task<string> CreateNewTask(Todo value);
}