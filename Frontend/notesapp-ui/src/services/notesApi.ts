import { api } from "./api";
import type { Note } from "../features/notes/types";

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
}

export const notesApi = {
  async getNotesPaged(
    page: number,
    pageSize: number
  ): Promise<PagedResult<Note>> {
    const res = await api.get("/notes", {
      params: { page, pageSize },
    });
    return res.data;
  },

  async createNote(title: string, content: string) {
    const res = await api.post("/notes", { title, content });
    return res.data;
  },

  async updateNote(id: string, title: string, content: string) {
    const res = await api.put(`/notes/${id}`, { title, content });
    return res.data;
  },

  async deleteNote(id: string) {
    await api.delete(`/notes/${id}`);
  },
};
