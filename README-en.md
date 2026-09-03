# My Expansion Team

### NFL Expansion Simulator — Full-Stack Web & Mobile Application

My Expansion Team is a full-stack application that simulates the process of creating and managing an NFL expansion franchise.

The application allows users to create their own expansion teams and build their rosters while following configurable rules inspired by the real NFL expansion process. Users can select protected players, acquire players from existing franchises, manage contracts and salary-cap constraints, perform trades, conduct drafts, configure roster rules, and track team performance.

The project was developed as my **Bachelor's Thesis (TFG) in Computer Science** and implemented as a complete multi-client system consisting of a REST API, web application and mobile application.

---

## Overview

Unlike traditional fantasy football applications, My Expansion Team focuses on the **NFL franchise expansion process**.

The application provides a complete environment in which users can:

- Create and manage expansion franchises
- Configure expansion and roster rules
- Select protected players from existing franchises
- Acquire players from existing teams
- Build and manage rosters
- Configure offensive and defensive formations
- Manage contracts and salary-cap constraints
- Perform trades
- Conduct an expansion draft
- Track team and player performance
- Import and manage NFL data
- Manage user accounts and permissions

All of these features are fully implemented and functional — not UI mockups.

---

# Getting Started

Local development only — the project is not deployed to a public environment.

**Requirements:** Visual Studio 2022 (.NET MAUI workload for mobile), .NET 9 SDK, SQL Server 2019/2022, SSMS.

```bash
# REST API
cd MyExpansionTeam
dotnet run
# → https://localhost:7087/swagger

# Web app
cd WebApp
dotnet run
# → https://localhost:7099
```

For the mobile app, open `METAPI.sln` in Visual Studio, set `MobileApp` as the startup project, and run on an Android emulator.

Full database setup and step-by-step installation instructions available upon request.

---

# Screenshots

| | |
|---|---|
| **Home** | **Lineup builder** |
| ![Home](docs/screenshots/web-home.png) | ![Lineup](docs/screenshots/web-lineup.png) |
| **Draft summary** | **Admin panel** |
| ![Draft summary](docs/screenshots/web-draft-summary.png) | ![Admin panel](docs/screenshots/web-admin-panel.png) |

**Mobile**

<img src="docs/screenshots/mobile-home.png" alt="Mobile home screen" width="300">

More screenshots (registration, account management, trades, data import, and the full mobile flow) available upon request.

---

## Key Features

### Expansion Team Management

Users can create and manage their own expansion franchises.

- Create teams with name, location and abbreviation
- Modify existing teams
- Duplicate teams as a starting point for new franchises
- Delete teams
- View team information
- Manage multiple personal teams

### Expansion Rules & Roster Management

The application models the main rules involved in building an expansion franchise.

- Configure expansion settings
- Define the number of protected players
- Define player acquisition limits
- Select protected players
- Acquire players from existing franchises
- Build customized rosters
- Configure offensive and defensive formations
- Validate roster constraints

### Salary Cap & Contracts

Salary and contract information is incorporated into the team-building process.

The application enforces salary-cap constraints when constructing and managing teams, allowing the expansion process to take financial restrictions into account rather than treating players as simple roster entries.

### Trades

Users can simulate player trades between teams.

- Select players involved in a trade
- Validate trade information
- Execute and save trades
- View previous trades associated with a team

### Draft

The application includes a draft system for expansion teams.

- Configure draft settings
- Manage draft selections
- Select prospects
- Save draft results
- Review draft information

### Player & Performance Data

The system manages detailed NFL player information and statistics.

It includes data covering areas such as:

- Player information
- Contracts
- Passing statistics
- Rushing statistics
- Receiving statistics
- Other performance statistics
- Team and franchise information

### Data Import

The application includes a dedicated data-import system for loading NFL information from CSV files.

The import pipeline:

- Parses the input data
- Validates records
- Detects incomplete or invalid data
- Detects duplicate records
- Generates the corresponding domain objects
- Persists the resulting information in the database

The implementation uses generic types and inheritance to support different types of imported data through a common import workflow.

The main dataset used during development was obtained from **Kaggle**, with ESPN and OverTheCap used as supplementary sources for specific NFL information.

