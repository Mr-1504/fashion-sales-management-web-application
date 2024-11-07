namespace BTL_LTWeb.Models
{
    public class TempUserOtp
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string OtpCode { get; set; } = null!;
        public DateTime OtpExpiration { get; set; }
    }
}
