# TaskFlow API

## Description

TaskFlow is a RESTful API built with ASP.NET Core that allows users to manage projects and tasks.

It includes authentication with JWT, role-based authorization (User/Admin), and global exception handling.

---

## Features

* User authentication (JWT)
* User registration & login
* Project management (CRUD)
* Task management (CRUD)
* Role-based authorization (Admin/User)
* Global exception handling middleware
* Swagger API documentation

---

## Technologies

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* JWT Authentication
* Swagger (OpenAPI)

---

## Setup & Installation

1. Clone the repository

```
git clone https://github.com/hyunahparc/task-c-mini-project.git
cd taskflow
```

2. Create `appsettings.Development.json` in the project root:

```
{
  "ConnectionStrings": {
    "DefaultConnection": "your_connection_string"
  },
  "Jwt": {
    "Key": "your_jwt_secret_key"
  }
}
```

3. Apply migrations

```
dotnet ef database update
```

4. Run the project

```
dotnet run
```

---

## Authentication

* Register: `POST /api/users/register`
* Login: `POST /api/users/login`
* Use the returned JWT token in:

```
Authorization: Bearer YOUR_TOKEN
```

---

## API Endpoints

### Auth

* POST `/api/users/register`
* POST `/api/users/login`

### Projects

* GET `/api/projects`
* GET `/api/projects/{id}`
* POST `/api/projects`
* PUT `/api/projects/{id}`
* DELETE `/api/projects/{id}`

### Tasks

* GET `/api/tasks`
* GET `/api/tasks/{id}`
* POST `/api/tasks`
* PUT `/api/tasks/{id}`
* DELETE `/api/tasks/{id}`

---

## Swagger

Run the app and open:

```
https://localhost:xxxx/swagger
```
