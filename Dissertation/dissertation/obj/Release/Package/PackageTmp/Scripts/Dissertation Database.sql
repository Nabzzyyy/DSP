---Creating tables:
CREATE TABLE UserDetails(
	UserID INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName VARCHAR(20) NOT NULL,
	LastName VARCHAR(20) NOT NULL,
	Email VARCHAR(100) UNIQUE NOT NULL,
	UserPassword VARCHAR(MAX) NOT NULL,
)
GO

CREATE TABLE ClientInformation(
	ClientID INT PRIMARY KEY IDENTITY NOT NULL,
	Nickname VARCHAR(20) NOT NULL,
	ComputerName VARCHAR(30) NOT NULL,
	CurrentIP VARCHAR(40) NOT NULL,
	OS VARCHAR(10) NOT NULL,
	LastStatus DateTime NOT NULL,
	ClientHash varchar(500) NOT NULL  
)
GO

CREATE TABLE Tokens(
	ID INT PRIMARY KEY IDENTITY NOT NULL,
	Token varchar(100) NOT NULL,
	ExpiryDateTime datetime NOT NULL,
	UserID INT NOT NULL, 
	FOREIGN KEY (UserID) REFERENCES UserDetails(UserID)
)
GO

CREATE TABLE UserAndClient(
	ClientID INT NOT NULL,
	UserID INT NOT NULL,
	FOREIGN KEY (UserID) REFERENCES UserDetails(UserID),
	FOREIGN KEY (ClientID) REFERENCES ClientInformation(ClientID)
)
GO

CREATE TABLE RefAlertType(
	AlertTypeID INT PRIMARY KEY IDENTITY NOT NULL,
	Name VARCHAR(20) NOT NULL
)
GO

CREATE TABLE Alert(
	AlertID INT PRIMARY KEY IDENTITY NOT NULL,
	Screenshot VARCHAR(MAX) NOT NULL,
	Keyword VARCHAR(20) NOT NULL,
	Location VARCHAR(MAX) NOT NULL,
	DateAndTime datetime NOT NULL,
	AlertTypeID INT NOT NULL,
	ClientID INT NOT NULL,
	FOREIGN KEY (AlertTypeID) REFERENCES RefAlertType(AlertTypeID),
	FOREIGN KEY (ClientID) REFERENCES ClientInformation(ClientID)
)
GO

CREATE TABLE RefDnsType(
	DnsTypeID INT PRIMARY KEY IDENTITY NOT NULL,
	DnsName VARCHAR(30) NOT NULL,
	PreferDns VARCHAR(30) NOT NULL,
	AlternativeDns VARCHAR(30) NOT NULL,
)
GO

CREATE TABLE ToS(
	TosID INT PRIMARY KEY IDENTITY NOT NULL,
	StartMinute INT NOT NULL,
	EndMinute INT NOT NULL,
	day INT NOT NULL,
	ClientID INT NOT NULL,
	FOREIGN KEY (ClientID) REFERENCES ClientInformation(ClientID)
)
GO

CREATE TABLE Settings(
	SettingID INT PRIMARY KEY IDENTITY NOT NULL,
	ClientID INT NOT NULL,
	DnsTypeID INT NOT NULL,
	TosID INT,
	FOREIGN KEY (ClientID) REFERENCES ClientInformation(ClientID),
	FOREIGN KEY (DnsTypeID) REFERENCES RefDnsType(DnsTypeID),
	FOREIGN KEY (TosID) REFERENCES ToS(TosID)
)
GO

CREATE TABLE ClientEmotions (
	EmotionID INT PRIMARY KEY IDENTITY NOT NULL,
    ToneName VARCHAR(15) NOT NULL,
	ToneScore DECIMAL(3,3) NOT NULL,
	ClientID INT NOT NULL,
	UserID INT NOT NULL,
    FOREIGN KEY (UserID) REFERENCES UserDetails(UserID),
	FOREIGN KEY (ClientID) REFERENCES ClientInformation(ClientID)

)
GO

---Creating Stored Procedures:
CREATE PROCEDURE SP_InsertUserDetails(
	@FirstName VARCHAR(20),
	@LastName VARCHAR(20),
	@Email VARCHAR(100),
	@Password VARCHAR(MAX)
)
AS
	INSERT INTO UserDetails (FirstName, LastName, Email, UserPassword)
	VALUES (@FirstName, @LastName, @Email, @Password)
GO

