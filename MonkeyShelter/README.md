﻿ASP.NET Core Web API project for managing a monkey shelter — arrival, departure, weight, vet check up and reporting.

Built with:

ASP.NET Core Web API

Dapper - for fast and simple database access

SQLite - database for easy testing

Console App — for manual interaction with the api


Setup Instructions
1. Clone the Repository

git clone https://github.com/velja11/MonkeyShelter

2. Open the Project

Open the solution in Visual Studio 

3. Configure the Database

SQLite database is created. The db file MonkeyShelter is in Data folder. For better tracking, download Sqlite studio.
3 IDSpecies are created.

The connection string is defined in appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Data Source=Data/monkeyShelter.db"
}


4. Install Required Packages

dotnet restore
(dotnet add package Dapper)
(dotnet add package Microsoft.Data.Sqlite)

5. Run the Application
   MonkeyShelter(Api project)
   dotnet run

   MonkeyShelter.DemoApp(console app)
   dotnet run

7. Swagger (OpenAPI UI) will be available at:

https://localhost:7265/swagger

You can use Swagger UI to test the following endpoints:

Add a monkey	/api/Monkey/arrival	POST
Depart a monkey	/api/Monkey/departure/{id}	PATCH
Update monkey weight	/api/Monkey/{id}/weight	PATCH
Get monkeys for vet checks	/api/Monkey/vet-checks	GET
Update last vet check	/api/Monkey/{id}/vet-check	PATCH
Reports - Monkeys per species	/api/Report/monkey-count-per-species	GET
Reports - Arrivals between dates	/api/Report/monkey-arrivals-between-dates	GET

The project also includes a Console Application -MonkeyShelter.DemoApp- for manual interaction and testing API



-- Main Features --

Arrival of Monkeys

Limit: maximum 7 arrivals per day

Departure of Monkeys

Limit: maximum 5 departures per day

Safety checks to maintain shelter balance

Weight Updates

Only for active (non-departed) monkeys

Vet Check Management

Required every 60 days

Reporting

Monkey count per species

Arrivals between specific dates

--Project Structure--

Layer	Description
Controllers - Handles API endpoints
Repositories - Database operations using Dapper
Models / DTOs - Defines entities and data transfer objects
Responses - Standardized output (success, data, error messages)


--Validation and Business Rules--
 
 - Cannot add more than 7 monkeys per day

 - Cannot deparute more than 5 monkeys per day

 - Cannot have more departures than arrivals (with max +2 tolerance)

 - Monkey weight must be > 0

 - Cannot update data for departed monkeys

--ConsoleApp - MonkeyShelter.DemoApp --


Features available via Console App:

-View existing monkeys

-Register monkey arrivals

-Register departures

-Update monkey weights

-Perform vet check updates

-View basic reports directly from the console

 - Cannot allow the last monkey of a species to depart
