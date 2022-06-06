using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronPdf;
using GeneratePDF_demo.Models;
using System.IO;
using System.Text;
using System.Globalization;

namespace GeneratePDF_demo.BusinessFlow
{
    public class GeneratePdfFlow
    {
        public ConsentResponse ProcessGeneratePdf(ContactRequest request)
        {
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

            return ConsentModelResponse;
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
                try
                {
                    using (StreamWriter outputFile = new StreamWriter(stream, Encoding.UTF8))
                    {
                        stream = null;
                        outputFile.Write(MappingDataAuth(request, stringHTML));
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

            var Renderer = new ChromePdfRenderer();

            using var PDF = Renderer.RenderHtmlFileAsPdf(targetPath + "/auth_bg_Mapdate.html");
            PDF.SaveAs(nameFile + ".pdf");

            string currentPath = Directory.GetCurrentDirectory().Replace("\\", "/") + $"/Authorization_for_Background_Check" + ".pdf";
            ConsentModel.path = currentPath;

            return ConsentModel;
        }
        public string MappingDataAuth(ContactRequest request, string stringHTML)
        {
            string response = stringHTML;
            response = response.Replace("{{contractCreatedAt}}", request.contractCreatedAt.ToString("d MMMM yyyy", new CultureInfo("th-TH")))
                .Replace("{{idCard}}", request.idCard)
                .Replace("{{houseNo}}", request.houseNo)
                .Replace("{{villageNo}}", request.villageNo)
                .Replace("{{laneContact}}", request.laneContact)
                .Replace("{{roadContact}}", request.roadContact)
                .Replace("{{SubDistrict}}", request.SubDistrict)
                .Replace("{{districtContact}}", request.districtContact)
                .Replace("{{province}}", request.province)
                .Replace("{{phoneNumber}}", request.phoneNumber)
                .Replace("{{preName}}", request.preName)
                .Replace("{{firstName}}", request.firstName)
                .Replace("{{lastName}}", request.lastName)
                .Replace("{{currentPath}}", Directory.GetCurrentDirectory().Replace("\\", "/"));

            return response;
        }
    }
}
