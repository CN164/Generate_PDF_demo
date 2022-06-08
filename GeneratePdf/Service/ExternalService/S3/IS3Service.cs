using Amazon.S3.Model;
using System.Threading.Tasks;
using GeneratePDF_demo.Models;

namespace GeneratePDF_demo.Service
{
    public interface IS3Service
    {
        SignedUrlResponse GetSignUrl(UploadfileRequest model);
        UploadfileResponse UploadFile(string pathUrl, string pathFile);
        Task<ListObjectsResponse> GetAllobjectS3Async(ListObjectsRequest listRequest);
        Task CopyObject(CopyObjectRequest copyObjectRequest);
    }
}
