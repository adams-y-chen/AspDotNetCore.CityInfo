﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Profiles
{
    public class PointOfInterestProfile : Profile
    {
        public PointOfInterestProfile()
        {
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestForCreationDto>().ReverseMap();
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestForUpdateDto>().ReverseMap();
            //CreateMap<Models.PointOfInterestForCreationDto, Entities.PointOfInterest>();
            //CreateMap<Models.PointOfInterestForUpdateDto, Entities.PointOfInterest>();
        }
    }
}
