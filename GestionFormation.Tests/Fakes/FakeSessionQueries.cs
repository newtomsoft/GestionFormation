using System;
using System.Collections.Generic;
using GestionFormation.CoreDomain.Sessions.Queries;

namespace GestionFormation.Tests.Fakes
{
    public class FakeSessionQueries : ISessionQueries
    {
        private List<ISessionResult> _session = new List<ISessionResult>();

        public void AddSession(Guid trainingId, Guid sessionId, DateTime startSessuib, int duration, Guid? locationId, Guid? trainerId)
        {
            var sessionResult = new FakeSessionResult();
            sessionResult.TrainingId = trainingId;
            sessionResult.SessionId = sessionId;
            sessionResult.SessionStart = startSessuib;
            sessionResult.Duration = duration;
            sessionResult.TrainerId = trainerId;
            sessionResult.LocationId = locationId;
            _session.Add(sessionResult);
        }

        public IEnumerable<ISessionResult> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISessionResult> GetAll(Guid TrainingId)
        {
            return _session;
        }

        public IEnumerable<ICompleteSessionResult> GetAllCompleteSession()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAllLocation()
        {
            throw new NotImplementedException();
        }

        public ICompleteSessionResult GetSession(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        public IClosedSessionResult GetClosedSession(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        private class FakeSessionResult : ISessionResult
        {
            public Guid SessionId { get; set; }
            public Guid TrainingId { get; set; }
            public DateTime SessionStart { get; set; }
            public int Duration { get; set; }
            public int Seats { get; set; }
            public int ReservedSeats { get; set; }
            public Guid? LocationId { get; set; }
            public Guid? TrainerId { get; set; }
            public bool Canceled { get; set; }
            public string CancelReason { get; set; }
        }
    }
}