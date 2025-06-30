using Microsoft.AspNetCore.Builder;
using SplitAndMerge;
using System.Net.Mime;
using System.Text;
using System.Xml.Linq;

using Microsoft.AspNetCore.Builder;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace CSCS_Web_Enzo_1
{
    public class CSCSWebFunctions
    {
        internal void Init(Interpreter interpreter)
        {
            interpreter.RegisterFunction("CreateEndpoint", new CreateEndpointFunction());
        }

        class CreateEndpointFunction : ParserFunction
        {
            private async Task<Variable> ExecScriptFunction(HttpContext context,
                string scriptFunctionName, string httpMethod)
            {
                // Prepare arguments for the CSCS script function
                //var args = new List<Variable>();

                // Create a request object containing all parts of the request
                var requestData = new Variable(Variable.VarType.ARRAY);

                //requestData.AddVariable(new Variable(httpMethod));
                requestData.AddVariable(new Variable(httpMethod));
                requestData.AddVariable(new Variable(context.Request.Path));
                // ???

                // Add route parameters
                var routeParams = new Variable(Variable.VarType.ARRAY);
                foreach (var (key, value) in context.Request.RouteValues)
                {
                    routeParams.SetHashVariable(key, new Variable(value?.ToString()));
                }
                requestData.AddVariable(routeParams);

                // Add query parameters
                var queryParams = new Variable(Variable.VarType.ARRAY);
                foreach (var (key, value) in context.Request.Query)
                {
                    queryParams.SetHashVariable(key, new Variable(value.ToString()));
                }
                requestData.AddVariable(queryParams);

                // Add headers
                var headers = new Variable(Variable.VarType.ARRAY);
                foreach (var (key, value) in context.Request.Headers)
                {
                    headers.SetHashVariable(key, new Variable(value.ToString()));
                }
                requestData.AddVariable(headers);

                // Add body (for POST/PUT)
                var body = Variable.EmptyInstance;
                if (context.Request.ContentLength > 0)
                {
                    try
                    {
                        using var reader = new StreamReader(context.Request.Body);
                        var bodyContent = await reader.ReadToEndAsync();
                        body = new Variable(bodyContent);
                    }
                    catch
                    {

                        // /* Handle error */ !!!!!!!

                    }
                }
                requestData.AddVariable(body);

                // Execute the CSCS script function with all request data
                //return CSCSWebApplication.Interpreter.Run(scriptFunctionName, new List<Variable> { requestData });
                return CSCSWebApplication.Interpreter.Run(scriptFunctionName, requestData);
            }

            protected override Variable Evaluate(ParsingScript script)
            {
                List<Variable> args = script.GetFunctionArgs();
                Utils.CheckArgs(args.Count, 3, m_name);

                var httpMethod = Utils.GetSafeString(args, 0).ToUpper();
                var endpointRoute = Utils.GetSafeString(args, 1);
                var scriptFunctionName = Utils.GetSafeString(args, 2).ToLower();

                switch (httpMethod)
                {
                    case "GET":
                        CSCSWebApplication.WebApplication.MapGet(endpointRoute,
                            async context => {
                                var result = await ExecScriptFunction(context, scriptFunctionName, httpMethod);
                                await ProcessResponse(context, result);
                            });
                        break;
                    case "POST":
                        CSCSWebApplication.WebApplication.MapPost(endpointRoute,
                            async context => {
                                var result = await ExecScriptFunction(context, scriptFunctionName, httpMethod);
                                await ProcessResponse(context, result);
                            });
                        break;
                    case "PUT":
                        CSCSWebApplication.WebApplication.MapPut(endpointRoute,
                            async context => {
                                var result = await ExecScriptFunction(context, scriptFunctionName, httpMethod);
                                await ProcessResponse(context, result);
                            });
                        break;
                    case "DELETE":
                        CSCSWebApplication.WebApplication.MapDelete(endpointRoute,
                            async context => {
                                var result = await ExecScriptFunction(context, scriptFunctionName, httpMethod);
                                await ProcessResponse(context, result);
                            });
                        break;
                    default:
                        throw new Exception($"Invalid HTTP method: {httpMethod}");
                }

                return Variable.EmptyInstance;
            }

            private async Task ProcessResponse(HttpContext context, Variable result)
            {
                if (result == null || result.Type == Variable.VarType.NONE)
                {
                    context.Response.StatusCode = 204; // No Content
                    return;
                }

                // Handle different response types
                //if (result.Type == Variable.VarType.ARRAY || result.Type == Variable.VarType.DICTIONARY)
                if (result.Type == Variable.VarType.ARRAY  /* || result.Type == Variable.VarType.DICTIONARY*/)
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(result.Tuple));
                }
                else if (result.Type == Variable.VarType.NUMBER)
                {
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync(result.AsString());
                }
                else // Default to string handling
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(result.AsString());
                }
            }
        }
    }

    
    //public class CSCSWebFunctions_WITH_MIDDLEWARE
    //{
    //    internal void Init(Interpreter interpreter)
    //    {
    //        interpreter.RegisterFunction("CreateEndpoint", new CreateEndpointFunction());
    //        interpreter.RegisterFunction("UseMiddleware", new UseMiddlewareFunction());
    //    }

    //    class CreateEndpointFunction : ParserFunction
    //    {
    //        private List<string> _currentMiddlewares = new List<string>();
    //        private string _currentEndpointFunction = string.Empty;

    //        // Add this method to support middleware registration
    //        public void AddMiddleware(string middlewareFunction)
    //        {
    //            if (!string.IsNullOrWhiteSpace(middlewareFunction))
    //            {
    //                _currentMiddlewares.Add(middlewareFunction.ToLower());
    //            }
    //        }

    //        protected override Variable Evaluate(ParsingScript script)
    //        {
    //            List<Variable> args = script.GetFunctionArgs();

    //            // Handle middleware registration case
    //            if (args.Count == 1 && !string.IsNullOrEmpty(_currentEndpointFunction))
    //            {
    //                var middlewareFunction = Utils.GetSafeString(args, 0).ToLower();
    //                _currentMiddlewares.Add(middlewareFunction);
    //                return Variable.EmptyInstance;
    //            }

    //            // Handle endpoint creation case
    //            Utils.CheckArgs(args.Count, 3, m_name, true);

    //            // Reset middleware chain for new endpoint
    //            _currentMiddlewares.Clear();
    //            var httpMethod = Utils.GetSafeString(args, 0).ToUpper();
    //            var endpointRoute = Utils.GetSafeString(args, 1);
    //            _currentEndpointFunction = Utils.GetSafeString(args, 2).ToLower();

    //            var routeHandler = BuildRouteHandler();

    //            switch (httpMethod)
    //            {
    //                case "GET":
    //                    CSCSWebApplication.WebApplication.MapGet(endpointRoute, routeHandler);
    //                    break;
    //                case "POST":
    //                    CSCSWebApplication.WebApplication.MapPost(endpointRoute, routeHandler);
    //                    break;
    //                case "PUT":
    //                    CSCSWebApplication.WebApplication.MapPut(endpointRoute, routeHandler);
    //                    break;
    //                case "DELETE":
    //                    CSCSWebApplication.WebApplication.MapDelete(endpointRoute, routeHandler);
    //                    break;
    //                default:
    //                    throw new Exception($"Invalid HTTP method: {httpMethod}");
    //            }

    //            return Variable.EmptyInstance;
    //        }

    //        private RequestDelegate BuildRouteHandler()
    //        {
    //            return async context =>
    //            {
    //                var requestData = CreateRequestData(context);

    //                // Execute all middlewares in order
    //                foreach (var middleware in _currentMiddlewares)
    //                {
    //                    var result = await CSCSWebApplication.Interpreter.RunAsync(
    //                        middleware,
    //                        new List<Variable> { requestData.Clone() });

    //                    if (result != null && result.Type != Variable.VarType.NONE)
    //                    {
    //                        // Middleware returned a response - stop processing
    //                        await ProcessResponse(context, result);
    //                        return;
    //                    }
    //                }

    //                // Execute main endpoint function if all middlewares passed
    //                var endpointResult = await CSCSWebApplication.Interpreter.RunAsync(
    //                    _currentEndpointFunction,
    //                    new List<Variable> { requestData });

    //                await ProcessResponse(context, endpointResult);
    //            };
    //        }

    //        private Variable CreateRequestData(HttpContext context)
    //        {
    //            var requestData = new Variable(Variable.VarType.ARRAY);

    //            // [0] HTTP Method
    //            requestData.Tuple.Add(new Variable(context.Request.Method));

    //            // [1] Path
    //            requestData.Tuple.Add(new Variable(context.Request.Path));

    //            // [2] Query Parameters (as dictionary)
    //            var queryParams = new Variable(Variable.VarType.DICTIONARY);
    //            foreach (var (key, value) in context.Request.Query)
    //            {
    //                queryParams.SetHashVariable(key, new Variable(value.ToString()));
    //            }
    //            requestData.Tuple.Add(queryParams);

    //            // [3] Route Parameters (as dictionary)
    //            var routeParams = new Variable(Variable.VarType.DICTIONARY);
    //            foreach (var (key, value) in context.Request.RouteValues)
    //            {
    //                if (key != null)
    //                {
    //                    routeParams.SetHashVariable(key, new Variable(value?.ToString()));
    //                }
    //            }
    //            requestData.Tuple.Add(routeParams);

    //            // [4] Headers (as dictionary)
    //            var headers = new Variable(Variable.VarType.DICTIONARY);
    //            foreach (var (key, value) in context.Request.Headers)
    //            {
    //                headers.SetHashVariable(key, new Variable(value.ToString()));
    //            }
    //            requestData.Tuple.Add(headers);

    //            // [5] Body (as string)
    //            var body = Variable.EmptyInstance;
    //            if (context.Request.ContentLength > 0)
    //            {
    //                try
    //                {
    //                    using var reader = new StreamReader(context.Request.Body);
    //                    var bodyContent = await reader.ReadToEndAsync();
    //                    body = new Variable(bodyContent);
    //                }
    //                catch { /* Handle error */ }
    //            }
    //            requestData.Tuple.Add(body);

    //            return requestData;
    //        }

    //        private async Task ProcessResponse(HttpContext context, Variable result)
    //        {
    //            if (result == null || result.Type == Variable.VarType.NONE)
    //            {
    //                context.Response.StatusCode = 204; // No Content
    //                return;
    //            }

    //            // Handle JSON responses
    //            if (result.Type == Variable.VarType.ARRAY ||
    //                result.Type == Variable.VarType.DICTIONARY)
    //            {
    //                context.Response.ContentType = "application/json";
    //                await context.Response.WriteAsync(JsonSerializer.Serialize(result.ToJson()));
    //            }
    //            // Handle string responses
    //            else if (result.Type == Variable.VarType.STRING)
    //            {
    //                // Detect if it's HTML
    //                var str = result.AsString();
    //                if (str.StartsWith("<") && str.EndsWith(">"))
    //                {
    //                    context.Response.ContentType = "text/html";
    //                }
    //                else
    //                {
    //                    context.Response.ContentType = "text/plain";
    //                }
    //                await context.Response.WriteAsync(str);
    //            }
    //            // Handle numeric responses
    //            else if (result.Type == Variable.VarType.NUMBER)
    //            {
    //                context.Response.ContentType = "text/plain";
    //                await context.Response.WriteAsync(result.AsString());
    //            }
    //            // Default case
    //            else
    //            {
    //                context.Response.ContentType = "text/plain";
    //                await context.Response.WriteAsync(result.AsString());
    //            }
    //        }
    //    }
    //    public class UseMiddlewareFunction : ParserFunction
    //    {
    //        protected override Variable Evaluate(ParsingScript script)
    //        {
    //            List<Variable> args = script.GetFunctionArgs();
    //            Utils.CheckArgs(args.Count, 1, m_name);

    //            var middlewareFunction = Utils.GetSafeString(args, 0).ToLower();

    //            // Get the current CreateEndpoint function instance
    //            var createEndpoint = script.Interpreter.GetFunction("CreateEndpoint") as CreateEndpointFunction;

    //            if (createEndpoint == null)
    //            {
    //                throw new Exception("CreateEndpoint function not found. UseMiddleware must be called after CreateEndpoint.");
    //            }

    //            createEndpoint.AddMiddleware(middlewareFunction);
    //            return Variable.EmptyInstance;
    //        }
    //    }
    //}

    //---------------------------------------------        
    //---------------------------------------------

    public class CSCSWebFunctions_Enzo_BEZ_AIja
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

    
}
