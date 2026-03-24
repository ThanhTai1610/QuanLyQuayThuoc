<template>
  <button type="button" @click="toggleChat" class="home-chatbot-fab">
    <span class="icon-chat"></span>
    <span>Tư vấn nhanh</span>
  </button>

  <section class="home-chatbot-widget" :class="{ 'open': isOpen }">
    <header class="home-chatbot-header">
      <strong>Dược sĩ ảo Pharmative</strong>
      <button type="button" @click="isOpen = false" class="home-chatbot-close">×</button>
    </header>
    <div ref="messageBox" class="home-chatbot-messages">
      <div v-for="(msg, index) in messages" :key="index" :class="['home-chatbot-msg', msg.role]">
        {{ msg.text }}
      </div>
    </div>
    <form @submit.prevent="sendMessage" class="home-chatbot-form">
      <input v-model="userInput" type="text" placeholder="Nhập câu hỏi...">
      <button type="submit">Gửi</button>
    </form>
  </section>
</template>

<script setup>
import { ref, reactive, nextTick } from 'vue';

const isOpen = ref(false);
const userInput = ref('');
const messageBox = ref(null);
const messages = reactive([{ role: 'bot', text: 'Xin chào! Mình có thể hỗ trợ tư vấn nhanh về sản phẩm.' }]);

const toggleChat = () => { isOpen.value = !isOpen.value; };

const sendMessage = () => {
  if (!userInput.value.trim()) return;
  const userText = userInput.value;
  messages.push({ role: 'user', text: userText });
  userInput.value = '';
  
  // Logic phản hồi cũ của bạn
  setTimeout(() => {
    let reply = "Mình đã nhận câu hỏi. Bạn có thể gọi 1800 6928 để được hỗ trợ.";
    if (userText.includes("giao hàng")) reply = "Phí giao hàng nội thành từ 15.000đ...";
    messages.push({ role: 'bot', text: reply });
    nextTick(() => { messageBox.value.scrollTop = messageBox.value.scrollHeight; });
  }, 300);
};
</script>