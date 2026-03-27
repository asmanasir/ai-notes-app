import { useEffect, useRef, useState } from "react";
import Input from "../ui/Input";
import Textarea from "../ui/Textarea";
import Button from "../ui/Button";
import AIToolsPanel from "../ai/AIToolsPanel";
import AIResult from "../ai/AIResult";
import { aiApi } from "../../services/ai";
import { exportNoteToPDF } from "../../utils/exportToPdf";
import { notesApi } from "../../services/notesApi";

interface Props {
  initialTitle?: string;
  initialContent?: string;
  initialTags?: string[];
  noteId?: string;
  onSave: (title: string, content: string, tags: string[]) => void;
}

export default function NoteEditor({
  initialTitle = "",
  initialContent = "",
  initialTags = [],
  noteId,
  onSave,
}: Props) {
  const [title, setTitle] = useState(initialTitle);
  const [content, setContent] = useState(initialContent);
  const [tags, setTags] = useState<string[]>(initialTags);
  const [tagInput, setTagInput] = useState("");

  const [aiResult, setAiResult] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [isAIGenerated, setIsAIGenerated] = useState(false);
  const [autoSaveStatus, setAutoSaveStatus] = useState<"saved" | "saving" | null>(null);

  const titleRef = useRef<HTMLInputElement>(null);
  const autoSaveTimer = useRef<ReturnType<typeof setTimeout> | null>(null);

  const maxChars = 2000;
  const isValid = title.trim().length > 0 && content.trim().length > 0;

  /* Auto-focus title on open */
  useEffect(() => {
    titleRef.current?.focus();
  }, []);

  /* Cmd / Ctrl + Enter to save */
  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if ((e.metaKey || e.ctrlKey) && e.key === "Enter" && isValid && !loading) {
        onSave(title.trim(), content.trim(), tags);
      }
    };
    window.addEventListener("keydown", handler);
    return () => window.removeEventListener("keydown", handler);
  }, [title, content, tags, isValid, loading, onSave]);

  /* Auto-save when editing an existing note */
  useEffect(() => {
    if (!noteId || !isValid) return;

    if (autoSaveTimer.current) clearTimeout(autoSaveTimer.current);

    autoSaveTimer.current = setTimeout(async () => {
      setAutoSaveStatus("saving");
      try {
        await notesApi.updateNote(noteId, title.trim(), content.trim(), tags);
        setAutoSaveStatus("saved");
      } catch {
        setAutoSaveStatus(null);
      }
    }, 1500);

    return () => {
      if (autoSaveTimer.current) clearTimeout(autoSaveTimer.current);
    };
  }, [title, content, tags, noteId, isValid]);

  /* AI handler */
  const handleAI = async (callback: () => Promise<{ output: string }>) => {
    setLoading(true);
    setAiResult(null);
    try {
      const res = await callback();
      const output = res.output?.trim();
      setAiResult(output || "Empty AI response.");
    } catch (err) {
      console.error("AI error:", err);
      setAiResult("AI service error.");
    } finally {
      setLoading(false);
    }
  };

  /* Tag input: add on Enter or comma */
  const handleTagKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter" || e.key === ",") {
      e.preventDefault();
      const tag = tagInput.trim().toLowerCase().replace(/,/g, "");
      if (tag && !tags.includes(tag)) {
        setTags([...tags, tag]);
      }
      setTagInput("");
    }
  };

  const removeTag = (tag: string) => setTags(tags.filter((t) => t !== tag));

  return (
    <div
      className="
        relative z-50 flex flex-col gap-4
        p-4 rounded-lg
        bg-white dark:bg-gray-900
        text-gray-900 dark:text-gray-100
        max-h-[85vh] overflow-y-auto
      "
    >
      {/* ================= HEADER ================= */}
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-semibold">Note Editor</h2>
        {autoSaveStatus && (
          <span className="text-xs text-gray-400">
            {autoSaveStatus === "saving" ? "Saving…" : "Saved"}
          </span>
        )}
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
      <div
        className="
          text-xs
          bg-yellow-50 text-yellow-800
          dark:bg-yellow-900/20 dark:text-yellow-300
          border border-yellow-200 dark:border-yellow-800
          rounded p-2
        "
      >
        AI-generated content is for informational purposes only and is not a
        substitute for professional medical advice, diagnosis, or treatment.
      </div>

      {/* ================= AI LOADING ================= */}
      {loading && (
        <div className="text-sm text-blue-500">AI is thinking…</div>
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
        <div
          className="
            text-xs
            bg-purple-50 text-purple-700
            dark:bg-purple-900/20 dark:text-purple-300
            border border-purple-200 dark:border-purple-800
            rounded p-2
          "
        >
          This content was generated using AI
        </div>
      )}

      {/* ================= TITLE ================= */}
      <Input
        ref={titleRef}
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
        className="min-h-[260px]"
      />

      {/* Character Counter */}
      <div className="text-xs text-gray-500 text-right">
        {content.length}/{maxChars} characters
      </div>

      {/* ================= TAGS ================= */}
      <div className="flex flex-col gap-1">
        <div className="flex flex-wrap gap-1">
          {tags.map((tag) => (
            <span
              key={tag}
              className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs bg-blue-100 dark:bg-blue-900/30 text-blue-700 dark:text-blue-300"
            >
              #{tag}
              <button
                onClick={() => removeTag(tag)}
                className="hover:text-red-500 leading-none"
              >
                ×
              </button>
            </span>
          ))}
        </div>
        <input
          type="text"
          placeholder="Add tags (press Enter or comma)…"
          value={tagInput}
          onChange={(e) => setTagInput(e.target.value)}
          onKeyDown={handleTagKeyDown}
          className="
            text-sm border rounded px-2 py-1
            bg-white dark:bg-gray-800
            border-gray-300 dark:border-gray-600
            text-gray-900 dark:text-gray-100
            placeholder:text-gray-400 dark:placeholder:text-gray-500
            focus:outline-none focus:ring-1 focus:ring-blue-400
          "
        />
      </div>

      {/* Validation */}
      {!isValid && (
        <p className="text-sm text-red-500">Title and content are required.</p>
      )}

      {/* ================= STICKY ACTIONS ================= */}
      <div
        className="
          sticky bottom-0 left-0
          bg-white dark:bg-gray-900
          border-t border-gray-200 dark:border-gray-700
          pt-3 mt-2
          flex gap-2
        "
      >
        <Button
          disabled={!isValid || loading}
          onClick={() => onSave(title.trim(), content.trim(), tags)}
        >
          {loading ? "Saving..." : "Save"}
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
