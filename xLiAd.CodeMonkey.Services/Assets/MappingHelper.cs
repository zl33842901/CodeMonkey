using AutoMapper;
using xLiAd.CodeMonkey.Entities;
using xLiAd.CodeMonkey.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xLiAd.CodeMonkey.Services.Assets
{
    public class MappingHelper
    {
        private static IMapper MapperInstance;
        public static IMapper Mapper
        {
            get
            {
                if (MapperInstance == null)
                    MapperInstance = Configuration.CreateMapper();
                return MapperInstance;
            }
        }

        private static MapperConfiguration Configuration { get; } = new MapperConfiguration(cfg =>
        {

            cfg.CreateMap<CycleEditDto, Cycle>();
            cfg.CreateMap<Cycle, CycleDto>();
            cfg.CreateMap<DepartEditDto, Depart>();
            cfg.CreateMap<Depart, DepartDto>();
        });
    }
}
