<template>
  <div class="min-h-screen bg-gray-100 py-8 px-4 relative">
    <h1 class="text-3xl font-bold text-center mb-8 text-gray-800">
      Space Type Target Chart
    </h1>

    <div class="absolute top-4 right-4 flex space-x-2">
      <div v-for="user in connectedUsers" :key="user.id" class="relative">
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

    <div class="max-w-4xl mx-auto bg-white rounded-lg shadow-md">
      <div class="p-6">
        <table class="w-full">
          <thead>
            <tr class="text-left border-b border-gray-200">
              <th class="pb-2 font-semibold text-gray-600">Space Type</th>
              <th class="pb-2 font-semibold text-gray-600">Current</th>
              <th class="pb-2 font-semibold text-gray-600">Target</th>
              <th class="pb-2 font-semibold text-gray-600">Diff</th>
              <th class="pb-2 font-semibold text-gray-600 w-1/2">Range</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(space, index) in spaces"
              :key="space.id"
              :class="{ 'bg-gray-50': index % 2 === 1 }"
            >
              <td class="py-2 text-sm font-medium text-gray-600">
                {{ space.type }}
              </td>
              <td class="py-2 text-sm text-gray-600">{{ space.current }}</td>
              <td class="py-2 text-sm text-gray-600">
                <input
                  type="number"
                  v-model.number="space.target"
                  @input="updateDiff(space)"
                  class="w-16 px-2 py-1 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  :aria-label="`Target for ${space.type}`"
                />
              </td>
              <td class="py-2 text-sm text-gray-600">{{ space.diff }}</td>
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
                      class="absolute z-20 top-3 transform -translate-y-1/2"
                      :style="{ left: `${(space.current / maxValue) * 100}%` }"
                    >
                      <div
                        class="w-3 h-3 rounded-full"
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
        <div class="w-4 h-4 bg-blue-500 mr-2"></div>
        <span class="text-sm text-gray-600">Under Target</span>
      </div>
      <div class="flex items-center">
        <div class="w-4 h-4 bg-blue-800 mr-2"></div>
        <span class="text-sm text-gray-600">Meeting Target</span>
      </div>
      <div class="flex items-center">
        <div
          class="w-4 h-4 bg-gradient-to-r from-orange-500 to-red-500 mr-2"
        ></div>
        <span class="text-sm text-gray-600">Over Target</span>
      </div>
      <div class="flex items-center">
        <div class="w-4 h-4 border-2 border-gray-400 border-dashed mr-2"></div>
        <span class="text-sm text-gray-600">Synced Value</span>
      </div>
    </div>

    <Button
      @click="addSpace"
      class="mt-8 mx-auto block bg-indigo-600 text-white py-2 px-4 rounded-lg hover:bg-indigo-700 transition duration-300 flex items-center justify-center"
    >
      <PlusCircle class="w-5 h-5 mr-2" />
      Add Space Type
    </Button>

    <!-- Tooltip -->
    <div
      v-if="tooltipVisible"
      class="absolute bg-gray-800 text-white px-2 py-1 rounded text-xs"
      :style="{ top: `${tooltipY}px`, left: `${tooltipX}px` }"
    >
      Synced by {{ tooltipContent }}
    </div>

    <!-- Settings Tab -->
    <div class="fixed right-0 top-1/2 transform -translate-y-1/2">
      <button
        @click="toggleSettings"
        class="bg-indigo-600 text-white p-2 rounded-l-lg hover:bg-indigo-700 transition duration-300"
        aria-label="Toggle Settings"
      >
        <Settings class="w-6 h-6" />
      </button>
    </div>

    <Notifications
      class="z-30"
      v-model:settings-open="settingsOpen"
      :users="connectedUsers"
    />
  </div>
</template>

<script setup>
import { ref, computed } from "vue";
import { PlusCircle, Settings } from "lucide-vue-next";

const spaces = ref([
  {
    id: 1,
    type: "Type 01",
    current: 25,
    target: 26,
    diff: -1,
    syncedValue: 24,
    syncedBy: "Gigi Singh",
  },
  {
    id: 2,
    type: "Type 02",
    current: 12,
    target: 10,
    diff: 2,
    syncedValue: 11,
    syncedBy: "Mario Romero",
  },
  {
    id: 3,
    type: "Type 03",
    current: 15,
    target: 12,
    diff: 3,
    syncedValue: 13,
    syncedBy: null,
  },
  {
    id: 4,
    type: "Type 04",
    current: 22,
    target: 21,
    diff: 1,
    syncedValue: 20,
    syncedBy: null,
  },
  {
    id: 5,
    type: "Type 05",
    current: 43,
    target: 40,
    diff: 3,
    syncedValue: 41,
    syncedBy: null,
  },
]);

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
  if (current === target) return "#1E40AF"; // blue for target (same as current)
  const ratio = current / target;
  if (ratio < 1) return "#3B82F6"; // lighter blue for under target
  // Gradient from orange to red for over target
  const overRatio = Math.min((ratio - 1) / 0.5, 1); // Cap at 50% over
  return `rgb(255, ${Math.round(165 - overRatio * 165)}, 0)`;
};

const updateDiff = (space) => {
  space.diff = space.current - space.target;
};

const addSpace = () => {
  const newId = Math.max(...spaces.value.map((space) => space.id)) + 1;
  const newTarget = Math.floor(Math.random() * 30) + 10;
  const newCurrent = newTarget + Math.floor(Math.random() * 11) - 5;
  const newSyncedValue = newTarget + Math.floor(Math.random() * 5) - 2;
  spaces.value.push({
    id: newId,
    type: `Type ${String(newId).padStart(2, "0")}`,
    current: newCurrent,
    target: newTarget,
    diff: newCurrent - newTarget,
    syncedValue: newSyncedValue,
    syncedBy:
      connectedUsers.value[
        Math.floor(Math.random() * connectedUsers.value.length)
      ].name,
  });
};

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
  background: #ddd;
  outline: none;
  opacity: 0.7;
  transition: opacity 0.2s;
}

input[type="range"]::-webkit-slider-thumb {
  -webkit-appearance: none;
  appearance: none;
  width: 3px;
  height: 20px;
  background: #1e40af;
  cursor: pointer;
  border-radius: 25%;
}

input[type="range"]::-moz-range-thumb {
  width: 16px;
  height: 16px;
  background: #1e40af;
  cursor: pointer;
  border-radius: 50%;
}
</style>
