# ✈️ QueenFlight Backend Architecture & Data Flow

This document summarizes the technical architecture, data flow, and key components of the QueenFlight backend implemented in **Rev 2.0 (AirLabs + Redis)**.

## 1. 🏗️ Clean Architecture Structure
The solution is organized to enforce the "Dependency Rule": *Core defines logic, Infrastructure implements details, API coordinates.*

| Project | Role | Description | Key Components |
| :--- | :--- | :--- | :--- |
| **QueenFlight.Core** | 👑 The King (Domain) | Pure business logic, Entities, and Interfaces. No external dependencies. | `FlightState`, `IFlightCache`, `IFlightBroadcaster` |
| **QueenFlight.Infrastructure** | 🛠️ The Worker (Infra) | Implements interfaces. Handles Database, External APIs, Redis. | `FlightDataService`, `RedisFlightCache`, `ApplicationDbContext` |
| **QueenFlight.API** | 👩‍💼 The Receptionist (Entry) | Entry point. Handles HTTP Requests and SignalR connections. | `Program.cs`, `FlightHub`, `SignalRFlightBroadcaster` |

---

## 2. 🔄 End-to-End Data Flow (The "Pipeline")

### Step 1: Ingestion (Pull)
*   **Trigger**: `FlightDataService` (BackgroundWorker) wakes up every **120 seconds**.
*   **Action**: Calls **AirLabs API** to fetch flight data.
*   **Resilience**: Uses **Polly** (Retry Policy) to handle network failures gracefully.

### Step 2: Transformation (Clean)
*   **Parsing**: Raw JSON is parsed via `JsonDocument` (Low-level optimization).
*   **Validation**:
    *   Maps "ugly" JSON keys (hex, lat, dir) to clean `FlightState` model.
    *   **Filters**: Drops flights with missing coordinates (Ghost planes).

### Step 3: Persistence (Store)
*   **Interface**: Core asks: `_flightCache.SetFlightAsync()`.
*   **Implementation**: `RedisFlightCache` converts object to JSON and writes to **Redis**.
*   **Strategy**: "Smart Snapshot". Key: `aircraft:{icao24}`, TTL: 5 minutes.

### Step 4: Broadcasting (Push)
*   **Interface**: Core asks: `_broadcaster.BroadcastFlightCountAsync()`.
*   **Implementation**: `SignalRFlightBroadcaster` (in API layer) relays to SignalR.
*   **Hub**: `FlightHub` pushes event `ReceiveFlightUpdate` to all connected Frontends.

---

## 3. 📡 SignalR Mechanism
SignalR acts as a "Real-time Radio Station" replacing the need for Frontend polling.

*   **Hub (`FlightHub.cs`)**: The endpoint clients connect to (`/flighthub`).
*   **Groups**: Clients subscribe to groups (e.g., "GlobalFlightData").
*   **Events**:
    *   `SubscribeToUpdates`: Client starts listening.
    *   `ReceiveFlightUpdate`: Server pushes new data count.

---

## 4. 🔮 Future: Metadata Enrichment Strategy
Currently, we store "Raw Flight State". To achieve a Premium UI (like FlightRadar24), we will implement **Hybrid Enrichment**:

1.  **Ingest**: Save Polling Data (Lat/Lng) to **Redis**.
2.  **Refer**: Frontend requests Metadata (Airline Logo, Aircraft Model).
3.  **Enrich**: Backend looks up static data in **PostgreSQL** (`Aircrafts` table) and returns merged data.
    *   *Why?* Saves API bandwidth and ensures high-quality static data.
