// composables/useWebSocket.js
import { ref, onMounted, onUnmounted, watchEffect } from "vue";
import { io, Socket } from "socket.io-client";
import { useAlertQueue } from "@/composables/useAlertQueue"; // Import the alert queue composable

interface Space {
  id: number;
  type: string;
  current: number;
  target: number;
  diff: number;
  syncedValue: number;
  syncedBy: string | null;
}

interface ResponseData {
  data: {
    [key: string]: {
      current: number;
    };
  };
  owner: {
    id: number;
    name: string;
    avatar: string;
  };
}

export function useWebSocket() {
  const spaces = ref<Space[]>([]);
  const error = ref<Error | null>(null);
  const sessionId = "2";
  const participantIds = ref<number[]>([]);
  const participants = ref<
    {
      name: string;
      id: number;
      avatar: string;
    }[]
  >([]);
  let socket: Socket | null = null;

  const { alerts, showAlert } = useAlertQueue(); // Get alerts and showAlert function
  // const url = `http://eq-api-dev-alb-1162581781.us-east-2.elb.amazonaws.com`;
  const url = "https://121f-38-104-71-142.ngrok-free.app";

  async function fetchSpaces() {
    try {
      console.log("fetching spaces");

      const previous = await $fetch<ResponseData>(
        `${url}/elements/latest?sessionId=${sessionId}`
      );
      const live = await $fetch<ResponseData>(`${url}/elements/live`);
      const targets = await $fetch<ResponseData>(`${url}/targets`);

      spaces.value = mapResponseToSpaces(previous, live, targets);
      showAlert("Spaces fetched successfully");
    } catch (err) {
      error.value = err as Error;
      showAlert("Error fetching spaces");
    }
  }

  function initializeSocket() {
    const userId = useRoute().query.userId;

    socket = io(`${url}`, { query: { userId } });

    socket.on("connect", () => {
      showAlert("Connected to server");
    });

    socket.on("disconnect", () => {
      showAlert("Disconnected from server");
    });

    socket.on("participants", (data) => {
      console.log("socket participants");
      participantIds.value = data;
    });

    socket.on("events", () => {
      console.log("event");
      fetchSpaces();
    });
  }

  function mapResponseToSpaces(
    previous: ResponseData,
    live: ResponseData,
    targets: any
  ): Space[] {
    return Object.keys(live.data).map((type, index) => {
      const current = live.data[type].current;
      const syncedValue = previous.data[type].current;
      const target = targets.data[type];
      const diff = current - target;

      return {
        id: index + 1,
        type,
        current,
        target,
        diff,
        syncedValue,
        syncedBy: live.owner.name || null,
      };
    });
  }

  const getParticipants = async () => {
    if (participantIds.value.length === 0) return;

    const query = new URLSearchParams();
    query.set("ids", participantIds.value.join(","));
    const response = await fetch(`${url}/users?${query.toString()}`, {});

    participants.value = (await response.json()) as {
      name: string;
      id: number;
      avatar: string;
    }[];
  };

  const patchTargets = async () => {
    try {
      console.log("patching targets", spaces.value);

      const targets = spaces.value.reduce((acc, space) => {
        acc[space.type] = space.target;
        return acc;
      }, {});
      console.log("Targets updated:", targets);

      const res = await fetch(`${url}/targets`, {
        method: "PATCH",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(targets),
      });

      showAlert("Targets patched successfully"); // Show alert for successful patch
    } catch (err) {
      console.error("Error updating targets:", err);
      error.value = err as Error;
      showAlert("Error patching targets"); // Show alert for patch error
    }
  };

  watchEffect(() => {
    getParticipants();
  });

  // onUnmounted(() => {
  //   if (socket) {
  //     socket.disconnect();
  //   }
  // });

  onMounted(async () => {
    await fetchSpaces();
    initializeSocket();
    getParticipants();
  });

  return { spaces, error, participants, patchTargets, alerts }; // Return alerts for UI display
}
