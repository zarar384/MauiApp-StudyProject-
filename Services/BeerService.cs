using MauiApp_StudyProject_.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp_StudyProject_.Services
{
    public class BeerService
    {
        public List<Beer> GetBeers()
        {
            // In a real application, this method would retrieve data from a database or an API.
            return new List<Beer>
            {
                new Beer { Id = 1, Name = "Pale Ale", Description = "A light and hoppy beer.", AlcoholContent = 5.0 },
                new Beer { Id = 2, Name = "Stout", Description = "A dark and rich beer.", AlcoholContent = 6.5 },
                new Beer { Id = 3, Name = "Lager", Description = "A crisp and refreshing beer.", AlcoholContent = 4.5 },
                new Beer { Id = 4, Name = "IPA", Description = "A strong and bitter beer.", AlcoholContent = 7.0 },
                new Beer { Id = 5, Name = "Wheat Beer", Description = "A smooth and fruity beer.", AlcoholContent = 5.5 },
                new Beer { Id = 6, Name = "Pilsner", Description = "A light and crisp beer.", AlcoholContent = 4.8 },
                new Beer { Id = 7, Name = "Porter", Description = "A dark and malty beer.", AlcoholContent = 6.0 },
            };
        }
    }
}
