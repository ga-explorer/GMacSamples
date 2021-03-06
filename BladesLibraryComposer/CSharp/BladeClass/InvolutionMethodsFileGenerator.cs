﻿using System;
using System.Linq;
using GMac.GMacCompiler.Semantic.ASTConstants;
using GMac.GMacUtils;
using TextComposerLib.Text.Linear;
using TextComposerLib.Text.Structured;

namespace BladesLibraryComposer.CSharp.BladeClass
{
    internal sealed class InvolutionMethodsFileGenerator : BladesLibraryCodeFileGenerator
    {
        internal InvolutionMethodsFileGenerator(BladesLibrary libGen)
            : base(libGen)
        {
        }


        private void GenerateNegativeFunction(int kvSpaceDim)
        {
            var caseTemplate = Templates["negative_case"];

            var casesText = new ListComposer("," + Environment.NewLine);

            for (var i = 0; i < kvSpaceDim; i++)
                casesText.Add(caseTemplate, "num", i);

            TextComposer.AppendAtNewLine(
                Templates["negative"],
                "num", kvSpaceDim,
                "double", GMacLanguage.ScalarTypeName,
                "cases", casesText
                );
        }

        private void GenerateMainInvolutionFunction(string macroName, Func<int, bool> useNegative)
        {
            var caseTemplate1 = Templates["main_negative_case"];
            var caseTemplate2 = Templates["main_negative_case2"];

            var casesText = new ListComposer(Environment.NewLine);

            foreach (var grade in CurrentFrame.Grades())
                if (useNegative(grade))
                    casesText.Add(caseTemplate1,
                        "frame", CurrentFrameName,
                        "grade", grade,
                        "num", CurrentFrame.KvSpaceDimension(grade)
                        );
                else
                    casesText.Add(caseTemplate2,
                        "grade", grade
                        );

            TextComposer.AppendAtNewLine(
                Templates["main_involution"],
                "frame", CurrentFrameName,
                "name", macroName,
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
                GenerateNegativeFunction(kvSpaceDim);

            GenerateMainInvolutionFunction(DefaultMacro.EuclideanUnary.Negative, grade => true);

            GenerateMainInvolutionFunction(DefaultMacro.EuclideanUnary.Reverse, FrameUtils.GradeHasNegativeReverse);

            GenerateMainInvolutionFunction(DefaultMacro.EuclideanUnary.GradeInvolution, FrameUtils.GradeHasNegativeGradeInv);

            GenerateMainInvolutionFunction(DefaultMacro.EuclideanUnary.CliffordConjugate, FrameUtils.GradeHasNegativeClifConj);

            GenerateBladeFileFinishCode();

            FileComposer.FinalizeText();
        }
    }
}
