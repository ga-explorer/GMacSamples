using System;
using System.Linq;
using GMac.GMacUtils;
using TextComposerLib.Text.Linear;
using TextComposerLib.Text.Structured;

namespace BladesLibraryComposer.CSharp.BladeClass
{
    internal sealed class EqualsMethodsFileGenerator : BladesLibraryCodeFileGenerator
    {
        internal EqualsMethodsFileGenerator(BladesLibrary libGen)
            : base(libGen)
        {
        }


        private void GenerateEqualsFunction(int kvSpaceDim)
        {
            var caseTemplate = Templates["equals_case"];

            var casesText = new ListComposer(Environment.NewLine);

            for (var i = 0; i < kvSpaceDim; i++)
                casesText.Add(caseTemplate, "num", i);

            TextComposer.AppendAtNewLine(
                Templates["equals"],
                "num", kvSpaceDim,
                "double", GMacLanguage.ScalarTypeName,
                "cases", casesText
                );
        }

        private void GenerateMainEqualsFunction()
        {
            var caseTemplate = Templates["main_equals_case"];

            var casesText = new ListComposer(Environment.NewLine);

            foreach (var grade in CurrentFrame.Grades())
                casesText.Add(caseTemplate,
                    "grade", grade,
                    "num", CurrentFrame.KvSpaceDimension(grade)
                    );

            TextComposer.AppendAtNewLine(
                Templates["main_equals"],
                "frame", CurrentFrameName,
                "cases", casesText
                );
        }

        public override void Generate()
        {
            GenerateBladeFileStartCode();

            var kvSpaceDimList =
                Enumerable
                .Range(0, CurrentFrame.VSpaceDimension)
                .Select(grade => CurrentFrame.KvSpaceDimension(grade))
                .Distinct();

            foreach (var kvSpaceDim in kvSpaceDimList)
                GenerateEqualsFunction(kvSpaceDim);

            GenerateMainEqualsFunction();

            GenerateBladeFileFinishCode();

            FileComposer.FinalizeText();
        }

    }
}
