# QueenFlight Web - Real-time Aviation Tracking MVP

**QueenFlight Web** is a browser-based, real-time aviation tracking platform designed for high performance and low latency. Acting as a "personal radar station," it processes raw flight data from external providers into meaningful, visual information without requiring any app installation.

Built with a modern architecture (**Next.js + .NET Core 9**), the system is optimized for concurrency, resilience, and a seamless user experience across devices.

---

## Project Overview

* **Goal:** Provide aviation enthusiasts and travelers with accurate, real-time flight positions.
* **Core Capability:** Synchronize flight data from global sources with sub-10-second latency using .NET's high-performance processing capabilities.
* **Experience:** Instant access via web browser with an interactive, "Dark Mode" radar interface.

---

## Key Features (MVP)

* **Real-time Live Tracking:** Automatic data polling from OpenSky Network every 10-15 seconds.
* **Interactive 3D Map:** Powered by **Mapbox GL JS**, featuring smooth zooming, panning, and user geolocation support.
* **Dynamic Visualization:** Aircraft icons automatically rotate based on actual heading/bearing.
* **Flight Details:** Instant popups displaying Callsign, Altitude (Barometric), Velocity, and Country of Origin.
* **Intelligent Filtering:** Backend logic filters out incomplete or "noise" data (e.g., aircraft with missing coordinates) before broadcasting to clients.
* **Resilience:** Built-in retry policies using **Polly** to handle external API instability.

---

## Tech Stack

### Frontend (Client-side)
* **Framework:** Next.js (App Router)
* **Maps:** Mapbox GL JS & Geocoding API
* **State Management:** React Hooks
* **Real-time:** @microsoft/signalr
* **Styling:** Tailwind CSS

### Backend (Server-side)
* **Core:** ASP.NET Core 9 Web API
* **Communication:** SignalR (MessagePack Protocol)
* **Background Tasks:** Hosted Services (Worker)
* **Resilience:** Polly (Retry & Circuit Breaker patterns)
* **HTTP Client:** IHttpClientFactory with Strongly Typed Clients
* **Data Processing:** LINQ & Parallel.ForEach

### Infrastructure & Data
* **Live Data API:** OpenSky Network
* **Database:** PostgreSQL (Metadata storage)
* **Caching:** Redis (Transient flight state)

---

## Architecture & Implementation

The system follows a strict **Separation of Concerns** principle:

1. **Data Ingestion (Hosted Service):** A background worker in .NET polls the OpenSky API periodically (every 10s). It utilizes IHttpClientFactory to prevent socket exhaustion and Polly to handle network jitters.
2. **Processing Pipeline:**
    * Incoming JSON is mapped to C# Records (Immutable).
    * **Noise Filtering:** Removing aircraft with null coordinates.
    * **Delta Updates:** The logic compares new data against cached states to broadcast only changed entities.
3. **Broadcasting:** Cleaned data is pushed to the **SignalR Hub**, which broadcasts updates to all connected Next.js clients instantly.

---

## Getting Started

### Prerequisites
* .NET 9 SDK
* Node.js (LTS)
* Docker (Optional, for DB/Redis)
* API Keys: Mapbox, OpenSky Network

### 1. Backend Setup

```bash
cd server
# Restore dependencies
dotnet restore

# Configure Secrets (Recommended)
dotnet user-secrets set "OpenSky:Username" "YOUR_USERNAME"
dotnet user-secrets set "OpenSky:Password" "YOUR_PASSWORD"

# Run the API
dotnet run
```

### 2. Frontend Setup

```bash
cd client
# Install dependencies
npm install

# Setup Environment Variables
echo "NEXT_PUBLIC_MAPBOX_TOKEN=pk.your_token_here" > .env.local

# Run the development server
npm run dev
```

Visit http://localhost:3000 to view the application.

---

## Known Constraints

* **API Rate Limits:** The Free Tier of OpenSky Network has request limits, resulting in a potential data delay of 10-20 seconds.
* **Coverage:** Some military or general aviation aircraft may not appear if they lack ADS-B transponders or are filtered by the provider.

---

## Author

**Designed & Developed by Shiroru Thai**
