﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Events.PhysicianEvent;
internal class PhysicianCreatedEvent : BaseEvent
{
    public PhysicianCreatedEvent(Physician item)
    {
        Item = item;
    }

    public Physician Item { get; }
}