﻿// <auto-generated />
using System;
using ContactManager.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ContactManager.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210520115752_AddFavourites")]
    partial class AddFavourites
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ContactManager.Models.Character", b =>
                {
                    b.Property<int>("CharacterID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Introduction")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("Kind")
                        .HasColumnType("int");

                    b.Property<int>("Moral")
                        .HasColumnType("int");

                    b.Property<string>("OwnerID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("ProfileImage")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("RealName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("CharacterID");

                    b.ToTable("Character");
                });

            modelBuilder.Entity("ContactManager.Models.CharacterPowerEntry", b =>
                {
                    b.Property<int>("CharacterID")
                        .HasColumnType("int");

                    b.Property<int>("PowerID")
                        .HasColumnType("int");

                    b.Property<int?>("Level")
                        .HasColumnType("int");

                    b.Property<string>("Specification")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CharacterID", "PowerID");

                    b.HasIndex("PowerID");

                    b.ToTable("CharacterPowerEntry");
                });

            modelBuilder.Entity("ContactManager.Models.Favourite", b =>
                {
                    b.Property<int>("FavouriteID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("FavType")
                        .HasColumnType("int");

                    b.Property<string>("OwnerID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SelectedFavID")
                        .HasColumnType("int");

                    b.HasKey("FavouriteID");

                    b.ToTable("Favourite");
                });

            modelBuilder.Entity("ContactManager.Models.OpponentEntry", b =>
                {
                    b.Property<int>("OpponentEntryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CharacterID")
                        .HasColumnType("int");

                    b.Property<int?>("OpponentCharacterID")
                        .HasColumnType("int");

                    b.Property<int?>("OpponentTeamID")
                        .HasColumnType("int");

                    b.Property<int?>("TeamID")
                        .HasColumnType("int");

                    b.HasKey("OpponentEntryID");

                    b.HasIndex("CharacterID");

                    b.HasIndex("TeamID");

                    b.ToTable("OpponentEntry");
                });

            modelBuilder.Entity("ContactManager.Models.Power", b =>
                {
                    b.Property<int>("PowerID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("OwnerID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PowerDescription")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PowerName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("PowerType")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("PowerID");

                    b.ToTable("Power");
                });

            modelBuilder.Entity("ContactManager.Models.Team", b =>
                {
                    b.Property<int>("TeamID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Introduction")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("OwnerID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("ProfileImage")
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("Side")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TeamMotto")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("TeamName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("TeamID");

                    b.ToTable("Team");
                });

            modelBuilder.Entity("ContactManager.Models.TeamEntry", b =>
                {
                    b.Property<int>("TeamID")
                        .HasColumnType("int");

                    b.Property<int>("CharacterID")
                        .HasColumnType("int");

                    b.Property<int?>("Role")
                        .HasColumnType("int");

                    b.HasKey("TeamID", "CharacterID");

                    b.HasIndex("CharacterID");

                    b.ToTable("TeamEntry");
                });

            modelBuilder.Entity("ContactManager.Models.Universe", b =>
                {
                    b.Property<int>("UniverseID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Creator")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Genre")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Origin")
                        .HasColumnType("int");

                    b.Property<string>("OwnerID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("ProfileImage")
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("UniverseID");

                    b.ToTable("Universe");
                });

            modelBuilder.Entity("ContactManager.Models.UniverseEntry", b =>
                {
                    b.Property<int>("UniverseEntryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CharacterID")
                        .HasColumnType("int");

                    b.Property<int>("NewMember")
                        .HasColumnType("int");

                    b.Property<int?>("TeamID")
                        .HasColumnType("int");

                    b.Property<int>("UniverseID")
                        .HasColumnType("int");

                    b.HasKey("UniverseEntryID");

                    b.HasIndex("CharacterID");

                    b.HasIndex("TeamID");

                    b.HasIndex("UniverseID");

                    b.ToTable("UniverseEntry");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

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

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

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

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("ContactManager.Models.CharacterPowerEntry", b =>
                {
                    b.HasOne("ContactManager.Models.Character", "Character")
                        .WithMany("CharacterPowerEntries")
                        .HasForeignKey("CharacterID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ContactManager.Models.Power", "Power")
                        .WithMany("CharacterPowerEntries")
                        .HasForeignKey("PowerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Character");

                    b.Navigation("Power");
                });

            modelBuilder.Entity("ContactManager.Models.OpponentEntry", b =>
                {
                    b.HasOne("ContactManager.Models.Character", "Character")
                        .WithMany("OpponentEntries")
                        .HasForeignKey("CharacterID");

                    b.HasOne("ContactManager.Models.Team", "Team")
                        .WithMany("OpponentEntries")
                        .HasForeignKey("TeamID");

                    b.Navigation("Character");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("ContactManager.Models.TeamEntry", b =>
                {
                    b.HasOne("ContactManager.Models.Character", "Character")
                        .WithMany("TeamEntries")
                        .HasForeignKey("CharacterID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ContactManager.Models.Team", "Team")
                        .WithMany("TeamEntries")
                        .HasForeignKey("TeamID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Character");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("ContactManager.Models.UniverseEntry", b =>
                {
                    b.HasOne("ContactManager.Models.Character", "Character")
                        .WithMany("UniverseEntries")
                        .HasForeignKey("CharacterID");

                    b.HasOne("ContactManager.Models.Team", "Team")
                        .WithMany("UniverseEntries")
                        .HasForeignKey("TeamID");

                    b.HasOne("ContactManager.Models.Universe", "Universe")
                        .WithMany("UniverseEntries")
                        .HasForeignKey("UniverseID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Character");

                    b.Navigation("Team");

                    b.Navigation("Universe");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ContactManager.Models.Character", b =>
                {
                    b.Navigation("CharacterPowerEntries");

                    b.Navigation("OpponentEntries");

                    b.Navigation("TeamEntries");

                    b.Navigation("UniverseEntries");
                });

            modelBuilder.Entity("ContactManager.Models.Power", b =>
                {
                    b.Navigation("CharacterPowerEntries");
                });

            modelBuilder.Entity("ContactManager.Models.Team", b =>
                {
                    b.Navigation("OpponentEntries");

                    b.Navigation("TeamEntries");

                    b.Navigation("UniverseEntries");
                });

            modelBuilder.Entity("ContactManager.Models.Universe", b =>
                {
                    b.Navigation("UniverseEntries");
                });
#pragma warning restore 612, 618
        }
    }
}
