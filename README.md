Employee Records Project

A full-stack Employee Records Management system with:

-Backend: .NET 9 Web API (REST API, Entity Framework Core, ASP.NET Identity, Swagger)
-Frontend: React (basic app for interacting with the API)
-Database: SQL Server (Code-First with EF Core)
-Authentication: JWT with role-based access (Admin, Employee)

🚀 Features

Authentication & Roles
 -Login with JWT (no registration, only admin can create users)

 Two roles:
 -Administrator: manage users, projects, and tasks
 -Employee: manage own profile, view and update tasks

Functionality
 -Users: Admin can create/update/delete users
 -Projects: Admin can create projects and assign/remove employees
 -Tasks: Employees can create tasks in projects they belong to; mark tasks as completed
 -Profile: Employees can edit their data and upload profile pictures

🛠️ Technologies
 -Backend: .NET SDK 8.0/9.0, ASP.NET Core, EF Core, Dapper (optional), SQL Server
 -Frontend: React JS
 -Tools: Visual Studio 2022, SQL Server Management Studio 19, Swagger/Scalar for API docs
 -Extras: xUnit (unit tests)

⚙️ Setup & Run
1. Clone the repository
git clone https://github.com/LeoJahaj/EmployeeRecords.git
cd EmployeeRecords

2. Backend (.NET API)

Navigate to the API folder:

cd EmployeeReccordsApi


Restore packages:

dotnet restore


Apply migrations and update database:

dotnet ef database update


Run the API:

dotnet run

The API will run at https://localhost:5001 or http://localhost:5000.

3. Frontend (React)

Navigate to the frontend folder:

cd ../employee-frontend


Install dependencies:

npm install


Start the React app:

npm start


The app will run at http://localhost:3000.
