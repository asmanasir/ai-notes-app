import clsx from "clsx";

interface CardProps {
  children: React.ReactNode;
  className?: string;
}

export default function Card({ children, className }: CardProps) {
  return (
    <div
      className={clsx(
        "bg-white dark:bg-gray-800 rounded-xl shadow-sm hover:shadow-md",
        "border border-gray-200 dark:border-gray-700",
        "transition-all p-5",
        className
      )}
    >
      {children}
    </div>
  );
}
