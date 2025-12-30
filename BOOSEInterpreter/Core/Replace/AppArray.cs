using System;
using System.Globalization;
using System.Linq;
using BOOSE;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Replacement implementation for BOOSE <c>array</c> declarations supporting 1D and 2D
    /// arrays of integers or real numbers.
    /// </summary>
    /// <remarks>
    /// Parses declarations such as:
    /// <code>
    /// array int myArr 10
    /// array real myArr 10,20
    /// array myArr 10,20   (defaults to int)
    /// </code>
    /// Allocations occur during <see cref="Compile"/>. The class exposes methods to set and
    /// retrieve cell values with bounds checking and performs type-specific storage in
    /// separate backing arrays.
    /// </remarks>
    public class AppArray : Evaluation
    {
        private string elementType = "int";
        private int rows = 0;
        private int cols = 1;

        private int[,]? intArray;
        private double[,]? realArray;

        /// <summary>
        /// Gets the declared element type for this array (<c>int</c> or <c>real</c>).
        /// </summary>
        public string ElementType => elementType;

        /// <summary>
        /// Gets the number of rows allocated for this array (first dimension).
        /// </summary>
        public int Rows => rows;

        /// <summary>
        /// Gets the number of columns allocated for this array (second dimension).
        /// </summary>
        public int Cols => cols;

        /// <summary>
        /// Parses the textual declaration in <see cref="ParameterList"/>, establishes the
        /// element type and dimensions, allocates the underlying storage and registers the
        /// variable with the program's variable table.
        /// </summary>
        /// <exception cref="CommandException">Thrown when the declaration is invalid or when
        /// no <see cref="Program"/> is associated with this command.</exception>
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

        /// <summary>
        /// No runtime action is required for an array declaration; allocation and
        /// registration are handled in <see cref="Compile"/>.
        /// </summary>
        public override void Execute()
        {
            // Declaration done at compile time.
        }

        /// <summary>
        /// Evaluates <paramref name="valueExpression"/> in the current program context and
        /// assigns the resulting value to the specified cell.
        /// </summary>
        /// <param name="row">Zero-based row index.</param>
        /// <param name="col">Zero-based column index.</param>
        /// <param name="valueExpression">Expression to evaluate for the cell value.</param>
        /// <exception cref="CommandException">Thrown when the command is not associated with
        /// a <see cref="Program"/>, when indices are out of range, or when the backing store
        /// for the declared element type is not allocated.</exception>
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

        /// <summary>
        /// Retrieves the contents of the specified cell as a string. Real values are formatted
        /// using invariant culture.
        /// </summary>
        /// <param name="row">Zero-based row index.</param>
        /// <param name="col">Zero-based column index.</param>
        /// <returns>The cell value formatted as a string.</returns>
        /// <exception cref="CommandException">Thrown when the command is not associated with
        /// a <see cref="Program"/>, when indices are invalid, or when the backing store is
        /// not allocated.</exception>
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

        /// <summary>
        /// Validates that the given indices are within the allocated array bounds and throws
        /// a <see cref="CommandException"/> when they are not.
        /// </summary>
        /// <param name="row">Zero-based row index.</param>
        /// <param name="col">Zero-based column index.</param>
        /// <exception cref="CommandException">Thrown when either index is out of range.</exception>
        private void ValidateIndex(int row, int col)
        {
            if (row < 0 || row >= rows)
                throw new CommandException($"Array row index {row} out of range (0..{rows - 1}).");
            if (col < 0 || col >= cols)
                throw new CommandException($"Array column index {col} out of range (0..{cols - 1}).");
        }
    }
}
