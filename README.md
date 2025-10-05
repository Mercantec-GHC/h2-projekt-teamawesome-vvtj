# NOVA Hotels Booking System

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technologies used](#technologies-used)
- [Project Structure](#project-structure)
- [Learning Outcomes](#learning-outcomes)
- [Conclusion](#conclusion)

  
## Overview

The NOVA Hotels Booking System is a full-stack web application designed to manage hotel reservations, user accounts, and room information for the NOVA Hotels chain.

The goal of the project is to create a fully functional hotel booking system that allows users to browse hotel information, register an account, and make room reservations. The project also showcases secure authentication, clean architecture, and integration between frontend and backend systems.

## Features

•	Blazor WebAssembly Frontend: Delivers a Single Page Application (SPA) experience using .NET running directly in the browser.

•	User Authentication & Registration: Secure registration and login using JSON Web Tokens (JWT), including password validation and error handling.

•	Hotel Information Pages: Includes sections for About Us, Rooms, and Contact with dynamic content and hotel details.

•	User Profile Management: Allows users to view, create, and update their personal profiles and accounts.

•	Booking Functionality: Provides a form for authenticated users to browse available rooms, create and manage bookings.

•	API Backend: Built with ASP.NET Core, featuring RESTful endpoints, JWT authentication, Swagger/OpenAPI documentation, and Azure Key Vault integration for secrets management.

•	Entity Framework Core Integration: Handles database operations and migrations.

•	Dependency Injection (DI): Promotes modularity and clean architecture.

•	Cloud-Ready Configuration: Designed for deployment to Azure with environment-based settings.


### Technologies used

•	Frontend: Blazor WebAssembly (.NET 9)

•	Backend: ASP.NET Core Web API

•	Database: Entity Framework Core (SQL Server or SQLite)

•	Authentication: JSON Web Tokens (JWT)

•	Cloud Integration: Azure Key Vault, Azure App Configuration

•	Documentation: Swagger / OpenAPI

### Project Structure

/ActiveDirectory

/API

  ├── Controllers/

  ├── Data/

  ├── Interfaces/

  ├── Migrations/

  ├── Services/

  ├── Program.cs

  └── API.csproj


/Blazor
 
  ├── Components/
  
  ├── Helpers/
  
  ├── Interfaces/
  
  ├── Layout/
  
  ├── Models/
  
  ├── Pages/
 
         ├── Auth/
 
                ├── Login.razor
 
                ├── Register.razor
 
                └── ChangePassword.razor
 
         ├── Booking/
 
                └── Booking.razor
  
         └── User/
 
                 ├── CreateUserProfile.razor

                 ├── EditUserProfile.razor

                 ├── UserAccount.razor

                 ├── UserBookings.razor
 
                 └── UserProfile.razor
   
         ├── AboutUs.razor
 
         ├── AvailableRooms.razor
 
         ├── Contact.razor

         ├── Home.razor
  
         ├── Rooms.razor
 
 
  ├── Services/

  ├── App.razor

  ├── Program.cs

  └── Blazor.csproj


/DomainModels

  └── (Shared DTOs and domain models)


/H2-Projekt.ServiceDefaults
 
  └── (Common service configurations)
  
•	API: ASP.NET Core Web API backend, handles all backend logic, including authentication, data access, and business rules.

•	Blazor: WebAssembly frontend, contains the client-side application with all UI components and pages.

•	DomainModels: Shared models and DTOs for strong typing between frontend and backend. 

•	ServiceDefaults: Common service configuration and defaults.

### Learning Outcomes

Through the development of this project, the following learning goals were achieved:

•	Understanding of Full-Stack Development: Building an integrated system combining Blazor frontend and ASP.NET backend.

•	API Design and Implementation: Learning how to create, configure, and implement APIs, including defining endpoints, building controllers, handling HTTP requests, and managing data flow between client and server.

•	Implementation of Authentication and Authorization: Applying JWT-based security in a practical environment.

•	Database Management and Migrations: Using Entity Framework Core for relational data handling.

•	Clean Architecture and Dependency Injection: Designing loosely coupled, testable, and maintainable components.

•	Handeling of hotels booking logic.

This hands-on development experience strengthened our understanding of software development practices, team collaboration, and real-world problem-solving in web application development.


### Conclusion

The NOVA Hotels Booking System successfully demonstrates how a modern, cloud-ready booking platform can be developed using Microsoft’s .NET ecosystem.

The project combines secure backend logic, responsive user interfaces, and well-structured code organization, reflecting a strong grasp of both theoretical concepts and practical implementation.

It also illustrates the team’s ability to design, develop, and deliver a complete web solution — from API development and data management to user interface design and cloud integration.
