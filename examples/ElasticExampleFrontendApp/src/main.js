import 'primevue/resources/themes/lara-light-indigo/theme.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import PrimeVue from 'primevue/config'

const app = createApp(App)
app.use(PrimeVue)

app.use(router)

app.mount('#app')
