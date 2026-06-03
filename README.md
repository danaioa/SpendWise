# SpendWise

 Live Demo: https://spendwise.student-dev.ro

SpendWise is a full-stack personal finance management application built with ASP.NET Core Web API, React, TypeScript and SQL Server.

The application helps users manage their personal finances by tracking expenses, planning budgets, monitoring recurring payments, scanning products and automatically tracking monthly savings.

---

## Live Website

The application is publicly deployed and accessible online:

**https://spendwise.student-dev.ro**

The project is containerized with Docker.

---

### Demo Account

Email: testfrontend123@gmail.com

Password: Test123!

---

## Features

### Authentication

* User registration
* Secure login
* JWT authentication
* Protected routes

### Dashboard

* Monthly income overview
* Total expenses tracking
* Remaining balance calculation
* Budget monitoring by category

### Transactions

* Expense and income tracking
* Monthly transaction history
* Automatic salary records
* Recurring payments integration

### Budget Planning

* Automatic budget calculation
* Percentage-based category allocation
* Budget usage tracking
* Overspending warnings

### Recurring Payments

* Fixed monthly expenses
* Automatic monthly calculations
* Subscription and bill management

### Product Scanner

* Barcode scanning support
* Product database integration
* Fast expense creation

### Savings

* Monthly savings history
* Automatic month closing
* Savings tracking over time

### Landing Page

* Public presentation page
* Project overview
* User onboarding

---

## Technologies

### Backend

* ASP.NET Core 8
* Entity Framework Core
* ASP.NET Identity
* JWT Authentication
* SQL Server

### Frontend

* React
* TypeScript
* Axios
* React Router
* Lucide React

### Deployment

* Docker
* Docker Compose
* Linux Server
* Nginx Reverse Proxy

---

## Database Entities

The application uses the following main entities:

* User
* Role
* Transaction
* Category
* Budget
* BudgetPlan
* BudgetPlanCategory
* RecurringTransaction
* Product
* MonthlySaving

---

## Architecture

```text
Browser
   │
   ▼
React Frontend
   │
   ▼
ASP.NET Core Web API
   │
   ▼
SQL Server
```

All services are deployed using Docker containers and orchestrated through Docker Compose.

---

## Screenshots

### Landing Page

<img width="1811" height="1002" alt="image" src="https://github.com/user-attachments/assets/8a30c925-60a2-489e-9b93-fcc187395106" />


### Dashboard

<img width="1901" height="1025" alt="image" src="https://github.com/user-attachments/assets/c3f9f3f1-eb54-44fc-b0a4-00404cab578e" />


### Budget Planning

<img width="1899" height="1025" alt="image" src="https://github.com/user-attachments/assets/4d36005b-ef2f-4009-8a09-b0ed8a04b9ad" />


### Transactions

<img width="1884" height="1013" alt="image" src="https://github.com/user-attachments/assets/8389ff13-04fe-4ca4-a1cc-9b03cc746c62" />


### Product Scanner

<img width="1912" height="1038" alt="image" src="https://github.com/user-attachments/assets/f11bc0de-898a-453c-b7fe-0d1aff5832ce" />


### Savings

<img width="1913" height="1023" alt="image" src="https://github.com/user-attachments/assets/c588b958-15d8-4f64-8190-cc2a9879483d" />


---

## Running Locally

### Backend

```bash
cd SpendWise.API
dotnet restore
dotnet run
```

### Frontend

```bash
cd spendwise-client
npm install
npm run dev
```

### Database

Update the SQL Server connection string in:

```json
appsettings.json
```

Apply migrations:

```bash
dotnet ef database update
```

---

## Deployment

The production environment runs on:

* Linux Server
* Docker Containers
* Docker Compose
* SQL Server 2022
* Nginx Reverse Proxy

Deployment process:

```bash
git pull
docker compose up -d --build
```

---

## Academic Context

This project was developed as part of the Web Application Development course.

The application demonstrates:

* RESTful API design
* ASP.NET Core development
* Entity Framework Core
* SQL Server integration
* JWT Authentication
* React SPA development
* Docker deployment
* Full-stack software architecture
* Production hosting on a public domain
