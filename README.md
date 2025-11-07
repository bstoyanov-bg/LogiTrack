# 🚚 LogiTrack — Distributed Shipment & Driver Management Platform

**LogiTrack** is a modular logistics management system designed for scalability, real-time communication, and fault-tolerant operations.  
It leverages **ASP.NET Core 10**, **Entity Framework Core**, **Redis**, **gRPC**, **SignalR**, and **Angular** to manage shipments, drivers, and live data updates efficiently.

---

## 🏗️ Architecture Overview

The platform currently consists of the following key components:

| Service | Description | Technologies |
|----------|--------------|---------------|
| **ShipmentService** | Handles shipment lifecycle (CRUD, status tracking, Redis caching, SignalR notifications, and gRPC communication with DriverService) | ASP.NET Core Web API, EF Core, Redis, SignalR, gRPC Client |
| **DriverService** | Manages drivers, updates their statuses, and handles gRPC requests from ShipmentService | ASP.NET Core gRPC Server, EF Core |
| **Angular Dashboard** | Provides a visual interface for tracking shipments, monitoring health, and managing drivers | Angular 17, RxJS, Angular Material |
| **SQL Server (x2)** | Separate databases for shipments and drivers | Entity Framework Core |
| **Redis** | In-memory cache layer for fast data retrieval and reduced DB load | StackExchange.Redis |

---

## ⚙️ Features Implemented

✅ **Shipment Management**
- Full CRUD operations via REST API  
- Redis caching for `GetAllShipments`  
- Real-time updates via SignalR Hub  
- gRPC communication with DriverService for driver assignment  

✅ **Driver Management**
- Dedicated DriverService with EF Core and SQL  
- REST API endpoints for drivers (`/api/drivers`)  
- gRPC endpoints (`DriverManager`) for assignment, registration, and status updates  
- Automatic seeding on startup  

✅ **Health Monitoring**
- `/health` and `/health/db` endpoints in both services  
- Angular **Health Dashboard** displaying API, Redis, and SignalR status in real time  

✅ **Frontend**
- Angular dashboards for Shipments and Drivers  
- Real-time UI updates (SignalR push, no manual refresh)  
- Health monitoring and connection indicators  

---

## 🧭 System Design Diagram

```mermaid
graph TD
    subgraph Frontend
        UI[Angular Dashboard]
    end

    subgraph ShipmentLayer
        A[ShipmentService]
        B[(SQL Server - Shipments)]
        C[(Redis Cache)]
    end

    subgraph DriverLayer
        D[DriverService]
        E[(SQL Server - Drivers)]
    end

    subgraph Monitoring
        F[/Health Endpoints/]
    end

    %% Connections
    UI -->|REST / SignalR| A
    UI -->|REST| D
    A -->|SQL Queries| B
    A -->|Cache Read/Write| C
    A -->|gRPC Call| D
    D -->|SQL Queries| E
    A -->|/health| F
    D -->|/health| F