### User Management & Security

The system includes authentication and authorization functionality.

- User registration
- Login and logout
- Credential management
- User profiles
- Account deletion
- Role-based authorization
- Administrative user management

The REST API uses **JWT Bearer authentication**, while the web application manages its own authenticated user session.

---

# Architecture

The application was designed as a multi-client system around a central REST API.

```text
                                 ┌─────────────────────────┐
                                 │     Web Application     │
                                 │     ASP.NET Core MVC    │
                                 └────────────┬────────────┘
                                              │
                                              │ HTTP
                                              │
┌─────────────────────────┐                   ▼
│    Mobile Application   │      ┌─────────────────────────┐
│        .NET MAUI        │ ──▶ │         REST API        │
│         MVVM            │      │       ASP.NET Core      │
└─────────────────────────┘      └────────────┬────────────┘
                                              │
                                              ▼
                                 ┌─────────────────────────┐
                                 │      Business Layer     │
                                 │         METCore         │
                                 └────────────┬────────────┘
                                              │
                                              ▼
                                 ┌─────────────────────────┐
                                 │     Data Access Layer   │
                                 │         METDAL          │
                                 │    EF Core + Repos.     │
                                 └────────────┬────────────┘
                                              │
                                              ▼
                                 ┌─────────────────────────┐
                                 │       SQL Server        │
                                 └─────────────────────────┘
```

The architecture separates presentation, business logic and data access while allowing multiple clients to consume the same API and underlying business logic.

Both clients consume the same backend through HTTP rather than duplicating business logic.

---

# Solution Structure

The solution is divided into five projects:

```text
MyExpansionTeam/
│
├── MyExpansionTeam/
│   └── REST API
│
├── METCore/
│   ├── Domain models
│   ├── DTOs
│   ├── Service interfaces
│   ├── Business services
│   └── Mapping
│
├── METDAL/
│   ├── Entity Framework Core
│   ├── Database context
│   ├── Entity configuration
│   ├── Repositories
│   └── Database migrations
│
├── WebApp/
│   ├── ASP.NET Core MVC
│   ├── Controllers
│   ├── Views
│   └── HTTP/AJAX communication with API
│
└── MobileApp/
    ├── .NET MAUI
    ├── MVVM
    ├── Views
    └── HTTP communication with API
```

### MyExpansionTeam

The ASP.NET Core Web API responsible for exposing the application's functionality through REST endpoints.

The API contains controllers covering the main application domains, including users, teams, players, franchises, trades, drafts and related data.

### METCore

The shared core of the application.

It contains:

* Domain models
* Data Transfer Objects (DTOs)
* Service interfaces
* Business logic
* Mapping configuration

This layer provides the common contracts and logic used throughout the solution.

### METDAL

The Data Access Layer.

It is responsible for communication with SQL Server through Entity Framework Core and implements the repository-based data access architecture.

### WebApp

The browser-based client implemented using ASP.NET Core MVC.

The web application communicates with the REST API through HTTP and provides the complete user-facing web experience.

### MobileApp

The mobile client implemented using .NET MAUI.

The application follows the MVVM pattern and communicates with the same REST API used by the web client.

---

# Technology Stack

## Backend

* **C#**
* **ASP.NET Core Web API**
* **.NET 9**
* **Entity Framework Core**
* **AutoMapper**
* **JWT Bearer Authentication**
* **Swagger / OpenAPI**

## Web

* **ASP.NET Core MVC**
* **Razor**
* **HTML / CSS**
* **JavaScript**
* **AJAX**
* **HTTP Client**

## Mobile

* **.NET MAUI**
* **XAML**
* **C#**
* **MVVM**

## Database

* **Microsoft SQL Server**
* **Entity Framework Core**
* **SQL Server Management Studio**
* Database migrations
* Stored procedures

## Development Tools

* **Visual Studio**
* **Postman**
* **Git / GitHub**

---

# Database

Microsoft SQL Server is used as the relational database for the application.

The database models the different entities involved in the NFL expansion simulation, including users, franchises, teams, players, contracts, statistics, drafts and trades.

Entity Framework Core is used for object-relational mapping and database access.

The project also makes use of stored procedures for database operations and more complex queries.

