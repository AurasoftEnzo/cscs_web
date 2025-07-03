
using Microsoft.Data.SqlClient;
//using SplitAndMerge;


//**REMOVE**
using CSCSMath;
using SplitAndMerge;
using CSCS.InterpreterManager;

namespace CSCS_Web_Enzo_1
{
    public static class CSCSWebApplication
    {
        static CSCSConfig CSCSConfig { get; set; }
        
        public static WebApplication WebApplication { get; set; }
        public static Interpreter Interpreter { get; set; } = new Interpreter();



        // Interpreter Manager
        static InterpreterManagerModule InterpreterManagerModule { get; set; } = new InterpreterManagerModule();
        static InterpreterManagerInstance InterpreterManagerInstance { get; set; } = (InterpreterManagerInstance)InterpreterManagerModule.CreateInstance(Interpreter);

        // Sql
        static CscsSqlModule CscsSqlModule { get; set; } = new CscsSqlModule();
        static CscsSqlModuleInstance CscsSqlModuleInstance { get; set; } = (CscsSqlModuleInstance)CscsSqlModule.CreateInstance(Interpreter);

        // Math
        //static CscsMathModule CscsMathModule = new CSCSMath.CscsMathModule();
        static CscsMathModuleInstance CscsMathModuleInstance { get; set; } = (CscsMathModuleInstance) new CSCSMath.CscsMathModule().CreateInstance(Interpreter);



        static CSCSWebFunctions CSCSWebFunctions { get; set; } = new CSCSWebFunctions(Interpreter);
        //new CSCSWebFunctions();

        //CSCSWebFunctions = 
        //CSCSWebFunctions.Init(Interpreter);
        


        public static void Initialize(WebApplication serverApp, CSCSConfig cscsConfig)
        {
            //CscsMathModuleInstance CscsMathModuleInstance = (CscsMathModuleInstance)CscsMathModule.CreateInstance(Interpreter);

            CSCSConfig = cscsConfig;

            WebApplication = serverApp;



            //CSCS.InterpreterManager

            //// **REMOVE**
            //CscsMathModule CSCSMath = new CscsMathModule();
            //var CSCSMathInstance = CSCSMath.CreateInstance(Interpreter);




            //Interpreter.RegisterFunction("CreateEndpoint", new CreateEndpointFunction(appIndex));

            //SQL
            //new CscsSqlModule().CreateInstance


            //CSCS_SQL.ConnectionString = CSCSConfig.SQLConnectionString;
            //CSCS_SQL = new CSCS_SQL();
            //CSCS_SQL.SqlServerConnection = new SqlConnection(CSCS_SQL.ConnectionString);
            //CSCS_SQL.Init(Interpreter);

            


            //skriptu izvršit
            Console.WriteLine(
                RunStartScript(Path.Combine(CSCSConfig.ScriptsDirectory, CSCSConfig.StartScript))
                );


            //start server
            WebApplication.Run();
        }


        public static Variable RunStartScript(string fileName, bool encode = false)
        {
            //Init();

            if (encode)
            {
                //EncodeFileFunction.EncodeDecode(fileName, false);
            }
            string script = Utils.GetFileContents(fileName);
            if (encode)
            {
                //EncodeFileFunction.EncodeDecode(fileName, true);
            }

            //zakomentirat    (?)
            //PreprocessScripts();

            //preprocess this file
            var tokenSet = GetPreprocessTokens();
            var scriptsDirStr = CSCSConfig.ScriptsDirectory;
            Utils.PreprocessScriptFile(fileName, tokenSet, scriptsDirStr);

            Variable result = null;
            try
            {
                result = Interpreter.Process(script, fileName, true);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception: " + exc.Message);
                Console.WriteLine(exc.StackTrace);
                Interpreter.InvalidateStacksAfterLevel(0);
                var onException = CustomFunction.Run(Interpreter, Constants.ON_EXCEPTION, new Variable("Global Scope"),
                            new Variable(exc.Message), Variable.EmptyInstance);
                if (onException == null)
                {
                    throw;
                }
            }

            return result;
        }


