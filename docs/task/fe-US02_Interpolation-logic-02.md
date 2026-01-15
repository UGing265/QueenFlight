# Task: US-02 Frontend UI & Interpolation Logic

## 1. Logic & Progress Check
- **Goal**: Display smooth aircraft movement (Interpolation) and show flight details in a Modal when clicked.
- **Current State**: 
    - Map renders real-time data from SignalR (but "teleports" every 10s).
    - flightDetailsController API is ready (`GET /api/flights/{icao24}`).
- **Missing**: 
    - `useInterpolation.ts`: Client-side logic to "guess" position between server updates.
    - `FlightDetailModal.tsx`: UI to fetch and display metadata.

## 2. Layout Strategy & Component Tree
### Visual Hierarchy
- **Map View (Z-Index 0)**: Fullscreen background.
- **Overlay Layer (Z-Index 10)**: Aircraft markers (Canvas/WebGL).
- **Modal Layer (Z-Index 50)**: Centered, glassmorphism modal for flight details.

### Atomic Design Tree
- **Atoms**:
    - `PlaneIcon`: Rotatable SVG/Image.
    - `LoadingSpinner`: For async fetch state.
    - `Badge`: For "On Ground" / "In Air" status.
- **Molecules**:
    - `InfoRow`: Label + Value pair (e.g., "Speed: 800 km/h").
    - `AirlineLogo`: Image with fallback text.
- **Organisms**:
    - `FlightDetailModal`: Composed of Header (Logo + CallSign), Body (Aircraft Info), and Footer (Coordinates).
- **Templates**:
    - `MapLayout`: Contains `MapContainer` and `FlightDetailModal`.

## 3. Implementation Plan (Atomic Layers)

### A. useInterpolation Hook (The Physics Engine)
Instead of relying solely on server updates (T=10s), we use **Dead Reckoning**:
- **Equation**: `NewPos = OldPos + (Velocity * TimeDelta)`
- **Loop**: Utilize `requestAnimationFrame` (60fps) for buttery smooth movement.
- **Correction**: When new server data arrives, blend it to avoid "snapping" (Linear Interpolation - Lerp).

### B. FlightDetailModal (The UI)
- **State**: `isOpen`, `isLoading`, `data`, `error`.
- **Trigger**: Click event on `PlaneMarker`.
- **Action**: Fetch `GET /api/flights/{icao24}`.

## 4. Logic Flow
1. **SignalR Update** -> `flightData` arrives -> Update Target Position.
2. **Render Loop (60Hz)** -> `useInterpolation` calculates intermediate `lat/lng` based on velocity/heading.
3. **User Click Plane** -> `setSelectedFlight(icao24)` -> Open Modal -> Set `loading=true`.
4. **Effect** -> Call API -> `loading=false` -> Show Data.

## 5. Architectural Rationale
- **Separation of Concerns**: Interpolation logic is complex math, separated into a custom hook `useInterpolation.ts` to keep `MapContainer` clean.
- **Atomic Modal**: The Modal is built from small, reusable atoms (`InfoRow`), making it easy to add more fields later (e.g., Vertical Speed, Squawk).
- **Performance**: Using `requestAnimationFrame` outside of React Render Cycle (if possible) or optimized State updates to prevent Map lagging.

## 6. Backend Contract
**GET /api/flights/{icao24}**
```json
{
  "icao24": "888123",
  "callsign": "HVN123",
  "airline": { "name": "Vietnam Airlines", "iataCode": "VN" },
  "aircraft": { "model": "Boeing 787-9 Dreamliner", "registration": "VN-A868" },
  "originAirport": { "iataCode": "SGN", "name": "Tan Son Nhat" },
  "destinationAirport": { "iataCode": "HAN", "name": "Noi Bai" }
}
```
