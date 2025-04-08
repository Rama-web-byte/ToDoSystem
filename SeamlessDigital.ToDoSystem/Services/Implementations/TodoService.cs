using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SeamlessDigital.ToDoSystem.Data;
using SeamlessDigital.ToDoSystem.Models;
using SeamlessDigital.ToDoSystem.ViewModels;
using SeamlessDigital.ToDoSystem.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeamlessDigital.ToDoSystem.Services.Implementations
{
    public class TodoService : ITodoRepo
    {
        private readonly TodoContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IWeatherAPIRepo _weatherAPIService;
        public TodoService(TodoContext context, IConfiguration configuration, HttpClient httpClient, IWeatherAPIRepo weatherAPIService)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClient;
            _weatherAPIService = weatherAPIService;
        }

        public async Task<IEnumerable<TaskItemViewModel>> GetAllTasksAsync()
        {
            return await GetTaskQuery();
        }

        public async Task<TaskItemViewModel> GetTaskByIdAsync(int id)
        {
           var taskItem = await _context.todotasks.Include(c => c.Category).FirstOrDefaultAsync(c => c.Id == id);
            if (taskItem != null)
            {
                var taskViewModel = new TaskItemViewModel
                {
                    Id = taskItem.Id,
                    Title = taskItem.Title,
                    Completed = taskItem.Completed,
                    UserId = taskItem.UserId,
                    Priority = taskItem.Priority,
                    Latitude = taskItem.Latitude,
                    Longitude = taskItem.Longitude,
                    DueDate = taskItem.DueDate,
                    CategoryId = taskItem.CategoryId,
                    Category = taskItem.Category, // or any other property from Category
                    Weather = new WeatherDto()
                };
                if (taskItem.Latitude.HasValue && taskItem.Longitude.HasValue)
                {
                    var weatherData = await _weatherAPIService.GetWeatherAsync(taskItem.Latitude.Value, taskItem.Longitude.Value);
                    if (weatherData != null)
                    {
                        // Assign the weather data to the ViewModel
                        taskViewModel.Weather.Temperature= weatherData.Temperature;
                        taskViewModel.Weather.Condition = weatherData.Condition;
                    }
                }

                return taskViewModel;
            }
            else
            {
                return null;
            }
           
        }

        public async Task<IEnumerable<TaskItemViewModel>> SearchTasksAsync(string? title, int? priority, DateTime? dueDate)
        {
            return await GetTaskQuery(title, priority, dueDate);
        }
        public async Task<CreateTaskViewModel> AddTaskAsync(CreateTaskViewModel todoItem)
        {
            if (todoItem.CategoryId.HasValue)
            {
                var category = await _context.categories
               .Include(c => c.ParentCategory) // Ensure ParentCategory is loaded
               .FirstOrDefaultAsync(c => c.Id == todoItem.CategoryId.Value);


                // Assign the retrieved category to the todoItem
                if (category == null)
                {
                    throw new ArgumentNullException("Invalid Category ID.");
                }
            }
            var todoTask = new Todotask
            {

                Title = todoItem.Title,
                Completed = todoItem.Completed,
                UserId = todoItem.UserId,
                CategoryId = todoItem.CategoryId, // Only set the ID, not full Category
                Priority = todoItem.Priority,
                Latitude = todoItem.Latitude,
                Longitude = todoItem.Longitude,
                DueDate = todoItem.DueDate
            };

            _context.todotasks.Add(todoTask);
            await _context.SaveChangesAsync();
            todoItem.Id=todoTask.Id;
            return todoItem;
        }

        public async Task<bool> UpdateTaskAsync(int id, CreateTaskViewModel updatetask)
        {
            var existingToDo = await _context.todotasks.FindAsync(id);
            if (existingToDo == null) return false;

            existingToDo.Title = updatetask.Title;
            existingToDo.Completed=updatetask.Completed;
            existingToDo.UserId= updatetask.UserId;
            existingToDo.Priority = updatetask.Priority;
            existingToDo.DueDate = updatetask.DueDate;
            existingToDo.CategoryId = updatetask.CategoryId;
            existingToDo.Latitude = updatetask.Latitude;
            existingToDo.Longitude = updatetask.Longitude;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var todoItem = await _context.todotasks.FindAsync(id);
            if (todoItem == null) return false;

            _context.todotasks.Remove(todoItem);
            await _context.SaveChangesAsync();
            return true;
        }
        private async Task<IEnumerable<TaskItemViewModel>> GetTaskQuery(string? title = null, int? priority = null, DateTime? dueDate = null)
        {
            var query = _context.todotasks.Include(c => c.Category).AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(t => EF.Functions.Like(t.Title.ToLower(), $"%{title.ToLower()}%"));

            if (priority.HasValue)
                query = query.Where(t => t.Priority == priority.Value);

            if (dueDate.HasValue)
                query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == dueDate.Value.Date);

            var taskList = await query.ToListAsync(); // Execute the DB query first

            // Create a list of asynchronous weather fetch tasks
            var weatherTasks = taskList
                .Where(task => task.Latitude.HasValue && task.Longitude.HasValue && task.Latitude.Value != 0.0 && task.Longitude.Value != 0.0)
                .Select(async item =>
                {
                    var weatherData = await _weatherAPIService.GetWeatherAsync(item.Latitude.Value, item.Longitude.Value);

                    return new TaskItemViewModel
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Completed = item.Completed,
                        UserId = item.UserId,
                        CategoryId = item.CategoryId,
                        Priority = item.Priority,
                        Latitude = item.Latitude,
                        Longitude = item.Longitude,
                        DueDate = item.DueDate,
                        Category = item.Category,
                        Weather = weatherData != null
                            ? new WeatherDto { Temperature = weatherData.Temperature, Condition = weatherData.Condition }
                            : null
                    };
                });

            // Await all weather tasks to fetch data in parallel
            var tasksWithWeatherData = await Task.WhenAll(weatherTasks);

            // Convert remaining tasks (without weather data) to ViewModel
            var tasksWithoutWeather = taskList
                .Where(task => !task.Latitude.HasValue || !task.Longitude.HasValue || task.Latitude.Value == 0.0 || task.Longitude.Value == 0.0)
                .Select(task => new TaskItemViewModel
                {
                    Id = task.Id,
                    Title = task.Title,
                    Completed = task.Completed,
                    UserId = task.UserId,
                    CategoryId = task.CategoryId,
                    Priority = task.Priority,
                    Latitude = task.Latitude,
                    Longitude = task.Longitude,
                    DueDate = task.DueDate,
                    Category = task.Category,
                    Weather = null
                });

            // Combine both lists
            return tasksWithWeatherData.Concat(tasksWithoutWeather);
           


         
        }

    }

    

    }
