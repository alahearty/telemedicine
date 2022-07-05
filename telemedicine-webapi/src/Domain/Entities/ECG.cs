using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Entities;

public class ECG : BaseAuditableEntity
{

    public DateTime LastMeasurement { get; set; }

    [Required]
    public virtual HealthAnalysisReport Analyze { get; set; }

    public ECG()
    {
        Comments = new List<Comment>();
        Datas = new List<ECGData>();
    }

    public virtual ICollection<Comment> Comments { get; set; }
    public virtual ICollection<ECGData> Datas { get; set; }
}
