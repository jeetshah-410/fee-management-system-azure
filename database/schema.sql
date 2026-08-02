-- database/schema.sql

-- 1. Students Table
CREATE TABLE Students (
    StudentID INT PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Course NVARCHAR(200) NOT NULL,
    TotalFee DECIMAL(18, 2) NOT NULL,
    PaidAmount DECIMAL(18, 2) NOT NULL,
    DueDate DATETIME2 NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    LastReminderSentDate DATETIME2 NULL
);

-- 2. Administrators Table
CREATE TABLE Administrators (
    AdminID INT PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Role NVARCHAR(50) NOT NULL
);
