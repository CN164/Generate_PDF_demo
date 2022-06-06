using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronPdf;
using GeneratePDF_demo.BusinessFlow;
using GeneratePDF_demo.Models;


namespace GeneratePDF_demo.Controllers
{
    public class GeneratePdfController : ControllerBase
    {
        private readonly GeneratePdfFlow _generatePdfFlow;

        public GeneratePdfController(GeneratePdfFlow generatePdfFlow)
        {
            this._generatePdfFlow = generatePdfFlow;
        }
        [HttpPost("api/generatepdf")]
        public string GeneratePdf([FromBody] ContactRequest request)
        {
            string response = _generatePdfFlow.ProcessGeneratePdf(request);
            
            return response;
        }
    }
}
