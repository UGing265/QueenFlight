# Task: Real-time Map View (Frontend Foundation)
## Layout Strategy
- **Visual Hierarchy**: Full-screen layout (`w-full h-screen`).
- **Atomic Design**:
    - `Atoms`: 
        - `FlightMarker` (Div with Rotatable SVG Icon).
        - `MapContainer` (Leaflet/Mapbox Wrapper).
    - `Organisms`: `MapPage` (Combines MapContainer + Overlay UI).

## Logic Flow
1. **Init**: `useFlightSignalR` hook connects to Backend (`/flighthub`).
2. **Listen**: Subscribes to `"GlobalFlightData"` channel.
3. **Update**: On `ReceiveFlightData` event -> Update React State (`flightData`).
4. **Render**: `MapContainer` iterates `flightData` -> Updates `mapboxgl.Marker` position & rotation.

## architectural Rationale
- **SignalR Hook**: Decoupled socket logic from UI. Allows reusability in other/future components.
- **Smart Markers**: Markers manage their own DOM references (`useRef <Map>`) instead of full React re-renders for performance (60fps target).
- **Dark Mode**: Default style `mapbox://styles/mapbox/dark-v11` for high contrast aviation aesthetic.

## Backend Contract
```typescript
interface FlightPayload {
    icao24: string;    // Unique ID
    lat: number;       // WGS84
    lng: number;       // WGS84
    heading: number;   // 0-360 degrees
    velocity: number;  // km/h
    serverTimestamp: number;
}
```
