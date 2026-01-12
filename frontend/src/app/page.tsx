import MapContainer from "@/components/map/MapContainer";

export default function Home() {
    return (
        <main className="flex min-h-screen flex-col items-center justify-between p-0 overflow-hidden bg-black">
            <div className="z-10 max-w-5xl w-full items-center justify-between font-mono text-sm lg:flex absolute top-5 left-5 pointer-events-none">
                <p className="fixed left-0 top-0 flex w-full justify-center border-b border-gray-300 bg-gradient-to-b from-zinc-200 pb-6 pt-8 backdrop-blur-2xl dark:border-neutral-800 dark:bg-zinc-800/30 dark:from-inherit lg:static lg:w-auto  lg:rounded-xl lg:border lg:bg-gray-200 lg:p-4 lg:dark:bg-zinc-800/30">
                    QueenFlight&nbsp;
                    <code className="font-mono font-bold">ALPHA</code>
                </p>
            </div>

            <div className="w-full h-screen">
                <MapContainer />
            </div>
        </main>
    );
}
