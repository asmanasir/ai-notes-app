import { useCallback, useEffect, useState } from "react";
import { notesApi } from "../services/notesApi";
import type { Note } from "../features/notes/types";

export function useNotesPagination(page: number, pageSize: number) {
  const [notes, setNotes] = useState<Note[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // ========================
  // Fetch notes (shared)
  // ========================
  const loadNotes = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      const data = await notesApi.getNotesPaged(page, pageSize);

      setNotes(data.items);
      setTotalCount(data.totalCount);
    } catch {
      setError("Failed to load notes");
    } finally {
      setLoading(false);
    }
  }, [page, pageSize]);

  // Fetch on mount & page change
  useEffect(() => {
    loadNotes();
  }, [loadNotes]);

  // ========================
  // Create
  // ========================
  const addNote = async (title: string, content: string) => {
    await notesApi.createNote(title, content);
    await loadNotes(); // 🔁 refetch
  };

  // ========================
  // Update
  // ========================
  const updateNote = async (
    id: string,
    title: string,
    content: string
  ) => {
    await notesApi.updateNote(id, title, content);
    await loadNotes(); // 🔁 refetch
  };

  // ========================
  // Delete
  // ========================
  const deleteNote = async (id: string) => {
    await notesApi.deleteNote(id);
    await loadNotes(); // 🔁 refetch
  };

  return {
    notes,
    totalCount,
    loading,
    error,

    // actions
    addNote,
    updateNote,
    deleteNote,

    // manual reload (optional)
    reload: loadNotes,
  };
}
