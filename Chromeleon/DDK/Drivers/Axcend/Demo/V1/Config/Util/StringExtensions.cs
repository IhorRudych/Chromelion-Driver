// Copyright 2018 Thermo Fisher Scientific Inc.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MyCompany
{
    /// <summary>String Extensions</summary>
    public static class StringExtensions
    {
        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string psz1, string psz2);

        /// <summary>Orders strings that have numbers taking into account their numeric values</summary>
        public static int NaturalCompare(this string x, string y)
        {
            return StrCmpLogicalW(x ?? string.Empty, y ?? string.Empty);
        }
    }

    /// <summary>String Natural Comparer - Orders strings that have numbers taking into account their numeric values</summary>
    public class StringNaturalComparer : IComparer<string>
    {
        /// <summary>Orders strings that have numbers taking into account their numeric values</summary>
        public int Compare(string x, string y)
        {
            return x.NaturalCompare(y);
        }
    }
}
