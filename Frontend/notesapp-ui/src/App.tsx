import { useState } from "react";

import AppLayout from "./components/layout/AppLayout";
import NotesList from "./components/notes/NotesList";
import NoteEditor from "./components/notes/NoteEditor";
import Modal from "./components/ui/Modal";
import Button from "./components/ui/Button";

import { useNotes } from "./features/notes/useNotes";
import type { Note } from "./features/notes/types";

function App() {
  const {
    notes,
    loading,
    addNote,
    updateNote,
    deleteNote,
    togglePin, // ✅ now comes from hook
  } = useNotes();

  const [modalOpen, setModalOpen] = useState(false);
  const [editingNote, setEditingNote] = useState<Note | null>(null);

  const sortedNotes = [...notes].sort(
    (a, b) => Number(b.pinned ?? false) - Number(a.pinned ?? false)
  );

  const openCreate = () => {
    setEditingNote(null);
    setModalOpen(true);
  };

  const openEdit = (note: Note) => {
    setEditingNote(note);
    setModalOpen(true);
  };

  const handleSave = async (title: string, content: string) => {
    if (editingNote) {
      await updateNote(editingNote.id, title, content);
    } else {
      await addNote(title, content);
    }
    setModalOpen(false);
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen text-gray-500">
        Loading notes...
      </div>
    );
  }

  return (
    <AppLayout>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-semibold">Notes App</h1>
        <Button onClick={openCreate}>➕ Create Note</Button>
      </div>

      <NotesList
        notes={sortedNotes}
        onEdit={openEdit}
        onDelete={deleteNote}
        onTogglePin={togglePin} // ✅ WORKS
      />

      <Modal open={modalOpen} onClose={() => setModalOpen(false)}>
        <NoteEditor
          initialTitle={editingNote?.title}
          initialContent={editingNote?.content}
          onSave={handleSave}
        />
      </Modal>
    </AppLayout>
  );
}

export default App;
