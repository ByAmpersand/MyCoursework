using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kyrsova_v1
{
    class ExpensesExceptions : Exception
    {
        public ExpensesExceptions() { }
    }

    class EmptyListException : ExpensesExceptions
    {
        public EmptyListException() { }
    }

    class NoFindingsException : ExpensesExceptions
    {
        public NoFindingsException() { }
    }

    class WrongArgumentsException : ExpensesExceptions
    {
        public WrongArgumentsException() { }
    }

    class FileReadException : ExpensesExceptions
    {
        static int line;
        public FileReadException(int tmp) { line = tmp; }
    } 
}