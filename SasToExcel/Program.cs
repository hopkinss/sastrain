using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SasService;


namespace SasToExcel
{
    class Program
    {
        static void Main(string[] args)
        {

            // the app expects a commandline argument to an excel file
            // SAS can only see the network because of how it's configured so no local files
            var xlsIn = args[0];

            var sas = new SasSubmit( ConfigurationManager.AppSettings["appserver"],
                                    ConfigurationManager.AppSettings["host"],
                                    ConfigurationManager.AppSettings["port"]);

            // build a SAS program
            var cmd = new StringBuilder();
            cmd.AppendLine($"libname out \"{Path.GetDirectoryName(xlsIn)}\"; ");
            cmd.AppendLine($"proc import datafile=\"{args[0]}\" out=out.junk dbms=xlsx replace;run;");

            var log = sas.RunCode(cmd.ToString());

            using (var sw = new StreamWriter(Path.Combine(Path.GetDirectoryName(args[0]),
                                                        $"{Path.GetFileNameWithoutExtension(args[0])}.log" )))
            {
                foreach(var v in log)
                {
                    sw.WriteLine(v);
                }
            }
        }
    }
}
