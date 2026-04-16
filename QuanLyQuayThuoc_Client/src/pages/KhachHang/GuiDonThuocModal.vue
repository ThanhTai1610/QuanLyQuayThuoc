<template>
  <Teleport to="body">
    <div v-if="isOpen" class="modal-overlay">
      <div class="modal-content-custom">
        <div class="modal-header-custom">
          <h5>Gửi đơn thuốc tư vấn</h5>
          <button class="close-btn" @click="closeModal">&times;</button>
        </div>

        <div class="modal-body-custom">
          <div class="section-card">
            <h6>Thông tin liên hệ</h6>
            <div class="row">
              <div class="col-md-7">
                <label>Họ và tên <span class="text-danger">*</span></label>
                <input type="text" v-model="formData.hoTen" placeholder="Nhập họ và tên" class="form-control-custom" />
              </div>
              <div class="col-md-5">
                <label>Số điện thoại <span class="text-danger">*</span></label>
                <input type="text" v-model="formData.soDienThoai" placeholder="Nhập số điện thoại" class="form-control-custom" />
              </div>
            </div>
            <div class="mt-2">
              <label>Ghi chú (không bắt buộc)</label>
              <textarea v-model="formData.ghiChu" placeholder="Ví dụ: Tôi cần tư vấn cách dùng thuốc này..." class="form-control-custom"></textarea>
            </div>
          </div>

          <div class="section-card mt-3">
            <h6>Ảnh chụp đơn thuốc <span class="text-danger">*</span></h6>
            <div class="upload-box" @click="triggerFileInput">
              <i class="fas fa-camera mb-2"></i>
              <span>Bấm để tải ảnh đơn thuốc lên</span>
              <input type="file" ref="fileInput" multiple hidden @change="handleFileChange" accept="image/*" />
            </div>
            <div class="preview-list mt-2" v-if="previews.length > 0">
              <div v-for="(img, idx) in previews" :key="idx" class="preview-item">
                <img :src="img" />
                <button @click="removeImage(idx)">&times;</button>
              </div>
            </div>
          </div>

          <div class="section-card mt-3">
            <h6>Sản phẩm cần tư vấn</h6>
            <div class="product-item-mini mt-2" v-if="product">
              <img :src="getImageUrl(product.hinhAnhChinh)" alt="product" />
              <div class="product-info">
                <div class="name font-weight-bold">{{ product.tenThuoc }}</div>
                <div class="qty-control-mini">
                  <button @click="soLuong > 1 ? soLuong-- : null">-</button>
                  <span>{{ soLuong }}</span>
                  <button @click="soLuong++">+</button>
                  <span class="unit-tag">Đơn vị</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer-custom">
          <button class="btn-submit-full" :disabled="loading" @click="submitOrder">
            {{ loading ? 'Đang gửi yêu cầu...' : 'Gửi yêu cầu tư vấn' }}
          </button>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import { ref, reactive } from 'vue';
import Swal from 'sweetalert2';
import axiosClient from '../../api/axiosClient'; // Đảm bảo đúng path tới axiosClient của bạn

const props = defineProps({
  isOpen: Boolean,
  product: Object
});
const emit = defineEmits(['close']);

const fileInput = ref(null);
const previews = ref([]);
const files = ref([]);
const soLuong = ref(1);
const loading = ref(false);

// Để trống theo yêu cầu của Tài
const formData = reactive({
  hoTen: '',
  soDienThoai: '',
  ghiChu: ''
});

const closeModal = () => {
  // Reset form khi đóng
  formData.hoTen = '';
  formData.soDienThoai = '';
  formData.ghiChu = '';
  previews.value = [];
  files.value = [];
  emit('close');
};

const triggerFileInput = () => fileInput.value.click();

const handleFileChange = (e) => {
  const selectedFiles = Array.from(e.target.files);
  selectedFiles.forEach(file => {
    files.value.push(file);
    previews.value.push(URL.createObjectURL(file));
  });
};

const removeImage = (idx) => {
  previews.value.splice(idx, 1);
  files.value.splice(idx, 1);
};

const getImageUrl = (path) => {
  if (!path) return 'https://via.placeholder.com/400x400.png?text=Duoc+Pham';
  if (path.startsWith('http')) return path;
  return `${import.meta.env.VITE_API_URL.replace('/api', '')}${path.startsWith('/') ? '' : '/'}${path}`;
};

const submitOrder = async () => {
  // Validation cơ bản
  if (!formData.hoTen || !formData.soDienThoai) {
    return Swal.fire('Lỗi', 'Vui lòng nhập Họ tên và Số điện thoại nhé!', 'error');
  }
  if (files.value.length === 0) {
    return Swal.fire('Thông báo', 'Bạn cần tải ảnh đơn thuốc lên để dược sĩ kiểm tra nhé!', 'warning');
  }

  loading.value = true;
  try {
    // Sử dụng FormData để gửi được cả File ảnh
    const sendData = new FormData();
    sendData.append('HoTen', formData.hoTen);
    sendData.append('SoDienThoai', formData.soDienThoai);
    sendData.append('GhiChu', formData.ghiChu);
    sendData.append('TenThuoc', props.product.tenThuoc);
    sendData.append('SoLuong', soLuong.value);
    
    files.value.forEach(file => {
      sendData.append('Files', file);
    });

    // Gọi tới API Backend (Tài sẽ phải tạo endpoint này)
    await axiosClient.post('/LienHe/GuiDonThuoc', sendData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });

    Swal.fire('Thành công', 'Yêu cầu đã được gửi. Dược sĩ sẽ sớm liên hệ với bạn qua SĐT!', 'success');
    closeModal();
  } catch (error) {
    console.error(error);
    Swal.fire('Lỗi', 'Không thể gửi yêu cầu lúc này. Tài kiểm tra lại Backend nhé!', 'error');
  } finally {
    loading.value = false;
  }
};
</script>



<style scoped>
.modal-overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); z-index: 9999; display: flex; justify-content: center; align-items: center; }

.modal-content-custom { width: 600px; background: #f0f2f5; border-radius: 12px; overflow: hidden; max-height: 90vh; overflow-y: auto; }

.modal-header-custom { padding: 15px; background: white; border-bottom: 1px solid #ddd; display: flex; justify-content: space-between; }

.section-card { background: white; padding: 15px; border-radius: 8px; }

.form-control-custom { width: 100%; border: 1px solid #ddd; border-radius: 6px; padding: 8px; margin-top: 5px; }

.upload-box { border: 2px dashed #007bff; border-radius: 8px; padding: 20px; text-align: center; cursor: pointer; color: #666; }

.preview-list { display: flex; gap: 10px; flex-wrap: wrap; }

.preview-item { position: relative; width: 60px; height: 60px; }

.preview-item img { width: 100%; height: 100%; object-fit: cover; border-radius: 4px; }

.preview-item button { position: absolute; top: -5px; right: -5px; background: red; color: white; border: none; border-radius: 50%; width: 18px; height: 18px; font-size: 12px; }

.product-item-mini { display: flex; align-items: center; gap: 15px; background: #fff; padding: 10px; border-radius: 8px; }

.product-item-mini img { width: 50px; height: 50px; border-radius: 4px; }

.qty-control-mini { display: flex; align-items: center; gap: 10px; border: 1px solid #ddd; border-radius: 4px; padding: 2px 8px; width: fit-content; margin-top: 5px; }

.btn-submit-full { width: 100%; background: #2f5acf; color: white; border: none; padding: 12px; border-radius: 25px; font-weight: bold; }

.consult-process p { font-size: 13px; color: #555; margin: 0; }
</style>