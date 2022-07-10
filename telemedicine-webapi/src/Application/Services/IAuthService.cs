using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telemedicine_webapi.Application.Common.Models.Authentication;

namespace telemedicine_webapi.Application.Services;
public interface IAuthService
{
    AuthenticationResult Login(string email, string password);
    AuthenticationResult Register(string firstName, string lastName, string email, string password);
}
