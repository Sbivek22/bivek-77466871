using System;
using System.Globalization;
using BOOSEInterpreter.Core;
using BOOSE;

namespace BOOSEInterpreter.Core.Runtime
{
    /// <summary>
    /// Helper methods for evaluating BOOSE expressions/parameters at runtime.
    /// </summary>
    public static class BooseEval
    {
        /// <summary>
        /// Evaluate an expression and coerce it to an <see cref="int"/>.
        /// </summary>
        /// <param name="program">The current <see cref="StoredProgram"/> context used to resolve variables.</param>
        /// <param name="expression">The expression to evaluate. May be a numeric literal, variable name or arithmetic expression.</param>
        /// <returns>The expression value converted to an <see cref="int"/> (rounded from <see cref="double"/> when necessary).</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="program"/> is <c>null</c>.</exception>
        public static int Int(StoredProgram program, string expression)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            expression = (expression ?? string.Empty).Trim();

            if (int.TryParse(expression, NumberStyles.Integer, CultureInfo.InvariantCulture, out int literal))
                return literal;

            // IMPORTANT:
            // The BOOSE trial evaluator can restrict certain operators (notably '*') which breaks
            // coursework examples like: "circle count * 10".
            // Use our own numeric parser so arithmetic and variables work consistently.
            double d = Double(program, expression);
            return (int)Math.Round(d);
        }

        /// <summary>
        /// Evaluate an expression and coerce it to a <see cref="double"/>.
        /// </summary>
        /// <param name="program">The current <see cref="StoredProgram"/> used to resolve variables.</param>
        /// <param name="expression">The expression to evaluate. May include numeric literals, variables and arithmetic operators.</param>
        /// <returns>The evaluated <see cref="double"/> value.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="program"/> is <c>null</c>.</exception>
        public static double Double(StoredProgram program, string expression)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            expression = (expression ?? string.Empty).Trim();

            if (double.TryParse(expression, NumberStyles.Float, CultureInfo.InvariantCulture, out double literal))
                return literal;

