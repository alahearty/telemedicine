﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using telemedicine_webapi.Infrastructure.Persistence.Context;

#nullable disable

namespace telemedicine_webapi.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Domain.Entities.Message", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MessageText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReceiverUserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SenderUserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SenderUserId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("PatientPhysician", b =>
                {
                    b.Property<int>("DoctorsId")
                        .HasColumnType("int");

                    b.Property<int>("PatientsId")
                        .HasColumnType("int");

                    b.HasKey("DoctorsId", "PatientsId");

                    b.HasIndex("PatientsId");

                    b.ToTable("PatientPhysician");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.Appointment", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("AppointmentScheduleId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PatientId")
                        .HasColumnType("int");

                    b.Property<int?>("TelemedicineServiceId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AppointmentScheduleId");

                    b.HasIndex("PatientId");

                    b.HasIndex("TelemedicineServiceId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.Comment", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("HealthAnalysisReportId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PatientId")
                        .HasColumnType("int");

                    b.Property<int?>("PhysicianId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("HealthAnalysisReportId");

                    b.HasIndex("PatientId");

                    b.HasIndex("PhysicianId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.HealthAnalysisReport", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HeartRate")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastMeasurement")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PatientId")
                        .HasColumnType("int");

                    b.Property<string>("Pressure")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Temp")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("PatientId");

                    b.ToTable("HealthAnalysisReport");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.Hospital", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HospitalName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Hospitals");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.PhysicianPatientTransaction", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("CommentId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PatientId")
                        .HasColumnType("int");

                    b.Property<int?>("PhysicianId")
                        .HasColumnType("int");

                    b.Property<int?>("ServiceId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CommentId");

                    b.HasIndex("PatientId");

                    b.HasIndex("PhysicianId");

                    b.HasIndex("ServiceId");

                    b.ToTable("PhysianPatientTransactions");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.ScheduleTime", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EndScheduleTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PhysicianId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("StartScheduleTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("PhysicianId");

                    b.ToTable("ScheduleTimes");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.TelemedicinePayment", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsPaid")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PaymentId")
                        .HasColumnType("int");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PaymentId");

                    b.ToTable("TelemedicinePayments");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.TelemedicineService", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServiceName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TelemedicineServices");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.TodoItem", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Done")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ListId")
                        .HasColumnType("int");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Reminder")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("ListId");

                    b.ToTable("TodoItems");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.TodoList", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("TodoLists");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("AccountType")
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Avatar")
                        .HasColumnType("varbinary(max)");

                    b.Property<DateTime>("Birth")
                        .HasColumnType("Date");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("User");

                    b.HasDiscriminator<string>("Discriminator").HasValue("User");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.Patient", b =>
                {
                    b.HasBaseType("telemedicine_webapi.Domain.Entities.User");

                    b.Property<Guid>("DeviceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsArchive")
                        .HasColumnType("bit")
                        .HasColumnName("Patient_IsArchive");

                    b.Property<string>("Patronimic")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("Patient");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.Physician", b =>
                {
                    b.HasBaseType("telemedicine_webapi.Domain.Entities.User");

                    b.Property<int?>("HospitalId")
                        .HasColumnType("int");

                    b.Property<bool>("IsArchive")
                        .HasColumnType("bit");

                    b.Property<string>("License")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MedicalSpecialization")
                        .HasColumnType("nvarchar(max)");

                    b.HasIndex("HospitalId");

                    b.HasDiscriminator().HasValue("Physician");
                });

            modelBuilder.Entity("Domain.Entities.Message", b =>
                {
                    b.HasOne("telemedicine_webapi.Domain.Entities.User", "AppUser")
                        .WithMany("Messages")
                        .HasForeignKey("SenderUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PatientPhysician", b =>
                {
                    b.HasOne("telemedicine_webapi.Domain.Entities.Physician", null)
                        .WithMany()
                        .HasForeignKey("DoctorsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("telemedicine_webapi.Domain.Entities.Patient", null)
                        .WithMany()
                        .HasForeignKey("PatientsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.Appointment", b =>
                {
                    b.HasOne("telemedicine_webapi.Domain.Entities.ScheduleTime", "AppointmentSchedule")
                        .WithMany()
                        .HasForeignKey("AppointmentScheduleId");

                    b.HasOne("telemedicine_webapi.Domain.Entities.Patient", "Patient")
                        .WithMany()
                        .HasForeignKey("PatientId");

                    b.HasOne("telemedicine_webapi.Domain.Entities.TelemedicineService", "TelemedicineService")
                        .WithMany()
                        .HasForeignKey("TelemedicineServiceId");

                    b.Navigation("AppointmentSchedule");

                    b.Navigation("Patient");

                    b.Navigation("TelemedicineService");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.Comment", b =>
                {
                    b.HasOne("telemedicine_webapi.Domain.Entities.HealthAnalysisReport", "HealthAnalysisReport")
                        .WithMany("Comments")
                        .HasForeignKey("HealthAnalysisReportId");

                    b.HasOne("telemedicine_webapi.Domain.Entities.Patient", null)
                        .WithMany("Comments")
                        .HasForeignKey("PatientId");

                    b.HasOne("telemedicine_webapi.Domain.Entities.Physician", "Physician")
                        .WithMany()
                        .HasForeignKey("PhysicianId");

                    b.Navigation("HealthAnalysisReport");

                    b.Navigation("Physician");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.HealthAnalysisReport", b =>
                {
                    b.HasOne("telemedicine_webapi.Domain.Entities.Patient", "Patient")
                        .WithMany("Analyzes")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.PhysicianPatientTransaction", b =>
                {
                    b.HasOne("telemedicine_webapi.Domain.Entities.Comment", "Comment")
                        .WithMany()
                        .HasForeignKey("CommentId");

                    b.HasOne("telemedicine_webapi.Domain.Entities.Patient", "Patient")
                        .WithMany()
                        .HasForeignKey("PatientId");

                    b.HasOne("telemedicine_webapi.Domain.Entities.Physician", "Physician")
                        .WithMany()
                        .HasForeignKey("PhysicianId");

                    b.HasOne("telemedicine_webapi.Domain.Entities.TelemedicineService", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId");

                    b.Navigation("Comment");

                    b.Navigation("Patient");

                    b.Navigation("Physician");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.ScheduleTime", b =>
                {
                    b.HasOne("telemedicine_webapi.Domain.Entities.Physician", "Physician")
                        .WithMany()
                        .HasForeignKey("PhysicianId");

                    b.Navigation("Physician");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.TelemedicinePayment", b =>
                {
                    b.HasOne("telemedicine_webapi.Domain.Entities.PhysicianPatientTransaction", "Payment")
                        .WithMany()
                        .HasForeignKey("PaymentId");

                    b.Navigation("Payment");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.TodoItem", b =>
                {
                    b.HasOne("telemedicine_webapi.Domain.Entities.TodoList", "List")
                        .WithMany("Items")
                        .HasForeignKey("ListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("List");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.TodoList", b =>
                {
                    b.OwnsOne("telemedicine_webapi.Domain.ValueObjects.Colour", "Colour", b1 =>
                        {
                            b1.Property<int>("TodoListId")
                                .HasColumnType("int");

                            b1.Property<string>("Code")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("TodoListId");

                            b1.ToTable("TodoLists");

                            b1.WithOwner()
                                .HasForeignKey("TodoListId");
                        });

                    b.Navigation("Colour")
                        .IsRequired();
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.Physician", b =>
                {
                    b.HasOne("telemedicine_webapi.Domain.Entities.Hospital", "Hospital")
                        .WithMany("Doctors")
                        .HasForeignKey("HospitalId");

                    b.Navigation("Hospital");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.HealthAnalysisReport", b =>
                {
                    b.Navigation("Comments");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.Hospital", b =>
                {
                    b.Navigation("Doctors");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.TodoList", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.User", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("telemedicine_webapi.Domain.Entities.Patient", b =>
                {
                    b.Navigation("Analyzes");

                    b.Navigation("Comments");
                });
#pragma warning restore 612, 618
        }
    }
}
