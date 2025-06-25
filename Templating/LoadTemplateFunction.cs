//using SplitAndMerge;

//namespace CSCS_Web_Enzo_1.CSCS.Template
//{
//    //public class LoadTemplateFunction : ParserFunction
//    //{
//    //    private readonly TemplateService _templateService;

//    //    public LoadTemplateFunction(TemplateService templateService)
//    //    {
//    //        _templateService = templateService;
//    //    }

//    //    protected override Variable Evaluate(ParsingScript script)
//    //    {
//    //        var templateName = script.GetNextToken();
//    //        var content = _templateService.LoadTemplate(templateName);
//    //        return new Variable(content ?? string.Empty);
//    //    }
//    //}

//    public class LoadTemplateFunction : ParserFunction
//    {
//        private readonly TemplateService _templateService;

//        public LoadTemplateFunction(TemplateService templateService)
//        {
//            _templateService = templateService;
//        }

//        protected override Variable Evaluate(ParsingScript script)
//        {
//            var templateName = Utils.GetToken(script, Constants.NEXT_OR_END_ARRAY);
//            var content = _templateService.LoadTemplate(templateName);
//            return new Variable(content);
//        }
//    }
//}