            return new SimpleDoubleParser(program, expression).ParseExpression();
        }

        /// <summary>
        /// Lightweight recursive-descent parser for arithmetic expressions producing <see cref="double"/> values.
        /// </summary>
        /// <remarks>
        /// This parser understands +, -, *, /, parentheses, numeric literals and variable identifiers.
        /// Identifiers are resolved via <see cref="StoredProgram.GetVariable"/>; <see cref="AppReal"/>
        /// variables return their double value while other variables provide an integer value.
        /// </remarks>
        private sealed class SimpleDoubleParser
        {
            private readonly StoredProgram _program;
            private readonly string _s;
            private int _pos;

            public SimpleDoubleParser(StoredProgram program, string s)
            {
                _program = program;
                _s = s ?? string.Empty;
                _pos = 0;
            }

            /// <summary>
            /// Parse and evaluate a full expression supporting addition and subtraction at the top level.
            /// </summary>
            /// <returns>The evaluated <see cref="double"/> result.

            public double ParseExpression()
            {
                double v = ParseTerm();
                SkipWs();
                while (_pos < _s.Length)
                {
                    if (Match('+')) v += ParseTerm();
                    else if (Match('-')) v -= ParseTerm();
                    else break;
                    SkipWs();
                }
                return v;
            }

            /// <summary>
            /// Parse a term (handles multiplication and division).
            /// </summary>
            /// <returns>The evaluated term as a <see cref="double"/>.</returns>
            private double ParseTerm()
            {
                double v = ParseFactor();
                SkipWs();
                while (_pos < _s.Length)
                {
                    if (Match('*')) v *= ParseFactor();
                    else if (Match('/'))
                    {
                        double denom = ParseFactor();
                        if (denom == 0)
                            throw new StoredProgramException("Division by zero.");
                        v /= denom;
                    }
                    else break;
                    SkipWs();
                }
                return v;
            }

            /// <summary>
            /// Parse a factor: unary +/- operators, parenthesised expressions, numeric literals or identifiers.
            /// </summary>
            /// <returns>The evaluated factor as a <see cref="double"/>.</returns>
            private double ParseFactor()
            {
                SkipWs();

                if (Match('+')) return ParseFactor();
                if (Match('-')) return -ParseFactor();

                if (Match('('))
                {
                    double v = ParseExpression();
                    SkipWs();
                    if (!Match(')')) throw new StoredProgramException("Missing ')'");
                    return v;
                }

                // number literal
                int start = _pos;
                while (_pos < _s.Length && (char.IsDigit(_s[_pos]) || _s[_pos] == '.'))
                    _pos++;

                if (_pos > start)
                {
                    string num = _s.Substring(start, _pos - start);
                    if (double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
                        return d;
                    throw new StoredProgramException($"Invalid number '{num}'");
                }

                // identifier (variable)
                string name = ReadIdentifier();
                if (name.Length > 0)
                {
                    var ev = _program.GetVariable(name);

                    // If it is AppReal, use the double Value
                    if (ev is BOOSEInterpreter.Core.Replace.AppReal ar)
                        return ar.Value;

                    // Otherwise use int Value
                    return ev.Value;
                }

                throw new StoredProgramException($"Invalid expression near '{Remaining()}'");
            }

            /// <summary>
            /// Read a C-style identifier from the current parse position (letters, digits and underscore, starting with letter or underscore).
            /// </summary>
            /// <returns>The identifier text or an empty string if none found.</returns>
            private string ReadIdentifier()
            {
                SkipWs();
                int start = _pos;
                if (_pos < _s.Length && (char.IsLetter(_s[_pos]) || _s[_pos] == '_'))
                {
                    _pos++;
                    while (_pos < _s.Length && (char.IsLetterOrDigit(_s[_pos]) || _s[_pos] == '_'))
                        _pos++;
                    return _s.Substring(start, _pos - start);
                }
                return string.Empty;
            }

            /// <summary>Advance the parser position past any whitespace.</summary>
            private void SkipWs()
            {
                while (_pos < _s.Length && char.IsWhiteSpace(_s[_pos])) _pos++;
            }

            /// <summary>Attempt to match the next character; advance on success.</summary>
            private bool Match(char c)
            {
                if (_pos < _s.Length && _s[_pos] == c)
                {
                    _pos++;
                    return true;
                }
                return false;
            }

            /// <summary>Return the remaining unparsed text (trimmed) for error messages.</summary>
            private string Remaining()
            {
                return _pos >= _s.Length ? string.Empty : _s.Substring(_pos).Trim();
            }
        }


        /// <summary>
        /// Evaluate an expression and coerce it to a <see cref="bool"/>.
        /// </summary>
        /// <param name="program">The current <see cref="StoredProgram"/> context used to resolve variables.</param>
        /// <param name="expression">The boolean expression to evaluate. May include comparisons and logical operators.</param>
        /// <returns>The evaluated boolean value.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="program"/> is <c>null</c>.</exception>
        public static bool Bool(StoredProgram program, string expression)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            expression = (expression ?? string.Empty).Trim();

            if (bool.TryParse(expression, out bool literal))
                return literal;

            return new SimpleBoolParser(program, expression).ParseBool();
        }

        /// <summary>
        /// Recursive-descent parser for boolean expressions.
        /// </summary>
        /// <remarks>
        /// Supports logical operators <c>! && ||</c>, parentheses, boolean literals and numeric comparisons.
        /// Numeric sub-expressions are parsed using an internal numeric expression parser so comparisons like
        /// <c>x * 2 &gt; y</c> evaluate correctly.
        /// </remarks>
        private sealed class SimpleBoolParser
        {
            private readonly StoredProgram _program;
            private readonly string _s;
            private int _pos;

            public SimpleBoolParser(StoredProgram program, string s)
            {
                _program = program;
                _s = s ?? string.Empty;
                _pos = 0;
            }

            /// <summary>
            /// Parse and evaluate a complete boolean expression.
            /// </summary>
            /// <returns>The evaluated boolean result.</returns>
            public bool ParseBool()
            {
                bool v = ParseOr();
                SkipWs();
                if (_pos < _s.Length)
                    throw new StoredProgramException($"Invalid expression, syntax problem {Remaining()}");
                return v;
            }

            /// <summary>Parse an OR-expression (left-associative).</summary>
            private bool ParseOr()
            {
                bool v = ParseAnd();
                SkipWs();
                while (Match("||"))
                {
                    bool r = ParseAnd();
                    v = v || r;
                    SkipWs();
                }
                return v;
            }

            /// <summary>Parse an AND-expression (left-associative).</summary>
            private bool ParseAnd()
            {
                bool v = ParseNot();
                SkipWs();
                while (Match("&&"))
                {
                    bool r = ParseNot();
                    v = v && r;
                    SkipWs();
                }
                return v;
            }

            /// <summary>Parse a NOT-expression or a comparison/variable/parenthesised boolean.</summary>
            private bool ParseNot()
            {
                SkipWs();
                if (Match("!"))
                    return !ParseNot();

                // Parentheses for boolean
                if (Match("("))
                {
                    bool inner = ParseOr();
                    SkipWs();
                    if (!Match(")")) throw new StoredProgramException("Missing ')'");
                    return inner;
                }

                // boolean literals
                if (MatchWord("true")) return true;
                if (MatchWord("false")) return false;

                // comparisons or single variable treated as bool
                double left = ParseNumberExpr();
                SkipWs();

                if (Match(">=")) return left >= ParseNumberExpr();
                if (Match("<=")) return left <= ParseNumberExpr();
                if (Match("==")) return left == ParseNumberExpr();
                if (Match("!=")) return left != ParseNumberExpr();
                if (Match(">")) return left > ParseNumberExpr();
                if (Match("<")) return left < ParseNumberExpr();

                // no comparator => non-zero means true
                return Math.Abs(left) > double.Epsilon;
            }

            // ---- numeric expression for comparisons ----
            /// <summary>Parse a numeric expression used for comparisons within boolean expressions.</summary>
            /// <returns>The evaluated numeric value as a <see cref="double"/>.</returns>
            private double ParseNumberExpr()
            {
                double v = ParseTerm();
                SkipWs();
                while (true)
                {
                    if (Match("+")) v += ParseTerm();
                    else if (Match("-")) v -= ParseTerm();
                    else break;
                    SkipWs();
                }
                return v;
            }

            /// <summary>Parse a numeric term (handles * and /).</summary>
            private double ParseTerm()
            {
                double v = ParseFactor();
                SkipWs();
                while (true)
                {
                    if (Match("*")) v *= ParseFactor();
                    else if (Match("/")) v /= ParseFactor();
                    else break;
                    SkipWs();
                }
                return v;
            }

            /// <summary>Parse a numeric factor used by the boolean numeric parser.</summary>
            private double ParseFactor()
            {
                SkipWs();
                if (Match("+")) return ParseFactor();
                if (Match("-")) return -ParseFactor();

                if (Match("("))
                {
                    double inner = ParseNumberExpr();
                    SkipWs();
                    if (!Match(")")) throw new StoredProgramException("Missing ')'");
                    return inner;
                }

                // number literal
                if (TryReadNumber(out double num))
                    return num;

                // variable
                string name = ReadIdentifier();
                if (name.Length > 0)
                {
                    var ev = _program.GetVariable(name);

                    // AppReal => use double Value
                    if (ev is BOOSEInterpreter.Core.Replace.AppReal ar)
                        return ar.Value;

                    // AppBoolean => 1/0
                    if (ev is BOOSEInterpreter.Core.Replace.AppBoolean ab)
                        return ab.BoolValue ? 1.0 : 0.0;

                    // int / default
                    return ev.Value;
                }

                throw new StoredProgramException($"Invalid expression, syntax problem {Remaining()}");
            }

            // ---- low-level ----
            /// <summary>Advance the parser position past any whitespace.</summary>
            private void SkipWs() { while (_pos < _s.Length && char.IsWhiteSpace(_s[_pos])) _pos++; }

            /// <summary>Attempt to match the specified token at the current position, advancing on success.</summary>
            private bool Match(string token)
            {
                SkipWs();
                if (_pos + token.Length > _s.Length) return false;
                if (string.Compare(_s, _pos, token, 0, token.Length, StringComparison.Ordinal) == 0)
                {
                    _pos += token.Length;
                    return true;
                }
                return false;
            }

            /// <summary>Match a whole word (ensures the token is not followed by identifier characters).</summary>
            private bool MatchWord(string word)
            {
                int start = _pos;
                if (!Match(word)) return false;
                if (_pos < _s.Length && (char.IsLetterOrDigit(_s[_pos]) || _s[_pos] == '_'))
                {
                    _pos = start;
                    return false;
                }
                return true;
            }

            /// <summary>Try to read a floating-point number token at the current position.</summary>
            private bool TryReadNumber(out double value)
            {
                SkipWs();
                int start = _pos;
                bool sawDot = false;

                while (_pos < _s.Length)
                {
                    char c = _s[_pos];
                    if (char.IsDigit(c)) { _pos++; continue; }
                    if (c == '.' && !sawDot) { sawDot = true; _pos++; continue; }
                    break;
                }

                if (_pos == start)
                {
                    value = 0;
                    return false;
                }

                string token = _s.Substring(start, _pos - start);
                return double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            }

            /// <summary>Read an identifier (variable name) from the current position.</summary>
            private string ReadIdentifier()
            {
                SkipWs();
                int start = _pos;
                if (_pos < _s.Length && (char.IsLetter(_s[_pos]) || _s[_pos] == '_'))
                {
                    _pos++;
                    while (_pos < _s.Length && (char.IsLetterOrDigit(_s[_pos]) || _s[_pos] == '_'))
                        _pos++;
                    return _s.Substring(start, _pos - start);
                }
                return string.Empty;
            }

            /// <summary>Return the remaining unparsed text (trimmed) for error messages.</summary>
            private string Remaining() => _pos >= _s.Length ? string.Empty : _s.Substring(_pos).Trim();
        }


        /// <summary>
        /// Evaluate an expression that may include string literals and coerce it to a <see cref="string"/>.
        /// </summary>
        /// <param name="program">The current <see cref="StoredProgram"/> used to resolve variables.</param>
        /// <param name="expression">The expression to evaluate. May be a quoted string, numeric expression or an expression that the program evaluates to a string.</param>
        /// <returns>The evaluated string value. Numeric results are formatted consistently using the invariant culture.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="program"/> is <c>null</c>.</exception>
        public static string String(StoredProgram program, string expression)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            expression = (expression ?? string.Empty).Trim();

            // Quick path: quoted string literal.
            if ((expression.StartsWith("\"") && expression.EndsWith("\"")) ||
                (expression.StartsWith("'") && expression.EndsWith("'")))
            {
                return expression.Substring(1, expression.Length - 2);
            }

            // First try: treat it as a numeric expression using OUR Double() (so AppReal works correctly).
            try
            {
                double val = Double(program, expression);
                return val.ToString("0.0################", CultureInfo.InvariantCulture);
            }
            catch
            {
                // >>> ADD THIS BLOCK HERE (before EvaluateExpressionWithString)

                // Handle simple string concatenation like: "£"+y  or  "£" + (length*width)
                int plus = expression.IndexOf('+');
                if (plus > 0)
                {
                    string left = expression.Substring(0, plus).Trim();
                    string right = expression.Substring(plus + 1).Trim();

                    bool leftQuoted =
                        (left.StartsWith("\"") && left.EndsWith("\"")) ||
                        (left.StartsWith("'") && left.EndsWith("'"));

                    if (leftQuoted)
                    {
                        string prefix = left.Substring(1, left.Length - 2);
                        double val2 = Double(program, right);
                        return prefix + val2.ToString("0.0################", CultureInfo.InvariantCulture);
                    }
                }

                // <<< END ADD

                // Not numeric, so use BOOSE string evaluation (needed for other string expressions)
                string result = program.EvaluateExpressionWithString(expression);

                // If BOOSE returned a numeric string, format it consistently.
                if (double.TryParse(result, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
                    return d.ToString("0.0################", CultureInfo.InvariantCulture);

                return result;
            }
        }

    }
}
