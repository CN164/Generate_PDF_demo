namespace GeneratePDF_demo.Models
{
    public class UploadfileRequest
    {
        public string bucket { get; set; }
        public string key { get; set; }
        public string method { get; set; }
    }

    public class UploadfileResponse
    {
        public string url { get; set; }
        public bool ststus { get; set; }
    }
    public class SignedUrlResponse
    {
        public string url { get; set; }
    }
}
