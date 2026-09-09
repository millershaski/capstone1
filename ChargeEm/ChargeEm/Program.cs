// FILENAME: Program.cs
//
// WRITTEN BY: Tyler J. Millershaski
// DATE CREATED: 06 Sep 2026
//
// PART OF PROJECT: ChargeEm
//
// PROJECT PURPOSE:
//   The purpose of this project is to provide a simple and easy-to-use application for generating quotes for charging services. It allows users to input their service details and receive a quote based on the provided information.
//
// FILE PURPOSE:
//   This is the entry point for the application start up.
//
// COMPILATION NOTES:
//   This project compiled normally in Visual Studio 2022 with .NET 6.0 SDK installed. It may require additional dependencies or configurations to compile successfully in other environments.
//
// LIBRARIES AND 3RD PARTY DEPENDENCIES:
//   Microsoft.NetCore.App (included with .NET 6.0 SDK)
//   Microsoft.WindowsDesktop.App (included with .NET 6.0 SDK)
//
// COMMAND LINE PARAMETER LIST (in Parameter Order):
//   (None)
//
// ENVIRONMENTAL RETURNS:
//    (Nothing)
//
// SAMPLE INVOCATION:
//   This project can be launched from Visual Studio 2022 or by running the compiled executable file directly. No command line parameters are required. (note that the name of the executable file may vary based on the project configuration and build settings beyond the scope of this file.)
//
// GLOBAL VARIABLE LIST (Alphabetically):
//   (None)
//
// MODIFICATION HISTORY:
// WHO     		WHEN         	WHAT
// Millershaski 06 Sep 2026 	Initial Version
namespace ChargeEm
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]

        //  METHOD NAME: Main
        //  
        //  METHOD PURPOSE:
        //   This is the main entry point for the application start up.
        //  	 
        //  PARAMETERS LIST (in Parameter Order):
        //   (None)
        //  	
        //  RETURNS:
        //   (Nothing)
        //  	
        //  LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //   (None)
        //  
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
