﻿// <auto-generated />
using System;
using Golbaus_BE.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Golbaus_BE.Migrations
{
    [DbContext(typeof(ApiDbContext))]
    [Migration("20231229091925_Add_Table_Posts_Comments_Tags_PostTagMaps_UserFollowMaps")]
    partial class Add_Table_Posts_Comments_Tags_PostTagMaps_UserFollowMaps
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Golbaus_BE.Entities.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Content")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("DownVote")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("PostId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Remark")
                        .HasColumnType("longtext");

                    b.Property<string>("ReplyFor")
                        .HasColumnType("longtext");

                    b.Property<int>("UpVote")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Golbaus_BE.Entities.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Content")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("DownVote")
                        .HasColumnType("int");

                    b.Property<string>("Excerpt")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("PublishDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("PublishType")
                        .HasColumnType("int");

                    b.Property<string>("Remark")
                        .HasColumnType("longtext");

                    b.Property<string>("Thumbnail")
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .HasColumnType("longtext");

                    b.Property<int>("UpVote")
                        .HasColumnType("int");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<int>("ViewCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Golbaus_BE.Entities.PostTagMap", b =>
                {
                    b.Property<Guid>("PostId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("TagId")
                        .HasColumnType("char(36)");

                    b.HasKey("PostId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("PostTagMaps");
                });

            modelBuilder.Entity("Golbaus_BE.Entities.Role", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "90eff107-f2ce-4068-9568-30edacbc8214",
                            ConcurrencyStamp = "fa28fea7-add7-4636-9bb0-bd162d7ce2aa",
                            Name = "SuperAdmin",
                            NormalizedName = "SUPERADMIN"
                        },
                        new
                        {
                            Id = "8bfb5745-ba0f-4011-aace-6e75e8502cdf",
                            ConcurrencyStamp = "bb25e5b9-8212-409f-8196-54408a0904f2",
                            Name = "Admin",
                            NormalizedName = "Admin"
                        },
                        new
                        {
                            Id = "0d37bd4f-6550-4dbc-8894-23b571194be4",
                            ConcurrencyStamp = "f1fe13ac-2edc-4e98-9272-3bce3b6c4240",
                            Name = "User",
                            NormalizedName = "User"
                        });
                });

            modelBuilder.Entity("Golbaus_BE.Entities.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Golbaus_BE.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("Avatar")
                        .HasColumnType("longtext");

                    b.Property<string>("Bio")
                        .HasColumnType("longtext");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("DateJoined")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DoB")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("FullName")
                        .HasColumnType("longtext");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("PostCount")
                        .HasColumnType("int");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "4bfcc5f7-5bdc-4827-a909-4f04ac5770ff",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "71229555-966a-4b46-ba94-06a26b8b0fd6",
                            DateJoined = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "admin@gmail.com",
                            EmailConfirmed = true,
                            FullName = "Admin",
                            Gender = 0,
                            LockoutEnabled = false,
                            NormalizedEmail = "ADMIN@GMAIL.COM",
                            NormalizedUserName = "ADMIN@GMAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEPfKFjAJUx/nsX8aO/1Y8XHo4PUw+PZjg0JaaQmTgsYL2G9BQV1SuxQRZ7OmGPkXhQ==",
                            PhoneNumberConfirmed = false,
                            PostCount = 0,
                            SecurityStamp = "",
                            TwoFactorEnabled = false,
                            UserName = "admin"
                        },
                        new
                        {
                            Id = "0f82194c-dc6a-45ff-8ad2-5a6ea82be10f",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "0df00d6b-0cdc-4eaf-948f-d3cdbbe92b44",
                            DateJoined = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "superadmin@gmail.com",
                            EmailConfirmed = true,
                            FullName = "Super admin",
                            Gender = 0,
                            LockoutEnabled = false,
                            NormalizedEmail = "SUPERADMIN@GMAIL.COM",
                            NormalizedUserName = "SUPERADMIN@GMAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEHwZ+TnNqx/YyJl5a3w1xsfo5yssGxUU+gWLAzEiEjsnm+r653+dssCK5ryQKyXFGw==",
                            PhoneNumberConfirmed = false,
                            PostCount = 0,
                            SecurityStamp = "",
                            TwoFactorEnabled = false,
                            UserName = "superadmin"
                        });
                });

            modelBuilder.Entity("Golbaus_BE.Entities.UserFollowMap", b =>
                {
                    b.Property<string>("FollowerId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("FollowingId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("FollowerId", "FollowingId");

                    b.HasIndex("FollowingId");

                    b.ToTable("UserFollowMaps");
                });

            modelBuilder.Entity("Golbaus_BE.Entities.UserRoleMap", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = "0f82194c-dc6a-45ff-8ad2-5a6ea82be10f",
                            RoleId = "90eff107-f2ce-4068-9568-30edacbc8214"
                        },
                        new
                        {
                            UserId = "4bfcc5f7-5bdc-4827-a909-4f04ac5770ff",
                            RoleId = "8bfb5745-ba0f-4011-aace-6e75e8502cdf"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Golbaus_BE.Entities.Comment", b =>
                {
                    b.HasOne("Golbaus_BE.Entities.Comment", "Parent")
                        .WithMany("Childs")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Golbaus_BE.Entities.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Golbaus_BE.Entities.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Parent");

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Golbaus_BE.Entities.Post", b =>
                {
                    b.HasOne("Golbaus_BE.Entities.User", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Golbaus_BE.Entities.PostTagMap", b =>
                {
                    b.HasOne("Golbaus_BE.Entities.Post", "Post")
                        .WithMany("PostTagMaps")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Golbaus_BE.Entities.Tag", "Tag")
                        .WithMany("PostTagMaps")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("Golbaus_BE.Entities.UserFollowMap", b =>
                {
                    b.HasOne("Golbaus_BE.Entities.User", "Follower")
                        .WithMany("UserFollowerMaps")
                        .HasForeignKey("FollowerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Golbaus_BE.Entities.User", "Following")
                        .WithMany("UserFollowingMaps")
                        .HasForeignKey("FollowingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Follower");

                    b.Navigation("Following");
                });

            modelBuilder.Entity("Golbaus_BE.Entities.UserRoleMap", b =>
                {
                    b.HasOne("Golbaus_BE.Entities.Role", "Role")
                        .WithMany("UserRoleMaps")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Golbaus_BE.Entities.User", "User")
                        .WithMany("UserRoleMaps")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Golbaus_BE.Entities.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Golbaus_BE.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Golbaus_BE.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Golbaus_BE.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Golbaus_BE.Entities.Comment", b =>
                {
                    b.Navigation("Childs");
                });

            modelBuilder.Entity("Golbaus_BE.Entities.Post", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("PostTagMaps");
                });

            modelBuilder.Entity("Golbaus_BE.Entities.Role", b =>
                {
                    b.Navigation("UserRoleMaps");
                });

            modelBuilder.Entity("Golbaus_BE.Entities.Tag", b =>
                {
                    b.Navigation("PostTagMaps");
                });

            modelBuilder.Entity("Golbaus_BE.Entities.User", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Posts");

                    b.Navigation("UserFollowerMaps");

                    b.Navigation("UserFollowingMaps");

                    b.Navigation("UserRoleMaps");
                });
#pragma warning restore 612, 618
        }
    }
}
