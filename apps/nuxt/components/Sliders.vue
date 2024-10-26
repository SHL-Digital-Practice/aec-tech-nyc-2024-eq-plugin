<template>
  <div class="min-h-screen bg-gray-100 py-8 px-4">
    <div class="max-w-4xl mx-auto">
      <div class="flex justify-between items-center mb-8">
        <h1 class="text-3xl font-bold text-gray-800">
          Space Type Target Chart
        </h1>
        <button
          @click="toggleSettings"
          class="bg-gray-200 p-2 rounded-full hover:bg-gray-300 focus:outline-none focus:ring-2 focus:ring-gray-400"
        >
          <CogIcon class="w-6 h-6 text-gray-600" />
        </button>
      </div>
      <div class="bg-white rounded-lg shadow-md overflow-hidden">
        <div v-if="showSettings" class="p-6 border-b border-gray-200">
          <h2 class="text-xl font-semibold mb-4">Settings</h2>
          <div>
            <h3 class="text-lg font-medium mb-2">Notifications</h3>
            <ul class="space-y-2">
              <li
                v-for="notification in notifications"
                :key="notification.id"
                class="flex items-center justify-between bg-gray-50 p-3 rounded"
              >
                <span>{{ notification.message }}</span>
                <button
                  @click="removeNotification(notification.id)"
                  class="text-red-500 hover:text-red-700"
                >
                  <XIcon class="w-5 h-5" />
                </button>
              </li>
            </ul>
          </div>
        </div>
        <div class="p-6">
          <div class="overflow-x-auto">
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
                  <td class="py-2 text-sm text-gray-600">
                    {{ space.current }}
                  </td>
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
                      <svg class="w-full" height="30">
                        <line
                          x1="0"
                          y1="15"
                          x2="100%"
                          y2="15"
                          stroke="#E5E7EB"
                          stroke-width="2"
                        />
                        <circle
                          :cx="`${(space.current / maxValue) * 100}%`"
                          cy="15"
                          r="6"
                          :fill="getMarkerColor(space.current, space.target)"
                        />
                        <line
                          :x1="`${(space.target / maxValue) * 100}%`"
                          :x2="`${(space.target / maxValue) * 100}%`"
                          y1="5"
                          y2="25"
                          stroke="#1E40AF"
                          stroke-width="2"
                        />
                        <g v-if="space.syncedValue">
                          <circle
                            :cx="`${(space.syncedValue / maxValue) * 100}%`"
                            cy="15"
                            r="4"
                            fill="#9CA3AF"
                            opacity="0.5"
                            @mouseenter="showTooltip(space, $event)"
                            @mouseleave="hideTooltip"
                          />
                        </g>
                      </svg>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
      <div class="mt-6 flex justify-center space-x-6">
        <div class="flex items-center">
          <div class="w-4 h-4 bg-blue-500 mr-2"></div>
          <span class="text-sm text-gray-600">Under Target</span>
        </div>
        <div class="flex items-center">
          <div class="w-4 h-4 bg-green-500 mr-2"></div>
          <span class="text-sm text-gray-600">Meeting Target</span>
        </div>
        <div class="flex items-center">
          <div
            class="w-4 h-4 bg-gradient-to-r from-orange-500 to-red-500 mr-2"
          ></div>
          <span class="text-sm text-gray-600">Over Target</span>
        </div>
        <div class="flex items-center">
          <div class="w-4 h-4 bg-blue-800 mr-2"></div>
          <span class="text-sm text-gray-600">Target</span>
        </div>
        <div class="flex items-center">
          <div class="w-4 h-4 bg-gray-400 mr-2 opacity-50"></div>
          <span class="text-sm text-gray-600">Synced Value</span>
        </div>
      </div>
      <button
        @click="addSpace"
        class="mt-8 mx-auto block bg-indigo-600 text-white py-2 px-4 rounded-lg hover:bg-indigo-700 transition duration-300 flex items-center justify-center"
      >
        <PlusCircleIcon class="w-5 h-5 mr-2" />
        Add Space Type
      </button>
      <div
        v-if="tooltipVisible"
        class="absolute bg-gray-800 text-white px-2 py-1 rounded text-xs"
        :style="{ top: `${tooltipY}px`, left: `${tooltipX}px` }"
      >
        Synced by {{ tooltipContent }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from "vue";
// import { PlusCircleIcon, CogIcon, XIcon } from "lucide-vue-next";

const showSettings = ref(false);
const spaces = ref([
  {
    id: 1,
    type: "Type 01",
    current: 25,
    target: 26,
    diff: -1,
    syncedValue: 24,
    syncedBy: "John",
  },
  {
    id: 2,
    type: "Type 02",
    current: 12,
    target: 10,
    diff: 2,
    syncedValue: 11,
    syncedBy: "Alice",
  },
  {
    id: 3,
    type: "Type 03",
    current: 15,
    target: 12,
    diff: 3,
    syncedValue: 13,
    syncedBy: "Bob",
  },
  {
    id: 4,
    type: "Type 04",
    current: 22,
    target: 21,
    diff: 1,
    syncedValue: 20,
    syncedBy: "Emma",
  },
  {
    id: 5,
    type: "Type 05",
    current: 43,
    target: 40,
    diff: 3,
    syncedValue: 41,
    syncedBy: "David",
  },
]);

const notifications = ref([
  { id: 1, message: "Mario has changed the target for Classroom" },
  { id: 2, message: "Gigi has deleted 2 Kitchen" },
  { id: 3, message: "Luigi has added a new space type: Garage" },
]);

const maxValue = computed(
  () =>
    Math.max(
      ...spaces.value.flatMap((space) => [
        space.current,
        space.target,
        space.syncedValue,
      ])
    ) * 1.2
);

const getMarkerColor = (current, target) => {
  const ratio = current / target;
  if (ratio < 1) return "#3B82F6";
  if (ratio === 1) return "#10B981";
  const overRatio = Math.min((ratio - 1) / 0.5, 1);
  return `rgb(${255}, ${Math.round(165 - overRatio * 165)}, 0)`;
};

const updateDiff = (space) => {
  space.diff = space.current - space.target;
  addNotification(`${space.syncedBy} has changed the target for ${space.type}`);
};

const addSpace = () => {
  const newId = Math.max(...spaces.value.map((space) => space.id)) + 1;
  const newTarget = Math.floor(Math.random() * 30) + 10;
  const newCurrent = newTarget + Math.floor(Math.random() * 11) - 5;
  const newSyncedValue = newTarget + Math.floor(Math.random() * 5) - 2;
  const newSpace = {
    id: newId,
    type: `Type ${String(newId).padStart(2, "0")}`,
    current: newCurrent,
    target: newTarget,
    diff: newCurrent - newTarget,
    syncedValue: newSyncedValue,
    syncedBy: ["John", "Alice", "Bob", "Emma", "David"][
      Math.floor(Math.random() * 5)
    ],
  };
  spaces.value.push(newSpace);
  addNotification(
    `${newSpace.syncedBy} has added a new space type: ${newSpace.type}`
  );
};

// Tooltip logic
const tooltipVisible = ref(false);
const tooltipContent = ref("");
const tooltipX = ref(0);
const tooltipY = ref(0);

const showTooltip = (space, event) => {
  tooltipContent.value = space.syncedBy[0];
  tooltipX.value = event.clientX + 10;
  tooltipY.value = event.clientY + 10;
  tooltipVisible.value = true;
};

const hideTooltip = () => {
  tooltipVisible.value = false;
};

const toggleSettings = () => {
  showSettings.value = !showSettings.value;
};

const addNotification = (message) => {
  const newId = Math.max(...notifications.value.map((n) => n.id), 0) + 1;
  notifications.value.unshift({ id: newId, message });
};

const removeNotification = (id) => {
  notifications.value = notifications.value.filter((n) => n.id !== id);
};
</script>
