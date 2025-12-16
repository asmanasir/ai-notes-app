export interface Note {
  id: string;
  title: string;
  content: string;
  pinned?: boolean;
  isAIGenerated?: boolean;

  createdAt: string;
  updatedAt: string; // ✅ ADD THIS
}
