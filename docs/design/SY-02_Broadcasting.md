# SY-02: Real-time Broadcasting Engine (Archon Prime Design)

## 🏛️ Executive Summary
The **Real-time Broadcasting Engine** is the circulatory system of QueenFlight. It is responsible for pushing flight state snapshots from the server to thousands of concurrent clients with minimal latency and bandwidth overhead.
By utilizing **SignalR** with **MessagePack** serialization, we ensure that the "Heavy Lifting" (polling AirLabs, data normalization) is done once on the server, while the "Fine Broadcasting" delivers a **Predictive Payload** (Position + Velocity Vector). This allows clients to perform autonomous interpolation (Dead Reckoning) during the 120-second data blind spots, maintaining a buttery smooth 60fps experience without hammering the database or API.

---

## 🎯 Requirements Analysis

### Functional Requirements (FRs)
1.  **Predictive Payload Delivery**: Server must broadcast a compact packet containing `Latitude`, `Longitude`, `Velocity`, `Heading`, and `ServerTimestamp` for each flight.
2.  **Binary Serialization**: Use MessagePack instead of JSON to reduce payload size by ~30-50% (critical for broadcasting 5,000+ flight objects).
3.  **Resilient Connectivity**: SignalR must handle auto-reconnects and connection drops gracefully.
4.  **Group Management**: (Future-proof) Ability to broadcast to specific groups (e.g., users viewing "Europe" vs "Asia"). For MVP, we use a single global channel.

### Non-Functional Requirements (NFRs)
*   **Latency**: Broadcast propagation < 500ms after data ingestion.
*   **Concurrency**: Support 10k+ concurrent connections on a single instance (with Azure SignalR Service scaling in mind).
*   **Bandwidth Efficiency**: Payload size per user update < 50KB (for 1000 flights).

### Assumptions
*   Clients clock might drift; `ServerTimestamp` is the Source of Truth.
*   Reliability over speed: It's better to skip a frame than to show old data.

---

## 🏗️ High-Level Architecture

```mermaid
graph TD
    AL[AirLabs API] -->|Polls 120s| FS[FlightFetcher Service]
    FS -->|Normalizes| RC[(Redis Cache)]
    FS -->|Publish Event| BS[Broadcasting Service]
    
    subgraph SignalR Hub
        BS -->|MessagePack| HUB[FlightHub]
    end
    
    HUB -->|WebSocket / Binary| ClientA[Client A]
    HUB -->|WebSocket / Binary| ClientB[Client B]
    
    note right of ClientA: Client performs Interpolation\nusing Vector (Lat,Lng, V, H)
```

**Description of Data Flow:**
1.  **Ingestion**: `FlightFetcher` wakes up (every 120s), fetches raw data, and updates `Redis`.
2.  **Trigger**: Upon successful update, it invokes `IFlightBroadcaster`.
3.  **Serialization**: The broadcaster maps domain entities to lightweight `FlightDto` optimized for MessagePack (short property names, binary types).
4.  **Distribution**: `SignalR` caches the serialized binary blob and pushes it to all connected clients in the `GlobalFlightData` group.

---

## 🧩 Component Tech Stack

| Component | Technology Choice | Justification |
| :--- | :--- | :--- |
| **Transport** | **SignalR (WebSockets)** | Best-in-class real-time abstraction for .NET. Fallbacks to SSE/Long Polling if needed. |
| **Protocol** | **MessagePack** | Binary serialization is faster to parse and smaller on the wire than JSON. Essential for array-heavy payloads. |
| **Payload Structure** | **DTO / Array-of-Arrays** | Flattening objects (e.g., `[id, lat, lng, v, h]`) vs Maps can further save space. We will stick to DTOs for readability unless profiling demands optimization. |
| **Server** | **Kestrel / .NET 9** | High-performance async I/O. |

---

## ⚖️ Trade-off Analysis

### Decision 1: MessagePack vs. JSON
*   **Option A: JSON (Text)**
    *   *Pros*: Human readable, native everywhere, easy to debug.
    *   *Cons*: Verbose (field names repeated 1000 times), larger size, slower parsing.
*   **Option B: MessagePack (Binary)**
    *   *Pros*: Compact, fast (zero-copy possible), type-safe.
    *   *Cons*: Binary blobs harder to inspect in Network tab (need tools).
*   **Verdict**: **MessagePack**. Since we are broadcasting arrays of thousands of flights, the repeated field names in JSON ("latitude": 123.45) add up to MBs of wasted bandwidth. MessagePack fixes this.

### Decision 2: Snapshot Broadcasting vs. Delta Updates
*   **Option A: Full Snapshot (Send all flights every time)**
    *   *Pros*: Stateless clients. If a client drops and reconnects, the next packet heals everything. Simple logic.
    *   *Cons*: Higher bandwidth.
*   **Option B: Delta Updates (Send only changes)**
    *   *Pros*: Tiny payloads.
    *   *Cons*: Complex state management. If client misses a packet, state is corrupted. Needs "Sync" endpoint.
*   **Verdict**: **Full Snapshot** (for MVP). Since updates are infrequent (120s), sending a full snapshot is acceptable and robust.

---

## 🛡️ Failure Scenarios & Mitigation

*   **Scenario: SignalR Connection Drops**
    *   *Mitigation*: Client enables `WithAutomaticReconnect`. During reconnection, client freezes interpolation or shows "Reconnecting..." toast.
*   **Scenario: Server Crash during Broadcast**
    *   *Mitigation*: State is in Redis. When Server restarts, the Background Service waits for the next 120s cycle (or triggers immediate one if Redis data is stale).
*   **Scenario: MessagePack Version Mismatch**
    *   *Mitigation*: Strict DTO versioning. Using `[Key]` attributes ensures backward compatibility.
