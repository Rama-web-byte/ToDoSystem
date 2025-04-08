
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SeamlessDigital.ToDoSystem.Data;
using SeamlessDigital.ToDoSystem.Models;
using SeamlessDigital.ToDoSystem.Services.Contracts;

namespace SeamlessDigital.ToDoSystem.Services.Implementations
{
    public class DummyJsonAPIService:IDummyJsonAPIRepo
    {
        private readonly HttpClient client;
        private readonly IServiceProvider provider;
        private string url = "https://dummyjson.com/todos";

        public DummyJsonAPIService(HttpClient _client, IServiceProvider serviceProvider)
        {
            client=_client;
            provider = serviceProvider;
        }

        public async Task FetchjsonTodosAsync()
        {
            using var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
            if (await context.todotasks.AnyAsync()) return;
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                var root = JsonConvert.DeserializeObject<Root>(await response.Content.ReadAsStringAsync());
                if (root?.Todos != null)
                {
                    var todoEntities = root.Todos.Select(x => new Todotask
                    {
                        Id = x.Id,
                        Title = x.Todo,
                        Completed = x.Completed,
                        UserId = x.UserId,
                        Priority = 3, // Default priority
                        DueDate = null, // No due date provided
                        CategoryId = null,// No category assigned from API
                        Latitude = null, // Location not provided in API
                        Longitude = null

                    }).ToList();

                    context.todotasks.AddRange(todoEntities);
                    await context.SaveChangesAsync();
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}");
            }
            catch (System.Text.Json.JsonException ex)
            {
                Console.WriteLine($"JSON Parsing Error: {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database Error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Dependency Injection Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching To-Do items: {ex.Message}");
            }

        }
    }
}
