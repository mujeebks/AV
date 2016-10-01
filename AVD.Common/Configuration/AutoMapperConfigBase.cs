using System;
using System.Linq;
using System.Text;
using AutoMapper;

namespace AVD.Common.Configuration
{
    public abstract class AutoMapperConfigBase
    {
        public bool Initalized { get; set; }
        
        public void ConfigureMappings(bool doAssertConfigurationIsValid = false)
        {
            if(!Initalized)
                Configure();
            Initalized = true;

            if (doAssertConfigurationIsValid)
            Mapper.AssertConfigurationIsValid();
        }

        protected abstract void Configure();
    }
}
