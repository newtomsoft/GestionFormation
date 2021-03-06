﻿using System;
using GestionFormation.CoreDomain.Locations;
using GestionFormation.CoreDomain.Locations.Events;
using GestionFormation.EventStore;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure.Locations.Projections
{
    public class LocationSqlProjection : IProjectionHandler,
        IEventHandler<LocationCreated>,
        IEventHandler<LocationUpdated>,
        IEventHandler<LocationDeleted>,
        IEventHandler<LocationDisabled>
    {
        public void Handle(LocationCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Locations.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new LocationSqlEntity();
                    context.Locations.Add(entity);
                }

                entity.Id = @event.AggregateId;
                entity.Name = @event.Name;
                entity.Address = @event.Address;
                entity.Enabled = true;
                entity.Seats = @event.Seats;
                context.SaveChanges();
            }
        }

        public void Handle(LocationUpdated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var lieu = context.Locations.Find(@event.AggregateId);
                if( lieu == null )
                    throw new EntityNotFoundException(@event.AggregateId, "Lieu");

                lieu.Name = @event.Name;
                lieu.Address = @event.Address;
                lieu.Seats = @event.Seats;
                context.SaveChanges();
            }
        }

        public void Handle(LocationDeleted @event)
        {
            DisableLocation(@event.AggregateId);
        }

        public void Handle(LocationDisabled @event)
        {
            DisableLocation(@event.AggregateId);
        }

        private void DisableLocation(Guid locationId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var lieu = context.Locations.Find(locationId);
                if (lieu == null)
                    throw new EntityNotFoundException(locationId, "Lieu");

                lieu.Enabled = false;
                context.SaveChanges();
            }
        }
    }
}
