using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVD.Common.Logging;
using AVD.Core.Shared.Dtos;
using AVD.DataAccessLayer;
using AVD.DataAccessLayer.Enums;
using AVD.DataAccessLayer.Models;
using AVD.DataAccessLayer.Repositories;

namespace AVD.Core.Shared
{
    public class PersonMapper
    {
        /// <summary>
        /// Given a person DTO, converts it into a client data model
        /// </summary>
        /// <param name="personDto">Client DTO to transform</param>
        /// <returns>Client data model equivalent to the DTO provided</returns>
        public void UpdatePersonModel(User person, PersonDto personDto, ref List<BaseModel> entitesToDelete)
        {
            Logger.Instance.LogFunctionEntry(GetType().Name, "UpdatePersonModel");
             
            person.FirstName = personDto.FirstName;
            person.MiddleName = personDto.MiddleName;
            person.LastName = personDto.LastName;
            person.Title = personDto.Title;
            Logger.Instance.LogFunctionExit(GetType().Name, "UpdatePersonModel");
        }
    }
}
