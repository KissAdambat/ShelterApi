using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShelterApi.Models;
using ShelterApi.Models.DTOs;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace ShelterApi.Controllers
{
    [Route("/animals")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private readonly AnimalshelterContext _context;

        public AnimalsController(AnimalshelterContext context)
        {
            _context = context;
        }
        [HttpPost("NewAnimal")]
        public async Task<ActionResult<Animal>> AddNewAnimal([FromBody] NewAnimalDTO newAnimal)
        {
            try
            {
                var animal = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = newAnimal.Name,
                    Species = newAnimal.Species,
                    Age = newAnimal.Age,
                    Weight = newAnimal.Weight,
                    RegTime = DateTime.Now,
                    ShelterId = newAnimal.ShelterId
                };
                if (animal is not null)
                {
                    await _context.Animals.AddAsync(animal);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, new { message = "Felvéve", result = animal });
                }
                return StatusCode(400, new { message = "Hiba", result = animal });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Belső hiba", result = e.Message });
            }
        }

        [HttpGet("AllAnimals")]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAllAnimals()
        {
            try
            {
                var animals = await _context.Animals.ToListAsync();
                if (_context.Animals == null)
                {
                    return StatusCode(200, new { message = "Nincs egy állat sem" });
                }
                else
                {
                    return StatusCode(200, new { message = "Sikeres lekérés", result = animals });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Belső hiba", result = e.Message });
            }
        }

        [HttpGet("ById/{id}")]
        public async Task<ActionResult<Animal>> GetAnimalById([FromRoute]Guid id)
        {
            try
            {
                var animal = await _context.Animals.FindAsync(id);
                if (animal == null)
                {
                    return StatusCode(404, new { message = "Nincs ilyen állat" });
                }
                else
                {
                    return StatusCode(200, new { message = "Sikeres lekérés", result = animal });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Belső hiba", result = e.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAnimal([FromQuery] Guid id, UpdateAnimalDTO updateAnimalDTO)
        {
            try
            {
                var animal = await _context.Animals.FindAsync(id);
                if (animal is not null)
                {
                    animal.Name = updateAnimalDTO.Name;
                    animal.Species = updateAnimalDTO.Species;
                    animal.Age = updateAnimalDTO.Age;
                    animal.Weight = updateAnimalDTO.Weight;
                    animal.ShelterId = updateAnimalDTO.ShelterId;
                    _context.Animals.Update(animal);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, new { message = "Frissítve", result = animal });
                }
                return StatusCode(404, new { message = "Nincs találat", result = animal });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Belső hiba", result = e.Message });
            }
        }

        [HttpDelete("deleteAnimal/{id}")]

        public async Task<IActionResult> DeleteAnimal([FromRoute] Guid id)
        {
            try
            {
                var animal = await _context.Animals.FindAsync(id);
                if (animal is not null)
                {
                    _context.Animals.Remove(animal);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, new { message = "Tötrölve", result = animal.Name });
                }
                return StatusCode(404, new { message = "Nincs találat", result = animal });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Belső hiba", result = e.Message });
            }
        }
    }
}
