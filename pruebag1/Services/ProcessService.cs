using MongoDB.Driver;
using pruebag1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pruebag1.Services
{
    public class ProcessService
    {
        private readonly IMongoCollection<Process> _process;

        public ProcessService(IMongoDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _process = database.GetCollection<Process>("process");
        }

        public List<Process> Get() =>
            _process.Find(proc => true).ToList();

        public Process Get(string id) =>
            _process.Find<Process>(proc => proc.Id == id).FirstOrDefault();

        public Process Create(Process proc)
        {
            _process.InsertOne(proc);
            return proc;
        }

        public void Update(string id, Process processIn) =>
            _process.ReplaceOne(proc => proc.Id == id, processIn);
    }

}
