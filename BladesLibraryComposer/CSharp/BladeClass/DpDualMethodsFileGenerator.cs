using System;
using System.Linq;
using GMac.GMacAST.Symbols;
using GMac.GMacCompiler.Semantic.ASTConstants;
using GMac.GMacUtils;
using TextComposerLib.Text.Linear;
using TextComposerLib.Text.Structured;

namespace BladesLibraryComposer.CSharp.BladeClass
{
    internal sealed class DpDualMethodsFileGenerator : BladesLibraryCodeFileGenerator 
    {
        internal string OperatorName { get; }

        internal AstMacro EgpDualMacro { get; }


        internal DpDualMethodsFileGenerator(BladesLibrary libGen, string opName)
            : base(libGen)
        {
            OperatorName = opName;

            EgpDualMacro = CurrentFrame.Macro(DefaultMacro.EuclideanBinary.GeometricProductDual);
        }


        private void GenerateDeltaProductDualFunctions(int inGrade1, int inGrade2)
        {
            var gpCaseText = new ListComposer(Environment.NewLine);
            var gradesList =
                CurrentFrame.GradesOfEGp(inGrade1, inGrade2)
                .OrderByDescending(grade => grade);

            foreach (var outGrade in gradesList)
            {
                var invGrade = CurrentFrame.VSpaceDimension - outGrade;

                var funcName = BladesLibraryGenerator.GetBinaryFunctionName(EgpDualMacro.Name, inGrade1, inGrade2, invGrade);

                gpCaseText.Add(Templates["dp_case"],
                    "name", funcName,
                    "num", CurrentFrame.KvSpaceDimension(outGrade),
                    "frame", CurrentFrameName,
                    "grade", invGrade
                    );
            }

            TextComposer.AppendAtNewLine(
                Templates["dp"],
                "frame", CurrentFrameName,
                "name", BladesLibraryGenerator.GetBinaryFunctionName(OperatorName, inGrade1, inGrade2),
                "double", GMacLanguage.ScalarTypeName,
                "dp_case", gpCaseText
                );
        }

        private void GenerateMainDeltaProductDualFunction()
        {
            var casesText = new ListComposer(Environment.NewLine);

            foreach (var inGrade1 in CurrentFrame.Grades())
                foreach (var inGrade2 in CurrentFrame.Grades())
                {
                    var id = inGrade1 + inGrade2 * CurrentFrame.GradesCount;

                    casesText.Add(Templates["dp_main_case"],
                        "name", BladesLibraryGenerator.GetBinaryFunctionName(OperatorName, inGrade1, inGrade2),
                        "id", id,
                        "g1", inGrade1,
                        "g2", inGrade2,
                        "frame", CurrentFrameName
                        );
                }

            TextComposer.AppendAtNewLine(
                Templates["dp_main"],
                "name", OperatorName,
                "frame", CurrentFrameName,
                "cases", casesText
                );
        }

        public override void Generate()
        {
            GenerateBladeFileStartCode();

            foreach (var grade1 in CurrentFrame.Grades())
                foreach (var grade2 in CurrentFrame.Grades())
                    GenerateDeltaProductDualFunctions(grade1, grade2);

            GenerateMainDeltaProductDualFunction();

            GenerateBladeFileFinishCode();

            FileComposer.FinalizeText();
        }
    }
}
