using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public CitiesController(ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public IActionResult GetCities()
        {
            var cityEntities = _cityInfoRepository.GetCities();

            //var results = new List<CityWithoutPointOfInterestDto>();

            //foreach (var cityEntity in cityEntities)
            //{
            //    results.Add(new CityWithoutPointOfInterestDto()
            //    {
            //        Id = cityEntity.Id,
            //        Name = cityEntity.Name,
            //        Description = cityEntity.Description
            //    });
            //}

            // Map from Entities.City to CityDto. Defined in CityProfile.
            var results = _mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cityEntities);

            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointsOfInterest = false)
        {
            var cityEntity = _cityInfoRepository.GetCity(id, includePointsOfInterest);

            if (cityEntity == null)
            {
                return NotFound();
            }

            if (includePointsOfInterest)
            {
                //var cityResult = new CityDto()
                //{
                //    Id = cityEntity.Id,
                //    Name = cityEntity.Name,
                //    Description = cityEntity.Description
                //};

                //foreach (var p in cityEntity.PointOfInterest)
                //{
                //    cityResult.PointOfInterest.Add(
                //        new PointOfInterestDto()
                //        {
                //            Id = p.Id,
                //            Name = p.Name,
                //            Description = p.Description
                //        });
                //}

                var cityResult = _mapper.Map<CityDto>(cityEntity);

                return Ok(cityResult);
            }
            else
            {
                //var cityResult = new CityWithoutPointOfInterestDto()
                //{
                //    Id = cityEntity.Id,
                //    Name = cityEntity.Name,
                //    Description = cityEntity.Description
                //};

                var cityResult = _mapper.Map<CityWithoutPointOfInterestDto>(cityEntity);

                return Ok(cityResult);
            }

        }
    }
}
