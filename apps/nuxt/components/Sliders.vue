<template>
  <div class="min-h-screen bg-neutral-900 py-8 px-4 relative">
    <div class="flex items-center justify-between px-10">
      <div class="flex items-center space-x-2">
        <img :src="'equalizer_logo.svg'" class="w-8 h-8 animate-pulse" />
        <h1
          class="text-3xl tracking-widest font-bold text-center text-neutral-50"
        >
          EQUALIZER
        </h1>
      </div>

      <Alerts />

      <div class="flex space-x-2">
        <div v-for="user in participants" :key="user.id" class="relative">
          <div class="relative w-10 h-10">
            <img
              :src="user.avatar"
              :alt="user.name"
              class="w-10 h-10 rounded-full border-2 border-white object-cover"
            />

            <span
              class="absolute bottom-0 right-0 w-3 h-3 bg-green-500 border-2 border-white rounded-full"
            ></span>
          </div>
        </div>
      </div>
    </div>

    <div class="max-w-4xl mx-auto bg-neutral-900 rounded-lg shadow-md">
      <div class="p-6">
        <table class="w-full">
          <thead>
            <tr class="text-left border-b border-neutral-200">
              <th class="pb-2 font-semibold text-white">Space Type</th>
              <th class="pb-2 font-semibold text-white">Current</th>
              <th class="pb-2 font-semibold text-white">Target</th>
              <th class="pb-2 font-semibold text-white">Diff</th>
              <th class="pb-2 font-semibold text-white w-1/2">Range</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(space, index) in spaces"
              :key="space.id"
              :class="{ 'bg-neutral-800': index % 2 === 1 }"
            >
              <td class="py-2 px-2 text-sm font-medium text-white">
                {{ space.type }}
              </td>
              <td class="py-2 text-sm text-white">{{ space.current }}</td>
              <td class="py-2 text-sm text-white">
                <input
                  type="number"
                  v-model.number="space.target"
                  @input="updateDiff(space)"
                  class="w-16 px-2 py-1 border bg-transparent border-neutral-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  :aria-label="`Target for ${space.type}`"
                />
              </td>
              <td
                class="py-2 text-md font-semibold"
                :style="{
                  color: getMarkerColor(space.current, space.target),
                }"
              >
                {{ space.diff > 0 ? "+" : "" }}{{ space.diff }}
              </td>
              <td class="py-2">
                <div class="relative">
                  <input
                    type="range"
                    v-model.number="space.target"
                    :min="0"
                    :max="maxValue"
                    step="1"
                    @input="updateDiff(space)"
                    :aria-label="`Slider for ${space.type} target`"
                  />
                  <!-- Overlay container -->
                  <div class="absolute inset-1 pointer-events-none">
                    <!-- Current value marker -->
                    <div
                      class="absolute z-20 top-3 -ml-2 transform -translate-y-1/2"
                      :style="{ left: `${(space.current / maxValue) * 100}%` }"
                    >
                      <div
                        class="w-4 h-4 rounded-full border-2 border-white"
                        :style="{
                          backgroundColor: getMarkerColor(
                            space.current,
                            space.target
                          ),
                        }"
                      ></div>
                    </div>
                    <!-- Synced value avatar -->
                    <div
                      v-if="space.syncedValue && space.syncedBy"
                      class="absolute top-0 transform -translate-x-1/2"
                      :style="{
                        left: `${(space.syncedValue / maxValue) * 100}%`,
                      }"
                    >
                      <img
                        :src="getUserAvatar(space.syncedBy)"
                        class="w-6 h-6 rounded-full shadow-lg"
                        @mouseenter="showTooltip(space, $event)"
                        @mouseleave="hideTooltip"
                      />
                      <!-- Optional circle border around the avatar -->
                      <div
                        class="absolute inset-0 rounded-full border-2 border-white"
                      ></div>
                    </div>
                  </div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div class="mt-6 max-w-4xl mx-auto flex justify-center space-x-6">
      <div class="flex items-center">
        <div
          class="w-10 h-4 rounded-full mr-2 border-2 border-white"
          style="background: linear-gradient(to right, #4835a2, #a7d383)"
        ></div>
        <span class="text-sm text-neutral-100">Under Target</span>
      </div>
      <div class="flex items-center">
        <div
          class="w-4 h-4 rounded-full mr-2 border-2 border-white"
          style="background: linear-gradient(to right, #4ade80, #4ade80)"
        ></div>
        <span class="text-sm text-neutral-100">Meeting Target</span>
      </div>
      <div class="flex items-center">
        <div
          class="w-10 h-4 rounded-full mr-2 border-2 border-white"
          style="background: linear-gradient(to right, #a7d383, #e53d26)"
        ></div>
        <span class="text-sm text-neutral-100">Over Target</span>
      </div>

      <div class="flex items-center">
        <div class="w-4 h-4 rounded-full border-2 border-white mr-2"></div>
        <span class="text-sm text-neutral-100">Latest</span>
      </div>
    </div>

    <Button
      @click="addSpace"
      class="mt-8 mx-auto text-white py-2 px-4 rounded-lg hover:bg-green-700 transition duration-300 flex items-center justify-center"
    >
      <PlusCircle class="w-5 h-5 mr-2" />
      Add Space Type
    </Button>

    <!-- Tooltip -->
    <div
      v-if="tooltipVisible"
      class="absolute bg-neutral-800 text-white px-2 py-1 rounded text-xs"
      :style="{ top: `${tooltipY}px`, left: `${tooltipX}px` }"
    >
      Synced by {{ tooltipContent }}
    </div>

    <!-- Settings Tab -->
    <div class="fixed right-0 top-14 transform -translate-y-1/2">
      <button
        @click="toggleSettings"
        class="bg-green-600 text-white p-2 rounded-l-lg hover:bg-green-700 transition duration-300"
        aria-label="Toggle Settings"
      >
        <Bell class="w-6 h-6" />
      </button>
    </div>

    <Notifications
      class="z-30"
      v-model:settings-open="settingsOpen"
      :users="participants"
    />
  </div>
