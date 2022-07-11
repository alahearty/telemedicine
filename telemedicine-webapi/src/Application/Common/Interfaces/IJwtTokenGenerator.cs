using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Application.Common.Interfaces;
public interface IJwtTokenGenerator
{
    string GenerateJwtToken(Guid userId, string firstName, string lastName);
}
