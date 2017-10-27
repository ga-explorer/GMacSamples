namespace BladesLibraryComposer.CSharp
{
    public sealed partial class BladesLibrary
    {
        #region GMacCode
        internal const string GMacCodeTemplates =
@"
delimiters < >

begin factor_struct_member
    f<num> : Multivector
end factor_struct_member

begin factor_struct
structure BladeFactorStruct(
    <members>
    )
end factor_struct

begin factor_macro_step
let final.f<num> = (inputVectors.f<num> lcp B) lcp B
let B = final.f<num> lcp B

end factor_macro_step

begin factor_macro
macro Factor<num>(B : Multivector, inputVectors : BladeFactorStruct) : BladeFactorStruct
begin
    declare final : BladeFactorStruct
    
    <steps>
    
    let final.f<num> = B
    
    return final
end
end factor_macro

begin vectors_op_macro
macro VectorsOP(<vop_inputs>) : Multivector
begin
    return <vop_expr>
end
end vectors_op_macro

";
        #endregion


        //        #region General
//        internal const string GeneralTemplates =
//@"
//delimiters #
//
//begin main_case1
//case #caseid#:
//    return #name##num#(Coefs);
//end main_case1
//
//begin main_case2
//case #caseid#:
//    return #name##num#(Coefs, #arg2#);
//end main_case2
//
//begin main_case3
//case #caseid#:
//    return new #frame#Blade(#grade#, #name##num#(Coefs));
//end main_case3
//
//begin main_case4
//case #caseid#:
//    return new #frame#Blade(#grade#, #name##num#(Coefs, #arg2#));
//end main_case4
//";
//        #endregion

        #region Blade
        internal const string BladeTemplates =
@"
delimiters #

begin blade_file_start
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace GMacBlade.#frame#
{
    /// <summary>
    /// This class represents an immutable blade in the #frame# frame with arbitrary grade 
    /// (i.e. grade is determined at runtime) based on additive representation of the blade as a 
    /// linear combination of basis blades of the same grade (i.e. it's actually a k-vector representation).
    /// </summary>
    public sealed partial class #frame#Blade
    {
end blade_file_start

begin blade
/// <summary>
/// Grade of blade.
/// </summary>
public int Grade { get; private set; }

/// <summary>
/// The k-vector space dimension of this blade (equals the length of the Coef array)
/// </summary>
public int KvSpaceDim { get { return GradeToKvSpaceDim[Grade]; } }

/// <summary>
/// Ordered coefficients of blade in the additive representation. 
/// </summary>
internal #double#[] Coefs { get; set; }


/// <summary>
/// This blade is a zero blade: it has no internal coefficients and its grade is any legal grade
/// This kind of blade should be treated separately in operations on blades
/// </summary>
public bool IsZeroBlade { get { return Coefs.Length == 0; } }

/// <summary>
/// True if this blade is a null blade
/// </summary>
public bool IsNull
{
    get
    {
        if (IsZeroBlade)
            return true;

        var c = #norm2_opname#;
        return !(c <= -Epsilon || c >= Epsilon);
    }
}

public bool IsScalar { get { return IsZeroBlade || Grade == 0; } }

public bool IsVector { get { return IsZeroBlade || Grade == 1; } }

public bool IsPseudoVector { get { return IsZeroBlade || Grade == MaxGrade - 1; } }

public bool IsPseudoScalar { get { return IsZeroBlade || Grade == MaxGrade; } }

public #double# this[int index] { get { return Coefs[index]; } }

public string[] BasisBladesNames { get { return BasisBladesNamesArray[Grade]; } }

/// <summary>
/// True if the coefficients represent a blade; not a general non-simple k-vector.
/// </summary>
public bool IsBlade { get { return SelfDPGrade() == 0; } }

/// <summary>
/// True if the coefficients represent a general non-simple k-vector; not a blade.
/// </summary>
public bool IsNonBlade { get { return SelfDPGrade() != 0; } }

/// <summary>
/// Create a blade and initialize its coefficients by the given array. Use this vary
/// carefully as the blade type is supposed to be immutable
/// </summary>
internal #frame#Blade(int grade, #double#[] coefs)
{
    if (coefs.Length != GradeToKvSpaceDim[grade])
        throw new ArgumentException(""The given array has the wrong number of items for this grade"", ""coefs"");

    Grade = grade;
    Coefs = coefs;
}

/// <summary>
/// Create a scalar blade (a grade-0 blade)
/// </summary>
public #frame#Blade(#double# scalar)
{
    Grade = 0;
    Coefs = new [] { scalar };
}

/// <summary>
/// Create the zero blade
/// </summary>
private #frame#Blade()
{
    Grade = 0;
    Coefs = new #double#[0];
}


/// <summary>
/// Test if this blade is of a given grade. A zero blade is assumed to have any grade
/// </summary>
public bool IsOfGrade(int grade)
{
    return Grade == grade || (grade >= 0 && grade <= MaxGrade && IsZero);
}

/// <summary>
/// If this blade is of grade 1 convert it to a vector
/// </summary>
/// <returns></returns>
public #frame#Vector ToVector()
{
    if (Grade == 1)
        return new #frame#Vector(Coefs);

    if (IsZero)
        return new #frame#Vector();

    throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
}


public static #frame#Blade Meet(#frame#Blade bladeA, #frame#Blade bladeB)
{
    //blade A1 is the part of A not in B
    var bladeA1 = bladeA.DPDual(bladeB).DP(bladeA);

    return bladeA1.ELCP(bladeB);
}

public static #frame#Blade Join(#frame#Blade bladeA, #frame#Blade bladeB)
{
    //blade A1 is the part of A not in B
    var bladeA1 = bladeA.DPDual(bladeB).DP(bladeA);

    return bladeA1.OP(bladeB);
}

public static void MeetJoin(#frame#Blade bladeA, #frame#Blade bladeB, out #frame#Blade bladeMeet, out #frame#Blade bladeJoin)
{
    //blade A1 is the part of A not in B
    var bladeA1 = bladeA.DPDual(bladeB).DP(bladeA);

    bladeMeet = bladeA1.ELCP(bladeB);
    bladeJoin = bladeA1.OP(bladeB);
}

public static void MeetJoin(#frame#Blade bladeA, #frame#Blade bladeB, out #frame#Blade bladeA1, out #frame#Blade bladeB1, out #frame#Blade bladeMeet)
{
    //blade A1 is the part of A not in B
    bladeA1 = bladeA.DPDual(bladeB).DP(bladeA);

    bladeMeet = bladeA1.ELCP(bladeB);

    bladeB1 = bladeMeet.ELCP(bladeB);
}

public static void MeetJoin(#frame#Blade bladeA, #frame#Blade bladeB, out #frame#Blade bladeA1, out #frame#Blade bladeB1, out #frame#Blade bladeMeet, out #frame#Blade bladeJoin)
{
    //blade A1 is the part of A not in B
    bladeA1 = bladeA.DPDual(bladeB).DP(bladeA);

    bladeMeet = bladeA1.ELCP(bladeB);
    bladeJoin = bladeA1.OP(bladeB);

    bladeB1 = bladeMeet.ELCP(bladeB);
}


public override bool Equals(object obj)
{
    return !ReferenceEquals(obj, null) && Equals(obj as #frame#Blade);
}

public override int GetHashCode()
{
    return Grade.GetHashCode() ^ Coefs.GetHashCode();
}

public override string ToString()
{
    if (IsZeroBlade)
        return default(#double#).ToString(CultureInfo.InvariantCulture);

    if (IsScalar)
        return Coefs[0].ToString(CultureInfo.InvariantCulture);

    var s = new StringBuilder();

    for (var i = 0; i < KvSpaceDim; i++)
    {
        s.Append(""("")
            .Append(Coefs[i].ToString(CultureInfo.InvariantCulture))
            .Append("" "")
            .Append(BasisBladesNames[i])
            .Append("") + "");
    }

    s.Length -= 3;

    return s.ToString();
}

end blade
";
        #endregion

        #region Vector
        internal const string VectorTemplates =
@"
delimiters #

begin vector
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace GMacBlade.#frame#
{
    /// <summary>
    /// This class represents a mutable vector in the #frame# frame
    /// </summary>
    public sealed class #frame#Vector
    {
        public static #frame#Vector[] BasisVectors()
        {
            return new[]
            {
                #basis_vectors#
            };
        }


        #members_declare#

        public #double# NormSquared
        {
            get { return #norm2#; }
        }

        public #double# EuclideanNormSquared
        {
            get { return #enorm2#; }
        }

        public #double# EuclideanNorm
        {
            get { return Math.Sqrt(#enorm2#); }
        }


        public #frame#Vector()
        {
        }

        public #frame#Vector(#init_inputs#)
        {
            #init_assign#
        }

        public #frame#Vector(#double#[] c)
        {
            #init_assign_array#
        }

        /// <summary>
        /// Convert this to a unit-vector in the Euclidean space
        /// </summary>
        /// <returns></returns>
        public #double# Normalize()
        {
            var scalar = EuclideanNorm;
            var invScalar = 1.0D / scalar;

            #normalize#

            return scalar;
        }

        public #double#[] ToArray()
        {
            return new[] { #members_list# };
        }

        public #frame#Blade ToBlade()
        {
            return new #frame#Blade(1, new[] { #members_list# });
        }
    }
}

end vector

begin factored_blade
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMacBlade.#frame#
{
    /// <summary>
    /// Represents a factored blade in Euclidean space
    /// </summary>
    public sealed class #frame#FactoredBlade
    {
        public #double# Norm { get; private set; }

        public #frame#Vector[] Vectors { get; private set; }


        public int Grade { get { return Vectors.Length; } }


        internal #frame#FactoredBlade(#double# norm)
        {
            Norm = norm;
            Vectors = new #frame#Vector[0];
        }

        internal #frame#FactoredBlade(#double# norm, #frame#Vector vector)
        {
            Norm = norm;
            Vectors = new [] { vector };
        }

        internal #frame#FactoredBlade(#double# norm, #frame#Vector[] vectors)
        {
            Norm = norm;
            Vectors = vectors;
        }

        /// <summary>
        /// Convert each vector to a normal vector (assuming Euclidean space) and factor 
        /// the squared norms to the NormSquared member
        /// </summary>
        /// <returns></returns>
        public #frame#FactoredBlade Normalize()
        {
            for (var idx = 0; idx < Vectors.Length; idx++)
                Norm *= Vectors[idx].Normalize();

            return this;
        }

        public #frame#Blade ToBlade()
        {
            return #frame#Blade.OP(Vectors).Times(Norm);
        }
    }
}

end factored_blade
";
        #endregion

        #region Outermorphism
        internal const string OutermorphismTemplates = @"
delimiters #

begin om_file_start
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace GMacBlade.#frame#
{
    /// <summary>
    /// This class represents a mutable outermorphism in the #frame# frame by only storing a #grade# by #grade#
    /// matrix of the original vector linear transform and computing the other k-vectors matrices as needed
    /// </summary>
    public sealed partial class #frame#Outermorphism
    {
end om_file_start

begin om_apply
public static #double#[] Apply_#grade#(#double#[,] omCoefs, #double#[] bladeCoefs)
{
    var coefs = new #double#[#num#];

    #computations#
    return coefs;
}


end om_apply

begin om_apply_code_case
case #grade#:
    return new #frame#Blade(#grade#, Apply_#grade#(Coefs, blade.Coefs));
end om_apply_code_case

begin outermorphism
public #double#[,] Coefs { get; private set; }


public #frame#Outermorphism()
{
    Coefs = new #double#[#frame#Blade.MaxGrade, #frame#Blade.MaxGrade];
}

private #frame#Outermorphism(#double#[,] coefs)
{
    Coefs = coefs;
}


public #frame#Outermorphism Transpose()
{
    var coefs = new #double#[#frame#Blade.MaxGrade, #frame#Blade.MaxGrade];

    #transpose_code#

    return new #frame#Outermorphism(coefs);
}

public #double# MetricDeterminant()
{
    #double# det;

    #metric_det_code#

    return det;
}

public #double# EuclideanDeterminant()
{
    #double# det;

    #euclidean_det_code#

    return det;
}

public #frame#Blade Apply(#frame#Blade blade)
{
    if (blade.IsZero)
        return #frame#Blade.ZeroBlade;

    switch (blade.Grade)
    {
        case 0:
            return blade;
        #apply_cases_code#
    }

    throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
}


public static #frame#Outermorphism operator +(#frame#Outermorphism om1, #frame#Outermorphism om2)
{
    var coefs = new #double#[#frame#Blade.MaxGrade, #frame#Blade.MaxGrade];

    #plus_code#

    return new #frame#Outermorphism(coefs);
}

public static #frame#Outermorphism operator -(#frame#Outermorphism om1, #frame#Outermorphism om2)
{
    var coefs = new #double#[#frame#Blade.MaxGrade, #frame#Blade.MaxGrade];

    #subt_code#

    return new #frame#Outermorphism(coefs);
}

public static #frame#Outermorphism operator *(#frame#Outermorphism om1, #frame#Outermorphism om2)
{
    var coefs = new #double#[#frame#Blade.MaxGrade, #frame#Blade.MaxGrade];

    #compose_code#

    return new #frame#Outermorphism(coefs);
}

public static #frame#Outermorphism operator *(#double# scalar, #frame#Outermorphism om)
{
    var coefs = new #double#[#frame#Blade.MaxGrade, #frame#Blade.MaxGrade];

    #times_code#

    return new #frame#Outermorphism(coefs);
}

public static #frame#Outermorphism operator *(#frame#Outermorphism om, #double# scalar)
{
    var coefs = new #double#[#frame#Blade.MaxGrade, #frame#Blade.MaxGrade];

    #times_code#

    return new #frame#Outermorphism(coefs);
}

public static #frame#Outermorphism operator /(#frame#Outermorphism om, #double# scalar)
{
    var coefs = new #double#[#frame#Blade.MaxGrade, #frame#Blade.MaxGrade];

    #divide_code#

    return new #frame#Outermorphism(coefs);
}

public static #frame#Outermorphism operator -(#frame#Outermorphism om)
{
    var coefs = new #double#[#frame#Blade.MaxGrade, #frame#Blade.MaxGrade];

    #negative_code#

    return new #frame#Outermorphism(coefs);
}

end outermorphism
";
        #endregion

        #region Static
        internal const string StaticTemplates =
@"
delimiters #

begin static_basisblade_name
new [] { #names# }
end static_basisblade_name

begin static_basisblade_declare
public static readonly #frame#Blade E#id# = new #frame#Blade(#grade#, new[] { #coefs# });
end static_basisblade_declare

begin static
/// <summary>
/// The maximum grade for any blade of this class
/// </summary>
public const int MaxGrade = #grade#;

/// <summary>
/// The accuracy factor for computations. Any value below this is considered zero
/// </summary>
public static #double# Epsilon = 1e-12;

/// <summary>
/// A lookup table for finding the k-vctor space dimension of a blade using its grade 
/// as index to this array
/// </summary>
internal static readonly int[] GradeToKvSpaceDim = { #kvdims# };

/// <summary>
/// An array of arrays containing basis blades names for this frame grouped by grade
/// </summary>
private static readonly string[][] BasisBladesNamesArray =
    {
        #basisnames#
    };

/// <summary>
/// The zero blade
/// </summary>
public static readonly #frame#Blade ZeroBlade = new #frame#Blade();

#basisblades#

/// <summary>
/// Create a new coefficients array to be used for later creation of a blade
/// </summary>
/// <param name=""grade""></param>
/// <returns></returns>
public static #double#[] CreateCoefsArray(int grade)
{
    return new #double#[GradeToKvSpaceDim[grade]];
}

end static
";
        #endregion

        #region Equals
        internal const string EqualsTemplates = @"
delimiters #

begin equals_case
c = coefs1[#num#] - coefs2[#num#];
if (c <= -Epsilon || c >= Epsilon) return false;

end equals_case

begin equals
private static bool Equals#num#(#double#[] coefs1, #double#[] coefs2)
{
    #double# c;

    #cases#
    return true;
}


end equals

begin main_equals_case
case #grade#:
    return Equals#num#(Coefs, blade2.Coefs);
end main_equals_case

begin main_equals
public bool Equals(#frame#Blade blade2)
{
    if ((object)blade2 == null) 
        return false;

    if (ReferenceEquals(this, blade2)) 
        return true;

    if (IsZeroBlade) 
        return blade2.IsZero;

    if (blade2.IsZeroBlade) 
        return IsZero;

    if (Grade != blade2.Grade) 
        return false;

    switch (Grade)
    {
        #cases#
    }

    throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
}

end main_equals
";
        #endregion

        #region IsZero
        internal const string IsZeroTemplates = @"
delimiters #

begin iszero_case
coefs[#num#] <= -Epsilon || coefs[#num#] >= Epsilon
end iszero_case

begin trimcoefs_case
(coefs[#num#] <= -Epsilon || coefs[#num#] >= Epsilon) ? coefs[#num#] : 0.0D
end trimcoefs_case

begin iszero
private static bool IsZero#num#(#double#[] coefs)
{
    return !(
        #iszero_case#
        );
}

private static #double#[] TrimCoefs#num#(#double#[] coefs)
{
    return new[]
    {
        #trimcoefs_case#
    };
}


end iszero

begin main_iszero_case
case #grade#:
    return IsZero#num#(Coefs);
end main_iszero_case

begin main_trimcoefs_case
case #grade#:
    return new #frame#Blade(#grade#, TrimCoefs#num#(Coefs));
end main_trimcoefs_case

begin main_iszero
public bool IsZero
{
    get
    {
        if (IsZeroBlade)
            return true;

        switch (Grade)
        {
            #main_iszero_case#
        }

        throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
    }
}

/// <summary>
/// Set all near-zero coefficients to zero. If all coefficients are near zero a ZeroBlade is returned
/// </summary>
public #frame#Blade TrimNearZero
{
    get
    {
        if (IsZero)
            return ZeroBlade;

        switch (Grade)
        {
            #main_trimcoefs_case#
        }

        throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
    }
}

end main_iszero";
        #endregion

        #region Involution
        internal const string InvolutionTemplates = @"
delimiters #

begin negative_case
-coefs[#num#]
end negative_case

begin negative
private static #double#[] Negative#num#(#double#[] coefs)
{
    return new[]
    {
        #cases#
    };
}


end negative

begin main_negative_case
case #grade#:
    return new #frame#Blade(#grade#, Negative#num#(Coefs));

end main_negative_case

begin main_negative_case2
case #grade#:
    return this;

end main_negative_case2

begin main_involution
public #frame#Blade #name#
{
    get
    {
        if (IsZeroBlade)
            return this;

        switch (Grade)
        {
            #cases#
        }

        throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
    }
}


end main_involution
";
        #endregion

        #region Norm
        internal const string NormTemplates =
@"
delimiters #

begin norm
private static #double# #name#_#grade#(#double#[] coefs)
{
    var result = 0.0D;

    #computations#
    return result;
}


end norm

begin main_norm_case
case #grade#:
    return #name#_#grade#(Coefs);
end main_norm_case

begin main_norm
public #double# #name#
{
    get
    {
        if (IsZeroBlade)
            return 0.0D;

        switch (Grade)
        {
            #main_norm_case#
        }

        throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
    }
}


end main_norm
";
        #endregion

        #region Misc
        internal const string MiscTemplates =
@"
delimiters #

begin add_case
coefs1[#index#] + coefs2[#index#]
end add_case

begin subt_case
coefs1[#index#] - coefs2[#index#]
end subt_case

begin times_case
scalar * coefs[#index#]
end times_case

begin misc
private static #double#[] Add#num#(#double#[] coefs1, #double#[] coefs2)
{
    return new[]
    {
        #addcases#
    };
}

private static #double#[] Subtract#num#(#double#[] coefs1, #double#[] coefs2)
{
    return new[]
    {
        #subtcases#
    };
}

private static #double#[] Times#num#(#double#[] coefs, #double# scalar)
{
    return new[]
    {
        #timescases#
    };
}


end misc

begin edual
private static #double#[] EuclideanDual#grade#(#double#[] coefs)
{
    var c = new #double#[#num#];

    #computations#
    return c;
}


end edual

//begin self_dp_grade_case
// Input Grade: #ingrade#
//Output Grade: #outgrade#
//#computations#
//end self_dp_grade_case

begin self_dp_grade
private static int SelfDPGrade#grade#(#double#[] coefs)
{
    #double# c = 0.0D;

    #computations#
    return 0;
}


end self_dp_grade


begin main_add_case
case #grade#:
    return new #frame#Blade(#grade#, Add#num#(Coefs, blade2.Coefs));
end main_add_case

begin main_subt_case
case #grade#:
    return new #frame#Blade(#grade#, Subtract#num#(Coefs, blade2.Coefs));
end main_subt_case

begin main_times_case
case #grade#:
    return new #frame#Blade(#grade#, Times#num#(Coefs, scalar));
end main_times_case

begin main_divide_case
case #grade#:
    return new #frame#Blade(#grade#, Times#num#(Coefs, 1.0D / scalar));
end main_divide_case

begin main_inverse_case
case #grade#:
    return new #frame#Blade(#grade#, Times#num#(Coefs, #sign#1.0D / scalar));
end main_inverse_case

begin main_edual_case
case #grade#:
    return new #frame#Blade(#invgrade#, EuclideanDual#grade#(Coefs));
end main_edual_case

begin main_self_dp_grade_case
case #grade#:
    return SelfDPGrade#grade#(Coefs);
end main_self_dp_grade_case


begin main_self_dp_grade
/// <summary>
/// The grade of the delta product of this k-vector with itself. If the grade is 0 this 
/// k-vector is a blade
/// </summary>
/// <returns></returns>
public int SelfDPGrade()
{
    if (Grade <= 1 || Grade >= MaxGrade - 1 || IsZero)
        return 0;

    switch (Grade)
    {
        #main_self_dp_grade_case#
    }

    throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
}

end main_self_dp_grade

begin misc_main
public #frame#Blade Add(#frame#Blade blade2)
{
    if (blade2.IsZero)
        return this;

    if (IsZero)
        return blade2;

    if (Grade != blade2.Grade)
        throw new InvalidOperationException(""Can't add two non-zero blades of different grades"");

    switch (Grade)
    {
        #main_add_case#
    }

    throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
}

public #frame#Blade Subtract(#frame#Blade blade2)
{
    if (blade2.IsZero)
        return this;

    if (IsZero)
        return blade2.Negative;

    if (Grade != blade2.Grade)
        throw new InvalidOperationException(""Can't subtract two non-zero blades of different grades"");

    switch (Grade)
    {
        #main_subt_case#
    }

    throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
}

public #frame#Blade Times(#double# scalar)
{
    switch (Grade)
    {
        #main_times_case#
    }

    throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
}

public #frame#Blade Divide(#double# scalar)
{
    switch (Grade)
    {
        #main_divide_case#
    }

    throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
}

public #frame#Blade Inverse
{
    get
    {
        var scalar = #norm2_opname#;

        if ((scalar <= -Epsilon || scalar >= Epsilon) == false)
            throw new InvalidOperationException(""Null blade has no inverse"");

        switch (Grade)
        {
            #main_inverse_case#
        }

        throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
    }
}

public #frame#Blade EuclideanInverse
{
    get
    {
        var scalar = #emag2_opname#;

        if ((scalar <= -Epsilon || scalar >= Epsilon) == false)
            throw new InvalidOperationException(""Null blade has no inverse"");

        switch (Grade)
        {
            #main_inverse_case#
        }

        throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
    }
}

public #frame#Blade EuclideanDual
{
    get
    {
        if (IsZero)
            return ZeroBlade;

        switch (Grade)
        {
            #main_edual_case#
        }

        throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
    }
}

end misc_main
";
        #endregion

        #region BilinearProduct
        internal const string BilinearProductTemplates =
@"
delimiters #

begin bilinearproduct
private static #double#[] #name#(#double#[] coefs1, #double#[] coefs2)
{
    var c = new #double#[#num#];

    #computations#
    return c;
}


end bilinearproduct

begin bilinearproduct_main_case
//grade1: #g1#, grade2: #g2#
case #id#:
    return new #frame#Blade(#grade#, #name#(Coefs, blade2.Coefs));

end bilinearproduct_main_case

begin bilinearproduct_main
public #frame#Blade #name#(#frame#Blade blade2)
{
    if (IsZero || blade2.IsZero || #zerocond#)
        return ZeroBlade;

    var id = Grade + blade2.Grade * (MaxGrade + 1);

    switch (id)
    {
        #cases#
    }

    throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
}

end bilinearproduct_main

begin gp_case
new #frame#Blade(#grade#, #name#(coefs1, coefs2))
end gp_case

begin gp
private static #frame#Blade[] #name#(#double#[] coefs1, #double#[] coefs2)
{
    return new[]
    {
        #gp_case#
    };
}

end gp

begin gp_main_case
//grade1: #g1#, grade2: #g2#
case #id#:
    return #name#(Coefs, blade2.Coefs);
end gp_main_case

begin gp_main
public #frame#Blade[] #name#(#frame#Blade blade2)
{
    if (IsZero || blade2.IsZero)
        return new #frame#Blade[0];

    var id = Grade + blade2.Grade * (MaxGrade + 1);

    switch (id)
    {
        #cases#
    }

    throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
}

end gp_main

begin self_bilinearproduct
private static #double#[] #name#(#double#[] coefs)
{
    var c = new #double#[#num#];

    #computations#
    return c;
}


end self_bilinearproduct

begin selfgp_case
new #frame#Blade(#grade#, #name#(coefs))
end selfgp_case

begin selfgp
private static #frame#Blade[] #name#(#double#[] coefs)
{
    return new[]
    {
        #selfgp_case#
    };
}

end selfgp

begin selfgp_main_case
//grade: #grade#
case #grade#:
    return #name#(Coefs);
end selfgp_main_case

begin selfgp_main
public #frame#Blade[] #name#()
{
    if (IsZero)
        return new #frame#Blade[0];

    switch (Grade)
    {
        #cases#
    }

    throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
}

end selfgp_main


begin dp_case
coefs = #name#(coefs1, coefs2);
if (IsZero#num#(coefs) == false)
    return new #frame#Blade(#grade#, coefs);


end dp_case

begin dp
private static #frame#Blade #name#(#double#[] coefs1, #double#[] coefs2)
{
    //Try all Euclidean geometric products for these two input grades starting from largest to smallest
    //output grade

    #double#[] coefs;

    #dp_case#
    return ZeroBlade;
}


end dp

begin dp_main_case
//grade1: #g1#, grade2: #g2#
case #id#:
    return #name#(Coefs, blade2.Coefs);
end dp_main_case

begin dp_main
public #frame#Blade #name#(#frame#Blade blade2)
{
    if (IsZero || blade2.IsZero)
        return ZeroBlade;

    var id = Grade + blade2.Grade * (MaxGrade + 1);

    switch (id)
    {
        #cases#
    }

    throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
}
end dp_main

begin applyversor_main_case
//grade1: #g1#, grade2: #g2#
case #id#:
    return new #frame#Blade(#grade#, #name#(Coefs, blade2.Coefs));
end applyversor_main_case

begin applyversor_main
public #frame#Blade #name#(#frame#Blade blade2)
{
    if (blade2.IsZero)
        return ZeroBlade;

    var id = Grade + blade2.Grade * (MaxGrade + 1);

    switch (id)
    {
        #cases#
    }

    throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
}

end applyversor_main

begin op_vectors
private static #frame#Blade OP#grade#(#frame#Vector[] vectors)
{
    var coefs = new #double#[#num#];

    #computations#
    return new #frame#Blade(#grade#, coefs);
}


end op_vectors

begin op_vectors_main_case
case #grade#:
    return OP#grade#(vectors);
end op_vectors_main_case

begin op_vectors_main
public static #frame#Blade OP(#frame#Vector[] vectors)
{
    switch (vectors.Length)
    {
        case 0:
            return ZeroBlade;
        case 1:
            return vectors[0].ToBlade();
        #op_vectors_main_case#
    }

    throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
}


end op_vectors_main
";
        #endregion

        #region Factor

        internal const string FactorizationTemplates =
@"
delimiters #

begin factor
private static #frame#Vector[] Factor#id#(#double#[] coefs)
{
    var vectors = new[] 
    {
        #newvectors#
    };

    #computations#

    return vectors;
}


end factor

begin maxcoefid_case
c = Math.Abs(coefs[#index#]);
if (c > maxCoef)
{
    maxCoef = c;
    maxCoefId = #id#;
}

end maxcoefid_case

begin maxcoefid
private static int MaxCoefId#grade#(#double#[] coefs)
{
    var c = Math.Abs(coefs[0]);
    var maxCoef = c;
    var maxCoefId = #initid#;

    #maxcoefid_case#
    c = Math.Abs(coefs[#maxindex#]);
    if (c > maxCoef)
        maxCoefId = #maxid#;

    return maxCoefId;
}


end maxcoefid

begin factorgrade_case
case #id#:
    return Factor#id#(coefs);
end factorgrade_case

begin factorgrade
private static #frame#Vector[] FactorGrade#grade#(#double#[] coefs)
{
    var maxCoefId = MaxCoefId#grade#(coefs);

    switch (maxCoefId)
    {
        #factorgrade_case#
    }

    throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
}


end factorgrade

begin factor_main_case
case #grade#:
    return new #frame#FactoredBlade(#name#_#grade#(Coefs), FactorGrade#grade#(Coefs));

end factor_main_case

begin factor_main
public #frame#FactoredBlade Factor()
{
    if (IsZero)
        return new #frame#FactoredBlade(0.0D);

    switch (Grade)
    {
        case 0:
            return new #frame#FactoredBlade(Coefs[0]);

        case 1:
            var vector = ToVector();
            var norm = vector.Normalize();
            return new #frame#FactoredBlade(norm, vector);

        #factor_main_case#
        case #maxgrade#:
            return new #frame#FactoredBlade(Coefs[0],  #frame#Vector.BasisVectors());
    }

    throw new InvalidDataException(""Internal error. Blade grade not acceptable!"");
}

end factor_main
";
        #endregion
    }
}
