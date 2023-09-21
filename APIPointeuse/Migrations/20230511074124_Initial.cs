using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIPointeuse.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cycles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cycles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subsections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subsections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schoolclasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCycle = table.Column<int>(type: "int", nullable: false),
                    IdSection = table.Column<int>(type: "int", nullable: false),
                    IdSubsection = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schoolclasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schoolclasses_Cycles_IdCycle",
                        column: x => x.IdCycle,
                        principalTable: "Cycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Schoolclasses_Sections_IdSection",
                        column: x => x.IdSection,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Schoolclasses_Subsections_IdSubsection",
                        column: x => x.IdSubsection,
                        principalTable: "Subsections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Periods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BeginningPeriod = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndingPeriod = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdSchoolclass = table.Column<int>(type: "int", nullable: false),
                    IsMajor = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Periods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Periods_Schoolclasses_IdSchoolclass",
                        column: x => x.IdSchoolclass,
                        principalTable: "Schoolclasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Signature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdSchoolclass = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Schoolclasses_IdSchoolclass",
                        column: x => x.IdSchoolclass,
                        principalTable: "Schoolclasses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArrivalDateTime",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArrivalSavedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdStudent = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArrivalDateTime", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArrivalDateTime_Students_IdStudent",
                        column: x => x.IdStudent,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentsDevice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdStudent = table.Column<int>(type: "int", nullable: false),
                    System = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentsDevice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentsDevice_Students_IdStudent",
                        column: x => x.IdStudent,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArrivalDateTime_IdStudent",
                table: "ArrivalDateTime",
                column: "IdStudent");

            migrationBuilder.CreateIndex(
                name: "IX_Periods_IdSchoolclass",
                table: "Periods",
                column: "IdSchoolclass");

            migrationBuilder.CreateIndex(
                name: "IX_Schoolclasses_IdCycle",
                table: "Schoolclasses",
                column: "IdCycle");

            migrationBuilder.CreateIndex(
                name: "IX_Schoolclasses_IdSection",
                table: "Schoolclasses",
                column: "IdSection");

            migrationBuilder.CreateIndex(
                name: "IX_Schoolclasses_IdSubsection",
                table: "Schoolclasses",
                column: "IdSubsection");

            migrationBuilder.CreateIndex(
                name: "IX_Students_IdSchoolclass",
                table: "Students",
                column: "IdSchoolclass");

            migrationBuilder.CreateIndex(
                name: "IX_StudentsDevice_IdStudent",
                table: "StudentsDevice",
                column: "IdStudent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArrivalDateTime");

            migrationBuilder.DropTable(
                name: "Periods");

            migrationBuilder.DropTable(
                name: "StudentsDevice");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Schoolclasses");

            migrationBuilder.DropTable(
                name: "Cycles");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "Subsections");
        }
    }
}