</template>

<script setup>
import { ref, computed } from "vue";
import { PlusCircle, Bell } from "lucide-vue-next";

const { participants, patchTargets, spaces } = useWebSocket();

const previousTargets = ref(spaces.value.map((space) => space.target));

const debouncedFn = useDebounceFn(() => {
  const currentTargets = spaces.value.map((space) => space.target);

  const hasTargetsChanged = currentTargets.some((target, index) => {
    return target !== previousTargets.value[index];
  });

  if (hasTargetsChanged) {
    console.log("DEBOUNCED - Targets have changed");
    previousTargets.value = [...currentTargets];
    patchTargets();
  }
}, 200);

watch(
  spaces,
  () => {
    debouncedFn();
  },
  {
    deep: true,
  }
);

const connectedUsers = ref([
  {
    id: 1,
    name: "Gigi Singh",
    avatar:
      "https://hebbkx1anhila5yf.public.blob.vercel-storage.com/Gigi%20Singh-D76O99dV1sh3RPutCZZkL2aDxtBLfk.jfif",
  },
  {
    id: 2,
    name: "Mario Romero",
    avatar:
      "https://hebbkx1anhila5yf.public.blob.vercel-storage.com/Mario%20Romero-DqlVKba19kVahwvRSu3ds6cTSpPgdW.jfif",
  },
]);

onMounted(() => {
  //fegthit hte latest here
  // http://eq-api-dev-alb-1162581781.us-east-2.elb.amazonaws.com/elements/latest?sessionId=1
});

const tooltipVisible = ref(false);
const tooltipContent = ref("");
const tooltipX = ref(0);
const tooltipY = ref(0);

const settingsOpen = ref(false);

const maxValue = computed(() => {
  return (
    Math.max(
      ...spaces.value.flatMap((space) => [
        space.current,
        space.target,
        space.syncedValue,
      ])
    ) * 1.2
  );
});

const getMarkerColor = (current, target) => {
  if (current === target) return "#4ade80";

  const ratio = current / target;

  if (ratio < 0.5) {
    // Under -50%
    return "#4835A2";
  } else if (ratio < 1) {
    // Between -50% and 100% below target
    const factor = (ratio - 0.5) / 0.5; // normalize between 0 and 1
    return interpolateColor("#4835A2", "#A7D383", factor);
  } else if (ratio < 1.5) {
    // 100% to 50% above target
    const factor = (ratio - 1) / 0.5; // normalize between 0 and 1
    return interpolateColor("#A7D383", "#E53D26", factor);
  }

  // 50%+ above target
  return "#E53D26";
};

const interpolateColor = (startColor, endColor, factor) => {
  const [r1, g1, b1] = hexToRgb(startColor);
  const [r2, g2, b2] = hexToRgb(endColor);

  const r = Math.round(r1 + (r2 - r1) * factor);
  const g = Math.round(g1 + (g2 - g1) * factor);
  const b = Math.round(b1 + (b2 - b1) * factor);

  return `rgb(${r}, ${g}, ${b})`;
};

const hexToRgb = (hex) => {
  const bigint = parseInt(hex.slice(1), 16);
  return [(bigint >> 16) & 255, (bigint >> 8) & 255, bigint & 255];
};

const updateDiff = (space) => {
  const diff = space.current - space.target;

  space.diff = diff;
};

const addSpace = () => {};

const showTooltip = (space, event) => {
  if (space.syncedBy) {
    tooltipContent.value = space.syncedBy;
    tooltipX.value = event.clientX + 10;
    tooltipY.value = event.clientY + 10;
    tooltipVisible.value = true;
  }
};

const hideTooltip = () => {
  tooltipVisible.value = false;
};

const toggleSettings = () => {
  settingsOpen.value = !settingsOpen.value;
};

const getUserAvatar = (name) => {
  const user = connectedUsers.value.find((u) => u.name === name);
  return user ? user.avatar : "/placeholder.svg?height=200&width=200";
};
</script>

<style scoped>
input[type="range"] {
  -webkit-appearance: none;
  width: 100%;
  height: 2px;
  background: white;
  outline: none;
  opacity: 0.7;
  transition: opacity 0.2s;
}

input[type="range"]::-webkit-slider-thumb {
  -webkit-appearance: none;
  appearance: none;
  width: 3px !important;
  height: 20px;
  background: white;
  cursor: pointer;
  border-radius: 25%;
}

input[type="range"]::-moz-range-thumb {
  width: 16px;
  height: 16px;
  background: white;
  cursor: pointer;
  border-radius: 50%;
}
</style>
