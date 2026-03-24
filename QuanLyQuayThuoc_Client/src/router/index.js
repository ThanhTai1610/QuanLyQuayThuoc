import { createRouter, createWebHistory } from 'vue-router';

const routes = [
  // --- 1. NHÓM XÁC THỰC (Dùng Layout trống/đơn giản) ---
  {
    path: '/auth',
    component: () => import('../Layouts/LayoutKhach.vue'),
    children: [
      { path: 'dang-nhap', name: 'DangNhap', component: () => import('../pages/Auth/DangNhap.vue') },
      { path: 'dang-ky', name: 'DangKy', component: () => import('../pages/Auth/DangKy.vue') },
      { path: 'quen-mat-khau', name: 'QuenMatKhau', component: () => import('../pages/Auth/QuenMatKhau.vue') },
    ]
  },

  // --- 2. NHÓM KHÁCH HÀNG (Dùng Layout Người dùng) ---
  {
    path: '/',
    component: () => import('../Layouts/LayoutNguoiDung.vue'),
    children: [
      { path: '', name: 'TrangChu', component: () => import('../pages/KhachHang/TrangChu.vue') },
      { path: 'san-pham', name: 'DanhSachSanPham', component: () => import('../pages/KhachHang/DanhSachSanPham.vue') },
      { path: 'chi-tiet/:id', name: 'ChiTietSanPham', component: () => import('../pages/KhachHang/ChiTietSanPham.vue') },
      { path: 'gio-hang', name: 'GioHang', component: () => import('../pages/KhachHang/GioHang.vue') },
      { path: 'dat-hang', name: 'DatHang', component: () => import('../pages/KhachHang/DatHang.vue') },
      { path: 'lich-su-don-hang', name: 'LichSuDonHang', component: () => import('../pages/KhachHang/LichSuDonHang.vue') },
      { path: 'ho-so', name: 'ThongTinCaNhan', component: () => import('../pages/Auth/ThongTinCaNhan.vue') },
      { path: 'tu-van', name: 'ChatbotTuVan', component: () => import('../pages/KhachHang/ChatbotTuVan.vue') },
    ]
  },

  // --- 3. NHÓM NHÂN VIÊN (Dùng Layout Admin/Staff) ---
  {
    path: '/nhan-vien',
    component: () => import('../Layouts/LayoutAdmin.vue'),
    children: [
      { path: 'ban-hang', name: 'BanHang', component: () => import('../pages/NhanVien/POS/BanHangTaiQuay.vue') },
      { path: 'xu-ly-don', name: 'XuLyDonHang', component: () => import('../pages/NhanVien/XuLyDonHang.vue') },
      { path: 'kiem-ke', name: 'KiemKe', component: () => import('../pages/NhanVien/KiemKeDieuChuyen.vue') },
      { path: 'lo-hang', name: 'QuanLyLoHang', component: () => import('../pages/NhanVien/QuanLyLoHang.vue') },
    ]
  },

  // --- 4. NHÓM QUẢN TRỊ (Dùng Layout Admin) ---
  {
    path: '/admin',
    component: () => import('../Layouts/LayoutAdmin.vue'),
    children: [
      { path: 'thong-ke', name: 'BaoCao', component: () => import('../pages/QuanTriVien/BaoCaoPhanTich.vue') },
      { path: 'nguoi-dung', name: 'QuanLyNguoiDung', component: () => import('../pages/QuanTriVien/QuanLyNguoiDung.vue') },
      { path: 'danh-muc', name: 'QuanLyDanhMuc', component: () => import('../pages/QuanTriVien/QuanLyDanhMuc.vue') },
      { path: 'kho', name: 'QuanLyKho', component: () => import('../pages/QuanTriVien/QuanLyKho.vue') },
    ]
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// // Chặn truy cập nếu chưa đăng nhập (Navigation Guard)
// router.beforeEach((to, from, next) => {
//   const publicPages = ['/auth/dang-nhap', '/auth/dang-ky', '/'];
//   const authRequired = !publicPages.includes(to.path);
//   const loggedIn = localStorage.getItem('token');

//   if (authRequired && !loggedIn) {
//     return next('/auth/dang-nhap');
//   }
//   next();
// });

export default router;