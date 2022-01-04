﻿// <auto-generated />
using System;
using ET.WebAPI.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ET.WebAPI.Database.Migrations
{
    [DbContext(typeof(ApiDbContext))]
    [Migration("20211224104224_changed_data_structure")]
    partial class changed_data_structure
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ET.WebAPI.Database.Entities.Device", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Latitude")
                        .HasPrecision(7, 5)
                        .HasColumnType("decimal(7,5)");

                    b.Property<decimal>("Longitude")
                        .HasPrecision(8, 5)
                        .HasColumnType("decimal(8,5)");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("SensorName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("ET.WebAPI.Database.Entities.NumericReading", b =>
                {
                    b.Property<int>("ReadingType")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("datetimeoffset");

                    b.Property<double>("Value")
                        .HasColumnType("float");

                    b.Property<Guid>("DeviceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ReadingType", "Timestamp", "Value", "DeviceId")
                        .IsClustered();

                    b.HasIndex("DeviceId");

                    b.ToTable("NumericReadings");
                });

            modelBuilder.Entity("ET.WebAPI.Database.Entities.NumericReading", b =>
                {
                    b.HasOne("ET.WebAPI.Database.Entities.Device", "Device")
                        .WithMany("NumericReadings")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Device");
                });

            modelBuilder.Entity("ET.WebAPI.Database.Entities.Device", b =>
                {
                    b.Navigation("NumericReadings");
                });
#pragma warning restore 612, 618
        }
    }
}
