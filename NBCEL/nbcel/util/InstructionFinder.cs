/*
* Licensed to the Apache Software Foundation (ASF) under one or more
* contributor license agreements.  See the NOTICE file distributed with
* this work for additional information regarding copyright ownership.
* The ASF licenses this file to You under the Apache License, Version 2.0
* (the "License"); you may not use this file except in compliance with
* the License.  You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License.
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NBCEL.generic;
using Sharpen;

namespace NBCEL.util
{
	/// <summary>
	///     InstructionFinder is a tool to search for given instructions patterns, i.e.,
	///     match sequences of instructions in an instruction list via regular
	///     expressions.
	/// </summary>
	/// <remarks>
	///     InstructionFinder is a tool to search for given instructions patterns, i.e.,
	///     match sequences of instructions in an instruction list via regular
	///     expressions. This can be used, e.g., in order to implement a peep hole
	///     optimizer that looks for code patterns and replaces them with faster
	///     equivalents.
	///     <p>
	///         This class internally uses the java.util.regex
	///         package to search for regular expressions.
	///         A typical application would look like this:
	///         <pre>
	///             InstructionFinder f   = new InstructionFinder(il);
	///             String            pat = &quot;IfInstruction ICONST_0 GOTO ICONST_1 NOP (IFEQ|IFNE)&quot;;
	///             for (Iterator i = f.search(pat, constraint); i.hasNext(); ) {
	///             InstructionHandle[] match = (InstructionHandle[])i.next();
	///             ...
	///             il.delete(match[1], match[5]);
	///             ...
	///             }
	///         </pre>
	/// </remarks>
	/// <seealso cref="NBCEL.generic.Instruction" />
	/// <seealso cref="NBCEL.generic.InstructionList" />
	public class InstructionFinder
    {
        private const int OFFSET = 32767;

        private const int NO_OPCODES = 256;

        private static readonly IDictionary<string, string> map
            = new Dictionary<string, string>();

        private readonly InstructionList il;

        private InstructionHandle[] handles;

        private string il_string;

        static InstructionFinder()
        {
            // Initialize pattern map
            Collections.Put(map, "arithmeticinstruction",
                "(irem|lrem|iand|ior|ineg|isub|lneg|fneg|fmul|ldiv|fadd|lxor|frem|idiv|land|ixor|ishr|fsub|lshl|fdiv|iadd|lor|dmul|lsub|ishl|imul|lmul|lushr|dneg|iushr|lshr|ddiv|drem|dadd|ladd|dsub)"
            );
            Collections.Put(map, "invokeinstruction",
                "(invokevirtual|invokeinterface|invokestatic|invokespecial|invokedynamic)"
            );
            Collections.Put(map, "arrayinstruction",
                "(baload|aastore|saload|caload|fastore|lastore|iaload|castore|iastore|aaload|bastore|sastore|faload|laload|daload|dastore)"
            );
            Collections.Put(map, "gotoinstruction", "(goto|goto_w)");
            Collections.Put(map, "conversioninstruction",
                "(d2l|l2d|i2s|d2i|l2i|i2b|l2f|d2f|f2i|i2d|i2l|f2d|i2c|f2l|i2f)"
            );
            Collections.Put(map, "localvariableinstruction",
                "(fstore|iinc|lload|dstore|dload|iload|aload|astore|istore|fload|lstore)"
            );
            Collections.Put(map, "loadinstruction", "(fload|dload|lload|iload|aload)"
            );
            Collections.Put(map, "fieldinstruction", "(getfield|putstatic|getstatic|putfield)"
            );
            Collections.Put(map, "cpinstruction",
                "(ldc2_w|invokeinterface|invokedynamic|multianewarray|putstatic|instanceof|getstatic|checkcast|getfield|invokespecial|ldc_w|invokestatic|invokevirtual|putfield|ldc|new|anewarray)"
            );
            Collections.Put(map, "stackinstruction", "(dup2|swap|dup2_x2|pop|pop2|dup|dup2_x1|dup_x2|dup_x1)"
            );
            Collections.Put(map, "branchinstruction",
                "(ifle|if_acmpne|if_icmpeq|if_acmpeq|ifnonnull|goto_w|iflt|ifnull|if_icmpne|tableswitch|if_icmple|ifeq|if_icmplt|jsr_w|if_icmpgt|ifgt|jsr|goto|ifne|ifge|lookupswitch|if_icmpge)"
            );
            Collections.Put(map, "returninstruction", "(lreturn|ireturn|freturn|dreturn|areturn|return)"
            );
            Collections.Put(map, "storeinstruction", "(istore|fstore|dstore|astore|lstore)"
            );
            Collections.Put(map, "select", "(tableswitch|lookupswitch)");
            Collections.Put(map, "ifinstruction",
                "(ifeq|ifgt|if_icmpne|if_icmpeq|ifge|ifnull|ifne|if_icmple|if_icmpge|if_acmpeq|if_icmplt|if_acmpne|ifnonnull|iflt|if_icmpgt|ifle)"
            );
            Collections.Put(map, "jsrinstruction", "(jsr|jsr_w)");
            Collections.Put(map, "variablelengthinstruction", "(tableswitch|jsr|goto|lookupswitch)"
            );
            Collections.Put(map, "unconditionalbranch", "(goto|jsr|jsr_w|athrow|goto_w)"
            );
            Collections.Put(map, "constantpushinstruction", "(dconst|bipush|sipush|fconst|iconst|lconst)"
            );
            Collections.Put(map, "typedinstruction",
                "(imul|lsub|aload|fload|lor|new|aaload|fcmpg|iand|iaload|lrem|idiv|d2l|isub|dcmpg|dastore|ret|f2d|f2i|drem|iinc|i2c|checkcast|frem|lreturn|astore|lushr|daload|dneg|fastore|istore|lshl|ldiv|lstore|areturn|ishr|ldc_w|invokeinterface|invokedynamic|aastore|lxor|ishl|l2d|i2f|return|faload|sipush|iushr|caload|instanceof|invokespecial|putfield|fmul|ireturn|laload|d2f|lneg|ixor|i2l|fdiv|lastore|multianewarray|i2b|getstatic|i2d|putstatic|fcmpl|saload|ladd|irem|dload|jsr_w|dconst|dcmpl|fsub|freturn|ldc|aconst_null|castore|lmul|ldc2_w|dadd|iconst|f2l|ddiv|dstore|land|jsr|anewarray|dmul|bipush|dsub|sastore|d2i|i2s|lshr|iadd|l2i|lload|bastore|fstore|fneg|iload|fadd|baload|fconst|ior|ineg|dreturn|l2f|lconst|getfield|invokevirtual|invokestatic|iastore)"
            );
            Collections.Put(map, "popinstruction", "(fstore|dstore|pop|pop2|astore|putstatic|istore|lstore)"
            );
            Collections.Put(map, "allocationinstruction", "(multianewarray|new|anewarray|newarray)"
            );
            Collections.Put(map, "indexedinstruction",
                "(lload|lstore|fload|ldc2_w|invokeinterface|invokedynamic|multianewarray|astore|dload|putstatic|instanceof|getstatic|checkcast|getfield|invokespecial|dstore|istore|iinc|ldc_w|ret|fstore|invokestatic|iload|putfield|invokevirtual|ldc|new|aload|anewarray)"
            );
            Collections.Put(map, "pushinstruction",
                "(dup|lload|dup2|bipush|fload|ldc2_w|sipush|lconst|fconst|dload|getstatic|ldc_w|aconst_null|dconst|iload|ldc|iconst|aload)"
            );
            Collections.Put(map, "stackproducer",
                "(imul|lsub|aload|fload|lor|new|aaload|fcmpg|iand|iaload|lrem|idiv|d2l|isub|dcmpg|dup|f2d|f2i|drem|i2c|checkcast|frem|lushr|daload|dneg|lshl|ldiv|ishr|ldc_w|invokeinterface|invokedynamic|lxor|ishl|l2d|i2f|faload|sipush|iushr|caload|instanceof|invokespecial|fmul|laload|d2f|lneg|ixor|i2l|fdiv|getstatic|i2b|swap|i2d|dup2|fcmpl|saload|ladd|irem|dload|jsr_w|dconst|dcmpl|fsub|ldc|arraylength|aconst_null|tableswitch|lmul|ldc2_w|iconst|dadd|f2l|ddiv|land|jsr|anewarray|dmul|bipush|dsub|d2i|newarray|i2s|lshr|iadd|lload|l2i|fneg|iload|fadd|baload|fconst|lookupswitch|ior|ineg|lconst|l2f|getfield|invokevirtual|invokestatic)"
            );
            Collections.Put(map, "stackconsumer",
                "(imul|lsub|lor|iflt|fcmpg|if_icmpgt|iand|ifeq|if_icmplt|lrem|ifnonnull|idiv|d2l|isub|dcmpg|dastore|if_icmpeq|f2d|f2i|drem|i2c|checkcast|frem|lreturn|astore|lushr|pop2|monitorexit|dneg|fastore|istore|lshl|ldiv|lstore|areturn|if_icmpge|ishr|monitorenter|invokeinterface|invokedynamic|aastore|lxor|ishl|l2d|i2f|return|iushr|instanceof|invokespecial|fmul|ireturn|d2f|lneg|ixor|pop|i2l|ifnull|fdiv|lastore|i2b|if_acmpeq|ifge|swap|i2d|putstatic|fcmpl|ladd|irem|dcmpl|fsub|freturn|ifgt|castore|lmul|dadd|f2l|ddiv|dstore|land|if_icmpne|if_acmpne|dmul|dsub|sastore|ifle|d2i|i2s|lshr|iadd|l2i|bastore|fstore|fneg|fadd|ior|ineg|ifne|dreturn|l2f|if_icmple|getfield|invokevirtual|invokestatic|iastore)"
            );
            Collections.Put(map, "exceptionthrower",
                "(irem|lrem|laload|putstatic|baload|dastore|areturn|getstatic|ldiv|anewarray|iastore|castore|idiv|saload|lastore|fastore|putfield|lreturn|caload|getfield|return|aastore|freturn|newarray|instanceof|multianewarray|athrow|faload|iaload|aaload|dreturn|monitorenter|checkcast|bastore|arraylength|new|invokevirtual|sastore|ldc_w|ireturn|invokespecial|monitorexit|invokeinterface|invokedynamic|ldc|invokestatic|daload)"
            );
            Collections.Put(map, "loadclass",
                "(multianewarray|invokeinterface|invokedynamic|instanceof|invokespecial|putfield|checkcast|putstatic|invokevirtual|new|getstatic|invokestatic|getfield|anewarray)"
            );
            Collections.Put(map, "instructiontargeter",
                "(ifle|if_acmpne|if_icmpeq|if_acmpeq|ifnonnull|goto_w|iflt|ifnull|if_icmpne|tableswitch|if_icmple|ifeq|if_icmplt|jsr_w|if_icmpgt|ifgt|jsr|goto|ifne|ifge|lookupswitch|if_icmpge)"
            );
            // Some aliases
            Collections.Put(map, "if_icmp", "(if_icmpne|if_icmpeq|if_icmple|if_icmpge|if_icmplt|if_icmpgt)"
            );
            Collections.Put(map, "if_acmp", "(if_acmpeq|if_acmpne)");
            Collections.Put(map, "if", "(ifeq|ifne|iflt|ifge|ifgt|ifle)");
            // Precompile some aliases first
            Collections.Put(map, "iconst", Precompile(Const.ICONST_0, Const
                .ICONST_5, Const.ICONST_M1));
            Collections.Put(map, "lconst", new string(new[]
            {
                '(', MakeChar(Const
                    .LCONST_0),
                '|', MakeChar(Const.LCONST_1), ')'
            }));
            Collections.Put(map, "dconst", new string(new[]
            {
                '(', MakeChar(Const
                    .DCONST_0),
                '|', MakeChar(Const.DCONST_1), ')'
            }));
            Collections.Put(map, "fconst", new string(new[]
            {
                '(', MakeChar(Const
                    .FCONST_0),
                '|', MakeChar(Const.FCONST_1), '|', MakeChar(Const.FCONST_2
                ),
                ')'
            }));
            Collections.Put(map, "lload", Precompile(Const.LLOAD_0, Const
                .LLOAD_3, Const.LLOAD));
            Collections.Put(map, "iload", Precompile(Const.ILOAD_0, Const
                .ILOAD_3, Const.ILOAD));
            Collections.Put(map, "dload", Precompile(Const.DLOAD_0, Const
                .DLOAD_3, Const.DLOAD));
            Collections.Put(map, "fload", Precompile(Const.FLOAD_0, Const
                .FLOAD_3, Const.FLOAD));
            Collections.Put(map, "aload", Precompile(Const.ALOAD_0, Const
                .ALOAD_3, Const.ALOAD));
            Collections.Put(map, "lstore", Precompile(Const.LSTORE_0, Const
                .LSTORE_3, Const.LSTORE));
            Collections.Put(map, "istore", Precompile(Const.ISTORE_0, Const
                .ISTORE_3, Const.ISTORE));
            Collections.Put(map, "dstore", Precompile(Const.DSTORE_0, Const
                .DSTORE_3, Const.DSTORE));
            Collections.Put(map, "fstore", Precompile(Const.FSTORE_0, Const
                .FSTORE_3, Const.FSTORE));
            Collections.Put(map, "astore", Precompile(Const.ASTORE_0, Const
                .ASTORE_3, Const.ASTORE));
            // Compile strings
            foreach (var entry in map)
            {
                var key = entry.Key;
                var value = entry.Value;
                var ch = value[1];
                // Omit already precompiled patterns
                if (ch < OFFSET) Collections.Put(map, key, CompilePattern(value));
            }

            // precompile all patterns
            // Add instruction alias to match anything
            var buf = new StringBuilder("(");
            for (short i = 0; i < NO_OPCODES; i++)
                if (Const.GetNoOfOperands(i) != Const.UNDEFINED)
                {
                    // Not an invalid opcode
                    buf.Append(MakeChar(i));
                    if (i < NO_OPCODES - 1) buf.Append('|');
                }

            buf.Append(')');
            Collections.Put(map, "instruction", buf.ToString());
        }

        /// <param name="il">instruction list to search for given patterns</param>
        public InstructionFinder(InstructionList il)
        {
            // char + OFFSET is outside of LATIN-1
            // Potential number, some are not used
            // instruction list as string
            // map instruction
            // list to array
            this.il = il;
            Reread();
        }

        /// <summary>
        ///     Reread the instruction list, e.g., after you've altered the list upon a
        ///     match.
        /// </summary>
        public void Reread()
        {
            var size = il.GetLength();
            var buf = new char[size];
            // Create a string with length equal to il length
            handles = il.GetInstructionHandles();
            // Map opcodes to characters
            for (var i = 0; i < size; i++) buf[i] = MakeChar(handles[i].GetInstruction().GetOpcode());
            il_string = new string(buf);
        }

        /// <summary>Map symbolic instruction names like "getfield" to a single character.</summary>
        /// <param name="pattern">instruction pattern in lower case</param>
        /// <returns>encoded string for a pattern such as "BranchInstruction".</returns>
        private static string MapName(string pattern)
        {
            var result = map.GetOrNull(pattern);
            if (result != null) return result;
            for (short i = 0; i < NO_OPCODES; i++)
                if (pattern.Equals(Const.GetOpcodeName(i)))
                    return string.Empty + MakeChar(i);
            throw new Exception("Instruction unknown: " + pattern);
        }

        /// <summary>
        ///     Replace symbolic names of instructions with the appropiate character and
        ///     remove all white space from string.
        /// </summary>
        /// <remarks>
        ///     Replace symbolic names of instructions with the appropiate character and
        ///     remove all white space from string. Meta characters such as +, * are
        ///     ignored.
        /// </remarks>
        /// <param name="pattern">The pattern to compile</param>
        /// <returns>translated regular expression string</returns>
        private static string CompilePattern(string pattern)
        {
            //Bug: BCEL-77 - Instructions are assumed to be english, to avoid odd Locale issues
            var lower = pattern.ToLower();
            var buf = new StringBuilder();
            var size = pattern.Length;
            for (var i = 0; i < size; i++)
            {
                var ch = lower[i];
                if (char.IsLetterOrDigit(ch))
                {
                    var name = new StringBuilder();
                    while ((char.IsLetterOrDigit(ch) || ch == '_') && i < size)
                    {
                        name.Append(ch);
                        if (++i < size)
                            ch = lower[i];
                        else
                            break;
                    }

                    i--;
                    buf.Append(MapName(name.ToString()));
                }
                else if (!char.IsWhiteSpace(ch))
                {
                    buf.Append(ch);
                }
            }

            return buf.ToString();
        }

        /// <returns>the matched piece of code as an array of instruction (handles)</returns>
        private InstructionHandle[] GetMatch(int matched_from, int match_length
        )
        {
            var match = new InstructionHandle[match_length
            ];
            Array.Copy(handles, matched_from, match, 0, match_length);
            return match;
        }

        /// <summary>Search for the given pattern in the instruction list.</summary>
        /// <remarks>
        ///     Search for the given pattern in the instruction list. You can search for
        ///     any valid opcode via its symbolic name, e.g. "istore". You can also use a
        ///     super class or an interface name to match a whole set of instructions, e.g.
        ///     "BranchInstruction" or "LoadInstruction". "istore" is also an alias for all
        ///     "istore_x" instructions. Additional aliases are "if" for "ifxx", "if_icmp"
        ///     for "if_icmpxx", "if_acmp" for "if_acmpxx".
        ///     Consecutive instruction names must be separated by white space which will
        ///     be removed during the compilation of the pattern.
        ///     For the rest the usual pattern matching rules for regular expressions
        ///     apply.
        ///     <P>
        ///         Example pattern:
        ///         <pre>
        ///             search(&quot;BranchInstruction NOP ((IfInstruction|GOTO)+ ISTORE Instruction)*&quot;);
        ///         </pre>
        ///         <p>
        ///             If you alter the instruction list upon a match such that other matching
        ///             areas are affected, you should call reread() to update the finder and call
        ///             search() again, because the matches are cached.
        /// </remarks>
        /// <param name="pattern">
        ///     the instruction pattern to search for, where case is ignored
        /// </param>
        /// <param name="from">where to start the search in the instruction list</param>
        /// <param name="constraint">
        ///     optional CodeConstraint to check the found code pattern for
        ///     user-defined constraints
        /// </param>
        /// <returns>
        ///     iterator of matches where e.nextElement() returns an array of
        ///     instruction handles describing the matched area
        /// </returns>
        public IEnumerator<InstructionHandle[]>
            Search(string pattern, InstructionHandle from, CodeConstraint
                constraint)
        {
            var search = CompilePattern(pattern);
            var start = -1;
            for (var i = 0; i < handles.Length; i++)
                if (handles[i] == from)
                {
                    start = i;
                    // Where to start search from (index)
                    break;
                }

            if (start == -1)
                throw new ClassGenException("Instruction handle " + from + " not found in instruction list."
                );
            var regex = new Regex(search, RegexOptions.Compiled);
            var matches = new
                List<InstructionHandle[]>();


            while (start < il_string.Length && regex.Matches(il_string, start).Count > 0)
            {
                var MATCH = regex.Matches(il_string, start).FirstOrDefault();
                var startExpr = MATCH.Index;
                var endExpr = MATCH.Index + MATCH.Length;
                var lenExpr = MATCH.Length;
                var match = GetMatch(startExpr, lenExpr);
                if (constraint == null || constraint.CheckCode(match)) matches.Add(match);
                start = endExpr;
            }

            return matches.GetEnumerator();
        }

        /// <summary>Start search beginning from the start of the given instruction list.</summary>
        /// <param name="pattern">
        ///     the instruction pattern to search for, where case is ignored
        /// </param>
        /// <returns>
        ///     iterator of matches where e.nextElement() returns an array of
        ///     instruction handles describing the matched area
        /// </returns>
        public IEnumerator<InstructionHandle[]>
            Search(string pattern)
        {
            return Search(pattern, il.GetStart(), null);
        }

        /// <summary>Start search beginning from `from'.</summary>
        /// <param name="pattern">
        ///     the instruction pattern to search for, where case is ignored
        /// </param>
        /// <param name="from">where to start the search in the instruction list</param>
        /// <returns>
        ///     iterator of matches where e.nextElement() returns an array of
        ///     instruction handles describing the matched area
        /// </returns>
        public IEnumerator<InstructionHandle[]>
            Search(string pattern, InstructionHandle from)
        {
            return Search(pattern, from, null);
        }

        /// <summary>Start search beginning from the start of the given instruction list.</summary>
        /// <remarks>
        ///     Start search beginning from the start of the given instruction list. Check
        ///     found matches with the constraint object.
        /// </remarks>
        /// <param name="pattern">the instruction pattern to search for, case is ignored</param>
        /// <param name="constraint">constraints to be checked on matching code</param>
        /// <returns>instruction handle or `null' if the match failed</returns>
        public IEnumerator<InstructionHandle[]>
            Search(string pattern, CodeConstraint constraint)
        {
            return Search(pattern, il.GetStart(), constraint);
        }

        /// <summary>Convert opcode number to char.</summary>
        private static char MakeChar(short opcode)
        {
            return (char) (opcode + OFFSET);
        }

        /// <returns>the inquired instruction list</returns>
        public InstructionList GetInstructionList()
        {
            return il;
        }

        private static string Precompile(short from, short to, short extra)
        {
            var buf = new StringBuilder("(");
            for (var i = from; i <= to; i++)
            {
                buf.Append(MakeChar(i));
                buf.Append('|');
            }

            buf.Append(MakeChar(extra));
            buf.Append(")");
            return buf.ToString();
        }

        /// <summary>
        ///     Code patterns found may be checked using an additional user-defined
        ///     constraint object whether they really match the needed criterion.
        /// </summary>
        /// <remarks>
        ///     Code patterns found may be checked using an additional user-defined
        ///     constraint object whether they really match the needed criterion. I.e.,
        ///     check constraints that can not expressed with regular expressions.
        /// </remarks>
        public interface CodeConstraint
        {
	        /// <param name="match">array of instructions matching the requested pattern</param>
	        /// <returns>true if the matched area is really useful</returns>
	        bool CheckCode(InstructionHandle[] match);
        }

        /*
        * Internal debugging routines.
        */
        //    private static final String pattern2string( String pattern ) {
        //        return pattern2string(pattern, true);
        //    }
        //    private static final String pattern2string( String pattern, boolean make_string ) {
        //        StringBuffer buf = new StringBuffer();
        //        for (int i = 0; i < pattern.length(); i++) {
        //            char ch = pattern.charAt(i);
        //            if (ch >= OFFSET) {
        //                if (make_string) {
        //                    buf.append(Constants.getOpcodeName(ch - OFFSET));
        //                } else {
        //                    buf.append((ch - OFFSET));
        //                }
        //            } else {
        //                buf.append(ch);
        //            }
        //        }
        //        return buf.toString();
        //    }
    }
}