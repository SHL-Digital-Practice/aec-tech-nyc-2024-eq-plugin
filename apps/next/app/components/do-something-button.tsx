"use client";

import { Button } from "@/components/ui/button";

export default function DoSomethingButton() {
  const doSomething = async () => {
    if (!window.chrome.webview) return;

    const bridge = window.chrome.webview.hostObjects.bridge;

    await bridge.DoSomething(150);
  };

  return <Button onClick={doSomething}>Do something</Button>;
}
