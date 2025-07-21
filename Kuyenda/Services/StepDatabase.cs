using SQLite;
using Kuyenda.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Kuyenda.Services
{
    public class StepDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public StepDatabase()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "kuyenda.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<StepModel>().Wait();
        }

        // Get all steps sorted by date descending
        public Task<List<StepModel>> GetAllStepsAsync()
        {
            return _database.Table<StepModel>()
                            .OrderByDescending(s => s.Date)
                            .ToListAsync();
        }

        // Get a single step entry by stringified date (yyyy-MM-dd)
        public Task<StepModel> GetStepByDateAsync(DateTime date)
        {
            string dateString = date.ToString("yyyy-MM-dd");
            return _database.Table<StepModel>()
                            .FirstOrDefaultAsync(s => s.Date == dateString);
        }

        // Insert or update a step entry
        public Task<int> SaveStepAsync(StepModel step)
        {
            if (step.Id != 0)
            {
                return _database.UpdateAsync(step);
            }
            else
            {
                return _database.InsertAsync(step);
            }
        }

        // Delete a step entry
        public Task<int> DeleteStepAsync(StepModel step)
        {
            return _database.DeleteAsync(step);
        }

        // Optional: Clear all records (for testing)
        public Task<int> DeleteAllStepsAsync()
        {
            return _database.DeleteAllAsync<StepModel>();
        }
    }
}
