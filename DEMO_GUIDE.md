# Telemedicine Platform - Academic Demonstration Guide

## 1. What problem does the software solve?

The platform allows patients to find hospitals and physicians, book a remote consultation, complete
a simulated sandbox payment and view their appointments. Physicians can review and accept requests;
administrators can manage directories and view dashboard data.

## 2. Why does the problem matter?

Travel to a clinic is difficult for people in remote areas. A telemedicine workflow demonstrates how
identity, role-based access, scheduling, payments and clinical data can be combined safely in a
web application.

## 3. Main objectives

- Provide role-based registration/login for Administrator, Physician and Patient.
- Manage hospitals, physicians, patients and telemedicine services.
- Let patients create a profile, select a service, choose a date/time and physician, and pay through
  a clearly labelled simulated flow.
- Create and persist appointments with correct patient/physician display names.
- Let physicians view/accept appointments and let patients cancel their own appointments.
- Protect patient and appointment data with per-user authorization.
- Provide dashboards for all three roles.

## 4. Technologies used

| Concern | Technology |
|---|---|
| Backend | ASP.NET Core 10, MediatR, AutoMapper, FluentValidation |
| Identity | ASP.NET Core Identity, JWT bearer, custom permission policies |
| Persistence | EF Core 10 + PostgreSQL (Npgsql) |
| Realtime | SignalR chat hub |
| Front end | Vue 3, Vuex 4, Vue Router 4, Vue CLI 5, Tailwind, PrimeVue, ApexCharts |
| Tests | NUnit/FluentAssertions, Respawn integration tests (23 tests) |

## 5. Architecture

```
Vue SPA (telemedicine-spa, port 8080)
  -> /api through Vue CLI proxy (http://localhost:5231)
      -> ASP.NET Core API (ports 7231/5231)
          -> Controllers -> MediatR commands/queries
              -> Application validation, DTO mapping, authorization helpers
              -> Infrastructure (EF Core, Identity, repositories, memory cache)
                  -> PostgreSQL telemedicine
          -> SignalR /chat-hub
```

The API and SPA are separate processes. The API exposes Swagger at `/swagger` and health checks at
`/health`.

## 6. Database design

| Entity | Purpose | Main fields |
|---|---|---|
| `AspNetUsers`/roles | Identity accounts | Id, Email, PasswordHash, role |
| `User` (TPH) | Patient/Physician profiles | FirstName, LastName, Email, Phone, Gender, Birth, License/Specialization |
| `Hospital` | Hospital directory | HospitalName, Address |
| `TelemedicineService` | Bookable service | ServiceName, Cost, Description |
| `TelemedicinePayment` | Simulated payment | AmountPaid, IsPaid, Service |
| `Appointment` | Consultation request | PatientId, PhysicianId, Service, AppointmentDate, Status |
| `HealthAnalysisReport`/`Comment`/`Message` | Clinical/chat features | template-linked fields |

Appointment status values: `Active`, `NotAttended`, `Attended`, `Cancelled` (serialized as numeric
enum values; the SPA maps them to readable labels).

## 7. Main modules

- Authentication and role policies.
- Admin hospital and service management.
- Patient profile and directory.
- Booking page (service, date, time, physician).
- Sandbox payment and appointment creation.
- Patient appointment dashboard and cancellation.
- Physician dashboard, appointments, unattended requests and patient list.
- Admin analytics dashboard.
- SignalR chat endpoints.

## 8. Demonstration sequence

Use the synthetic demo data:

| Role | Email | Password |
|---|---|---|
| Administrator | `administrator@localhost` | `Administrator1!` |
| Patient | `patient@demo.local` | `Patient@12345` |
| Physician | `physician@demo.local` | `Physician@12345` |

1. Start the API and SPA.
2. Sign in as the patient. Open **Book Appointment**.
3. Select **General Consultation**, choose a future date and time, and select **Dr. Demo**.
4. Continue to payment. Point out the **Sandbox / simulated payment** notice and that no card details
   are collected.
5. Click **Pay $500.00 (demo)**. Show the confirmation screen with the appointment reference.
6. Return to the dashboard and show the appointment with its service, date and status.
7. Sign in as the physician and open **Unattended**. Accept the new request.
8. Show the appointment becomes `Active` and is assigned to Dr. Demo.
9. As the physician, open **Patients** and show the patient from the appointment.
10. As the patient, cancel an appointment from the dashboard and show it disappears from upcoming
    appointments.
11. Sign in as the administrator and show the hospital/service management and dashboard.
12. Optional security check: a second patient cannot fetch the first patient's appointment or
    profile (403).

## 9. Data flow

1. Login validates the Identity password and returns a JWT containing `Role`, `uid` and `email`.
2. Vue stores the token and role in `localStorage`; the router enforces role-specific areas.
3. Booking stores the selected service/date/time/doctor in `sessionStorage` and routes to payment.
4. Payment calls `POST /api/TelemedicinePayment`, which records a simulated payment and returns a
   single-use token cached in memory.
5. The SPA calls `POST /api/Appointment` with the token, date and physician. The API validates the
   token, resolves the patient profile, assigns the physician, stores the appointment and consumes
   the token.
6. Queries use EF Core `Include` to return patient, physician and service display names.
7. Appointment access checks compare the JWT role and email with the appointment's patient or
   physician; administrators are allowed, other users receive 403.

## 10. Limitations and future improvements

- This is not a clinical system. It has no real video, prescriptions, regulatory compliance, audit
  trail or encryption at rest. Never enter real patient data.
- Payment is simulated. No gateway, card storage or real charge exists.
- Some analytics cards use template/sample values.
- Legacy Todo endpoints and some template files remain.
- Future work: real video consultation, encrypted clinical records, prescription workflow, audit
  logging, notification service and production payment integration.
