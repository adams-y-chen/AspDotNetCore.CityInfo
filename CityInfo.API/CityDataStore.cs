using CityInfo.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API
{
    public class CityDataStore
    {
        public static CityDataStore Current { get; } = new CityDataStore();

        public List<CityDto> Cities { get; set; }

        CityDataStore()
        {
            Cities = new List<CityDto>()
            {
                new CityDto()
                {
                    Id = 1,
                    Name = "New York City",
                    Description = "City with central park",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Liberty Statue",
                            Description = "A gift from French"
                        },
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "Central station",
                            Description = "It's a train station"
                        }
                    }
                },
                new CityDto()
                {
                    Id = 2,
                    Name = "Boston",
                    Description = "City with Redsox team"
                }
            };
        }
    }
}
