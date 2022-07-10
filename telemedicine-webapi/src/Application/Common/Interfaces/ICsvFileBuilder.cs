using telemedicine_webapi.Application.Hospitals.Queries.ExportHospitals;
using telemedicine_webapi.Application.Patients.Queries.ExportPatients;
using telemedicine_webapi.Application.TodoLists.Queries.ExportTodos;

namespace telemedicine_webapi.Application.Common.Interfaces;

public interface ICsvFileBuilder
{
    byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records);
    byte[] BuildHostpitalsFile(IEnumerable<HospitalRecord> records);
    byte[] BuildPatientsFile(IEnumerable<PatientFileRecord> records);
    byte[] BuildPhysiciansFile(IEnumerable<TodoItemRecord> records);
}
