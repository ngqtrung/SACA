using System;
using System.ComponentModel;

namespace SACA_Common.Enums
{

    public enum eType_ContestProgrammingLanguage
    {
        [Description("C (GCC 7.4.0)"), SortOrder(1)]
        C_GCC_7_4_0 = 48,

        [Description("C++ (Clang 7.0.1)"), SortOrder(2)]
        CPP_Clang_7_0_1 = 76,

        [Description("Java (OpenJDK 8)"), SortOrder(2.1)]
        Java_OpenJDK_8 = 27,

        [Description("Java (OpenJDK 13.0.1)"), SortOrder(3)]
        Java_OpenJDK_13_0_1 = 62,

        [Description("Python (2.7.17)"), SortOrder(4)]
        Python_2_7_17 = 70,

        [Description("C# (Mono 6.6.0.161)"), SortOrder(4.1)]
        CSharp_Mono_6_6_0_161 = 51,

        [Description("JavaScript (Node.js 12.14.0)"), SortOrder(4.2)]
        JavaScript_NodeJS_12_14_0 = 63,

        [Description("Assembly (NASM 2.14.02)"), SortOrder(5)]
        Assembly_NASM_2_14_02 = 45,

        [Description("Bash (5.0.0)"), SortOrder(6)]
        Bash_5_0_0 = 46,

        [Description("Basic (FBC 1.07.1)"), SortOrder(7)]
        Basic_FBC_1_07_1 = 47,

        [Description("C (Clang 7.0.1)"), SortOrder(8)]
        C_Clang_7_0_1 = 75,

        [Description("C++ (GCC 7.4.0)"), SortOrder(9)]
        CPP_GCC_7_4_0 = 52,

        [Description("C (GCC 8.3.0)"), SortOrder(10)]
        C_GCC_8_3_0 = 49,

        [Description("C++ (GCC 8.3.0)"), SortOrder(11)]
        CPP_GCC_8_3_0 = 53,

        [Description("C (GCC 9.2.0)"), SortOrder(12)]
        C_GCC_9_2_0 = 50,

        [Description("C++ (GCC 9.2.0)"), SortOrder(13)]
        CPP_GCC_9_2_0 = 54,

        [Description("Clojure (1.10.1)"), SortOrder(14)]
        Clojure_1_10_1 = 86,

        [Description("COBOL (GnuCOBOL 2.2)"), SortOrder(16)]
        COBOL_GnuCOBOL_2_2 = 77,

        [Description("Common Lisp (SBCL 2.0.0)"), SortOrder(17)]
        CommonLisp_SBCL_2_0_0 = 55,

        [Description("D (DMD 2.089.1)"), SortOrder(18)]
        D_DMD_2_089_1 = 56,

        [Description("Elixir (1.9.4)"), SortOrder(19)]
        Elixir_1_9_4 = 57,

        [Description("Erlang (OTP 22.2)"), SortOrder(20)]
        Erlang_OTP_22_2 = 58,

        [Description("Executable"), SortOrder(21)]
        Executable = 44,

        [Description("F# (.NET Core SDK 3.1.202)"), SortOrder(22)]
        FSharp_DotNetCore_3_1_202 = 87,

        [Description("Fortran (GFortran 9.2.0)"), SortOrder(23)]
        Fortran_GFortran_9_2_0 = 59,

        [Description("Go (1.13.5)"), SortOrder(24)]
        Go_1_13_5 = 60,

        [Description("Groovy (3.0.3)"), SortOrder(25)]
        Groovy_3_0_3 = 88,

        [Description("Haskell (GHC 8.8.1)"), SortOrder(26)]
        Haskell_GHC_8_8_1 = 61,

        [Description("Kotlin (1.3.70)"), SortOrder(28)]
        Kotlin_1_3_70 = 78,

        [Description("Lua (5.3.5)"), SortOrder(29)]
        Lua_5_3_5 = 64,

        [Description("Multi-file program"), SortOrder(30)]
        MultiFileProgram = 89,

        [Description("Objective-C (Clang 7.0.1)"), SortOrder(31)]
        ObjectiveC_Clang_7_0_1 = 79,

        [Description("OCaml (4.09.0)"), SortOrder(32)]
        OCaml_4_09_0 = 65,

        [Description("Octave (5.1.0)"), SortOrder(33)]
        Octave_5_1_0 = 66,

        [Description("Pascal (FPC 3.0.4)"), SortOrder(34)]
        Pascal_FPC_3_0_4 = 67,

        [Description("Perl (5.28.1)"), SortOrder(35)]
        Perl_5_28_1 = 85,

        [Description("PHP (7.4.1)"), SortOrder(36)]
        PHP_7_4_1 = 68,

        [Description("Plain Text"), SortOrder(37)]
        PlainText = 43,

        [Description("Prolog (GNU Prolog 1.4.5)"), SortOrder(38)]
        Prolog_GNU_Prolog_1_4_5 = 69,

        [Description("Python (3.8.1)"), SortOrder(39)]
        Python_3_8_1 = 71,

        [Description("R (4.0.0)"), SortOrder(40)]
        R_4_0_0 = 80,

        [Description("Ruby (2.7.0)"), SortOrder(41)]
        Ruby_2_7_0 = 72,

        [Description("Rust (1.40.0)"), SortOrder(42)]
        Rust_1_40_0 = 73,

        [Description("Scala (2.13.2)"), SortOrder(43)]
        Scala_2_13_2 = 81,

        [Description("SQL (SQLite 3.27.2)"), SortOrder(44)]
        SQL_SQLite_3_27_2 = 82,

        [Description("Swift (5.2.3)"), SortOrder(45)]
        Swift_5_2_3 = 83,

        [Description("TypeScript (3.7.4)"), SortOrder(46)]
        TypeScript_3_7_4 = 74,

        [Description("Visual Basic.Net (vbnc 0.0.0.5943)"), SortOrder(47)]
        VisualBasicNet_vbnc_0_0_0_5943 = 84
    }
}
