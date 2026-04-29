# QueenFlight Architecture Report

> Generated: 2026-04-30
> Branch: feat/detail-flight

---

## 1. Overall Architecture Diagram (ASCII)

```
+-------------------------------------------------------------------------------------+
|                              QUEENFLIGHT ARCHITECTURE                                |
+-------------------------------------------------------------------------------------+

    +-----------------+         +------------------+         +------------------+
    |   EXTERNAL      |         |   BACKEND        |         |   FRONTEND       |
    |   SERVICES      |         |   (ASP.NET 9)    |         |   (Next.js 16)   |
    +-----------------+         +------------------+         +------------------+
           |                           |                           |
           v                           v                           ^
    +-----------------+         +------------------+         +------------------+
    | AirLabs API     |  HTTP   | QueenFlight.API  | WebSocket| useFlightSignalR|
    | (Flight Data)   |-------->| (REST + SignalR) |-------->| (React Hook)    |
    +-----------------+         |                  | SignalR +------------------+
           ^                     |  +------------+  |  HUB       |              |
           |                     |  |Controllers |  |            v              |
    +------+------+              |  +------------+  |     +------------------+ |
    |              |              |        |        |     | MapContainer     | |
    | PostgreSQL   |              |        v        |     | (Mapbox GL JS)   | |
    | Database     |<----------->|  +------------+  |     +------------------+ |
    +--------------+   EF Core   |  |Background  |  |            |              |
                                |  |Services    |  |            v              |
    +--------------+             |  +------------+  |     +------------------+ |
    | Redis        |              |        |        |     | useInterpolation | |
    | (Flight      |<------------>|  +------------+  |     | (60fps Physics)  | |
    |  Cache)      |  StackEx   |  | Services   |  |     +------------------+ |
    +--------------+  Redis     |  +------------+  |            |              |
                                |        |        |            v              |
                                |        v        |     +------------------+ |
                                |  +------------+  |     | FlightDetail     | |
                                |  |Interfaces  |  |     | Modal           | |
                                |  +------------+  |     +------------------+ |
                                +------------------+         +------------------+

+-------------------------------------------------------------------------------------+
|                              DATA FLOW DIAGRAM                                       |
+-------------------------------------------------------------------------------------+

    AirLabs API                      Backend                          Frontend
    (External)                    (ASP.NET Core)                    (Next.js)

        |                              |                                |
        |  600s polling               |                                |
        |----------------------------->|                                |
        |                              |                                |
        |  Flight JSON                 |  Map to FlightState             |
        |  {hex, lat, lng, speed...}   |  {Icao24, Lat, Lng...}         |
        |                              |                                |
        |                              |  Store in Redis (5min TTL)      |
        |                              |-----------------------+        |
        |                              |                       |        |
        |                              |                       v        |
        |                              |                 +---------+   |
        |                              |                 |  Redis  |   |
        |                              |                 | Cache   |   |
        |                              |                 +---------+   |
        |                              |                                |
        |                              |  Broadcast via SignalR         |
        |                              |----------------------->        |
        |                              |                       |        |
        |                              |                       v        |
        |                              |              +------------------+
        |                              |              | FlightHub        |
        |                              |              | /flighthub       |
        |                              |              +------------------+
        |                              |                       |
        |                              |     SignalR          |
        |                              |<----------------------+
        |                              |                       |
        |                              |                       |  ReceiveFlightData
        |                              |                       |  (MessagePack)
        |                              |                       v
        |                              |              +------------------+
        |                              |              | useFlightSignalR|
        |                              |              | hook            |
        |                              |              +------------------+
        |                              |                       |
        |                              |                       |  flightData
        |                              |                       v
        |                              |              +------------------+
        |                              |              | useInterpolation|
        |                              |              | (Dead Reckoning)|
        |                              |              +------------------+
        |                              |                       |
        |                              |                       |  interpolatedFlights
        |                              |                       v
        |                              |              +------------------+
        |                              |              | Mapbox GL JS    |
        |                              |              | (60fps render)  |
        |                              |              +------------------+
```

---

## 2. Tech Stack

### Frontend Layer

| Component | Technology | Version |
|-----------|------------|---------|
| Framework | Next.js | 16.1.1 |
| UI Library | React | 19.2.3 |
| Real-time Communication | SignalR (Client) | 10.0.0 |
| Protocol | MessagePack | 10.0.0 |
| Map | Mapbox GL JS | 3.17.0 |
| Icons | Lucide React | 0.562.0 |
| Styling | Tailwind CSS | 4 |
| Language | TypeScript | 5 |

