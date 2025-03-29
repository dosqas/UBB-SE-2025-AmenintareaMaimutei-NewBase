UDROP TABLE IF EXISTS UserPictureCoins;
DROP TABLE IF EXISTS UserPictures;
DROP TABLE IF EXISTS UserCoins;
DROP TABLE IF EXISTS UserProgress;
DROP TABLE IF EXISTS Enrollment;
DROP TABLE IF EXISTS CourseTags;
DROP TABLE IF EXISTS Tags;
DROP TABLE IF EXISTS Modules;
DROP TABLE IF EXISTS Courses;
DROP TABLE IF EXISTS CourseTypes;
DROP TABLE IF EXISTS Users;

-- Users Table
CREATE TABLE Users (
    users_id INT PRIMARY KEY,
    username VARCHAR(255) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    created_at INT
);

-- Course Types Table (Primary Key changed to course_id)
CREATE TABLE CourseTypes (
    course_id INT PRIMARY KEY,
    type_name VARCHAR(100) NOT NULL UNIQUE,
    price DECIMAL(10,2) NOT NULL CHECK (price >= 0)
);

-- Courses Table
CREATE TABLE Courses (
    course_id INT PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    type_id INT NOT NULL,
    created_at INT,
    FOREIGN KEY (type_id) REFERENCES CourseTypes(course_id) ON DELETE CASCADE
);

-- Modules Table
CREATE TABLE Modules (
    module_id INT PRIMARY KEY,
    course_id INT NOT NULL,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    position INT NOT NULL,
    FOREIGN KEY (course_id) REFERENCES Courses(course_id) ON DELETE CASCADE
);

-- Tags Table
CREATE TABLE Tags (
    tag_id INT PRIMARY KEY,
    tag_name VARCHAR(50) NOT NULL UNIQUE
);

-- Course Tags Relationship Table
CREATE TABLE CourseTags (
    course_id INT NOT NULL,
    tag_id INT NOT NULL,
    PRIMARY KEY (course_id, tag_id),
    FOREIGN KEY (course_id) REFERENCES Courses(course_id) ON DELETE CASCADE,
    FOREIGN KEY (tag_id) REFERENCES Tags(tag_id) ON DELETE CASCADE
);

-- Enrollment Table (Composite Primary Key for one-time enrollment per user-course)
CREATE TABLE Enrollment (
    users_id INT NOT NULL,
    course_id INT NOT NULL,
    enrolled_at INT,
    PRIMARY KEY (users_id, course_id),
    FOREIGN KEY (users_id) REFERENCES Users(users_id) ON DELETE CASCADE,
    FOREIGN KEY (course_id) REFERENCES Courses(course_id) ON DELETE CASCADE
);

-- User Progress Table
CREATE TABLE UserProgress (
    progress_id INT PRIMARY KEY,
    users_id INT NOT NULL,
    course_id INT NOT NULL,
    module_id INT NOT NULL,
    progress_percentage DECIMAL(5,2) CHECK (progress_percentage BETWEEN 0 AND 100),
    last_updated INT,
    UNIQUE (users_id, course_id, module_id),
    FOREIGN KEY (users_id) REFERENCES Users(users_id) ON DELETE NO ACTION,
    FOREIGN KEY (course_id) REFERENCES Courses(course_id) ON DELETE NO ACTION,
    FOREIGN KEY (module_id) REFERENCES Modules(module_id) ON DELETE NO ACTION
);

-- Coin System Table
CREATE TABLE UserCoins (
    users_id INT PRIMARY KEY,
    coin_balance INT DEFAULT 0,
    FOREIGN KEY (users_id) REFERENCES Users(users_id) ON DELETE CASCADE
);

-- User Pictures Table
CREATE TABLE UserPictures (
    picture_id INT PRIMARY KEY,
    users_id INT NOT NULL,
    image_url VARCHAR(500) NOT NULL,
    uploaded_at int,
    FOREIGN KEY (users_id) REFERENCES Users(users_id) ON DELETE NO ACTION
);

-- Table to track if a user has received coins for clicking on a picture (Only Once)
CREATE TABLE UserPictureCoins (
    users_id INT NOT NULL,
    picture_id INT NOT NULL,
    coins_received int DEFAULT 0,
    PRIMARY KEY (users_id, picture_id),
    FOREIGN KEY (users_id) REFERENCES Users(users_id) ON DELETE CASCADE,
    FOREIGN KEY (picture_id) REFERENCES UserPictures(picture_id) ON DELETE CASCADE
);

