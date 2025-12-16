import type { ButtonHTMLAttributes, ReactNode } from "react";
import clsx from "clsx";

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: "primary" | "secondary";
  size?: "sm" | "md";
  icon?: ReactNode;
  tooltip?: string;
}

export default function Button({
  children,
  variant = "primary",
  size = "md",
  icon,
  tooltip,
  className,
  ...props
}: ButtonProps) {
  return (
    <button
      {...props}
      title={tooltip}
      className={clsx(
        "inline-flex items-center gap-2 rounded font-medium transition",
        "focus:outline-none focus:ring-2 focus:ring-blue-500",
        "disabled:opacity-50 disabled:cursor-not-allowed",

        // Size
        size === "sm" && "px-2 py-1 text-sm",
        size === "md" && "px-4 py-2",

        // Variant
        variant === "primary" &&
          "bg-blue-600 text-white hover:bg-blue-700",
        variant === "secondary" &&
          "bg-gray-200 text-gray-800 hover:bg-gray-300 dark:bg-gray-700 dark:text-gray-200",

        className
      )}
    >
      {icon && <span className="flex-shrink-0">{icon}</span>}
      {children}
    </button>
  );
}
