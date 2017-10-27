using System;
using GMac.GMacAPI.Binding;
using GMac.GMacAPI.CodeGen;
using GMac.GMacAPI.Target;
using GMac.GMacUtils;
using TextComposerLib.Text.Linear;
using TextComposerLib.Text.Structured;

namespace BladesLibraryComposer.CSharp.BladeClass
{
    /// <summary>
    /// This class gives an example of generating several related macros into a single file
    /// Before calling the protected GenerateComputationsCode() mathod we must call the protected
    /// SetBaseMacro() mathod to change the base macro
    /// </summary>
    internal sealed class NormMethodsFileGenerator : BladesLibraryMacroCodeFileGenerator
    {
        private int _inGrade;

        internal string[] OperatorNames { get; }


        internal NormMethodsFileGenerator(BladesLibrary libGen, params string[] operatorNames)
            : base(libGen)
        {
            OperatorNames = operatorNames;
        }


        protected override void InitializeGenerator(GMacMacroCodeComposer macroCodeGen)
        {

        }

        protected override void SetMacroParametersBindings(GMacMacroBinding macroBinding)
        {
            macroBinding.BindToVariables(macroBinding.BaseMacro.OutputParameterValueAccess);

            macroBinding.BindMultivectorPartToVariables("mv", _inGrade);
        }

        protected override void SetTargetVariablesNames(GMacTargetVariablesNaming targetNaming)
        {
            targetNaming.SetScalarParameter(targetNaming.BaseMacro.OutputParameterValueAccess, "result");

            BladesLibraryGenerator.SetBasisBladeToArrayNaming(targetNaming, "mv", _inGrade, "coefs");

            BladesLibraryGenerator.SetTargetTempVariablesNames(targetNaming);
        }

        private void GenerateNormFunction(string opName, int inGrade)
        {
            _inGrade = inGrade;

            SetBaseMacro(CurrentFrame.Macro(opName));

            var computationsCode = GenerateComputationsCode();

            TextComposer.AppendAtNewLine(
                Templates["norm"],
                "name", opName,
                "grade", inGrade,
                "double", GMacLanguage.ScalarTypeName,
                "computations", computationsCode
                );
        }

        private void GenerateMainNormFunction(string opName)
        {
            var caseTemplate = Templates["main_norm_case"];

            var casesText = new ListComposer(Environment.NewLine);

            foreach (var grade in CurrentFrame.Grades())
                casesText.Add(
                    caseTemplate,
                    "name", opName,
                    "grade", grade
                    );

            TextComposer.AppendAtNewLine(
                Templates["main_norm"],
                "name", opName,
                "double", GMacLanguage.ScalarTypeName,
                "main_norm_case", casesText
                );
        }

        public override void Generate()
        {
            GenerateBladeFileStartCode();

            foreach (var opName in OperatorNames)
            {
                GenerateBeginRegion(opName);

                foreach (var inGrade in CurrentFrame.Grades())
                    GenerateNormFunction(opName, inGrade);

                GenerateMainNormFunction(opName);

                GenerateEndRegion();
            }

            GenerateBladeFileFinishCode();

            FileComposer.FinalizeText();
        }
    }
}
