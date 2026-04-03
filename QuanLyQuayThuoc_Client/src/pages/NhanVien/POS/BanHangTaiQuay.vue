  <template>
    <div class="container-fluid pos-root">
      <TimKiem @add-to-cart="themVaoGioHang" />

      <div class="row mt-3">
        <div class="col-xl-8 col-lg-7">
          <GioHang :cartItems="cacSanPhamTrongGio" @remove-item="xoaSanPham" @update-quantity="capNhatSoLuong" />
        </div>

        <div class="col-xl-4 col-lg-5">
          <ThanhToan :tongTienHang="tongTienHang" :maDonHang="maDonHang" @checkout="moHoaDon" @clear-cart="xoaGioHang" />
        </div>
      </div>

      <Modals :invoiceData="duLieuHoaDon" @add-quick-item="themVaoGioHang" @finish-payment="xuLyHoanThanhThanhToan" />
    </div>
  </template>

  <script setup>
  import { ref, computed, reactive, onMounted, onUnmounted } from 'vue';
  import axios from 'axios';
  import TimKiem from '../../NhanVien/POS/TimKiem.vue';
  import GioHang from '../../NhanVien/POS/GioHang.vue';
  import ThanhToan from '../../NhanVien/POS/ThanhToan.vue';
  import Modals from '../../NhanVien/POS/Modals.vue';
  import { useMomo } from '../../../services/useMomo';
  import { Modal } from 'bootstrap';
  import Swal from 'sweetalert2';
  import * as signalR from "@microsoft/signalr";

  // STATE
  const cacSanPhamTrongGio = ref([]);
  const maDonHang = ref('POS-' + Math.floor(Math.random() * 10000).toString().padStart(4, '0'));

  const duLieuHoaDon = reactive({
    maHd: maDonHang.value,
    thoiGian: '',
    khachHang: '',
    cacSanPhamTrongGio: [],
    tongTienHang: 0,
    giamGia: 0,
    canTra: 0,
    phuongThuc: 'Tiền mặt',
    _chiTietThanhToan: null
  });

  // COMPUTED
  const tongTienHang = computed(() => {
    return cacSanPhamTrongGio.value.reduce((tong, sanPham) => tong + (sanPham.giaBan * sanPham.soLuong), 0);
  });

  // CART LOGIC
  const themVaoGioHang = (sanPham) => {
    const sanPhamHienCo = cacSanPhamTrongGio.value.find(i => i.maThuoc === sanPham.maThuoc);

    if (sanPhamHienCo) {
      sanPhamHienCo.soLuong += 1;
    } else {
      cacSanPhamTrongGio.value.push({
        ...sanPham,
        soLuong: 1,
        maDvtSelected: sanPham.maDvtSelected || sanPham.danhSachDonVi[0]?.maDvt,
        loHangSelected: sanPham.loHangSelected || sanPham.danhSachLo[0]?.maLo
      });
    }
  };

  const capNhatSoLuong = ({ index, change }) => { 
    const sanPham = cacSanPhamTrongGio.value[index];
    if (sanPham) {
      sanPham.soLuong += change;
      if (sanPham.soLuong <= 0) xoaSanPham(index);
    }
  };

  const xoaSanPham = (viTri) => {
    cacSanPhamTrongGio.value.splice(viTri, 1);
  };

  const xoaGioHang = () => {
    if (cacSanPhamTrongGio.value.length === 0) return;

    Swal.fire({
      title: 'Xác nhận xóa?',
      text: "Toàn bộ thuốc trong giỏ sẽ bị loại bỏ",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Xoá',
      cancelButtonText: 'Hoàn tác'
    }).then((ketQua) => {
      if (ketQua.isConfirmed) {
        cacSanPhamTrongGio.value = [];
        Swal.fire(
          'Đã xóa!',
          'Giỏ hàng của bạn hiện đang trống.',
          'success'
        );
      }
    });
  };


  // OPEN INVOICE
  const moHoaDon = (chiTietThanhToan) => {
  if (cacSanPhamTrongGio.value.length === 0) {
    Swal.fire({
      icon: 'info',
      title: 'Giỏ hàng trống',
      text: 'Vui lòng chọn ít nhất một loại thuốc để thanh toán!',
      confirmButtonColor: '#3085d6'
    });
    return;
  }

  duLieuHoaDon.maHd = maDonHang.value;
  duLieuHoaDon.thoiGian = new Date().toLocaleString('vi-VN');
  duLieuHoaDon.cartItems = [...cacSanPhamTrongGio.value];
  duLieuHoaDon.tongTienHang = tongTienHang.value;
  duLieuHoaDon.giamGia = chiTietThanhToan.giamGia;
  duLieuHoaDon.canTra = chiTietThanhToan.khachCanTra;
  
  // Logic hiển thị tên phương thức
  const pt = chiTietThanhToan.phuongThuc;
  duLieuHoaDon.phuongThuc = pt === 'tien-mat' ? 'Tiền mặt' : (pt === 'momo' ? 'Ví MoMo' : 'Chuyển khoản');
  
  duLieuHoaDon._chiTietThanhToan = chiTietThanhToan;

  // SỬA TẠI ĐÂY
  const phanTuModal = document.getElementById('modalHoaDon');
  if (phanTuModal) {
    const modalCuaToi = Modal.getOrCreateInstance(phanTuModal); 
    modalCuaToi.show();
  }
};

  // CALL API THANH TOÁN
  const { createPayment } = useMomo();
  const goiApiThanhToan = async (chiTietThanhToan) => {
  if (cacSanPhamTrongGio.value.length === 0) return;

  try {
    const token = localStorage.getItem('token');
    
    // Đồng bộ giá trị phương thức thanh toán với Backend
    // Giả sử logic của bạn: 'momo' hoặc 'chuyen-khoan' tùy vào Component ThanhToan.vue trả về
    const phuongThuc = chiTietThanhToan.phuongThuc; 

    const dto = {
      maKhachHang: 0,
      phuongThucThanhToan: phuongThuc, // Truyền trực tiếp giá trị nhận được
      giamGia: chiTietThanhToan.giamGia || 0,
      chiTiet: cacSanPhamTrongGio.value.map(sanPham => ({
        maLo: sanPham.loHangSelected,
        maDVT: sanPham.maDvtSelected,
        soLuong: sanPham.soLuong,
        giaBan: sanPham.giaBan
      }))
    };

    const ketQua = await axios.post(
      'https://localhost:7070/api/BanHang/thanh-toan',
      dto,
      { headers: { Authorization: `Bearer ${token}` } }
    );

    if (ketQua.data.success) {
      // KIỂM TRA NẾU LÀ MOMO THÌ CHUYỂN TRANG
      if (phuongThuc === 'momo') {
        // Tắt modal trước khi đi
        datLaiTrang(); 
        
        // Gọi MoMo
        await createPayment(
          tongTienHang.value - (chiTietThanhToan.giamGia || 0), // Số tiền thực tế sau giảm giá
          `Bán tại quầy - HD: ${ketQua.data.maDonHang}`, 
          "NhanVien"
        );
        return;
      }

      // Nếu là Tiền mặt hoặc loại khác thì báo thành công tại chỗ
      Swal.fire({
        title: 'Thành công!',
        text: `Hóa đơn ${ketQua.data.maDonHang} đã được lưu hệ thống.`,
        icon: 'success',
        confirmButtonText: 'Đóng',
        confirmButtonColor: '#28a745',
        timer: 2500,
        timerProgressBar: true
      });

      datLaiTrang();
    }
  } catch (loi) {
    console.error(loi);
    Swal.fire({
      title: 'Lỗi thanh toán',
      text: loi.response?.data?.message || 'Không thể kết nối Server',
      icon: 'error'
    });
  }
};

  const xuLyHoanThanhThanhToan = () => {
    goiApiThanhToan(duLieuHoaDon._chiTietThanhToan);
  };

  const datLaiTrang = () => {
    cacSanPhamTrongGio.value = [];
    maDonHang.value = 'POS-' + Math.floor(Math.random() * 10000).toString().padStart(4, '0');
    const phanTuModal = document.getElementById('modalHoaDon');
    if (phanTuModal) {
      const phienBanModal = window.bootstrap.Modal.getInstance(phanTuModal);
      if (phienBanModal) {
        phienBanModal.hide();
      }
    }

    const nenModal = document.querySelector('.modal-backdrop');
    if (nenModal) {
      nenModal.remove();
      document.body.classList.remove('modal-open');
      document.body.style.overflow = '';
      document.body.style.paddingRight = '';
    }
  };


  // Khởi tạo kết nối SignalR (Giữ nguyên phần khai báo của bạn)
  const connection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7070/barcodeHub")
      .withAutomaticReconnect()
      .build();

  onMounted(() => {
      // Lắng nghe sự kiện "ReceiveBarcode"
      connection.on("ReceiveBarcode", (data) => {
          console.log("Dữ liệu nhận từ máy quét:", data);
          
          // data ở đây chính là thông tin thuốc mà Backend gửi qua SignalR
          // Chúng ta gọi hàm themVaoGioHang đã có sẵn của bạn
          themVaoGioHang(data);

          // Hiển thị thông báo nhỏ (Toast) thay vì alert gây gián đoạn
          Swal.fire({
              toast: true,
              position: 'top-end',
              icon: 'success',
              title: `Đã thêm: ${data.tenThuoc}`,
              showConfirmButton: false,
              timer: 1500
          });
      });

      connection.start()
          .then(() => console.log("SignalR Connected!"))
          .catch(err => console.error("SignalR Connection Error: ", err));
  });
  onUnmounted(() => {
      if (connection) {
          connection.stop();
      }
  });
  </script>

  <style scoped>
  @import "../../../assets/css_admin/pos.css";

  .pos-root {
    padding-top: 1rem;
    padding-bottom: 2rem;
    background-color: #f8f9fc;
    min-height: 100vh;
  }
  </style>