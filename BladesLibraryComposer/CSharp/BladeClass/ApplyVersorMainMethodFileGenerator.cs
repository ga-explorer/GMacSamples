using System;
using GMac.GMacUtils;
using TextComposerLib.Text.Linear;
using TextComposerLib.Text.Structured;

namespace BladesLibraryComposer.CSharp.BladeClass
{
    internal sealed class ApplyVersorMainMethodFileGenerator : BladesLibraryCodeFileGenerator 
    {
        internal string OperatorName { get; }


        internal ApplyVersorMainMethodFileGenerator(BladesLibrary libGen, string opName)
            : base(libGen)
        {
            OperatorName = opName;
        }


        public override void Generate()
        {
            GenerateBladeFileStartCode();

            var t2 = Templates["applyversor_main_case"];

            var casesText = new ListComposer(Environment.NewLine);

            foreach (var inGrade1 in CurrentFrame.Grades())
                foreach (var inGrade2 in CurrentFrame.Grades())
                {
                    var outGrade = inGrade2;

                    var id = inGrade1 + inGrade2 * CurrentFrame.GradesCount;

                    var name = BladesLibraryGenerator.GetBinaryFunctionName(OperatorName, inGrade1, inGrade2, outGrade);

                    casesText.Add(t2,
                        "name", name,
                        "id", id,
                        "g1", inGrade1,
                        "g2", inGrade2,
                        "grade", outGrade,
                        "frame", CurrentFrameName
                        );
                }

            TextComposer.AppendAtNewLine(
                Templates["applyversor_main"],
                "name", OperatorName,
                "frame", CurrentFrameName,
                "cases", casesText
                );

            GenerateBladeFileFinishCode();

            FileComposer.FinalizeText();
        }
    }
}
