using GMac.GMacCompiler.Semantic.ASTConstants;
using TextComposerLib.Text.Linear;

namespace BladesLibraryComposer.CSharp.BladeClass
{
    /// <summary>
    /// Generate the main code file for the blade class
    /// </summary>
    internal sealed class ClassFileGenerator : BladesLibraryCodeFileGenerator
    {
        internal ClassFileGenerator(BladesLibrary libGen)
            : base(libGen)
        {
        }


        public override void Generate()
        {
            GenerateBladeFileStartCode();

            TextComposer.Append(
                Templates["blade"],
                "frame", CurrentFrameName,
                "double", GMacLanguage.ScalarTypeName,
                "norm2_opname", DefaultMacro.MetricUnary.NormSquared
                );

            GenerateBladeFileFinishCode();

            FileComposer.FinalizeText();
        }
    }
}
