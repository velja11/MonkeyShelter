using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonkeyShelter.DTO;
using MonkeyShelter.Models;
using MonkeyShelter.Repositories;

namespace MonkeyShelter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository _reportRepository;

        public ReportController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        /// Returns the count of monkeys per species currently in the shelter.
        [HttpGet("monkey-count-per-species")]
        public async Task<IActionResult> GetMonkeyCountPerSpecies()
        {
            var result = await _reportRepository.GetMonkeysPerSpeciesAsync();

            return Ok(new OutputResponse<List<MonkeySpeciesCountDto>>
            {
                Success = true,
                Data = result
            });
        }

        /// Returns the count of monkeys per species currently in the shelter arrived between two dates.
        [HttpGet("monkey-arrivals-between-dates")]
        public async Task<IActionResult> GetMonkeyCountPerSpeciesBetweenDates([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _reportRepository.GetMonkeysPerSpeciesBetweenDatesAsync(startDate, endDate);

            return Ok(new OutputResponse<List<MonkeySpeciesArrivalCountDto>>
            {
                Success = true,
                Data = result
            });
        }
    }
}
