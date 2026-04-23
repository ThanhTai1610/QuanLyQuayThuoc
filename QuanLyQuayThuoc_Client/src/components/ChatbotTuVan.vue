<template>
  <div class="chatbot-container">
    <button @click="toggleChat" class="chatbot-fab" :class="{ 'fab-active': isOpen }">
      <span v-if="!isOpen" class="fab-content">
        <i class="fas fa-comments"></i> Tư vấn dược
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
            <p class="name">Dược sĩ tư vấn Pharmative</p>
            <p class="status">Tư vấn ngắn gọn, đúng trọng tâm</p>
          </div>
        </div>
      </div>

      <div ref="chatBox" class="chatbot-body">
        <div v-for="(msg, i) in messages" :key="i" :class="['msg-wrapper', msg.role]">
          <div class="msg-bubble shadow-sm">
            {{ msg.text }}
          </div>
          <span class="msg-time">{{ msg.time }}</span>
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
            placeholder="Mô tả triệu chứng, thuốc đang dùng hoặc sản phẩm bạn cần hỏi..."
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

const isOpen = ref(false);
const input = ref('');
const chatBox = ref(null);
const isTyping = ref(false);
const currentProduct = ref('');

function formatTime(date) {
  return `${date.getHours()}:${date.getMinutes().toString().padStart(2, '0')}`;
}

const createMessage = (role, text) => ({
  role,
  text,
  time: formatTime(new Date())
});

const normalizeReply = (text) =>
  String(text || '')
    .replace(/\r/g, '')
    .replace(/\n{3,}/g, '\n\n')
    .trim();

const messages = ref([
  createMessage(
    'bot',
    'Xin chào. Tôi hỗ trợ tư vấn về công dụng, cách dùng, lưu ý an toàn và lựa chọn sản phẩm phù hợp. Bạn hãy nêu triệu chứng, độ tuổi và thuốc đang dùng nếu có.'
  )
]);

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

const handleSend = async () => {
  if (!input.value.trim() || isTyping.value) return;

  const userText = input.value.trim();
  messages.value.push(createMessage('user', userText));
  input.value = '';
  isTyping.value = true;
  await scrollToBottom();

  try {
    const response = await axiosClient.post('/Chatbot/ask', {
      message: userText,
      tenThuoc: currentProduct.value
    });

    const reply =
      response.reply ||
      response.data?.reply ||
      'Tôi đã ghi nhận thông tin. Bạn có thể cho biết thêm triệu chứng chính hoặc tên sản phẩm cần hỏi để tôi tư vấn sát hơn.';

    messages.value.push(createMessage('bot', normalizeReply(reply)));
  } catch (error) {
    console.error('Lỗi Chatbot:', error);
    messages.value.push(
      createMessage(
        'bot',
        'Kết nối tư vấn đang gián đoạn. Bạn vui lòng thử lại sau ít phút. Nếu đang có triệu chứng nặng hoặc bất thường, hãy liên hệ cơ sở y tế gần nhất.'
      )
    );
  } finally {
    isTyping.value = false;
    await scrollToBottom();
  }
};

onMounted(() => {
  bus.on('open-chat', (data) => {
    isOpen.value = true;

    if (data?.tenThuoc) {
      const isNewProduct = currentProduct.value !== data.tenThuoc;
      currentProduct.value = data.tenThuoc;

      if (isNewProduct) {
        messages.value.push(
          createMessage(
            'bot',
            `Tôi thấy bạn đang xem sản phẩm "${data.tenThuoc}". Bạn có thể hỏi về công dụng, cách dùng, đối tượng sử dụng, lưu ý an toàn hoặc sản phẩm phù hợp với triệu chứng hiện tại.`
          )
        );
      }
    }

    scrollToBottom();
  });
});

onUnmounted(() => {
  bus.off('open-chat');
});
</script>
