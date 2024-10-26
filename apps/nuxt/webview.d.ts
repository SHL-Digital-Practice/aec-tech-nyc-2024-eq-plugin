declare global {
  interface Window {
    chrome: {
      webview?: {
        hostObjects: {
          MessagesBridge: {
            Connect: (sessionId: string) => Promise<void>;
          };
          StreamsBridge: {
            Start: (sessionId: string) => Promise<void>;
            Cancel: () => Promise<void>;
          };
        };
      };
    };
  }
}

export {};
