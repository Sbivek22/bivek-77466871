namespace WebAdditonal.Models
{
    public sealed class RunRequest
    {
        public string? Code { get; set; }
        public int Width { get; set; } = 900;
        public int Height { get; set; } = 600;
    }
}
