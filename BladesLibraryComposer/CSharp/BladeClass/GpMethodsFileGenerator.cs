using System;
using GMac.GMacUtils;
using TextComposerLib.Text.Linear;
using TextComposerLib.Text.Structured;

namespace BladesLibraryComposer.CSharp.BladeClass
{
    internal sealed class GpMethodsFileGenerator : BladesLibraryCodeFileGenerator 
    {
        internal string OperatorName { get; }


        internal GpMethodsFileGenerator(BladesLibrary libGen, string opName)
            : base(libGen)
        {
            OperatorName = opName;
        }


        internal void GenerateMainMethod()
        {
            var caseTemplate = Templates["gp_main_case"];

            var casesText = new ListComposer(Environment.NewLine);

            foreach (var inGrade1 in CurrentFrame.Grades())
                foreach (var inGrade2 in CurrentFrame.Grades())
                {
                    var id = inGrade1 + inGrade2 * CurrentFrame.GradesCount;

                    var name = BladesLibraryGenerator.GetBinaryFunctionName(OperatorName, inGrade1, inGrade2);

                    casesText.Add(
                        caseTemplate,
                        "name", name,
                        "id", id,
                        "g1", inGrade1,
                        "g2", inGrade2,
                        "frame", CurrentFrameName
                        );
                }

            TextComposer.AppendAtNewLine(
                Templates["gp_main"],
                "name", OperatorName,
                "frame", CurrentFrameName,
                "cases", casesText
                );
        }

        internal void GenerateIntermediateMethod(string gpCaseText, string name)
        {
            TextComposer.AppendAtNewLine(
                Templates["gp"],
                "frame", CurrentFrameName,
                "name", name,
                "double", GMacLanguage.ScalarTypeName,
                "gp_case", gpCaseText
                );
        }


        public override void Generate()
        {
            throw new NotImplementedException();
        }
    }
}
