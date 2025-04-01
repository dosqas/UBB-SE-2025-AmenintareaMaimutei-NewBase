drop database if exists CourseApp;
go
create database CourseApp;
go
use CourseApp
go

-- Users Table
CREATE TABLE Users (
    UserId INT PRIMARY KEY,
    Username VARCHAR(255) NOT NULL UNIQUE,
    Email VARCHAR(255) NOT NULL UNIQUE
);

-- Courses Table
CREATE TABLE Courses (
    CourseId INT PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Description VARCHAR(255) NOT NULL,
    isPremium BIT NOT NULL DEFAULT 0,
    Cost INT DEFAULT 0,
    ImageUrl VARCHAR(255),
    timeToComplete INT NOT NULL --in seconds
);

-- Modules Table
CREATE TABLE Modules (
    ModuleId INT PRIMARY KEY,
    CourseId INT NOT NULL,
    Title VARCHAR(255) NOT NULL,
    Description VARCHAR(MAX),
    Position INT NOT NULL,
    isBonus BIT NOT NULL DEFAULT 0,
    Cost INT DEFAULT 0,
    ImageUrl VARCHAR(255),
    FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE
);

-- Tags Table
CREATE TABLE Tags (
    TagId INT PRIMARY KEY,
    Name VARCHAR(50) NOT NULL UNIQUE
);

-- Course Tags Relationship Table
CREATE TABLE CourseTags (
    CourseId INT NOT NULL,
    TagId INT NOT NULL,
    PRIMARY KEY (CourseId, TagId),
    FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE,
    FOREIGN KEY (TagId) REFERENCES Tags(TagId) ON DELETE CASCADE
);

-- Enrollment Table
CREATE TABLE Enrollment (
    UserId INT NOT NULL,
    CourseId INT NOT NULL,
    EnrolledAt DATETIME NOT NULL,
    timeSpent INT DEFAULT 0, -- seconds
    isCompleted BIT NOT NULL DEFAULT 0,
    PRIMARY KEY (UserId, CourseId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE
);

-- User Progress Table
CREATE TABLE UserProgress (
    UserId INT NOT NULL,
    ModuleId INT NOT NULL,
    status VARCHAR(20) NOT NULL,
	PRIMARY KEY (UserId, ModuleId),
    CONSTRAINT CK_Status CHECK (status IN ('not_completed', 'completed')),
    CONSTRAINT UQ_UserProgress UNIQUE (UserId, ModuleId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE NO ACTION,
    FOREIGN KEY (ModuleId) REFERENCES Modules(ModuleId) ON DELETE NO ACTION
);

-- User Wallet Table
CREATE TABLE UserWallet (
    UserId INT PRIMARY KEY,
    coinBalance INT DEFAULT 0,
    lastLogin DATETIME,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

-- Insert sample Users
INSERT INTO Users (UserId, Username, Email)
VALUES 
(0, 'john_doe', 'john@example.com')

-- Insert sample Courses
INSERT INTO Courses (CourseId, Title, Description, isPremium, Cost, ImageUrl, timeToComplete)
VALUES 
(1, 'Python Basics', 'Learn Python programming fundamentals', 0, 0, 'python_basics.jpg', 300),
(2, 'Advanced JavaScript', 'Master JavaScript concepts', 1, 499, 'js_advanced.jpg', 480),
(3, 'SQL Essentials', 'Database management basics', 0, 0, 'sql_basic.jpg', 240);

-- Insert sample Modules
INSERT INTO Modules (ModuleId, CourseId, Title, Description, Position, isBonus, Cost)
VALUES 
(1, 1, 'Introduction to Python', 'Basic Python syntax', 1, 0, 0),
(2, 1, 'Python Data Types', 'Understanding Python data types', 2, 0, 0),
(3, 2, 'JavaScript DOM', 'Document Object Model basics', 1, 0, 0),
(4, 2, 'Async Programming', 'Promises and async/await', 2, 1, 99),
(5, 3, 'SQL Queries', 'Basic SQL query structure', 1, 0, 0);

-- Insert sample Tags
INSERT INTO Tags (TagId, name)
VALUES 
(1, 'Programming'),
(2, 'Web Development'),
(3, 'Database'),
(4, 'Beginner');

-- Insert sample CourseTags
INSERT INTO CourseTags (CourseId, TagId)
VALUES 
(1, 1), (1, 4),
(2, 1), (2, 2),
(3, 3), (3, 4);

