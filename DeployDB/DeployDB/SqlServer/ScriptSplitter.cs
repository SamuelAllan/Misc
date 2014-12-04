using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployDB.SqlServer
{
    public static class ScriptSplitter
    {
        /// <summary>
        /// It's common to use "GO" as a break in scripts in Sql Server Management Studio.
        /// 
        /// This method splits scripts into parts based on "GO", with the added constraint
        /// that GO must appear on its own line, as the only non-whitespace.
        /// </summary>
        public static List<string> Split(string script)
        {
            List<string> result = new List<string>();
            StringBuilder partBuilder = new StringBuilder();

            foreach (string line in SplitIntoLines(script))
            {
                if (line.Trim().ToUpperInvariant() == "GO")
                {
                    string part = partBuilder.ToString();
                    if (!string.IsNullOrWhiteSpace(part))
                        result.Add(part);

                    partBuilder.Clear();
                }
                else
                {
                    if (partBuilder.Length > 0)
                        partBuilder.AppendLine();
                    partBuilder.Append(line);
                }
            }
            string lastPart = partBuilder.ToString();
            if (!string.IsNullOrWhiteSpace(lastPart))
                result.Add(lastPart);

            return result;
        }

        private static IEnumerable<string> SplitIntoLines(string script)
        {
            string[] separator = new[] { Environment.NewLine };
            return script.Split(separator, StringSplitOptions.None);
        }
    }
}
