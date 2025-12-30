🧠 AI Notes App

Azure-Ready Full-Stack Application
(React + ASP.NET Core + Azure OpenAI + Azure SQL Database)

An AI-powered notes application built to demonstrate real-world Azure development practices, modern frontend architecture, and secure AI integration using Azure OpenAI.

This project is intentionally designed as a portfolio-grade showcase for Azure Developer and Full-Stack Engineer roles.

🌟 Highlights

✅ Azure OpenAI (GPT-4o-mini) integration
✅ Azure SQL Database for relational cloud persistence
✅ Entity Framework Core with auto-migrations
✅ CI/CD implemented using GitHub → Azure
✅ Secure secrets management (no keys in Git)
✅ Cloud-ready, production-grade architecture
✅ Modern, accessible UI with Dark / Light mode
✅ Clean, maintainable, TypeScript-safe codebase

🚀 Features
📝 Notes Management

Create, edit, delete notes

Pin / unpin important notes

Search notes instantly (/ shortcut)

Keyboard shortcuts for productivity

Input validation (title & content required)

Character limits with live counter

Responsive, card-based layout

🤖 AI Assistance (Azure OpenAI)

Powered by Azure OpenAI (GPT-4o-mini)

Generate full notes from a title

Summarize notes

Rewrite content

Improve writing quality

Generate tags automatically

AI-generated content badge

Clear AI disclaimer for responsible usage

🔐 Azure OpenAI is accessed securely via backend APIs
➡️ No AI keys are exposed in the frontend.

🗄️ Data Storage (Azure SQL Database)

Azure SQL Database (SQL Server)

Relational schema using Entity Framework Core

Code-first migrations

Retry-enabled SQL connections

Repository pattern for clean data access

Environment-based configuration via Azure App Settings

Fully deployed and running in Azure

📄 Export

Export individual notes to PDF

PDF export available:

Inside the editor

Directly from note cards

🎨 UI / UX

Dark / Light mode toggle (global)

Modal-based note editor

Accessible contrast in dark mode

Modern Tailwind UI

Lucide icons for clean, professional visuals

Keyboard shortcuts tooltip modal (?)

Mobile-friendly grid layout

⌨️ Keyboard Shortcuts
Shortcut	Action
/	Focus search
?	Open shortcuts help
Ctrl / Cmd + Enter	Save note
Esc	Close modal
🔐 Security & Best Practices

❌ No secrets committed to Git
✅ Environment-based configuration
✅ GitHub secret scanning compatible
✅ Azure App Settings compatible
✅ Backend-only AI & database key usage
✅ Production-safe architecture

🏗️ Tech Stack
Frontend

React + TypeScript

Vite

Tailwind CSS

Axios

Lucide Icons

Backend

ASP.NET Core Web API

Entity Framework Core

Azure SQL Database

Azure OpenAI (GPT-4o-mini)

RESTful API design

Cloud & DevOps

Azure App Service (Backend)

Azure Static Web Apps (Frontend)

Azure SQL Database

Azure OpenAI (Microsoft Azure AI / Foundry)

CI/CD via GitHub Actions

Environment variables & Azure App Settings

📁 Project Structure
NotesApp/
├─ Backend/
│  ├─ NotesApp.Api/
│  │  ├─ Controllers/
│  │  ├─ Services/
│  │  ├─ Repositories/
│  │  ├─ Program.cs
│  │  ├─ appsettings.json
│  │  └─ appsettings.Development.json (ignored)
│  │
│  ├─ NotesApp.Application/
│  ├─ NotesApp.Domain/
│  └─ NotesApp.Infrastructure/
│
├─ Frontend/
│  └─ notesapp-ui/
│     ├─ src/
│     │  ├─ components/
│     │  │  ├─ ai/
│     │  │  ├─ notes/
│     │  │  ├─ layout/
│     │  │  └─ ui/
│     │  ├─ features/
│     │  ├─ hooks/
│     │  ├─ services/
│     │  └─ utils/
│     └─ main.tsx
│
└─ README.md

🔐 Configuration & Secrets

Important: Secrets are never committed to source control.

Required Azure OpenAI Environment Variables (Backend)
AzureOpenAI__ApiKey
AzureOpenAI__Endpoint
AzureOpenAI__DeploymentName

Required Azure SQL Environment Variables (Backend)
ConnectionStrings__Default

Local Development Configuration

Create this file locally only:

Backend/NotesApp.Api/appsettings.Development.json


Example:

{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=NotesAppDb;User Id=sa;Password=YourStrongPassword!;TrustServerCertificate=True;"
  },
  "AzureOpenAI": {
    "ApiKey": "YOUR_AZURE_OPENAI_KEY",
    "Endpoint": "https://your-resource.openai.azure.com/",
    "DeploymentName": "gpt-4o-mini"
  }
}


✔️ File is ignored via .gitignore
✔️ GitHub secret scanning blocks accidental leaks

▶️ Running the Project Locally
Backend (ASP.NET Core API)
cd Backend/NotesApp.Api
dotnet restore
dotnet run


Runs at:

https://localhost:7110


Swagger UI:

https://localhost:7110/swagger

Frontend (React)
cd Frontend/notesapp-ui
npm install
npm run dev


Runs at:

http://localhost:5173

☁️ Azure Deployment

Frontend: Azure Static Web Apps

Backend: Azure App Service

Database: Azure SQL Database

AI: Azure OpenAI (GPT-4o-mini)

Secrets: Azure App Settings

CI/CD: GitHub → Azure

⚠️ Disclaimer

AI-generated content is provided for informational purposes only and is not a substitute for professional medical advice, diagnosis, or treatment.

🎯 Why This Project?

This project demonstrates:

Real Azure OpenAI integration

Relational cloud data modeling with Azure SQL

Secure secret management

Clean API & repository design

Entity Framework Core best practices

Modern React architecture

UX decisions for AI-powered features

CI/CD pipelines with GitHub & Azure

Designed to reflect real Azure developer responsibilities, not just a UI demo.

🛣️ Future Enhancements

Authentication & authorization (Azure Entra ID)

Multi-user support

Advanced tagging & filtering

Full-text search

Audit logs & activity history

Application Insights dashboards & alerts

Rate limiting & API throttling

Offline-first support

Docker & containerized deployment

👩‍💻 Author

Asma Hafeez Khan
Azure Developer | Full-Stack Engineer

⭐ Support

If you find this project useful, please consider ⭐ starring the repository.
