import { createContext, useContext, useState } from "react";
import type { ReactNode } from "react";
import { api } from "../services/api";

interface AuthUser {
  email: string;
}

interface AuthContextValue {
  user: AuthUser | null;
  token: string | null;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

function applyToken(t: string | null) {
  if (t) {
    api.defaults.headers.common["Authorization"] = `Bearer ${t}`;
  } else {
    delete api.defaults.headers.common["Authorization"];
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() => {
    const t = localStorage.getItem("token");
    applyToken(t); // set header synchronously on first load
    return t;
  });
  const [user, setUser] = useState<AuthUser | null>(() => {
    const stored = localStorage.getItem("user");
    return stored ? JSON.parse(stored) : null;
  });

  const save = (t: string, email: string) => {
    applyToken(t);
    setToken(t);
    setUser({ email });
    localStorage.setItem("token", t);
    localStorage.setItem("user", JSON.stringify({ email }));
  };

  const login = async (email: string, password: string) => {
    const res = await api.post<{ token: string; email: string }>("/auth/login", {
      email,
      password,
    });
    save(res.data.token, res.data.email);
  };

  const register = async (email: string, password: string) => {
    const res = await api.post<{ token: string; email: string }>("/auth/register", {
      email,
      password,
    });
    save(res.data.token, res.data.email);
  };

  const logout = () => {
    applyToken(null);
    setToken(null);
    setUser(null);
    localStorage.removeItem("token");
    localStorage.removeItem("user");
  };

  return (
    <AuthContext.Provider value={{ user, token, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside AuthProvider");
  return ctx;
}
