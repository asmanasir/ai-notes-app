import { api } from "./api";
import type { Note } from "../features/notes/types";

export const notesApi = {
  async getNotes(): Promise<Note[]> {
    const res = await api.get("/Notes");
    return res.data;
  },

  async createNote(title: string, content: string) {
    const res = await api.post("/Notes", { title, content });
    return res.data;
  },

  async updateNote(id: string, title: string, content: string) {
    const res = await api.put(`/Notes/${id}`, { title, content });
    return res.data;
  },

  async deleteNote(id: string) {
    await api.delete(`/Notes/${id}`);
  },
};
