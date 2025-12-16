import React, { useEffect, useRef } from "react";
import clsx from "clsx";

type Props = React.TextareaHTMLAttributes<HTMLTextAreaElement>;

const Textarea = React.forwardRef<HTMLTextAreaElement, Props>(
  ({ className, value, ...props }, ref) => {
    const innerRef = useRef<HTMLTextAreaElement | null>(null);

    // merge refs
    const setRefs = (el: HTMLTextAreaElement) => {
      innerRef.current = el;
      if (typeof ref === "function") ref(el);
      else if (ref) ref.current = el;
    };

    // auto-grow
    useEffect(() => {
      if (!innerRef.current) return;
      innerRef.current.style.height = "auto";
      innerRef.current.style.height = `${innerRef.current.scrollHeight}px`;
    }, [value]);

    return (
      <textarea
        ref={setRefs}
        value={value}
        className={clsx(
          // Size
          "w-full min-h-[240px] resize-none px-3 py-2",

          // Typography
          "text-sm leading-relaxed",

          // Colors
          "bg-white dark:bg-gray-800",
          "text-gray-900 dark:text-gray-100",
          "placeholder-gray-400 dark:placeholder-gray-500",

          // Borders & focus
          "border border-gray-300 dark:border-gray-700 rounded-md",
          "focus:outline-none focus:ring-2 focus:ring-blue-500",

          className
        )}
        {...props}
      />
    );
  }
);

Textarea.displayName = "Textarea";
export default Textarea;
