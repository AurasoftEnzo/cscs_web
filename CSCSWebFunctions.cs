using Microsoft.AspNetCore.Builder;
using SplitAndMerge;
using System.Net.Mime;
using System.Text;
using System.Xml.Linq;

namespace CSCS_Web_Enzo_1
{
    public class CSCSWebFunctions
    {
        //static WebApplication WebApplication { get; set; } = CSCSWebApplication.GetWebApplication();

        internal void Init(Interpreter interpreter)
        {
            interpreter.RegisterFunction("CreateEndpoint", new CreateEndpointFunction());
        }

        class CreateEndpointFunction : ParserFunction
        {
            private Variable ExecScriptFunction(string scriptFunctionName, List<Variable> args)
            {
                if(args == null || args.Count == 0)
                {
                    args = new List<Variable>() { new Variable(new List<Variable>() { new Variable(1), new Variable(2), new Variable(3), }) };
                }

                Variable result = CSCSWebApplication.Interpreter.Run(
                    scriptFunctionName  , args     /* null, new Variable(item), Variable.EmptyInstance, GetScript(win)*/
                    );
                return result;
            }


            protected override Variable Evaluate(ParsingScript script)
            {
                List<Variable> args = script.GetFunctionArgs();
                Utils.CheckArgs(args.Count, 3, m_name);

                //var endpointName = Utils.GetSafeString(args, 0).ToLower();
                var httpMethod = Utils.GetSafeString(args, 0).ToUpper();
                var endpointRoute = Utils.GetSafeString(args, 1);
                var scriptFunctionName = Utils.GetSafeString(args, 2).ToLower();

                switch (httpMethod)
                {
                    case "GET":
                        CSCSWebApplication.WebApplication.MapGet(endpointRoute,
                            () => new CustomHTMLResult(ExecScriptFunction(scriptFunctionName, null).AsString()));
                        break;
                    case "POST":
                        //CSCSWebApplication.WebApplication.MapPost(endpointRoute,
                        //    () => ExecScriptFunction(scriptFunctionName, null).AsString());
                        
                        CSCSWebApplication.WebApplication.MapPost(endpointRoute,
                            () => ExecScriptFunction(scriptFunctionName, null).AsString());
                        break;

                    // case "PUT":
                    //     script.SetHttpMethod(HttpMethod.PUT);
                    //     break;
                    // case "DELETE":
                    //     script.SetHttpMethod(HttpMethod.DELETE);
                    //     break;

                    default:
                        throw new Exception($"Invalid HTTP method: {httpMethod}");
                }

                return Variable.EmptyInstance;
            }
        }
    }

    class CustomHTMLResult : IResult
    {
        private readonly string _htmlContent;
        public CustomHTMLResult(string htmlContent)
        {
            _htmlContent = htmlContent;
        }
        public async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            httpContext.Response.ContentType = MediaTypeNames.Text.Html;
            httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(_htmlContent);
            await httpContext.Response.WriteAsync(_htmlContent);
        }
    }
}
