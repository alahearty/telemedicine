using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Application.Common.Models.Authentication;

public record AuthenticationResponse(int Id, string Token);

