"use client";

import React, { useEffect, useRef, useState } from "react";
import mapboxgl from "mapbox-gl";
import "mapbox-gl/dist/mapbox-gl.css";
import { useFlightSignalR, FlightPayload } from "@/hooks/useFlightSignalR";
import { Plane } from "lucide-react";

// Ensure token is present
const MAPBOX_TOKEN = process.env.NEXT_PUBLIC_MAPBOX_TOKEN || "";
if (MAPBOX_TOKEN) {
    mapboxgl.accessToken = MAPBOX_TOKEN;
}

export default function MapContainer() {
    const mapContainer = useRef<HTMLDivElement>(null);
    const map = useRef<mapboxgl.Map | null>(null);
    const markersRef = useRef<Map<string, mapboxgl.Marker>>(new Map());

    const { flightData, isConnected } = useFlightSignalR();

    // 1. Initialize Map
    useEffect(() => {
        if (map.current || !mapContainer.current) return;

        if (!MAPBOX_TOKEN) {
            console.error("❌ No Mapbox Token found!");
            return;
        }

        map.current = new mapboxgl.Map({
            container: mapContainer.current,
            style: "mapbox://styles/mapbox/dark-v11",
            center: [106.79, 10.84], // hcm 
            zoom: 10,
            projection: 'globe' // Globe view for aesthetic
        });

        map.current.on('style.load', () => {
            map.current?.setFog({
                color: 'rgb(186, 210, 235)', // Lower atmosphere
                'high-color': 'rgb(36, 92, 223)', // Upper atmosphere
                'horizon-blend': 0.02, // Atmosphere thickness (default 0.2 at low zooms)
                'space-color': 'rgb(11, 11, 25)', // Background color
                'star-intensity': 0.6 // Background star brightness (default 0.35 at low zooms )
            });
        });

    }, []);

    // 2. Update Markers when Flight Data changes
    useEffect(() => {
        if (!map.current) {
            console.warn("⚠️ Map not initialized yet, skipping marker update");
            return;
        }

        console.log(`🗺️ Rendering markers for ${flightData.length} flights...`);
        const currentFlightIds = new Set<string>();

        flightData.forEach((flight) => {
            currentFlightIds.add(flight.icao24);

            // Check if marker exists
            let marker = markersRef.current.get(flight.icao24);

            if (!marker) {
                // Debug log for first marker creation
                if (markersRef.current.size === 0) {
                    console.log(`📍 Creating first marker for ${flight.icao24} at [${flight.lng}, ${flight.lat}]`);
                }

                // Create new marker element
                const el = document.createElement('div');
                el.className = 'flight-marker';
                // Explicitly set size here just in case CSS fails
                el.style.width = '24px';
                el.style.height = '24px';

                el.innerHTML = `<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#4ade80" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="filter: drop-shadow(0 0 4px #4ade80);"><path d="M2 12h20"/><path d="M13 2l9 10-9 10"/><path d="M7 6l4 6-4 6"/></svg>`;

                marker = new mapboxgl.Marker({ element: el, rotation: flight.heading })
                    .setLngLat([flight.lng, flight.lat])
                    .setPopup(new mapboxgl.Popup({ offset: 25 }).setHTML(`<b>${flight.icao24}</b><br>Speed: ${flight.velocity} km/h`))
                    .addTo(map.current!);

                markersRef.current.set(flight.icao24, marker);
            } else {
                // Update position
                marker.setLngLat([flight.lng, flight.lat]);
                marker.setRotation(flight.heading);
            }
        });

        console.log(`✅ Total markers on map: ${markersRef.current.size}`);

        // Cleanup stale markers
        markersRef.current.forEach((marker, id) => {
            if (!currentFlightIds.has(id)) {
                marker.remove();
                markersRef.current.delete(id);
            }
        });

    }, [flightData]);

    return (
        <div className="relative w-full h-screen">
            {!isConnected && (
                <div className="absolute top-4 left-4 z-50 bg-red-500 text-white px-4 py-2 rounded shadow">
                    📡 Connecting to Live Air Traffic...
                </div>
            )}
            <div ref={mapContainer} className="w-full h-full" />
        </div>
    );
}
