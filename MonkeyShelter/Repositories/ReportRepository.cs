using Dapper;
using MonkeyShelter.DTO;
using System.Data;

namespace MonkeyShelter.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly IDbConnection _db;

        public ReportRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<List<MonkeySpeciesCountDto>> GetMonkeysPerSpeciesAsync()
        {
            string sql = @"
            SELECT 
                s.SpeciesName AS SpeciesName,
                COUNT(m.Id) AS MonkeyCount
            FROM 
                Monkeys m
            INNER JOIN 
                MonkeySpecies s ON m.SpeciesId = s.Id
            WHERE 
                m.DepartureDate IS NULL
            GROUP BY 
                s.SpeciesName";

            var result = await _db.QueryAsync<MonkeySpeciesCountDto>(sql);
            return result.ToList();
        }

        

        public async Task<List<MonkeySpeciesArrivalCountDto>> GetMonkeysPerSpeciesBetweenDatesAsync(DateTime startDate, DateTime endDate)
        {
            string sql = @"
            SELECT 
                s.SpeciesName AS SpeciesName,
                COUNT(m.Id) AS ArrivalCount
            FROM 
                Monkeys m
            INNER JOIN 
                MonkeySpecies s ON m.SpeciesId = s.Id
            WHERE 
                m.ArrivalDate BETWEEN @StartDate AND @EndDate
                AND m.DepartureDate IS NULL
            GROUP BY 
                s.SpeciesName";

            var result = await _db.QueryAsync<MonkeySpeciesArrivalCountDto>(sql, new { StartDate = startDate, EndDate = endDate });
            return result.ToList();
        }
    }
}
