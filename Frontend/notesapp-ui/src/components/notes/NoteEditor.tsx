import { useEffect, useRef, useState } from "react";
import { useEditor, EditorContent } from "@tiptap/react";
import StarterKit from "@tiptap/starter-kit";
import Placeholder from "@tiptap/extension-placeholder";
import Input from "../ui/Input";
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
  const [tags, setTags] = useState<string[]>(initialTags);
  const [tagInput, setTagInput] = useState("");

  const [aiResult, setAiResult] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [isAIGenerated, setIsAIGenerated] = useState(false);
  const [autoSaveStatus, setAutoSaveStatus] = useState<"saved" | "saving" | null>(null);

  const titleRef = useRef<HTMLInputElement>(null);
  const autoSaveTimer = useRef<ReturnType<typeof setTimeout> | null>(null);

  const editor = useEditor({
    extensions: [
      StarterKit,
      Placeholder.configure({ placeholder: "Note content…" }),
    ],
    content: initialContent,
    editorProps: {
      attributes: {
        class:
          "prose dark:prose-invert max-w-none min-h-[260px] focus:outline-none p-3 text-sm text-gray-900 dark:text-gray-100",
      },
    },
  });

  const getHtml = () => editor?.getHTML() ?? "";
  const getText = () => editor?.getText() ?? "";

  const isValid = title.trim().length > 0 && getText().trim().length > 0;

  /* Auto-focus title on open */
  useEffect(() => {
    titleRef.current?.focus();
  }, []);

  /* Cmd/Ctrl+Enter to save */
  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if ((e.metaKey || e.ctrlKey) && e.key === "Enter" && isValid && !loading) {
        onSave(title.trim(), getHtml(), tags);
      }
    };
    window.addEventListener("keydown", handler);
    return () => window.removeEventListener("keydown", handler);
  }, [title, tags, isValid, loading, onSave]);

  /* Auto-save for existing notes */
  useEffect(() => {
    if (!noteId || !editor) return;

    const onUpdate = () => {
      if (autoSaveTimer.current) clearTimeout(autoSaveTimer.current);
      autoSaveTimer.current = setTimeout(async () => {
        if (!title.trim() || !getText().trim()) return;
        setAutoSaveStatus("saving");
        try {
          await notesApi.updateNote(noteId, title.trim(), getHtml(), tags);
          setAutoSaveStatus("saved");
        } catch {
          setAutoSaveStatus(null);
        }
      }, 1500);
    };

    editor.on("update", onUpdate);
    return () => {
      editor.off("update", onUpdate);
      if (autoSaveTimer.current) clearTimeout(autoSaveTimer.current);
    };
  }, [noteId, editor, title, tags]);

  /* Auto-save when title or tags change */
  useEffect(() => {
    if (!noteId || !isValid) return;
    if (autoSaveTimer.current) clearTimeout(autoSaveTimer.current);
    autoSaveTimer.current = setTimeout(async () => {
      setAutoSaveStatus("saving");
      try {
        await notesApi.updateNote(noteId, title.trim(), getHtml(), tags);
        setAutoSaveStatus("saved");
      } catch {
        setAutoSaveStatus(null);
      }
    }, 1500);
    return () => { if (autoSaveTimer.current) clearTimeout(autoSaveTimer.current); };
  }, [title, tags, noteId]);

  /* AI handler — tags auto-apply, others show in result panel */
  const handleAI = async (
    callback: () => Promise<{ output: string }>,
    mode: "tags" | "content" | "apply" = "content"
  ) => {
    setLoading(true);
    setAiResult(null);
    try {
      const res = await callback();
      const output = res.output?.trim() ?? "";

      if (mode === "tags") {
        // Parse comma-separated tags and merge into existing tags
        const newTags = output
          .split(",")
          .map((t) => t.trim().toLowerCase().replace(/^#/, ""))
          .filter((t) => t.length > 0 && !tags.includes(t));
        if (newTags.length > 0) setTags((prev) => [...prev, ...newTags]);
      } else if (mode === "apply") {
        // Inject directly into editor (Generate Note)
        editor?.commands.setContent(output);
        setIsAIGenerated(true);
      } else {
        setAiResult(output || "Empty AI response.");
      }
    } catch (err) {
      console.error("AI error:", err);
      setAiResult("AI service error.");
    } finally {
      setLoading(false);
    }
  };

  /* Tag input */
  const handleTagKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter" || e.key === ",") {
      e.preventDefault();
      const tag = tagInput.trim().toLowerCase().replace(/,/g, "");
      if (tag && !tags.includes(tag)) setTags([...tags, tag]);
      setTagInput("");
    }
  };

  const removeTag = (tag: string) => setTags(tags.filter((t) => t !== tag));

  /* Toolbar actions */
  const toolbar = [
    { label: "B", title: "Bold", action: () => editor?.chain().focus().toggleBold().run(), active: () => editor?.isActive("bold") },
    { label: "I", title: "Italic", action: () => editor?.chain().focus().toggleItalic().run(), active: () => editor?.isActive("italic") },
    { label: "S", title: "Strike", action: () => editor?.chain().focus().toggleStrike().run(), active: () => editor?.isActive("strike") },
    { label: "H2", title: "Heading", action: () => editor?.chain().focus().toggleHeading({ level: 2 }).run(), active: () => editor?.isActive("heading", { level: 2 }) },
    { label: "• List", title: "Bullet list", action: () => editor?.chain().focus().toggleBulletList().run(), active: () => editor?.isActive("bulletList") },
    { label: "1. List", title: "Ordered list", action: () => editor?.chain().focus().toggleOrderedList().run(), active: () => editor?.isActive("orderedList") },
  ];

  return (
    <div className="relative z-50 flex flex-col gap-4 p-4 rounded-lg bg-white dark:bg-gray-900 text-gray-900 dark:text-gray-100 max-h-[85vh] overflow-y-auto">

      {/* ===== HEADER ===== */}
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-semibold">Note Editor</h2>
        {autoSaveStatus && (
          <span className="text-xs text-gray-400">
            {autoSaveStatus === "saving" ? "Saving…" : "Saved"}
          </span>
        )}
      </div>

      {/* ===== AI TOOLS ===== */}
      <AIToolsPanel
        loading={loading}
        onSummarize={() => handleAI(() => aiApi.summarize(getText()))}
        onRewrite={() => handleAI(() => aiApi.rewrite(getText()))}
        onImprove={() => handleAI(() => aiApi.improve(getText()))}
        onGenerateTags={() => handleAI(() => aiApi.generateTags(getText()), "tags")}
        onGenerateNote={() => handleAI(() => aiApi.generateNote(title), "apply")}
      />

      {/* ===== DISCLAIMER ===== */}
      <div className="text-xs bg-yellow-50 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-300 border border-yellow-200 dark:border-yellow-800 rounded p-2">
        AI-generated content is for informational purposes only and is not a substitute for professional medical advice, diagnosis, or treatment.
      </div>

      {loading && <div className="text-sm text-blue-500">AI is thinking…</div>}

      {/* ===== AI RESULT ===== */}
      <AIResult
        result={aiResult}
        onApply={() => {
          if (aiResult) {
            editor?.commands.setContent(aiResult);
            setIsAIGenerated(true);
          }
          setAiResult(null);
        }}
        onClear={() => setAiResult(null)}
      />

      {isAIGenerated && (
        <div className="text-xs bg-purple-50 text-purple-700 dark:bg-purple-900/20 dark:text-purple-300 border border-purple-200 dark:border-purple-800 rounded p-2">
          This content was generated using AI
        </div>
      )}

      {/* ===== TITLE ===== */}
      <Input
        ref={titleRef}
        placeholder="Note title..."
        value={title}
        onChange={(e) => setTitle(e.target.value)}
      />

      {/* ===== RICH TEXT EDITOR ===== */}
      <div className="border border-gray-300 dark:border-gray-600 rounded-lg overflow-hidden">
        {/* Toolbar */}
        <div className="flex flex-wrap gap-1 p-2 border-b border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-800">
          {toolbar.map((btn) => (
            <button
              key={btn.label}
              title={btn.title}
              onClick={btn.action}
              className={`px-2 py-1 text-xs rounded font-medium transition-colors
                ${btn.active?.()
                  ? "bg-blue-600 text-white"
                  : "bg-white dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-600 border border-gray-300 dark:border-gray-600"
                }`}
            >
              {btn.label}
            </button>
          ))}
        </div>
        {/* Editor */}
        <div className="bg-white dark:bg-gray-900">
          <EditorContent editor={editor} />
        </div>
      </div>

      {/* Character count */}
      <div className="text-xs text-gray-500 text-right">
        {getText().length} characters
      </div>

      {/* ===== TAGS ===== */}
      <div className="flex flex-col gap-1">
        <div className="flex flex-wrap gap-1">
          {tags.map((tag) => (
            <span
              key={tag}
              className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs bg-blue-100 dark:bg-blue-900/30 text-blue-700 dark:text-blue-300"
            >
              #{tag}
              <button onClick={() => removeTag(tag)} className="hover:text-red-500 leading-none">×</button>
            </span>
          ))}
        </div>
        <input
          type="text"
          placeholder="Add tags (Enter or comma)… or use AI Generate Tags"
          value={tagInput}
          onChange={(e) => setTagInput(e.target.value)}
          onKeyDown={handleTagKeyDown}
          className="text-sm border rounded px-2 py-1 bg-white dark:bg-gray-800 border-gray-300 dark:border-gray-600 text-gray-900 dark:text-gray-100 placeholder:text-gray-400 dark:placeholder:text-gray-500 focus:outline-none focus:ring-1 focus:ring-blue-400"
        />
      </div>

      {!isValid && (
        <p className="text-sm text-red-500">Title and content are required.</p>
      )}

      {/* ===== ACTIONS ===== */}
      <div className="sticky bottom-0 left-0 bg-white dark:bg-gray-900 border-t border-gray-200 dark:border-gray-700 pt-3 mt-2 flex gap-2">
        <Button
          disabled={!isValid || loading}
          onClick={() => onSave(title.trim(), getHtml(), tags)}
        >
          {loading ? "Saving..." : "Save"}
        </Button>
        <Button
          disabled={!isValid}
          variant="secondary"
          onClick={() => exportNoteToPDF(title, getHtml())}
        >
          Export PDF
        </Button>
      </div>
    </div>
  );
}
