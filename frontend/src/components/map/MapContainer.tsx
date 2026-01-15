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


    const [isMapReady, setIsMapReady] = useState(false);

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
            projection: 'globe'
        });

        // Initialize Fog
        map.current.on('style.load', () => {
            map.current?.setFog({
                color: 'rgb(186, 210, 235)',
                'high-color': 'rgb(36, 92, 223)',
                'horizon-blend': 0.02,
                'space-color': 'rgb(11, 11, 25)',
                'star-intensity': 0.6
            });
        });

        // Initialize Sources & Layers
        map.current.on('load', () => {
            // Load an aircraft icon
            map.current?.loadImage(
                'https://upload.wikimedia.org/wikipedia/commons/thumb/c/c5/Airplane_silhouette.svg/2048px-Airplane_silhouette.svg.png',
                (error, image) => {
                    if (error) {
                        console.error("Could not load flight icon", error);
                        // Even if image fails, we should set ready so map works (maybe without icons or fallback)
                        // But here we return to avoid adding layer referring to missing image
                        // Better practice: Use a fallback or proceed. 
                    }
                    if (!map.current?.hasImage('plane-icon') && image) {
                        map.current?.addImage('plane-icon', image, { sdf: true });
                    }

                    // Add GeoJSON Source
                    if (!map.current?.getSource('flights')) {
                        map.current?.addSource('flights', {
                            type: 'geojson',
                            data: {
                                type: 'FeatureCollection',
                                features: []
                            }
                        });
                    }

                    // Add Layer
                    if (!map.current?.getLayer('flights-layer')) {
                        map.current?.addLayer({
                            id: 'flights-layer',
                            source: 'flights',
                            type: 'symbol',
                            layout: {
                                'icon-image': 'plane-icon', // Refers to the image added above
                                'icon-size': 0.02,
                                'icon-rotate': ['get', 'rotation'],
                                'icon-allow-overlap': true,
                                'icon-ignore-placement': true
                            },
                            paint: {
                                'icon-color': '#4ade80',
                                'icon-halo-color': '#000000',
                                'icon-halo-width': 1
                            }
                        });
                    }

                    // Mark map as ready only after source/layer are added
                    setIsMapReady(true);
                });
        });

    }, []);

    // 2. Update GeoJSON Data (Depends on flightData AND isMapReady)
    useEffect(() => {
        if (!isMapReady || !map.current || !map.current.getSource('flights')) return;

        const features: GeoJSON.Feature[] = flightData.map(flight => ({
            type: 'Feature',
            geometry: {
                type: 'Point',
                coordinates: [flight.lng, flight.lat]
            },
            properties: {
                id: flight.icao24,
                rotation: flight.heading,
                velocity: flight.velocity
            }
        }));

        const source = map.current.getSource('flights') as mapboxgl.GeoJSONSource;
        if (source) {
            source.setData({
                type: 'FeatureCollection',
                features: features
            });
            console.log(`🚀 WebGL: Updated ${features.length} points on GPU`);
        }

    }, [flightData, isMapReady]); // Re-run when map becomes ready

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
