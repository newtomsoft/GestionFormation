﻿using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Sessions.Queries;
using GestionFormation.Infrastructure.Sessions.Projections;

namespace GestionFormation.Infrastructure.Sessions.Queries
{
    public class CompleteSessionResult : SessionResult, ICompleteSessionResult
    {       
        public CompleteSessionResult(SessionSqlEntity session, string trainingName, string location, string trainerLastname, string trainerFirstname, int color): base(session)
        {
            Training = trainingName;
            Trainer = new FullName(trainerLastname, trainerFirstname);
            Location = location;
            Color = color;
        }
       
        public string Training { get; }
        public FullName Trainer { get;  }
        public string Location { get;  }
        public int Color { get; }
    }
}