using System;
using System.Linq;
using GMac.GMacCompiler.Semantic.ASTConstants;
using GMac.GMacUtils;
using TextComposerLib.Text.Linear;
using TextComposerLib.Text.Structured;

namespace BladesLibraryComposer.CSharp.BladeClass
{
    internal sealed class DpMethodsFileGenerator : BladesLibraryCodeFileGenerator 
    {
        internal string OperatorName { get; }


        internal DpMethodsFileGenerator(BladesLibrary libGen, string opName)
            : base(libGen)
        {
            OperatorName = opName;
        }


        private void GenerateMethods(int inGrade1, int inGrade2)
        {
            var gpCaseText = new ListComposer(Environment.NewLine);
            var gradesList = CurrentFrame.GradesOfEGp(inGrade1, inGrade2).OrderByDescending(grade => grade);

            foreach (var outGrade in gradesList)
            {
                var funcName = BladesLibraryGenerator.GetBinaryFunctionName(DefaultMacro.EuclideanBinary.GeometricProduct, inGrade1, inGrade2, outGrade);

                gpCaseText.Add(Templates["dp_case"],
                    "name", funcName,
                    "num", CurrentFrame.KvSpaceDimension(outGrade),
                    "frame", CurrentFrameName,
                    "grade", outGrade
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

        private void GenerateMainMethod()
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
                    GenerateMethods(grade1, grade2);

            GenerateMainMethod();

            GenerateBladeFileFinishCode();

            FileComposer.FinalizeText();
        }
    }
}
