using System;

namespace Apache.NBCEL.Java.Lang
{
    internal class AssertionError : Exception
    {
        public AssertionError(string error = "")
        {
            Error = error;
        }

        public string Error { get; }
    }
}