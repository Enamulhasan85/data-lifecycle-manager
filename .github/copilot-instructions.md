# GitHub Copilot Instructions

## Project Context

This is a **.NET MVC project** called **Data Lifecycle Manager (DLM)**.  
It handles **data archiving, lifecycle management, retention policies, and ETL workflows**.  
It is designed to be used in multiple domains such as hospitals or academic systems.

## Architecture Notes

- Single MVC project (no class libraries) using **simplified Clean Architecture** principles.
- Layers are separated by folders:
  - `Controllers` / `Views` → Presentation layer
  - `Application/Interfaces` & `Application/Services` → Business logic / use cases
  - `Data/` → EF Core DbContext, Repositories, UnitOfWork, Seeders
  - `Configuration/` → App and infrastructure configurations
  - `Extensions/` → Startup extension methods (ServiceCollection, WebApplication, etc.)

## Guidance for Copilot

- Suggest proper folder placement for new classes or services.
- Detect and warn if architecture principles are violated (e.g., domain logic in Controllers, infra logic in Application layer).
- Suggest improvements to naming or organization.
- Focus on **maintaining clean architecture within a single project**.
- Output actionable advice and examples if something is misplaced.
