using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.MongoDataAccessLayer.Models
{
    public class MultiProperty
    {

        string _propertyName;
        dynamic _propertyValue;

        public string propertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                _propertyName = value;
            }
        }
        public dynamic propertyValue
        {
            get
            {
                return _propertyValue;
            }
            set
            {
                _propertyValue = value;
            }
        }
    }
}
