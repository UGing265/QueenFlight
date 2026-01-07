# ✈️ Technical Report: Backend Real-Time Engine Implementation

**Date:** 2026-01-07
**Status:** ✅ Completed

---

## 1. Executive Summary
This document details the implementation of the **Real-Time Flight Ingestion Engine**, the core backend component responsible for fetching, processing, and broadcasting live flight data. 

The system leverages **AirLabs API** for data sourcing, **Redis** for high-performance state persistence, and **SignalR** for real-time broadcasting to connected clients. The architecture follows strict **Clean Architecture** principles to ensuring maintainability and preventing "spaghetti code".

---

## 2. Architecture Overview (Clean Architecture)

The solution is divided into three distinct layers, enforcing the "Dependency Rule" (Inner layers usually know nothing about outer layers).

### 👑 Core Layer (`QueenFlight.Core`)
*   **Role:** The "King". Contains enterprise logic and generic interfaces. Zero external dependencies.
*   **Key Components:**
    *   `FlightState` (Record): An immutable data structure representing the "Ground Truth" of a flight at a specific moment.
    *   `IFlightCache` (Interface): Defines *what* storage operations are needed (Get/Set), without knowing *how* (Redis/SQL).
    *   `IFlightBroadcaster` (Interface): Defines *what* needs to be broadcasted, without knowing *how* (SignalR/Kafka).

### 🛠️ Infrastructure Layer (`QueenFlight.Infrastructure`)
*   **Role:** The "Worker". implements the interfaces defined in Core. Handles "dirty" I/O operations.
*   **Key Components:**
    *   `FlightDataService`: A background worker acting as the heartbeat of the system.
    *   `RedisFlightCache`: Concrete implementation of storage using **Redis**.
    *   **Polly**: Resilience policy to handle external API failures (Network jitters, Rate limits).

### 👩‍💼 API Layer (`QueenFlight.API`)
*   **Role:** The "Gateway". Handles external communication with Clients (Frontend).
*   **Key Components:**
    *   `FlightHub`: The SignalR WebSocket endpoint.
    *   `SignalRFlightBroadcaster`: Concrete implementation that pushes data to the Hub.

---

## 3. Detailed Mechanism: The "Ingestion Pipeline"

The system operates on a **120-second heartbeat** (governed by AirLabs Free Tier limits).

### Step 1: The Trigger (Infrastructure)
The `FlightDataService` is a `.NET BackgroundService`. It uses a `PeriodicTimer` which is non-blocking (unlike `Thread.Sleep`), ensuring efficient CPU usage while waiting for the next cycle.

### Step 2: Ingestion & Resilience (The "Pull")
*   The system sends a `GET` request to `api.airlabs.co`.
*   **Resilience Strategy**: If the network fails, we do not crash. We use **Exponential Backoff**:
    1.  Fail -> Wait 2s -> Retry.
    2.  Fail -> Wait 4s -> Retry.
    3.  Fail -> Wait 8s -> Retry.
    *   *Why?* prevents the specific "Thundering Herd" problem where immediate retries could worsen a server outage.

### Step 3: Transformation & Sanitization (The "Clean")
*   Raw JSON from AirLabs is "messy" (short keys like `hex`, `lat`).
*   We parse this using `JsonDocument` (High-performance, zero-allocation parsing).
*   **Sanitization Rules**:
    *   Flights without `Latitude` or `Longitude` are explicitly **DROPPED**. These are "Ghost Planes" (e.g., on the ground with transponders off) and provide no value to the map.

### Step 4: Persistence (The "Store")
*   **Technology**: Redis (Key-Value Store).
*   **Schema**:
    *   **Key**: `aircraft:{icao24}` (e.g., `aircraft:888126`).
    *   **Value**: JSON String of the `FlightState`.
    *   **TTL (Time-To-Live)**: **5 Minutes**.
*   *Why TTL?* This effectively implements "Garbage Collection". If a plane lands or goes out of range, it stops sending updates. After 5 minutes, Redis automatically deletes it. No cronjob needed!

### Step 5: Broadcasting (The "Push")
*   Once data is safely in Redis, the Worker calls `IFlightBroadcaster`.
*   The `SignalRFlightBroadcaster` sends a message to the `GlobalFlightData` group.
*   **Payload**: The total count of active flights (for MVP). In the future, this will send the actual delta list (Diff).

---

## 4. Why this Architecture? (Rationale)

### ❓ Why separate `IFlightBroadcaster`?
*   **Problem**: The Worker (Infrastructure) initially depended on `FlightHub` (API). This is a circular dependency (Infra -> API -> Infra).
*   **Solution**: Inverted the dependency. Worker now calls an Interface in Core. API implements that interface.
*   **Benefit**: You can now test the Worker without launching a Web Server.

### ❓ Why Redis and not just a C# List?
*   **Persistence**: If we restart the Backend (e.g., to deploy a fix), an in-memory List is wiped. Users would see a blank map for 2 minutes. Redis survives the restart.
*   **Scalability**: If we scale to 2 Backend servers, they can share the same Redis "brain".

### ❓ Why generic Interfaces?
*   If AirLabs raises prices, we can switch to **OpenSky Network** by changing only `FlightDataService`. The rest of the app (Redis, SignalR, Frontend) remains 100% untouched.

---
*End of Report*