        static void PreprocessScripts()
        {
            var filesStr = ""; // CSCSConfig.PreprocesFilesList ???    // CSCSConfig.StartScript;// App.GetConfiguration("PreprocessFiles", "");
            var tokenSet = GetPreprocessTokens();
            //var tokenSet = "include,startdebugger,function,csfunction,add_comp_namespace,add_comp_definition,dllfunction,dllsub,importdll,define";
            if (string.IsNullOrWhiteSpace(filesStr) || tokenSet.Count == 0)
            {
                return;
            }
            var scriptsDirStr = CSCSConfig.ScriptsDirectory; // App.GetConfiguration("ScriptsPath", "");

            var files = filesStr.Split(',');
            foreach (var file in files)
            {
                Utils.PreprocessScriptFile(file, tokenSet, scriptsDirStr);
            }
        }

        public static HashSet<string> GetPreprocessTokens()
        {
            var tokenSet = new HashSet<string>();
            var tokensStr = CSCSConfig.PreprocessTokens;
            var doPreprocess = true;     // App.GetConfiguration("Preprocess", "");
            if (string.IsNullOrWhiteSpace(tokensStr))
            {
                return tokenSet;
            }

            var tokens = tokensStr.Split(',');
            foreach (var token in tokens)
            {
                tokenSet.Add(token);
            }
            return tokenSet;
        }



        






        //static string ExecuteScript(string fileName)
        //{
        //    try
        //    {
        //        Variable result = runScript(Path.Combine(CSCSConfig.ScriptsDirectory, fileName));
        //        return result?.AsString();
        //    }
        //    catch (Exception ex)
        //    {
        //        return $"Error: {ex.Message}";
        //    }
        //}


        //static Variable runScript(string fileName)
        //{
        //    string script = Utils.GetFileContents(fileName);

        //    //string step2 = PreprocessScripts();

        //    //preprocess this file
        //    var tokenSet = GetPreprocessTokens();
        //    var scriptsDirStr = CSCSConfig.ScriptsDirectory; // App.GetConfiguration("ScriptsPath", "");
        //    string split2 = Utils.PreprocessScriptFile(fileName, tokenSet, scriptsDirStr);

        //    //Variable result = null;
        //    return Interpreter.Process(split2, fileName, false);

        //}

        //                //static string PreprocessScripts()
        //                //{
        //                //    var filesStr = CSCSConfig.PreprocessSripts; // App.GetConfiguration("PreprocessFiles", "");
        //                //    var tokenSet = GetPreprocessTokens();
        //                //    if (string.IsNullOrWhiteSpace(filesStr) || tokenSet.Count == 0)
        //                //    {
        //                //        return "";
        //                //    }
        //                //    var scriptsDirStr = CSCSConfig.ScriptsDirectory; // App.GetConfiguration("ScriptsPath", "");

        //                //    var files = filesStr.Split(',');
        //                //    foreach (var file in files)
        //                //    {
        //                //        string step2 = Utils.PreprocessScriptFile(file, tokenSet, scriptsDirStr);
        //                //        return step2;
        //                //    }
        //                //}

        //static HashSet<string> GetPreprocessTokens()
        //{
        //    var tokenSet = new HashSet<string>();
        //    var tokensStr = "include,startdebugger,function,csfunction,add_comp_namespace,add_comp_definition,dllfunction,dllsub,importdll,define"; // App.GetConfiguration("PreprocessTokens", "");
        //    var doPreprocess = "true"; // App.GetConfiguration("Preprocess", "");
        //    if (string.IsNullOrWhiteSpace(tokensStr) ||
        //        !string.Equals(doPreprocess, "true", StringComparison.OrdinalIgnoreCase))
        //    {
        //        return tokenSet;
        //    }

        //    var tokens = tokensStr.Split(',');
        //    foreach (var token in tokens)
        //    {
        //        tokenSet.Add(token);
        //    }
        //    return tokenSet;
        //}
    }
}
