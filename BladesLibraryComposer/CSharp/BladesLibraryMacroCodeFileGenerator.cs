﻿using GMac.GMacAPI.CodeGen;
using GMac.GMacAST.Symbols;
using TextComposerLib.Text.Linear;

namespace BladesLibraryComposer.CSharp
{
    public abstract class BladesLibraryMacroCodeFileGenerator : GMacMacroCodeFileComposer
    {
        internal AstFrame CurrentFrame { get; }

        internal string CurrentFrameName { get; }

        internal BladesLibrary BladesLibraryGenerator => (BladesLibrary)LibraryComposer;


        internal BladesLibraryMacroCodeFileGenerator(BladesLibrary libGen)
            : base(libGen)
        {
            CurrentFrame = libGen.CurrentFrame;
            CurrentFrameName = libGen.CurrentFrameName;
        }

        internal BladesLibraryMacroCodeFileGenerator(BladesLibrary libGen, string baseMacroName)
            : base(libGen, libGen.CurrentFrame.Macro(baseMacroName))
        {
            CurrentFrame = libGen.CurrentFrame;
            CurrentFrameName = libGen.CurrentFrameName;
        }

        internal BladesLibraryMacroCodeFileGenerator(BladesLibrary libGen, AstMacro baseMacro)
            : base(libGen, baseMacro)
        {
            CurrentFrame = libGen.CurrentFrame;
            CurrentFrameName = libGen.CurrentFrameName;
        }


        internal void GenerateBladeFileStartCode()
        {
            TextComposer.AppendLine(
                Templates["blade_file_start"].GenerateUsing(CurrentFrameName)
                );

            TextComposer.IncreaseIndentation();
            TextComposer.IncreaseIndentation();
        }

        internal void GenerateBladeFileFinishCode()
        {
            TextComposer
                .DecreaseIndentation()
                .AppendLineAtNewLine("}")
                .DecreaseIndentation()
                .AppendLineAtNewLine("}");
        }

        internal void GenerateOutermorphismFileStartCode()
        {
            TextComposer.AppendLine(
                Templates["om_file_start"],
                "frame", CurrentFrameName,
                "grade", CurrentFrame.VSpaceDimension
                );

            TextComposer.IncreaseIndentation();
            TextComposer.IncreaseIndentation();
        }

        internal void GenerateOutermorphismFileFinishCode()
        {
            TextComposer
                .DecreaseIndentation()
                .AppendLineAtNewLine("}")
                .DecreaseIndentation()
                .AppendLineAtNewLine("}");
        }

        internal void GenerateBeginRegion(string regionText)
        {
            TextComposer.AppendLineAtNewLine(@"#region " + regionText);
        }

        internal void GenerateEndRegion()
        {
            TextComposer.AppendLineAtNewLine(@"#endregion");
        }
    }

}
