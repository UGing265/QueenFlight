"use client";

import { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";

const HUB_URL = process.env.NEXT_PUBLIC_API_URL + "/flighthub";

export interface FlightPayload {
    icao24: string;
    lat: number;
    lng: number;
    heading: number;
    velocity: number;
    serverTimestamp: number;
}

export function useFlightSignalR() {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [flightData, setFlightData] = useState<FlightPayload[]>([]);
    const [isConnected, setIsConnected] = useState(false);

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl(HUB_URL)
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        setConnection(newConnection);
    }, []);

    useEffect(() => {
        if (connection) {
            connection
                .start()
                .then(() => {
                    console.log("📡 Connected to FlightHub");
                    setIsConnected(true);

                    // Subscribe to 'GlobalFlightData' group if required by Backend
                    connection.invoke("SubscribeToUpdates").catch((err) => console.error(err));

                    connection.on("ReceiveFlightData", (data: FlightPayload[]) => {
                        // For high-frequency updates, we might want to avoid setState here directly
                        // But for 120s polling, this is fine.
                        console.log(`✈️ Received ${data.length} flights`);
                        setFlightData(data);
                    });
                })
                .catch((err) => console.error("❌ Connection failed: ", err));

            return () => {
                connection.stop();
            };
        }
    }, [connection]);

    return { flightData, isConnected };
}
