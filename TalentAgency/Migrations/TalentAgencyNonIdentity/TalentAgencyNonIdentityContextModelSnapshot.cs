﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TalentAgency.Data;

namespace TalentAgency.Migrations.TalentAgencyNonIdentity
{
    [DbContext(typeof(TalentAgencyNonIdentityContext))]
    partial class TalentAgencyNonIdentityContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.25")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TalentAgency.Models.Apply", b =>
                {
                    b.Property<string>("Apply_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Date_created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Event_id")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Introduction")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Talent_id")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Apply_id");

                    b.ToTable("Apply");
                });

            modelBuilder.Entity("TalentAgency.Models.Event", b =>
                {
                    b.Property<string>("Event_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Event_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("date_created")
                        .HasColumnType("datetime2");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("email")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Event_id");

                    b.ToTable("Event");
                });
#pragma warning restore 612, 618
        }
    }
}
