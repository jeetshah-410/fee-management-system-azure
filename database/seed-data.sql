-- database/seed-data.sql
-- Seed data for 20 sample students and 2 administrators

INSERT INTO Administrators (AdminID, Name, Role) VALUES 
(1, 'Emily Davis', 'Fee.Admin'),
(2, 'Michael Smith', 'Fee.Admin');

-- 7 Paid Students (PaidAmount >= TotalFee)
INSERT INTO Students (StudentID, Name, Course, TotalFee, PaidAmount, DueDate, Email) VALUES
(1, 'John Miller', 'Computer Science', 5000.00, 5000.00, DATEADD(day, 30, GETUTCDATE()), 'john.miller@university.edu'),
(2, 'Sarah Wilson', 'Mechanical Eng', 4500.00, 4500.00, DATEADD(day, -5, GETUTCDATE()), 'sarah.wilson@university.edu'),
(3, 'David Brown', 'Mathematics', 4000.00, 4000.00, DATEADD(day, 10, GETUTCDATE()), 'david.brown@university.edu'),
(4, 'Jessica Taylor', 'Physics', 4800.00, 4800.00, DATEADD(day, -10, GETUTCDATE()), 'jessica.taylor@university.edu'),
(5, 'James Anderson', 'Chemistry', 4200.00, 4200.00, DATEADD(day, 15, GETUTCDATE()), 'james.anderson@university.edu'),
(6, 'Ashley Thomas', 'Biology', 4600.00, 4600.00, DATEADD(day, 20, GETUTCDATE()), 'ashley.thomas@university.edu'),
(7, 'Robert Jackson', 'History', 3500.00, 3500.00, DATEADD(day, 5, GETUTCDATE()), 'robert.jackson@university.edu');

-- 6 Partially Paid Students (PaidAmount < TotalFee AND DueDate > Now)
INSERT INTO Students (StudentID, Name, Course, TotalFee, PaidAmount, DueDate, Email) VALUES
(8, 'Mary White', 'Computer Science', 5000.00, 2500.00, DATEADD(day, 30, GETUTCDATE()), 'mary.white@university.edu'),
(9, 'William Harris', 'Mechanical Eng', 4500.00, 2000.00, DATEADD(day, 25, GETUTCDATE()), 'william.harris@university.edu'),
(10, 'Amanda Martin', 'Mathematics', 4000.00, 1500.00, DATEADD(day, 20, GETUTCDATE()), 'amanda.martin@university.edu'),
(11, 'Richard Thompson', 'Physics', 4800.00, 2400.00, DATEADD(day, 15, GETUTCDATE()), 'richard.thompson@university.edu'),
(12, 'Melissa Garcia', 'Chemistry', 4200.00, 1000.00, DATEADD(day, 10, GETUTCDATE()), 'melissa.garcia@university.edu'),
(13, 'Charles Martinez', 'Biology', 4600.00, 0.00, DATEADD(day, 5, GETUTCDATE()), 'charles.martinez@university.edu');

-- 7 Overdue Students (PaidAmount < TotalFee AND DueDate < Now)
INSERT INTO Students (StudentID, Name, Course, TotalFee, PaidAmount, DueDate, Email) VALUES
(14, 'Brian Robinson', 'History', 3500.00, 1000.00, DATEADD(day, -5, GETUTCDATE()), 'brian.robinson@university.edu'),
(15, 'Nicole Clark', 'Computer Science', 5000.00, 2500.00, DATEADD(day, -10, GETUTCDATE()), 'nicole.clark@university.edu'),
(16, 'Kevin Rodriguez', 'Mechanical Eng', 4500.00, 0.00, DATEADD(day, -15, GETUTCDATE()), 'kevin.rodriguez@university.edu'),
(17, 'Rachel Lewis', 'Mathematics', 4000.00, 2000.00, DATEADD(day, -20, GETUTCDATE()), 'rachel.lewis@university.edu'),
(18, 'Steven Lee', 'Physics', 4800.00, 500.00, DATEADD(day, -25, GETUTCDATE()), 'steven.lee@university.edu'),
(19, 'Michelle Walker', 'Chemistry', 4200.00, 3000.00, DATEADD(day, -30, GETUTCDATE()), 'michelle.walker@university.edu'),
(20, 'Daniel Hall', 'Biology', 4600.00, 0.00, DATEADD(day, -35, GETUTCDATE()), 'daniel.hall@university.edu');
