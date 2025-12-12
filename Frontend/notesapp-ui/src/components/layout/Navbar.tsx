import Button from "../ui/Button";
import { useTheme } from "../../hooks/useTheme";

export default function Header() {
  const { theme, toggleTheme } = useTheme();

  return (
    <header className="flex items-center justify-between px-6 py-3 border-b border-gray-200 dark:border-gray-700">
      <h1 className="text-lg font-semibold">
        Notes App
      </h1>

      <Button
        size="sm"
        variant="secondary"
        onClick={toggleTheme}
      >
        {theme === "dark" ? "🌙 Dark" : "☀️ Light"}
      </Button>
    </header>
  );
}
