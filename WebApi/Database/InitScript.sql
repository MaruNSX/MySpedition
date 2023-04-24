CREATE TABLE Employees 
(
	Id int NOT NULL PRIMARY KEY IDENTITY,
    FirstName nvarchar(255) NOT NULL,
	LastName nvarchar(255) NOT NULL,
	BirthDate datetime NOT NULL,
	CarRegistrationNumber nvarchar(10) NOT NULL
)

GO

CREATE TABLE Orders
(
	Id int NOT NULL PRIMARY KEY IDENTITY,
	StartTime DateTime,
	EndTime DateTime,
	EmployeeId int FOREIGN KEY REFERENCES Employees(Id),
	Price decimal not null
)