CREATE PROCEDURE SP_InsertClientInformation(
	@NickName VARCHAR(20),
	@ComputerName VARCHAR(30),
	@CurrentIP VARCHAR(40),
	@OS VARCHAR(10),
	@UserID INT,
	@LastStatus DateTime,
	@ClientHash VARCHAR(MAX)
)
AS 
DECLARE @ID INT = 0

	INSERT INTO ClientInformation (Nickname, ComputerName, CurrentIP, OS, LastStatus, ClientHash)
	VALUES (@NickName, @ComputerName, @CurrentIP, @OS, @LastStatus, @ClientHash)

	SET @ID = (SELECT TOP 1 ClientID from ClientInformation ORDER BY ClientID DESC)

	INSERT INTO UserAndClient (ClientID, UserID)
	VALUES (@ID, @UserID)

	SELECT @ID
GO

CREATE PROCEDURE SP_InsertTokens(
	@Token VARCHAR(100),
	@ExpiryDate datetime,
	@UserID INT
)
AS
	INSERT INTO Tokens (Token, ExpiryDateTime, UserID)
	VALUES (@Token, @ExpiryDate, @UserID)
GO

CREATE PROCEDURE SP_InsertAlert(
	@Screenshot VARCHAR(MAX),
	@Keyword VARCHAR(20),
	@Location VARCHAR(MAX),
	@DateAndTime datetime,
	@AlertTypeID INT,
	@ClientID INT
)
AS
	INSERT INTO Alert (Screenshot, Keyword, Location, DateAndTime, AlertTypeID, ClientID)
	VALUES (@Screenshot, @Keyword, @Location, @DateAndTime, @AlertTypeID, @ClientID)
GO

CREATE PROCEDURE SP_InsertToS(
	@StartMinute INT,
	@EndMinute INT,
	@Day INT,
	@ClientID INT
	
)
AS
	INSERT INTO ToS (ClientID, StartMinute, EndMinute, day)
	VALUES (@StartMinute, @EndMinute, @Day, @ClientID)
GO

CREATE PROCEDURE SP_InsertClientEmotions(
	@ToneName VARCHAR(15),
	@ToneScore DECIMAL(3,3),
	@ClientID INT,
	@UserID INT
)
AS
	INSERT INTO ClientEmotions(ToneName, ToneScore, ClientID, UserID)
	VALUES (@ToneName, @ToneScore, @ClientID, @UserID)
GO


---Insert DNS Types:
INSERT INTO RefDnsType (DnsTypeID, DnsName, PreferDns, AlternativeDns) VALUES (1, 'Family-friendly', '94.140.14.15', '94.140.15.16')
GO

INSERT INTO RefDnsType (DnsTypeID, DnsName, PreferDns, AlternativeDns) VALUES (2, 'None', '8.8.8.8', '8.8.4.4')
GO
---Creating View:
CREATE VIEW view_Client AS
SELECT C.ClientID, C.Nickname, C.ComputerName, C.CurrentIP, C.OS, C.LastStatus, A.AlertID, A.Screenshot, A.Keyword, A.Location, A.DateAndTime, A.AlertTypeID, U.UserID
FROM ClientInformation AS  C
LEFT JOIN Alert AS A ON A.ClientID = C.ClientID
INNER JOIN UserAndClient AS UAC ON UAC.ClientID = C.ClientID
INNER JOIN UserDetails AS U ON U.UserID = UAC.UserID
GO


CREATE VIEW view_Setting AS 
SELECT C.ClientID, C.Nickname, C.ComputerName, C.CurrentIP, C.OS, C.LastStatus, S.SettingID, D.DnsName, D.PreferDns, D.AlternativeDns, T.ToSID, T.StartMinute AS ToS_Start_Minute, T.EndMinute AS ToS_End_Minute , T.Day as ToS_Day, UAC.UserID as UserID
FROM ClientInformation AS  C
LEFT JOIN Settings AS S ON S.ClientID = C.ClientID
LEFT JOIN RefDnsType AS D ON D.DnsTypeID = S.DnsTypeID
LEFT JOIN ToS AS T ON T.TosID = S.TosID
INNER JOIN UserAndClient AS UAC ON UAC.ClientID = C.ClientID
GO

---View Devices:
SELECT C.ClientID, C.Nickname, C.LastStatus, COUNT(A.AlertID) AS totalViolations
FROM ClientInformation AS C
LEFT JOIN Alert AS A ON A.ClientID = C.ClientID
INNER JOIN UserAndClient AS UAC ON UAC.ClientID = C.ClientID
INNER JOIN UserDetails AS U ON U.UserID = UAC.UserID
GROUP BY C.ClientID, C.NickName, C.LastStatus
GO

---Getting recent ClientID from userID:
Select TOP 1 ClientID from UserAndClient where UserID = 1 ORDER BY ClientID DESC