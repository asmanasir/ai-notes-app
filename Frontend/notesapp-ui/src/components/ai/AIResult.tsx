// src/components/ai/AIResult.tsx
import Button from "../ui/Button";

interface Props {
  result: string | null;
  onApply: () => void;
  onClear: () => void;
}

export default function AIResult({ result, onApply, onClear }: Props) {
  if (!result) return null;

  return (
    <div className="p-4 mb-4 bg-blue-50 dark:bg-gray-700 rounded-lg border border-blue-200 dark:border-gray-600">
      <h3 className="text-lg font-semibold text-blue-700 dark:text-blue-300 mb-2">
        AI Suggestion
      </h3>

      <p className="whitespace-pre-line text-gray-800 dark:text-gray-200 mb-4">
        {result}
      </p>

      <div className="flex gap-2">
        <Button onClick={onApply}>Apply</Button>
        <Button variant="secondary" onClick={onClear}>
          Dismiss
        </Button>
      </div>
    </div>
  );
}
