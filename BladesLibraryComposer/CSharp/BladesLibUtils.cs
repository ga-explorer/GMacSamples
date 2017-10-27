using System.Text;

namespace BladesLibraryComposer.CSharp
{
    internal static class BladesLibUtils
    {
        internal static string CoefPart(this string name, object row, object column)
        {
            return
                new StringBuilder()
                .Append(name)
                .Append("[")
                .Append(row)
                .Append(", ")
                .Append(column)
                .Append("]")
                .ToString();
        }


    }
}
