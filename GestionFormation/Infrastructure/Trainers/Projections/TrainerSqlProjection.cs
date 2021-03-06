﻿using GestionFormation.CoreDomain.Trainers.Events;
using GestionFormation.EventStore;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure.Trainers.Projections
{
    public class TrainerSqlProjection : IProjectionHandler,
        IEventHandler<TrainerCreated>,
        IEventHandler<TrainerUpdated>,
        IEventHandler<TrainerDeleted>,
        IEventHandler<TrainerDisabled>
    {
        public void Handle(TrainerCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Trainers.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new TrainerSqlEntity();
                    context.Trainers.Add(entity);
                }

                entity.TrainerId = @event.AggregateId;
                entity.Lastname = @event.Lastname;
                entity.Firstname = @event.Firstname;
                entity.Email = @event.Email;
                entity.Enabled = true;
                context.SaveChanges();
            }
        }

        public void Handle(TrainerUpdated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var trainerToUpdate = context.Trainers.Find(@event.AggregateId);
                if( trainerToUpdate == null )
                    throw new EntityNotFoundException(@event.AggregateId, "Formateur");

                trainerToUpdate.Lastname = @event.Lastname;
                trainerToUpdate.Firstname = @event.Firstname;
                trainerToUpdate.Email = @event.Email;
                context.SaveChanges();
            }
        }

        public void Handle(TrainerDeleted @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.GetEntity<TrainerSqlEntity>(@event.AggregateId);
                context.Trainers.Attach(entity);
                context.Trainers.Remove(entity);
                context.SaveChanges();
            }
        }

        public void Handle(TrainerDisabled @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.GetEntity<TrainerSqlEntity>(@event.AggregateId);
                entity.Enabled = false;
                context.SaveChanges();
            }
        }
    }
}
