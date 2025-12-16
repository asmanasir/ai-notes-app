import { Check, X, Bot } from "lucide-react";
import Button from "../ui/Button";

interface Props {
  result: string | null;
  onApply: () => void;
  onClear: () => void;
}

export default function AIResult({ result, onApply, onClear }: Props) {
  if (!result) return null;

  return (
    <div className="border border-purple-200 dark:border-purple-700 rounded-md p-3 bg-purple-50 dark:bg-purple-900/20">
      <div className="flex items-center gap-2 mb-2 text-purple-700 dark:text-purple-300">
        <Bot size={16} />
        <span className="text-sm font-semibold">AI Suggestion</span>
      </div>

      <p className="text-sm mb-3 whitespace-pre-wrap">{result}</p>

      <div className="flex gap-2">
        <Button size="sm" onClick={onApply}>
          <Check size={14} className="mr-1" />
          Apply
        </Button>

        <Button size="sm" variant="secondary" onClick={onClear}>
          <X size={14} className="mr-1" />
          Dismiss
        </Button>
      </div>
    </div>
  );
}
