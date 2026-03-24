/**
 * Báo cáo & Phân tích — dữ liệu mẫu + Chart.js 2.x (frontend only)
 */
(function () {
    'use strict';

    Chart.defaults.global.defaultFontFamily =
        'Nunito, -apple-system, system-ui, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif';
    Chart.defaults.global.defaultFontColor = '#858796';

    function formatVNDShort(value) {
        if (value >= 1e9) return (value / 1e9).toFixed(1) + ' tỷ';
        if (value >= 1e6) return (value / 1e6).toFixed(1) + ' tr';
        if (value >= 1e3) return (value / 1e3).toFixed(0) + ' nghìn';
        return value + '';
    }

    var doanhThuTheoKy = {
        ngay: {
            labels: ['T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'CN'],
            doanhThu: [18500000, 22100000, 19800000, 25400000, 31200000, 28900000, 17600000],
            loiNhuan: [4200000, 5100000, 4500000, 5800000, 7200000, 6600000, 3900000]
        },
        tuan: {
            labels: ['Tuần 1', 'Tuần 2', 'Tuần 3', 'Tuần 4'],
            doanhThu: [142000000, 158000000, 151000000, 167000000],
            loiNhuan: [32000000, 36200000, 33800000, 37800000]
        },
        thang: {
            labels: ['T1', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'T8', 'T9', 'T10', 'T11', 'T12'],
            doanhThu: [
                320000000, 298000000, 355000000, 342000000, 380000000, 365000000,
                390000000, 372000000, 401000000, 418000000, 445000000, 468000000
            ],
            loiNhuan: [
                72000000, 67000000, 80000000, 77000000, 86000000, 82000000,
                88000000, 84000000, 91000000, 95000000, 101000000, 106000000
            ]
        },
        quy: {
            labels: ['Quý 1', 'Quý 2', 'Quý 3', 'Quý 4'],
            doanhThu: [973000000, 1087000000, 1163000000, 1331000000],
            loiNhuan: [219000000, 245000000, 262000000, 300000000]
        }
    };

    var ctxLine = document.getElementById('chartDoanhThuLoiNhuan');
    var chartDoanhThu = null;

    if (ctxLine) {
        chartDoanhThu = new Chart(ctxLine, {
            type: 'line',
            data: {
                labels: doanhThuTheoKy.thang.labels,
                datasets: [
                    {
                        label: 'Doanh thu (VNĐ)',
                        data: doanhThuTheoKy.thang.doanhThu,
                        borderColor: 'rgba(78, 115, 223, 1)',
                        backgroundColor: 'rgba(78, 115, 223, 0.08)',
                        pointBackgroundColor: 'rgba(78, 115, 223, 1)',
                        pointBorderColor: '#fff',
                        pointHoverRadius: 4,
                        pointHoverBorderWidth: 2,
                        lineTension: 0.3,
                        fill: true
                    },
                    {
                        label: 'Lợi nhuận (DT − giá vốn lô hàng)',
                        data: doanhThuTheoKy.thang.loiNhuan,
                        borderColor: 'rgba(28, 200, 138, 1)',
                        backgroundColor: 'rgba(28, 200, 138, 0.06)',
                        pointBackgroundColor: 'rgba(28, 200, 138, 1)',
                        pointBorderColor: '#fff',
                        pointHoverRadius: 4,
                        lineTension: 0.3,
                        fill: true
                    }
                ]
            },
            options: {
                maintainAspectRatio: false,
                layout: { padding: { top: 10, right: 10, bottom: 0, left: 5 } },
                legend: { display: true, position: 'bottom' },
                tooltips: {
                    callbacks: {
                        label: function (tooltipItem, data) {
                            var label = data.datasets[tooltipItem.datasetIndex].label || '';
                            var v = tooltipItem.yLabel;
                            return label + ': ' + v.toLocaleString('vi-VN') + ' ₫';
                        }
                    }
                },
                scales: {
                    xAxes: [{
                        gridLines: { display: false },
                        ticks: { maxTicksLimit: 12 }
                    }],
                    yAxes: [{
                        ticks: {
                            maxTicksLimit: 6,
                            callback: function (value) {
                                return formatVNDShort(value) + ' ₫';
                            }
                        },
                        gridLines: { color: 'rgba(0,0,0,0.05)' }
                    }]
                }
            }
        });

        var kyHienTai = 'thang';
        document.querySelectorAll('[data-bc-ky]').forEach(function (btn) {
            btn.addEventListener('click', function () {
                var ky = btn.getAttribute('data-bc-ky');
                if (!doanhThuTheoKy[ky]) return;
                kyHienTai = ky;
                document.querySelectorAll('[data-bc-ky]').forEach(function (b) {
                    b.classList.toggle('active', b.getAttribute('data-bc-ky') === ky);
                });
                var d = doanhThuTheoKy[ky];
                chartDoanhThu.data.labels = d.labels;
                chartDoanhThu.data.datasets[0].data = d.doanhThu;
                chartDoanhThu.data.datasets[1].data = d.loiNhuan;
                chartDoanhThu.update();
            });
        });
    }

    /* Top 5 bán chạy — biểu đồ tròn */
    var ctxPie = document.getElementById('chartTopBanChay');
    if (ctxPie) {
        new Chart(ctxPie, {
            type: 'doughnut',
            data: {
                labels: [
                    'Paracetamol 500mg',
                    'Vitamin C 1000mg',
                    'Oresol',
                    'Khẩu trang y tế',
                    'Thuốc ho Prospan'
                ],
                datasets: [{
                    data: [28, 22, 18, 17, 15],
                    backgroundColor: [
                        '#4e73df',
                        '#1cc88a',
                        '#36b9cc',
                        '#f6c23e',
                        '#e74a3b'
                    ],
                    borderWidth: 2,
                    borderColor: '#fff'
                }]
            },
            options: {
                maintainAspectRatio: false,
                legend: { display: false },
                cutoutPercentage: 45,
                tooltips: {
                    callbacks: {
                        label: function (item, data) {
                            var label = data.labels[item.index] || '';
                            var val = data.datasets[0].data[item.index];
                            return label + ': ' + val + '% (tỷ trọng demo)';
                        }
                    }
                }
            }
        });
    }

    /* Thanh toán */
    var ctxPay = document.getElementById('chartThanhToan');
    if (ctxPay) {
        new Chart(ctxPay, {
            type: 'doughnut',
            data: {
                labels: ['Tiền mặt', 'Chuyển khoản / Ví điện tử'],
                datasets: [{
                    data: [42, 58],
                    backgroundColor: ['#4e73df', '#1cc88a'],
                    borderWidth: 2,
                    borderColor: '#fff'
                }]
            },
            options: {
                maintainAspectRatio: false,
                legend: { position: 'bottom' },
                cutoutPercentage: 50,
                tooltips: {
                    callbacks: {
                        label: function (item, data) {
                            return data.labels[item.index] + ': ' + data.datasets[0].data[item.index] + '%';
                        }
                    }
                }
            }
        });
    }

    /* Hiệu suất nhân viên — cột ngang */
    var ctxStaff = document.getElementById('chartNhanVien');
    if (ctxStaff) {
        new Chart(ctxStaff, {
            type: 'horizontalBar',
            data: {
                labels: ['Trần Thị Bình', 'Nguyễn Văn An', 'Lê Minh Cường', 'Phạm Thu Dung'],
                datasets: [{
                    label: 'Đơn chốt tại quầy (tháng)',
                    data: [312, 278, 241, 198],
                    backgroundColor: 'rgba(78, 115, 223, 0.85)',
                    borderColor: 'rgba(78, 115, 223, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                maintainAspectRatio: false,
                legend: { display: false },
                scales: {
                    xAxes: [{
                        ticks: { beginAtZero: true, stepSize: 50 },
                        gridLines: { color: 'rgba(0,0,0,0.05)' }
                    }],
                    yAxes: [{
                        gridLines: { display: false }
                    }]
                }
            }
        });
    }
})();
