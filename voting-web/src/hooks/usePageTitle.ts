import { useEffect } from "react";

/**
 * Sets document.title dynamically.
 * Format: "BVS | <pageName>"
 */
export function usePageTitle(pageName: string) {
  useEffect(() => {
    document.title = `BVS | ${pageName}`;
    return () => {
      document.title = "BVS";
    };
  }, [pageName]);
}
