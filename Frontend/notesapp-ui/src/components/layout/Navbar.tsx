import Button from "../ui/Button";
import { useTheme } from "../../hooks/useTheme";
import { useAuth } from "../../context/AuthContext";

export default function Header() {
  const { theme, toggleTheme } = useTheme();
  const { user, logout } = useAuth();

  return (
    <header className="flex items-center justify-between px-6 py-3 border-b border-gray-200 dark:border-gray-700">
      <h1 className="text-lg font-semibold">Notes App</h1>

      <div className="flex items-center gap-3">
        <Button size="sm" variant="secondary" onClick={toggleTheme}>
          {theme === "dark" ? "🌙 Dark" : "☀️ Light"}
        </Button>

        {user && (
          <>
            <span className="text-sm text-gray-500 dark:text-gray-400 hidden sm:block">
              {user.email}
            </span>
            <Button size="sm" variant="secondary" onClick={logout}>
              Log out
            </Button>
          </>
        )}
      </div>
    </header>
  );
}
