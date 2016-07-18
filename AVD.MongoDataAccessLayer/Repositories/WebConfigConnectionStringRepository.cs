using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVD.MongoDataAccessLayer.Repositories;

namespace AVD.MongoDataAccessLayer.Repositories
{
    public class WebConfigConnectionStringRepository : IMongoConnectionStringRepository
    {
        public string ReadConnectionString(string connectionStringName)
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }
    }
}