### Backend Layer

| Component | Technology | Version |
|-----------|------------|---------|
| Framework | ASP.NET Core | 9.0 |
| Real-time | SignalR | 9.0.0 |
| Protocol | MessagePack | 3.1.4 |
| Database ORM | Entity Framework Core | 9.0.0 |
| Database Provider | Npgsql (PostgreSQL) | 9.0.0 |
| Cache | StackExchange.Redis | 2.10.1 |
| HTTP Client | HttpClientFactory | Built-in |
| Resilience | Polly | via Microsoft.Extensions.Http.Polly |

### Data Layer

| Component | Technology |
|-----------|------------|
| Database | PostgreSQL |
| Cache | Redis (Alpine) |

### External Services

| Service | Purpose |
|---------|---------|
| AirLabs API | Flight tracking data (airlabs.co) |

---

## 3. Project Structure

### Backend Structure

```
backend/src/
├── QueenFlight.API/                    # Presentation Layer
│   ├── Controllers/
│   │   ├── AuthController.cs           # Authentication (placeholder)
│   │   ├── AlertsController.cs         # Alerts (placeholder)
│   │   └── FlightDetailsController.cs  # Flight detail API
│   ├── Hubs/
│   │   └── FlightHub.cs                # SignalR Hub for real-time flight data
│   ├── Services/
│   │   └── SignalRFlightBroadcaster.cs # SignalR broadcasting implementation
│   ├── BackgroundServices/
│   │   └── FlightFetcherJob.cs         # Placeholder background job
│   ├── Program.cs                      # Application entry point & DI setup
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── QueenFlight.Infrastructure/        # Infrastructure Layer
│   ├── Data/
│   │   └── AppDbContext.cs            # EF Core DbContext
│   ├── Services/
│   │   ├── FlightDataService.cs        # Background service polling AirLabs
│   │   ├── RedisFlightCache.cs         # Redis caching implementation
│   │   └── FlightDetailsProvider.cs    # Flight detail retrieval & auto-seeding
│   ├── ExternalServices/
│   │   ├── AirLabsClient.cs            # AirLabs API client
│   │   ├── GmailSmtpService.cs         # Email service
│   │   └── TelegramBotService.cs       # Telegram bot service
│   ├── Interfaces/
│   │   └── IFlightCache.cs            # Cache interface
│   └── Migrations/                     # EF Core migrations
│
└── QueenFlight.Core/                   # Domain Layer
    ├── Entities/
    │   ├── Aircraft.cs                 # Aircraft registry
    │   ├── AircraftModel.cs            # Aircraft model definitions
    │   ├── Airline.cs                  # Airline information
    │   ├── Airport.cs                  # Airport information
    │   ├── Country.cs                  # Country reference data
    │   ├── Manufacturer.cs             # Aircraft manufacturers
    │   ├── User.cs                     # User accounts
    │   └── UserPreference.cs           # User preferences
    ├── DTOs/
    │   ├── FlightPayloadDto.cs         # Real-time flight data DTO (MessagePack)
    │   ├── FlightDetailDto.cs          # Detailed flight information DTO
    │   └── FlightPositionDto.cs        # Position DTO (placeholder)
    ├── Models/
    │   └── FlightState.cs              # Flight state record
    ├── Interfaces/
    │   ├── IFlightBroadcaster.cs       # Broadcasting interface
    │   ├── IFlightCache.cs             # Cache interface
    │   ├── IFlightDataClient.cs         # Flight data client (placeholder)
    │   └── IFlightDetailsProvider.cs   # Flight details provider interface
    └── Enums/
        └── UpdateFrequency.cs          # Placeholder enum
```

### Frontend Structure

