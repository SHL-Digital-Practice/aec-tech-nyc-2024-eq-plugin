import { ref, onBeforeUnmount } from "vue";
import { io } from "socket.io-client";

export function useWebSocket(
  url = "http://eq-api-dev-alb-1162581781.us-east-2.elb.amazonaws.com/?userId=1"
) {
  const rooms = ref([]);

  const webSocket = io(url);

  webSocket.on("connect", () => {
    console.log("Connected to WebSocket server");
  });

  webSocket.on("disconnect", () => {
    console.log("Disconnected from WebSocket server");
  });

  webSocket.on("events", (event) => {
    const message = JSON.parse(event);
    if (message.data) {
      const { created, updated, deleted } = message.data;

      created.forEach((room) => rooms.value.push(room));
      updated.forEach((updatedRoom) => {
        const index = rooms.value.findIndex(
          (room) => room.id === updatedRoom.id
        );
        if (index !== -1) {
          rooms.value[index] = updatedRoom;
        }
      });

      deleted.forEach((deletedRoom) => {
        rooms.value = rooms.value.filter((room) => room.id !== deletedRoom.id);
      });
    }
  });

  onBeforeUnmount(() => {
    webSocket.disconnect();
  });

  return { rooms };
}
