-- Create Database (Optional - uncomment if creating from scratch)
-- CREATE DATABASE [LeaveTrackDb];
-- GO
-- USE [LeaveTrackDb];
-- GO

-- 1. Roles Table
CREATE TABLE [dbo].[Roles] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    [NormalizedName] NVARCHAR(50) NOT NULL UNIQUE,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Seed basic roles
INSERT INTO [dbo].[Roles] ([Name], [NormalizedName]) 
VALUES ('Admin', 'ADMIN'), ('Manager', 'MANAGER'), ('Employee', 'EMPLOYEE');

-- 2. Users Table (Core Auth Credentials)
CREATE TABLE [dbo].[Users] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Username] NVARCHAR(100) NOT NULL UNIQUE,
    [Email] NVARCHAR(150) NOT NULL UNIQUE,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [RoleId] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id])
);

-- 3. Employees Table (Detailed Employee Meta)
CREATE TABLE [dbo].[Employees] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL UNIQUE,
    [EmployeeCode] NVARCHAR(20) NOT NULL UNIQUE,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [Department] NVARCHAR(100) NOT NULL,
    [Designation] NVARCHAR(100) NOT NULL,
    [PhoneNumber] NVARCHAR(20) NULL,
    [DateOfJoining] DATE NOT NULL,
    [ManagerId] INT NULL, -- Self-referencing relationship
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Employees_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Employees_Employees_ManagerId] FOREIGN KEY ([ManagerId]) REFERENCES [dbo].[Employees] ([Id])
);

-- 4. Leave Types Table
CREATE TABLE [dbo].[LeaveTypes] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL UNIQUE,
    [DefaultDaysAllocated] INT NOT NULL DEFAULT 0, -- Annual limit
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [PK_LeaveTypes] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Seed standard leave types
INSERT INTO [dbo].[LeaveTypes] ([Name], [DefaultDaysAllocated])
VALUES ('Casual Leave', 12), ('Sick Leave', 10), ('Earned Leave', 15), ('Maternity Leave', 90);

-- 5. Leave Requests Table
CREATE TABLE [dbo].[LeaveRequests] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [EmployeeId] INT NOT NULL,
    [LeaveTypeId] INT NOT NULL,
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NOT NULL,
    [NumberOfDays] DECIMAL(4,1) NOT NULL, -- To support half days (e.g. 0.5, 1.5)
    [Reason] NVARCHAR(500) NOT NULL,
    [Status] VARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, Approved, Rejected
    [ApprovedById] INT NULL, -- Employee ID of the approving Manager/Admin
    [RejectionReason] NVARCHAR(250) NULL,
    [AppliedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [ActionedAt] DATETIME2 NULL,
    CONSTRAINT [PK_LeaveRequests] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LeaveRequests_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]),
    CONSTRAINT [FK_LeaveRequests_LeaveTypes_LeaveTypeId] FOREIGN KEY ([LeaveTypeId]) REFERENCES [dbo].[LeaveTypes] ([Id]),
    CONSTRAINT [FK_LeaveRequests_Employees_ApprovedById] FOREIGN KEY ([ApprovedById]) REFERENCES [dbo].[Employees] ([Id]),
    CONSTRAINT [CK_LeaveRequests_Dates] CHECK ([StartDate] <= [EndDate]),
    CONSTRAINT [CK_LeaveRequests_Status] CHECK ([Status] IN ('Pending', 'Approved', 'Rejected'))
);

-- 6. Attendance Table
CREATE TABLE [dbo].[Attendance] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [EmployeeId] INT NOT NULL,
    [Date] DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    [CheckIn] DATETIME2 NULL,
    [CheckOut] DATETIME2 NULL,
    [Status] VARCHAR(20) NOT NULL DEFAULT 'Absent', -- Present, Absent, HalfDay, Late
    [Remarks] NVARCHAR(250) NULL,
    CONSTRAINT [PK_Attendance] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Attendance_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]),
    CONSTRAINT [UQ_Attendance_Employee_Date] UNIQUE ([EmployeeId], [Date]),
    CONSTRAINT [CK_Attendance_Status] CHECK ([Status] IN ('Present', 'Absent', 'HalfDay', 'Late'))
);

-- 7. Holidays Table
CREATE TABLE [dbo].[Holidays] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [HolidayDate] DATE NOT NULL UNIQUE,
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Holidays] PRIMARY KEY CLUSTERED ([Id] ASC)
);
