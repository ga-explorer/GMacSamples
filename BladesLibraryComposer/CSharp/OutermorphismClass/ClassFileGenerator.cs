﻿using System;
using GMac.GMacCompiler.Semantic.ASTConstants;
using GMac.GMacUtils;
using TextComposerLib.Text.Linear;
using TextComposerLib.Text.Structured;

namespace BladesLibraryComposer.CSharp.OutermorphismClass
{
    internal class ClassFileGenerator : BladesLibraryCodeFileGenerator
    {
        internal ClassFileGenerator(BladesLibrary libGen)
            : base(libGen)
        {
        }


        private string GenerateOutermorphismTranposeCode()
        {
            var codeText = new ListComposer(Environment.NewLine);

            for (var i = 0; i < CurrentFrame.VSpaceDimension; i++)
                for (var j = 0; j < CurrentFrame.VSpaceDimension; j++)
                    codeText.Add(
                        "coefs".CoefPart(i, j) + " = " + "Coefs".CoefPart(j, i) + ";"
                        );

            return codeText.ToString();
        }

        private string GenerateOutermorphismDeterminantCode(string opName)
        {
            var macroGenerator = CreateMacroCodeGenerator(opName);

            macroGenerator.ActionSetMacroParametersBindings =
                macroBinding =>
                {
                    macroBinding.BindToVariables("result");

                    for (var i = 0; i < CurrentFrame.VSpaceDimension; i++)
                    {
                        var id = CurrentFrame.BasisVectorId(i);

                        for (var j = 0; j < CurrentFrame.VSpaceDimension; j++)
                        {
                            var valueAccessName = "om.ImageV" + (j + 1) + ".#E" + id + "#";

                            macroBinding.BindToVariables(valueAccessName);
                        }
                    }
                };

            macroGenerator.ActionSetTargetVariablesNames =
                targetNaming =>
                {
                    targetNaming.SetScalarParameter("result", "det");

                    for (var i = 0; i < CurrentFrame.VSpaceDimension; i++)
                    {
                        var id = CurrentFrame.BasisVectorId(i);

                        for (var j = 0; j < CurrentFrame.VSpaceDimension; j++)
                        {
                            var varName = "Coefs".CoefPart(i, j);

                            var valueAccessName = "om.ImageV" + (j + 1) + ".#E" + id + "#";

                            targetNaming.SetScalarParameter(valueAccessName, varName);
                        }
                    }

                    BladesLibraryGenerator.SetTargetTempVariablesNames(targetNaming);
                };

            return macroGenerator.Generate();
        }

        private string GenerateOutermorphismPlusCode()
        {
            var codeText = new ListComposer(Environment.NewLine);

            for (var i = 0; i < CurrentFrame.VSpaceDimension; i++)
                for (var j = 0; j < CurrentFrame.VSpaceDimension; j++)
                    codeText.Add(
                        "coefs".CoefPart(i, j) + " = " +
                        "om1.Coefs".CoefPart(i, j) + " + " +
                        "om2.Coefs".CoefPart(i, j) + ";"
                        );

            return codeText.ToString();
        }

        private string GenerateOutermorphismSubtCode()
        {
            var codeText = new ListComposer(Environment.NewLine);

            for (var i = 0; i < CurrentFrame.VSpaceDimension; i++)
                for (var j = 0; j < CurrentFrame.VSpaceDimension; j++)
                    codeText.Add(
                        "coefs".CoefPart(i, j) + " = " +
                        "om1.Coefs".CoefPart(i, j) + " - " +
                        "om2.Coefs".CoefPart(i, j) + ";"
                        );

            return codeText.ToString();
        }

        private string GenerateOutermorphismComposeCode()
        {
            var codeText = new ListComposer(Environment.NewLine);

            var sumText = new ListComposer(" + ");

            for (var i = 0; i < CurrentFrame.VSpaceDimension; i++)
                for (var j = 0; j < CurrentFrame.VSpaceDimension; j++)
                {
                    sumText.Clear();

                    for (var k = 0; k < CurrentFrame.VSpaceDimension; k++)
                        sumText.Add(
                            "om1.Coefs".CoefPart(i, k) + " * " + "om2.Coefs".CoefPart(k, j)
                            );

                    codeText.Add(
                        "coefs".CoefPart(i, j) + " = " + sumText + ";"
                        );
                }

            return codeText.ToString();
        }

        private string GenerateOutermorphismTimesCode()
        {
            var codeText = new ListComposer(Environment.NewLine);

            for (var i = 0; i < CurrentFrame.VSpaceDimension; i++)
                for (var j = 0; j < CurrentFrame.VSpaceDimension; j++)
                    codeText.Add(
                        "coefs".CoefPart(i, j) + " = " +
                        "om.Coefs".CoefPart(i, j) + " * scalar;"
                        );

            return codeText.ToString();
        }

        private string GenerateOutermorphismDivideCode()
        {
            var codeText = new ListComposer(Environment.NewLine);

            for (var i = 0; i < CurrentFrame.VSpaceDimension; i++)
                for (var j = 0; j < CurrentFrame.VSpaceDimension; j++)
                    codeText.Add(
                        "coefs".CoefPart(i, j) + " = " +
                        "om.Coefs".CoefPart(i, j) + " / scalar;"
                        );

            return codeText.ToString();
        }

        private string GenerateOutermorphismNegativesCode()
        {
            var codeText = new ListComposer(Environment.NewLine);

            for (var i = 0; i < CurrentFrame.VSpaceDimension; i++)
                for (var j = 0; j < CurrentFrame.VSpaceDimension; j++)
                    codeText.Add(
                        "coefs".CoefPart(i, j) + " = " +
                        "-om.Coefs".CoefPart(i, j) + ";"
                        );

            return codeText.ToString();
        }

        private string GenerateOutermorphismApplyCasesCode()
        {
            var codeText = new ListComposer(Environment.NewLine);

            for (var inGrade = 1; inGrade <= CurrentFrame.VSpaceDimension; inGrade++)
                codeText.Add(
                    Templates["om_apply_code_case"],
                    "grade", inGrade,
                    "frame", CurrentFrameName
                    );

            return codeText.ToString();
        }

        public override void Generate()
        {
            GenerateOutermorphismFileStartCode();

            TextComposer.Append(
                Templates["outermorphism"],
                "frame", CurrentFrameName,
                "double", GMacLanguage.ScalarTypeName,
                "transpose_code", GenerateOutermorphismTranposeCode(),
                "metric_det_code", GenerateOutermorphismDeterminantCode(DefaultMacro.Outermorphism.MetricDeterminant),
                "euclidean_det_code", GenerateOutermorphismDeterminantCode(DefaultMacro.Outermorphism.EuclideanDeterminant),
                "plus_code", GenerateOutermorphismPlusCode(),
                "subt_code", GenerateOutermorphismSubtCode(),
                "compose_code", GenerateOutermorphismComposeCode(),
                "times_code", GenerateOutermorphismTimesCode(),
                "divide_code", GenerateOutermorphismDivideCode(),
                "negative_code", GenerateOutermorphismNegativesCode(),
                "apply_cases_code", GenerateOutermorphismApplyCasesCode()
                );

            GenerateOutermorphismFileFinishCode();

            FileComposer.FinalizeText();
        }
    }
}
