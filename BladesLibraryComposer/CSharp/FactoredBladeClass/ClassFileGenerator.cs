using TextComposerLib.Text.Linear;

namespace BladesLibraryComposer.CSharp.FactoredBladeClass
{
    internal class ClassFileGenerator : BladesLibraryCodeFileGenerator
    {
        internal ClassFileGenerator(BladesLibrary libGen)
            : base(libGen)
        {
        }


        public override void Generate()
        {
            TextComposer.Append(
                Templates["factored_blade"],
                "frame", CurrentFrameName,
                "double", GMacLanguage.ScalarTypeName
                );

            FileComposer.FinalizeText();
        }
    }
}
