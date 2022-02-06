using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<CityDto> GetCities()
        {
            return CityDataStore.Current.Cities;
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id)
        {
            var city = CityDataStore.Current.Cities
                .FirstOrDefault( x => x.Id == id );

            if (city == null)
            {
                return NotFound();
            }

            return Ok(city);
        }
    }
}
