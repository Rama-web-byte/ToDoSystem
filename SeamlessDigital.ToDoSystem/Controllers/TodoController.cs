using Serilog;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SeamlessDigital.ToDoSystem.Services.Contracts;
using SeamlessDigital.ToDoSystem.ViewModels;

namespace SeamlessDigital.ToDoSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ToDoController : ControllerBase
    {
        #region Fields
        private readonly ITodoRepo _repository;
        private readonly ILogger<ToDoController> _logger;
        #endregion

        #region Ctor
        public ToDoController(ITodoRepo repository, ILogger<ToDoController> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get all ToDo items from the repository.
        /// </summary>
        /// <returns>A list of ToDo items.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemViewModel>>> GetAllToDos()
        {
            try
            {
                var todos = await _repository.GetAllTasksAsync();

                if (todos == null || !todos.Any())
                {
                    _logger.LogWarning("No To-Do items found.");
                    return NotFound("No To-Do items found.");
                }

                _logger.LogInformation("Successfully fetched all To-Do items.");
                return Ok(todos);
            }
            catch (ArgumentNullException argEx)
            {
                _logger.LogError(argEx, "ArgumentNullException occurred while fetching all To-Do items.");
                return StatusCode(500, "An unexpected LogError occurred. Please try again later.");
            }
            catch (InvalidOperationException invOpEx)
            {
                _logger.LogError(invOpEx, "InvalidOperationException occurred while fetching all To-Do items.");
                return StatusCode(500, "An unexpected LogError occurred. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "A Fatal Error occurred while fetching all To-Do items.");
                return StatusCode(500, "An unexpected LogError occurred. Please try again later.");
            }
        }


        /// <summary>
        /// Get a specific ToDo item by its ID.
        /// </summary>
        /// <param name="id">The ID of the ToDo item.</param>
        /// <returns>The requested ToDo item.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemViewModel>> GetToDoById(int id)
        {
            try
            {
                var todoItem = await _repository.GetTaskByIdAsync(id);

                if (todoItem == null)
                {
                    _logger.LogWarning($"To-Do item with ID {id} not found.");
                    return NotFound($"To-Do item with ID {id} not found.");
                }

                _logger.LogInformation($"Successfully fetched To-Do item with ID {id}.");
                return Ok(todoItem);
            }
            catch (ArgumentNullException argEx)
            {
                _logger.LogError(argEx, "ArgumentNullException occurred while fetching To-Do item by ID.");
                return StatusCode(500, "An unexpected LogError occurred. Please try again later.");
            }
            catch (InvalidOperationException invOpEx)
            {
                _logger.LogError(invOpEx, "InvalidOperationException occurred while fetching To-Do item by ID.");
                return StatusCode(500, "An unexpected LogError occurred. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "A Fatal Error occurred while fetching To-Do item by ID.");
                return StatusCode(500, "An unexpected LogError occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Add a new ToDo item to the repository.
        /// </summary>
        /// <param name="todoItem">The new ToDo item to be added.</param>
        /// <returns>The created ToDo item.</returns>
        [HttpPost]
        public async Task<ActionResult<CreateTaskViewModel>> AddToDoItem([FromBody] CreateTaskViewModel todoItem)
        {
            try
            {
                if (todoItem == null)
                {
                    _logger.LogWarning("Received invalid data for new To-Do item.");
                    return BadRequest("Invalid data.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdTodo = await _repository.AddTaskAsync(todoItem);
                _logger.LogInformation($"Successfully added new To-Do item with ID {createdTodo.Id}.");

                return CreatedAtAction(nameof(GetToDoById), new { id = createdTodo.Id }, createdTodo);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "ArgumentException occurred while adding new To-Do item.");
                return StatusCode(500, "An unexpected LogError occurred. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "A Fatal Error occurred while adding a new To-Do item.");
                return StatusCode(500, "An unexpected LogError occurred. Please try again later.");
            }
        }
        /// <summary>
        /// Update an existing ToDo item by ID.
        /// </summary>
        /// <param name="id">The ID of the ToDo item to be updated.</param>
        /// <param name="todoItem">The updated ToDo item data.</param>
        /// <returns>No content if the update was successful, otherwise NotFound.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateToDoItem(int id, [FromBody] CreateTaskViewModel todoItem)
        {
            try
            {
                var result = await _repository.UpdateTaskAsync(id, todoItem);
                if (!result)
                {
                    _logger.LogWarning($"To-Do item with ID {id} not found for update.");
                    return NotFound($"To-Do item with ID {id} not found.");
                }

                _logger.LogInformation($"Successfully updated To-Do item with ID {id}.");
                return NoContent();
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "ArgumentException occurred while updating To-Do item.");
                return StatusCode(500, "An unexpected LogError occurred. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "A Fatal Error occurred while updating To-Do item.");
                return StatusCode(500, "An unexpected LogError occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Delete an existing ToDo item by ID.
        /// </summary>
        /// <param name="id">The ID of the ToDo item to be deleted.</param>
        /// <returns>No content if the deletion was successful, otherwise NotFound.</returns>

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDoItem(int id)
        {
            try
            {
                var result = await _repository.DeleteTaskAsync(id);
                if (!result)
                {
                    _logger.LogWarning($"To-Do item with ID {id} not found for deletion.");
                    return NotFound($"To-Do item with ID {id} not found.");
                }

                _logger.LogInformation($"Successfully deleted To-Do item with ID {id}.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "A Fatal Error occurred while deleting To-Do item.");
                return StatusCode(500, "An unexpected LogError occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Search for ToDo items based on optional query parameters.
        /// </summary>
        /// <param name="title">Optional title filter.</param>
        /// <param name="priority">Optional priority filter.</param>
        /// <param name="dueDate">Optional due date filter.</param>
        /// <returns>A list of matching ToDo items.</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TaskItemViewModel>>> SearchToDos([FromQuery] string? title, [FromQuery] int? priority, [FromQuery] DateTime? dueDate)
        {
            try
            {
                var todos = await _repository.SearchTasksAsync(title, priority, dueDate);

                if (todos == null || !todos.Any())
                {
                    _logger.LogWarning("No matching To-Do items found for the search criteria.");
                    return NotFound("No matching To-Do items found.");
                }

                _logger.LogInformation("Successfully fetched To-Do items based on search criteria.");
                return Ok(todos);
            }
            catch (ArgumentNullException argEx)
            {
                _logger.LogError(argEx, "ArgumentNullException occurred during search for To-Do items.");
                return StatusCode(500, "An unexpected LogError occurred. Please try again later.");
            }
            catch (InvalidOperationException invOpEx)
            {
                _logger.LogError(invOpEx, "InvalidOperationException occurred during search for To-Do items.");
                return StatusCode(500, "An unexpected LogError occurred. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "A Fatal Error occurred during search for To-Do items.");
                return StatusCode(500, "An unexpected LogError occurred. Please try again later.");
            }


        }

        #endregion
    }
}