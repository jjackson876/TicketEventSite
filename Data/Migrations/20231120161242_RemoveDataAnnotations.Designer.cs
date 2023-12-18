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
    [Migration("20231120161242_RemoveDataAnnotations")]
    partial class RemoveDataAnnotations
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
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

                    b.Property<bool?>("IsNotPurchasable")
                        .HasColumnType("bit");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int?>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EventListingId");

                    b.ToTable("Admissions");
                });

            modelBuilder.Entity("EventsAPI.Models.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CCardCVV")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CCardExpDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CCardNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("EventListingId")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EventListingId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("EventsAPI.Models.BoughtTicket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BookingId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<double>("SubTotal")
                        .HasColumnType("float");

                    b.Property<string>("TicketType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BookingId");

                    b.ToTable("BoughtTickets");
                });

            modelBuilder.Entity("EventsAPI.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("EventsAPI.Models.EvHeader", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

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

                    b.Property<string>("Permit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PromoImage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PromoImage2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PromoImage3")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

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
                    b.HasOne("EventsAPI.Models.EvHeader", "EvHeader")
                        .WithMany("Admissions")
                        .HasForeignKey("EventListingId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("EvHeader");
                });

            modelBuilder.Entity("EventsAPI.Models.Booking", b =>
                {
                    b.HasOne("EventsAPI.Models.EvHeader", "EvHeader")
                        .WithMany()
                        .HasForeignKey("EventListingId");

                    b.Navigation("EvHeader");
                });

            modelBuilder.Entity("EventsAPI.Models.BoughtTicket", b =>
                {
                    b.HasOne("EventsAPI.Models.Booking", "Booking")
                        .WithMany()
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Booking");
                });

            modelBuilder.Entity("EventsAPI.Models.EvHeader", b =>
                {
                    b.HasOne("EventsAPI.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("EventsAPI.Models.Music", b =>
                {
                    b.HasOne("EventsAPI.Models.EvHeader", "EventListing")
                        .WithMany("Music")
                        .HasForeignKey("EventListingId");

                    b.Navigation("EventListing");
                });

            modelBuilder.Entity("EventsAPI.Models.Outlet", b =>
                {
                    b.HasOne("EventsAPI.Models.EvHeader", "EventListing")
                        .WithMany("Outlet")
                        .HasForeignKey("EventListingId");

                    b.Navigation("EventListing");
                });

            modelBuilder.Entity("EventsAPI.Models.Sponsor", b =>
                {
                    b.HasOne("EventsAPI.Models.EvHeader", "EventListing")
                        .WithMany("Sponsor")
                        .HasForeignKey("EventListingId");

                    b.Navigation("EventListing");
                });

            modelBuilder.Entity("EventsAPI.Models.EvHeader", b =>
                {
                    b.Navigation("Admissions");

                    b.Navigation("Music");

                    b.Navigation("Outlet");

                    b.Navigation("Sponsor");
                });
#pragma warning restore 612, 618
        }
    }
}