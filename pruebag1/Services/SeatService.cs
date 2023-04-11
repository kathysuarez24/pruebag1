using MongoDB.Driver;
using pruebag1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pruebag1.Services
{
    public class SeatService
    {
        private readonly IMongoCollection<Seat> _seats;

        public SeatService(IMongoDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _seats = database.GetCollection<Seat>("datos");
        }

        public List<Seat> Get() =>
            _seats.Find(seat => true).ToList();

        public Seat Get(string id) =>
            _seats.Find<Seat>(seat => seat.Reference == id).FirstOrDefault();

        public List<Seat> GetPendientes() =>
            _seats.Find<Seat>(seat => seat.Status == "pendiente").ToList();

        public Seat Create(Seat seat)
        {
            _seats.InsertOne(seat);
            return seat;
        }
    }

}