The resulting data model contains more than 15 specialized entities for different categories of NFL information and statistics.

---

# REST API

The REST API acts as the central backend for both client applications.

Representative API areas include:

```text
/api/Auth
/api/Users
/api/Players
/api/Franchises
/api/Teams
```

Team-related operations include:

* Creating and updating teams
* Retrieving team information
* Managing rosters
* Configuring roster settings
* Performing trades
* Saving draft results
* Retrieving team trade history
* Duplicating teams
* Deleting teams

The API uses DTOs to define the data exchanged with clients and separates API contracts from the underlying database entities.

Swagger/OpenAPI is available during development for API exploration and testing.

---

# Data Model

The application works with a relatively complex NFL domain model rather than a small set of generic CRUD entities.

The database represents different types of information such as:

* Users
* Franchises
* Teams
* Players
* Contracts
* Draft prospects
* Draft selections
* Trades
* Team rosters
* Passing statistics
* Rushing statistics
* Receiving statistics
* Other season statistics

This required translating NFL-specific rules and concepts into a relational data model and corresponding application logic.

---

# Data Import Architecture

One of the more technically involved components of the project is the data-import pipeline.

The application accepts CSV datasets and processes them through a common import flow.

Conceptually:

```text
CSV File
   │
   ▼
Read & Parse
   │
   ▼
Validate Data
   │
   ├── Invalid records
   ├── Incomplete records
   └── Duplicate records
   │
   ▼
Generate Domain Objects
   │
   ▼
Persist Data
   │
   ▼
SQL Server
```

The implementation uses generics and inheritance to allow different types of imported data to be processed through the same general workflow while still producing the appropriate domain objects.

---

# Authentication & Authorization

Authentication is implemented using JWT Bearer tokens at the API level.

The system also implements authorization through user roles, allowing administrative functionality to be restricted to authorized users.

The web application maintains its authenticated user session independently while using the API as its backend.

---

# Testing

The application was extensively tested using functional test cases derived from the system's defined use cases.

Testing covered areas including:

* User registration
* Authentication
* Logout
* Credential management
* Team creation and management
* Roster management
* Player protection
* Player selection
* Trades
* Drafts
* Draft configuration
* Performance tracking
* Data import
* Administrative functionality
* Role management

The tests included both successful and invalid scenarios, such as incomplete input, invalid credentials, duplicate data, invalid operations and cancellation of processes.

---

# Development Approach

The project followed the **Unified Process (UP)** as its software development methodology.

The methodology was selected because of its:

* Iterative and incremental nature
* Architecture-centric approach
* Use-case-driven development
* Ability to accommodate changes during development

The development process covered:

1. Requirements gathering
2. Analysis
3. System design
4. Architecture and database design
5. Technology selection
6. Implementation and integration
7. Testing
8. Documentation

---

# Project Documentation

This project was developed as my **Bachelor's Thesis (Trabajo de Fin de Grado)** in Computer Science.

The complete thesis documents the requirements, analysis, architecture, database design, implementation, testing, conclusions, future development possibilities and installation/user manuals.

The full thesis/documentation is available upon request.

The thesis contains detailed technical information that is intentionally not duplicated in full in this README.

---

# Future Development

The current repository represents the completed version of the Bachelor's Thesis and is not intended to be actively developed further.

The thesis identified several possible future directions, including:

* Social features and user interaction
* Advanced fantasy leagues
* Rankings and competitive systems
* Additional statistics and content
* Expansion to other sports such as NBA, MLB and NHL
* Real-time data integration
* Additional platforms
* Further scalability improvements

These possibilities are documented in detail in the TFG.

---

# Project Status

**Completed — Bachelor's Thesis / Portfolio Project**

This repository contains the final version of the project submitted as my Bachelor's Thesis.

The application is fully functional across its web and mobile clients and is presented as a portfolio project demonstrating the complete development of a multi-client full-stack application.

It is not currently deployed as a public production service.

---

# Author

**Jorge Rodríguez Rodríguez**

Computer Science Graduate · Full-Stack Developer

Areas of interest:

* Full-Stack Development
* Backend Development
* .NET / ASP.NET Core
* REST API Design
* Software Architecture
* Database Design
* C#
