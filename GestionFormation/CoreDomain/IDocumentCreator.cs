using System;
using System.Collections.Generic;
using GestionFormation.App.Views.Seats;

namespace GestionFormation.CoreDomain
{
    public interface IDocumentCreator
    {        
        string CreateCertificateOfAttendance(FullName student, string company, string training, string location, int duration, FullName trainer, DateTime startSession);

        string CreateDegree(FullName student, string company, DateTime startSession, DateTime endSession, FullName trainer);

        string CreateTimesheet(string training, DateTime startSession, int duration, string location, FullName trainer, IReadOnlyList<Attendee> participants);

        string CreateSurvey(FullName trainer, string training);

        string CreateFreeAgreement(string agreementNumber, string company, string address, string zipCode, string city, FullName contact, string training, DateTime startSession, int duration, string location, IReadOnlyList<Attendee> attendees);

        string CreatePaidAgreement(string agreementNumber, string company, string address, string zipCode, string city, FullName contact, string training, DateTime startSession, int duration, string location, IReadOnlyList<Attendee> attendees, AgreementPriceType priceType, decimal price);

        string CreateFirstPage(string training, DateTime startSession, string company, FullName contact, string address, string zipCode, string city, string signature);
    }

    public class Attendee
    {
        public Attendee(FullName student, string company)
        {
            Student = student;
            Company = company;
        }
        public FullName Student { get; }
        public string Company { get; }
    }
}