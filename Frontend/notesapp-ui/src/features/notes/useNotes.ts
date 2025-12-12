import { useEffect, useState } from "react";
import type { Note } from "./types";
import { notesApi } from "../../services/notesApi";

export function useNotes() {
  const [notes, setNotes] = useState<Note[]>([]);
  const [loading, setLoading] = useState(true);

  // ========================
  // Fetch notes on load
  // ========================
  useEffect(() => {
    const loadNotes = async () => {
      try {
        const data: Note[] = await notesApi.getNotes();
        setNotes(data);
      } catch (err) {
        console.error("Failed to load notes", err);
      } finally {
        setLoading(false);
      }
    };

    loadNotes();
  }, []);

  // ========================
  // Create note
  // ========================
  const addNote = async (title: string, content: string) => {
    const created: Note = await notesApi.createNote(title, content);
    setNotes((prev) => [created, ...prev]);
  };

  // ========================
  // Update note
  // ========================
  const updateNote = async (
    id: string,
    title: string,
    content: string
  ) => {
    const updated: Note = await notesApi.updateNote(id, title, content);
    setNotes((prev) =>
      prev.map((n) => (n.id === id ? updated : n))
    );
  };

  // ========================
  // Delete note
  // ========================
  const deleteNote = async (id: string) => {
    await notesApi.deleteNote(id);
    setNotes((prev) => prev.filter((n) => n.id !== id));
  };

  // ========================
  // Toggle pin
  // ========================
  const togglePin = async (id: string) => {
    setNotes((prev) =>
      prev.map((n) =>
        n.id === id ? { ...n, pinned: !n.pinned } : n
      )
    );
  };

  return {
    notes,
    loading,
    addNote,
    updateNote,
    deleteNote,
    togglePin,
  };
}
