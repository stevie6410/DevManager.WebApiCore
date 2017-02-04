namespace DevManager.WebApiCore.SqlServerDeploy
{
    public static class ExtMethods
    {
        public static string ToAlterScript(this string script, string objectType)
        {
            var _script = script;
            var from = "CREATE " + objectType;
            var to = "ALTER " + objectType;
            _script = _script.Replace(from, to);
            return _script;
        }

        public static string ToCreateScript(this string script, string objectType)
        {
            var _script = script;
            var from = "ALTER " + objectType;
            var to = "CREATE " + objectType;
            _script = _script.Replace(from, to);
            return _script;
        }
    }
}