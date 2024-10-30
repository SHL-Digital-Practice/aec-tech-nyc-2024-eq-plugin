<template>
  <div class="alert-container">
    <transition-group name="alert" tag="div" class="space-y-2">
      <Alert
        v-for="(alert, index) in alerts"
        :key="index"
        class="alert-item"
        @click="clearAlert(index)"
      >
        {{ alert }}
      </Alert>
    </transition-group>
  </div>
</template>

<script setup lang="ts">
import { useAlertQueue } from "@/composables/useAlertQueue";
const { alerts, showAlert, clearAlert } = useAlertQueue();

// showAlert("This is the first alert!");
</script>

<style scoped>
.alert-container {
  position: absolute;
  bottom: 0;
  right: 0;
  display: flex;
  flex-direction: column-reverse;
  gap: 10px;
  padding: 16px;
}

.alert-enter-active,
.alert-leave-active {
  transition: opacity 0.5s;
}

.alert-enter,
.alert-leave-to {
  opacity: 0;
}
</style>