```
frontend/src/
├── app/
│   ├── layout.tsx                     # Root layout with fonts
│   ├── page.tsx                       # Main page (Home)
│   └── globals.css                    # Global styles
│
├── components/
│   ├── FlightDetailModal.tsx          # Flight detail sidebar modal
│   ├── map/
│   │   ├── MapContainer.tsx           # Main map component (Mapbox)
│   │   ├── RadarMap.tsx              # Radar map component
│   │   ├── PlaneMarker.tsx           # Plane marker component
│   │   └── UserLocation.tsx          # User location component (empty)
│   └── ui/
│       ├── button.tsx                 # Button component
│       └── dialog.tsx                # Dialog component
│
├── hooks/
│   ├── useFlightSignalR.ts            # SignalR connection & flight data subscription
│   ├── useFlightData.ts               # Flight data hook (empty)
│   ├── useInterpolation.ts            # 60fps position interpolation (Dead Reckoning)
│   ├── useGeolocation.ts              # Geolocation hook (empty)
│   └── useAuth.ts                     # Authentication hook (empty)
│
├── lib/
│   ├── firebase.ts                    # Firebase config (empty)
│   ├── signalr-connection.ts          # SignalR connection setup (empty)
│   ├── mapbox-config.ts               # Mapbox configuration (empty)
│   └── utils.ts                      # Utility functions (empty)
│
├── services/
│   └── api.ts                        # API service (empty)
│
└── types/
    └── flight.ts                      # Flight type definitions
```

---

## 4. Data Flow

### Real-time Flight Data Flow

```
AirLabs API (600s polling)
    │
    v
FlightDataService.ExecuteAsync()
    │
    ├── HTTP GET /flights?api_key=xxx
    │
    v
Parse JSON Response
    │
    v
For each flight:
    │
    ├── TryMapAirLabs() -> FlightState
    │
    ├── Validate (Lat, Lng, TrueTrack, Velocity required)
    │
    ├── IFlightCache.SetFlightAsync()
    │       │
    │       v
    │   RedisFlightCache
    │       │
    │       v
    │   StringSetAsync("aircraft:{icao24}", json, TTL: 5min)
    │
    └── Map to FlightPayloadDto
            │
            v
    IFlightBroadcaster.BroadcastFlightDataAsync()
            │
            v
    SignalRFlightBroadcaster
            │
            v
    IHubContext<FlightHub>.Clients.Group("GlobalFlightData")
            │
            v
    SendAsync("ReceiveFlightData", flights)
            │
            v
    SignalR (MessagePack Protocol)
            │
            v
Frontend: useFlightSignalR
            │
            v
    connection.on("ReceiveFlightData", (data) => ...)
            │
            v
    setFlightData(mappedData)
            │
            v
    useInterpolation (60fps animation loop)
            │
            v
    requestAnimationFrame(animate)
            │
            v
    Extrapolate positions: NewPos = OldPos + (Velocity * DeltaTime)
            │
            v
    setInterpolatedData(nextFrameData)
            │
            v
    Mapbox GL JS updateSource()
```

---

## 5. Key Components

### Backend Components

#### FlightHub (`QueenFlight.API.Hubs.FlightHub`)
SignalR Hub for real-time flight data broadcasting.
- `SubscribeToUpdates()` — Joins "GlobalFlightData" group
- `UnsubscribeFromUpdates()` — Leaves "GlobalFlightData" group
- `OnConnectedAsync()` — Logs new client connection
- `OnDisconnectedAsync()` — Logs client disconnection
- Connection endpoint: `/flighthub`

#### SignalRFlightBroadcaster (`QueenFlight.API.Services.SignalRFlightBroadcaster`)
Implements `IFlightBroadcaster` to broadcast flight data via SignalR.
- `BroadcastFlightCountAsync(int count)` — Sends flight count to group
- `BroadcastFlightDataAsync(List<FlightPayloadDto> flights)` — Sends flight list to group

#### FlightDataService (`QueenFlight.Infrastructure.Services.FlightDataService`)
Background service that polls AirLabs API every 600 seconds.
- Implements `BackgroundService` (IHostedService)
- Uses Polly retry policy with exponential backoff (3 retries, 2^n seconds)
- Fetches global flights from AirLabs API
- Caches flights in Redis with 5-minute TTL
- Broadcasts updates via `IFlightBroadcaster`

#### RedisFlightCache (`QueenFlight.Infrastructure.Services.RedisFlightCache`)
Redis-based flight position caching.
- Key pattern: `aircraft:{icao24}`
- TTL: 5 minutes
- `SetFlightAsync(FlightState flight)` — Store flight
- `GetFlightAsync(string icao24)` — Retrieve single flight
- `GetAllFlightsAsync()` — Retrieve all cached flights

#### FlightDetailsProvider (`QueenFlight.Infrastructure.Services.FlightDetailsProvider`)
Provides enriched flight details with auto-seeding capability.
- **Cache Hit Path:** Check local PostgreSQL DB for aircraft metadata
- **Cache Miss Path:** Fetch from AirLabs API + auto-seed database
- **Auto-Seeding Logic:**
  - Creates Airline if not exists
  - Creates Manufacturer if not exists
  - Creates AircraftModel if not exists
  - Creates/Updates Aircraft record

