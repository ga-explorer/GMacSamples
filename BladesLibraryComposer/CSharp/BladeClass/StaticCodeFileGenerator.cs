using System;
using GMac.GMacUtils;
using TextComposerLib.Text.Linear;
using TextComposerLib.Text.Parametric;
using TextComposerLib.Text.Structured;

namespace BladesLibraryComposer.CSharp.BladeClass
{
    /// <summary>
    /// This class generates a static utilities file for the blade class
    /// </summary>
    internal sealed class StaticCodeFileGenerator : BladesLibraryCodeFileGenerator 
    {
        internal StaticCodeFileGenerator(BladesLibrary libGen)
            : base(libGen)
        {
        }


        private string GenerateDeclarations(int grade)
        {
            var kvDim = CurrentFrame.KvSpaceDimension(grade);

            var template = Templates["static_basisblade_declare"];

            var declaresText = new ListComposer(Environment.NewLine);

            var coefsText = new ListComposer(", ");

            for (var index = 0; index < kvDim; index++)
            {
                coefsText.Clear();

                for (var i = 0; i < kvDim; i++)
                    coefsText.Add((i == index) ? "1.0D" : "0.0D");

                declaresText.Add(
                    template,
                    "frame", CurrentFrameName,
                    "id", CurrentFrame.BasisBladeId(grade, index),
                    "grade", grade,
                    "coefs", coefsText
                    );
            }

            declaresText.Add("");

            return declaresText.ToString();
        }

        private string GenerateBasisBladesNames(int grade)
        {
            var namesText = new ListComposer(", ") { ActiveItemSuffix = "\"", ActiveItemPrefix = "\"" };

            for (var index = 0; index < CurrentFrame.KvSpaceDimension(grade); index++)
                namesText.Add(
                    CurrentFrame.BasisBlade(grade, index).IndexedName
                    );

            return Templates["static_basisblade_name"].GenerateText("names", namesText);
        }

        public override void Generate()
        {
            GenerateBladeFileStartCode();

            var kvdimsText = new ListComposer(", ");
            var basisnamesText = new ListComposer("," + Environment.NewLine);
            var basisbladesText = new ListComposer(Environment.NewLine);

            foreach (var grade in CurrentFrame.Grades())
            {
                kvdimsText.Add(CurrentFrame.KvSpaceDimension(grade));

                basisnamesText.Add(GenerateBasisBladesNames(grade));

                basisbladesText.Add(GenerateDeclarations(grade));
            }

            TextComposer.Append(
                Templates["static"],
                "frame", CurrentFrameName,
                "grade", CurrentFrame.VSpaceDimension,
                "double", GMacLanguage.ScalarTypeName,
                "kvdims", kvdimsText,
                "basisnames", basisnamesText,
                "basisblades", basisbladesText
                );

            GenerateBladeFileFinishCode();

            FileComposer.FinalizeText();
        }
    }
}
