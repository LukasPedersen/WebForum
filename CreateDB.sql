--if Database exists drop it
Use master
GO
EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'Forum'
GO
ALTER DATABASE [Forum] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
GO
USE [master]
GO
DROP DATABASE [Forum]
GO
--Create database
CREATE DATABASE Forum
GO
USE Forum
GO
--Create tables
CREATE TABLE Users(
	users_ID INT NOT NULL IDENTITY PRIMARY KEY,
	f_name VARCHAR(50),
	l_name VARCHAR(50),
	email VARCHAR(50),
	userSince DATE,
	active BIT,
);
GO
CREATE TABLE Login(
	login_ID INT NOT NULL IDENTITY PRIMARY KEY,
	users_ID INT UNIQUE FOREIGN KEY REFERENCES Users(users_ID),
	userName VARCHAR(50),
	"passWord" VARCHAR(50),
);
GO
CREATE TABLE Topics(
	topics_ID INT NOT NULL IDENTITY PRIMARY KEY,
	tag_ID INT,
	forfatterUser_ID INT,
	chatRoom_ID INT,
	headder VARCHAR(100),
	"text" VARCHAR(300),
	forfatter VARCHAR(50),
);

GO
CREATE TABLE Threads(
	thread_ID INT NOT NULL IDENTITY PRIMARY KEY,
	users_ID INT FOREIGN KEY REFERENCES Users(users_ID),
	topics_ID INT FOREIGN KEY REFERENCES Topics(topics_ID),
	tag_ID INT,
	headder VARCHAR(100),
	content VARCHAR(300),
	forfatter VARCHAR(50),
);
GO
CREATE TABLE Tags(
	tag_ID INT NOT NULL IDENTITY PRIMARY KEY,
	thread_ID INT FOREIGN KEY REFERENCES Threads(thread_ID),
	tagName VARCHAR(50),
);
GO

CREATE TABLE Chat(
	chat_ID INT NOT NULL IDENTITY PRIMARY KEY,
	sender VARCHAR(50),
	content VARCHAR(50),
);
GO
CREATE TABLE Users_Chat(
	users_Chat_ID INT NOT NULL IDENTITY PRIMARY KEY,
	user1_ID INT FOREIGN KEY REFERENCES Users(users_ID),
	user2_ID INT FOREIGN KEY REFERENCES Users(users_ID),
	chat_ID INT FOREIGN KEY REFERENCES Chat(chat_ID),
);

--Create stored procedures

GO
--Create new topic
CREATE PROCEDURE spCreateTopic @forfatterUser_ID INT, @headder VARCHAR(100), @text VARCHAR(300), @forfatter VARCHAR(50)
AS
BEGIN
	INSERT INTO Topics(forfatterUser_ID, headder, text, forfatter)
	VALUES (@forfatterUser_ID, @headder, @text, @forfatter)
END
GO
--Delete Topic
CREATE PROCEDURE spDeleteTopic @topics_ID INT, @forfatter VARCHAR(50)
AS
DELETE FROM Topics WHERE topics_ID = @topics_ID AND forfatter = @forfatter
GO
--Create new Thread
CREATE PROCEDURE spCreateThread @users_ID INT, @topics_ID INT, @headder VARCHAR(100), @content VARCHAR(300), @forfatter VARCHAR(50)
AS
BEGIN
	INSERT INTO dbo.Threads(users_ID, topics_ID, headder, content, forfatter)
	VALUES (@users_ID, @topics_ID, @headder, @content, @forfatter)
END
GO
-- Get all topics from username
CREATE PROCEDURE spGetAllTopicsFromForfatter @forfatter VARCHAR(50)
AS
SELECT * FROM Topics WHERE forfatter = @forfatter
GO
-- Get all threads from username
CREATE PROCEDURE spGetAllThreadsFromForfatter @users_ID INT
AS
SELECT * FROM Threads WHERE users_ID = @users_ID
GO
--Get all Topics
CREATE PROCEDURE spGetAllTopics
AS
SELECT topics_ID, chatRoom_ID, headder, text, forfatter FROM Topics
GO
--Create user
CREATE PROCEDURE spCreateUser @username VARCHAR(50), @password VARCHAR(50), @f_name VARCHAR(50), @l_name VARCHAR(50), @email VARCHAR(50), @active BIT
AS
BEGIN
INSERT INTO Users (f_name, l_name, email, userSince, active)
VALUES (@f_name, @l_name, @email, GETDATE(), @active)

DECLARE @lastIdentity as INT
SET @lastIdentity = @@IDENTITY


INSERT INTO Login (users_ID, userName, passWord)
VALUES (@lastIdentity, @username, @password)
END
GO
--Create new tag for tag table
CREATE PROCEDURE spCreateTag @tagName VARCHAR(50)
AS
INSERT INTO Tags (tagName)
VALUES (@tagName)

--Insert data
GO
--Create some users
EXEC spCreateUser @username = 'Luke Skywalker', @password = 'DeathStar', @f_name = 'Lukas', @l_name = 'Pedersen', @email = 'luka0591@elevcampus.dk', @active = 1
EXEC spCreateUser @username = 'Jonas Skywalker', @password = 'DeathStar', @f_name = 'Jonas', @l_name = 'Lassen', @email = 'blop3163@elevcampus.dk', @active = 1
EXEC spCreateUser @username = 'CoolMads123', @password = '123', @f_name = 'Mads', @l_name = 'Madsen', @email = 'Mads8572@elevcampus.dk', @active = 1
EXEC spCreateUser @username = 'Is', @password = 'Furry', @f_name = 'Hans', @l_name = 'lolsis', @email = 'Hans6381@elevcampus.dk', @active = 0
EXEC spCreateUser @username = 'KanViKlareDet', @password = 'JaViKan', @f_name = 'Bob', @l_name = 'HanBygger', @email = 'bob3172@elevcampus.dk', @active = 1
GO
--Create some topics
EXEC spCreateTopic @forfatterUser_ID = 1, @headder = 'counter strike global offensive', @text = 'Counter-Strike: Global Offensive (CS:GO) expands upon the team-based first person shooter gameplay the original Counter-Strike pioneered when it launched in 1999. Two teams compete in multiple rounds of objective-based game modes with the goal of winning enough rounds to win the match.', @forfatter = 'test'
GO
--Create some threads
EXEC spCreateThread @users_ID = 1, @topics_ID = 1, @headder = 'how to level up fast', @content = 'Get gud kiddo', @forfatter = 'test'
EXEC spCreateThread @users_ID = 2, @topics_ID = 1, @headder = 'Really', @content = '"Get gud kiddo" go F**k yourself', @forfatter = 'Jonas skywalker'
GO
SELECT * FROM Topics