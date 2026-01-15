import { useEffect, useRef, useState } from "react";
import { FlightPayload } from "./useFlightSignalR";

// Constants for Physics
const EARTH_RADIUS_METERS = 6378137;
const VELOCITY_MULTIPLIER = 1.0; // Adjust if input is km/h (e.g., 0.27778 for km/h -> m/s)

export function useInterpolation(serverData: FlightPayload[]) {
    // This state holds the "Real-time" predicted positions
    const [interpolatedData, setInterpolatedData] = useState<FlightPayload[]>([]);

    // Refs to access latest state inside the animation loop without dependencies
    const dataRef = useRef<FlightPayload[]>([]);
    const lastFrameTimeRef = useRef<number>(0);
    const requestRef = useRef<number>(0);

    // 1. Sync Ref with Server Data whenever it updates
    useEffect(() => {
        // When new data arrives, we "snap" our local model to the fresh source of truth.
        // In a more advanced version, we could "Lerp" (blend) to avoid jumps, 
        // but for <10s intervals or simple tracking, snapping + extrapolation is sufficient.

        // We preserve existing extrapolated entities if they are missing? 
        // No, serverData is the absolute truth list.
        dataRef.current = serverData;
        setInterpolatedData(serverData);
    }, [serverData]);

    // 2. The Physics Loop (60fps)
    const animate = (time: number) => {
        if (lastFrameTimeRef.current !== 0) {
            // Calculate time delta in Seconds (e.g., 0.016s for 60fps)
            const deltaTime = (time - lastFrameTimeRef.current) / 1000;

            if (dataRef.current.length > 0) {
                // Calculate new positions for all aircraft
                const nextFrameData = dataRef.current.map(flight => {
                    // Physics: NewPos = OldPos + (Velocity * Time)
                    // We need to convert meters traveled into Lat/Lng degrees

                    const velocityMPS = flight.velocity * VELOCITY_MULTIPLIER;
                    const distanceMeters = velocityMPS * deltaTime;

                    // Heading: 0 = North, 90 = East
                    // Convert degrees to radians
                    const headingRad = (flight.heading * Math.PI) / 180;

                    // Calculate displacement
                    const deltaLatMeters = distanceMeters * Math.cos(headingRad);
                    const deltaLngMeters = distanceMeters * Math.sin(headingRad);

                    // Convert meters to decimal degrees
                    // 1 deg Lat ~= 111,111 meters
                    // 1 deg Lng ~= 111,111 * cos(lat) meters
                    const deltaLatDeg = deltaLatMeters / 111111;
                    const deltaLngDeg = deltaLngMeters / (111111 * Math.cos(flight.lat * (Math.PI / 180)));

                    return {
                        ...flight,
                        lat: flight.lat + deltaLatDeg,
                        lng: flight.lng + deltaLngDeg
                    };
                });

                // Update Ref AND State
                dataRef.current = nextFrameData;
                setInterpolatedData(nextFrameData);
            }
        }

        lastFrameTimeRef.current = time;
        requestRef.current = requestAnimationFrame(animate);
    };

    // 3. Start/Stop Loop
    useEffect(() => {
        requestRef.current = requestAnimationFrame(animate);
        return () => cancelAnimationFrame(requestRef.current);
    }, []);

    return interpolatedData;
}
