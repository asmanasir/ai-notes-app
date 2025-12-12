AI Notes App
Azure-Ready Full-Stack Application (React + ASP.NET Core + Azure OpenAI)

An AI-powered notes application built to demonstrate real-world Azure development practices, modern frontend architecture, and secure AI integration using Azure OpenAI.

This project is intentionally designed as a portfolio showcase for Azure Developer roles.

🌟 Highlights

✅ Azure OpenAI integration
✅ Secure secrets management (no keys in Git)
✅ Cloud-ready architecture
✅ Production-quality UI/UX
✅ Clean, maintainable codebase

🚀 Features
📝 Notes Management

Create, edit, delete notes

Pin / unpin important notes

Input validation (title & content required)

Character limits with live counter

Responsive, card-based layout

🤖 AI Assistance (Azure OpenAI)

Summarize notes

Rewrite content

Improve writing

Generate tags

Generate full notes from a title

AI-generated content badge

Informational disclaimer for AI output

📄 Export

Export individual notes to PDF

PDF export available both in editor & note cards

🎨 UI / UX

Dark / Light mode toggle (global)

Modal-based editor

Accessible contrast in dark mode

Modern Tailwind design

Mobile-friendly grid layout

🔐 Security & Best Practices

No secrets committed to Git

Environment-based configuration

GitHub secret scanning compatible

Azure-ready configuration structure

🏗️ Tech Stack
Frontend

React + TypeScript

Vite

Tailwind CSS

Axios

Backend

ASP.NET Core Web API

Azure OpenAI

RESTful API design

Cloud & DevOps

Azure App Service (planned)

Azure Static Web Apps (planned)

GitHub

Environment variables & appsettings

📁 Project Structure
NoteApp/
├─ Backend/
│  └─ NotesApp.Api/
│     ├─ Controllers/
│     ├─ Services/
│     ├─ Models/
│     ├─ appsettings.json
│     ├─ appsettings.Development.json (ignored)
│     └─ Program.cs
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

Required Azure OpenAI Settings
Backend Environment Variables
AzureOpenAI__ApiKey
AzureOpenAI__Endpoint

Local Development Configuration

Create the following file locally only:

Backend/NotesApp.Api/appsettings.Development.json


Example:

{
  "AzureOpenAI": {
    "ApiKey": "YOUR_AZURE_OPENAI_KEY",
    "Endpoint": "https://your-resource-name.openai.azure.com/"
  }
}


✔️ This file is ignored via .gitignore
✔️ GitHub secret scanning will block accidental leaks

▶️ Running the Project Locally
Backend (ASP.NET Core API)
cd Backend/NotesApp.Api
dotnet restore
dotnet run


Backend runs at:

https://localhost:7110


Swagger UI:

https://localhost:7110/swagger

Frontend (React)
cd Frontend/notesapp-ui
npm install
npm run dev


Frontend runs at:

http://localhost:5173

☁️ Azure Deployment (Planned)

Frontend: Azure Static Web Apps

Backend: Azure App Service

Secrets: Azure App Configuration / App Settings

Monitoring: Application Insights

CI/CD: GitHub Actions

⚠️ Disclaimer

AI-generated content is provided for informational purposes only
and is not a substitute for professional medical advice, diagnosis, or treatment.

🎯 Why This Project?

This project demonstrates:

Azure OpenAI integration in a real app

Secure secret management

Clean API design

Modern React architecture

UX decisions for AI-powered features

Cloud-ready structure for Azure deployment

Designed specifically to reflect real Azure developer responsibilities, not just UI demos.

🛣️ Future Enhancements

Authentication (Azure Entra ID)

Note search & filtering

Pagination

Tag management

CI/CD pipeline

Azure SQL / Cosmos DB

Role-based access

Audit logs

👩‍💻 Author

Asmak
Azure Developer | Full-Stack Engineer

⭐ Support

If you find this project useful, please consider ⭐ starring the repository.