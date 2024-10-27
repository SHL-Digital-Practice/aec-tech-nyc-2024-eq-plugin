import { io } from "socket.io-client";

const participantIds = ref([]);

const participants = ref<
  {
    name: string;
    id: number;
    avatar: string;
  }[]
>([]);

let socket: any = null;

export default function useWebsocket() {
  onMounted(() => {
    const userId = useRoute().query.userId;

    socket = io(
      "http://eq-api-dev-alb-1162581781.us-east-2.elb.amazonaws.com",
      { query: { userId } }
    );

    socket.on("connect", () => {
      console.log("Connected to server");
    });

    socket.on("disconnect", () => {
      console.log("Disconnected from server");
    });

    socket.on("participants", (data) => {
      console.log("Participants", data);
      participantIds.value = data;
    });

    socket.on("events", (data) => {
      console.log("Events", data);
    });
  });

  watchEffect(async () => {
    console.log("Participants", participantIds.value);
    const query = new URLSearchParams();
    query.set("ids", participantIds.value.join(","));
    const response = await fetch(
      `http://eq-api-dev-alb-1162581781.us-east-2.elb.amazonaws.com/users?${query.toString()}`,
      {}
    );

    const data = (await response.json()) as {
      name: string;
      id: number;
      avatar: string;
    }[];

    participants.value = data;
  });

  onUnmounted(() => {
    if (socket) {
      socket.disconnect();
    }
  });

  const updateTarget = () => {
    socket.emit("update-target");
  };

  return { participants, updateTarget };
}
