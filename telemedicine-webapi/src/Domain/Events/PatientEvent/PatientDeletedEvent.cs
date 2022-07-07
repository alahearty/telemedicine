using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Events.PatientEvent;
internal class PatientDeletedEvent : BaseEvent
{
    public PatientDeletedEvent(Patient item)
    {
        Item = item;
    }

    public Patient Item { get; }
}