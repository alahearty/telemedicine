using System.ComponentModel.DataAnnotations.Schema;

namespace telemedicine_webapi.Domain.Entities
{
    [ComplexType]
    public class Pressure
    {
        public int Sys { get; set; }
        public int Dia { get; set; }
    }
}