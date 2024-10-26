declare global {
  interface Window {
    chrome: {
      webview: {
        hostObjects: {
          bridge: any;
        };
      };
    };
  }
}

export {};
