/*
By Xinghe Lu

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace Project4
{
	
    class Testexec
    {
		public bool startProcess(string process, string args)
    {
      process = Path.GetFullPath(process);
      Console.Write("\n  fileSpec - \"{0}\"", process);
      ProcessStartInfo psi = new ProcessStartInfo
      {
        FileName = process,
        Arguments = args,
        // set UseShellExecute to true to see child console, false hides console
        UseShellExecute = true
      };
      try
      {
        Process p = Process.Start(psi);
        return true;
      }
      catch(Exception ex)
      {
        Console.Write("\n  {0}", ex.Message);
        return false;
      }
    }
        static void Main(string[] args)
        {
			Console.Write("\n  current directory is: \"{0}\"", Directory.GetCurrentDirectory());
            Testexec ps = new Testexec();
			ps.startProcess("Server/bin/debug/Server.exe", "");
			Testexec ps3 = new Testexec();
			ps3.startProcess("GeneralClient/bin/debug/GeneralClient.exe", "/R http://localhost:8080/CommService /L http://localhost:8087/CommService");
			Testexec ps1 = new Testexec();
			ps1.startProcess("Client/bin/debug/Client.exe", "/R http://localhost:8080/CommService /L http://localhost:8085/CommService /Log Yes");
			Testexec ps2 = new Testexec();
			ps2.startProcess("Client/bin/debug/Client.exe", "/R http://localhost:8080/CommService /L http://localhost:8086/CommService /Log No");
			Testexec ps4 = new Testexec();
			ps4.startProcess("Client2/bin/debug/Client2.exe", "/R http://localhost:8080/CommService /L http://localhost:8090/CommService");
			Testexec ps5 = new Testexec();
			ps5.startProcess("Client2/bin/debug/Client2.exe", "/R http://localhost:8080/CommService /L http://localhost:8011/CommService");
			
			
			Console.Write("\n  press key to exit: ");
			Console.ReadKey();
        }
    }
}
