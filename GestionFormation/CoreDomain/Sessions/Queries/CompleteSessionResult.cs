﻿using GestionFormation.CoreDomain.Sessions.Projections;

namespace GestionFormation.CoreDomain.Sessions.Queries
{
    public class CompleteSessionResult : SessionResult, ICompleteSessionResult
    {       
        public CompleteSessionResult(SessionSqlEntity session, string trainingName, string location, string trainerLastname, string trainerFirstname): base(session)
        {
            Training = trainingName;
            Trainer = new FullName(trainerLastname, trainerFirstname);
            Location = location;
        }
       
        public string Training { get; }
        public FullName Trainer { get;  }
        public string Location { get;  }
    }
}