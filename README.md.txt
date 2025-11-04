# 🚚 LogiTrack — Shipment & Driver Management Platform

**LogiTrack** is a modular logistics management system built with **ASP.NET Core 10**, **Entity Framework Core**, **Redis**, and **gRPC**.  
It provides shipment tracking, driver assignment, and efficient data caching through Redis for optimal performance and scalability.

---

## 🏗️ Architecture Overview

The platform currently consists of the following components:

| Service | Description | Technologies |
|----------|--------------|---------------|
| **ShipmentService** | Handles shipment lifecycle (CRUD operations, status tracking, Redis caching, and gRPC communication) | ASP.NET Core Web API, EF Core, Redis, gRPC Client |
| **DriverService** | Simulates driver management and communicates with ShipmentService via gRPC | ASP.NET Core gRPC Server |
| **SQL Server** | Stores persistent shipment data | Entity Framework Core |
| **Redis** | Provides in-memory caching for frequently accessed data | StackExchange.Redis |

---

## 🧭 System Design Diagram

```mermaid
graph TD
    A[Swagger / API Client] -->|HTTP REST| B[ShipmentService]
    B -->|SQL Queries| C[(SQL Server)]
    B -->|Cache Read/Write| D[(Redis)]
    B -->|gRPC Call| E[DriverService]
    E -->|Response| B