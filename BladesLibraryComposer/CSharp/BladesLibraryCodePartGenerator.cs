using GMac.GMacAPI.CodeGen;
using GMac.GMacAST.Symbols;

namespace BladesLibraryComposer.CSharp
{
    internal abstract class BladesLibraryCodePartGenerator : GMacCodePartComposer
    {
        internal AstFrame Frame { get; private set; }

        internal string FrameTargetName { get; private set; }

        internal BladesLibrary BladesLibraryGenerator => (BladesLibrary) LibraryComposer;


        internal BladesLibraryCodePartGenerator(BladesLibrary libGen)
            : base(libGen)
        {
            Frame = libGen.CurrentFrame;
            FrameTargetName = libGen.CurrentFrameName;
        }
    }
}
