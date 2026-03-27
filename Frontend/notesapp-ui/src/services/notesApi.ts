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
    pageSize: number,
    search?: string
  ): Promise<PagedResult<Note>> {
    const res = await api.get("/notes", {
      params: { page, pageSize, search: search || undefined },
    });
    return res.data;
  },

  async createNote(
    title: string,
    content: string,
    tags: string[] = [],
    summary: string | null = null
  ) {
    const res = await api.post("/notes", { title, content, tags, summary });
    return res.data;
  },

  async updateNote(
    id: string,
    title: string,
    content: string,
    tags: string[] = [],
    summary: string | null = null,
    pinned: boolean = false
  ) {
    const res = await api.put(`/notes/${id}`, {
      title,
      content,
      tags,
      summary,
      pinned,
    });
    return res.data;
  },

  async togglePin(id: string) {
    const res = await api.patch(`/notes/${id}/pin`);
    return res.data as { id: string; pinned: boolean };
  },

  async deleteNote(id: string) {
    await api.delete(`/notes/${id}`);
  },
};
