/**
 * Format a date string or Date object to Indian Standard Time (IST).
 * Output example: "09 Jun 2026, 01:20 PM IST"
 */
export function formatToIST(dateInput: string | Date): string {
  let raw = dateInput;
  // If the API returns a datetime string without timezone info, treat it as UTC
  if (typeof raw === "string" && !/Z|[+-]\d{2}:\d{2}$/.test(raw)) {
    raw = raw + "Z";
  }
  const date = typeof raw === "string" ? new Date(raw) : raw;

  return date.toLocaleString("en-IN", {
    timeZone: "Asia/Kolkata",
    day: "2-digit",
    month: "short",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
    hour12: true,
  }) + " IST";
}
