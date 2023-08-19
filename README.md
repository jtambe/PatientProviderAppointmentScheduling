This is a Backend API Solution for Patient - Provider Appointments Scheduling

<br/> _Following are important models used_ <br/>
1. Client (Patient)
2. Provider
3. ProviderSchedule
4. Reservation

<br/> _Following requirements are addressed in the code_ <br/>
1. A Provider can submit schedules for multiple days in one single API call
2. A Client can reserve an available slot
3. A Client can confirm the reservation

<br/> _Following assumptions are made in this prototype solution_ <br/>
1. ProviderSchedule is passed for each day in a list/array as input 
2. Assuming that Front-End passes time converted to UTC time when making requests. For the sake of ease in this prototype, I have simply used DateTime.Now instead of DateTime.UtcNow in code
3. Once the provider schedule is submitted, it is considered confirmed and unchangeable.
4. When submitting provider's available schedule for each day, I am assuming there is one long stretch of hours are submitted. For example for Aug 18, there can only be one available schedule of maybe 5-6 hours
5. Clients are required to book 15 minutes of slots

<br/> _Following constraints are verified as per requirements/assumptions when making reservations/confirmation_ <br/>
1. Is the requested slot endtime <= requested slot starttime
2. Is provider scheduled for the day and time, that fits the requested slot? if not return null
3. Is there another reservation that is confirmed?
4. Is there another pending/unconfirmed reservation but reservationTime was less than 30 minutes ago?
5. Reservation needs to be done 24 hours before
6. Assumption that reservations are only allowed for 15 minutes
7. Confirmation is successful only if completed within 30 minutes after reservation time

<br/> <b>_Database_</b> <br/>
1. I have used Dapper as ORM model and used local SQL database.
2. You will need to modify you connection string in appsettings.json
2. You will need to create following tables in SQL database 

```
CREATE TABLE [dbo].[Client]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[ClientEmail] [nvarchar](100) NULL,
	[ClientPhone] [nvarchar](15) NULL,
	CONSTRAINT PK_Client PRIMARY KEY (ID)
)

CREATE TABLE [dbo].[Provider]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[ProviderEmail] [nvarchar](100) NULL,
	[ProviderPhone] [nvarchar](15) NULL,
	CONSTRAINT PK_Provider PRIMARY KEY (ID) 
)


CREATE TABLE [dbo].[ProviderSchedule]
(
	Id INT Identity,
	ProviderId INT NOT NULL,
	StartTime DateTime NOT NULL,
	EndTime DateTime NOT NULL,
	CONSTRAINT PK_ProviderSchedule PRIMARY KEY (Id),
	CONSTRAINT FK_ProviderSchedule_ProviderId FOREIGN KEY (ProviderId) REFERENCES [Provider](Id)
)


CREATE TABLE [dbo].[Reservation]
(
	Id INT Identity,
	ProviderId INT NOT NULL,
	ClientId INT NOT NULL,
	StartTime DateTime NOT NULL,
	EndTime DateTime NOT NULL,
	ReservationTime DateTime NOT NULL DEFAULT GETUTCDATE(),
	IsConfirmed BIT NOT NULL Default(0),
	ConfirmationTime DateTime,
	CONSTRAINT PK_Reservation PRIMARY KEY (Id),
	CONSTRAINT FK_Reservation_ProviderId FOREIGN KEY (ProviderId) REFERENCES [Provider](Id),
	CONSTRAINT FK_Reservation_ClientId FOREIGN KEY (ClientId) REFERENCES [Client](Id)
)
```


<br/> _Noteworthy topics/points covered in this solution_ <br/>
1. Business Logic is written separate, in Services
2. Data access is written separate, in Repositories
3. App level (services + repository) dependencies are handled in separate file viz. Dependencies.cs
4. This solution shows SOLID principals such as Separation of Concern, Interface segregation, Dependency injection
5. Used Swagger UI modifications on Schema definitions for swagger UI page
6. Added try catch blocks
7. Added logging using ILogger
8. Used SQL DB for operations (not just dummy data). Scripts for table is also shared in this README.md
9. Added Unit Tests for Business Logic in Services code