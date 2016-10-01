using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using Newtonsoft.Json;

using AVD.DataAccessLayer;
using AVD.DataAccessLayer.Models;

using AVD.Common.Helpers;
using AVD.Common.Logging;

namespace AVD.Core
{
    /// <summary>
    /// This configures the automapper. It should be called on startup and once per
    /// app domain. For sites, a call to AutoMapperConfiguration.Configure() in Global.Application_Start()
    /// will be sufficient
    /// </summary>
    public class AutoMapperConfiguration
    {
        public static bool Initialized { get; private set; }

        static AutoMapperConfiguration()
        {
            Initialized = false;
        }

        public static void Configure()
        {
            if (Initialized)
                return;

            Logger.Instance.LogFunctionEntry(typeof(AutoMapperConfiguration).Name, "Configure");

            // Create the maps between the Models and reusable LookupDtos
            //Mapper.CreateMap<CruiseDestination, CruiseDestinationNodeDto>().ForMember(
            //    t => t.CruiseDestinations,
            //    expression =>
            //        expression.MapFrom(
            //            exp =>
            //                exp.CruiseDestinations1.Where(c => (!c.IsArchieved.HasValue || c.IsArchieved.Value == false) && c.CruiseShipItineraries.SelectMany(x => x.CruiseShipSailings).Any())));

            //Mapper.CreateMap<CruiseDestination, CruiseDestinationLookupDto>();
          
            // Will change to OperatorDto when UI is ready
            //Mapper.CreateMap<Operator, CruiseLineLookupDto>()
            //    .ForMember(dto => dto.CruiseLineId, expression => expression.MapFrom(@operator => @operator.OperatorId))
            //    .ForMember(dto => dto.Name, expression => expression.MapFrom(@operator => @operator.Name))
            //    .ForMember(d => d.IsOperatorSupported, exp => exp.MapFrom(s => CruiseCatalogManager.IsOperatorSupportedCached(s.OperatorId)));

          
            // TODO-ARCH: Identify and load up the mappings on startup
            // TODO-ARCH: Implement dependencies between mappings
            // new HotelTravelServiceMapper().ConfigureMappings();
           

            Initialized = true;

            Logger.Instance.LogFunctionExit(typeof(AutoMapperConfiguration).Name, "Configure");
        }
    }
}
