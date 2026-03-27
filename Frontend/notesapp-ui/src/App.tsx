import { useEffect, useRef, useState } from "react";
import { api } from "./services/api";

import AppLayout from "./components/layout/AppLayout";
import NotesList from "./components/notes/NotesList";
import NoteEditor from "./components/notes/NoteEditor";
import Modal from "./components/ui/Modal";
import Button from "./components/ui/Button";
import Input from "./components/ui/Input";

import { useNotesPagination } from "./hooks/useNotesPagination";
import type { Note } from "./features/notes/types";
import ShortcutsModal from "./components/ui/ShortcutsModal";
import NoteCardSkeleton from "./components/notes/NoteCardSkeleton";

function App() {
  // Warm up the API on first render to reduce cold-start delay
  const warmedUp = useRef(false);
  useEffect(() => {
    if (!warmedUp.current) {
      warmedUp.current = true;
      api.get("/health").catch(() => {});
    }
  }, []);

  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(5);
  const [search, setSearch] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const [activeTag, setActiveTag] = useState<string | null>(null);

  // Debounce search so we don't fire a request on every keystroke
  useEffect(() => {
    const timer = setTimeout(() => setDebouncedSearch(search), 300);
    return () => clearTimeout(timer);
  }, [search]);

  const {
    notes,
    totalCount,
    loading,
    addNote,
    updateNote,
    togglePin,
    deleteNote,
  } = useNotesPagination(page, pageSize, debouncedSearch);

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
  const handleSave = async (title: string, content: string, tags: string[]) => {
    if (editingNote) {
      await updateNote(editingNote.id, title, content, tags, editingNote.pinned);
    } else {
      await addNote(title, content, tags);
    }

    setModalOpen(false);
    setEditingNote(null);
    setPage(1);
  };

  /* =========================
     Tag filter (client-side on current page)
  ========================= */
  const filteredNotes = activeTag
    ? notes.filter((n) => n.tags.includes(activeTag))
    : notes;

  /* Collect all unique tags from current page for the filter bar */
  const allTags = Array.from(new Set(notes.flatMap((n) => n.tags))).sort();

  return (
    <AppLayout>
      {/* ================= HEADER ================= */}
      <div className="flex items-center justify-end mb-4">
        <div className="flex gap-3 items-center">
          {/* Search */}
          <Input
            id="search-input"
            placeholder="Search notes…"
            value={search}
            onChange={(e) => {
              setSearch(e.target.value);
              setPage(1);
            }}
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

      {/* ================= TAG FILTER ================= */}
      {allTags.length > 0 && (
        <div className="flex flex-wrap gap-2 mb-4">
          {allTags.map((tag) => (
            <button
              key={tag}
              onClick={() => setActiveTag(activeTag === tag ? null : tag)}
              className={`
                px-2 py-0.5 rounded-full text-xs font-medium border transition-colors
                ${activeTag === tag
                  ? "bg-blue-600 text-white border-blue-600"
                  : "bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 border-gray-300 dark:border-gray-600 hover:border-blue-400"
                }
              `}
            >
              #{tag}
            </button>
          ))}
        </div>
      )}

      {/* ================= CONTENT ================= */}
      {loading ? (
        <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-6 mt-6">
          {Array.from({ length: pageSize }).map((_, i) => (
            <NoteCardSkeleton key={i} />
          ))}
        </div>
      ) : filteredNotes.length === 0 ? (
        <div className="text-center py-20 space-y-4">
          <div className="text-6xl">📝</div>
          <h2 className="text-xl font-semibold">No notes found</h2>
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
          onTogglePin={togglePin}
        />
      )}

      {/* ================= NOTE MODAL ================= */}
      <Modal open={modalOpen} onClose={() => { setModalOpen(false); setEditingNote(null); }}>
        <NoteEditor
          initialTitle={editingNote?.title}
          initialContent={editingNote?.content}
          initialTags={editingNote?.tags ?? []}
          noteId={editingNote?.id}
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
