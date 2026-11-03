namespace MauiApp_StudyProject_.Models
{
   public class Brewery
    {
        public int Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// A list of beers produced by the brewery.
        /// </summary>
        public List<Beer> Beers { get; set; } = new List<Beer>();
    }
}
