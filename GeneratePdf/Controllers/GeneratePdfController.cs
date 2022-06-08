using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronPdf;
using GeneratePDF_demo.BusinessFlow;
using GeneratePDF_demo.Models;
using GeneratePDF_demo.Service;

namespace GeneratePDF_demo.Controllers
{
    public class GeneratePdfController : ControllerBase
    {
        private readonly GeneratePdfFlow _generatePdfFlow;

        public GeneratePdfController(GeneratePdfFlow generatePdfFlow)
        {
            this._generatePdfFlow = generatePdfFlow;
        }
        [HttpPost("api/generatepdf/authenticate")]
        public ConsentResponse GeneratePdf([FromBody] ContactRequest request)
        {
            ConsentResponse response = _generatePdfFlow.ProcessGeneratePdf(request);
            
            return response;
        }
    }
}
