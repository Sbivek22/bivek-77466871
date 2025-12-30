using System;
using System.Globalization;
using System.Linq;
using BOOSE;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Replace replacement for BOOSE Array (1D or 2D).
    /// 
    /// Supported declaration forms:
    /// <code>
    /// array int myArr 10
    /// array real myArr 10,20
    /// array myArr 10,20   (defaults to int)
    /// </code>
    /// </summary>
    public class AppArray : Evaluation
    {
        private string elementType = "int";
        private int rows = 0;
        private int cols = 1;

        private int[,]? intArray;
        private double[,]? realArray;

        public string ElementType => elementType;
        public int Rows => rows;
        public int Cols => cols;

        public override void Compile()
        {
            if (this.Program == null)
                throw new CommandException("Array has not been initialised with a StoredProgram.");
            string text = (ParameterList ?? string.Empty).Trim();
            if (text.Length == 0)
                throw new CommandException("Array requires parameters.");

            // Tokenise: allow commas and spaces.
            var tokens = text.Replace("(", " ").Replace(")", " ")
                .Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => t.Length > 0)
                .ToArray();

            if (tokens.Length < 2)
                throw new CommandException("Array declaration should include a name and size.");

            int idx = 0;
            if (tokens[0].Equals("int", StringComparison.OrdinalIgnoreCase) ||
                tokens[0].Equals("real", StringComparison.OrdinalIgnoreCase))
            {
                elementType = tokens[0].ToLowerInvariant();
                idx = 1;
            }

            VarName = tokens[idx++];

            // sizes
            if (idx >= tokens.Length)
                throw new CommandException("Array declaration missing size.");

            rows = Math.Max(0, BooseEval.Int(this.Program, tokens[idx++]));
            cols = (idx < tokens.Length) ? Math.Max(1, BooseEval.Int(this.Program, tokens[idx])) : 1;

            // Allocate.
            if (elementType == "real")
                realArray = new double[rows, cols];
            else
                intArray = new int[rows, cols];

            Value = rows * cols;
            base.Value = Value;
            if (!this.Program.VariableExists(VarName))
                this.Program.AddVariable(this);
        }

        public override void Execute()
        {
            // Declaration done at compile time.
        }

        public void SetCell(int row, int col, string valueExpression)
        {
            if (this.Program == null)
                throw new CommandException("Array has not been initialised with a StoredProgram.");

            ValidateIndex(row, col);

            if (elementType == "real")
            {
                if (realArray == null) throw new CommandException("Real array not allocated.");
                realArray[row, col] = BooseEval.Double(this.Program, valueExpression);
            }
            else
            {
                if (intArray == null) throw new CommandException("Int array not allocated.");
                intArray[row, col] = BooseEval.Int(this.Program, valueExpression);
            }
        }

        public string GetCell(int row, int col)
        {
            if (this.Program == null)
                throw new CommandException("Array has not been initialised with a StoredProgram.");

            ValidateIndex(row, col);

            if (elementType == "real")
            {
                if (realArray == null) throw new CommandException("Real array not allocated.");
                return realArray[row, col].ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                if (intArray == null) throw new CommandException("Int array not allocated.");
                return intArray[row, col].ToString(CultureInfo.InvariantCulture);
            }
        }

        private void ValidateIndex(int row, int col)
        {
            if (row < 0 || row >= rows)
                throw new CommandException($"Array row index {row} out of range (0..{rows - 1}).");
            if (col < 0 || col >= cols)
                throw new CommandException($"Array column index {col} out of range (0..{cols - 1}).");
        }
    }
}
