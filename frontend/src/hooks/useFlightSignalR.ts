"use client";

import { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import { MessagePackHubProtocol } from "@microsoft/signalr-protocol-msgpack";

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
            .withHubProtocol(new MessagePackHubProtocol()) // Enable Binary Protocol
            .configureLogging(signalR.LogLevel.Information) // Enable built-in SignalR logs
            .build();

        setConnection(newConnection);

        // Lifecycle Logs
        newConnection.onreconnecting(error => {
            console.warn(`%c ⚠️ Connection lost. Reconnecting... Error: ${error}`, 'color: orange; font-weight: bold;');
            setIsConnected(false);
        });

        newConnection.onreconnected(connectionId => {
            console.log(`%c ✅ Connection reestablished. ID: ${connectionId}`, 'color: green; font-weight: bold;');
            setIsConnected(true);
            // Re-subscribe just in case
            newConnection.invoke("SubscribeToUpdates").catch(err => console.error("❌ Re-subscribe failed", err));
        });

        newConnection.onclose(error => {
            console.error(`%c ❌ Connection closed permanently. Error: ${error}`, 'color: red; font-weight: bold;');
            setIsConnected(false);
        });

        console.log(`%c 🔌 Starting SignalR connection to: ${HUB_URL}`, 'color: cyan');

        newConnection.start()
            .then(() => {
                console.log("%c ✅ SignalR Connected Successfully!", 'color: lime; font-weight: bold; font-size: 14px');
                setIsConnected(true);

                // Subscribe to 'GlobalFlightData' group if required by Backend
                newConnection.invoke("SubscribeToUpdates")
                    .then(() => console.log("%c 🔔 Subscribed to GlobalFlightData", 'color: cyan'))
                    .catch((err) => console.error("❌ Subscribe failed:", err));

                newConnection.on("ReceiveFlightData", (data: any[]) => {
                    console.log(`%c 📥 RAW EVENT RECEIVED. Items: ${data?.length}`, 'background: #333; color: #fff');

                    // For high-frequency updates, we might want to avoid setState here directly
                    // But for 120s polling, this is fine.
                    if (data?.length > 0) {
                        console.log("📦 Sample Data Item:", data[0]);
                    }

                    // Mapping if needed (in case Backend sends different casing)
                    const mappedData = data.map((d: any) => ({
                        icao24: d.icao24 || d.Icao24,
                        lat: d.lat || d.Lat,
                        lng: d.lng || d.Lng,
                        heading: d.heading || d.Heading,
                        velocity: d.velocity || d.Velocity,
                        serverTimestamp: d.serverTimestamp || d.ServerTimestamp
                    }));

                    setFlightData(mappedData);
                });
            })
            .catch((err) => console.error("❌ Connection failed to start: ", err));

        return () => {
            newConnection.stop();
        };
    }, []); // Empty dependency array to run once on mount

    return { flightData, isConnected };
}
