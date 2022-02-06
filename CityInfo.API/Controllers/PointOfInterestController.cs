using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    public class PointOfInterestController : ControllerBase
    {
        private ILogger<PointOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _cityInfoRepository;
        private IMapper _mapper;

        // logger is injected by DI container.
        public PointOfInterestController(ILogger<PointOfInterestController> logger,
            IMailService mailService,
            ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                if (!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with Id ${cityId} was not found.");
                    return NotFound();
                }

                var pointOfInterestEntities = _cityInfoRepository.GetPointsOfInterestForCity(cityId);

                //var result = new List<PointOfInterestDto>();

                //foreach (var p in pointOfInterestEntities)
                //{
                //    result.Add(new PointOfInterestDto
                //    {
                //        Id = p.Id,
                //        Name = p.Name,
                //        Description = p.Description
                //    });
                //}

                var result = _mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterestEntities);

                return Ok(result);
            } 
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting point of interests in city {cityId}", ex);
                // throw;
                return StatusCode(500, "Internal error while handling your request.");
            }
        }

        [HttpGet("{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                _logger.LogInformation($"City with Id ${cityId} was not found.");
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            //var result = new PointOfInterestDto()
            //{
            //    Id = pointOfInterestEntity.Id,
            //    Name = pointOfInterestEntity.Name,
            //    Description = pointOfInterestEntity.Description
            //};

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterestEntity));
        }

        [HttpPost]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            // Model validation results is autmatically checked so the code below is not needed.
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}

            // Complex validation that the DTO model validation annotation that don't handle.
            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError(
                    "Description", "Description should be different than the name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CityDataStore.Current.Cities
                .FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            //// demo purpose. will be removed.
            //var maxId = CityDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest)
            //    .Max(p => p.Id);

            //var finalPointOfInterest = new PointOfInterestDto()
            //{
            //    Id = maxId + 1,
            //    Name = pointOfInterest.Name,
            //    Description = pointOfInterest.Description
            //};

            //city.PointsOfInterest.Add(finalPointOfInterest);

            var finalPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterest);

            _cityInfoRepository.AddPointOfInterestForCity(cityId, finalPointOfInterest);

            _cityInfoRepository.Save();

            var createdPointOfInterest = _mapper.Map<PointOfInterestForCreationDto>(finalPointOfInterest);

            return CreatedAtRoute(
                "GetPointOfInterest",
                new { cityId, id = finalPointOfInterest.Id },
                createdPointOfInterest);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, 
            [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            // Complex validation that the DTO model validation annotation that don't handle.
            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError(
                    "Description", "Description should be different than the name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CityDataStore.Current.Cities
                .FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            //// Find the stored pointOfInterest and update it.
            //var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            //if (pointOfInterestFromStore == null)
            //{
            //    return NotFound();
            //}

            //// Note: PUT request requires to update the entire object.
            //pointOfInterestFromStore.Name = pointOfInterest.Name;
            //pointOfInterestFromStore.Description = pointOfInterest.Description;

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            // This will copy the fields from pointOfInterest to pointOfInterestEntity.
            _mapper.Map(pointOfInterest, pointOfInterestEntity);

            _cityInfoRepository.UpdatePointOfInterestForCity(cityId, pointOfInterestEntity);

            _cityInfoRepository.Save();

            // Succeeded and no contenct to return.
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            var city = CityDataStore.Current.Cities
                .FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            //// Find the stored pointOfInterest and update it.
            //var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            //if (pointOfInterestFromStore == null)
            //{
            //    return NotFound();
            //}

            //var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            //{
            //    Name = pointOfInterestFromStore.Name,
            //    Description = pointOfInterestFromStore.Description
            //};

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            // Apply the patching instructions from PATCH request
            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            // This only contains the errors of the JsonPatchDocument. E.g. the json specifies an property that doesn't exist in PointOfInterestForUpdateDto.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Complex validation that the DTO model validation annotation that don't handle.
            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError(
                    "Description", "Description should be different than the name.");
            }

            // Validate the patched DTO 
            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            _cityInfoRepository.UpdatePointOfInterestForCity(cityId, pointOfInterestEntity);

            _cityInfoRepository.Save();

            //// Save back to data store.
            //pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            //pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            // Succeeded and no contenct to return.
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CityDataStore.Current.Cities
                .FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            //// Find the stored pointOfInterest and update it.
            //var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            //if (pointOfInterestFromStore == null)
            //{
            //    return NotFound();
            //}

            //city.PointsOfInterest.Remove(pointOfInterestFromStore);

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

            _cityInfoRepository.Save();

            _mailService.Send("Point of interest deleted.",
                $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");
            _logger.LogInformation($"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }
    }
}
