import { createRouter, createWebHistory } from 'vue-router';

const ROLE_ADMIN = 1;
const ROLE_NHAN_VIEN = 2;
const ROLE_KHACH_HANG = 3;

const layNguoiDungDangNhap = () => {
  try {
    const raw = localStorage.getItem('user');
    return raw ? JSON.parse(raw) : null;
  } catch {
    return null;
  }
};

const layTrangMacDinhTheoVaiTro = (maVaiTro) => {
  if (maVaiTro === ROLE_ADMIN) {
    return '/admin/thong-ke';
  }

  if (maVaiTro === ROLE_NHAN_VIEN) {
    return '/nhan-vien/ban-hang';
  }

  return '/';
};

const routes = [
  {
    path: '/auth',
    component: () => import('../Layouts/LayoutKhach.vue'),
    meta: { public: true },
    children: [
      { path: 'dang-nhap', name: 'DangNhap', component: () => import('../pages/Auth/DangNhap.vue'), meta: { public: true, onlyGuest: true } },
      { path: 'dang-ky', name: 'DangKy', component: () => import('../pages/Auth/DangKy.vue'), meta: { public: true, onlyGuest: true } },
      { path: 'quen-mat-khau', name: 'QuenMatKhau', component: () => import('../pages/Auth/QuenMatKhau.vue'), meta: { public: true, onlyGuest: true } },
      { path: 'quen-mat-khau/otp', name: 'QuenMatKhauOTP', component: () => import('../pages/Auth/QuenMatKhauOTP.vue'), meta: { public: true, onlyGuest: true } },
      { path: 'quen-mat-khau/doi-mk', name: 'DatLaiMatKhau', component: () => import('../pages/Auth/DatLaiMatKhau.vue'), meta: { public: true, onlyGuest: true } },
    ]
  },
  {
    path: '/',
    component: () => import('../Layouts/LayoutNguoiDung.vue'),
    children: [
      { path: '', name: 'TrangChu', component: () => import('../pages/KhachHang/TrangChu.vue'), meta: { public: true } },
      { path: 'san-pham', name: 'DanhSachSanPham', component: () => import('../pages/KhachHang/DanhSachSanPham.vue'), meta: { public: true } },
      { path: 'chi-tiet/:id', name: 'ChiTietSanPham', component: () => import('../pages/KhachHang/ChiTietSanPham.vue'), meta: { public: true } },
      { path: 'tu-van', name: 'ChatbotTuVan', component: () => import('../components/ChatbotTuVan.vue'), meta: { public: true } },
      { path: 'dia-chi', name: 'QuanLySoDiaChi', component: () => import('../pages/KhachHang/QuanLySoDiaChi.vue'), meta: { roles: [ROLE_KHACH_HANG] } },
      { path: 'gio-hang', name: 'GioHang', component: () => import('../pages/KhachHang/GioHang.vue'), meta: { roles: [ROLE_KHACH_HANG] } },
      { path: 'dat-hang', name: 'DatHang', component: () => import('../pages/KhachHang/DatHang.vue'), meta: { roles: [ROLE_KHACH_HANG] } },
      { path: 'lich-su-don-hang', name: 'LichSuDonHang', component: () => import('../pages/Auth/LichSuDonHang.vue'), meta: { roles: [ROLE_KHACH_HANG] } },
      { path: 'ho-so', name: 'ThongTinCaNhan', component: () => import('../pages/Auth/ThongTinCaNhan.vue'), meta: { roles: [ROLE_KHACH_HANG, ROLE_NHAN_VIEN, ROLE_ADMIN] } },
    ]
  },
  {
    path: '/nhan-vien',
    component: () => import('../Layouts/LayoutAdmin.vue'),
    meta: { roles: [ROLE_NHAN_VIEN, ROLE_ADMIN] },
    children: [
      { path: 'ban-hang', name: 'BanHang', component: () => import('../pages/NhanVien/POS/BanHangTaiQuay.vue'), meta: { roles: [ROLE_NHAN_VIEN, ROLE_ADMIN] } },
      { path: 'xu-ly-don', name: 'XuLyDonHang', component: () => import('../pages/NhanVien/XuLyDonHang.vue'), meta: { roles: [ROLE_NHAN_VIEN, ROLE_ADMIN] } },
      { path: 'kiem-ke', name: 'KiemKe', component: () => import('../pages/NhanVien/KiemKeDieuChuyen.vue'), meta: { roles: [ROLE_NHAN_VIEN, ROLE_ADMIN] } },
      { path: 'lo-hang', name: 'QuanLyLoHang', component: () => import('../pages/NhanVien/QuanLyLoHang.vue'), meta: { roles: [ROLE_NHAN_VIEN, ROLE_ADMIN] } },
    ]
  },
  {
    path: '/admin',
    component: () => import('../Layouts/LayoutAdmin.vue'),
    meta: { roles: [ROLE_ADMIN] },
    children: [
      { path: 'thong-ke', name: 'BaoCao', component: () => import('../pages/QuanTriVien/BaoCaoPhanTich.vue'), meta: { roles: [ROLE_ADMIN] } },
      { path: 'nguoi-dung', name: 'QuanLyNguoiDung', component: () => import('../pages/QuanTriVien/QuanLyNguoiDung.vue'), meta: { roles: [ROLE_ADMIN] } },
      { path: 'danh-muc', name: 'QuanLyDanhMuc', component: () => import('../pages/QuanTriVien/QuanLyDanhMuc.vue'), meta: { roles: [ROLE_ADMIN] } },
      { path: 'kho', name: 'QuanLyKho', component: () => import('../pages/QuanTriVien/QuanLyKho.vue'), meta: { roles: [ROLE_ADMIN, ROLE_NHAN_VIEN] } },
    ]
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to) => {
  const token = localStorage.getItem('token');
  const user = layNguoiDungDangNhap();
  const maVaiTro = user?.maVaiTro;

  if (to.matched.some((record) => record.meta?.onlyGuest) && token && maVaiTro) {
    return layTrangMacDinhTheoVaiTro(maVaiTro);
  }

  const requiredRoles = to.matched
    .flatMap((record) => (Array.isArray(record.meta?.roles) ? record.meta.roles : []));

  if (requiredRoles.length === 0) {
    return true;
  }

  if (!token || !maVaiTro) {
    return {
      name: 'DangNhap',
      query: { redirect: to.fullPath }
    };
  }

  if (!requiredRoles.includes(maVaiTro)) {
    return layTrangMacDinhTheoVaiTro(maVaiTro);
  }

  return true;
});

export default router;
