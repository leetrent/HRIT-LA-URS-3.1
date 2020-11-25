using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace lmsextreg.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "URS");

            migrationBuilder.CreateTable(
                name: "Agency",
                schema: "URS",
                columns: table => new
                {
                    AgencyID = table.Column<string>(nullable: false),
                    AgencyName = table.Column<string>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    OPMCode = table.Column<string>(nullable: true),
                    TreasuryCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agency", x => x.AgencyID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "URS",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnrollmentStatus",
                schema: "URS",
                columns: table => new
                {
                    StatusCode = table.Column<string>(nullable: false),
                    StatusLabel = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrollmentStatus", x => x.StatusCode);
                });

            migrationBuilder.CreateTable(
                name: "EventType",
                schema: "URS",
                columns: table => new
                {
                    EventTypeCode = table.Column<string>(nullable: false),
                    EventTypeLabel = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventType", x => x.EventTypeCode);
                });

            migrationBuilder.CreateTable(
                name: "LMSProgram",
                schema: "URS",
                columns: table => new
                {
                    LMSProgramID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    ShortName = table.Column<string>(nullable: false),
                    LongName = table.Column<string>(nullable: false),
                    CommonInbox = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LMSProgram", x => x.LMSProgramID);
                });

            migrationBuilder.CreateTable(
                name: "SubAgency",
                schema: "URS",
                columns: table => new
                {
                    SubAgencyID = table.Column<string>(nullable: false),
                    AgencyID = table.Column<string>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    OPMCode = table.Column<string>(nullable: true),
                    SubAgencyName = table.Column<string>(nullable: false),
                    TreasuryCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubAgency", x => x.SubAgencyID);
                    table.ForeignKey(
                        name: "FK_SubAgency_Agency_AgencyID",
                        column: x => x.AgencyID,
                        principalSchema: "URS",
                        principalTable: "Agency",
                        principalColumn: "AgencyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "URS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "URS",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatusTransition",
                schema: "URS",
                columns: table => new
                {
                    StatusTransitionID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    FromStatusCode = table.Column<string>(nullable: false),
                    ToStatusCode = table.Column<string>(nullable: false),
                    TransitionCode = table.Column<string>(nullable: false),
                    TransitionLabel = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusTransition", x => x.StatusTransitionID);
                    table.ForeignKey(
                        name: "FK_StatusTransition_EnrollmentStatus_FromStatusCode",
                        column: x => x.FromStatusCode,
                        principalSchema: "URS",
                        principalTable: "EnrollmentStatus",
                        principalColumn: "StatusCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StatusTransition_EnrollmentStatus_ToStatusCode",
                        column: x => x.ToStatusCode,
                        principalSchema: "URS",
                        principalTable: "EnrollmentStatus",
                        principalColumn: "StatusCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "URS",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<byte>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<byte>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<byte>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<byte>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    MiddleName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: false),
                    JobTitle = table.Column<string>(nullable: false),
                    AgencyID = table.Column<string>(nullable: false),
                    SubAgencyID = table.Column<string>(nullable: false),
                    DateRegistered = table.Column<DateTime>(nullable: false),
                    DateAccountExpires = table.Column<DateTime>(nullable: false),
                    DatePasswordExpires = table.Column<DateTime>(nullable: false),
                    RulesOfBehaviorAgreedTo = table.Column<byte>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Agency_AgencyID",
                        column: x => x.AgencyID,
                        principalSchema: "URS",
                        principalTable: "Agency",
                        principalColumn: "AgencyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_SubAgency_SubAgencyID",
                        column: x => x.SubAgencyID,
                        principalSchema: "URS",
                        principalTable: "SubAgency",
                        principalColumn: "SubAgencyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "URS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "URS",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "URS",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "URS",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "URS",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "URS",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "URS",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "URS",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "URS",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventLog",
                schema: "URS",
                columns: table => new
                {
                    EventLogID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    EventTypeCode = table.Column<string>(nullable: false),
                    UserCreatedID = table.Column<string>(nullable: false),
                    UserCreatedName = table.Column<string>(nullable: false),
                    DataValues = table.Column<string>(nullable: true),
                    DateTimeCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLog", x => x.EventLogID);
                    table.ForeignKey(
                        name: "FK_EventLog_EventType_EventTypeCode",
                        column: x => x.EventTypeCode,
                        principalSchema: "URS",
                        principalTable: "EventType",
                        principalColumn: "EventTypeCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventLog_AspNetUsers_UserCreatedID",
                        column: x => x.UserCreatedID,
                        principalSchema: "URS",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramApprover",
                schema: "URS",
                columns: table => new
                {
                    LMSProgramID = table.Column<int>(nullable: false),
                    ApproverUserId = table.Column<string>(nullable: false),
                    EmailNotify = table.Column<byte>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramApprover", x => new { x.LMSProgramID, x.ApproverUserId });
                    table.ForeignKey(
                        name: "FK_ProgramApprover_AspNetUsers_ApproverUserId",
                        column: x => x.ApproverUserId,
                        principalSchema: "URS",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramApprover_LMSProgram_LMSProgramID",
                        column: x => x.LMSProgramID,
                        principalSchema: "URS",
                        principalTable: "LMSProgram",
                        principalColumn: "LMSProgramID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramEnrollment",
                schema: "URS",
                columns: table => new
                {
                    ProgramEnrollmentID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    LMSProgramID = table.Column<int>(nullable: false),
                    StudentUserId = table.Column<string>(nullable: false),
                    ApproverUserId = table.Column<string>(nullable: true),
                    StatusCode = table.Column<string>(nullable: false),
                    UserCreated = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserLastUpdated = table.Column<string>(nullable: true),
                    DateLastUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramEnrollment", x => x.ProgramEnrollmentID);
                    table.ForeignKey(
                        name: "FK_ProgramEnrollment_AspNetUsers_ApproverUserId",
                        column: x => x.ApproverUserId,
                        principalSchema: "URS",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramEnrollment_LMSProgram_LMSProgramID",
                        column: x => x.LMSProgramID,
                        principalSchema: "URS",
                        principalTable: "LMSProgram",
                        principalColumn: "LMSProgramID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramEnrollment_EnrollmentStatus_StatusCode",
                        column: x => x.StatusCode,
                        principalSchema: "URS",
                        principalTable: "EnrollmentStatus",
                        principalColumn: "StatusCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramEnrollment_AspNetUsers_StudentUserId",
                        column: x => x.StudentUserId,
                        principalSchema: "URS",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnrollmentHistory",
                schema: "URS",
                columns: table => new
                {
                    EnrollmentHistoryID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    ProgramEnrollmentID = table.Column<int>(nullable: false),
                    StatusTransitionID = table.Column<int>(nullable: false),
                    ActorUserId = table.Column<string>(nullable: false),
                    ActorRemarks = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrollmentHistory", x => x.EnrollmentHistoryID);
                    table.ForeignKey(
                        name: "FK_EnrollmentHistory_AspNetUsers_ActorUserId",
                        column: x => x.ActorUserId,
                        principalSchema: "URS",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnrollmentHistory_ProgramEnrollment_ProgramEnrollmentID",
                        column: x => x.ProgramEnrollmentID,
                        principalSchema: "URS",
                        principalTable: "ProgramEnrollment",
                        principalColumn: "ProgramEnrollmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnrollmentHistory_StatusTransition_StatusTransitionID",
                        column: x => x.StatusTransitionID,
                        principalSchema: "URS",
                        principalTable: "StatusTransition",
                        principalColumn: "StatusTransitionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "URS",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "URS",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "URS",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "URS",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "URS",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AgencyID",
                schema: "URS",
                table: "AspNetUsers",
                column: "AgencyID");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "URS",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "URS",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SubAgencyID",
                schema: "URS",
                table: "AspNetUsers",
                column: "SubAgencyID");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentHistory_ActorUserId",
                schema: "URS",
                table: "EnrollmentHistory",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentHistory_ProgramEnrollmentID",
                schema: "URS",
                table: "EnrollmentHistory",
                column: "ProgramEnrollmentID");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentHistory_StatusTransitionID",
                schema: "URS",
                table: "EnrollmentHistory",
                column: "StatusTransitionID");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentStatus_StatusLabel",
                schema: "URS",
                table: "EnrollmentStatus",
                column: "StatusLabel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventLog_EventTypeCode",
                schema: "URS",
                table: "EventLog",
                column: "EventTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_EventLog_UserCreatedID",
                schema: "URS",
                table: "EventLog",
                column: "UserCreatedID");

            migrationBuilder.CreateIndex(
                name: "IX_EventType_EventTypeLabel",
                schema: "URS",
                table: "EventType",
                column: "EventTypeLabel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgramApprover_ApproverUserId",
                schema: "URS",
                table: "ProgramApprover",
                column: "ApproverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramEnrollment_ApproverUserId",
                schema: "URS",
                table: "ProgramEnrollment",
                column: "ApproverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramEnrollment_StatusCode",
                schema: "URS",
                table: "ProgramEnrollment",
                column: "StatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramEnrollment_StudentUserId",
                schema: "URS",
                table: "ProgramEnrollment",
                column: "StudentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramEnrollment_LMSProgramID_StudentUserId",
                schema: "URS",
                table: "ProgramEnrollment",
                columns: new[] { "LMSProgramID", "StudentUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatusTransition_ToStatusCode",
                schema: "URS",
                table: "StatusTransition",
                column: "ToStatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_StatusTransition_FromStatusCode_ToStatusCode",
                schema: "URS",
                table: "StatusTransition",
                columns: new[] { "FromStatusCode", "ToStatusCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubAgency_AgencyID",
                schema: "URS",
                table: "SubAgency",
                column: "AgencyID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "URS");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "URS");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "URS");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "URS");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "URS");

            migrationBuilder.DropTable(
                name: "EnrollmentHistory",
                schema: "URS");

            migrationBuilder.DropTable(
                name: "EventLog",
                schema: "URS");

            migrationBuilder.DropTable(
                name: "ProgramApprover",
                schema: "URS");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "URS");

            migrationBuilder.DropTable(
                name: "ProgramEnrollment",
                schema: "URS");

            migrationBuilder.DropTable(
                name: "StatusTransition",
                schema: "URS");

            migrationBuilder.DropTable(
                name: "EventType",
                schema: "URS");

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "URS");

            migrationBuilder.DropTable(
                name: "LMSProgram",
                schema: "URS");

            migrationBuilder.DropTable(
                name: "EnrollmentStatus",
                schema: "URS");

            migrationBuilder.DropTable(
                name: "SubAgency",
                schema: "URS");

            migrationBuilder.DropTable(
                name: "Agency",
                schema: "URS");
        }
    }
}
