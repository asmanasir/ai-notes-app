interface Props {
  loading: boolean;
  onSummarize: () => void;
  onRewrite: () => void;
  onImprove: () => void;
  onGenerateTags: () => void;
  onGenerateNote: () => void;
}

export default function AIToolsPanel({
  loading,
  onSummarize,
  onRewrite,
  onImprove,
  onGenerateTags,
  onGenerateNote,
}: Props) {
  return (
    <div className="border rounded-lg p-4 bg-white dark:bg-gray-900 dark:border-gray-700 shadow-sm">
      <h3 className="font-semibold mb-3 text-gray-800 dark:text-gray-100">
        AI Tools
      </h3>

      <div className="grid grid-cols-2 gap-3">
        <button
          onClick={onSummarize}
          disabled={loading}
          className="ai-btn"
        >
          ⭐ Summarize
        </button>

        <button
          onClick={onRewrite}
          disabled={loading}
          className="ai-btn"
        >
          🔄 Rewrite
        </button>

        <button
          onClick={onImprove}
          disabled={loading}
          className="ai-btn"
        >
          ✍ Improve Writing
        </button>

        <button
          onClick={onGenerateTags}
          disabled={loading}
          className="ai-btn"
        >
          🏷 Generate Tags
        </button>

        <button
          onClick={onGenerateNote}
          disabled={loading}
          className="ai-btn col-span-2"
        >
          📝 Generate Note
        </button>
      </div>
    </div>
  );
}
