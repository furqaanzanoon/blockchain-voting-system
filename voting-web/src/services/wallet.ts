export const connectWallet =
  async (): Promise<string | null> => {
    try {
      if (!(window as any).ethereum) {
        console.warn(
          "MetaMask is not installed.\nPlease install MetaMask extension."
        );

        return null;
      }

      const accounts =
        await ((window as any).ethereum).request({
          method:
            "eth_requestAccounts",
        });

      if (
        accounts &&
        accounts.length > 0
      ) {
        return accounts[0];
      }

      return null;
    } catch (error) {
      console.error(
        "Wallet connection error:",
        error
      );

      return null;
    }
  };