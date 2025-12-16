export default function ShortcutsModal() {
  return (
    <div className="space-y-4 text-sm">
      <h3 className="text-lg font-semibold">
        Keyboard Shortcuts
      </h3>

      <div className="flex justify-between">
        <span>Save note</span>
        <kbd className="kbd">Ctrl / Cmd + Enter</kbd>
      </div>

      <div className="flex justify-between">
        <span>Search</span>
        <kbd className="kbd">/</kbd>
      </div>

      <div className="flex justify-between">
        <span>Show shortcuts</span>
        <kbd className="kbd">?</kbd>
      </div>

      <div className="flex justify-between">
        <span>Close modal</span>
        <kbd className="kbd">Esc</kbd>
      </div>
    </div>
  );
}
