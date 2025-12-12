import Navbar from "./Navbar";
export default function AppLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="flex h-screen">
      <div className="flex-1 flex flex-col overflow-y-auto">
        <Navbar />

        <main className="flex-1 p-6 max-w-5xl mx-auto w-full">
        {children}
      </main>

      </div>
    </div>
  );
}
