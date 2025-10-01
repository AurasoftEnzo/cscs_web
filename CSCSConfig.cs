namespace cscs_web
{
    public class CSCSConfig
    {
        public string SQLConnectionString { get; set; }
        public string ScriptsDirectory { get; set; }
        public string StartScript { get; set; }

        public string PreprocessTokens { get; set; }
        public string TemplatesDirectory { get; set; }
        public string ProgramsDirectory { get; set; }

        //public string PreprocessSripts { get; set; }

        public string StaticFilesDirectory { get; set; }

        public string JwtSecretKey { get; set; }
        public string CommonDB { get; set; }
    }
}
