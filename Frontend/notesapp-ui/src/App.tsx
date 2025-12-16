import { useEffect, useState } from "react";

import AppLayout from "./components/layout/AppLayout";
import NotesList from "./components/notes/NotesList";
import NoteEditor from "./components/notes/NoteEditor";
import Modal from "./components/ui/Modal";
import Button from "./components/ui/Button";
import Input from "./components/ui/Input";

import { useNotesPagination } from "./hooks/useNotesPagination";
import type { Note } from "./features/notes/types";
import ShortcutsModal from "./components/ui/ShortcutsModal";

function App() {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(5);
  const [search, setSearch] = useState("");

  const {
    notes,
    totalCount,
    loading,
    addNote,
    updateNote,
    deleteNote,
  } = useNotesPagination(page, pageSize);

  const [modalOpen, setModalOpen] = useState(false);
  const [editingNote, setEditingNote] = useState<Note | null>(null);
  const [shortcutsOpen, setShortcutsOpen] = useState(false);

  /* =========================
     Keyboard Shortcuts
     ?  → Open shortcuts
     /  → Focus search
  ========================= */
  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if (e.key === "?") {
        setShortcutsOpen(true);
      }

      if (e.key === "/" && !modalOpen) {
        e.preventDefault();
        const el = document.getElementById("search-input");
        el?.focus();
      }
    };

    window.addEventListener("keydown", handler);
    return () => window.removeEventListener("keydown", handler);
  }, [modalOpen]);

  /* =========================
     Save handler
  ========================= */
  const handleSave = async (title: string, content: string) => {
    if (editingNote) {
      await updateNote(editingNote.id, title, content);
    } else {
      await addNote(title, content);
    }

    setModalOpen(false);
    setEditingNote(null);
    setPage(1);
  };

  /* =========================
     Sort + Search
  ========================= */
  const sortedNotes = [...notes].sort((a, b) => {
    if (a.pinned && !b.pinned) return -1;
    if (!a.pinned && b.pinned) return 1;
    return (
      new Date(b.updatedAt).getTime() -
      new Date(a.updatedAt).getTime()
    );
  });

  const filteredNotes = sortedNotes.filter((note) => {
    const q = search.toLowerCase();
    return (
      note.title.toLowerCase().includes(q) ||
      note.content.toLowerCase().includes(q)
    );
  });

  return (
    <AppLayout>
      {/* ================= HEADER ================= */}
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-semibold">Notes App</h1>

        <div className="flex gap-3 items-center">
          {/* Search */}
          <Input
            id="search-input"
            placeholder="Search notes…"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="
              w-56
              bg-white dark:bg-gray-800
              text-gray-900 dark:text-gray-100
              border border-gray-300 dark:border-gray-600
              placeholder:text-gray-400 dark:placeholder:text-gray-500
            "
          />

          {/* Page size */}
          <select
            value={pageSize}
            onChange={(e) => {
              setPage(1);
              setPageSize(Number(e.target.value));
            }}
            className="
              border px-2 py-1 rounded
              bg-white dark:bg-gray-800
              text-gray-900 dark:text-gray-100
              border-gray-300 dark:border-gray-600
            "
          >
            <option value={5}>5</option>
            <option value={10}>10</option>
            <option value={20}>20</option>
          </select>

          <Button onClick={() => setModalOpen(true)}>
            ➕ Create
          </Button>
        </div>
      </div>

      {/* ================= CONTENT ================= */}
      {loading ? (
        <div className="text-center text-gray-500">
          Loading…
        </div>
      ) : filteredNotes.length === 0 ? (
        <div className="text-center py-20 space-y-4">
          <div className="text-6xl">📝</div>
          <h2 className="text-xl font-semibold">
            No notes found
          </h2>
          <p className="text-gray-500 dark:text-gray-400">
            Create your first note or try a different search.
          </p>
          <Button onClick={() => setModalOpen(true)}>
            ➕ Create your first note
          </Button>
        </div>
      ) : (
        <NotesList
          notes={filteredNotes}
          page={page}
          pageSize={pageSize}
          totalCount={totalCount}
          onPageChange={setPage}
          onEdit={(note) => {
            setEditingNote(note);
            setModalOpen(true);
          }}
          onDelete={async (id) => {
            await deleteNote(id);
            setPage(1);
          }}
          onTogglePin={() => {}}
        />
      )}

      {/* ================= NOTE MODAL ================= */}
      <Modal open={modalOpen} onClose={() => setModalOpen(false)}>
        <NoteEditor
          initialTitle={editingNote?.title}
          initialContent={editingNote?.content}
          onSave={handleSave}
        />
      </Modal>

      {/* ================= SHORTCUTS MODAL ================= */}
      <Modal
        open={shortcutsOpen}
        onClose={() => setShortcutsOpen(false)}
      >
        <ShortcutsModal />
      </Modal>
    </AppLayout>
  );
}

export default App;
