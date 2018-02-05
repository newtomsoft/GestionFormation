using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Locations.Queries;

namespace GestionFormation.Tests.Fakes
{
    public class FakeLocationQueries : ILocationQueries
    {
        private List<ILocationResult> _lieux = new List<ILocationResult>();

        public void Add(string nom, string addresse, int places)
        {
            _lieux.Add(new Result(Guid.NewGuid(), nom, addresse, places));
        }
        
        public IReadOnlyList<ILocationResult> GetAll()
        {
            return _lieux;
        }

        public Guid? GetLocation(string nom)
        {
            return _lieux.FirstOrDefault(a => a.Name == nom)?.LocationId;
        }

        private class Result : ILocationResult
        {
            public Result(Guid lieuId, string nom, string addresse, int places)
            {
                LocationId = lieuId;
                Name = nom;
                Address = addresse;
                Seats = places;
            }
            public Guid LocationId { get;  }
            public string Name { get; }
            public string Address { get;  }
            public int Seats { get; }
        }
    }
}