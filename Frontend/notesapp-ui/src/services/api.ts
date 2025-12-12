import axios from "axios";

export const api = axios.create({
  baseURL: "https://localhost:7110/api",  // <-- FIX FOR ASP.NET BACKEND
  headers: {
    "Content-Type": "application/json",
  },
});
