using SQLite;

namespace MauiApp_StudyProject_.Models
{
    [Table("Beers")]
    public class Beer : BaseEntity
    {
        [MaxLength(100), Unique, Indexed]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// The alcohol content of the beer, 
        /// represented as a percentage.
        /// </summary>
        public double AlcoholContent { get; set; }
    }
}
