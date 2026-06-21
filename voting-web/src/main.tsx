import { Buffer } from "buffer";
// @ts-ignore
window.Buffer = window.Buffer || Buffer;

import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import { ToastProvider } from "./context/ToastContext";
import "./index.css";
import { initSession } from "./utils/session";

// Initialize session state: clears auth details if browser was closed
initSession();

ReactDOM.createRoot(
  document.getElementById("root")!
).render(
  <React.StrictMode>
    <ToastProvider>
      <App />
    </ToastProvider>
  </React.StrictMode>
);