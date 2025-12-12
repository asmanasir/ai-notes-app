import { useState } from "react";
import Input from "../ui/Input";
import Textarea from "../ui/Textarea";
import Button from "../ui/Button";
import AIToolsPanel from "../ai/AIToolsPanel";
import AIResult from "../ai/AIResult";
import { aiApi } from "../../services/ai";
import { exportNoteToPDF } from "../../utils/exportToPdf";

interface Props {
  initialTitle?: string;
  initialContent?: string;
  onSave: (title: string, content: string) => void;
}

export default function NoteEditor({
  initialTitle = "",
  initialContent = "",
  onSave,
}: Props) {
  const [title, setTitle] = useState(initialTitle);
  const [content, setContent] = useState(initialContent);

  const [aiResult, setAiResult] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [isAIGenerated, setIsAIGenerated] = useState(false);

  const maxChars = 2000;
  const isValid =
    title.trim().length > 0 && content.trim().length > 0;

  // =========================
  // AI handler
  // =========================
  const handleAI = async (
    callback: () => Promise<{ output: string }>
  ) => {
    setLoading(true);
    setAiResult(null);

    try {
      const res = await callback();
      const output = res.output?.trim();

      if (!output) {
        setAiResult("⚠ Empty AI response.");
      } else {
        setAiResult(output);
      }
    } catch (err) {
      console.error("AI error:", err);
      setAiResult("⚠ AI service error.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="relative z-50 flex flex-col gap-4 p-4 rounded
                    bg-white dark:bg-gray-900
                    text-gray-900 dark:text-gray-100">

      {/* ================= HEADER ================= */}
      <div className="flex justify-between items-center">
        <h2 className="text-lg font-semibold">Note Editor</h2>
      </div>

      {/* ================= AI TOOLS ================= */}
      <AIToolsPanel
        loading={loading}
        onSummarize={() => handleAI(() => aiApi.summarize(content))}
        onRewrite={() => handleAI(() => aiApi.rewrite(content))}
        onImprove={() => handleAI(() => aiApi.improve(content))}
        onGenerateTags={() => handleAI(() => aiApi.generateTags(content))}
        onGenerateNote={() => handleAI(() => aiApi.generateNote(title))}
      />

      {/* ================= DISCLAIMER ================= */}
      <div className="text-xs text-yellow-700 bg-yellow-50 dark:bg-yellow-900/20
                      border border-yellow-200 dark:border-yellow-800
                      rounded p-2">
        ⚠ AI-generated content is for informational purposes only and is not a
        substitute for professional medical advice, diagnosis, or treatment.
      </div>

      {/* ================= AI LOADING ================= */}
      {loading && (
        <div className="text-sm text-blue-500">
          🤖 AI is thinking...
        </div>
      )}

      {/* ================= AI RESULT ================= */}
      <AIResult
        result={aiResult}
        onApply={() => {
          if (aiResult) {
            setContent(aiResult);
            setIsAIGenerated(true);
          }
          setAiResult(null);
        }}
        onClear={() => setAiResult(null)}
      />

      {/* ================= AI BADGE ================= */}
      {isAIGenerated && (
        <div className="text-xs text-purple-600 bg-purple-50 dark:bg-purple-900/20
                        border border-purple-200 dark:border-purple-800
                        rounded p-2">
          🤖 This content was generated using AI
        </div>
      )}

      {/* ================= TITLE ================= */}
      <Input
        placeholder="Note title..."
        value={title}
        onChange={(e) => setTitle(e.target.value)}
      />

      {/* ================= CONTENT ================= */}
      <Textarea
        placeholder="Note content..."
        value={content}
        maxLength={maxChars}
        onChange={(e) => setContent(e.target.value)}
      />

      {/* Character Counter */}
      <div className="text-xs text-gray-500 text-right">
        {content.length}/{maxChars} characters
      </div>

      {/* Validation */}
      {!isValid && (
        <p className="text-sm text-red-500">
          ⚠ Title and content are required.
        </p>
      )}

      {/* ================= ACTIONS ================= */}
      <div className="flex gap-2">
        <Button
          disabled={!isValid}
          onClick={() => onSave(title.trim(), content.trim())}
        >
          Save
        </Button>

        <Button
          disabled={!isValid}
          variant="secondary"
          onClick={() => exportNoteToPDF(title, content)}
        >
          Export PDF
        </Button>
      </div>
    </div>
  );
}
