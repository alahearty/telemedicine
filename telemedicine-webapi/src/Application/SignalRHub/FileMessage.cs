﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.SignalRHub
{
    public class FileMessage
    {
        public string FileHeader { get; set; }
        public byte[] FileBinary { get; set; }
    }
}
