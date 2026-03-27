export default function NoteCardSkeleton() {
  return (
    <div className="bg-white dark:bg-gray-800 p-5 rounded-xl shadow border dark:border-gray-700 flex flex-col gap-3 animate-pulse">
      {/* Title */}
      <div className="h-5 bg-gray-200 dark:bg-gray-700 rounded w-3/4" />

      {/* Content lines */}
      <div className="flex flex-col gap-2">
        <div className="h-3 bg-gray-200 dark:bg-gray-700 rounded w-full" />
        <div className="h-3 bg-gray-200 dark:bg-gray-700 rounded w-full" />
        <div className="h-3 bg-gray-200 dark:bg-gray-700 rounded w-2/3" />
      </div>

      {/* Tags */}
      <div className="flex gap-1">
        <div className="h-4 bg-gray-200 dark:bg-gray-700 rounded-full w-12" />
        <div className="h-4 bg-gray-200 dark:bg-gray-700 rounded-full w-16" />
      </div>

      {/* Actions */}
      <div className="flex gap-2 mt-auto">
        <div className="h-7 bg-gray-200 dark:bg-gray-700 rounded w-14" />
        <div className="h-7 bg-gray-200 dark:bg-gray-700 rounded w-16" />
      </div>
    </div>
  );
}
