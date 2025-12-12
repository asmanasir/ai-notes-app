import React from "react";

interface ModalProps {
  open: boolean;
  onClose: () => void;
  children: React.ReactNode;
}

export default function Modal({ open, onClose, children }: ModalProps) {
  if (!open) return null;

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
      <div className="bg-white dark:bg-gray-800 rounded-lg shadow-lg p-6 w-full max-w-lg">
        <button
          onClick={onClose}
          className="float-right text-gray-500 hover:text-gray-800"
        >
          ✖
        </button>
        <div className="mt-6">{children}</div>
      </div>
    </div>
  );
}