-- Search Indexes for Performance Optimization
CREATE INDEX idx_course_title ON Courses(title);
CREATE INDEX idx_course_tags ON CourseTags(tag_id);
CREATE INDEX idx_user_progress ON UserProgress(users_id, course_id);


INSERT INTO Users (users_id, username, email, created_at) VALUES
(1, 'john_doe', 'john@example.com', 1625097600),
(2, 'jane_smith', 'jane@example.com', 1625184000),
(3, 'mike_jones', 'mike@example.com', 1625270400),
(4, 'sarah_williams', 'sarah@example.com', 1625356800),
(5, 'david_brown', 'david@example.com', 1625443200),
(6, 'emily_davis', 'emily@example.com', 1625529600),
(7, 'robert_johnson', 'robert@example.com', 1625616000),
(8, 'lisa_miller', 'lisa@example.com', 1625702400),
(9, 'thomas_wilson', 'thomas@example.com', 1625788800),
(10, 'jennifer_taylor', 'jennifer@example.com', 1625875200);

INSERT INTO CourseTypes (course_id, type_name, price) VALUES
(1, 'Free', 0),
(2, 'Premium', 10);

INSERT INTO Tags (tag_id, tag_name) VALUES
(1, 'Programming'),
(2, 'Design'),
(3, 'Business'),
(4, 'Marketing'),
(5, 'Data Science'),
(6, 'Web Development'),
(7, 'Mobile Development'),
(8, 'Cloud Computing'),
(9, 'Artificial Intelligence'),
(10, 'Cybersecurity');

INSERT INTO Courses (course_id, title, description, type_id, created_at) VALUES
(1, 'Introduction to Python', 'Learn Python programming basics', 1, 1625097600),
(2, 'Advanced Python', 'Deep dive into Python features', 2, 1625184000),
(3, 'Web Design Fundamentals', 'Learn HTML, CSS, and design principles', 1, 1625270400),
(4, 'Digital Marketing 101', 'Introduction to digital marketing strategies', 2, 1625356800),
(5, 'Data Science with R', 'Data analysis and visualization with R', 2, 1625443200),
(6, 'iOS App Development', 'Build iOS apps with Swift', 2, 1625529600),
(7, 'AWS Certified Solutions Architect', 'Prepare for AWS certification', 2, 1625616000),
(8, 'Machine Learning Basics', 'Introduction to ML algorithms', 1, 1625702400),
(9, 'Ethical Hacking', 'Cybersecurity and penetration testing', 1, 1625788800),
(10, 'Business Analytics', 'Data-driven business decision making', 1, 1625875200);

INSERT INTO CourseTags (course_id, tag_id) VALUES
(1, 1), (1, 6),
(2, 1), (2, 5),
(3, 2), (3, 6),
(4, 3), (4, 4),
(5, 1), (5, 5),
(6, 1), (6, 7),
(7, 8),
(8, 1), (8, 5), (8, 9),
(9, 10),
(10, 3), (10, 5);

INSERT INTO Modules (module_id, course_id, title, description, position) VALUES
-- Python Introduction (course_id 1)
(1, 1, 'Getting Started', 'Introduction to Python', 1),
(2, 1, 'Variables and Data Types', 'Learn about Python data types', 2),
(3, 1, 'Control Flow', 'Conditionals and loops', 3),
(4, 1, 'Functions', 'Creating and using functions', 4),
(5, 1, 'Final Project', 'Apply what you learned', 5),

-- Advanced Python (course_id 2)
(6, 2, 'Object-Oriented Programming', 'Classes and objects', 1),
(7, 2, 'Decorators', 'Advanced function decorators', 2),
(8, 2, 'Generators', 'Working with generators', 3),
(9, 2, 'Concurrency', 'Multithreading and multiprocessing', 4),

-- Web Design (course_id 3)
(10, 3, 'HTML Basics', 'Structure of web pages', 1),
(11, 3, 'CSS Fundamentals', 'Styling web pages', 2),
(12, 3, 'Responsive Design', 'Design for all devices', 3),
(13, 3, 'Design Principles', 'UI/UX best practices', 4),

