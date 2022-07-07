using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Events.HospitalEvent;
internal class HospitalDeleteEvent : BaseEvent
{
    public HospitalDeleteEvent(Hospital item)
    {
        Item = item;
    }

    public Hospital Item { get; }
}