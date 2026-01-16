import { X, Plane, ExternalLink, MapPin, Wind, Navigation } from "lucide-react";
import { useEffect, useState } from "react";

interface FlightDetailDto {
    icao24: string;
    callsign?: string;
    registration?: string;
    airlineName?: string;
    airlineLogoUrl?: string;
    airlineIata?: string;
    manufacturer?: string;
    model?: string;
    modelCode?: string;
    imageUrl?: string;
    originAirport?: string;
    destinationAirport?: string;
    isEnriched: boolean;
}

interface FlightDetailModalProps {
    icao24: string | null;
    onClose: () => void;
}

export default function FlightDetailModal({ icao24, onClose }: FlightDetailModalProps) {
    const [data, setData] = useState<FlightDetailDto | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Fetch Logic
    useEffect(() => {
        if (!icao24) return;

        const fetchData = async () => {
            setLoading(true);
            setError(null);
            setData(null);

            try {
                const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/flights/${icao24}`);
                if (!response.ok) {
                    throw new Error("Failed to fetch flight details");
                }
                const result = await response.json();
                setData(result);
            } catch (err) {
                console.error(err);
                setError("Could not load flight details.");
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [icao24]);

    if (!icao24) return null;

    return (
        // SIDEBAR COMPONENT
        // CRITICAL: USING INLINE STYLES FOR EVERYTHING TO BYPASS TAILWIND FAILURE
        <div
            style={{
                position: 'fixed',
                top: 0,
                left: 0,
                width: '400px', // Fixed width
                height: '100vh',
                zIndex: 999999,
                backgroundColor: 'rgba(15, 23, 42, 0.95)', // Slate-900 equivalent
                color: 'white',
                borderRight: '1px solid #334155',
                display: 'flex',
                flexDirection: 'column',
                boxShadow: '10px 0 30px rgba(0,0,0,0.5)'
            }}
        >
            {/* Header Area */}
            <div style={{ padding: '20px', background: '#1e293b', borderBottom: '1px solid #334155', position: 'relative' }}>
                <button
                    onClick={onClose}
                    style={{ position: 'absolute', top: '10px', right: '10px', background: '#ef4444', color: 'white', border: 'none', padding: '5px 10px', cursor: 'pointer', borderRadius: '4px' }}
                >
                    CLOSE X
                </button>

                {/* Top Level Info */}
                <div style={{ marginTop: '20px' }}>
                    <div>
                        {loading ? (
                            <div style={{ height: '30px', background: '#334155', width: '100px' }}>Loading...</div>
                        ) : (
                            <div style={{ display: 'flex', alignItems: 'center', gap: '8px', color: '#60a5fa', marginBottom: '8px' }}>
                                <Plane size={24} />
                                <span style={{ fontWeight: 'bold', fontSize: '18px' }}>{data?.airlineName || "Private Flight"}</span>
                            </div>
                        )}

                        <h1 style={{ fontSize: '36px', fontWeight: '900', lineHeight: 1 }}>
                            {loading ? "..." : (data?.callsign || data?.icao24?.toUpperCase())}
                        </h1>
                    </div>
                </div>
            </div>

            {/* Scrollable Content */}
            <div style={{ flex: 1, overflowY: 'auto', padding: '20px' }}>

                {loading ? <div>Loading Details...</div> : error ? (
                    <div style={{ color: '#f87171', background: 'rgba(239,68,68,0.1)', padding: '10px', borderRadius: '8px' }}>
                        {error}
                    </div>
                ) : (data && (
                    <>
                        {/* Route Card */}
                        <div style={{ background: 'rgba(30, 41, 59, 0.5)', padding: '20px', borderRadius: '12px', border: '1px solid rgba(51, 65, 85, 0.5)', marginBottom: '20px' }}>
                            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                                <div style={{ textAlign: 'center', width: '33%' }}>
                                    <div style={{ fontSize: '24px', fontWeight: 'bold', color: '#4ade80' }}>{data.originAirport || "N/A"}</div>
                                    <div style={{ fontSize: '10px', color: '#64748b', textTransform: 'uppercase' }}>Origin</div>
                                </div>
                                <div style={{ flex: 1, textAlign: 'center' }}>➔</div>
                                <div style={{ textAlign: 'center', width: '33%' }}>
                                    <div style={{ fontSize: '24px', fontWeight: 'bold', color: '#fbbf24' }}>{data.destinationAirport || "N/A"}</div>
                                    <div style={{ fontSize: '10px', color: '#64748b', textTransform: 'uppercase' }}>Dest</div>
                                </div>
                            </div>
                        </div>

                        {/* Technical Details */}
                        <div>
                            <h3 style={{ fontSize: '12px', fontWeight: 'bold', color: '#64748b', textTransform: 'uppercase', marginBottom: '10px' }}>Flight Details</h3>
                            <div style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                                <p>Registration: {data.registration || "Unknown"}</p>
                                <p>Model: {data.model || data.modelCode}</p>
                                <p>Altitude: -- ft</p>
                            </div>
                        </div>
                    </>
                ))}
            </div>

            {/* Footer */}
            <div style={{ padding: '15px', borderTop: '1px solid #1e293b', background: 'rgba(15, 23, 42, 0.5)', textAlign: 'center', fontSize: '10px', color: '#475569' }}>
                <p>ID: {icao24} • QUEENS FLIGHT RADAR</p>
            </div>
        </div>
    );
}

// Sub-components
function MiniStat({ label, value, color = "text-slate-200" }: { label: string, value: string, color?: string }) {
    return (
        <div className="text-center p-2 bg-slate-900/50 rounded">
            <div className="text-[10px] text-slate-500 uppercase">{label}</div>
            <div className={`text-sm font-semibold ${color}`}>{value}</div>
        </div>
    );
}

function TechRow({ icon, label, value }: { icon: any, label: string, value?: string }) {
    return (
        <div className="flex items-center justify-between p-3 bg-slate-800/30 rounded-lg hover:bg-slate-800/50 transition-colors border border-slate-700/20">
            <div className="flex items-center gap-3 text-slate-400">
                {icon}
                <span className="text-sm font-medium">{label}</span>
            </div>
            <span className="text-sm text-slate-200 font-mono">{value || "N/A"}</span>
        </div>
    );
}

function SkeletonLoader() {
    return (
        <div className="space-y-4">
            <div className="h-32 bg-slate-800 rounded-xl animate-pulse"></div>
            <div className="space-y-2">
                <div className="h-10 bg-slate-800 rounded animate-pulse"></div>
                <div className="h-10 bg-slate-800 rounded animate-pulse"></div>
                <div className="h-10 bg-slate-800 rounded animate-pulse"></div>
            </div>
        </div>
    );
}
