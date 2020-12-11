﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Retro_Gamer.Models;

namespace Retro_Gamer.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20200723164039_SeedGames")]
    partial class SeedGames
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Retro_Gamer.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(220)")
                        .HasMaxLength(220);

                    b.Property<int>("Genres")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<DateTime>("RelaseDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("DbGame");

                    b.HasData(
                        new
                        {
                            Id = 3,
                            Description = "Super Mario game inc",
                            Genres = 4,
                            Name = "Super Mario",
                            Rating = 10,
                            RelaseDate = new DateTime(2020, 7, 23, 16, 40, 38, 713, DateTimeKind.Local).AddTicks(2546)
                        });
                });
#pragma warning restore 612, 618
        }
    }
}