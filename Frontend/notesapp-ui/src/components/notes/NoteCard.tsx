import Button from "../ui/Button";
import { exportNoteToPDF } from "../../utils/exportToPdf";
import type { Note } from "../../features/notes/types";

interface Props {
  note: Note;
  onEdit: () => void;
  onDelete: () => void;
  onTogglePin: (id: string) => void;
}

export default function NoteCard({
  note,
  onEdit,
  onDelete,
  onTogglePin,
}: Props) {
  return (
    <div className="bg-white dark:bg-gray-800 p-5 rounded-xl shadow border dark:border-gray-700 flex flex-col gap-3">
      {/* Title + Pin */}
      <div className="flex justify-between items-start">
        <h3 className="font-semibold text-lg truncate text-gray-900 dark:text-gray-100">
          {note.title}
        </h3>

        <button
          onClick={() => onTogglePin(note.id)}
          title={note.pinned ? "Unpin note" : "Pin note"}
          className={`text-xl transition ${
            note.pinned
              ? "text-yellow-400"
              : "text-gray-400 hover:text-yellow-300"
          }`}
        >
          📌
        </button>
      </div>

      {/* Content */}
      <p className="text-sm text-gray-600 dark:text-gray-300 line-clamp-4">
        {note.content}
      </p>

      {/* AI badge */}
      {note.isAIGenerated && (
        <span className="inline-block text-xs text-purple-700 bg-purple-100 dark:bg-purple-900/30 dark:text-purple-300 px-2 py-1 rounded">
          🤖 AI Generated
        </span>
      )}

      {/* Actions */}
      <div className="flex flex-wrap gap-2 mt-auto">
        <Button size="sm" onClick={onEdit}>
          Edit
        </Button>

        <Button size="sm" variant="secondary" onClick={onDelete}>
          Delete
        </Button>

        <Button
          size="sm"
          variant="secondary"
          onClick={() => exportNoteToPDF(note.title, note.content)}
        >
          Export PDF
        </Button>
      </div>
    </div>
  );
}
