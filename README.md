# TeamTaskManagement
A robust task management system designed for teams, developed using **ASP.NET Core Web API**.  
It supports role-based access control, task assignments, team management, and Redis caching.

---

## 🚀 Features

- ✅ **Role-based Authorization** (`Admin`, `TeamLeader`, `Member`)
- 👥 **Team Management**: Create, delete, and view teams and their members
- 🗂️ **Task Management**:
  - Create, edit, delete task templates
  - Assign tasks to team members
  - Update task status (with approval logic)
- 🛡️ **Authentication & Authorization** using **ASP.NET Core Identity**
- ⚡ **Caching** with **Redis**
- 📦 **Unit of Work & Repository Pattern**
- 🧠 **Specification Pattern** for clean filtering logic
- 📄 **DTOs** and **AutoMapper** for clean API responses

---

## 🏗️ Tech Stack

- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server**
- **Redis**
- **AutoMapper**
- **Identity**
- **Specification Pattern**
- **Unit of Work Pattern**
- **JWT Authentication**

---

## 🔐 Roles & Permissions

| Role        | Permissions                                               |
|-------------|-----------------------------------------------------------|
| `Admin`     | Full control: teams, users, tasks                         |
| `TeamLeader`| Assign tasks, edit task templates, manage members         |
| `Member`    | View assigned tasks, set task status (with approval)      |

---

## 📁 Project Structure

```
TeamTaskManagement/
│
├── TeamTaskManagement.API        # Web API Layer (Controllers, Auth, etc.)
├── TeamTaskManagement.Core       # Entities, Interfaces, Specifications
├── TeamTaskManagement.Service    # Business Logic (Team & Task Services)
├── TeamTaskManagement.Repository # Repositories, Unit of Work
```

---

## 📌 API Highlights

- **[POST] /api/team** - Create a new team (Admin only)
- **[POST] /api/team/AddMember** - Add user to a team
- **[POST] /api/task** - Create a task template
- **[POST] /api/task/assigntask** - Assign task to user (TeamLeader)
- **[PUT] /api/task/edittask/{id}** - Edit task template
- **[PUT] /api/task/setstatus** - Update task status (requires approval)
- **[GET] /api/task/usertasks/{userId}** - Get user's assigned tasks

---

## 🧪 Caching Strategy

- Applied caching on:
  - Teams list
  - Team members
  - User tasks
- Custom `[Cached]` attribute using Redis

---

## 🔧 Getting Started

1. Clone the repo:
   ```bash
   git clone https://github.com/MalakMustafa7/TeamTaskManagement.git
   ```

2. Configure your `appsettings.json` with:
   - SQL Server connection string
   - JWT secrets
   - Redis connection string

3. Apply migrations:
   ```bash
   dotnet ef database update
   ```

4. Run the API:
   ```bash
   dotnet run
   ```

---

## 📌 Future Enhancements

- ✅ Notification system for task updates
- 📊 Task analytics dashboard
- 🔁 Background jobs with Hangfire
- 🧪 Unit + Integration testing

---
