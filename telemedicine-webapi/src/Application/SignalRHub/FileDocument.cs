using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.SignalRHub
{
    public class FileDocument
    {
        public List<IFormFile> Files { get; set; }
    }
}
