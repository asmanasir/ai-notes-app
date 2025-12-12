import type { Note } from "../../features/notes/types";
import NoteCard from "./NoteCard";

interface Props {
  notes: Note[];
  onEdit: (note: Note) => void;
  onDelete: (id: string) => void;
  onTogglePin: (id: string) => void;
}

export default function NotesList({
  notes,
  onEdit,
  onDelete,
  onTogglePin,
}: Props) {
  return (
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
  );
}
