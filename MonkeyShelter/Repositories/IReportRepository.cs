using MonkeyShelter.DTO;

namespace MonkeyShelter.Repositories
{
    public interface IReportRepository
    {
        Task<List<MonkeySpeciesCountDto>> GetMonkeysPerSpeciesAsync();
        Task<List<MonkeySpeciesArrivalCountDto>> GetMonkeysPerSpeciesBetweenDatesAsync(DateTime startDate, DateTime endDate);
    }
}
