import { ref } from "vue";

export function useAlertQueue() {
  const alerts = ref([]);

  const showAlert = (message) => {
    console.log("SHOW ALERT", message);

    alerts.value.push(message);
    setTimeout(() => {
      alerts.value.shift();
    }, 2000);
  };

  const clearAlert = (index) => {
    alerts.value.splice(index, 1);
  };

  return { alerts, showAlert, clearAlert };
}
