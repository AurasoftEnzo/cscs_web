//using SplitAndMerge;
//using System.Collections.Generic;

//namespace cscs_web.CSCS.Template
//{
//    //public class RenderTemplateFunction : ParserFunction
//    //{
//    //    private readonly TemplateService _templateService;

//    //    public RenderTemplateFunction(TemplateService templateService)
//    //    {
//    //        _templateService = templateService;
//    //    }

//    //    protected override Variable Evaluate(ParsingScript script)
//    //    {
//    //        var templateContent = script.GetNextToken();
//    //        var variables = new Dictionary<string, string>();

//    //        // Parse variables in format "var1=value1,var2=value2"
//    //        while (script.HasNext())
//    //        {
//    //            var assignment = script.GetNextToken();
//    //            var parts = assignment.Split('=');
//    //            if (parts.Length == 2)
//    //            {
//    //                variables[parts[0]] = parts[1];
//    //            }
//    //        }

//    //        var result = _templateService.RenderTemplate(templateContent, variables);
//    //        return new Variable(result);
//    //    }
//    //}

    
    
    
    
    
    
//    //public class RenderTemplateFunction : ParserFunction
//    //{
//    //    private readonly TemplateService _templateService;

//    //    public RenderTemplateFunction(TemplateService templateService)
//    //    {
//    //        _templateService = templateService;
//    //    }

//    //    protected override Variable Evaluate(ParsingScript script)
//    //    {
//    //        // Get template content (either direct string or from variable)
//    //        var templateVar = script.Execute(Constants.NEXT_ARG_ARRAY);
//    //        string templateContent = templateVar.String;

//    //        // Parse variables in format "key1=value1,key2=value2"
//    //        var variables = new Dictionary<string, object>();
//    //        while (script.TryCurrent() && !script.StartsWith(Constants.END_STATEMENT))
//    //        {
//    //            var pair = Utils.GetToken(script, Constants.NEXT_ARG_ARRAY);
//    //            var parts = pair.Split('=');
//    //            if (parts.Length == 2)
//    //            {
//    //                variables[parts[0].Trim()] = parts[1].Trim().Trim('"');
//    //            }
//    //        }

//    //        string result = _templateService.Render(templateContent, variables);
//    //        return new Variable(result);
//    //    }
//    //}
//}
