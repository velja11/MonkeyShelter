using MonkeyShelter.DTO;
using MonkeyShelter.Models;

namespace MonkeyShelter.Repositories
{
    public interface IMonkeyRepository
    {
        Task<int> AddMonkeyAsync(Monkey monkey);
        Task<List<Monkey>> GetAllAsync();
        Task<bool> SpeciesExistsAsync(int speciesId);
        Task<int> GetTodayArrivalsCountAsync();
        Task<Monkey?> GetMonkeyByIdAsync(int monkeyId);
        Task<int> GetTodayDepartureCountAsync();
        Task<int> GetSpeciesCountAsync(int speciesId);
        Task<List<Monkey>> UpdateDepartureDateAsync(int monkeyId, DateTime departureDate);
        Task<Monkey> UpdateMonkeyWeightAsync(UpdateWeightDTO weight, int id);
        Task<IEnumerable<MonkeyVetCheck>> GetMonkeysForVetCheckAsync();
        Task UpdateLastVetCheckAsync(int monkeyId);

    }
}
