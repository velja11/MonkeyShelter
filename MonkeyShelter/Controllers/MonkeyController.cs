using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonkeyShelter.DTO;
using MonkeyShelter.Models;
using MonkeyShelter.Repositories;

namespace MonkeyShelter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonkeyController : ControllerBase
    {
        private readonly IMonkeyRepository _monkeyRepository;

        public MonkeyController(IMonkeyRepository monkeyRepository)
        {
            _monkeyRepository = monkeyRepository;
        }

        /// Adds a new monkey to the shelter if the daily limit(7 monkeys) allows it. Return ID of new added monkey
        [HttpPost("arrival")]
        public async Task<IActionResult> AddMonkey([FromBody] MonkeyArrival dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new OutputResponse<string> {
                    Success = false,
                    ErrorMessage = "Invalid request data."
                });

            if (dto.Weight <= 0)
            {
                return BadRequest(new OutputResponse<string>
                {
                    Success = false,
                    ErrorMessage = "Weight must be greater than 0."
                });
            }

            try
            {



                var species = await _monkeyRepository.SpeciesExistsAsync(dto.SpeciesId);
                if (!species)
                {
                    return BadRequest(new OutputResponse<string>
                    {
                        Success = false,
                        ErrorMessage = "Invalid ID species!"
                    });
                }

                var todaysArrival = await _monkeyRepository.GetTodayArrivalsCountAsync();
                if (todaysArrival >= 7)
                {
                    return BadRequest(new OutputResponse<string>
                    {
                        Success = false,
                        ErrorMessage = "Cannot add more than 7 monkeys per day!"
                    });
                }

                var monkey = new Monkey
                {
                    Name = dto.Name,
                    SpeciesId = dto.SpeciesId,
                    Weight = dto.Weight,
                    ArrivalDate = DateTime.UtcNow,
                    ShelterId = dto.ShelterId
                };

                var newmonkeyID = await _monkeyRepository.AddMonkeyAsync(monkey);
                return Ok(new OutputResponse<object>
                {
                    Success = true,
                    Data = new { id = newmonkeyID, message = "Monkey added successfully." }
                });
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new OutputResponse<string>
                {
                    Success = false,
                    ErrorMessage = "Error occured while adding monkey!"
                });
                
            }
        }

        // Marks a monkey as departed from the shelter if rules allow it (up to 5 departures daily, species check, etc.). Returns as list of remaining monkeys
        [HttpPatch("departure/{id}")]
        public async Task<IActionResult> DepartMonkey(int id)
        {
            try
            {
                var monkey = await _monkeyRepository.GetMonkeyByIdAsync(id);

                if(monkey == null)
                {
                    return NotFound(new OutputResponse<string>
                    {
                        Success = false,
                        ErrorMessage = "Monkey is not founded!"
                    });
                }

                if (monkey?.DepartureDate != null)
                    return BadRequest(new OutputResponse<string>
                    {
                        Success = false,
                        ErrorMessage = "Monkey is already departed!"
                    });

                var departuresToday = await _monkeyRepository.GetTodayDepartureCountAsync();

                if(departuresToday > 5)
                     return BadRequest(new OutputResponse<string>
                     {
                         Success = false,
                         ErrorMessage = "Maximum departures approach!"
                     });

                var arrivalsToday = await _monkeyRepository.GetTodayArrivalsCountAsync();
                if(departuresToday - arrivalsToday > 2)
                    return BadRequest(new OutputResponse<string>
                    {
                        Success = false,
                        ErrorMessage = "Cannot leave the shelter!"
                    });

                var speciesCount = await _monkeyRepository.GetSpeciesCountAsync(monkey!.SpeciesId);
                if (speciesCount <= 1)
                    return BadRequest(new OutputResponse<string>
                    {
                        Success = false,
                        ErrorMessage = "Last species in the shelter. Cannot leave!"
                    });

                monkey.DepartureDate = DateTime.UtcNow;
                var remainingMonkeys = await _monkeyRepository.UpdateDepartureDateAsync(id, monkey.DepartureDate.Value);

                return Ok(new OutputResponse<List<Monkey>>
                {
                    Success = true,
                    Data = remainingMonkeys
                });
            }
            catch
            {
                return StatusCode(500, new OutputResponse<string>
                {
                    Success = false,
                    ErrorMessage = "Error occured while departure monkey!"
                });
            }
        }

        // Updates the weight of a monkey if it is still active (not departed). Returns a information about changed monkey
        [HttpPatch("{id}/weight")]
        public async Task<IActionResult> UpdateMonkeyWeight(int id, [FromBody] UpdateWeightDTO updateWeight)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new OutputResponse<string>
                {
                    Success = false,
                    ErrorMessage = "Invalid weight data."
                });
            }

            var monkey = await _monkeyRepository.GetMonkeyByIdAsync(id);

            if (monkey == null)
            {
                return NotFound(new OutputResponse<string>
                {
                    Success = false,
                    ErrorMessage = "Monkey not found."
                });
            }

            if (monkey.DepartureDate != null)
            {
                return BadRequest(new OutputResponse<string>
                {
                    Success = false,
                    ErrorMessage = "Cannot update weight for a departed monkey."
                });
            }

            var updatedMonkey = await _monkeyRepository.UpdateMonkeyWeightAsync(updateWeight, id);

            return Ok(new OutputResponse<Monkey>
            {
                Success = true,
                Data = updatedMonkey
            });

        }

        // Returns the list of monkeys that are due for a vet check (every 60 days). 
        [HttpGet("vet-checks")]
        public async Task<IActionResult> GetMonkeysForVetCheck()
        {
            var monkeys = await _monkeyRepository.GetMonkeysForVetCheckAsync();

            return Ok(new OutputResponse<IEnumerable<MonkeyVetCheck>>
            {
                Success = true,
                Data = monkeys
            });
        }

        // Updates the last vet check date for a specific monkey. Returns updated information
        [HttpPatch("{id}/vet-check")]
        public async Task<IActionResult> UpdateVetCheck(int id)
        {
            try
            {
                var monkey = await _monkeyRepository.GetMonkeyByIdAsync(id);
                if (monkey == null)
                {
                    return NotFound(new OutputResponse<string>
                    {
                        Success = false,
                        ErrorMessage = "Monkey not found."
                    });
                }

                await _monkeyRepository.UpdateLastVetCheckAsync(id);

                var updatedMonkey = await _monkeyRepository.GetMonkeyByIdAsync(id);

                return Ok(new OutputResponse<Monkey>
                {
                    Success = true,
                    Data = updatedMonkey
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new OutputResponse<string>
                {
                    Success = false,
                    ErrorMessage = "An error occurred while updating vet check."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var monkeys = await _monkeyRepository.GetAllAsync();
            return Ok(monkeys);
        }
    }
}
