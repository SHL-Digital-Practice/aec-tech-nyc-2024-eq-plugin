"use client";

import { useEffect } from "react";

export default function BridgeProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  useEffect(() => {
    const main = async () => {
      if (!window.chrome.webview) return;

      // Get the bridge.
      const bridge = window.chrome.webview.hostObjects.bridge;

      // Call the bridge with no argument.
      console.log(await bridge.GetGreeting());

      // Call the bridge with value argument.
      console.log(await bridge.GetMessage("testing..."));

      // Call the bridge with object argument.
      const person = await bridge.CreatePerson("John Doe", 42);
      console.log("person", person);

      // Access an index property
      bridge[123] = "Test 123";
      console.log("Index property:", await bridge[123]);
    };
    main();
  }, []);

  return <>{children}</>;
}
