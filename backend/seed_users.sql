INSERT INTO "Employees" ("FullName", "Email", "Department", "Designation", "DateOfJoining")
VALUES ('Manager User', 'manager@leavetrack.com', 'Engineering', 'Engineering Manager', '2026-01-01');

INSERT INTO "Users" ("Username", "PasswordHash", "RoleId", "EmployeeId")
VALUES ('manager1', '$2b$11$oVw/6PXReMgD0GodOuPY/OhtiKyh0fIG5EQwU1nwXaxXAvdyWSd5C', 2, (SELECT "Id" FROM "Employees" WHERE "Email" = 'manager@leavetrack.com'));

INSERT INTO "Employees" ("FullName", "Email", "Department", "Designation", "DateOfJoining")
VALUES ('HR User', 'hr@leavetrack.com', 'Human Resources', 'HR Manager', '2026-01-01');

INSERT INTO "Users" ("Username", "PasswordHash", "RoleId", "EmployeeId")
VALUES ('hr1', '$2b$11$cSQsZXu6Gzi3dmoGSc.psOeKn6zSab6gIDGk.Yjio3diSzKYSOJ.i', 4, (SELECT "Id" FROM "Employees" WHERE "Email" = 'hr@leavetrack.com'));
