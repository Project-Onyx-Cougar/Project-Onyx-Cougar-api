using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;

namespace Onyx.Cougar.Api.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("[controller]")]
    public class CatalogController : ControllerBase
    {
        private static List<Item> Items = new List<Item>
        {
            new Item { Id = 1, Name = "Shirt", Description = "Ohio State shirt", Brand = "Nike", Price = 29.99M },
            new Item { Id = 2, Name = "Shorts", Description = "Ohio State shorts", Brand = "Nike", Price = 44.99M },
            new Item { Id = 3, Name = "Shoes", Description = "Running Shoes", Brand = "Nike", Price = 109.99M }
        };

        [HttpGet]
        public IActionResult GetItems()
        {
            return Ok(Items);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetItem(int id)
        {
            var item = Items.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost]
        public IActionResult Post(Item item)
        {
            item.Id = Items.Count > 0 ? Items.Max(i => i.Id) + 1 : 1;
            Items.Add(item);
            return Created($"/catalog/{item.Id}", item);
        }

        [HttpPost("{id:int}/ratings")]
        public IActionResult PostRating(int id, [FromBody] Rating rating)
        {
            var item = Items.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            item.Ratings.Add(rating);
            return Ok(item);
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] Item item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            var existingItem = Items.FirstOrDefault(i => i.Id == id);
            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Name = item.Name;
            existingItem.Description = item.Description;
            existingItem.Brand = item.Brand;
            existingItem.Price = item.Price;

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "delete:catalog")]
        public IActionResult Delete(int id)
        {
            var item = Items.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            Items.Remove(item);
            return NoContent();
        }

        public class Item
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Brand { get; set; }
            public decimal Price { get; set; }
            public List<Rating> Ratings { get; set; } = new List<Rating>();
        }

        public class Rating
        {
            public int Stars { get; set; }
            public string Comment { get; set; }
        }
    }
}
