import { X, Plane, ExternalLink, MapPin } from "lucide-react";
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
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/40 backdrop-blur-sm" onClick={onClose}>
            {/* Modal Content - Stop propagation to prevent closing when clicking inside */}
            <div
                className="relative w-full max-w-md bg-slate-900/90 border border-slate-700 rounded-2xl shadow-2xl overflow-hidden text-slate-100"
                onClick={(e) => e.stopPropagation()}
            >
                {/* Header */}
                <div className="relative h-32 bg-gradient-to-r from-blue-900 via-indigo-900 to-slate-900 flex items-center justify-center overflow-hidden">
                    {/* Background Pattern */}
                    <div className="absolute inset-0 opacity-20 bg-[url('https://www.transparenttextures.com/patterns/carbon-fibre.png')]"></div>

                    <button
                        onClick={onClose}
                        className="absolute top-3 right-3 p-2 bg-black/20 hover:bg-black/40 rounded-full transition-colors"
                    >
                        <X size={20} className="text-white" />
                    </button>

                    {loading ? (
                        <div className="animate-pulse flex flex-col items-center">
                            <div className="h-4 w-24 bg-white/20 rounded mb-2"></div>
                            <div className="h-8 w-32 bg-white/20 rounded"></div>
                        </div>
                    ) : (data && (
                        <div className="text-center z-10">
                            {data.airlineLogoUrl ? (
                                <img src={data.airlineLogoUrl} alt="Logo" className="h-12 object-contain mx-auto mb-2 drop-shadow-lg" />
                            ) : (
                                <Plane size={40} className="mx-auto mb-2 text-blue-400" />
                            )}
                            <h2 className="text-2xl font-bold tracking-wider">{data.callsign || data.icao24.toUpperCase()}</h2>
                            {data.airlineName && <p className="text-sm text-blue-200">{data.airlineName}</p>}
                        </div>
                    ))}
                </div>

                {/* Body */}
                <div className="p-6 space-y-4">
                    {loading ? (
                        <SkeletonLoader />
                    ) : error ? (
                        <div className="text-center py-6 text-red-400 bg-red-900/10 rounded-xl">
                            <p>{error}</p>
                        </div>
                    ) : (data && (
                        <>
                            {/* Route Info */}
                            <div className="flex items-center justify-between bg-slate-800/50 p-4 rounded-xl border border-slate-700/50">
                                <div className="text-center">
                                    <p className="text-xs text-slate-400 mb-1">ORIGIN</p>
                                    <p className="text-xl font-bold text-green-400">{data.originAirport || "N/A"}</p>
                                </div>
                                <div className="flex-1 px-4 flex flex-col items-center">
                                    <Plane className="text-slate-500 rotate-90" size={16} />
                                    <div className="w-full h-px bg-slate-600 my-2 relative">
                                        <div className="absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 w-1 h-1 bg-slate-400 rounded-full"></div>
                                    </div>
                                    <p className="text-[10px] text-slate-500">{data.isEnriched ? "Live Data" : "Estimating"}</p>
                                </div>
                                <div className="text-center">
                                    <p className="text-xs text-slate-400 mb-1">DESTINATION</p>
                                    <p className="text-xl font-bold text-amber-400">{data.destinationAirport || "N/A"}</p>
                                </div>
                            </div>

                            {/* Aircraft Info */}
                            <div className="grid grid-cols-2 gap-3">
                                <InfoItem label="Registration" value={data.registration || "Unknown"} icon={<ExternalLink size={14} />} />
                                <InfoItem label="Model" value={data.model || data.modelCode || "Unknown"} />
                                <InfoItem label="ICAO Code" value={data.icao24} />
                                <InfoItem label="Altitude" value="-- ft" sub="Coming soon" />
                            </div>

                            {/* Image Preview (If available) */}
                            {data.imageUrl && (
                                <div className="mt-4 rounded-xl overflow-hidden border border-slate-700/50 shadow-lg relative group">
                                    <div className="absolute inset-0 bg-gradient-to-t from-black/80 to-transparent transition-opacity group-hover:opacity-60"></div>
                                    <img src={data.imageUrl} alt="Aircraft" className="w-full h-32 object-cover" />
                                    <p className="absolute bottom-2 left-3 text-xs text-white/80 font-mono">Photo © AirLabs</p>
                                </div>
                            )}
                        </>
                    ))}
                </div>

                {/* Footer status */}
                {data && !data.isEnriched && (
                    <div className="bg-amber-900/20 py-2 text-center border-t border-amber-900/30">
                        <p className="text-xs text-amber-500">⚠ Data populated from public lookup (Not real-time verified)</p>
                    </div>
                )}
            </div>
        </div>
    );
}

// Atomic Components (Internal for simplicity)
function InfoItem({ label, value, sub, icon }: { label: string, value: string, sub?: string, icon?: React.ReactNode }) {
    return (
        <div className="bg-slate-800/40 p-3 rounded-lg border border-slate-700/30 hover:bg-slate-800/60 transition-colors">
            <div className="flex items-center justify-between mb-1">
                <p className="text-xs text-slate-400 font-semibold uppercase">{label}</p>
                {icon && <span className="text-slate-500">{icon}</span>}
            </div>
            <p className="text-sm font-medium text-slate-100 truncate">{value}</p>
            {sub && <p className="text-[10px] text-slate-500">{sub}</p>}
        </div>
    );
}

function SkeletonLoader() {
    return (
        <div className="space-y-4 animate-pulse">
            <div className="h-20 bg-slate-800 rounded-xl"></div>
            <div className="grid grid-cols-2 gap-3">
                <div className="h-16 bg-slate-800 rounded-lg"></div>
                <div className="h-16 bg-slate-800 rounded-lg"></div>
                <div className="h-16 bg-slate-800 rounded-lg"></div>
                <div className="h-16 bg-slate-800 rounded-lg"></div>
            </div>
            <div className="h-32 bg-slate-800 rounded-xl"></div>
        </div>
    );
}
