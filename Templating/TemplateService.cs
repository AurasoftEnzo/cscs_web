

////namespace CSCS_Web_Enzo_1.CSCS.Template
////{
////    public class TemplateService
////    {
////        private readonly IWebHostEnvironment _env;

////        public TemplateService(IWebHostEnvironment env)
////        {
////            _env = env;
////        }

////        public string LoadTemplate(string templateName)
////        {
////            var templatePath = Path.Combine(_env.WebRootPath, "templates", $"{templateName}.html");
////            return File.Exists(templatePath) ? File.ReadAllText(templatePath) : null;
////        }

////        public string RenderTemplate(string templateContent, Dictionary<string, string> variables)
////        {
////            return variables.Aggregate(templateContent,
////                (current, variable) => current.Replace("{{" + variable.Key + "}}", variable.Value));
////        }
////    }
////}


//using System.IO;
//using Microsoft.AspNetCore.Hosting;

//namespace CSCS_Web_Enzo_1.CSCS.Template
//{
//    public class TemplateService
//    {
//        private readonly IWebHostEnvironment _env;

//        public TemplateService(IWebHostEnvironment env)
//        {
//            _env = env;
//        }

//        public string LoadTemplate(string templateName)
//        {
//            var templatePath = Path.Combine(_env.WebRootPath, "templates", $"{templateName}.html");
//            return File.Exists(templatePath) ? File.ReadAllText(templatePath)
//                 : throw new FileNotFoundException($"Template {templateName} not found");
//        }

//        public string Render(string templateContent, Dictionary<string, object> variables)
//        {
//            foreach (var variable in variables)
//            {
//                templateContent = templateContent.Replace(
//                    $"{{{{{variable.Key}}}}}",
//                    variable.Value?.ToString() ?? string.Empty);
//            }
//            return templateContent;
//        }
//    }
//}