import { Buffer } from "buffer";
// @ts-ignore
window.Buffer = window.Buffer || Buffer;

import React from "react";
import ReactDOM from "react-dom/client";
import { createWeb3Modal, defaultConfig } from '@web3modal/ethers/react';
import App from "./App";
import { ToastProvider } from "./context/ToastContext";
import "./index.css";
import { initSession } from "./utils/session";

// 1. Get projectId from env or use the one provided by user
const projectId = import.meta.env.VITE_WALLETCONNECT_PROJECT_ID || '5182f27105bda8bb50065dd51ea4d879';

// 2. Set Sepolia chain details
const sepolia = {
  chainId: 11155111,
  name: 'Sepolia',
  currency: 'ETH',
  explorerUrl: 'https://sepolia.etherscan.io',
  rpcUrl: 'https://ethereum-sepolia-rpc.publicnode.com'
};

// 3. Create metadata
const metadata = {
  name: 'Blockchain Voting System',
  description: 'Decentralized Voting System Web App',
  url: window.location.origin,
  icons: ['https://avatars.githubusercontent.com/u/37784885']
};

// 4. Create Ethers config
const ethersConfig = defaultConfig({
  metadata,
  defaultChainId: 11155111,
  enableEIP6963: true,
  enableInjected: true,
  enableCoinbase: true
});

// 5. Initialize Web3Modal
createWeb3Modal({
  ethersConfig,
  chains: [sepolia],
  projectId,
  enableAnalytics: false,
  themeMode: 'dark',
  themeVariables: {
    '--w3m-accent': '#06b6d4',
  } as any
});

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