-- Digital Marketing (course_id 4)
(14, 4, 'SEO Basics', 'Search engine optimization', 1),
(15, 4, 'Social Media Marketing', 'Marketing on social platforms', 2),
(16, 4, 'Content Marketing', 'Creating engaging content', 3),
(17, 4, 'Email Marketing', 'Effective email campaigns', 4);

INSERT INTO Enrollment (users_id, course_id, enrolled_at) VALUES
(1, 1, 1625097600),
(1, 2, 1625184000),
(2, 1, 1625270400),
(2, 3, 1625356800),
(3, 4, 1625443200),
(3, 5, 1625529600),
(4, 6, 1625616000),
(4, 7, 1625702400),
(5, 8, 1625788800),
(5, 9, 1625875200),
(6, 10, 1625961600),
(6, 1, 1626048000),
(7, 2, 1626134400),
(7, 3, 1626220800),
(8, 4, 1626307200),
(8, 5, 1626393600),
(9, 6, 1626480000),
(9, 7, 1626566400),
(10, 8, 1626652800),
(10, 9, 1626739200);

INSERT INTO UserProgress (progress_id, users_id, course_id, module_id, progress_percentage, last_updated) VALUES
(1, 1, 1, 1, 100, 1625097600),
(2, 1, 1, 2, 100, 1625184000),
(3, 1, 1, 3, 75, 1625270400),
(4, 1, 1, 4, 50, 1625356800),
(5, 1, 1, 5, 25, 1625443200),
(6, 1, 2, 6, 100, 1625529600),
(7, 1, 2, 7, 50, 1625616000),
(8, 2, 1, 1, 100, 1625702400),
(9, 2, 1, 2, 75, 1625788800),
(10, 2, 3, 10, 100, 1625875200),
(11, 2, 3, 11, 50, 1625961600),
(12, 3, 4, 14, 100, 1626048000),
(13, 3, 4, 15, 75, 1626134400),
(14, 3, 5, 1, 25, 1626220800),
(15, 4, 6, 1, 100, 1626307200),
(16, 4, 6, 2, 50, 1626393600),
(17, 4, 7, 1, 25, 1626480000),
(18, 5, 8, 1, 100, 1626566400),
(19, 5, 9, 1, 75, 1626652800),
(20, 5, 9, 2, 50, 1626739200);

INSERT INTO UserCoins (users_id, coin_balance) VALUES
(1, 150),
(2, 75),
(3, 200),
(4, 50),
(5, 300),
(6, 100),
(7, 25),
(8, 175),
(9, 225),
(10, 125);

INSERT INTO UserPictures (picture_id, users_id, image_url, uploaded_at) VALUES
(1, 1, 'https://example.com/pics/john.jpg', 1625097600),
(2, 2, 'https://example.com/pics/jane.jpg', 1625184000),
(3, 3, 'https://example.com/pics/mike.jpg', 1625270400),
(4, 4, 'https://example.com/pics/sarah.jpg', 1625356800),
(5, 5, 'https://example.com/pics/david.jpg', 1625443200),
(6, 6, 'https://example.com/pics/emily.jpg', 1625529600),
(7, 7, 'https://example.com/pics/robert.jpg', 1625616000),
(8, 8, 'https://example.com/pics/lisa.jpg', 1625702400),
(9, 9, 'https://example.com/pics/thomas.jpg', 1625788800),
(10, 10, 'https://example.com/pics/jennifer.jpg', 1625875200);

INSERT INTO UserPictureCoins (users_id, picture_id, coins_received) VALUES
-- John clicked on Jane's and Mike's pictures
(1, 2, 10),
(1, 3, 10),
-- Jane clicked on John's and Sarah's pictures
(2, 1, 10),
(2, 4, 10),
-- Mike clicked on John's and Jane's pictures
(3, 1, 10),
(3, 2, 10),
-- Sarah clicked on David's and Emily's pictures
(4, 5, 10),
(4, 6, 10),
-- David clicked on Robert's and Lisa's pictures
(5, 7, 10),
(5, 8, 10);


SELECT * from Users;
Select * from Courses;
Select * from CourseTags;
Select * from CourseTypes;
Select * from Enrollment;
Select * from Modules;
Select * from Tags;
Select * from UserCoins;
Select * from UserPictureCoins;
Select * from UserPictures;
Select * from UserProgress;