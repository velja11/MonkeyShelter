using Dapper;
using MonkeyShelter.DTO;
using MonkeyShelter.Models;
using System.Data;

namespace MonkeyShelter.Repositories
{
    public class MonkeyRepository: IMonkeyRepository
    {
        private readonly IDbConnection _db;

        public MonkeyRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> AddMonkeyAsync(Monkey monkey)
        {
            string sql = @"INSERT INTO Monkeys (Name, SpeciesId, Weight, ArrivalDate, DepartureDate, ShelterId)
                       VALUES (@Name, @SpeciesId, @Weight, @ArrivalDate, @DepartureDate, @ShelterId);
                       SELECT last_insert_rowid();";  // ➡️ Vrati ID poslednje ubačenog

        
            try
            {
                var insertedId = await _db.ExecuteScalarAsync<int>(sql, monkey);
                return insertedId;
                //await _db.ExecuteAsync(sql, monkey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❗ Error inserting monkey: {ex.Message}");
                throw;
            }

            
        }


        public async Task<bool> SpeciesExistsAsync(int speciesId)
        {
            var sql = "SELECT COUNT(1) FROM MonkeySpecies WHERE Id = @SpeciesId";
            var count = await _db.ExecuteScalarAsync<int>(sql, new { SpeciesId = speciesId });
            return count > 0;
        }

        public async Task<int> GetTodayArrivalsCountAsync()
        {
            var today = DateTime.UtcNow.Date;
            string sql = "SELECT COUNT(*) FROM Monkeys WHERE date(ArrivalDate) = date(@Today)";
            return await _db.ExecuteScalarAsync<int>(sql, new { Today = today });
        }

        //------departure
        public async Task<int> GetTodayDepartureCountAsync()
        {
            string sql = "SELECT COUNT(*) FROM Monkeys WHERE DATE(DepartureDate) = DATE('now')";
            return await _db.ExecuteScalarAsync<int>(sql);
        }

        public async Task<int> GetSpeciesCountAsync(int speciesId)
        {
            string sql = "SELECT COUNT(*) FROM Monkeys WHERE SpeciesId = @speciesId AND DepartureDate IS NULL";
            return await _db.ExecuteScalarAsync<int>(sql, new { speciesId });
        }


        public async Task<Monkey?> GetMonkeyByIdAsync(int monkeyId)
        {
            string sql = "SELECT Name, SpeciesId,Weight,ArrivalDate, ShelterId, DepartureDate FROM Monkeys WHERE Id = @Id";
            return await _db.QueryFirstOrDefaultAsync<Monkey>(sql, new { Id = monkeyId });
        }


        public async Task<List<Monkey>> UpdateDepartureDateAsync(int monkeyId, DateTime departureDate)
        {
            string sql = @"UPDATE Monkeys
                   SET DepartureDate = @DepartureDate
                   WHERE Id = @Id";

            await _db.ExecuteAsync(sql, new { DepartureDate = departureDate, Id = monkeyId });

            string getRemainingSql = @"SELECT * FROM Monkeys WHERE DepartureDate IS NULL";

            var remainingMonkeys = (await _db.QueryAsync<Monkey>(getRemainingSql)).ToList();
            return remainingMonkeys;
        }





        public async Task<List<Monkey>> GetAllAsync()
        {
            return (await _db.QueryAsync<Monkey>("SELECT * FROM Monkeys")).ToList();
        }

        public async Task<Monkey?> UpdateMonkeyWeightAsync(UpdateWeightDTO weight, int monkeyId)
        {
            string sql = @"UPDATE Monkeys
                   SET Weight = @Weight
                   WHERE Id = @Id";

            await _db.ExecuteAsync(sql, new { Weight = weight.Weight, Id = monkeyId });

            string sqlSelect = @"SELECT Id, Name, SpeciesId, Weight, ArrivalDate, DepartureDate, ShelterId 
                         FROM Monkeys
                         WHERE Id = @Id";

            var monkey = await _db.QueryFirstOrDefaultAsync<Monkey>(sqlSelect, new { Id = monkeyId });

            return monkey;
        }

        public async Task<IEnumerable<MonkeyVetCheck>> GetMonkeysForVetCheckAsync()
        {
            string sql = @"
                SELECT Id, Name, SpeciesId, Weight, ArrivalDate, DepartureDate, ShelterId,
                CASE 
                   WHEN DATE('now') > DATE(COALESCE(LastVetCheck, ArrivalDate), '+60 days') THEN 1
                   ELSE 0
               END AS NeedsVetCheck
                FROM Monkeys
                WHERE DepartureDate IS NULL; ";

            return await _db.QueryAsync<MonkeyVetCheck>(sql);
        }

        public async Task UpdateLastVetCheckAsync(int monkeyId)
        {
            string sql = @"UPDATE Monkeys 
                   SET LastVetCheck = @Now
                   WHERE Id = @Id";

            await _db.ExecuteAsync(sql, new { Now = DateTime.UtcNow, Id = monkeyId });
        }
    }
}
