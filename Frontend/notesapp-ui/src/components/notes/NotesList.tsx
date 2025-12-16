import type { Note } from "../../features/notes/types";
import NoteCard from "./NoteCard";

interface Props {
  notes: Note[];
  page: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (page: number) => void;
  onEdit: (note: Note) => void;
  onDelete: (id: string) => void;
  onTogglePin: (id: string) => void;
}

export default function NotesList({
  notes,
  page,
  pageSize,
  totalCount,
  onPageChange,
  onEdit,
  onDelete,
  onTogglePin,
}: Props) {
  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <>
      {/* Notes grid */}
      <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-6 mt-6">
        {notes.map((note) => (
          <NoteCard
            key={note.id}
            note={note}
            onEdit={() => onEdit(note)}
            onDelete={() => onDelete(note.id)}
            onTogglePin={() => onTogglePin(note.id)}
          />
        ))}
      </div>

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex items-center justify-center gap-4 mt-10">
          {/* Prev */}
          <button
            disabled={page === 1}
            onClick={() => onPageChange(page - 1)}
            className="
              px-3 py-1 rounded-md border
              text-sm font-medium
              bg-white text-gray-700 border-gray-300
              hover:bg-gray-100
              disabled:opacity-40 disabled:cursor-not-allowed
              dark:bg-gray-800 dark:text-gray-200 dark:border-gray-600
              dark:hover:bg-gray-700
            "
          >
            Prev
          </button>

          {/* Page text */}
          <span className="text-sm font-medium text-gray-700 dark:text-gray-300">
            Page <strong>{page}</strong> of <strong>{totalPages}</strong>
          </span>

          {/* Next */}
          <button
            disabled={page === totalPages}
            onClick={() => onPageChange(page + 1)}
            className="
              px-3 py-1 rounded-md border
              text-sm font-medium
              bg-white text-gray-700 border-gray-300
              hover:bg-gray-100
              disabled:opacity-40 disabled:cursor-not-allowed
              dark:bg-gray-800 dark:text-gray-200 dark:border-gray-600
              dark:hover:bg-gray-700
            "
          >
            Next
          </button>
        </div>
      )}
    </>
  );
}
