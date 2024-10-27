// composables/useSpaces.js
import { ref, onMounted } from "vue";

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

interface UseSpacesReturn {
  spaces: Ref<Space[]>;
  error: Ref<Error | null>;
}

export function useSpaces(): UseSpacesReturn {
  const spaces = ref<Space[]>([]);
  const error = ref<Error | null>(null);
  const sessionId = "2";

  onMounted(async () => {
    try {
      const previous = await $fetch<ResponseData>(
        `http://eq-api-dev-alb-1162581781.us-east-2.elb.amazonaws.com/elements/latest?sessionId=${sessionId}`
      );
      const live = await $fetch<ResponseData>(
        `http://eq-api-dev-alb-1162581781.us-east-2.elb.amazonaws.com/elements/live`
      );

      spaces.value = mapResponseToSpaces(previous, live);
    } catch (err) {
      error.value = err as Error;
    }
  });

  function mapResponseToSpaces(
    previous: ResponseData,
    live: ResponseData
  ): Space[] {
    const defaultTarget = 10;
    return Object.keys(live.data).map((type, index) => {
      const current = live.data[type].current;
      const syncedValue = previous.data[type].current;
      const target = defaultTarget;
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

  return {
    spaces,
    error,
  };
}
