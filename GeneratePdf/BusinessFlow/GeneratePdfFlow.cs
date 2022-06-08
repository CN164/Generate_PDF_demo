using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronPdf;
using GeneratePDF_demo.Models;
using System.IO;
using System.Text;
using System.Globalization;
using BarcodeLib;
using System.Drawing;
using IronBarCode;
using Amazon.S3.Model;
using GeneratePDF_demo.Service;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;

namespace GeneratePDF_demo.BusinessFlow
{
    public class GeneratePdfFlow
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private string bucketName = "s3://ntb-receipt-dev/local/2022/01/28/20000083560/";
        private string keyName = "";
        private string filePath = "";
        private IS3Service s3Service;

        public ConsentResponse ProcessGeneratePdf(ContactRequest request)
        {
            UploadfileResponse uploadfileResponse = null;
            ConsentResponse ConsentModelResponse = new ConsentResponse();
            string tempFolder = Directory.GetCurrentDirectory() + $"/Html/auth_bg_Mapdata";
            if (Directory.Exists(tempFolder))
                Directory.Delete(tempFolder, true);
            try
            {
                ConsentModelResponse = GenerateHtmlAndMapingData(request);
                ConsentModelResponse.message = "Delete and Generate complate!";
            }
            catch (Exception ex)
            {
                Console.WriteLine("generate Authorization for Background Check file: " + ex.Message);
                throw ex;
            }
            finally
            {
                Directory.Delete(tempFolder, true);
            }

            //UploadfileRequest uploadfileRequest = GetUploadfileRequest("PUT");
            //ListObjectsRequest listRequest = new ListObjectsRequest
            //{
            //    BucketName = uploadfileRequest.bucket,
            //    Prefix = uploadfileRequest.key
            //};
            //var response = s3Service.GetAllobjectS3Async(listRequest);
            //if (response?.Result != null)
            //{
            //    MakeACopyFile(response.Result?.S3Objects, listRequest, s3Service);
            //}

            //uploadfileResponse = UploadFile();

            return ConsentModelResponse;
        }
        public void Uploads3(IS3Service s3Service)
        {
            UploadfileRequest uploadfileRequest = GetUploadfileRequest("PUT");
            ListObjectsRequest listRequest = new ListObjectsRequest
            {
                BucketName = uploadfileRequest.bucket,
                Prefix = uploadfileRequest.key
            };
            var response = s3Service.GetAllobjectS3Async(listRequest);
            if (response?.Result != null)
            {
                MakeACopyFile(response.Result?.S3Objects, listRequest, s3Service);
            }
        }
        public ConsentResponse GenerateHtmlAndMapingData(ContactRequest request)
        {
            ConsentResponse ConsentModel = new ConsentResponse();
            string nameFile = "Authorization_for_Background_Check";
            string stringHTML = string.Empty;
            string sourcePath = Directory.GetCurrentDirectory() + $"/Html/auth_bg_Mapdata" + ".html";
            string targetPath = Directory.GetCurrentDirectory() + $"/Html/auth_bg_Mapdata/";
            FileStream fileStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                stringHTML = streamReader.ReadToEnd();
            }
            if (!(Directory.Exists(targetPath)))
            {
                //Mapping Data To Template html And Create New File
                Directory.CreateDirectory(targetPath);
                Stream stream = null;
                stream = new FileStream(targetPath + "/auth_bg_Mapdate.html", FileMode.OpenOrCreate);

                GeneratedBarcode MyBarCode = IronBarCode.BarcodeWriter.CreateBarcode((request.idCard), BarcodeWriterEncoding.QRCode);
                MyBarCode.SaveAsPng("MyBarCode.png");
                MyBarCode.SaveAsHtmlFile("MyBarCode.html"); 

                try
                {
                    using (StreamWriter outputFile = new StreamWriter(stream, Encoding.UTF8))
                    {
                        stream = null;
                        outputFile.Write(MappingDataAuth(request, stringHTML));
                        //outputFile.Write(barcode.GetImageData(SaveTypes.PNG));
                        outputFile.Flush();
                        outputFile.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("generate html mapping" + ex.Message);
                    throw ex;
                }
                finally
                {
                    if (stream != null)
                        stream.Dispose();
                }
            }

            //using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            //{
            //    IS3Service s3Service = scope.ServiceProvider.GetService<IS3Service>();
            //}

                var Renderer = new ChromePdfRenderer();

            using var PDF = Renderer.RenderHtmlFileAsPdf(targetPath + "/auth_bg_Mapdate.html");
            PDF.SaveAs(nameFile + ".pdf");

            string currentPath = Directory.GetCurrentDirectory().Replace("\\", "/") + $"/Authorization_for_Background_Check" + ".pdf";

            
            keyName = nameFile;
            filePath = currentPath;
            
            ConsentModel.path = currentPath;

            //UploadFile(); //upload normal
            //Uploads3(s3Service); //upload ntb

            return ConsentModel;
        }
        public string MappingDataAuth(ContactRequest request, string stringHTML)
        {
            string response = stringHTML;
            response = response.Replace("{{contractCreatedAt}}", request.contractCreatedAt.ToString("d MMMM yyyy", new CultureInfo("th-TH")))
                .Replace("{{idCard}}", Convert.ToInt64(request.idCard).ToString("#-####-#####-##-#"))
                .Replace("{{houseNo}}", request.houseNo)
                .Replace("{{villageNo}}", request.villageNo)
                .Replace("{{laneContact}}", request.laneContact)
                .Replace("{{roadContact}}", request.roadContact)
                .Replace("{{SubDistrict}}", request.SubDistrict)
                .Replace("{{districtContact}}", request.districtContact)
                .Replace("{{province}}", request.province)
                .Replace("{{phoneNumber}}", Convert.ToInt64(request.phoneNumber).ToString("0##-###-####"))
                .Replace("{{preName}}", request.preName)
                .Replace("{{firstName}}", request.firstName) 
                .Replace("{{lastName}}", request.lastName)
                .Replace("{{currentPath}}", Directory.GetCurrentDirectory().Replace("\\", "/"));

            return response;
        }
        private UploadfileRequest GetUploadfileRequest(string method)
        {
            string key = string.Empty;
            return new UploadfileRequest()
            {
                key = key,
                bucket = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                method = method
            };
        }
        private void MakeACopyFile(List<S3Object> S3Objects, ListObjectsRequest listRequest, IS3Service s3Service)
        {
            foreach (S3Object obj in S3Objects)
            {
                CopyObjectRequest copyObjectRequest = new CopyObjectRequest()
                {
                    SourceBucket = listRequest.BucketName,
                    SourceKey = listRequest.Prefix,
                    DestinationBucket = listRequest.BucketName,
                    DestinationKey = listRequest.Prefix + $"-{DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss")}"
                };
                s3Service.CopyObject(copyObjectRequest);
            }
        }
        public UploadfileResponse UploadFile(UploadfileRequest uploadRequest, string fileName, IS3Service s3Service)
        {
            string url = s3Service.GetSignUrl(uploadRequest).url;
            UploadfileResponse response = s3Service.UploadFile(url, fileName);
            return response;
        }
        //public async void UploadFile()
        //{
            
        //var Client = new AmazonS3Client(Amazon.RegionEndpoint.APSoutheast1);
        //    try
        //    {
        //        PutObjectRequest putRequest = new PutObjectRequest
        //        {
        //            BucketName = bucketName,
        //            Key = keyName,
        //            FilePath = filePath,
        //            ContentType = "document/pdf"
        //        };
        //        PutObjectResponse putResponse = await Client.PutObjectAsync(putRequest);
        //    }
        //    catch (AmazonS3Exception amazonS3Exception)
        //    {
        //        if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
        //        {
        //            throw new Exception("Check the provided AWS Credentials.");
        //        }
        //        else
        //        {
        //            throw new Exception("Error occurred: " + amazonS3Exception.Message);
        //        }
        //    }
        //}
        private UploadfileRequest GetUploadFileRequest(string method)
        {
            //InstallmentBusinessLogic installmentBusinessLogic = new InstallmentBusinessLogic();
            return new UploadfileRequest()
            {
                key = keyName,
                bucket = bucketName,
                method = method
            };
        }
    }
}
