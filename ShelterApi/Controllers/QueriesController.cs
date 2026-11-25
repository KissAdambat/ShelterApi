using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShelterApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ShelterApi.Controllers
{
    [Route("/queries")]
    [ApiController]
    public class QueriesController : ControllerBase
    {
        private readonly AnimalshelterContext _context;

        public QueriesController(AnimalshelterContext context)
        {
            _context = context;
        }

        [HttpGet("allSheltersWithAnimals")]
        public async Task<IActionResult> AllSheltersWithAnimals()
        {
            try
            {
                var shelters = await _context.Shelters
                    .Select(s => new
                    {
                        s.Id,
                        s.Name,
                        s.City,
                        s.Capacity,
                        s.RegTime,
                        Animals = _context.Animals.Where(a => a.ShelterId == s.Id).ToList()
                    })
                    .ToListAsync();

                return StatusCode(201,new {message = "Sikeres lekérés",result = shelters});
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Belső hiba", result = e.Message });
            }
        }

        [HttpGet("allAnimalsWithShelter")]
        public async Task<IActionResult> AllAnimalsWithShelter()
        {
            try
            {
                var result = await _context.Animals
                    .Join(
                        _context.Shelters,
                        animal => animal.ShelterId,
                        shelter => shelter.Id,
                        (animal, shelter) => new
                        {
                            animal.Id,
                            animal.Name,
                            animal.Species,
                            animal.Age,
                            animal.Weight,
                            animal.RegTime,
                            animal.ShelterId,
                            ShelterName = shelter.Name
                        }).ToListAsync();

                return StatusCode(201,new{message = "Sikeres lekérés",result});
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Belső hiba", result = e.Message });
            }
        }

        [HttpGet("animalsOfShelter")]
        public async Task<IActionResult> AnimalsOfShelter([FromQuery] Guid shelterId)
        {
            try
            {
                var shelter = await _context.Shelters
                    .Where(s => s.Id == shelterId)
                    .Select(s => new
                    {
                        s.Id,
                        s.Name,
                        s.City,
                        s.Capacity,
                        s.RegTime,
                        Animals = _context.Animals.Where(a => a.ShelterId == s.Id).ToList()
                    }).FirstOrDefaultAsync();

                if (shelter is not null)
                {
                    return StatusCode(200, new { message = "Sikeres lekérés", result = shelter });
                }
                else
                {
                    return StatusCode(404, new { message = "Nincs ilyen menhely" });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Belső hiba", result = e.Message });
            }
        }

        [HttpGet("animalCountByShelter")]
        public async Task<IActionResult> AnimalCountByShelter()
        {
            try
            {
                var result = await _context.Shelters
                    .Select(s => new
                    {
                        ShelterId = s.Id,
                        ShelterName = s.Name,
                        City = s.City,
                        AnimalCount = _context.Animals.Count(a => a.ShelterId == s.Id)
                    }).OrderByDescending(x => x.AnimalCount).ToListAsync();

                return StatusCode(200, new { message = "Sikeres lekérés. Menhelykénti állatszám", result = result });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Belső hiba", result = e.Message });
            }
        }
    }
}
