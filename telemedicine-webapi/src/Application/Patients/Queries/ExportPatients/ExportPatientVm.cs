namespace telemedicine_webapi.Application.Patients.Queries.ExportPatients;

public class ExportPatientVm
{
    public ExportPatientVm(string fileName, string contentType, byte[] content)
    {
        FileName = fileName;
        ContentType = contentType;
        Content = content;
    }

    public string FileName { get; set; }

    public string ContentType { get; set; }

    public byte[] Content { get; set; }
}
