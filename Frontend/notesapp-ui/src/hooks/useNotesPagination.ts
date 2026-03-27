import { useCallback, useEffect, useState } from "react";
import { notesApi } from "../services/notesApi";
import type { Note } from "../features/notes/types";

export function useNotesPagination(page: number, pageSize: number, search?: string, enabled = true) {
  const [notes, setNotes] = useState<Note[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadNotes = useCallback(async () => {
    if (!enabled) return;
    try {
      setLoading(true);
      setError(null);
      const data = await notesApi.getNotesPaged(page, pageSize, search);
      setNotes(data.items);
      setTotalCount(data.totalCount);
    } catch {
      setError("Failed to load notes");
    } finally {
      setLoading(false);
    }
  }, [page, pageSize, search, enabled]);

  useEffect(() => {
    loadNotes();
  }, [loadNotes]);

  const addNote = async (title: string, content: string, tags: string[] = []) => {
    await notesApi.createNote(title, content, tags);
    await loadNotes();
  };

  const updateNote = async (
    id: string,
    title: string,
    content: string,
    tags: string[] = [],
    pinned: boolean = false
  ) => {
    await notesApi.updateNote(id, title, content, tags, null, pinned);
    await loadNotes();
  };

  const togglePin = async (id: string) => {
    await notesApi.togglePin(id);
    await loadNotes();
  };

  const deleteNote = async (id: string) => {
    await notesApi.deleteNote(id);
    await loadNotes();
  };

  return {
    notes,
    totalCount,
    loading,
    error,
    addNote,
    updateNote,
    togglePin,
    deleteNote,
    reload: loadNotes,
  };
}
