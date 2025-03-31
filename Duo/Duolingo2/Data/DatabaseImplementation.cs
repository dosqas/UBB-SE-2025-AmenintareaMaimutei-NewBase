using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace Duo.Data
{
    public class DatabaseInitializer
    {
        private readonly DatabaseConnection _dbConnection;

        public DatabaseInitializer(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task InitializeDatabaseAsync()
        {
            using (var connection = _dbConnection.GetConnection())
            {
                await connection.OpenAsync();

                var commands = new[]
                {
                @"CREATE TABLE IF NOT EXISTS Users (
                    UserId INT PRIMARY KEY,
                    Username VARCHAR(255) NOT NULL UNIQUE,
                    Email VARCHAR(255) NOT NULL UNIQUE,
                    CreatedAt DATETIME2
                )",

                @"CREATE TABLE IF NOT EXISTS CourseTypes (
                    TypeId INT PRIMARY KEY,
                    TypeName VARCHAR(100) NOT NULL UNIQUE,
                    Price DECIMAL(10,2) NOT NULL CHECK (Price >= 0)
                )",

                @"CREATE TABLE IF NOT EXISTS Courses (
                    CourseId INT PRIMARY KEY,
                    Title VARCHAR(255) NOT NULL,
                    Description TEXT,
                    TypeId INT NOT NULL,
                    CreatedAt DATETIME2,
                    DifficultyLevel INT NOT NULL,
                    TimerDurationMinutes INT NOT NULL,
                    TimerCompletionReward DECIMAL(10,2) NOT NULL,
                    CompletionReward DECIMAL(10,2) NOT NULL,
                    FOREIGN KEY (TypeId) REFERENCES CourseTypes(TypeId) ON DELETE CASCADE
                )",

                @"CREATE TABLE IF NOT EXISTS Modules (
                    ModuleId INT PRIMARY KEY,
                    CourseId INT NOT NULL,
                    Title VARCHAR(255) NOT NULL,
                    Description TEXT,
                    Position INT NOT NULL,
                    IsBonusModule BIT NOT NULL DEFAULT 0,
                    UnlockCost DECIMAL(10,2) NOT NULL DEFAULT 0,
                    FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE
                )",

                @"CREATE TABLE IF NOT EXISTS Tags (
                    TagId INT PRIMARY KEY,
                    TagName VARCHAR(50) NOT NULL UNIQUE
                )",

                @"CREATE TABLE IF NOT EXISTS CourseTags (
                    CourseId INT NOT NULL,
                    TagId INT NOT NULL,
                    PRIMARY KEY (CourseId, TagId),
                    FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE,
                    FOREIGN KEY (TagId) REFERENCES Tags(TagId) ON DELETE CASCADE
                )",

                @"CREATE TABLE IF NOT EXISTS Enrollment (
                    UserId INT NOT NULL,
                    CourseId INT NOT NULL,
                    EnrolledAt DATETIME2,
                    PRIMARY KEY (UserId, CourseId),
                    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
                    FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE
                )",

                @"CREATE TABLE IF NOT EXISTS UserProgress (
                    ProgressId INT PRIMARY KEY,
                    UserId INT NOT NULL,
                    CourseId INT NOT NULL,
                    ModuleId INT NOT NULL,
                    Status VARCHAR(250) NOT NULL,
                    LastUpdated DATETIME2,
                    UNIQUE (UserId, CourseId, ModuleId),
                    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE NO ACTION,
                    FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE NO ACTION,
                    FOREIGN KEY (ModuleId) REFERENCES Modules(ModuleId) ON DELETE NO ACTION
                )",

                @"CREATE TABLE IF NOT EXISTS UserCoins (
                    UserId INT PRIMARY KEY,
                    CoinBalance DECIMAL(10,2) DEFAULT 0,
                    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
                )",

                @"CREATE TABLE IF NOT EXISTS Pictures (
                    PictureId INT PRIMARY KEY,
                    UserId INT NOT NULL,
                    ImageUrl VARCHAR(500) NOT NULL,
                    UploadedAt DATETIME2,
                    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE NO ACTION
                )",

                @"CREATE TABLE IF NOT EXISTS PictureCoins (
                    UserId INT NOT NULL,
                    PictureId INT NOT NULL,
                    CoinsReceived DECIMAL(10,2) DEFAULT 0,
                    PRIMARY KEY (UserId, PictureId),
                    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
                    FOREIGN KEY (PictureId) REFERENCES Pictures(PictureId) ON DELETE CASCADE
                )",

                @"CREATE TABLE IF NOT EXISTS CourseTimer (
                    TimerId INT PRIMARY KEY,
                    UserId INT NOT NULL,
                    CourseId INT NOT NULL,
                    ElapsedTimeMinutes INT NOT NULL DEFAULT 0,
                    LastUpdated DATETIME2,
                    IsCompleted BIT NOT NULL DEFAULT 0,
                    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
                    FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE
                )",

                @"CREATE TABLE IF NOT EXISTS UserModuleUnlock (
                    UserId INT NOT NULL,
                    ModuleId INT NOT NULL,
                    UnlockedAt DATETIME2,
                    PRIMARY KEY (UserId, ModuleId),
                    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
                    FOREIGN KEY (ModuleId) REFERENCES Modules(ModuleId) ON DELETE CASCADE
                )",

                @"CREATE TABLE IF NOT EXISTS DailyLoginReward (
                    RewardId INT PRIMARY KEY,
                    UserId INT NOT NULL,
                    RewardDate DATETIME2,
                    CoinsReceived DECIMAL(10,2) NOT NULL,
                    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
                )",

                @"CREATE TABLE IF NOT EXISTS ModuleImages (
                    ModuleId INT,
                    PictureId INT,
                    PRIMARY KEY (ModuleId, PictureId),
                    FOREIGN KEY (ModuleId) REFERENCES Modules(ModuleId) ON DELETE CASCADE,
                    FOREIGN KEY (PictureId) REFERENCES Pictures(PictureId) ON DELETE CASCADE
                )"
            };

                foreach (var commandText in commands)
                {
                    using (var command = new SqlCommand(commandText, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }

                // Create indexes
                var indexCommands = new[]
                {
                "CREATE INDEX IF NOT EXISTS idx_course_title ON Courses(Title)",
                "CREATE INDEX IF NOT EXISTS idx_course_tags ON CourseTags(TagId)",
                "CREATE INDEX IF NOT EXISTS idx_user_progress ON UserProgress(UserId, CourseId)",
                "CREATE INDEX IF NOT EXISTS idx_course_timer ON CourseTimer(UserId, CourseId)",
                "CREATE INDEX IF NOT EXISTS idx_daily_login ON DailyLoginReward(UserId, RewardDate)"
            };

                foreach (var indexCommand in indexCommands)
                {
                    using (var command = new SqlCommand(indexCommand, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }
    }
}