🧠 AI Notes App
Azure-Ready Full-Stack Application (React + ASP.NET Core + Azure OpenAI)

An AI-powered notes application built to demonstrate real-world Azure development practices, modern frontend architecture, and secure AI integration using Azure OpenAI (Microsoft Azure AI / Foundry).

This project is intentionally designed as a portfolio showcase for Azure Developer & Full-Stack Engineer roles.

🌟 Highlights

✅ Azure OpenAI (GPT-4o-mini) integration

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

Powered by Azure OpenAI (GPT-4o-mini deployment)

Summarize notes

Rewrite content

Improve writing

Generate tags

Generate full notes from a title

AI-generated content badge

Clear AI disclaimer for responsible usage

Azure OpenAI is accessed securely via backend APIs — no AI keys are exposed in the frontend

📄 Export

Export individual notes to PDF

PDF export available:

Inside the editor

Directly from note cards

🎨 UI / UX

Dark / Light mode toggle (global)

Modal-based editor

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

✅ Backend-only AI key usage

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

Azure OpenAI (GPT-4o-mini)

RESTful API design

Cloud & DevOps

Azure App Service

Azure OpenAI (Microsoft Azure AI / Foundry)

CI/CD via GitHub Actions

Environment variables & App Settings

GitHub

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

Required Azure OpenAI Environment Variables (Backend)
AzureOpenAI__ApiKey
AzureOpenAI__Endpoint

Local Development Configuration

Create this file locally only:

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

AI: Azure OpenAI (GPT-4o-mini)

Secrets: Azure App Settings

CI/CD: GitHub → Azure (already implemented)

⚠️ Disclaimer

AI-generated content is provided for informational purposes only and is not a substitute for professional medical advice, diagnosis, or treatment.

🎯 Why This Project?

This project demonstrates:

Real Azure OpenAI integration

Secure secret management

Clean API design

Modern React architecture

UX decisions for AI-powered features

CI/CD pipelines with GitHub & Azure

Cloud-ready, production-oriented thinking

Designed to reflect real Azure developer responsibilities, not just a UI demo.

🛣️ Future Enhancements

Authentication (Azure Entra ID)

Role-based access

Tags & advanced filtering

Audit logs

Azure SQL / Cosmos DB

Application Insights dashboards

👩‍💻 Author

Asmak
Azure Developer | Full-Stack Engineer

⭐ Support

If you find this project useful, please consider ⭐ starring the repository.
