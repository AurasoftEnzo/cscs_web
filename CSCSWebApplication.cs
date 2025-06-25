
using Microsoft.Data.SqlClient;
using SplitAndMerge;

namespace CSCS_Web_Enzo_1
{
    public static class CSCSWebApplication
    {
        static CSCSConfig CSCSConfig { get; set; }
        
        public static WebApplication WebApplication { get; set; }
        public static Interpreter Interpreter { get; set; }
        
        static CSCS_SQL CSCS_SQL { get; set; }
        static CSCSWebFunctions CSCSWebFunctions { get; set; }

        public static void Initialize(WebApplication app, CSCSConfig cscsConfig)
        {
            CSCSConfig = cscsConfig;

            WebApplication = app;
            
            Interpreter = new Interpreter();

            //Interpreter.RegisterFunction("CreateEndpoint", new CreateEndpointFunction(appIndex));


            CSCS_SQL.ConnectionString = CSCSConfig.SQLConnectionString;
            CSCS_SQL = new CSCS_SQL();
            CSCS_SQL.SqlServerConnection = new SqlConnection(CSCS_SQL.ConnectionString);
            CSCS_SQL.Init(Interpreter);

            CSCSWebFunctions = new CSCSWebFunctions();
            CSCSWebFunctions.Init(Interpreter);


            //skriptu izvršit
            Console.WriteLine(ExecuteScript(CSCSConfig.StartScript));


            //start server
            WebApplication.Run();
        }


        static string ExecuteScript(string fileName)
        {
            try
            {
                Variable result = runScript(Path.Combine(CSCSConfig.ScriptsDirectory, fileName));
                return result?.AsString();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }


        static Variable runScript(string fileName)
        {
            string script = Utils.GetFileContents(fileName);

            PreprocessScripts();

            //preprocess this file
            var tokenSet = GetPreprocessTokens();
            var scriptsDirStr = CSCSConfig.ScriptsDirectory; // App.GetConfiguration("ScriptsPath", "");
            var split2 = Utils.PreprocessScriptFile(fileName, tokenSet, scriptsDirStr);

            Variable result = null;
            return Interpreter.Process(split2, fileName, false);

        }

        static void PreprocessScripts()
        {
            var filesStr = ""; // App.GetConfiguration("PreprocessFiles", "");
            var tokenSet = GetPreprocessTokens();
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

        static HashSet<string> GetPreprocessTokens()
        {
            var tokenSet = new HashSet<string>();
            var tokensStr = "include,startdebugger,function,csfunction,add_comp_namespace,add_comp_definition,dllfunction,dllsub,importdll,define"; // App.GetConfiguration("PreprocessTokens", "");
            var doPreprocess = "true"; // App.GetConfiguration("Preprocess", "");
            if (string.IsNullOrWhiteSpace(tokensStr) ||
                !string.Equals(doPreprocess, "true", StringComparison.OrdinalIgnoreCase))
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
    }
}
