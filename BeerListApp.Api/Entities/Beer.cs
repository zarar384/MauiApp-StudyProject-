using System.ComponentModel.DataAnnotations;

namespace BeerListApp.Api.Entities
{
    public class Beer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double AlcoholContent { get; set; }
    }
}
