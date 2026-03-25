import axios from 'axios';

const axiosClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'https://localhost:7070/api', 
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true // QUAN TRỌNG: Để gửi/nhận Cookie HttpOnly từ Backend
});

// BỘ CHẶN GỬI ĐI
axiosClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// BỘ CHẶN PHẢN HỒI
axiosClient.interceptors.response.use(
  (response) => {
    // Trả về data trực tiếp để các trang sau dùng ngắn gọn hơn
    return response.data; 
  },
  (error) => {
    if (error.response && error.response.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      // Tránh lặp vô tận nếu đang ở trang login
      if (!window.location.pathname.includes('/auth/dang-nhap')) {
        window.location.href = '/auth/dang-nhap';
      }
    }
    return Promise.reject(error);
  }
);

export default axiosClient;