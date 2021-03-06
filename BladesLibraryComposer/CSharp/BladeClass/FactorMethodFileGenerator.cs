﻿using System;
using GMac.GMacAPI.Binding;
using GMac.GMacAPI.CodeGen;
using GMac.GMacAPI.Target;
using GMac.GMacAST.Symbols;
using GMac.GMacUtils;
using TextComposerLib.Text.Linear;
using TextComposerLib.Text.Structured;

namespace BladesLibraryComposer.CSharp.BladeClass
{
    internal sealed class FactorMethodFileGenerator : BladesLibraryMacroCodeFileGenerator 
    {
        internal int InputGrade { get; }

        internal int InputId { get; }


        internal FactorMethodFileGenerator(BladesLibrary libGen, int inGrade, int inId, AstMacro gmacMacroInfo)
            : base(libGen, gmacMacroInfo)
        {
            InputGrade = inGrade;
            InputId = inId;
        }


        protected override void InitializeGenerator(GMacMacroCodeComposer macroCodeGen)
        {

        }

        protected override void SetMacroParametersBindings(GMacMacroBinding macroBinding)
        {
            macroBinding.BindMultivectorPartToVariables("B", InputGrade);

            var idx = 1;
            foreach (var basisVectorId in InputId.GetBasicPatterns())
            {
                var valueAccessName = "inputVectors.f" + idx + ".#E" + basisVectorId + "#";

                macroBinding.BindScalarToConstant(valueAccessName, 1);

                valueAccessName = "result.f" + idx + ".@G1@";

                macroBinding.BindToVariables(valueAccessName);

                idx++;
            }
        }

        protected override void SetTargetVariablesNames(GMacTargetVariablesNaming targetNaming)
        {
            BladesLibraryGenerator.SetBasisBladeToArrayNaming(targetNaming, "B", InputGrade, "coefs");

            for (var idx = 1; idx <= InputGrade; idx++)
            {
                var valueAccessName = "result.f" + idx + ".@G1@";

                var outputName = "vectors[" + (idx - 1) + "].C";

                targetNaming.SetMultivectorParameters(
                    valueAccessName, 
                    vectorId => outputName + (CurrentFrame.BasisBladeIndex(vectorId) + 1)
                    );
            }

            BladesLibraryGenerator.SetTargetTempVariablesNames(targetNaming);
        }

        public override void Generate()
        {
            GenerateBladeFileStartCode();

            var computationsText = GenerateComputationsCode();

            var newVectorsText = new ListComposer("," + Environment.NewLine);

            for (var i = 0; i < InputGrade; i++)
                newVectorsText.Add("new " + CurrentFrameName + "Vector()");

            TextComposer.AppendAtNewLine(
                Templates["factor"],
                "frame", CurrentFrameName,
                "id", InputId,
                "double", GMacLanguage.ScalarTypeName,
                "newvectors", newVectorsText,
                "computations", computationsText
                );

            GenerateBladeFileFinishCode();

            FileComposer.FinalizeText();
        }
    }
}
