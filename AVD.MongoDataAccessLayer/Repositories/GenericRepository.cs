using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics.Contracts;
using System.IO;
using AVD.MongoDataAccessLayer.Repositories;


namespace AVD.MongoDataAccessLayer.Repositories
{
    public class GenericRepository
    {


        private readonly IMongoConnectionStringRepository _connectionStringRepository;

        public GenericRepository(IMongoConnectionStringRepository connectionStringRepository)
        {
            if (connectionStringRepository == null) throw new ArgumentNullException("ConnectionStringRepository");
            _connectionStringRepository = connectionStringRepository;
        }

        public IMongoConnectionStringRepository ConnectionStringRepository
        {
            get
            {
                return _connectionStringRepository;
            }
        }
      
    }
}
