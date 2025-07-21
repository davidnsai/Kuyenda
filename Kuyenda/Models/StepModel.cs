using SQLite;

namespace Kuyenda.Models
{
    public class StepModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int Steps { get; set; }

        public string ?Date { get; set; }
    }
}
