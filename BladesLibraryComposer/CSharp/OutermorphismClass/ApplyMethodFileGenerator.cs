using GMac.GMacAPI.Binding;
using GMac.GMacAPI.CodeGen;
using GMac.GMacAPI.Target;
using GMac.GMacCompiler.Semantic.ASTConstants;
using GMac.GMacUtils;
using TextComposerLib.Text.Linear;

namespace BladesLibraryComposer.CSharp.OutermorphismClass
{
    internal class ApplyMethodFileGenerator : BladesLibraryMacroCodeFileGenerator
    {
        internal int InputGrade { get; }


        internal ApplyMethodFileGenerator(BladesLibrary libGen, int inGrade)
            : base(libGen, DefaultMacro.Outermorphism.Apply)
        {
            InputGrade = inGrade;
        }


        protected override void InitializeGenerator(GMacMacroCodeComposer macroCodeGen)
        {

        }

        protected override void SetMacroParametersBindings(GMacMacroBinding macroBinding)
        {
            macroBinding.BindMultivectorPartToVariables("result", InputGrade);
            macroBinding.BindMultivectorPartToVariables("mv", InputGrade);

            for (var i = 0; i < CurrentFrame.VSpaceDimension; i++)
            {
                var id = CurrentFrame.BasisVectorId(i);

                for (var j = 0; j < CurrentFrame.VSpaceDimension; j++)
                {
                    var valueAccessName = "om.ImageV" + (j + 1) + ".#E" + id + "#";

                    macroBinding.BindToVariables(valueAccessName);
                }
            }
        }

        protected override void SetTargetVariablesNames(GMacTargetVariablesNaming targetNaming)
        {
            BladesLibraryGenerator.SetBasisBladeToArrayNaming(targetNaming, "result", InputGrade, "coefs");
            BladesLibraryGenerator.SetBasisBladeToArrayNaming(targetNaming, "mv", InputGrade, "bladeCoefs");

            for (var i = 0; i < CurrentFrame.VSpaceDimension; i++)
            {
                var id = CurrentFrame.BasisVectorId(i);

                for (var j = 0; j < CurrentFrame.VSpaceDimension; j++)
                {
                    var varName = "omCoefs".CoefPart(i, j);

                    var valueAccessName = "om.ImageV" + (j + 1) + ".#E" + id + "#";

                    targetNaming.SetScalarParameter(valueAccessName, varName);
                }
            }

            BladesLibraryGenerator.SetTargetTempVariablesNames(targetNaming);
        }

        public override void Generate()
        {
            GenerateOutermorphismFileStartCode();

            var computationsText = GenerateComputationsCode();

            TextComposer.Append(
                Templates["om_apply"],
                "double", GMacLanguage.ScalarTypeName,
                "grade", InputGrade,
                "num", CurrentFrame.KvSpaceDimension(InputGrade),
                "computations", computationsText
                );

            GenerateOutermorphismFileFinishCode();

            FileComposer.FinalizeText();
        }
    }
}
