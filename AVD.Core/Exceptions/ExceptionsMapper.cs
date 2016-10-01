using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AVD.Common.Configuration;
using AVD.Core.Exceptions.Dtos;
using AVD.DataAccessLayer.Models;
using AVD.Common.Configuration;
using AVD.Core.Exceptions.Dtos;
using AVD.DataAccessLayer.Models;

namespace AVD.Core.Exceptions
{
    public class ExceptionsMapper : AutoMapperConfigBase
    {
        internal static ErrorMessageDto Map(ErrorMessage errorMessage)
        {
            return AutoMapper.Mapper.Map<ErrorMessageDto>(errorMessage);
        }
        internal static ErrorMessage Map(ErrorMessageDto errorMessage)
        {
            return AutoMapper.Mapper.Map<ErrorMessage>(errorMessage);
        }

        protected override void Configure()
        {
            var dateMinus10 = DateTime.Now.AddDays(-10);
            Mapper.CreateMap<ErrorMessage, ErrorMessageDto>()
                .ForMember(a => a.Category,
                    exp => exp.MapFrom(cp => ErrorMessage.GetErrorMessageCategory(cp.ExceptionFullName)))
                .ForMember(a => a.Name, exp => exp.MapFrom(cp => ErrorMessage.GetErrorMessageName(cp.ExceptionFullName)))
                .ForMember(a => a.CodeDescription, exp => exp.Ignore())
                // Todo: optimize
                .ForMember(a => a.LastOccuranceDateTime, exp => exp.MapFrom(cp => cp.ErrorOccurances.Any() ? cp.ErrorOccurances.Max(t => t.DateCreated) : (DateTime?)null))
                .ForMember(a => a.NumberOfOccurancesInLast10Days, exp => exp.MapFrom(cp => cp.ErrorOccurances.Count(t => t.DateCreated <= dateMinus10)))
                .ForMember(a => a.ParentErrorMessageId, exp => exp.Ignore());

            Mapper.CreateMap<ErrorMessageDto, ErrorMessage>()
                .ForMember(a => a.ErrorOccurances, exp => exp.Ignore());

        }
    }
}
