export const normalizeStatus = (status: string | number): string => {
  if (typeof status === "number") {
    return (
      ["Draft", "Active", "Closed"][status] ?? "Unknown"
    );
  }

  return status;
};
