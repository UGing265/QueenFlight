# Task: Real-time Broadcasting Service (SY-02)

## 1. Internal Logic Flow (Luồng xử lý chi tiết [Logic Flow])

### Activity: Broadcasting Cycle (Every 600s)
1.  **Ingestion Start**: `FlightDataService` (Background Worker) wakes up on Timer tick.
2.  **External Poll**: Service sends HTTP GET to **AirLabs API**.
    *   *Retry Policy*: Uses Exponential Backoff (Poll -> Wait 2s -> Retry) on network failure.
3.  **Data Parsing**:
    *   Receives JSON response.
    *   Iterates through `response` array.
4.  **Transformation Logic (In-Memory)**:
    *   **Filter**: Discards flights with missing `lat`, `lng`, `speed`, or `heading`.
    *   **Mapping**: Converts Raw AirLabs JSON -> `FlightState` (Domain Entity).
    *   **Persistence**: Saves valid entity to `RedisFlightCache`.
    *   **DTO Mapped**: Simultaneously maps `FlightState` -> `FlightPayloadDto` (Optimized View Model).
        *   `lat`, `lng` cast to `double`.
        *   `velocity` cast to `float` (km/h).
5.  **Broadcasting Event**:
    *   Service invokes `IFlightBroadcaster.BroadcastFlightDataAsync(List<FlightPayloadDto>)`.
6.  **Infrastructure Adapter**:
    *   `SignalRFlightBroadcaster` receives the DTO list.
    *   Calls `HubContext.Clients.Group("GlobalFlightData").SendAsync("ReceiveFlightUpdate", payloads)`.
7.  **Serialization**:
    *   SignalR Pipeline uses `MessagePack` formatter to compress the List into a binary blob.
8.  **Distribution**:
    *   Binary blob is pushed via WebSockets to all subscribed Clients.

## 2. Architectural Rationale (Tại sao thiết kế như vậy? [Rationale])
*   **Separation of Concerns**:
    *   `FlightDataService` handles **Ingestion & Mapping**.
    *   `IFlightBroadcaster` handles **Distribution**.
    *   `SignalRFlightBroadcaster` handles **Protocol specifics (SignalR)**.
    *   *Why?* If we switch AirLabs to OpenSky, only the Service changes. If we switch SignalR to Kafka, only the Broadcaster changes.
*   **Performance vs. Complexity**:
    *   **Decision**: We perform DTO mapping **inside the ingestion loop** to iterate only once ($O(N)$).
    *   **Trade-off**: Slightly higher CPU usage during ingestion, but prevents iterating the list a second time for broadcasting.
*   **Protocol**:
    *   **MessagePack**: Chosen over JSON to reduce bandwidth by ~50% for high-frequency array updates.

## 3. API Contract (Frontend Interface)
*   **Endpoint**: `GET /flighthub`
*   **Protocols**: `Information`, `WebSockets`
*   **Serialization**: `MessagePack` (Binary)

### Subscriptions
*   **Client Action**: Invoke `SubscribeToUpdates()`
*   **Server Response**: Adds ConnectionId to Group `GlobalFlightData`.

### Events (Server -> Client)
| Event Name | Data Type | Description |
| :--- | :--- | :--- |
| `ReceiveFlightUpdate` | `FlightPayloadDto[]` | Array of flight snapshots for interpolation. |
| `ReceiveFlightCount` | `int` | Count of active flights (Diagnostic). |

### DTO Specification (`FlightPayloadDto`)
| Key (Short) | Type | Mapping | Note |
| :--- | :--- | :--- | :--- |
| `i` | `string` | `Icao24` | Unique Hex ID. |
| `la` | `double` | `Latitude` | WGS84. |
| `lo` | `double` | `Longitude` | WGS84. |
| `v` | `float` | `Velocity` | Speed in km/h. |
| `h` | `float` | `Heading` | True Track (0-360). |
| `ts` | `long` | `Timestamp` | Unix Epoch (Source of Truth). |
