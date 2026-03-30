import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import $ from 'jquery'
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';

// Cấu hình jQuery toàn cục
window.jQuery = window.$ = $
if ($ && !$.camelCase) {
    $.camelCase = (str) => str.replace(/-([a-z])/g, (g) => g[1].toUpperCase());
}
if ($ && !$.type) {
    $.type = (obj) => ({}).toString.call(obj).slice(8, -1).toLowerCase();
}

// Import CSS (Thứ tự quan trọng: Bootstrap trước, Custom sau)
import './assets/fonts/icomoon/style.css'
import './assets/fonts/flaticon/font/flaticon.css'
import './assets/css/bootstrap.min.css'
import './assets/css/account-profile.css';
import './assets/css/style.css'
import './assets/css/home-page.css'
import './assets/css/theme-green.css'
import './assets/css_admin/sb-admin-2.min.css'
import './assets/css_admin/sidebar.css'
import './assets/vendor/fontawesome-free/css/all.min.css'


const app = createApp(App)
app.use(router)
app.mount('#app')