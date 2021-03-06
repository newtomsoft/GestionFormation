using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Students.Queries;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure.Students.Projections;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure.Students.Queries
{
    public class StudentSqlQueries : IStudentQueries, IRuntimeDependency
    {
        public IReadOnlyList<IStudentResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Students.Where(a=>a.Removed == false).ToList().Select(entity => new StudentResult(entity)).ToList();
            }
        }

        private class StudentResult : IStudentResult
        {
            public StudentResult(StudentSqlEntity entity)
            {
                Id = entity.StudentId;
                Lastname = entity.Lastname;
                Firstname = entity.Firstname;
            }

            public Guid Id { get; }
            public string Lastname { get; }
            public string Firstname { get; }           
        }
    }
}