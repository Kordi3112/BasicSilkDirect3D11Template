using Silk.NET.Core.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class CrashHandler
    {
        public static string ErrorHandlerPath = "ErrorHandler.exe";


        internal static void CheckForThrow(int code, string msg = "")
        {
            HResult hResult = code;
            if (hResult.IsFailure)

                throw new Exception("Error code: " + code + " " + msg);
        }

        internal static void CheckForError(int code, string msg = "")
        {

            try
            {
                CheckForThrow(code);
            }
            catch (Exception e)
            {
                Error(e.Message + " " + msg);
            }

        }

        internal static void Error(string massage)
        {

            var processInfo = new ProcessStartInfo()
            {
                FileName = ErrorHandlerPath,
                Arguments = "\n \n " + massage,
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal,
            };

            Process.Start(processInfo);

#if DEBUG
            throw new Exception(massage);
#endif

            Environment.Exit(0);
        }


    }
}
