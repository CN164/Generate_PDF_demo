using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using GeneratePDF_demo.Models;


namespace GeneratePDF_demo.Service
{
    public class S3Service
    {
        private static readonly IAmazonS3 _client = new AmazonS3Client();

        public Task<ListObjectsResponse> GetAllObjectS3Async(ListObjectsRequest listRequest)
        {
            try
            {
                Task<ListObjectsResponse> response = _client.ListObjectsAsync(listRequest);
                return response;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("      !!! Error encountered on server. Message:'{0}' when writing an object", e.Message);
                throw e.InnerException;
            }
            catch (Exception e)
            {
                Console.WriteLine("      !!! Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                throw e.InnerException;
            }
        }
        public SignedUrlResponse GetsignUrl(UploadfileResponse model)
        {
            SignedUrlResponse response = new SignedUrlResponse();
            try
            {
                using (WebClient webc = new WebClient())
                {
                    webc.Headers["Content-Type"] = "application/json";
                    webc.Headers["x-api-key"] = "";
                    string url = "/s3/signed-url";
                    string json = JsonConvert.SerializeObject(model);
                    var result = webc.UploadString(url, "POST", json);
                    response = JsonConvert.DeserializeObject<SignedUrlResponse>(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("# Error Message GetSignUrl: " + ex.Message);
            }
            return response;
        }
        public UploadfileResponse UploadFile(string pathUrl, string pathFile)
        {
            UploadfileResponse response = new UploadfileResponse();
            try
            {
                using (WebClient webc = new WebClient())
                {
                    webc.Headers["Content-Type"] = "application/pdf";
                    webc.Headers["x-api-key"] = "";
                    webc.UploadFile(pathUrl, "PUT", pathFile);
                    int stringIndex = pathUrl.IndexOf("?");
                    response.url = pathUrl.Substring(0, stringIndex);
                    response.ststus = true;
                }
            }
            catch (Exception ex)
            {
                response.url = "";
                response.ststus = false;
                Console.WriteLine("# Error Message UploadFile: " + ex.Message);
            }
            return response;
        }
        public async Task CopyObject(CopyObjectRequest copyObjectRequest)
        {
            await _client.CopyObjectAsync(copyObjectRequest);
        }
    }
}
