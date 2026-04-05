//using BarcodeStandard;
//using SkiaSharp;

//namespace QuanLyQuayThuoc.Services // Thay bằng namespace của dự án bạn
//{
//    public interface IBarcodeService
//    {
//        string GenerateBarcode(string maVach);
//    }

//    public class BarcodeService : IBarcodeService
//    {
//        public string GenerateBarcode(string maVach)
//        {
//            try
//            {
//                var barcode = new Barcode();

//                // CẤU HÌNH MỚI CHO BẢN 3.x
//                barcode.IncludeLabel = true;

//                // Thay vì gán trực tiếp, ta dùng đối tượng SKFont
//                // Lỗi "Cannot implicitly convert" sẽ hết khi dùng đúng Constructor của SKFont
//                var typeface = SKTypeface.FromFamilyName("Arial");
//                barcode.LabelFont = new SKFont(typeface, 18); // 18 là kích thước chữ (FontSize)

//                // Vẽ mã vạch Code 128
//                var img = barcode.Encode(BarcodeStandard.Type.Code128, maVach, SKColors.Black, SKColors.White, 300, 120);

//                // Chuyển hình ảnh thành chuỗi Base64
//                using (var data = img.Encode(SKEncodedImageFormat.Png, 100))
//                {
//                    return "data:image/png;base64," + Convert.ToBase64String(data.ToArray());
//                }
//            }
//            catch (Exception)
//            {
//                return string.Empty;
//            }
//        }
//    }
//}