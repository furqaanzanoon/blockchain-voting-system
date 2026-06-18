import {
  createContext,
  useContext,
  useState,
  useCallback,
  useEffect,
  useRef,
} from "react";

type ToastType =
  | "success"
  | "error"
  | "info"
  | "warning";

interface Toast {
  id: number;
  message: string;
  type: ToastType;
  duration: number;
}

interface ToastContextValue {
  showToast: (
    message: string,
    type?: ToastType,
    duration?: number
  ) => void;
}

const ToastContext =
  createContext<ToastContextValue | null>(
    null
  );

let nextId = 0;

export function ToastProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  const [toasts, setToasts] = useState<
    Toast[]
  >([]);

  const addToast = useCallback(
    (
      message: string,
      type: ToastType = "info",
      duration = 3500
    ) => {
      const id = nextId++;
      setToasts((prev) => [
        ...prev,
        { id, message, type, duration },
      ]);
    },
    []
  );

  const removeToast = useCallback(
    (id: number) => {
      setToasts((prev) =>
        prev.filter((t) => t.id !== id)
      );
    },
    []
  );

  return (
    <ToastContext.Provider
      value={{ showToast: addToast }}
    >
      {children}

      <div
        style={{
          position: "fixed",
          top: "1.5rem",
          right: "1.5rem",
          zIndex: 9999,
          display: "flex",
          flexDirection: "column",
          gap: "0.75rem",
          pointerEvents: "none",
          maxWidth: "420px",
          width: "100%",
        }}
      >
        {toasts.map((toast) => (
          <ToastItem
            key={toast.id}
            toast={toast}
            onClose={() =>
              removeToast(toast.id)
            }
          />
        ))}
      </div>
    </ToastContext.Provider>
  );
}

const iconMap: Record<ToastType, string> = {
  success: "✓",
  error: "✕",
  info: "ℹ",
  warning: "⚠",
};

const colorMap: Record<
  ToastType,
  {
    bg: string;
    border: string;
    icon: string;
    bar: string;
  }
> = {
  success: {
    bg: "rgba(16, 185, 129, 0.12)",
    border: "rgba(16, 185, 129, 0.35)",
    icon: "#10b981",
    bar: "#10b981",
  },
  error: {
    bg: "rgba(239, 68, 68, 0.12)",
    border: "rgba(239, 68, 68, 0.35)",
    icon: "#ef4444",
    bar: "#ef4444",
  },
  info: {
    bg: "rgba(34, 211, 238, 0.12)",
    border: "rgba(34, 211, 238, 0.35)",
    icon: "#22d3ee",
    bar: "#22d3ee",
  },
  warning: {
    bg: "rgba(245, 158, 11, 0.12)",
    border: "rgba(245, 158, 11, 0.35)",
    icon: "#f59e0b",
    bar: "#f59e0b",
  },
};

function ToastItem({
  toast,
  onClose,
}: {
  toast: Toast;
  onClose: () => void;
}) {
  const [visible, setVisible] =
    useState(false);
  const [exiting, setExiting] =
    useState(false);
  const timerRef =
    useRef<ReturnType<typeof setTimeout> | undefined>(undefined);

  useEffect(() => {
    // trigger enter animation
    requestAnimationFrame(() =>
      setVisible(true)
    );

    timerRef.current = setTimeout(() => {
      setExiting(true);
      setTimeout(onClose, 350);
    }, toast.duration);

    return () =>
      clearTimeout(timerRef.current);
  }, [toast.duration, onClose]);

  const colors = colorMap[toast.type];

  return (
    <div
      style={{
        pointerEvents: "auto",
        background: colors.bg,
        backdropFilter: "blur(16px)",
        WebkitBackdropFilter: "blur(16px)",
        border: `1px solid ${colors.border}`,
        borderRadius: "1rem",
        padding: "1rem 1.25rem",
        display: "flex",
        alignItems: "flex-start",
        gap: "0.75rem",
        boxShadow:
          "0 8px 32px rgba(0,0,0,0.4)",
        transform:
          visible && !exiting
            ? "translateX(0)"
            : "translateX(120%)",
        opacity:
          visible && !exiting ? 1 : 0,
        transition:
          "transform 0.35s cubic-bezier(0.16, 1, 0.3, 1), opacity 0.35s ease",
        overflow: "hidden",
        position: "relative",
        cursor: "pointer",
      }}
      onClick={() => {
        clearTimeout(timerRef.current);
        setExiting(true);
        setTimeout(onClose, 350);
      }}
    >
      {/* icon */}
      <div
        style={{
          width: "2rem",
          height: "2rem",
          borderRadius: "0.5rem",
          background: colors.icon,
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          fontSize: "1rem",
          fontWeight: 700,
          color: "#000",
          flexShrink: 0,
        }}
      >
        {iconMap[toast.type]}
      </div>

      {/* message */}
      <div
        style={{
          flex: 1,
          minWidth: 0,
        }}
      >
        <p
          style={{
            margin: 0,
            fontWeight: 600,
            fontSize: "0.875rem",
            textTransform: "capitalize",
            color: colors.icon,
            marginBottom: "0.15rem",
          }}
        >
          {toast.type}
        </p>
        <p
          style={{
            margin: 0,
            fontSize: "0.875rem",
            color: "#e2e8f0",
            lineHeight: 1.5,
            wordBreak: "break-word",
          }}
        >
          {toast.message}
        </p>
      </div>

      {/* close button */}
      <button
        onClick={(e) => {
          e.stopPropagation();
          clearTimeout(timerRef.current);
          setExiting(true);
          setTimeout(onClose, 350);
        }}
        style={{
          background: "none",
          border: "none",
          color: "#94a3b8",
          cursor: "pointer",
          fontSize: "1.1rem",
          padding: "0.15rem",
          lineHeight: 1,
          flexShrink: 0,
        }}
      >
        ✕
      </button>

      {/* progress bar */}
      <div
        style={{
          position: "absolute",
          bottom: 0,
          left: 0,
          height: "3px",
          background: colors.bar,
          borderRadius: "0 0 1rem 1rem",
          animation: `toast-progress ${toast.duration}ms linear forwards`,
        }}
      />
    </div>
  );
}

export function useToast() {
  const ctx = useContext(ToastContext);

  if (!ctx) {
    throw new Error(
      "useToast must be used within a ToastProvider"
    );
  }

  return ctx;
}
