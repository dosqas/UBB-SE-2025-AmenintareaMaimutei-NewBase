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
    timeToComplete INT DEFAULT 3600 NOT NULL, --in seconds
    Difficulty VARCHAR(255) NOT NULL
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
    ImageClicked BIT NOT NULL DEFAULT 0,
	PRIMARY KEY (UserId, ModuleId),
    CONSTRAINT CK_Status CHECK (status IN ('not_completed', 'completed')),
    CONSTRAINT UQ_UserProgress UNIQUE (UserId, ModuleId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE NO ACTION,
    FOREIGN KEY (ModuleId) REFERENCES Modules(ModuleId) ON DELETE NO ACTION
);

-- Create CourseCompletions table
CREATE TABLE CourseCompletions (
    UserId INT NOT NULL,
    CourseId INT NOT NULL,
    CompletionRewardClaimed BIT NOT NULL DEFAULT 0,
    TimedRewardClaimed BIT NOT NULL DEFAULT 0,
    CompletedAt DATETIME NOT NULL,
    PRIMARY KEY (UserId, CourseId)
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

-- Insert sample Wallet
INSERT INTO UserWallet VALUES (0, 0, '2021-01-01')

-- Insert Courses
INSERT INTO Courses (CourseId, Title, Description, isPremium, Cost, ImageUrl, timeToComplete)
VALUES 
(1, 'Python Basics', 'Learn Python programming fundamentals', 0, 0, 'python_basics.jpg', 3600),
(2, 'Advanced JavaScript', 'Master JavaScript concepts', 1, 499, 'js_advanced.jpg', 4800),
(3, 'SQL Essentials', 'Database management basics', 0, 0, 'sql_basic.jpg', 3600),
(4, 'Web Development with React', 'Build dynamic web apps with React', 1, 599, 'react_web.jpg', 5400),
(5, 'Data Science with Python', 'Learn data analysis and visualization', 0, 0, 'data_science.jpg', 7200),
(6, 'Machine Learning Basics', 'Introduction to ML concepts', 1, 799, 'ml_basics.jpg', 8400),
(7, 'Cybersecurity Fundamentals', 'Basics of cybersecurity and encryption', 0, 0, 'cybersecurity.jpg', 4800),
(8, 'HTML & CSS for Beginners', 'Learn to build beautiful websites', 0, 0, 'html_css.jpg', 3600),
(9, 'DevOps and CI/CD', 'Continuous integration and deployment', 1, 699, 'devops.jpg', 6000),
(10, 'Mobile App Development', 'Create mobile apps with Flutter', 0, 0, 'flutter_apps.jpg', 7200),
(11, 'C++ for Beginners', 'Introduction to C++ programming', 0, 0, 'cpp_basics.jpg', 4200),
(12, 'Game Development with Unity', 'Build 2D and 3D games using Unity', 1, 899, 'unity_games.jpg', 9000),
(13, 'Blockchain Fundamentals', 'Introduction to blockchain technology', 0, 0, 'blockchain.jpg', 5400),
(14, 'Cloud Computing with AWS', 'Learn cloud computing with AWS services', 0, 0, 'aws_cloud.jpg', 7200),
(15, 'Artificial Intelligence in Practice', 'Apply AI techniques to real-world problems', 1, 850, 'ai_practice.jpg', 9600);

-- Insert Modules
INSERT INTO Modules (ModuleId, CourseId, Title, Description, Position, isBonus, Cost)
VALUES 
-- Course 1: Python Basics (Modules 1-12)
(1, 1, 'Introduction to Python', 'Basic Python syntax and setup', 1, 0, 0),
(2, 1, 'Python Variables and Data Types', 'Understanding variables, numbers, and strings', 2, 0, 0),
(3, 1, 'Control Flow', 'Using conditionals and loops in Python', 3, 0, 0),
(4, 1, 'Functions and Modules', 'Defining functions and importing modules', 4, 0, 0),
(5, 1, 'Error Handling', 'Managing exceptions and errors', 5, 0, 0),
(6, 1, 'File I/O', 'Reading from and writing to files', 6, 0, 0),
(7, 1, 'Object-Oriented Programming', 'Classes and objects in Python', 7, 0, 0),
(8, 1, 'Working with Libraries', 'Overview of popular Python libraries', 8, 0, 0),
(9, 1, 'Introduction to Data Science', 'Basics of data manipulation', 9, 0, 0),
(10, 1, 'Advanced Python Features', 'Decorators, generators, and comprehensions', 10, 0, 0),
(11, 1, 'Project: Build a CLI App', 'Hands-on project to create a command-line tool', 11, 0, 0),
(12, 1, 'Bonus: Python Performance Optimization', 'Tips for writing faster Python code', 12, 1, 199),

-- Course 2: Advanced JavaScript (Modules 13-24)
(13, 2, 'JavaScript DOM Manipulation', 'Interact with the browser DOM', 1, 0, 0),
(14, 2, 'ES6 Features', 'Modern JavaScript syntax and features', 2, 0, 0),
(15, 2, 'Async Programming', 'Working with promises and async/await', 3, 0, 0),
(16, 2, 'Advanced Functions', 'Closures, callbacks, and higher-order functions', 4, 0, 0),
(17, 2, 'Event Loop and Concurrency', 'Understanding JavaScript runtime', 5, 0, 0),
(18, 2, 'Web Performance Optimization', 'Techniques for faster web applications', 6, 0, 0),
(19, 2, 'JavaScript Frameworks Overview', 'Comparison of popular frameworks', 7, 0, 0),
(20, 2, 'Error Handling in JavaScript', 'Debugging and exception management', 8, 0, 0),
(21, 2, 'Testing JavaScript Applications', 'Tools and techniques for testing', 9, 0, 0),
(22, 2, 'Debugging Techniques', 'Effective strategies to fix bugs', 10, 0, 0),
(23, 2, 'Optimization Strategies', 'Improving code performance', 11, 0, 0),
(24, 2, 'Bonus: JavaScript Security Best Practices', 'Secure coding for web apps', 12, 1, 299),

-- Course 3: SQL Essentials (Modules 25-36)
(25, 3, 'Introduction to SQL', 'Basics of SQL and relational databases', 1, 0, 0),
(26, 3, 'Data Retrieval', 'SELECT statements and filtering', 2, 0, 0),
(27, 3, 'Sorting and Filtering', 'ORDER BY, WHERE, and LIMIT clauses', 3, 0, 0),
(28, 3, 'Aggregate Functions', 'SUM, COUNT, AVG, and more', 4, 0, 0),
(29, 3, 'Joins Explained', 'INNER, OUTER, and cross joins', 5, 0, 0),
(30, 3, 'Subqueries', 'Nested queries and their uses', 6, 0, 0),
(31, 3, 'Database Design', 'Normalization and schema design', 7, 0, 0),
(32, 3, 'Indexing and Optimization', 'Improving query performance', 8, 0, 0),
(33, 3, 'Views and Transactions', 'Managing views and transactional control', 9, 0, 0),
(34, 3, 'Stored Procedures', 'Creating reusable SQL code', 10, 0, 0),
(35, 3, 'Database Security', 'Protecting data with best practices', 11, 0, 0),
(36, 3, 'Bonus: Advanced SQL Tuning', 'Optimizing complex queries', 12, 1, 150),

-- Course 4: Web Development with React (Modules 37-48)
(37, 4, 'React Basics', 'Introduction to React and its ecosystem', 1, 0, 0),
(38, 4, 'JSX and Components', 'Building blocks of React apps', 2, 0, 0),
(39, 4, 'State and Props', 'Managing data within components', 3, 0, 0),
(40, 4, 'Lifecycle Methods', 'Component lifecycle and hooks', 4, 0, 0),
(41, 4, 'Hooks Overview', 'Using React hooks effectively', 5, 0, 0),
(42, 4, 'Routing with React Router', 'Implementing navigation', 6, 0, 0),
(43, 4, 'State Management with Redux', 'Global state management', 7, 0, 0),
(44, 4, 'Handling Forms', 'Managing user input in forms', 8, 0, 0),
(45, 4, 'API Integration', 'Fetching data from APIs', 9, 0, 0),
(46, 4, 'Testing React Apps', 'Techniques for testing components', 10, 0, 0),
(47, 4, 'Deployment Strategies', 'Deploying React applications', 11, 0, 0),
(48, 4, 'Bonus: Advanced Performance Optimization in React', 'Enhance app performance', 12, 1, 299),

-- Course 5: Data Science with Python (Modules 49-60)
(49, 5, 'Introduction to Data Science', 'Overview of data science concepts', 1, 0, 0),
(50, 5, 'Python for Data Science', 'Python libraries and tools', 2, 0, 0),
(51, 5, 'Data Analysis with Pandas', 'Manipulating data with Pandas', 3, 0, 0),
(52, 5, 'Data Visualization with Matplotlib', 'Creating charts and graphs', 4, 0, 0),
(53, 5, 'Statistical Analysis', 'Basics of statistics for data science', 5, 0, 0),
(54, 5, 'Machine Learning Introduction', 'Overview of ML algorithms', 6, 0, 0),
(55, 5, 'Data Cleaning Techniques', 'Handling missing and inconsistent data', 7, 0, 0),
(56, 5, 'Feature Engineering', 'Preparing data for modeling', 8, 0, 0),
(57, 5, 'Model Evaluation', 'Metrics and validation techniques', 9, 0, 0),
(58, 5, 'Working with Big Data', 'Introduction to big data tools', 10, 0, 0),
(59, 5, 'Project: Data Analysis', 'Apply your skills to a real dataset', 11, 0, 0),
(60, 5, 'Bonus: Advanced Data Wrangling Techniques', 'Master data transformation methods', 12, 1, 350),

-- Course 6: Machine Learning Basics (Modules 61-72)
(61, 6, 'Introduction to Machine Learning', 'What is ML and its applications', 1, 0, 0),
(62, 6, 'Supervised Learning', 'Regression and classification basics', 2, 0, 0),
(63, 6, 'Unsupervised Learning', 'Clustering and dimensionality reduction', 3, 0, 0),
(64, 6, 'Regression Analysis', 'Deep dive into regression models', 4, 0, 0),
(65, 6, 'Classification Techniques', 'Methods for classifying data', 5, 0, 0),
(66, 6, 'Model Evaluation', 'Assessing model performance', 6, 0, 0),
(67, 6, 'Overfitting and Underfitting', 'Balancing model complexity', 7, 0, 0),
(68, 6, 'Feature Scaling', 'Normalization and standardization', 8, 0, 0),
(69, 6, 'Ensemble Methods', 'Boosting and bagging techniques', 9, 0, 0),
(70, 6, 'Neural Networks Introduction', 'Basics of neural network design', 10, 0, 0),
(71, 6, 'Practical ML Project', 'End-to-end machine learning project', 11, 0, 0),
(72, 6, 'Bonus: Advanced Hyperparameter Tuning', 'Optimize your ML models', 12, 1, 450),

-- Course 7: Cybersecurity Fundamentals (Modules 73-84)
(73, 7, 'Introduction to Cybersecurity', 'Fundamental principles of cybersecurity', 1, 0, 0),
(74, 7, 'Types of Cyber Threats', 'Understanding malware, phishing, and more', 2, 0, 0),
(75, 7, 'Network Security Basics', 'Securing networks and protocols', 3, 0, 0),
(76, 7, 'Encryption and Cryptography', 'Basic encryption techniques', 4, 0, 0),
(77, 7, 'Secure Coding Practices', 'Writing secure code', 5, 0, 0),
(78, 7, 'Firewalls and VPNs', 'Implementing network security tools', 6, 0, 0),
(79, 7, 'Vulnerability Assessment', 'Identifying and mitigating risks', 7, 0, 0),
(80, 7, 'Incident Response', 'Responding to security breaches', 8, 0, 0),
(81, 7, 'Security Policies', 'Developing effective security policies', 9, 0, 0),
(82, 7, 'Ethical Hacking Overview', 'Introduction to penetration testing', 10, 0, 0),
(83, 7, 'Real-world Cybersecurity Cases', 'Case studies in cybersecurity', 11, 0, 0),
(84, 7, 'Bonus: Advanced Penetration Testing Techniques', 'Deep dive into ethical hacking', 12, 1, 300),

-- Course 8: HTML & CSS for Beginners (Modules 85-96)
(85, 8, 'Introduction to HTML', 'Basics of HTML structure', 1, 0, 0),
(86, 8, 'HTML Elements and Attributes', 'Detailed look at HTML elements', 2, 0, 0),
(87, 8, 'CSS Basics', 'Introduction to CSS styling', 3, 0, 0),
(88, 8, 'Styling with CSS', 'Advanced CSS selectors and properties', 4, 0, 0),
(89, 8, 'Responsive Design', 'Building mobile-friendly layouts', 5, 0, 0),
(90, 8, 'Flexbox Layout', 'Using Flexbox for responsive design', 6, 0, 0),
(91, 8, 'Grid Layout', 'Designing with CSS Grid', 7, 0, 0),
(92, 8, 'Working with Images and Media', 'Optimizing media in web design', 8, 0, 0),
(93, 8, 'Web Accessibility Basics', 'Making websites accessible', 9, 0, 0),
(94, 8, 'Building a Simple Web Page', 'From design to code', 10, 0, 0),
(95, 8, 'Project: Personal Portfolio', 'Create your own portfolio site', 11, 0, 0),
(96, 8, 'Bonus: Advanced CSS Animations', 'Bring websites to life with animations', 12, 1, 150),

-- Course 9: DevOps and CI/CD (Modules 97-108)
(97, 9, 'Introduction to DevOps', 'Understanding DevOps culture and practices', 1, 0, 0),
(98, 9, 'Understanding CI/CD', 'Principles of continuous integration and deployment', 2, 0, 0),
(99, 9, 'Version Control with Git', 'Using Git for source control', 3, 0, 0),
(100, 9, 'Continuous Integration Tools', 'Overview of popular CI tools', 4, 0, 0),
(101, 9, 'Continuous Deployment Strategies', 'Automating deployments', 5, 0, 0),
(102, 9, 'Containerization with Docker', 'Introduction to Docker', 6, 0, 0),
(103, 9, 'Orchestration with Kubernetes', 'Managing containerized applications', 7, 0, 0),
(104, 9, 'Infrastructure as Code', 'Automating infrastructure setup', 8, 0, 0),
(105, 9, 'Monitoring and Logging', 'Tools for system monitoring', 9, 0, 0),
(106, 9, 'Security in DevOps', 'Integrating security into DevOps', 10, 0, 0),
(107, 9, 'Case Study: DevOps in Action', 'Real-world DevOps implementation', 11, 0, 0),
(108, 9, 'Bonus: Advanced Pipeline Optimization', 'Improve your CI/CD pipelines', 12, 1, 350),

-- Course 10: Mobile App Development with Flutter (Modules 109-120)
(109, 10, 'Introduction to Flutter', 'Overview of Flutter and Dart', 1, 0, 0),
(110, 10, 'Dart Basics', 'Fundamentals of the Dart language', 2, 0, 0),
(111, 10, 'Flutter Widgets', 'Building UI with Flutter widgets', 3, 0, 0),
(112, 10, 'Building Layouts', 'Structuring your Flutter app', 4, 0, 0),
(113, 10, 'State Management', 'Managing state in Flutter apps', 5, 0, 0),
(114, 10, 'Navigation and Routing', 'Moving between screens', 6, 0, 0),
(115, 10, 'API Integration', 'Fetching data in Flutter', 7, 0, 0),
(116, 10, 'Animations in Flutter', 'Adding motion to your apps', 8, 0, 0),
(117, 10, 'Testing Flutter Apps', 'Writing tests for Flutter', 9, 0, 0),
(118, 10, 'Deploying Flutter Apps', 'Releasing your app to stores', 10, 0, 0),
(119, 10, 'Project: Build a Mobile App', 'Capstone mobile app project', 11, 0, 0),
(120, 10, 'Bonus: Advanced Flutter Performance Tips', 'Optimize your Flutter apps', 12, 1, 250),

-- Course 11: C++ for Beginners (Modules 121-132)
(121, 11, 'Introduction to C++', 'Basics of C++ programming', 1, 0, 0),
(122, 11, 'Basic Syntax', 'Understanding C++ syntax', 2, 0, 0),
(123, 11, 'Data Types in C++', 'Working with C++ data types', 3, 0, 0),
(124, 11, 'Control Structures', 'Using loops and conditionals', 4, 0, 0),
(125, 11, 'Functions in C++', 'Defining and calling functions', 5, 0, 0),
(126, 11, 'Arrays and Strings', 'Handling collections and text', 6, 0, 0),
(127, 11, 'Pointers and Memory Management', 'Understanding pointers', 7, 0, 0),
(128, 11, 'Object-Oriented Concepts', 'Introduction to classes and objects', 8, 0, 0),
(129, 11, 'Standard Template Library', 'Using STL containers and algorithms', 9, 0, 0),
(130, 11, 'Error Handling', 'Managing exceptions in C++', 10, 0, 0),
(131, 11, 'Project: Simple C++ App', 'Build a basic C++ application', 11, 0, 0),
(132, 11, 'Bonus: Advanced C++ Optimization', 'Improve performance in C++ code', 12, 1, 200),

-- Course 12: Game Development with Unity (Modules 133-144)
(133, 12, 'Introduction to Unity', 'Getting started with Unity', 1, 0, 0),
(134, 12, 'Unity Interface Basics', 'Navigating the Unity editor', 2, 0, 0),
(135, 12, '2D Game Development', 'Basics of 2D game design', 3, 0, 0),
(136, 12, '3D Game Development', 'Introduction to 3D game mechanics', 4, 0, 0),
(137, 12, 'Scripting with C#', 'Programming in Unity using C#', 5, 0, 0),
(138, 12, 'Physics in Unity', 'Implementing physics in games', 6, 0, 0),
(139, 12, 'Animation Basics', 'Animating characters and objects', 7, 0, 0),
(140, 12, 'UI Design', 'Creating game menus and HUDs', 8, 0, 0),
(141, 12, 'Audio Integration', 'Adding sound effects and music', 9, 0, 0),
(142, 12, 'Game Optimization', 'Improving game performance', 10, 0, 0),
(143, 12, 'Project: Create a Unity Game', 'End-to-end game development project', 11, 0, 0),
(144, 12, 'Bonus: Advanced Unity Techniques', 'Master advanced features in Unity', 12, 1, 400),

-- Course 13: Blockchain Fundamentals (Modules 145-156)
(145, 13, 'Introduction to Blockchain', 'Understanding the blockchain concept', 1, 0, 0),
(146, 13, 'How Blockchain Works', 'Mechanics behind blockchain technology', 2, 0, 0),
(147, 13, 'Cryptography in Blockchain', 'Encryption methods used in blockchain', 3, 0, 0),
(148, 13, 'Decentralization', 'Benefits of decentralized systems', 4, 0, 0),
(149, 13, 'Smart Contracts', 'Introduction to smart contract development', 5, 0, 0),
(150, 13, 'Blockchain Use Cases', 'Real-world applications of blockchain', 6, 0, 0),
(151, 13, 'Consensus Mechanisms', 'Exploring proof-of-work and proof-of-stake', 7, 0, 0),
(152, 13, 'Building a Simple Blockchain', 'Hands-on blockchain creation', 8, 0, 0),
(153, 13, 'Security in Blockchain', 'Securing blockchain networks', 9, 0, 0),
(154, 13, 'Regulatory Considerations', 'Legal aspects of blockchain', 10, 0, 0),
(155, 13, 'Future of Blockchain', 'Trends and predictions', 11, 0, 0),
(156, 13, 'Bonus: Advanced Blockchain Applications', 'Exploring innovative blockchain uses', 12, 1, 300),

-- Course 14: Cloud Computing with AWS (Modules 157-168)
(157, 14, 'Introduction to AWS', 'Overview of AWS cloud services', 1, 0, 0),
(158, 14, 'AWS Core Services', 'Understanding essential AWS services', 2, 0, 0),
(159, 14, 'EC2 and Compute Services', 'Launching and managing EC2 instances', 3, 0, 0),
(160, 14, 'Storage Solutions', 'Overview of AWS storage options', 4, 0, 0),
(161, 14, 'Networking in AWS', 'Configuring VPCs and subnets', 5, 0, 0),
(162, 14, 'AWS Security', 'Securing AWS resources', 6, 0, 0),
(163, 14, 'Database Services', 'Using AWS database solutions', 7, 0, 0),
(164, 14, 'Serverless Computing', 'Introduction to Lambda and serverless', 8, 0, 0),
(165, 14, 'Monitoring and Management', 'Tools for AWS monitoring', 9, 0, 0),
(166, 14, 'Cost Optimization', 'Managing and reducing AWS costs', 10, 0, 0),
(167, 14, 'Case Study: AWS in Action', 'Real-world AWS deployment examples', 11, 0, 0),
(168, 14, 'Bonus: Advanced AWS Architecting', 'Designing robust AWS infrastructures', 12, 1, 350),

-- Course 15: Artificial Intelligence in Practice (Modules 169-180)
(169, 15, 'Introduction to AI', 'Overview of artificial intelligence', 1, 0, 0),
(170, 15, 'AI and Machine Learning', 'Fundamental AI and ML concepts', 2, 0, 0),
(171, 15, 'Neural Networks Deep Dive', 'Understanding the architecture of neural networks', 3, 0, 0),
(172, 15, 'Natural Language Processing', 'Processing human language with AI', 4, 0, 0),
(173, 15, 'Computer Vision', 'Image processing and recognition', 5, 0, 0),
(174, 15, 'Reinforcement Learning', 'Learning from interaction', 6, 0, 0),
(175, 15, 'Ethics in AI', 'Understanding AI ethics and bias', 7, 0, 0),
(176, 15, 'Data Preparation for AI', 'Cleaning and preparing data', 8, 0, 0),
(177, 15, 'Model Training and Evaluation', 'Training AI models and evaluating performance', 9, 0, 0),
(178, 15, 'Deployment of AI Solutions', 'Bringing AI models into production', 10, 0, 0),
(179, 15, 'Case Studies in AI', 'Real-world AI applications', 11, 0, 0),
(180, 15, 'Bonus: Advanced AI Model Optimization', 'Techniques to enhance AI performance', 12, 1, 450);

-- Insert Tags
INSERT INTO Tags (TagId, name)
VALUES 
(1, 'Programming'),
(2, 'Web Development'),
(3, 'Database'),
(4, 'Beginner'),
(5, 'Machine Learning'),
(6, 'Cybersecurity'),
(7, 'Cloud Computing'),
(8, 'Game Development');

-- Insert CourseTags
INSERT INTO CourseTags (CourseId, TagId)
VALUES 
(1, 1), (1, 4),
(2, 1), (2, 2),
(3, 3), (3, 4),
(4, 1), (4, 2),
(5, 1), (5, 5),
(6, 5), (6, 1),
(7, 6),
(8, 2), (8, 4),
(9, 7),
(10, 2),
(11, 1),
(12, 8),
(13, 7),
(14, 7),
(15, 5);

SELECT * FROM UserProgress;
