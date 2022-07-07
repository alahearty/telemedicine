using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Events.HospitalEvent;
public class HospitalCompleted : BaseEvent
{
    public HospitalCompleted(Hospital item)
    {
        Item = item;
    }

    public Hospital Item { get; }
}
