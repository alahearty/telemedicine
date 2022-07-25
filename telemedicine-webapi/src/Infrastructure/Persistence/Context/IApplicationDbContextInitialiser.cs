using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Infrastructure.Persistence.Context;
public interface IApplicationDbContextInitialiser
{
    Task InitialiseAsync();
    Task SeedAsync();
}
