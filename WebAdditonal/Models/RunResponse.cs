namespace WebAdditonal.Models
{
    public sealed class RunResponse
    {
        public bool Success { get; set; }
        public string Output { get; set; } = "";
        public string? ImageBase64 { get; set; }
    }
}
