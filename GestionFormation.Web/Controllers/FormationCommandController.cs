﻿using System;
using System.Linq;
using System.Web.Http;
using GestionFormation.Applications.Trainings;
using GestionFormation.CoreDomain.Trainings.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Web.Controllers
{
    [RoutePrefix("api/v1/formation/command")]    
    public class FormationCommandController : ExtendedApiController
    {
        private readonly ITrainingQueries _queries;
        private readonly EventBus _eventBus;

        public FormationCommandController(EventBus eventBus, ITrainingQueries queries)
        {
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        [Route("create"), HttpPost]
        public IHttpActionResult Create([FromBody]string nom)
        {
            return Run(()=> new CreateTraining(_eventBus, _queries).Execute(nom,1));
        }

        [Route("update"), HttpPost]
        public IHttpActionResult Update([FromBody] FormationToUpdate formationToUpdate)
        {
            return Run(() => new UpdateTraining(_eventBus, _queries).Execute(formationToUpdate.FormationId, formationToUpdate.NewName,1));
        }

        [Route("delete"), HttpPost]
        public IHttpActionResult Delete([FromBody] Guid formationId)
        {
            //return Run(() => new DeleteFormation(_eventBus).Execute(formationId));
            return null;
        }
    }

    public class FormationToUpdate
    {
        public Guid FormationId { get; set; }
        public string NewName { get; set; }
    }
}
