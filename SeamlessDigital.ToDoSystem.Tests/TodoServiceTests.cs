using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SeamlessDigital.ToDoSystem.Services.Implementations;
using SeamlessDigital.ToDoSystem.Data;
using SeamlessDigital.ToDoSystem.Models;
using SeamlessDigital.ToDoSystem.ViewModels;
using SeamlessDigital.ToDoSystem.Services.Contracts;
namespace SeamlessDigital.ToDoSystem.Tests
{
    [TestFixture]
    public class TodoServiceTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IWeatherAPIRepo> _mockWeatherService;
        private TodoContext _context;
        private TodoService _todoService;

        [SetUp]
        public void Setup()
        {
            _mockWeatherService = new Mock<IWeatherAPIRepo>(); //  Mock the interface
            _mockConfiguration = new Mock<IConfiguration>();
            HttpClient _httpClient = new HttpClient();

            // Mock GetWeatherAsync so it doesn't call real API
            _mockWeatherService
                .Setup(service => service.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(new WeatherData
                {
                    Temperature = 25,
                    Condition = "Sunny"
                });

            // Use InMemory Database
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new TodoContext(options);
            _todoService = new TodoService(_context, _mockConfiguration.Object, _httpClient, _mockWeatherService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted(); //  Clears DB after each test
            _context.Dispose();
        }

        // Test for GetAllTasksAsync method
        [Test]
        public async Task GetAllTasksAsync_ReturnsTasks()
        {
            // Arrange
            var task = new Todotask
            {
                Id = 1,
                Title = "Test Task",
                Completed = false,
                UserId = 1,
                Priority = 3,
                Latitude = 12.34,
                Longitude = 56.78,
                DueDate = DateTime.Now.AddDays(1)
            };
            _context.todotasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            var result = await _todoService.GetAllTasksAsync();

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Test Task", result.First().Title);
        }

        // Test for GetTaskByIdAsync method
        [Test]
        public async Task GetTaskByIdAsync_ReturnsCorrectTask()
        {
            // Arrange
            var task = new Todotask
            {
                Id = 1,
                Title = "Test Task",
                Completed = false,
                UserId = 1,
                Priority = 3,
                Latitude = 12.34,
                Longitude = 56.78,
                DueDate = DateTime.Now.AddDays(1)
            };
            _context.todotasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            var result = await _todoService.GetTaskByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Task", result.Title);
        }

        // Test for AddTaskAsync method
        [Test]
        public async Task AddTaskAsync_AddsNewTask()
        {
            // Arrange
            var newTask = new CreateTaskViewModel
            {
                Title = "New Task",
                Completed = false,
                UserId = 1,
                Priority = 3,
                Latitude = 12.34,
                Longitude = 56.78,
                DueDate = DateTime.Now.AddDays(1)
            };

            // Act
            var result = await _todoService.AddTaskAsync(newTask);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("New Task", result.Title);
            Assert.IsTrue(result.Id > 0); // Ensure that the task ID is set
        }

        // Test for UpdateTaskAsync method
        [Test]
        public async Task UpdateTaskAsync_UpdatesExistingTask()
        {
            // Arrange
            var existingTask = new Todotask
            {
                Id = 1,
                Title = "Existing Task",
                Completed = false,
                UserId = 1,
                Priority = 3,
                Latitude = 12.34,
                Longitude = 56.78,
                DueDate = DateTime.Now.AddDays(1)
            };
            _context.todotasks.Add(existingTask);
            await _context.SaveChangesAsync();

            var updatedTask = new CreateTaskViewModel
            {
                Title = "Updated Task",
                Completed = true,
                UserId = 1,
                Priority = 2,
                Latitude = 12.34,
                Longitude = 56.78,
                DueDate = DateTime.Now.AddDays(2)
            };

            // Act
            var result = await _todoService.UpdateTaskAsync(1, updatedTask);

            // Assert
            Assert.IsTrue(result);
            var taskInDb = await _context.todotasks.FindAsync(1);
            Assert.AreEqual("Updated Task", taskInDb?.Title);
        }

        // Test for DeleteTaskAsync method
        [Test]
        public async Task DeleteTaskAsync_DeletesExistingTask()
        {
            // Arrange
            var task = new Todotask
            {
                Id = 1,
                Title = "Task to Delete",
                Completed = false,
                UserId = 1,
                Priority = 3,
                Latitude = 12.34,
                Longitude = 56.78,
                DueDate = DateTime.Now.AddDays(1)
            };
            _context.todotasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            var result = await _todoService.DeleteTaskAsync(1);

            // Assert
            Assert.IsTrue(result);
            var taskInDb = await _context.todotasks.FindAsync(1);
            Assert.IsNull(taskInDb);
        }
    }
}