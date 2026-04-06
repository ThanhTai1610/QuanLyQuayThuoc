<template>
  <div class="chatbot-container">
    <button @click="toggleChat" class="chatbot-fab" :class="{ 'fab-active': isOpen }">
      <span v-if="!isOpen" class="fab-content">
        <i class="fas fa-comments"></i> Tư vấn AI
      </span>
      <span v-else class="fab-content">
        <i class="fas fa-times"></i> Đóng
      </span>
    </button>

    <div class="chatbot-window" :class="{ 'is-open': isOpen }">
      <div class="chatbot-header">
        <div class="header-info">
          <div class="avatar-status">
            <img src="https://cdn-icons-png.flaticon.com/512/387/387561.png" alt="Doctor Avatar">
            <span class="status-dot"></span>
          </div>
          <div class="header-text">
            <p class="name">Dược sĩ Pharmative (AI)</p>
            <p class="status">Sẵn sàng tư vấn 24/7</p>
          </div>
        </div>
      </div>

      <div ref="chatBox" class="chatbot-body">
        <div v-for="(msg, i) in messages" :key="i" :class="['msg-wrapper', msg.role]">
          <div class="msg-bubble shadow-sm">
            {{ msg.text }}
          </div>
          <span class="msg-time">{{ getCurrentTime() }}</span>
        </div>

        <div v-if="isTyping" class="msg-wrapper bot">
          <div class="msg-bubble typing">
            <span></span><span></span><span></span>
          </div>
        </div>
      </div>

      <div class="chatbot-footer">
  <div class="input-wrapper">
    <input 
      v-model="input" 
      @keyup.enter="handleSend" 
      placeholder="Hỏi dược sĩ về thuốc, triệu chứng..."
      type="text"
      :disabled="isTyping"
    >
    <button @click="handleSend" class="send-btn" :disabled="!input.trim() || isTyping">
      <i class="fas fa-paper-plane"></i>
    </button>
  </div>
</div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, nextTick } from 'vue';
import axiosClient from '../api/axiosClient'; 
import bus from '../api/bus'; 

// --- STATE ---
const isOpen = ref(false);
const input = ref('');
const chatBox = ref(null);
const isTyping = ref(false);
const currentProduct = ref(""); 

const messages = ref([
  { role: 'bot', text: 'Chào Tài! Tôi là dược sĩ ảo. Bạn cần tư vấn về cách dùng thuốc hay gặp triệu chứng gì không?' }
]);

// --- LOGIC FUNCTIONS ---

// Tự động cuộn xuống tin nhắn cuối cùng
const scrollToBottom = async () => {
  await nextTick();
  if (chatBox.value) {
    chatBox.value.scrollTop = chatBox.value.scrollHeight;
  }
};

const toggleChat = () => {
  isOpen.value = !isOpen.value;
  if (isOpen.value) scrollToBottom();
};

const getCurrentTime = () => {
  const now = new Date();
  return now.getHours() + ":" + now.getMinutes().toString().padStart(2, '0');
};

const handleSend = async () => {
  if (!input.value.trim() || isTyping.value) return;
  
  const userText = input.value;
  messages.value.push({ role: 'user', text: userText });
  input.value = '';
  isTyping.value = true;
  await scrollToBottom();

  try {
    
    const response = await axiosClient.post('/Chatbot/ask', {
      message: userText,
      tenThuoc: currentProduct.value // Truyền context thuốc đang xem
    });

    messages.value.push({ 
      role: 'bot', 
      text: response.reply || response.data?.reply || 'Dược sĩ đã nhận thông tin.' 
    });

  } catch (error) {
    console.error("Lỗi Chatbot:", error);
    messages.value.push({ 
      role: 'bot', 
      text: 'Kết nối với dược sĩ bị gián đoạn. Tài kiểm tra lại mạng hoặc đăng nhập lại nhé!' 
    });
  } finally {
    isTyping.value = false;
    await scrollToBottom();
  }
};

onMounted(() => {
  bus.on('open-chat', (data) => {
    isOpen.value = true;
    if (data?.tenThuoc) {
      currentProduct.value = data.tenThuoc;
      messages.value.push({ 
        role: 'bot', 
        text: `Chào bạn! Tôi thấy bạn đang quan tâm thuốc "${data.tenThuoc}". Bạn cần tư vấn kỹ hơn về sản phẩm này không?` 
      });
    }
    scrollToBottom();
  });
});

onUnmounted(() => {
  bus.off('open-chat');
});
</script>

