using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Events.PhysicianEvent;
internal class PhysicianUpdatedEvent : BaseEvent
{
    public PhysicianUpdatedEvent(Physician item)
    {
        Item = item;
    }

    public Physician Item { get; }
}