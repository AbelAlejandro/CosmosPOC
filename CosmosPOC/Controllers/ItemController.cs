using CosmosPOC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosPOC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;

        public ItemController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;

        }

        [HttpGet]
        public async Task<ActionResult> GetAllAsync(bool change = false, int? limit = 50, string? continuation_token = "")
        {
            try
            {
                (IEnumerable<Item> items, string continuation, int contLength) = await _cosmosDbService.GetItems(change, continuation_token, limit.Value);
                return Ok(new ItemDTO()
                {
                    Size = continuation.Length,
                    SizeEncoded = continuation.Length,
                    ContinuationToken = continuation,
                    Items = items
                });
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetAsync(string id)
        {
            try
            {
                var result = await _cosmosDbService.GetItemAsync(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failed");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody]Item item)
        {
            try
            {
                for (int i = 0; i < 200; i++)
                {
                    item.Id = Guid.NewGuid().ToString();
                    await _cosmosDbService.AddItemAsync(item);
                }
                return Created($"/api/{item.Name}", item);
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}