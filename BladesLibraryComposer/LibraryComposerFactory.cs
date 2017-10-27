using System.Collections.Generic;
using System.Windows.Forms;
using BladesLibraryComposer.CSharp;
using GMac.GMacAPI.CodeGen;
using GMac.GMacAST;
using GMac.GMacCompiler;
using GMac.GMacUtils;
using TextComposerLib.Files;
using TextComposerLib.Logs.Progress.UI;

namespace BladesLibraryComposer
{
    public static class LibraryComposerFactory
    {
        internal static List<string> GMacDslCodeList { get; } = new List<string>();


        static LibraryComposerFactory()
        {
            GMacDslCodeList.Add(@"
namespace geometry3d
frame e3d (e1, e2, e3) euclidean
");

            GMacDslCodeList.Add(@"
namespace geometry3d
frame cga5d (ep, e1, e2, e3, en) orthonormal '++++-'
"
                );
        }


        /// <summary>
        /// Compile given GMacDSL code into a GMacAST structure
        /// </summary>
        /// <param name="dslCode"></param>
        /// <returns></returns>
        private static AstRoot BeginCompilation(string dslCode)
        {
            //GMacSystemUtils.InitializeGMac();

            //Compile GMacDSL code into GMacAST
            var compiler = GMacProjectCompiler.CompileDslCode(dslCode, Application.LocalUserAppDataPath, "tempTask");

            //Reduce details of progress reporting during code composition
            compiler.Progress.DisableAfterNextReport = true;

            if (compiler.Progress.History.HasErrorsOrFailures)
            {
                //Compilation of GMacDSL code failed
                var formProgress = new FormProgress(compiler.Progress, null, null);
                formProgress.ShowDialog();

                return null;
            }

            //Compilation of GMacDSL code successful, return constructed GMacAST root
            return compiler.Root;
        }

        /// <summary>
        /// Given GMacDSL code this factory class compiles the code into a GMacAST structure and
        /// use the blades code library composer for C# to compose target C# code
        /// </summary>
        /// <param name="dslCode"></param>
        /// <param name="outputFolder"></param>
        /// <param name="generateMacros"></param>
        /// <param name="targetLanguageName"></param>
        /// <returns></returns>
        public static FilesComposer ComposeLibrary(string dslCode, string outputFolder, bool generateMacros, string targetLanguageName)
        {
            //Clear the progress log composer
            GMacSystemUtils.ResetProgress();

            //Compile GMacDSL code into a GMacAST structure
            var ast = BeginCompilation(dslCode);

            //If compilation fails return nothing
            if (ReferenceEquals(ast, null)) return null;

            //Create and initialize code library composer for C#
            GMacCodeLibraryComposer activeGenerator;

            //Select the composer based on the target language name
            switch (targetLanguageName)
            {
                case "C#":
                    activeGenerator = new BladesLibrary(ast);
                    break;

                default:
                    activeGenerator = new BladesLibrary(ast);
                    break;
            }

            //Set the output folder for generated files
            activeGenerator.CodeFilesComposer.RootFolder = outputFolder;

            //Select option for generating macros code, this takes the longest time
            //in the composition process and may be skipped initially while designing
            //structure of composed library
            activeGenerator.MacroGenDefaults.AllowGenerateMacroCode = generateMacros;

            //Specify GMacAST frames to be used for code compositions
            activeGenerator.SelectedSymbols.SetSymbols(ast.Frames);

            //Start code composition process and display its progress
            var formProgress = new FormProgress(activeGenerator.Progress, activeGenerator.Generate, null);
            formProgress.ShowDialog();

            //Save all generated files
            //activeGenerator.CodeFilesComposer.SaveToFolder();

            //Return generated folders\files as a FilesComposer object
            return activeGenerator.CodeFilesComposer;
        }
    }
}
