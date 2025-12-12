export interface Note {
  id: string;
  title: string;
  content: string;

  // ✅ UI-only enhancements
  pinned?: boolean;
  isAIGenerated?: boolean;
}
