// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  modules: [
    "@nuxtjs/tailwindcss",
    "@vueuse/nuxt",
    "shadcn-nuxt",
    "@nuxthub/core",
    "@nuxtus/nuxt-localtunnel",
  ],
  compatibilityDate: "2024-04-03",
  devtools: { enabled: true },

  devServer: {
    port: 3000,
  },

  //elements/latest
  shadcn: {
    /**
     * Prefix for all the imported component
     */
    prefix: "",
    /**
     * Directory that the component lives in.
     * @default "./components/ui"
     */
    componentDir: "./components/ui",
  },
  localtunnel: {
    // local_https: false,
    // allow_invalid_cert: true,
  },
});
