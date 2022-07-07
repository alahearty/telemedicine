using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Events.PatientEvent;
internal class PatientCreatedEvent : BaseEvent
{
    public PatientCreatedEvent(Patient item)
    {
        Item = item;
    }

    public Patient Item { get; }
}