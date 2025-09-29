using CSCS.InterpreterManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Primitives;
using SplitAndMerge;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace cscs_web
{
    public class CSCSWebFunctions
    {
        public CSCSWebFunctions(Interpreter interpreter)
        {
            Init(interpreter);
        }

        internal void Init(Interpreter interpreter)
        {
            interpreter.RegisterFunction("CreateEndpoint", new CreateEndpointFunction());
            interpreter.RegisterFunction("Response", new ResponseFunction());

            interpreter.RegisterFunction("ReadConfig", new ReadConfigFunction());
            //interpreter.RegisterFunction("TemplatesPath", new TemplatesPathFunction());
            //interpreter.RegisterFunction("ScriptsPath", new ScriptsPathFunction());

            interpreter.RegisterFunction("DeserializeJson", new DeserializeJsonFunction());
            interpreter.RegisterFunction("SerializeJson", new SerializeJsonFunction());
            interpreter.RegisterFunction("Sql2Json", new Sql2JsonFunction());

            interpreter.RegisterFunction("LoadTemplate", new LoadTemplateFunction());
            interpreter.RegisterFunction("FillTemplateFromDictionary", new FillTemplateFromDictionaryFunction());
            interpreter.RegisterFunction("FillTemplatePlaceholder", new FillTemplatePlaceholderFunction());
            //interpreter.RegisterFunction("FillTemplateFromDEFINEs", new FillTemplateFromDEFINEsFunction());
            interpreter.RegisterFunction("RenderCondition", new RenderConditionFunction());
            interpreter.RegisterFunction("RenderHtml", new RenderHtmlFunction());

            interpreter.RegisterFunction("GetValueFromForm", new GetValueFromFormFunction());
            
            
            interpreter.RegisterFunction("Chain", new ChainFunction());

            interpreter.RegisterFunction("ExtractEndpoints", new ExtractEndpointsFunction());

            Constants.FUNCT_WITH_SPACE.Add("DLoc");
            interpreter.RegisterFunction("DLoc", new DLocCommand());
        }
    }

    class DLocCommand : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            // Expect: loc variable_name = value;
            script.MoveForwardIf(' ');

            // 1. Get the variable name
            string varName = Utils.GetToken(script, Constants.TOKEN_SEPARATION);
            Utils.CheckForValidName(varName, script);

            script.MoveForwardIf(' ');

            // 2. Expect '='
            if (script.Current != '=')
            {
                Utils.ThrowErrorMsg("Expected '=' after local variable name.", script, "loc");
            }
            script.Forward(); // skip '='
            script.MoveForwardIf(' ');

            // 3. Parse the value expression
            Variable value = Utils.GetItem(script);

            // 4. Register as local variable in the current stack level
            if (script.StackLevel != null)
            {
                script.InterpreterInstance.AddLocalVariable(new GetVarFunction(value), script, varName);
            }
            else
            {
                // Optionally, allow local scope outside functions (file scope)
                string scopeName = Path.GetFileName(script.Filename);
                script.InterpreterInstance.AddLocalScopeVariable(varName, scopeName, new GetVarFunction(value));
            }

            return value;
        }

        //public override string Description()
        //{
        //    return "Defines a local variable: loc variable_name = value;";
        //}
    }

    class ChainFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            try
            {
                List<Variable> args = script.GetFunctionArgs();
                Utils.CheckArgs(args.Count, 1, m_name);

                var scriptPath = Utils.GetSafeString(args, 0);
                //var requestVariable = Utils.GetSafeVariable(args, 1);

                var scriptContent = File.ReadAllText(scriptPath);

                var res = CSCSWebApplication.Interpreter.Process(scriptContent);
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ChainFunction exception: " + ex.Message);
                return new Variable("Server error.");
            }
        }
    }
    
    class ExtractEndpointsFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var scriptsDirectory = Utils.GetSafeString(args, 0);
            //var requestVariable = Utils.GetSafeVariable(args, 1);


            var files = Directory.GetFiles(scriptsDirectory, "*.cscs", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var lines = File.ReadAllLines(file);
                foreach (var line in lines)
                {
                    if (line.Trim().ToLower().StartsWith("//createendpoint("))
                    {
                        var lineToExecute = line.Trim();
                        lineToExecute = lineToExecute.Substring(2, lineToExecute.IndexOf(';') + 1 - 2);

                        CSCSWebApplication.Interpreter.Process(lineToExecute);
                    }
                }
            }

            return Variable.EmptyInstance;
        }
    }

    class ReadConfigFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var configKey = Utils.GetSafeString(args, 0).ToLower();

            switch (configKey)
            {
                case "sqlconnectionstring":
                    return new Variable(CSCSWebApplication.CSCSConfig.SQLConnectionString);

                case "scriptsdirectory":
                    return new Variable(CSCSWebApplication.CSCSConfig.ScriptsDirectory);
                case "templatesdirectory":
                    return new Variable(CSCSWebApplication.CSCSConfig.TemplatesDirectory);
                
                case "startscript":
                    return new Variable(CSCSWebApplication.CSCSConfig.StartScript);
                
                case "jwtsecretkey":
                    return new Variable(CSCSWebApplication.CSCSConfig.JwtSecretKey);
                
                case "commondb":
                    return new Variable(CSCSWebApplication.CSCSConfig.CommonDB);

                default:
                    Console.WriteLine($"Unknown config key: {configKey}");
                    return new Variable("");
            }
        }   
    }


    class GetValueFromFormFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            var formString = Utils.GetSafeString(args, 0);
            var keyName = Utils.GetSafeString(args, 1);

            var formParts = formString.Split("&");

            string keyValue = "";
            foreach (var pair in formParts)
            {
                var pairSplitted = pair.Split("=");
                if (pairSplitted[0] == keyName)
                {
                    keyValue = pairSplitted[1];
                    return new Variable(Uri.UnescapeDataString(keyValue));
                }
            }

            return Variable.EmptyInstance;
        }
    }


    class CreateEndpointFunction : ParserFunction
    {
        private async Task<Variable> ExecScriptFunctionAsync(HttpContext context,
            string scriptFunctionName, string httpMethod)
        {
            // Create a request object containing all parts of the request
            var requestData = new Variable(Variable.VarType.ARRAY);

            requestData.SetHashVariable("HttpMethod", new Variable(context.Request.Method));
            requestData.SetHashVariable("RequestPath", new Variable(context.Request.Path));
            
            // Add route parameters
            var routeParams = new Variable(Variable.VarType.ARRAY);
            foreach (var (key, value) in context.Request.RouteValues)
            {
                routeParams.SetHashVariable(key, new Variable(value?.ToString()));
            }
            requestData.SetHashVariable("RouteValues", routeParams);

            // Add query parameters
            var queryParams = new Variable(Variable.VarType.ARRAY);
            foreach (var (key, value) in context.Request.Query)
            {
                queryParams.SetHashVariable(key, new Variable(value.ToString()));
            }
            requestData.SetHashVariable("QueryParams", queryParams);

            // Add headers
            var headers = new Variable(Variable.VarType.ARRAY);
            foreach (var (key, value) in context.Request.Headers)
            {
                headers.SetHashVariable(key, new Variable(value.ToString()));
            }
            requestData.SetHashVariable("Headers", headers);

            // Add body
            var body = Variable.EmptyInstance;
            if (context.Request.ContentLength > 0)
            {
                try
                {
                    using var reader = new StreamReader(context.Request.Body);
                    var bodyContent = await reader.ReadToEndAsync();

                    requestData.SetHashVariable("Body", new Variable(bodyContent));
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    requestData.SetHashVariable("Body", Variable.EmptyInstance);
                }
            }
            else
            {
                requestData.SetHashVariable("Body", Variable.EmptyInstance);
            }


            // Execute the CSCS script function with all request data
            try
            {
                return CSCSWebApplication.Interpreter.Run(scriptFunctionName, requestData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ExecScriptFunction exception: " + ex.Message);
                return new Variable("Server error.");
            }
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
                            var result = await ExecScriptFunctionAsync(context, scriptFunctionName, httpMethod);
                            await ProcessResponse(context, result);
                        });
                    break;
                case "POST":
                    CSCSWebApplication.WebApplication.MapPost(endpointRoute,
                        async context => {
                            var result = await ExecScriptFunctionAsync(context, scriptFunctionName, httpMethod);
                            await ProcessResponse(context, result);
                        });
                    break;
                case "PUT":
                    CSCSWebApplication.WebApplication.MapPut(endpointRoute,
                        async context => {
                            var result = await ExecScriptFunctionAsync(context, scriptFunctionName, httpMethod);
                            await ProcessResponse(context, result);
                        });
                    break;
                case "DELETE":
                    CSCSWebApplication.WebApplication.MapDelete(endpointRoute,
                        async context => {
                            var result = await ExecScriptFunctionAsync(context, scriptFunctionName, httpMethod);
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
            if (result.Type == Variable.VarType.ARRAY)
            {
                List<string> keys = result.GetKeys();
                List<string> lowerKeys = keys.Select(p => p.ToLower()).ToList();

                if (lowerKeys.Contains("headers"))
                {
                    Variable headersVariable = result.GetVariable("headers");

                    // Set content type
                    Variable contentType = headersVariable.GetVariable("content-type");
                    if (contentType != null && contentType.Type == Variable.VarType.STRING)
                    {
                        context.Response.ContentType = contentType.AsString();
                    }
                    else
                    {
                        context.Response.ContentType = "text/plain"; // Default content type
                    }
                    
                    // Set Set-Cookie
                    Variable setCookie = headersVariable.GetVariable("set-cookie");
                    if (setCookie != null && setCookie.Type == Variable.VarType.STRING)
                    {
                        context.Response.Headers.Append("Set-Cookie", setCookie.String);// = contentType.AsString();
                    }


                    // status code
                    Variable statusCode = result.GetVariable("statusCode");
                    if (statusCode != null && statusCode.Type == Variable.VarType.NUMBER)
                    {
                        context.Response.StatusCode = (int)Math.Floor(statusCode.Value);
                    }
                    else
                    {
                        context.Response.StatusCode = 200; // Default status code
                    }

                    // body
                    Variable body = result.GetVariable("body");
                    if (body != null && body.Type == Variable.VarType.STRING)
                    {
                        var bodyString = body.AsString();
                        await context.Response.WriteAsync(bodyString);
                    }
                    else
                    {
                        await context.Response.WriteAsync("");
                    }
                }
            }
            else // Default to string handling
            {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(result.AsString());
            }
        }
    }
        
    class ResponseFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 3, m_name);
            
            var headers = Utils.GetSafeVariable(args, 0);
            var body = Utils.GetSafeVariable(args, 1);
            var statusCode = Utils.GetSafeVariable(args, 2);

            var finalObject = new Variable(Variable.VarType.ARRAY);

            finalObject.SetHashVariable("headers", headers);
            finalObject.SetHashVariable("body", body);
            finalObject.SetHashVariable("statusCode", statusCode);

            return finalObject;
        }
    }



    #region JSON (De)Serialization

    class SerializeJsonFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            Variable jsonVariable = Utils.GetSafeVariable(args, 0);

            // Deserialize the JSON string into a variable
            string serializedJson = SerializeJson(jsonVariable, 0);    

            // Return the deserialized variable
            return new Variable(serializedJson);
        }

        private string SerializeJson(Variable jsonVariable, int indent)
        {
            if (jsonVariable.Type != Variable.VarType.ARRAY)
            {
                throw new Exception("parameter must be an array/hash table");
            }

            StringBuilder jsonStringBuilder = new StringBuilder("");

            if(jsonVariable.Tuple.FirstOrDefault() == null)
            {
                return Indent(indent) + "[]";
            }
            else
            {
                var keysStrings = jsonVariable.GetKeys();

                bool isList = keysStrings.Count == 0;

                //if tuple is array -> opening json list
                if (isList)
                {
                    jsonStringBuilder.Append(Indent(indent));
                    jsonStringBuilder.Append("[");
                }
                //if tuple is hash table -> opening json object
                else //isObject
                {
                    jsonStringBuilder.Append(Indent(indent));
                    jsonStringBuilder.Append("{");
                }



                for (int itemIndex = 0; itemIndex < jsonVariable.Tuple.Count; itemIndex++)
                {
                    Variable itemVariable = jsonVariable.Tuple[itemIndex];

                    string itemValue = "";

                    if (itemVariable.Type == Variable.VarType.ARRAY)
                    {
                        jsonStringBuilder.Append($"\n" + Indent(indent));
                        jsonStringBuilder.Append($"{(isList ? "" : $"\t\"{keysStrings.ElementAt(itemIndex)}\" : ") + SerializeJson(itemVariable, indent + 1)},");

                    }
                    else
                    {
                        switch (itemVariable.Type)
                        {
                            case Variable.VarType.NUMBER:
                            case Variable.VarType.INT:
                                itemValue = itemVariable.Value.ToString();
                                break;

                            case Variable.VarType.STRING:
                                itemValue = $"\"{itemVariable.String.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\t", "\\t")}\"";
                                break;

                            case Variable.VarType.DATETIME:
                                itemValue = $"\"{itemVariable.DateTime.ToString("yyyy-MM-dd")}\"";
                                break;

                            case Variable.VarType.NONE:
                            case Variable.VarType.UNDEFINED:
                            case Variable.VarType.OBJECT:
                                itemValue = "\"\"";
                                break;

                            case Variable.VarType.ARRAY_NUM:
                            case Variable.VarType.ARRAY_STR:
                            case Variable.VarType.ARRAY_INT:
                            case Variable.VarType.MAP_INT:
                            case Variable.VarType.MAP_NUM:
                            case Variable.VarType.MAP_STR:
                            case Variable.VarType.BYTE_ARRAY:
                            case Variable.VarType.QUIT:
                            case Variable.VarType.BREAK:
                            case Variable.VarType.CONTINUE:
                            case Variable.VarType.ENUM:
                            case Variable.VarType.VARIABLE:
                            case Variable.VarType.CUSTOM:
                            case Variable.VarType.POINTER:
                            default:
                                itemValue = "\"\"";
                                break;
                        }

                        //jsonStringBuilder.Append(Indent(indent));

                        jsonStringBuilder.Append("\n" + Indent(indent + 1) + $"{(isList ? "" : $"\"{keysStrings.ElementAt(itemIndex).Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\t", "\\t")}\" : ") + itemValue},");
                    }
                }


                //if tuple is array -> closing json list
                if (isList)
                {
                    jsonStringBuilder.Remove(jsonStringBuilder.Length - 1, 1);
                    jsonStringBuilder.Append("\n" + Indent(indent) + "]");
                }
                //if tuple is hash table -> closing json object
                else //isObject
                {
                    jsonStringBuilder.Remove(jsonStringBuilder.Length - 1, 1);
                    jsonStringBuilder.Append("\n" + Indent(indent) + "}");
                }
            }

            return jsonStringBuilder.ToString();
        }

        private string Indent(int amount)
        {
            string indent = "";
            for(int i = 0; i < amount; i++)
            {
                indent += "\t";
            }

            return indent;
        }
    }
    
    class Sql2JsonFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            Variable sqlResult = Utils.GetSafeVariable(args, 0);
            //bool trimStrings = script.get

            // Deserialize the JSON string into a variable
            string serializedJson = SerializeJsonFromCSCSSqlResult(sqlResult);    

            // Return the deserialized variable
            return new Variable(serializedJson);
        }

        private string SerializeJsonFromCSCSSqlResult(Variable sqlResult)
        {
            if(sqlResult == null || sqlResult.Tuple == null || sqlResult.Tuple.Count < 2)
            {
                return "[]"; // Return empty JSON array if no data
            }

            StringBuilder jsonBuilder = new StringBuilder("");

            List<Variable> headersTuple = sqlResult.Tuple[0].Tuple;
            List<string> headers = new List<string>();
            foreach (var headerVariable in headersTuple)
            {
                if (headerVariable.Type != Variable.VarType.STRING)
                {
                    throw new Exception("SQL result headers must be strings.");
                }

                headers.Add(headerVariable.String);
            }
            
            List<Variable> rowsTuple = sqlResult.Tuple.GetRange(1, sqlResult.Tuple.Count - 1);

            jsonBuilder.Append("[\n");

            foreach (var rowVariable in rowsTuple)
            {
                jsonBuilder.Append("\t{");

                for (int itemIndex = 0; itemIndex < rowVariable.Tuple.Count; itemIndex++)
                {
                    Variable itemVariable = rowVariable.Tuple[itemIndex];
                    string itemHeader = headers[itemIndex];
                    
                    string itemValue = null;
                    switch (itemVariable.Type)
                    {
                        case Variable.VarType.NUMBER:
                        case Variable.VarType.INT:
                            itemValue = itemVariable.Value.ToString();
                            break;

                        case Variable.VarType.STRING:
                            itemValue = $"\"{itemVariable.String}\"";
                            break;

                        case Variable.VarType.DATETIME:
                            itemValue = $"\"{itemVariable.DateTime.ToString("yyyy-MM-dd")}\"";
                            break;
                            
                        case Variable.VarType.NONE:
                        case Variable.VarType.UNDEFINED:
                        case Variable.VarType.OBJECT:
                            itemValue = "\"\"";
                            break;

                        case Variable.VarType.ARRAY:
                        case Variable.VarType.ARRAY_NUM:
                        case Variable.VarType.ARRAY_STR:
                        case Variable.VarType.ARRAY_INT:
                        case Variable.VarType.MAP_INT:
                        case Variable.VarType.MAP_NUM:
                        case Variable.VarType.MAP_STR:
                        case Variable.VarType.BYTE_ARRAY:
                        case Variable.VarType.QUIT:
                        case Variable.VarType.BREAK:
                        case Variable.VarType.CONTINUE:
                        case Variable.VarType.ENUM:
                        case Variable.VarType.VARIABLE:
                        case Variable.VarType.CUSTOM:
                        case Variable.VarType.POINTER:
                        default:
                            itemValue = "\"\"";
                            break;
                    }
                    
                    jsonBuilder.Append($"\t\t\"{itemHeader}\" : {itemValue},\n");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 2, 1);

                
                jsonBuilder.Append("\t},\n");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 2, 1);

            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }

    }
    
    
    class DeserializeJsonFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            string jsonString = Utils.GetSafeString(args, 0).Trim();

            // Deserialize the JSON string into a variable
            Variable deserializedVariable = ParseJSON(jsonString.Trim());

            // Return the deserialized variable
            return deserializedVariable;
        }

        private Variable ParseJSON(string json)
        {
            json = json.Trim();

            if (string.IsNullOrEmpty(json))
            {
                return new Variable(Variable.VarType.UNDEFINED);
            }

            // Handle null
            if (json == "null")
            {
                return new Variable(Variable.VarType.UNDEFINED);
            }

            // Handle boolean
            if (json == "true")
            {
                return new Variable(true);
            }
            if (json == "false")
            {
                return new Variable(false);
            }

            // Handle string
            if (json.StartsWith("\"") && json.EndsWith("\""))
            {
                string stringValue = json.Substring(1, json.Length - 2);
                // Unescape JSON string
                stringValue = stringValue.Replace("\\\"", "\"")
                                        .Replace("\\\\", "\\")
                                        .Replace("\\n", "\n")
                                        .Replace("\\r", "\r")
                                        .Replace("\\t", "\t");
                return new Variable(stringValue);
            }

            // Handle number
            // decimal point must be "."
            if (double.TryParse(json /*.Replace(".", ",")*/ , CultureInfo.InvariantCulture, out double numValue))
            {
                return new Variable(numValue);
            }

            // Handle array
            if (json.StartsWith("[") && json.EndsWith("]"))
            {
                return ParseJSONArray(json);
            }

            // Handle object
            if (json.StartsWith("{") && json.EndsWith("}"))
            {
                return ParseJSONObject(json);
            }

            // If we can't parse it, return as string
            return new Variable(json);
        }

        private Variable ParseJSONArray(string json)
        {
            Variable arrayVar = new Variable(Variable.VarType.ARRAY);

            // Remove brackets
            string content = json.Substring(1, json.Length - 2).Trim();

            if (string.IsNullOrEmpty(content))
            {
                return arrayVar;
            }

            List<string> elements = SplitJSONArray(content);

            foreach (string element in elements)
            {
                Variable elementVar = ParseJSON(element.Trim());
                arrayVar.AddVariable(elementVar);
            }

            return arrayVar;
        }

        private Variable ParseJSONObject(string json)
        {
            Variable objectVar = new Variable(Variable.VarType.ARRAY);

            // Remove braces
            string content = json.Substring(1, json.Length - 2).Trim();

            if (string.IsNullOrEmpty(content))
            {
                return objectVar;
            }

            List<string> pairs = SplitJSONObject(content);

            foreach (string pair in pairs)
            {
                int colonIndex = FindColonIndex(pair);
                if (colonIndex > 0)
                {
                    string key = pair.Substring(0, colonIndex).Trim();
                    string value = pair.Substring(colonIndex + 1).Trim();

                    // Remove quotes from key
                    if (key.StartsWith("\"") && key.EndsWith("\""))
                    {
                        key = key.Substring(1, key.Length - 2);
                    }

                    Variable valueVar = ParseJSON(value);
                    objectVar.SetHashVariable(key, valueVar);
                }
            }

            return objectVar;
        }

        private List<string> SplitJSONArray(string content)
        {
            List<string> elements = new List<string>();
            int bracketCount = 0;
            int braceCount = 0;
            int startIndex = 0;
            bool inString = false;
            bool escaped = false;

            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];

                if (escaped)
                {
                    escaped = false;
                    continue;
                }

                if (c == '\\' && inString)
                {
                    escaped = true;
                    continue;
                }

                if (c == '"')
                {
                    inString = !inString;
                }

                if (!inString)
                {
                    if (c == '[') bracketCount++;
                    else if (c == ']') bracketCount--;
                    else if (c == '{') braceCount++;
                    else if (c == '}') braceCount--;
                    else if (c == ',' && bracketCount == 0 && braceCount == 0)
                    {
                        elements.Add(content.Substring(startIndex, i - startIndex));
                        startIndex = i + 1;
                    }
                }
            }

            if (startIndex < content.Length)
            {
                elements.Add(content.Substring(startIndex));
            }

            return elements;
        }

        private List<string> SplitJSONObject(string content)
        {
            List<string> pairs = new List<string>();
            int bracketCount = 0;
            int braceCount = 0;
            int startIndex = 0;
            bool inString = false;
            bool escaped = false;

            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];

                if (escaped)
                {
                    escaped = false;
                    continue;
                }

                if (c == '\\' && inString)
                {
                    escaped = true;
                    continue;
                }

                if (c == '"')
                {
                    inString = !inString;
                }

                if (!inString)
                {
                    if (c == '[') bracketCount++;
                    else if (c == ']') bracketCount--;
                    else if (c == '{') braceCount++;
                    else if (c == '}') braceCount--;
                    else if (c == ',' && bracketCount == 0 && braceCount == 0)
                    {
                        pairs.Add(content.Substring(startIndex, i - startIndex));
                        startIndex = i + 1;
                    }
                }
            }

            if (startIndex < content.Length)
            {
                pairs.Add(content.Substring(startIndex));
            }

            return pairs;
        }

        private int FindColonIndex(string pair)
        {
            bool inString = false;
            bool escaped = false;

            for (int i = 0; i < pair.Length; i++)
            {
                char c = pair[i];

                if (escaped)
                {
                    escaped = false;
                    continue;
                }

                if (c == '\\' && inString)
                {
                    escaped = true;
                    continue;
                }

                if (c == '"')
                {
                    inString = !inString;
                }

                if (!inString && c == ':')
                {
                    return i;
                }
            }

            return -1;
        }
    }

    #endregion

    //--------------------------------------------------

    #region Templating HTMLs

    public static class HtmlTemplates
    {
        public static int templateHndl = 1;
        public static Dictionary<int, List<string>> TemplatesDictionary = new Dictionary<int, List<string>>();
    }

    class LoadTemplateFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var htmlTemplatePath = Utils.GetSafeString(args, 0);

            if (!File.Exists(htmlTemplatePath))
            {
                Console.WriteLine($"HTML template file not found: {htmlTemplatePath}");

                return new Variable(-1);
            }


            try
            {
                List<string> templateContent = File.ReadAllLines(htmlTemplatePath).ToList();

                HtmlTemplates.TemplatesDictionary[HtmlTemplates.templateHndl] = templateContent;

                return new Variable(HtmlTemplates.templateHndl++);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Reading template file threw an error: {ex.Message}");

                return new Variable(-2);
            }        
        }
    }

    public static class Placeholders
    {
        public static bool ReplaceAll(int templateHndl, string placeholderName, string newValue)
        {
            var tempHtmlLines = HtmlTemplates.TemplatesDictionary[templateHndl];

            List<string> relatedLines = tempHtmlLines.FindAll(p => p.Contains("{{" + placeholderName + "}}"));

            bool wasError = false;
            foreach (var line in relatedLines)
            {
                var lineIndex = tempHtmlLines.FindIndex(p => p.Contains("{{" + placeholderName + "}}"));
                if (lineIndex == -1)
                {
                    wasError = true;
                }

                string newLine = tempHtmlLines[lineIndex].Replace(@"{{" + placeholderName + "}}", newValue);

                HtmlTemplates.TemplatesDictionary[templateHndl][lineIndex] = newLine;
            }

            if (wasError)
            {
                throw new Exception($"Placeholder '{{{{{placeholderName}}}}}' not found in template with handle {templateHndl}.");
            }

            return true;
        }
    }

    class FillTemplatePlaceholderFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 3, m_name);

            var templateHndl = Utils.GetSafeInt(args, 0);
            var placeholderName = Utils.GetSafeString(args, 1);
            var newValue = Utils.GetSafeVariable(args, 2);

            Placeholders.ReplaceAll(templateHndl, placeholderName, newValue.AsString());

            //var tempHtmlLines = HtmlTemplates.TemplatesDictionary[templateHndl];

            //List<string> relatedLines = tempHtmlLines.FindAll(p => p.Contains("{{" + placeholderName + "}}"));

            //bool wasError = false;
            //foreach (var line in relatedLines)
            //{
            //    var lineIndex = tempHtmlLines.FindIndex(p => p.Contains("{{" + placeholderName + "}}"));
            //    if (lineIndex == -1)
            //    {
            //        wasError = true;
            //    }

            //    string newLine = tempHtmlLines[lineIndex].Replace(@"{{" + placeholderName + "}}", newValue.AsString());

            //    HtmlTemplates.TemplatesDictionary[templateHndl][lineIndex] = newLine;
            //}

            //if (wasError)
            //{
            //    throw new Exception($"Placeholder '{{{{{placeholderName}}}}}' not found in template with handle {templateHndl}.");
            //}

            return Variable.EmptyInstance;
            //return new Variable(wasError);
        }
    }

    class FillTemplateFromDictionaryFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            int htmlHndl = Utils.GetSafeInt(args, 0);
            Variable valuesDictionary = Utils.GetSafeVariable(args, 1);

            List<string> dictionaryKeys = valuesDictionary.GetKeys(); 
            List<Variable> dictionaryVariables = valuesDictionary.GetAllKeys(); 
           
            foreach (var key in dictionaryKeys)
            {
                Variable newValue = valuesDictionary.GetVariable(key);

                Placeholders.ReplaceAll(htmlHndl, key, newValue.AsString());
            }


            //List<string> htmlLines = HtmlTemplates.TemplatesDictionary[htmlHndl]


            //if (!File.Exists(htmlTemplatePath))
            //{
            //    Console.WriteLine($"HTML template file not found: {htmlTemplatePath}");
            //}



            //return new Variable(0);

            return Variable.EmptyInstance;
        }
    }

    class RenderConditionFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 3, m_name);

            int templateHndl = Utils.GetSafeInt(args, 0);   
            string conditionIdentifier = Utils.GetSafeString(args, 1);
            bool shouldDisplay = Utils.ConvertToBool(Utils.GetSafeInt(args, 2));


            var tempHtmlLines = HtmlTemplates.TemplatesDictionary[templateHndl];

            List<string> relatedLines = tempHtmlLines.FindAll(p => p.Contains("{{IF_" + conditionIdentifier + "}}"));

            if(relatedLines.Count != 2)
            {
                throw new Exception($"Condition '{{{{IF_{conditionIdentifier}}}}}' must have exactly 2 lines in the template with handle {templateHndl}.");
            }

            if (shouldDisplay)
            {
                foreach (var line in relatedLines)
                {
                    int lineIndex = tempHtmlLines.FindLastIndex(p => p.Contains("{{IF_" + conditionIdentifier + "}}"));
                    HtmlTemplates.TemplatesDictionary[templateHndl].RemoveAt(lineIndex);

                    lineIndex = tempHtmlLines.FindLastIndex(p => p.Contains("{{IF_" + conditionIdentifier + "}}"));
                }
            }
            else // remove template block
            {
                int firstIndex = tempHtmlLines.FindIndex(p => p.Contains("{{IF_" + conditionIdentifier + "}}"));
                HtmlTemplates.TemplatesDictionary[templateHndl].RemoveRange(
                    firstIndex,
                    tempHtmlLines.FindLastIndex(p => p.Contains("{{IF_" + conditionIdentifier + "}}")) - firstIndex + 1
                    );
            }
                        
            return Variable.EmptyInstance;
        }
    }


    class RenderHtmlFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            int htmlHndl = Utils.GetSafeInt(args, 0);
            
            List<string>? finalHtmlLines = new List<string>();
            if (!HtmlTemplates.TemplatesDictionary.TryGetValue(htmlHndl, out finalHtmlLines)){
                //Console.WriteLine($"Failed to retrieve HTML with handle {htmlHndl}!");
                throw new Exception($"Failed to retrieve HTML with handle {htmlHndl}!");
            }

            HtmlTemplates.TemplatesDictionary.Remove(htmlHndl);

            return new Variable(string.Join("\n", finalHtmlLines));
        }
    }

    #endregion
    
}
