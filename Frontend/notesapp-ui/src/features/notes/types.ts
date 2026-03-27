export interface Note {
  id: string;
  title: string;
  content: string;
  tags: string[];
  summary?: string;
  pinned: boolean;
  isAIGenerated?: boolean;
  createdAt: string;
  updatedAt: string;
}
