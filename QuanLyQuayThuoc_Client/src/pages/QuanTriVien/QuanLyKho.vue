<template>
  <div class="container-fluid qlk-page">

    <div class="qlk-rolebar">
      <div>
        <h1 class="h3 mb-0 text-gray-800">Quản lý Kho &amp; Lô hàng</h1>
        <div class="qlk-muted mt-1">Kết nối API backend — dữ liệu thời gian thực.</div>
      </div>
      <div class="d-flex align-items-center flex-wrap">
        <label class="small text-muted mr-2 mb-0">Chế độ</label>
        <select class="form-control form-control-sm" v-model="vaiTro" style="min-width:180px;">
          <option value="nhan-vien">Nhân viên (không chỉnh sửa lô)</option>
          <option value="admin">Admin (có quyền chỉnh sửa)</option>
        </select>
      </div>
    </div>

    <div class="row">
      <div class="col-lg-3 mb-3">
        <div class="card qlk-subnav">
          <div class="card-header py-3">
            <div class="font-weight-bold text-primary">Menu</div>
          </div>
          <div class="card-body">
            <ul class="nav nav-pills flex-column">
              <li class="nav-item" v-for="tab in tabs" :key="tab.value">
                <a class="nav-link" :class="{ active: tabHienTai === tab.value }"
                  href="#" @click.prevent="tabHienTai = tab.value">
                  {{ tab.label }}
                </a>
              </li>
            </ul>
          </div>
        </div>
      </div>

      <div class="col-lg-9">
        <component 
          :is="currentTabComponent" 
          :is-admin="vaiTro === 'admin'" 
        />
      </div>
    </div>

  </div>
</template>

<script setup>
import '../../assets/css_admin/quan-ly-kho.css';
import { ref, computed } from 'vue';

// Import các sub-components
import KhoTongQuan from './KhoTongQuan.vue';
import KhoLoHang   from './KhoLoHang.vue';
import KhoNhapKho  from './KhoNhapKho.vue';
import KhoCanhBao  from './KhoCanhBao.vue';

const vaiTro     = ref('nhan-vien');
const tabHienTai = ref('tong-quan');

const tabs = [
  { value: 'tong-quan', label: 'Tổng quan tồn kho'  },
  { value: 'lo-hang',   label: 'Danh sách lô hàng'  },
  { value: 'nhap-kho',  label: 'Nhập hàng mới'      },
  { value: 'canh-bao',  label: 'Cảnh báo hết hạn'   },
];

// Map tab value với Component tương ứng
const componentMap = {
  'tong-quan': KhoTongQuan,
  'lo-hang': KhoLoHang,
  'nhap-kho': KhoNhapKho,
  'canh-bao': KhoCanhBao
};

// Tự động trả về component theo tab đang chọn
const currentTabComponent = computed(() => componentMap[tabHienTai.value]);
</script>