#### AirLabsClient (`QueenFlight.Infrastructure.ExternalServices.AirLabsClient`)
HTTP client for AirLabs API endpoints.
- `GetFlightDetailsAsync(string icao24)` — Get single flight details
- `GetAirlinesAsync()` — Get all airlines
- `GetAirportsAsync()` — Get all airports

#### AppDbContext (`QueenFlight.Infrastructure.Data.AppDbContext`)
EF Core DbContext for PostgreSQL.
- **Reference Data:** Countries, Manufacturers, AircraftModels
- **Flight Metadata:** Airlines, Airports
- **Registry:** Aircrafts
- **Users:** Users, UserPreferences

### Frontend Components

#### useFlightSignalR (`frontend/src/hooks/useFlightSignalR.ts`)
React hook for SignalR real-time communication.
- Connects to `/flighthub` with MessagePack protocol
- Automatic reconnection on connection loss
- Subscribes to "GlobalFlightData" group
- Listens for: `ReceiveFlightCount`, `ReceiveFlightData`
- Maps MessagePack keys to JavaScript properties (i, la, lo, v, h, ts)

#### useInterpolation (`frontend/src/hooks/useInterpolation.ts`)
60fps animation loop for smooth flight position extrapolation.
- Implements Dead Reckoning physics
- Updates positions based on velocity and heading
- Uses `requestAnimationFrame` for 60fps rendering
- Converts velocity/heading to Lat/Lng displacement

#### MapContainer (`frontend/src/components/map/MapContainer.tsx`)
Main map component integrating Mapbox GL JS.
- Initializes Mapbox with dark style and globe projection
- Adds flight icon and GeoJSON source
- Renders planes as symbol layer with rotation
- Click handler for flight selection
- Updates GeoJSON on each interpolation frame

#### FlightDetailModal (`frontend/src/components/FlightDetailModal.tsx`)
Sidebar modal for flight details.
- Fetches flight details from `/api/flights/{icao24}`
- Displays airline info, registration, route
- Uses inline styles (bypasses Tailwind issues)
- Slide-in sidebar at 400px width

---

## 6. Database Schema

### Entity Relationship Diagram

```
┌─────────────────┐       ┌─────────────────┐       ┌─────────────────┐
│   countries     │       │ manufacturers   │       │    airlines     │
├─────────────────┤       ├─────────────────┤       ├─────────────────┤
│ PK iso_code(2)  │       │ PK id           │       │ PK id           │
│    name         │◄──────│ FK country_code │       │ FK country_code │◄┐
│    region       │       │    name          │       │    iata_code(2) │
└─────────────────┘       └─────────────────┘       │    icao_code(3) │ │
                                                     │    name         │ │
┌─────────────────┐       ┌─────────────────┐       │    callsign     │ │
│   airports      │       │ aircraft_models │       │    logo_url     │ │
├─────────────────┤       ├─────────────────┤       └─────────────────┼─┘
│ PK ident(15)    │       │ PK id           │               ^           │
│ FK country_code │◄──────│ FK manufacturer │◄──────────────┘           │
│    iata_code(3) │       │    name         │                              │
│    icao_code(4) │       │ icao_type_code  │       ┌─────────────────┐ │
│    name         │       │ wake_turbulence │       │    aircrafts    │ │
│    city         │       └─────────────────┘       ├─────────────────┤ │
│    latitude     │                                 │ PK icao24(24)   │ │
│    longitude    │                                 │ FK model_id     │◄┘
└─────────────────┘                                 │ FK airline_id   │◄─┐
                                                    │    registration │   │
┌─────────────────┐       ┌─────────────────┐     │    owner        │   │
│ user_preferences │       │      users      │     │ last_update     │   │
├─────────────────┤       ├─────────────────┤     │ created_at      │   │
│ PK user_id (FK) │◄──┐   │ PK id           │     └─────────────────┘   │
│ FK home_airport  │◄─┐│   │    email        │             ^             │
│    map_style     │  ││   │    password     │             │             │
│    show_weather  │  ││   │    role         │             │             │
└─────────────────┘  ││   └─────────────────┘             │             │
         ^           │└────────────────────────────────────┘             │
         │           └──────────────────────────────────────────────────┘
         │
    (points to airports.iata_code)
```

