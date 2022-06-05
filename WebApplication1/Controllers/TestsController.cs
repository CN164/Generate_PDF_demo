using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronPdf;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    public class TestsController : ControllerBase
    {
        [HttpGet("testAPI1")]
        public string testAPI()
        {
            var Renderer = new ChromePdfRenderer();

            using var PDF = Renderer.RenderHtmlFileAsPdf("Html/auth_bg.html");
            PDF.SaveAs("Authorization_for_Background Check.pdf");
            return "Generate Compleat!";
        }
    }
}
