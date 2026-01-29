# Smart Resume Builder with AI Job Matching System

An enterprise-ready Resume Builder application built with Clean Architecture, ASP.NET Core 8, and MySQL.

## 🚀 Key Features
- Clean Architecture: Decoupled Domain, Application, and Infrastructure layers.
- AI Job Matching: Rule-based skill extraction and gap analysis.
- Resume Builder: Multi-step wizard with dynamic fields.
- PDF Export: 3 professional templates (Modern, Classic, Corporate) using QuestPDF.
- ASP.NET Identity: Role-based access (Admin/User).
- REST API: JWT-secured endpoints with Swagger UI.
- Unit Testing: Verified business logic with xUnit.
- Professional UI: Responsive dashboard with Bootstrap 5 and Chart.js.

## 📁 Project Structure & File Guide

### 1. ResumeBuilder.Domain
The core of the application containing entities and repository interfaces.
- Entities/: Models like Resume.cs, Skill.cs, and JobRole.cs.
- Interfaces/: Repository contracts like IRepository.cs.

### 2. ResumeBuilder.Application
Contains business logic and services.
- DTOs/: Data Transfer Objects for cross-layer communication.
- Services/: Logic like JobMatchingService.cs and PdfGeneratorService.cs.
- Interfaces/: Service contracts like IResumeService.cs.

### 3. ResumeBuilder.Infrastructure
Handles data persistence and Identity.
- Data/: ApplicationDbContext.cs for database management.
- Repositories/: Implementation of IRepository.
- Identity/: Custom user profiles (ApplicationUser.cs).

### 4. ResumeBuilder.Web
Main MVC interface for users and admins.
- Controllers/: Handles page requests and form submissions.
- Views/: HTML/Razor templates (Index.cshtml, Wizard.cshtml).

### 5. ResumeBuilder.API
Standalone REST API with Swagger documentation.

## 🔄 Project Flow
1. **User Registration**: User signs up via `AccountController`. Identity creates a MySQL entry.
2. **Resume Building**: User completes a 4-step wizard. Data is mapped to `ResumeDto` and saved via `ResumeService`.
3. **AI Matching**: `JobMatchingService` triggers automatically, comparing resume skills against `JobRoles` in MySQL.
4. **Analysis**: Match scores and skill gaps are calculated and saved to the `ResumeJobMatch` table.
5. **Dashboard**: User views stats and charts (Chart.js) showing their market readiness.
6. **Export**: User downloads a professional PDF generated on-the-fly by `PdfGeneratorService` (QuestPDF).

## 🛠️ Database Setup (MySQL)
Default MySQL Connection:
- **Server**: localhost
- **Database**: SmartResumeBuilder
- **Username**: root
- **Password**: password

## 💻 .NET CLI Commands for Database
You can run these commands in the terminal (CMD/PowerShell) from the project root:

| Action | Command |
| :--- | :--- |
| **Apply Migrations** | `dotnet ef database update --project src/ResumeBuilder.Infrastructure` |
| **Add New Migration** | `dotnet ef migrations add NameGoesHere --project src/ResumeBuilder.Infrastructure --startup-project src/ResumeBuilder.Web` |
| **Remove Last Migration** | `dotnet ef migrations remove --project src/ResumeBuilder.Infrastructure` |
| **Drop Database** | `dotnet ef database drop --project src/ResumeBuilder.Infrastructure` |
| **Check Connectivity** | `dotnet ef db context info --startup-project src/ResumeBuilder.Web` |

## 🚀 Execution Steps
1. Open **SmartResumeBuilder.sln** in Visual Studio 2022.
2. Ensure MySQL is running on your machine.
3. Open **Package Manager Console**.
4. Set Default Project to **ResumeBuilder.Infrastructure**.
5. Run: `Update-Database`
6. Set **ResumeBuilder.Web** as Startup Project and press **F5**.

## 🔑 Default Login
- **Admin**: `shweta01818@gmail.com` / `Shweta@24`
- **User**: `shwetap92900@gmail.com` / `Shweta@25`

## Execution (Brief)
1. **Restore & Build**:
   ```bash
   dotnet restore
   dotnet build
   ```
2. **Database Setup**:
   ```bash
   dotnet ef database update --project src/ResumeBuilder.Infrastructure/ResumeBuilder.Infrastructure/ResumeBuilder.Infrastructure.csproj --startup-project src/ResumeBuilder.Web/ResumeBuilder.Web/ResumeBuilder.Web.csproj
   ```
3. **Run Application**:
   ```bash
   dotnet run --project src/ResumeBuilder.Web/ResumeBuilder.Web/ResumeBuilder.Web.csproj
   ```
4. **Access**:
   - Web App: `http://localhost:5000` or `https://localhost:5001`
   - Log in with the credentials above.
