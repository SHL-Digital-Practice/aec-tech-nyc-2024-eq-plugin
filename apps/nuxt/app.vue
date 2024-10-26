<template>
  <div>
    <Sliders />
  </div>

  <button @click="connectRevitMessages(sessionId)">connect</button>
</template>
<script setup lang="ts">
import { io } from "socket.io-client";
interface IMessageData {
  message: string;
  title?: string;
}

interface IDocumentMessageData extends IMessageData {
  data?: {
    created: string[];
    updated: string[];
    deleted: string[];
  };
}

const sessionId = ref("");

// const webSocket = io("http://localhost:3001/");
// webSocket.on("model:updated", (event) => {
//   const message: IDocumentMessageData = JSON.parse(event);
//   // toast({
//   //   title: message.title ?? "Message",
//   //   description: message.message,
//   // });
//   if (message.data) {
//     const { created, updated, deleted } = message.data;
//     console.log(`[${created.length}] CREATED: `, message.data.created);
//     console.log(`[${updated.length}] UPDATED: `, message.data.updated);
//     console.log(`[${deleted.length}] DELETED: `, message.data.deleted);
//   }
// });

// webSocket.on("connect", () => setConnected(true));
// webSocket.on("disconnect", () => setConnected(false));

// function setConnected(connected: boolean) {
//   if (connected) {
//     console.log("Connected");
//   } else {
//     console.log("Disconnected");
//   }
// }

async function connectRevitMessages(sessionId: string) {
  const messagesBridge = await window.chrome.webview?.hostObjects
    .MessagesBridge;
  if (messagesBridge) {
    const result = await messagesBridge.Connect(sessionId);
    console.log("Connect result: ", result);
  }
}

connectRevitMessages(sessionId.value);
</script>
