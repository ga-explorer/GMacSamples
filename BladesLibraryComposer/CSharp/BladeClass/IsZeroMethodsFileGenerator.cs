using System;
using System.Linq;
using GMac.GMacUtils;
using TextComposerLib.Text.Linear;
using TextComposerLib.Text.Structured;

namespace BladesLibraryComposer.CSharp.BladeClass
{
    internal sealed class IsZeroMethodsFileGenerator : BladesLibraryCodeFileGenerator
    {
        internal IsZeroMethodsFileGenerator(BladesLibrary libGen)
            : base(libGen)
        {
        }


        private void GenerateIsZeroFunction(int kvSpaceDim)
        {
            var t1 = Templates["iszero"];
            var t2 = Templates["iszero_case"];
            var t3 = Templates["trimcoefs_case"];

            var iszeroCasesText = new ListComposer(" ||" + Environment.NewLine);
            var trimCoefsCasesText = new ListComposer("," + Environment.NewLine);

            for (var i = 0; i < kvSpaceDim; i++)
            {
                iszeroCasesText.Add(t2, "num", i);
                trimCoefsCasesText.Add(t3, "num", i);
            }

            TextComposer.AppendAtNewLine(
                t1,
                "num", kvSpaceDim,
                "double", GMacLanguage.ScalarTypeName,
                "iszero_case", iszeroCasesText,
                "trimcoefs_case", trimCoefsCasesText
                );
        }

        private void GenerateMainIsZeroFunction()
        {
            var t1 = Templates["main_iszero"];
            var t2 = Templates["main_iszero_case"];
            var t3 = Templates["main_trimcoefs_case"];

            var iszeroCasesText = new ListComposer(Environment.NewLine);
            var trimcoefsCasesText = new ListComposer(Environment.NewLine);

            foreach (var grade in CurrentFrame.Grades())
            {
                iszeroCasesText.Add(t2,
                    "grade", grade,
                    "num", CurrentFrame.KvSpaceDimension(grade)
                    );

                trimcoefsCasesText.Add(t3,
                    "frame", CurrentFrameName,
                    "grade", grade,
                    "num", CurrentFrame.KvSpaceDimension(grade)
                    );
            }

            TextComposer.AppendAtNewLine(t1,
                "frame", CurrentFrameName,
                "main_iszero_case", iszeroCasesText,
                "main_trimcoefs_case", trimcoefsCasesText
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
                GenerateIsZeroFunction(kvSpaceDim);

            GenerateMainIsZeroFunction();

            GenerateBladeFileFinishCode();

            FileComposer.FinalizeText();
        }
    }
}
