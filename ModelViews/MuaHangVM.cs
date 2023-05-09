using System;
using System.ComponentModel.DataAnnotations;

namespace BookStore.ModelViews
{
    public class MuaHangVM
    {

        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Họ và Tên")]
        public string FullName { get; set; }
        //public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Địa chỉ nhận hàng")]
        public string Address { get; set; }
        //[Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành")]
        //public int TinhThanh { get; set; }
        //[Required(ErrorMessage = "Vui lòng chọn Quận/Huyện")]
        //public string QuanHuyen { get; set; }
        //[Required(ErrorMessage = "Vui lòng chọn Phường/Xã")]
        //public string PhuongXa { get; set; }
        public int PaymentId { get; set; }
        public string DesPayment { get; set; }
        public DateTime ShipDate { get; set; }

        public string? Note { get; set; } = null;

    }
}
