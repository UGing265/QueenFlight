# Backend Optimization Strategy: Smart Data Seeding & Caching
**US-02 Extension: Solving the "Cold Start" & "API Limit" Dilemma**

## 1. Context & Problem
- **Objective**: Display detailed flight metadata (Airline, Aircraft Model, Registration, Logo) when a user clicks on an aircraft icon.
- **Challenge**: 
    - **API Costs**: The AirLabs API (Free/Standard) has strict limits (e.g., 2,000 calls/month).
    - **Volume**: There are ~10,000 active aircraft at any moment. Fetching details for all of them individually ("Eager Loading") would drain the monthly quota in minutes.
    - **Cold Start**: A fresh Database is empty. Users clicking on any plane triggers an API call (1 credit). Filling the DB from scratch requires ~80,000 requests.

## 2. Strategic Solution: "3-Layer Data Defense"
To achieve **Near-Zero API Costs** for daily operations while maintaining high data fidelity, we implement a three-layer caching strategy.

### Layer 1: Static Data Seeding (Foundation)
*   **Concept**: Download the entire global database of Airlines and Airports **ONCE** at startup.
*   **Mechanism**:
    - Call `/airlines` and `/airports` (Bulk APIs) on server bootstrap.
    - Insert ~5,000 airlines and ~10,000 airports into Postgres.
*   **Benefit**: 
    - Immediate display of Airline Name, Logo, Country, and Route info (Origin/Dest Airports).
    - **Cost**: 2 API Calls per deployment.

### Layer 2: Dynamic Stream Seeding (Real-time Filler)
*   **Concept**: "Vợt" (Siphon) metadata from the existing real-time flight stream.
*   **Mechanism**:
    - The `FlightDataService` already polls `/flights` every 10s (~8,000 aircraft/request).
    - **Crucial Discovery**: This stream *already* contains `reg_number` (Registration) and `aircraft_icao` (Model Code).
    - **Action**: Instead of discarding this data, we push it to a background queue.
    - **Upsert Logic**: `INSERT INTO aircrafts (...) ON CONFLICT (icao24) DO NOTHING`.
*   **Benefit**:
    - Populates the `Aircrafts` table with 8,000+ records within minutes.
    - **Cost**: **$0** (Piggybacks on the existing tracking loop).

### Layer 3: Lazy Loading + Auto-Seed (The Safety Net)
*   **Concept**: Fetch deep details only when absolutely necessary (User Interaction).
*   **Mechanism**: 
    - When a user clicks a plane, we check DB Layers 1 & 2 first.
    - If data is still missing (e.g., rare private jet, new airline), call `/flight` API (1 Credit).
    - **Auto-Seed**: Save the result to DB immediately so it is never fetched again.
*   **Benefit**: Handles edge cases without breaking user experience.

## 3. Trade-off Analysis & Risk Management

| Risk | Impact | Mitigation Strategy |
| :--- | :--- | :--- |
| **Write Load** | Upserting 8,000 records every 10s could spike DB CPU. | **Batching & Throttling**: Only upset distinct ICAO24s that are *not* in a local RAM Set (Bloom Filter or Cache). |
| **Data Staleness** | Static data (Airlines) might get outdated (rebranding). | **Scheduled Refetch**: Run Static Seeder once/week (Cron Job). |
| **Concurrency** | Multiple threads trying to insert the same plane. | **DB-Level Lock**: Postgres `ON CONFLICT` handles race conditions atomically. |

## 4. Implementation Artifacts
- **Seeder**: `StaticDataSeeder.cs` (Layer 1).
- **Service**: `FlightDataService.cs` modification (Layer 2).
- **Logic**: `FlightDetailsProvider.cs` (Layer 3 & Logic Controller).
