# Employee Records Project

A **full-stack Employee Records Management system** with:\
- Backend: **.NET 9 Web API** (REST API, Entity Framework Core, ASP.NET
Identity, Swagger)\
- Frontend: **React** (basic app for interacting with the API)\
- Database: **SQL Server (Code-First with EF Core)**\
- Authentication: **JWT** with role-based access (`Admin`, `Employee`)

------------------------------------------------------------------------

##  Features

### Authentication & Roles

-   Login with JWT (no registration, only admin can create users)\
-   Two roles:
    -   **Administrator**: manage users, projects, and tasks\
    -   **Employee**: manage own profile, view and update tasks

### Functionality

-   **Users**: Admin can create/update/delete users\
-   **Projects**: Admin can create projects and assign/remove employees\
-   **Tasks**: Employees can create tasks in projects they belong to;
    mark tasks as completed\
-   **Profile**: Employees can edit their data and upload profile
    pictures

------------------------------------------------------------------------

##  Technologies

-   **Backend**: .NET SDK 8.0/9.0, ASP.NET Core, EF Core, Dapper
    (optional), SQL Server\
-   **Frontend**: React JS\
-   **Tools**: Visual Studio 2022, SQL Server Management Studio 19,
    Swagger/Scalar for API docs\
-   **Extras**: xUnit
    (unit tests)

------------------------------------------------------------------------

##  Setup & Run

### 1. Clone the repository

``` bash
git clone https://github.com/LeoJahaj/EmployeeRecords.git
cd EmployeeRecords
```

------------------------------------------------------------------------

### 2. Backend (.NET API)

1.  Navigate to the API folder:

    ``` bash
    cd EmployeeReccordsApi
    ```

2.  Restore packages:

    ``` bash
    dotnet restore
    ```

3.  Apply migrations and update database:

    ``` bash
    dotnet ef database update
    ```

4.  Run the API:

    ``` bash
    dotnet run
    ```

    The API will run at `https://localhost:5001` or
    `http://localhost:5000`.

------------------------------------------------------------------------

### 3. Frontend (React)

1.  Navigate to the frontend folder:

    ``` bash
    cd ../employee-frontend
    ```

2.  Install dependencies:

    ``` bash
    npm install
    ```

3.  Start the React app:

    ``` bash
    npm start
    ```

    The app will run at `http://localhost:3000`.

------------------------------------------------------------------------

##  Notes

-   You need **.NET SDK 9.0** installed.\
-   You need **Node.js** installed (for React).\
-   You need **SQL Server** running locally (or change connection string
    in `appsettings.json`).\
-   Swagger UI is available at: `https://localhost:5001/swagger`\
-   Roles:
    -   **Admin** → full control (users, projects, tasks)\
    -   **Employee** → limited to their own tasks & profile

