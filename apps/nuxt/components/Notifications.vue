<template>
  <!-- Settings Panel -->
  <transition name="slide">
    <div
      v-if="settingsOpen"
      class="fixed right-0 top-0 h-full w-80 bg-white shadow-lg p-6 overflow-y-auto"
    >
      <button
        @click="toggleSettings"
        class="absolute top-4 right-4 text-gray-500 hover:text-gray-700"
        aria-label="Close Settings"
      >
        <X class="w-6 h-6" />
      </button>

      <div class="mb-6">
        <h3 class="text-lg font-semibold mb-2">Notifications</h3>
        <ul class="space-y-2">
          <li
            v-for="notification in notifications"
            :key="notification.id"
            :class="[
              'p-3 rounded-lg',
              notification.read ? 'bg-gray-100' : 'bg-blue-100',
            ]"
          >
            <div class="flex items-center">
              <img
                :src="getUserAvatar(notification.user)"
                :alt="notification.user"
                class="w-8 h-8 rounded-full mr-2"
              />
              <div>
                <p class="text-sm text-gray-800">
                  {{ notification.message }}
                </p>
                <p class="text-xs text-gray-500 mt-1">
                  {{ notification.time }}
                </p>
              </div>
            </div>
          </li>
        </ul>
      </div>
    </div>
  </transition>
</template>

<script lang="ts" setup>
import { X } from "lucide-vue-next";
const settingsOpen = defineModel<boolean>("settingsOpen");
const props = defineProps<{ users: { name: string; avatar: string }[] }>();

const notifications = ref([
  {
    id: 1,
    user: "Mario Romero",
    message: "Mario has changed the target for Classroom",
    time: "2 hours ago",
    read: false,
  },
  {
    id: 2,
    user: "Gigi Singh",
    message: "Gigi has modified Classroom",
    time: "1 day ago",
    read: true,
  },
]);

const getUserAvatar = (name) => {
  const user = props.users.find((u) => u.name === name);
  return user ? user.avatar : "/placeholder.svg?height=200&width=200";
};

const toggleSettings = () => {
  console.log("SETTING TOGGLE");

  settingsOpen.value = !settingsOpen.value;
};
</script>

<style scoped>
.slide-enter-active,
.slide-leave-active {
  transition: transform 0.3s ease;
}

.slide-enter-from,
.slide-leave-to {
  transform: translateX(100%);
}
</style>
