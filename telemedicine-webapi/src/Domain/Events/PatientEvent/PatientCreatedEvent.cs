using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Events.PatientEvent;
internal class PatientCreatedEvent : BaseEvent
{
    public PatientCreatedEvent(Patient patient)
    {
        Patient = patient;
    }

    public Patient Patient { get; }
}