### Table Summary

| Table | Primary Key | Foreign Keys | Description |
|-------|-------------|--------------|-------------|
| `countries` | `iso_code` (2 char) | — | Country reference data |
| `manufacturers` | `id` | — | Aircraft manufacturers (Boeing, Airbus) |
| `aircraft_models` | `id` | `manufacturer_id` | Aircraft model types (B789, A320neo) |
| `airlines` | `id` | `country_code` | Airline information with IATA/ICAO codes |
| `airports` | `ident` | `country_code`, `home_airport_iata` | Airport data with IATA/ICAO codes |
| `aircrafts` | `icao24` | `model_id`, `airline_id` | Aircraft registry with metadata |
| `users` | `id` (UUID) | — | User accounts |
| `user_preferences` | `user_id` | `user_id` (1:1), `home_airport_iata` | User preferences |

---

## 7. SignalR Hub

### Hub Configuration

```csharp
// Program.cs
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
    hubOptions.EnableDetailedErrors = true;
})
.AddMessagePackProtocol();
```

### FlightHub Methods

| Method | Parameters | Description |
|--------|------------|-------------|
| `SubscribeToUpdates` | none | Joins "GlobalFlightData" group |
| `UnsubscribeFromUpdates` | none | Leaves "GlobalFlightData" group |

### Client Messages (Server to Client)

| Message Name | Payload | Description |
|-------------|---------|-------------|
| `ReceiveFlightCount` | `int` | Total active flight count |
| `ReceiveFlightData` | `FlightPayloadDto[]` | Array of flight positions |

### FlightPayloadDto (MessagePack Serialized)

```csharp
[MessagePackObject]
public class FlightPayloadDto
{
    [Key("i")]  public string  Icao24 { get; set; }      // ICAO24 hex
    [Key("la")] public double  Lat { get; set; }         // Latitude
    [Key("lo")] public double  Lng { get; set; }         // Longitude
    [Key("v")]  public float   Velocity { get; set; }    // Velocity (km/h or m/s)
    [Key("h")]  public float   Heading { get; set; }     // True track (degrees)
    [Key("ts")] public long    ServerTimestamp { get; set; } // Unix timestamp
}
```

---

## 8. Caching Strategy

| Key Pattern | Value | TTL | Purpose |
|-------------|-------|-----|---------|
| `aircraft:{icao24}` | JSON-serialized FlightState | 5 minutes | Real-time flight position cache |

- **TTL-based expiration** — 5-minute auto-invalidates stale data
- **Polling-based refresh** — FlightDataService refreshes every 600 seconds
- **Miss handling** — Missing flights refetched on next poll cycle

---

## 9. External Integrations

### AirLabs API

**Base URL:** `https://airlabs.co/api/v9`

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/flights` | GET | Global flight list (polled every 600s) |
| `/flight` | GET | Single flight details (on-demand) |
| `/airlines` | GET | Airline reference data |
| `/airports` | GET | Airport reference data |

### AirLabs Response Mapping

```
AirLabs Field              -> FlightState Property
----------------------------------------------
hex                        -> Icao24
flight_number/flight_icao  -> Callsign
flag                       -> OriginCountry
lng                        -> Longitude
lat                        -> Latitude
alt                        -> BaroAltitude
status == "ground"         -> OnGround
speed                      -> Velocity
dir                        -> TrueTrack
v_speed                    -> VerticalRate
squawk                     -> Squawk
```

### Resilience Pattern

```csharp
// Polly retry policy with exponential backoff
_retryPolicy = Policy
    .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
    .Or<HttpRequestException>()
    .WaitAndRetryAsync(3, retryAttempt =>
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
```

---

## 10. Implementation Status

| Requirement | Status | Notes |
|-------------|--------|-------|
| US-01: Real-time Map View | Fully Implemented | Mapbox dark mode, SignalR updates |
| US-02: Flight Details & Interpolation | Fully Implemented | 60fps dead reckoning, auto-seeding |
| US-03: Flight Search | NOT STARTED | No search UI, no flyTo animation |
| US-04: User Geolocation | Stub/Empty | `useGeolocation.ts` and `UserLocation.tsx` empty |
| SY-01: Data Ingestion (120s poll) | **Bug** | Uses 600s, not 120s as specified |
| SY-02: Broadcasting Service | Fully Implemented | SignalR + MessagePack |
| AD-01: Metadata Sync | NOT STARTED | No force refresh admin endpoint |
