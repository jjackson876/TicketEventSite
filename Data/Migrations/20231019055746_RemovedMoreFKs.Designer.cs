﻿// <auto-generated />
using System;
using EventsAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EventsAPI.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231019055746_RemovedMoreFKs")]
    partial class RemovedMoreFKs
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EventsAPI.Models.Admission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AdmissionType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("EventListingId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("EventListingId");

                    b.ToTable("Admissions");
                });

            modelBuilder.Entity("EventsAPI.Models.EventListing", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("EventDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("EventDesc")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EventLocation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("EventsAPI.Models.Music", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("EventListingId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("MusicProvider")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EventListingId");

                    b.ToTable("Musics");
                });

            modelBuilder.Entity("EventsAPI.Models.Outlet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("EventListingId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("OutletName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EventListingId");

                    b.ToTable("Outlets");
                });

            modelBuilder.Entity("EventsAPI.Models.Sponsor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("EventListingId")
                        .HasColumnType("int");

                    b.Property<string>("EventSponsor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("EventListingId");

                    b.ToTable("Sponsors");
                });

            modelBuilder.Entity("EventsAPI.Models.Admission", b =>
                {
                    b.HasOne("EventsAPI.Models.EventListing", "EventListing")
                        .WithMany("Admission")
                        .HasForeignKey("EventListingId");

                    b.Navigation("EventListing");
                });

            modelBuilder.Entity("EventsAPI.Models.Music", b =>
                {
                    b.HasOne("EventsAPI.Models.EventListing", "EventListing")
                        .WithMany("Music")
                        .HasForeignKey("EventListingId");

                    b.Navigation("EventListing");
                });

            modelBuilder.Entity("EventsAPI.Models.Outlet", b =>
                {
                    b.HasOne("EventsAPI.Models.EventListing", "EventListing")
                        .WithMany("Outlet")
                        .HasForeignKey("EventListingId");

                    b.Navigation("EventListing");
                });

            modelBuilder.Entity("EventsAPI.Models.Sponsor", b =>
                {
                    b.HasOne("EventsAPI.Models.EventListing", "EventListing")
                        .WithMany("Sponsor")
                        .HasForeignKey("EventListingId");

                    b.Navigation("EventListing");
                });

            modelBuilder.Entity("EventsAPI.Models.EventListing", b =>
                {
                    b.Navigation("Admission");

                    b.Navigation("Music");

                    b.Navigation("Outlet");

                    b.Navigation("Sponsor");
                });
#pragma warning restore 612, 618
        }
    }
}
