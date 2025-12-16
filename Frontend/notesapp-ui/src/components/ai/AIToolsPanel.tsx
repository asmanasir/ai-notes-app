import {
  Sparkles,
  Wand2,
  PenLine,
  Tags,
  FileText,
} from "lucide-react";
import Button from "../ui/Button";

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
    <div className="border border-gray-200 dark:border-gray-700 rounded-lg p-3 space-y-3">
      {/* Header */}
      <div className="flex items-center gap-2 text-sm font-medium text-gray-700 dark:text-gray-300">
        <Sparkles size={16} className="text-purple-500" />
        AI Tools
      </div>

      {/* Primary AI action */}
      <Button
        disabled={loading}
        onClick={onGenerateNote}
        className="w-full justify-center gap-2"
      >
        <FileText size={16} />
        Generate Note
      </Button>

      {/* Secondary tools */}
      <div className="grid grid-cols-2 gap-2">
        <Button
          size="sm"
          variant="secondary"
          disabled={loading}
          onClick={onSummarize}
          className="justify-start gap-2"
        >
          <Sparkles size={14} />
          Summarize
        </Button>

        <Button
          size="sm"
          variant="secondary"
          disabled={loading}
          onClick={onRewrite}
          className="justify-start gap-2"
        >
          <Wand2 size={14} />
          Rewrite
        </Button>

        <Button
          size="sm"
          variant="secondary"
          disabled={loading}
          onClick={onImprove}
          className="justify-start gap-2"
        >
          <PenLine size={14} />
          Improve
        </Button>

        <Button
          size="sm"
          variant="secondary"
          disabled={loading}
          onClick={onGenerateTags}
          className="justify-start gap-2"
        >
          <Tags size={14} />
          Tags
        </Button>
      </div>
    </div>
  );
}
