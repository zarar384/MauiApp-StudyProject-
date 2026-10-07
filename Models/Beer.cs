namespace MauiApp_StudyProject_.Models
{
    public class Beer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// The alcohol content of the beer, 
        /// represented as a percentage.
        /// </summary>
        public double AlcoholContent { get; set; }
    }
}
