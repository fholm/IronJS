// $ANTLR 3.2 Sep 23, 2009 12:02:23 ES3.g3 2009-12-30 11:53:13

#pragma warning disable 168, 219
#pragma warning disable 162
#pragma warning disable 219, 162

using System;
using Antlr.Runtime;

using IList = System.Collections.IList;
using ArrayList = System.Collections.ArrayList;
using Stack = Antlr.Runtime.Collections.StackList;

namespace IronJS.Compiler.Parser
{
    partial class ES3Lexer : Lexer
    {
        public const int PACKAGE = 52;
        public const int FUNCTION = 17;
        public const int LOR = 95;
        public const int VT = 134;
        public const int SHR = 87;
        public const int RegularExpressionChar = 170;
        public const int LT = 72;
        public const int WHILE = 30;
        public const int MOD = 83;
        public const int SHL = 86;
        public const int CONST = 37;
        public const int BackslashSequence = 168;
        public const int LS = 142;
        public const int CASE = 8;
        public const int CHAR = 35;
        public const int NEW = 21;
        public const int DO = 13;
        public const int DQUOTE = 131;
        public const int NOT = 92;
        public const int DecimalDigit = 152;
        public const int BYFIELD = 114;
        public const int EOF = -1;
        public const int BREAK = 7;
        public const int CEXPR = 117;
        public const int DIVASS = 110;
        public const int Identifier = 148;
        public const int BYINDEX = 115;
        public const int INC = 84;
        public const int RPAREN = 66;
        public const int FINAL = 43;
        public const int FORSTEP = 120;
        public const int IMPORT = 47;
        public const int EOL = 145;
        public const int POS = 129;
        public const int OctalDigit = 156;
        public const int RETURN = 22;
        public const int THIS = 24;
        public const int DOUBLE = 39;
        public const int ARGS = 111;
        public const int ExponentPart = 157;
        public const int WhiteSpace = 139;
        public const int VAR = 28;
        public const int EXPORT = 41;
        public const int VOID = 29;
        public const int LABELLED = 122;
        public const int SUPER = 58;
        public const int GOTO = 45;
        public const int EQ = 76;
        public const int XORASS = 108;
        public const int ADDASS = 99;
        public const int ARRAY = 112;
        public const int SHU = 88;
        public const int RBRACK = 68;
        public const int RBRACE = 64;
        public const int PRIVATE = 53;
        public const int STATIC = 57;
        public const int INV = 93;
        public const int SWITCH = 23;
        public const int NULL = 4;
        public const int ELSE = 14;
        public const int NATIVE = 51;
        public const int THROWS = 60;
        public const int INT = 48;
        public const int DELETE = 12;
        public const int MUL = 82;
        public const int IdentifierStartASCII = 151;
        public const int TRY = 26;
        public const int FF = 135;
        public const int SHLASS = 103;
        public const int OctalEscapeSequence = 164;
        public const int USP = 138;
        public const int RegularExpressionFirstChar = 169;
        public const int ANDASS = 106;
        public const int TYPEOF = 27;
        public const int IdentifierNameASCIIStart = 154;
        public const int QUE = 96;
        public const int OR = 90;
        public const int DEBUGGER = 38;
        public const int GT = 73;
        public const int PDEC = 127;
        public const int CALL = 116;
        public const int CharacterEscapeSequence = 162;
        public const int CATCH = 9;
        public const int FALSE = 6;
        public const int EscapeSequence = 167;
        public const int LAND = 94;
        public const int MULASS = 101;
        public const int THROW = 25;
        public const int PINC = 128;
        public const int DEC = 85;
        public const int PROTECTED = 54;
        public const int OctalIntegerLiteral = 160;
        public const int CLASS = 36;
        public const int LBRACK = 67;
        public const int ORASS = 107;
        public const int HexEscapeSequence = 165;
        public const int NAMEDVALUE = 123;
        public const int SingleLineComment = 147;
        public const int GTE = 75;
        public const int LBRACE = 63;
        public const int FOR = 16;
        public const int SUB = 81;
        public const int RegularExpressionLiteral = 155;
        public const int FLOAT = 44;
        public const int ABSTRACT = 32;
        public const int AND = 89;
        public const int DecimalIntegerLiteral = 158;
        public const int LTE = 74;
        public const int HexDigit = 150;
        public const int LPAREN = 65;
        public const int IF = 18;
        public const int SUBASS = 100;
        public const int SYNCHRONIZED = 59;
        public const int BOOLEAN = 33;
        public const int EXPR = 118;
        public const int IN = 19;
        public const int IMPLEMENTS = 46;
        public const int CONTINUE = 10;
        public const int OBJECT = 125;
        public const int COMMA = 71;
        public const int TRANSIENT = 61;
        public const int FORITER = 119;
        public const int MODASS = 102;
        public const int SHRASS = 104;
        public const int PS = 143;
        public const int DOT = 69;
        public const int MultiLineComment = 146;
        public const int IdentifierPart = 153;
        public const int WITH = 31;
        public const int ADD = 80;
        public const int BYTE = 34;
        public const int XOR = 91;
        public const int ZeroToThree = 163;
        public const int VOLATILE = 62;
        public const int ITEM = 121;
        public const int UnicodeEscapeSequence = 166;
        public const int NSAME = 79;
        public const int DEFAULT = 11;
        public const int SHUASS = 105;
        public const int TAB = 133;
        public const int SHORT = 56;
        public const int INSTANCEOF = 20;
        public const int SQUOTE = 132;
        public const int DecimalLiteral = 159;
        public const int TRUE = 5;
        public const int SAME = 78;
        public const int COLON = 97;
        public const int StringLiteral = 149;
        public const int NEQ = 77;
        public const int PAREXPR = 126;
        public const int ENUM = 40;
        public const int FINALLY = 15;
        public const int NBSP = 137;
        public const int HexIntegerLiteral = 161;
        public const int SP = 136;
        public const int BLOCK = 113;
        public const int NEG = 124;
        public const int LineTerminator = 144;
        public const int ASSIGN = 98;
        public const int INTERFACE = 49;
        public const int DIV = 109;
        public const int SEMIC = 70;
        public const int LONG = 50;
        public const int CR = 141;
        public const int PUBLIC = 55;
        public const int EXTENDS = 42;
        public const int BSLASH = 130;
        public const int LF = 140;

        // delegates
        // delegators

        public ES3Lexer()
        {
            InitializeCyclicDFAs();
        }
        public ES3Lexer(ICharStream input)
            : this(input, null)
        {
        }
        public ES3Lexer(ICharStream input, RecognizerSharedState state)
            : base(input, state)
        {
            InitializeCyclicDFAs();

        }

        override public string GrammarFileName
        {
            get { return "ES3.g3"; }
        }

        // $ANTLR start "NULL"
        public void mNULL() // throws RecognitionException [2]
        {
            try
            {
                int _type = NULL;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:10:6: ( 'null' )
                // ES3.g3:10:8: 'null'
                {
                    Match("null");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "NULL"

        // $ANTLR start "TRUE"
        public void mTRUE() // throws RecognitionException [2]
        {
            try
            {
                int _type = TRUE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:11:6: ( 'true' )
                // ES3.g3:11:8: 'true'
                {
                    Match("true");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "TRUE"

        // $ANTLR start "FALSE"
        public void mFALSE() // throws RecognitionException [2]
        {
            try
            {
                int _type = FALSE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:12:7: ( 'false' )
                // ES3.g3:12:9: 'false'
                {
                    Match("false");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "FALSE"

        // $ANTLR start "BREAK"
        public void mBREAK() // throws RecognitionException [2]
        {
            try
            {
                int _type = BREAK;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:13:7: ( 'break' )
                // ES3.g3:13:9: 'break'
                {
                    Match("break");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "BREAK"

        // $ANTLR start "CASE"
        public void mCASE() // throws RecognitionException [2]
        {
            try
            {
                int _type = CASE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:14:6: ( 'case' )
                // ES3.g3:14:8: 'case'
                {
                    Match("case");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "CASE"

        // $ANTLR start "CATCH"
        public void mCATCH() // throws RecognitionException [2]
        {
            try
            {
                int _type = CATCH;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:15:7: ( 'catch' )
                // ES3.g3:15:9: 'catch'
                {
                    Match("catch");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "CATCH"

        // $ANTLR start "CONTINUE"
        public void mCONTINUE() // throws RecognitionException [2]
        {
            try
            {
                int _type = CONTINUE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:16:10: ( 'continue' )
                // ES3.g3:16:12: 'continue'
                {
                    Match("continue");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "CONTINUE"

        // $ANTLR start "DEFAULT"
        public void mDEFAULT() // throws RecognitionException [2]
        {
            try
            {
                int _type = DEFAULT;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:17:9: ( 'default' )
                // ES3.g3:17:11: 'default'
                {
                    Match("default");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "DEFAULT"

        // $ANTLR start "DELETE"
        public void mDELETE() // throws RecognitionException [2]
        {
            try
            {
                int _type = DELETE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:18:8: ( 'delete' )
                // ES3.g3:18:10: 'delete'
                {
                    Match("delete");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "DELETE"

        // $ANTLR start "DO"
        public void mDO() // throws RecognitionException [2]
        {
            try
            {
                int _type = DO;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:19:4: ( 'do' )
                // ES3.g3:19:6: 'do'
                {
                    Match("do");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "DO"

        // $ANTLR start "ELSE"
        public void mELSE() // throws RecognitionException [2]
        {
            try
            {
                int _type = ELSE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:20:6: ( 'else' )
                // ES3.g3:20:8: 'else'
                {
                    Match("else");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "ELSE"

        // $ANTLR start "FINALLY"
        public void mFINALLY() // throws RecognitionException [2]
        {
            try
            {
                int _type = FINALLY;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:21:9: ( 'finally' )
                // ES3.g3:21:11: 'finally'
                {
                    Match("finally");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "FINALLY"

        // $ANTLR start "FOR"
        public void mFOR() // throws RecognitionException [2]
        {
            try
            {
                int _type = FOR;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:22:5: ( 'for' )
                // ES3.g3:22:7: 'for'
                {
                    Match("for");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "FOR"

        // $ANTLR start "FUNCTION"
        public void mFUNCTION() // throws RecognitionException [2]
        {
            try
            {
                int _type = FUNCTION;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:23:10: ( 'function' )
                // ES3.g3:23:12: 'function'
                {
                    Match("function");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "FUNCTION"

        // $ANTLR start "IF"
        public void mIF() // throws RecognitionException [2]
        {
            try
            {
                int _type = IF;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:24:4: ( 'if' )
                // ES3.g3:24:6: 'if'
                {
                    Match("if");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "IF"

        // $ANTLR start "IN"
        public void mIN() // throws RecognitionException [2]
        {
            try
            {
                int _type = IN;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:25:4: ( 'in' )
                // ES3.g3:25:6: 'in'
                {
                    Match("in");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "IN"

        // $ANTLR start "INSTANCEOF"
        public void mINSTANCEOF() // throws RecognitionException [2]
        {
            try
            {
                int _type = INSTANCEOF;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:26:12: ( 'instanceof' )
                // ES3.g3:26:14: 'instanceof'
                {
                    Match("instanceof");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "INSTANCEOF"

        // $ANTLR start "NEW"
        public void mNEW() // throws RecognitionException [2]
        {
            try
            {
                int _type = NEW;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:27:5: ( 'new' )
                // ES3.g3:27:7: 'new'
                {
                    Match("new");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "NEW"

        // $ANTLR start "RETURN"
        public void mRETURN() // throws RecognitionException [2]
        {
            try
            {
                int _type = RETURN;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:28:8: ( 'return' )
                // ES3.g3:28:10: 'return'
                {
                    Match("return");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "RETURN"

        // $ANTLR start "SWITCH"
        public void mSWITCH() // throws RecognitionException [2]
        {
            try
            {
                int _type = SWITCH;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:29:8: ( 'switch' )
                // ES3.g3:29:10: 'switch'
                {
                    Match("switch");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "SWITCH"

        // $ANTLR start "THIS"
        public void mTHIS() // throws RecognitionException [2]
        {
            try
            {
                int _type = THIS;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:30:6: ( 'this' )
                // ES3.g3:30:8: 'this'
                {
                    Match("this");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "THIS"

        // $ANTLR start "THROW"
        public void mTHROW() // throws RecognitionException [2]
        {
            try
            {
                int _type = THROW;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:31:7: ( 'throw' )
                // ES3.g3:31:9: 'throw'
                {
                    Match("throw");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "THROW"

        // $ANTLR start "TRY"
        public void mTRY() // throws RecognitionException [2]
        {
            try
            {
                int _type = TRY;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:32:5: ( 'try' )
                // ES3.g3:32:7: 'try'
                {
                    Match("try");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "TRY"

        // $ANTLR start "TYPEOF"
        public void mTYPEOF() // throws RecognitionException [2]
        {
            try
            {
                int _type = TYPEOF;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:33:8: ( 'typeof' )
                // ES3.g3:33:10: 'typeof'
                {
                    Match("typeof");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "TYPEOF"

        // $ANTLR start "VAR"
        public void mVAR() // throws RecognitionException [2]
        {
            try
            {
                int _type = VAR;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:34:5: ( 'var' )
                // ES3.g3:34:7: 'var'
                {
                    Match("var");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "VAR"

        // $ANTLR start "VOID"
        public void mVOID() // throws RecognitionException [2]
        {
            try
            {
                int _type = VOID;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:35:6: ( 'void' )
                // ES3.g3:35:8: 'void'
                {
                    Match("void");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "VOID"

        // $ANTLR start "WHILE"
        public void mWHILE() // throws RecognitionException [2]
        {
            try
            {
                int _type = WHILE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:36:7: ( 'while' )
                // ES3.g3:36:9: 'while'
                {
                    Match("while");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "WHILE"

        // $ANTLR start "WITH"
        public void mWITH() // throws RecognitionException [2]
        {
            try
            {
                int _type = WITH;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:37:6: ( 'with' )
                // ES3.g3:37:8: 'with'
                {
                    Match("with");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "WITH"

        // $ANTLR start "ABSTRACT"
        public void mABSTRACT() // throws RecognitionException [2]
        {
            try
            {
                int _type = ABSTRACT;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:38:10: ( 'abstract' )
                // ES3.g3:38:12: 'abstract'
                {
                    Match("abstract");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "ABSTRACT"

        // $ANTLR start "BOOLEAN"
        public void mBOOLEAN() // throws RecognitionException [2]
        {
            try
            {
                int _type = BOOLEAN;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:39:9: ( 'boolean' )
                // ES3.g3:39:11: 'boolean'
                {
                    Match("boolean");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "BOOLEAN"

        // $ANTLR start "BYTE"
        public void mBYTE() // throws RecognitionException [2]
        {
            try
            {
                int _type = BYTE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:40:6: ( 'byte' )
                // ES3.g3:40:8: 'byte'
                {
                    Match("byte");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "BYTE"

        // $ANTLR start "CHAR"
        public void mCHAR() // throws RecognitionException [2]
        {
            try
            {
                int _type = CHAR;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:41:6: ( 'char' )
                // ES3.g3:41:8: 'char'
                {
                    Match("char");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "CHAR"

        // $ANTLR start "CLASS"
        public void mCLASS() // throws RecognitionException [2]
        {
            try
            {
                int _type = CLASS;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:42:7: ( 'class' )
                // ES3.g3:42:9: 'class'
                {
                    Match("class");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "CLASS"

        // $ANTLR start "CONST"
        public void mCONST() // throws RecognitionException [2]
        {
            try
            {
                int _type = CONST;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:43:7: ( 'const' )
                // ES3.g3:43:9: 'const'
                {
                    Match("const");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "CONST"

        // $ANTLR start "DEBUGGER"
        public void mDEBUGGER() // throws RecognitionException [2]
        {
            try
            {
                int _type = DEBUGGER;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:44:10: ( 'debugger' )
                // ES3.g3:44:12: 'debugger'
                {
                    Match("debugger");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "DEBUGGER"

        // $ANTLR start "DOUBLE"
        public void mDOUBLE() // throws RecognitionException [2]
        {
            try
            {
                int _type = DOUBLE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:45:8: ( 'double' )
                // ES3.g3:45:10: 'double'
                {
                    Match("double");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "DOUBLE"

        // $ANTLR start "ENUM"
        public void mENUM() // throws RecognitionException [2]
        {
            try
            {
                int _type = ENUM;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:46:6: ( 'enum' )
                // ES3.g3:46:8: 'enum'
                {
                    Match("enum");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "ENUM"

        // $ANTLR start "EXPORT"
        public void mEXPORT() // throws RecognitionException [2]
        {
            try
            {
                int _type = EXPORT;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:47:8: ( 'export' )
                // ES3.g3:47:10: 'export'
                {
                    Match("export");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "EXPORT"

        // $ANTLR start "EXTENDS"
        public void mEXTENDS() // throws RecognitionException [2]
        {
            try
            {
                int _type = EXTENDS;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:48:9: ( 'extends' )
                // ES3.g3:48:11: 'extends'
                {
                    Match("extends");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "EXTENDS"

        // $ANTLR start "FINAL"
        public void mFINAL() // throws RecognitionException [2]
        {
            try
            {
                int _type = FINAL;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:49:7: ( 'final' )
                // ES3.g3:49:9: 'final'
                {
                    Match("final");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "FINAL"

        // $ANTLR start "FLOAT"
        public void mFLOAT() // throws RecognitionException [2]
        {
            try
            {
                int _type = FLOAT;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:50:7: ( 'float' )
                // ES3.g3:50:9: 'float'
                {
                    Match("float");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "FLOAT"

        // $ANTLR start "GOTO"
        public void mGOTO() // throws RecognitionException [2]
        {
            try
            {
                int _type = GOTO;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:51:6: ( 'goto' )
                // ES3.g3:51:8: 'goto'
                {
                    Match("goto");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "GOTO"

        // $ANTLR start "IMPLEMENTS"
        public void mIMPLEMENTS() // throws RecognitionException [2]
        {
            try
            {
                int _type = IMPLEMENTS;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:52:12: ( 'implements' )
                // ES3.g3:52:14: 'implements'
                {
                    Match("implements");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "IMPLEMENTS"

        // $ANTLR start "IMPORT"
        public void mIMPORT() // throws RecognitionException [2]
        {
            try
            {
                int _type = IMPORT;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:53:8: ( 'import' )
                // ES3.g3:53:10: 'import'
                {
                    Match("import");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "IMPORT"

        // $ANTLR start "INT"
        public void mINT() // throws RecognitionException [2]
        {
            try
            {
                int _type = INT;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:54:5: ( 'int' )
                // ES3.g3:54:7: 'int'
                {
                    Match("int");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "INT"

        // $ANTLR start "INTERFACE"
        public void mINTERFACE() // throws RecognitionException [2]
        {
            try
            {
                int _type = INTERFACE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:55:11: ( 'interface' )
                // ES3.g3:55:13: 'interface'
                {
                    Match("interface");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "INTERFACE"

        // $ANTLR start "LONG"
        public void mLONG() // throws RecognitionException [2]
        {
            try
            {
                int _type = LONG;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:56:6: ( 'long' )
                // ES3.g3:56:8: 'long'
                {
                    Match("long");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "LONG"

        // $ANTLR start "NATIVE"
        public void mNATIVE() // throws RecognitionException [2]
        {
            try
            {
                int _type = NATIVE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:57:8: ( 'native' )
                // ES3.g3:57:10: 'native'
                {
                    Match("native");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "NATIVE"

        // $ANTLR start "PACKAGE"
        public void mPACKAGE() // throws RecognitionException [2]
        {
            try
            {
                int _type = PACKAGE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:58:9: ( 'package' )
                // ES3.g3:58:11: 'package'
                {
                    Match("package");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "PACKAGE"

        // $ANTLR start "PRIVATE"
        public void mPRIVATE() // throws RecognitionException [2]
        {
            try
            {
                int _type = PRIVATE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:59:9: ( 'private' )
                // ES3.g3:59:11: 'private'
                {
                    Match("private");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "PRIVATE"

        // $ANTLR start "PROTECTED"
        public void mPROTECTED() // throws RecognitionException [2]
        {
            try
            {
                int _type = PROTECTED;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:60:11: ( 'protected' )
                // ES3.g3:60:13: 'protected'
                {
                    Match("protected");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "PROTECTED"

        // $ANTLR start "PUBLIC"
        public void mPUBLIC() // throws RecognitionException [2]
        {
            try
            {
                int _type = PUBLIC;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:61:8: ( 'public' )
                // ES3.g3:61:10: 'public'
                {
                    Match("public");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "PUBLIC"

        // $ANTLR start "SHORT"
        public void mSHORT() // throws RecognitionException [2]
        {
            try
            {
                int _type = SHORT;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:62:7: ( 'short' )
                // ES3.g3:62:9: 'short'
                {
                    Match("short");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "SHORT"

        // $ANTLR start "STATIC"
        public void mSTATIC() // throws RecognitionException [2]
        {
            try
            {
                int _type = STATIC;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:63:8: ( 'static' )
                // ES3.g3:63:10: 'static'
                {
                    Match("static");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "STATIC"

        // $ANTLR start "SUPER"
        public void mSUPER() // throws RecognitionException [2]
        {
            try
            {
                int _type = SUPER;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:64:7: ( 'super' )
                // ES3.g3:64:9: 'super'
                {
                    Match("super");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "SUPER"

        // $ANTLR start "SYNCHRONIZED"
        public void mSYNCHRONIZED() // throws RecognitionException [2]
        {
            try
            {
                int _type = SYNCHRONIZED;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:65:14: ( 'synchronized' )
                // ES3.g3:65:16: 'synchronized'
                {
                    Match("synchronized");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "SYNCHRONIZED"

        // $ANTLR start "THROWS"
        public void mTHROWS() // throws RecognitionException [2]
        {
            try
            {
                int _type = THROWS;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:66:8: ( 'throws' )
                // ES3.g3:66:10: 'throws'
                {
                    Match("throws");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "THROWS"

        // $ANTLR start "TRANSIENT"
        public void mTRANSIENT() // throws RecognitionException [2]
        {
            try
            {
                int _type = TRANSIENT;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:67:11: ( 'transient' )
                // ES3.g3:67:13: 'transient'
                {
                    Match("transient");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "TRANSIENT"

        // $ANTLR start "VOLATILE"
        public void mVOLATILE() // throws RecognitionException [2]
        {
            try
            {
                int _type = VOLATILE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:68:10: ( 'volatile' )
                // ES3.g3:68:12: 'volatile'
                {
                    Match("volatile");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "VOLATILE"

        // $ANTLR start "LBRACE"
        public void mLBRACE() // throws RecognitionException [2]
        {
            try
            {
                int _type = LBRACE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:69:8: ( '{' )
                // ES3.g3:69:10: '{'
                {
                    Match('{');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "LBRACE"

        // $ANTLR start "RBRACE"
        public void mRBRACE() // throws RecognitionException [2]
        {
            try
            {
                int _type = RBRACE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:70:8: ( '}' )
                // ES3.g3:70:10: '}'
                {
                    Match('}');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "RBRACE"

        // $ANTLR start "LPAREN"
        public void mLPAREN() // throws RecognitionException [2]
        {
            try
            {
                int _type = LPAREN;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:71:8: ( '(' )
                // ES3.g3:71:10: '('
                {
                    Match('(');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "LPAREN"

        // $ANTLR start "RPAREN"
        public void mRPAREN() // throws RecognitionException [2]
        {
            try
            {
                int _type = RPAREN;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:72:8: ( ')' )
                // ES3.g3:72:10: ')'
                {
                    Match(')');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "RPAREN"

        // $ANTLR start "LBRACK"
        public void mLBRACK() // throws RecognitionException [2]
        {
            try
            {
                int _type = LBRACK;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:73:8: ( '[' )
                // ES3.g3:73:10: '['
                {
                    Match('[');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "LBRACK"

        // $ANTLR start "RBRACK"
        public void mRBRACK() // throws RecognitionException [2]
        {
            try
            {
                int _type = RBRACK;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:74:8: ( ']' )
                // ES3.g3:74:10: ']'
                {
                    Match(']');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "RBRACK"

        // $ANTLR start "DOT"
        public void mDOT() // throws RecognitionException [2]
        {
            try
            {
                int _type = DOT;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:75:5: ( '.' )
                // ES3.g3:75:7: '.'
                {
                    Match('.');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "DOT"

        // $ANTLR start "SEMIC"
        public void mSEMIC() // throws RecognitionException [2]
        {
            try
            {
                int _type = SEMIC;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:76:7: ( ';' )
                // ES3.g3:76:9: ';'
                {
                    Match(';');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "SEMIC"

        // $ANTLR start "COMMA"
        public void mCOMMA() // throws RecognitionException [2]
        {
            try
            {
                int _type = COMMA;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:77:7: ( ',' )
                // ES3.g3:77:9: ','
                {
                    Match(',');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "COMMA"

        // $ANTLR start "LT"
        public void mLT() // throws RecognitionException [2]
        {
            try
            {
                int _type = LT;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:78:4: ( '<' )
                // ES3.g3:78:6: '<'
                {
                    Match('<');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "LT"

        // $ANTLR start "GT"
        public void mGT() // throws RecognitionException [2]
        {
            try
            {
                int _type = GT;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:79:4: ( '>' )
                // ES3.g3:79:6: '>'
                {
                    Match('>');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "GT"

        // $ANTLR start "LTE"
        public void mLTE() // throws RecognitionException [2]
        {
            try
            {
                int _type = LTE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:80:5: ( '<=' )
                // ES3.g3:80:7: '<='
                {
                    Match("<=");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "LTE"

        // $ANTLR start "GTE"
        public void mGTE() // throws RecognitionException [2]
        {
            try
            {
                int _type = GTE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:81:5: ( '>=' )
                // ES3.g3:81:7: '>='
                {
                    Match(">=");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "GTE"

        // $ANTLR start "EQ"
        public void mEQ() // throws RecognitionException [2]
        {
            try
            {
                int _type = EQ;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:82:4: ( '==' )
                // ES3.g3:82:6: '=='
                {
                    Match("==");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "EQ"

        // $ANTLR start "NEQ"
        public void mNEQ() // throws RecognitionException [2]
        {
            try
            {
                int _type = NEQ;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:83:5: ( '!=' )
                // ES3.g3:83:7: '!='
                {
                    Match("!=");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "NEQ"

        // $ANTLR start "SAME"
        public void mSAME() // throws RecognitionException [2]
        {
            try
            {
                int _type = SAME;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:84:6: ( '===' )
                // ES3.g3:84:8: '==='
                {
                    Match("===");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "SAME"

        // $ANTLR start "NSAME"
        public void mNSAME() // throws RecognitionException [2]
        {
            try
            {
                int _type = NSAME;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:85:7: ( '!==' )
                // ES3.g3:85:9: '!=='
                {
                    Match("!==");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "NSAME"

        // $ANTLR start "ADD"
        public void mADD() // throws RecognitionException [2]
        {
            try
            {
                int _type = ADD;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:86:5: ( '+' )
                // ES3.g3:86:7: '+'
                {
                    Match('+');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "ADD"

        // $ANTLR start "SUB"
        public void mSUB() // throws RecognitionException [2]
        {
            try
            {
                int _type = SUB;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:87:5: ( '-' )
                // ES3.g3:87:7: '-'
                {
                    Match('-');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "SUB"

        // $ANTLR start "MUL"
        public void mMUL() // throws RecognitionException [2]
        {
            try
            {
                int _type = MUL;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:88:5: ( '*' )
                // ES3.g3:88:7: '*'
                {
                    Match('*');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "MUL"

        // $ANTLR start "MOD"
        public void mMOD() // throws RecognitionException [2]
        {
            try
            {
                int _type = MOD;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:89:5: ( '%' )
                // ES3.g3:89:7: '%'
                {
                    Match('%');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "MOD"

        // $ANTLR start "INC"
        public void mINC() // throws RecognitionException [2]
        {
            try
            {
                int _type = INC;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:90:5: ( '++' )
                // ES3.g3:90:7: '++'
                {
                    Match("++");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "INC"

        // $ANTLR start "DEC"
        public void mDEC() // throws RecognitionException [2]
        {
            try
            {
                int _type = DEC;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:91:5: ( '--' )
                // ES3.g3:91:7: '--'
                {
                    Match("--");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "DEC"

        // $ANTLR start "SHL"
        public void mSHL() // throws RecognitionException [2]
        {
            try
            {
                int _type = SHL;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:92:5: ( '<<' )
                // ES3.g3:92:7: '<<'
                {
                    Match("<<");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "SHL"

        // $ANTLR start "SHR"
        public void mSHR() // throws RecognitionException [2]
        {
            try
            {
                int _type = SHR;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:93:5: ( '>>' )
                // ES3.g3:93:7: '>>'
                {
                    Match(">>");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "SHR"

        // $ANTLR start "SHU"
        public void mSHU() // throws RecognitionException [2]
        {
            try
            {
                int _type = SHU;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:94:5: ( '>>>' )
                // ES3.g3:94:7: '>>>'
                {
                    Match(">>>");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "SHU"

        // $ANTLR start "AND"
        public void mAND() // throws RecognitionException [2]
        {
            try
            {
                int _type = AND;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:95:5: ( '&' )
                // ES3.g3:95:7: '&'
                {
                    Match('&');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "AND"

        // $ANTLR start "OR"
        public void mOR() // throws RecognitionException [2]
        {
            try
            {
                int _type = OR;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:96:4: ( '|' )
                // ES3.g3:96:6: '|'
                {
                    Match('|');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "OR"

        // $ANTLR start "XOR"
        public void mXOR() // throws RecognitionException [2]
        {
            try
            {
                int _type = XOR;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:97:5: ( '^' )
                // ES3.g3:97:7: '^'
                {
                    Match('^');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "XOR"

        // $ANTLR start "NOT"
        public void mNOT() // throws RecognitionException [2]
        {
            try
            {
                int _type = NOT;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:98:5: ( '!' )
                // ES3.g3:98:7: '!'
                {
                    Match('!');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "NOT"

        // $ANTLR start "INV"
        public void mINV() // throws RecognitionException [2]
        {
            try
            {
                int _type = INV;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:99:5: ( '~' )
                // ES3.g3:99:7: '~'
                {
                    Match('~');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "INV"

        // $ANTLR start "LAND"
        public void mLAND() // throws RecognitionException [2]
        {
            try
            {
                int _type = LAND;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:100:6: ( '&&' )
                // ES3.g3:100:8: '&&'
                {
                    Match("&&");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "LAND"

        // $ANTLR start "LOR"
        public void mLOR() // throws RecognitionException [2]
        {
            try
            {
                int _type = LOR;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:101:5: ( '||' )
                // ES3.g3:101:7: '||'
                {
                    Match("||");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "LOR"

        // $ANTLR start "QUE"
        public void mQUE() // throws RecognitionException [2]
        {
            try
            {
                int _type = QUE;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:102:5: ( '?' )
                // ES3.g3:102:7: '?'
                {
                    Match('?');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "QUE"

        // $ANTLR start "COLON"
        public void mCOLON() // throws RecognitionException [2]
        {
            try
            {
                int _type = COLON;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:103:7: ( ':' )
                // ES3.g3:103:9: ':'
                {
                    Match(':');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "COLON"

        // $ANTLR start "ASSIGN"
        public void mASSIGN() // throws RecognitionException [2]
        {
            try
            {
                int _type = ASSIGN;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:104:8: ( '=' )
                // ES3.g3:104:10: '='
                {
                    Match('=');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "ASSIGN"

        // $ANTLR start "ADDASS"
        public void mADDASS() // throws RecognitionException [2]
        {
            try
            {
                int _type = ADDASS;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:105:8: ( '+=' )
                // ES3.g3:105:10: '+='
                {
                    Match("+=");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "ADDASS"

        // $ANTLR start "SUBASS"
        public void mSUBASS() // throws RecognitionException [2]
        {
            try
            {
                int _type = SUBASS;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:106:8: ( '-=' )
                // ES3.g3:106:10: '-='
                {
                    Match("-=");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "SUBASS"

        // $ANTLR start "MULASS"
        public void mMULASS() // throws RecognitionException [2]
        {
            try
            {
                int _type = MULASS;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:107:8: ( '*=' )
                // ES3.g3:107:10: '*='
                {
                    Match("*=");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "MULASS"

        // $ANTLR start "MODASS"
        public void mMODASS() // throws RecognitionException [2]
        {
            try
            {
                int _type = MODASS;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:108:8: ( '%=' )
                // ES3.g3:108:10: '%='
                {
                    Match("%=");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "MODASS"

        // $ANTLR start "SHLASS"
        public void mSHLASS() // throws RecognitionException [2]
        {
            try
            {
                int _type = SHLASS;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:109:8: ( '<<=' )
                // ES3.g3:109:10: '<<='
                {
                    Match("<<=");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "SHLASS"

        // $ANTLR start "SHRASS"
        public void mSHRASS() // throws RecognitionException [2]
        {
            try
            {
                int _type = SHRASS;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:110:8: ( '>>=' )
                // ES3.g3:110:10: '>>='
                {
                    Match(">>=");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "SHRASS"

        // $ANTLR start "SHUASS"
        public void mSHUASS() // throws RecognitionException [2]
        {
            try
            {
                int _type = SHUASS;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:111:8: ( '>>>=' )
                // ES3.g3:111:10: '>>>='
                {
                    Match(">>>=");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "SHUASS"

        // $ANTLR start "ANDASS"
        public void mANDASS() // throws RecognitionException [2]
        {
            try
            {
                int _type = ANDASS;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:112:8: ( '&=' )
                // ES3.g3:112:10: '&='
                {
                    Match("&=");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "ANDASS"

        // $ANTLR start "ORASS"
        public void mORASS() // throws RecognitionException [2]
        {
            try
            {
                int _type = ORASS;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:113:7: ( '|=' )
                // ES3.g3:113:9: '|='
                {
                    Match("|=");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "ORASS"

        // $ANTLR start "XORASS"
        public void mXORASS() // throws RecognitionException [2]
        {
            try
            {
                int _type = XORASS;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:114:8: ( '^=' )
                // ES3.g3:114:10: '^='
                {
                    Match("^=");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "XORASS"

        // $ANTLR start "DIV"
        public void mDIV() // throws RecognitionException [2]
        {
            try
            {
                int _type = DIV;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:115:5: ( '/' )
                // ES3.g3:115:7: '/'
                {
                    Match('/');

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "DIV"

        // $ANTLR start "DIVASS"
        public void mDIVASS() // throws RecognitionException [2]
        {
            try
            {
                int _type = DIVASS;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:116:8: ( '/=' )
                // ES3.g3:116:10: '/='
                {
                    Match("/=");


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "DIVASS"

        // $ANTLR start "BSLASH"
        public void mBSLASH() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:203:2: ( '\\\\' )
                // ES3.g3:203:4: '\\\\'
                {
                    Match('\\');

                }

            }
            finally
            {
            }
        }
        // $ANTLR end "BSLASH"

        // $ANTLR start "DQUOTE"
        public void mDQUOTE() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:207:2: ( '\"' )
                // ES3.g3:207:4: '\"'
                {
                    Match('\"');

                }

            }
            finally
            {
            }
        }
        // $ANTLR end "DQUOTE"

        // $ANTLR start "SQUOTE"
        public void mSQUOTE() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:211:2: ( '\\'' )
                // ES3.g3:211:4: '\\''
                {
                    Match('\'');

                }

            }
            finally
            {
            }
        }
        // $ANTLR end "SQUOTE"

        // $ANTLR start "TAB"
        public void mTAB() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:217:2: ( '\\u0009' )
                // ES3.g3:217:4: '\\u0009'
                {
                    Match('\t');

                }

            }
            finally
            {
            }
        }
        // $ANTLR end "TAB"

        // $ANTLR start "VT"
        public void mVT() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:221:2: ( '\\u000b' )
                // ES3.g3:221:4: '\\u000b'
                {
                    Match('\u000B');

                }

            }
            finally
            {
            }
        }
        // $ANTLR end "VT"

        // $ANTLR start "FF"
        public void mFF() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:225:2: ( '\\u000c' )
                // ES3.g3:225:4: '\\u000c'
                {
                    Match('\f');

                }

            }
            finally
            {
            }
        }
        // $ANTLR end "FF"

        // $ANTLR start "SP"
        public void mSP() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:229:2: ( '\\u0020' )
                // ES3.g3:229:4: '\\u0020'
                {
                    Match(' ');

                }

            }
            finally
            {
            }
        }
        // $ANTLR end "SP"

        // $ANTLR start "NBSP"
        public void mNBSP() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:233:2: ( '\\u00a0' )
                // ES3.g3:233:4: '\\u00a0'
                {
                    Match('\u00A0');

                }

            }
            finally
            {
            }
        }
        // $ANTLR end "NBSP"

        // $ANTLR start "USP"
        public void mUSP() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:237:2: ( '\\u1680' | '\\u180E' | '\\u2000' | '\\u2001' | '\\u2002' | '\\u2003' | '\\u2004' | '\\u2005' | '\\u2006' | '\\u2007' | '\\u2008' | '\\u2009' | '\\u200A' | '\\u202F' | '\\u205F' | '\\u3000' )
                // ES3.g3:
                {
                    if (input.LA(1) == '\u1680' || input.LA(1) == '\u180E' || (input.LA(1) >= '\u2000' && input.LA(1) <= '\u200A') || input.LA(1) == '\u202F' || input.LA(1) == '\u205F' || input.LA(1) == '\u3000')
                    {
                        input.Consume();

                    }
                    else
                    {
                        MismatchedSetException mse = new MismatchedSetException(null, input);
                        Recover(mse);
                        throw mse;
                    }


                }

            }
            finally
            {
            }
        }
        // $ANTLR end "USP"

        // $ANTLR start "WhiteSpace"
        public void mWhiteSpace() // throws RecognitionException [2]
        {
            try
            {
                int _type = WhiteSpace;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:256:2: ( ( TAB | VT | FF | SP | NBSP | USP )+ )
                // ES3.g3:256:4: ( TAB | VT | FF | SP | NBSP | USP )+
                {
                    // ES3.g3:256:4: ( TAB | VT | FF | SP | NBSP | USP )+
                    int cnt1 = 0;
                    do
                    {
                        int alt1 = 2;
                        int LA1_0 = input.LA(1);

                        if ((LA1_0 == '\t' || (LA1_0 >= '\u000B' && LA1_0 <= '\f') || LA1_0 == ' ' || LA1_0 == '\u00A0' || LA1_0 == '\u1680' || LA1_0 == '\u180E' || (LA1_0 >= '\u2000' && LA1_0 <= '\u200A') || LA1_0 == '\u202F' || LA1_0 == '\u205F' || LA1_0 == '\u3000'))
                        {
                            alt1 = 1;
                        }


                        switch (alt1)
                        {
                            case 1:
                                // ES3.g3:
                                {
                                    if (input.LA(1) == '\t' || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || input.LA(1) == ' ' || input.LA(1) == '\u00A0' || input.LA(1) == '\u1680' || input.LA(1) == '\u180E' || (input.LA(1) >= '\u2000' && input.LA(1) <= '\u200A') || input.LA(1) == '\u202F' || input.LA(1) == '\u205F' || input.LA(1) == '\u3000')
                                    {
                                        input.Consume();

                                    }
                                    else
                                    {
                                        MismatchedSetException mse = new MismatchedSetException(null, input);
                                        Recover(mse);
                                        throw mse;
                                    }


                                }
                                break;

                            default:
                                if (cnt1 >= 1) goto loop1;
                                EarlyExitException eee1 =
                                    new EarlyExitException(1, input);
                                throw eee1;
                        }
                        cnt1++;
                    } while (true);

                loop1:
                    ;	// Stops C# compiler whining that label 'loop1' has no statements

                    _channel = HIDDEN;

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "WhiteSpace"

        // $ANTLR start "LF"
        public void mLF() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:264:2: ( '\\n' )
                // ES3.g3:264:4: '\\n'
                {
                    Match('\n');

                }

            }
            finally
            {
            }
        }
        // $ANTLR end "LF"

        // $ANTLR start "CR"
        public void mCR() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:268:2: ( '\\r' )
                // ES3.g3:268:4: '\\r'
                {
                    Match('\r');

                }

            }
            finally
            {
            }
        }
        // $ANTLR end "CR"

        // $ANTLR start "LS"
        public void mLS() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:272:2: ( '\\u2028' )
                // ES3.g3:272:4: '\\u2028'
                {
                    Match('\u2028');

                }

            }
            finally
            {
            }
        }
        // $ANTLR end "LS"

        // $ANTLR start "PS"
        public void mPS() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:276:2: ( '\\u2029' )
                // ES3.g3:276:4: '\\u2029'
                {
                    Match('\u2029');

                }

            }
            finally
            {
            }
        }
        // $ANTLR end "PS"

        // $ANTLR start "LineTerminator"
        public void mLineTerminator() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:280:2: ( CR | LF | LS | PS )
                // ES3.g3:
                {
                    if (input.LA(1) == '\n' || input.LA(1) == '\r' || (input.LA(1) >= '\u2028' && input.LA(1) <= '\u2029'))
                    {
                        input.Consume();

                    }
                    else
                    {
                        MismatchedSetException mse = new MismatchedSetException(null, input);
                        Recover(mse);
                        throw mse;
                    }


                }

            }
            finally
            {
            }
        }
        // $ANTLR end "LineTerminator"

        // $ANTLR start "EOL"
        public void mEOL() // throws RecognitionException [2]
        {
            try
            {
                int _type = EOL;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:284:2: ( ( ( CR ( LF )? ) | LF | LS | PS ) )
                // ES3.g3:284:4: ( ( CR ( LF )? ) | LF | LS | PS )
                {
                    // ES3.g3:284:4: ( ( CR ( LF )? ) | LF | LS | PS )
                    int alt3 = 4;
                    switch (input.LA(1))
                    {
                        case '\r':
                            {
                                alt3 = 1;
                            }
                            break;
                        case '\n':
                            {
                                alt3 = 2;
                            }
                            break;
                        case '\u2028':
                            {
                                alt3 = 3;
                            }
                            break;
                        case '\u2029':
                            {
                                alt3 = 4;
                            }
                            break;
                        default:
                            NoViableAltException nvae_d3s0 =
                                new NoViableAltException("", 3, 0, input);

                            throw nvae_d3s0;
                    }

                    switch (alt3)
                    {
                        case 1:
                            // ES3.g3:284:6: ( CR ( LF )? )
                            {
                                // ES3.g3:284:6: ( CR ( LF )? )
                                // ES3.g3:284:8: CR ( LF )?
                                {
                                    mCR();
                                    // ES3.g3:284:11: ( LF )?
                                    int alt2 = 2;
                                    int LA2_0 = input.LA(1);

                                    if ((LA2_0 == '\n'))
                                    {
                                        alt2 = 1;
                                    }
                                    switch (alt2)
                                    {
                                        case 1:
                                            // ES3.g3:284:11: LF
                                            {
                                                mLF();

                                            }
                                            break;

                                    }


                                }


                            }
                            break;
                        case 2:
                            // ES3.g3:284:19: LF
                            {
                                mLF();

                            }
                            break;
                        case 3:
                            // ES3.g3:284:24: LS
                            {
                                mLS();

                            }
                            break;
                        case 4:
                            // ES3.g3:284:29: PS
                            {
                                mPS();

                            }
                            break;

                    }

                    _channel = HIDDEN;

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "EOL"

        // $ANTLR start "MultiLineComment"
        public void mMultiLineComment() // throws RecognitionException [2]
        {
            try
            {
                int _type = MultiLineComment;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:291:2: ( '/*' ( options {greedy=false; } : . )* '*/' )
                // ES3.g3:291:4: '/*' ( options {greedy=false; } : . )* '*/'
                {
                    Match("/*");

                    // ES3.g3:291:9: ( options {greedy=false; } : . )*
                    do
                    {
                        int alt4 = 2;
                        int LA4_0 = input.LA(1);

                        if ((LA4_0 == '*'))
                        {
                            int LA4_1 = input.LA(2);

                            if ((LA4_1 == '/'))
                            {
                                alt4 = 2;
                            }
                            else if (((LA4_1 >= '\u0000' && LA4_1 <= '.') || (LA4_1 >= '0' && LA4_1 <= '\uFFFF')))
                            {
                                alt4 = 1;
                            }


                        }
                        else if (((LA4_0 >= '\u0000' && LA4_0 <= ')') || (LA4_0 >= '+' && LA4_0 <= '\uFFFF')))
                        {
                            alt4 = 1;
                        }


                        switch (alt4)
                        {
                            case 1:
                                // ES3.g3:291:41: .
                                {
                                    MatchAny();

                                }
                                break;

                            default:
                                goto loop4;
                        }
                    } while (true);

                loop4:
                    ;	// Stops C# compiler whining that label 'loop4' has no statements

                    Match("*/");

                    _channel = HIDDEN;

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "MultiLineComment"

        // $ANTLR start "SingleLineComment"
        public void mSingleLineComment() // throws RecognitionException [2]
        {
            try
            {
                int _type = SingleLineComment;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:295:2: ( '//' (~ ( LineTerminator ) )* )
                // ES3.g3:295:4: '//' (~ ( LineTerminator ) )*
                {
                    Match("//");

                    // ES3.g3:295:9: (~ ( LineTerminator ) )*
                    do
                    {
                        int alt5 = 2;
                        int LA5_0 = input.LA(1);

                        if (((LA5_0 >= '\u0000' && LA5_0 <= '\t') || (LA5_0 >= '\u000B' && LA5_0 <= '\f') || (LA5_0 >= '\u000E' && LA5_0 <= '\u2027') || (LA5_0 >= '\u202A' && LA5_0 <= '\uFFFF')))
                        {
                            alt5 = 1;
                        }


                        switch (alt5)
                        {
                            case 1:
                                // ES3.g3:295:11: ~ ( LineTerminator )
                                {
                                    if ((input.LA(1) >= '\u0000' && input.LA(1) <= '\t') || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || (input.LA(1) >= '\u000E' && input.LA(1) <= '\u2027') || (input.LA(1) >= '\u202A' && input.LA(1) <= '\uFFFF'))
                                    {
                                        input.Consume();

                                    }
                                    else
                                    {
                                        MismatchedSetException mse = new MismatchedSetException(null, input);
                                        Recover(mse);
                                        throw mse;
                                    }


                                }
                                break;

                            default:
                                goto loop5;
                        }
                    } while (true);

                loop5:
                    ;	// Stops C# compiler whining that label 'loop5' has no statements

                    _channel = HIDDEN;

                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "SingleLineComment"

        // $ANTLR start "IdentifierStartASCII"
        public void mIdentifierStartASCII() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:396:2: ( 'a' .. 'z' | 'A' .. 'Z' | '$' | '_' | BSLASH 'u' HexDigit HexDigit HexDigit HexDigit )
                int alt6 = 5;
                switch (input.LA(1))
                {
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                    case 'h':
                    case 'i':
                    case 'j':
                    case 'k':
                    case 'l':
                    case 'm':
                    case 'n':
                    case 'o':
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                    case 't':
                    case 'u':
                    case 'v':
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'z':
                        {
                            alt6 = 1;
                        }
                        break;
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'H':
                    case 'I':
                    case 'J':
                    case 'K':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'O':
                    case 'P':
                    case 'Q':
                    case 'R':
                    case 'S':
                    case 'T':
                    case 'U':
                    case 'V':
                    case 'W':
                    case 'X':
                    case 'Y':
                    case 'Z':
                        {
                            alt6 = 2;
                        }
                        break;
                    case '$':
                        {
                            alt6 = 3;
                        }
                        break;
                    case '_':
                        {
                            alt6 = 4;
                        }
                        break;
                    case '\\':
                        {
                            alt6 = 5;
                        }
                        break;
                    default:
                        NoViableAltException nvae_d6s0 =
                            new NoViableAltException("", 6, 0, input);

                        throw nvae_d6s0;
                }

                switch (alt6)
                {
                    case 1:
                        // ES3.g3:396:4: 'a' .. 'z'
                        {
                            MatchRange('a', 'z');

                        }
                        break;
                    case 2:
                        // ES3.g3:396:15: 'A' .. 'Z'
                        {
                            MatchRange('A', 'Z');

                        }
                        break;
                    case 3:
                        // ES3.g3:397:4: '$'
                        {
                            Match('$');

                        }
                        break;
                    case 4:
                        // ES3.g3:398:4: '_'
                        {
                            Match('_');

                        }
                        break;
                    case 5:
                        // ES3.g3:399:4: BSLASH 'u' HexDigit HexDigit HexDigit HexDigit
                        {
                            mBSLASH();
                            Match('u');
                            mHexDigit();
                            mHexDigit();
                            mHexDigit();
                            mHexDigit();

                        }
                        break;

                }
            }
            finally
            {
            }
        }
        // $ANTLR end "IdentifierStartASCII"

        // $ANTLR start "IdentifierPart"
        public void mIdentifierPart() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:407:2: ( DecimalDigit | IdentifierStartASCII | {...}?)
                int alt7 = 3;
                switch (input.LA(1))
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        {
                            alt7 = 1;
                        }
                        break;
                    case '$':
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'H':
                    case 'I':
                    case 'J':
                    case 'K':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'O':
                    case 'P':
                    case 'Q':
                    case 'R':
                    case 'S':
                    case 'T':
                    case 'U':
                    case 'V':
                    case 'W':
                    case 'X':
                    case 'Y':
                    case 'Z':
                    case '\\':
                    case '_':
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                    case 'h':
                    case 'i':
                    case 'j':
                    case 'k':
                    case 'l':
                    case 'm':
                    case 'n':
                    case 'o':
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                    case 't':
                    case 'u':
                    case 'v':
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'z':
                        {
                            alt7 = 2;
                        }
                        break;
                    default:
                        alt7 = 3;
                        break;
                }

                switch (alt7)
                {
                    case 1:
                        // ES3.g3:407:4: DecimalDigit
                        {
                            mDecimalDigit();

                        }
                        break;
                    case 2:
                        // ES3.g3:408:4: IdentifierStartASCII
                        {
                            mIdentifierStartASCII();

                        }
                        break;
                    case 3:
                        // ES3.g3:409:4: {...}?
                        {
                            if (!((IsIdentifierPartUnicode(input.LA(1)))))
                            {
                                throw new FailedPredicateException(input, "IdentifierPart", " IsIdentifierPartUnicode(input.LA(1)) ");
                            }
                            MatchAny();

                        }
                        break;

                }
            }
            finally
            {
            }
        }
        // $ANTLR end "IdentifierPart"

        // $ANTLR start "IdentifierNameASCIIStart"
        public void mIdentifierNameASCIIStart() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:413:2: ( IdentifierStartASCII ( IdentifierPart )* )
                // ES3.g3:413:4: IdentifierStartASCII ( IdentifierPart )*
                {
                    mIdentifierStartASCII();
                    // ES3.g3:413:25: ( IdentifierPart )*
                    do
                    {
                        int alt8 = 2;
                        int LA8_0 = input.LA(1);

                        if ((LA8_0 == '$' || (LA8_0 >= '0' && LA8_0 <= '9') || (LA8_0 >= 'A' && LA8_0 <= 'Z') || LA8_0 == '\\' || LA8_0 == '_' || (LA8_0 >= 'a' && LA8_0 <= 'z')))
                        {
                            alt8 = 1;
                        }
                        else if (((IsIdentifierPartUnicode(input.LA(1)))))
                        {
                            alt8 = 1;
                        }


                        switch (alt8)
                        {
                            case 1:
                                // ES3.g3:413:25: IdentifierPart
                                {
                                    mIdentifierPart();

                                }
                                break;

                            default:
                                goto loop8;
                        }
                    } while (true);

                loop8:
                    ;	// Stops C# compiler whining that label 'loop8' has no statements


                }

            }
            finally
            {
            }
        }
        // $ANTLR end "IdentifierNameASCIIStart"

        // $ANTLR start "Identifier"
        public void mIdentifier() // throws RecognitionException [2]
        {
            try
            {
                int _type = Identifier;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:425:2: ( IdentifierNameASCIIStart | )
                int alt9 = 2;
                int LA9_0 = input.LA(1);

                if ((LA9_0 == '$' || (LA9_0 >= 'A' && LA9_0 <= 'Z') || LA9_0 == '\\' || LA9_0 == '_' || (LA9_0 >= 'a' && LA9_0 <= 'z')))
                {
                    alt9 = 1;
                }
                else
                {
                    alt9 = 2;
                }
                switch (alt9)
                {
                    case 1:
                        // ES3.g3:425:4: IdentifierNameASCIIStart
                        {
                            mIdentifierNameASCIIStart();

                        }
                        break;
                    case 2:
                        // ES3.g3:426:4: 
                        {
                            ConsumeIdentifierUnicodeStart();

                        }
                        break;

                }
                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "Identifier"

        // $ANTLR start "DecimalDigit"
        public void mDecimalDigit() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:509:2: ( '0' .. '9' )
                // ES3.g3:509:4: '0' .. '9'
                {
                    MatchRange('0', '9');

                }

            }
            finally
            {
            }
        }
        // $ANTLR end "DecimalDigit"

        // $ANTLR start "HexDigit"
        public void mHexDigit() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:513:2: ( DecimalDigit | 'a' .. 'f' | 'A' .. 'F' )
                // ES3.g3:
                {
                    if ((input.LA(1) >= '0' && input.LA(1) <= '9') || (input.LA(1) >= 'A' && input.LA(1) <= 'F') || (input.LA(1) >= 'a' && input.LA(1) <= 'f'))
                    {
                        input.Consume();

                    }
                    else
                    {
                        MismatchedSetException mse = new MismatchedSetException(null, input);
                        Recover(mse);
                        throw mse;
                    }


                }

            }
            finally
            {
            }
        }
        // $ANTLR end "HexDigit"

        // $ANTLR start "OctalDigit"
        public void mOctalDigit() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:517:2: ( '0' .. '7' )
                // ES3.g3:517:4: '0' .. '7'
                {
                    MatchRange('0', '7');

                }

            }
            finally
            {
            }
        }
        // $ANTLR end "OctalDigit"

        // $ANTLR start "ExponentPart"
        public void mExponentPart() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:521:2: ( ( 'e' | 'E' ) ( '+' | '-' )? ( DecimalDigit )+ )
                // ES3.g3:521:4: ( 'e' | 'E' ) ( '+' | '-' )? ( DecimalDigit )+
                {
                    if (input.LA(1) == 'E' || input.LA(1) == 'e')
                    {
                        input.Consume();

                    }
                    else
                    {
                        MismatchedSetException mse = new MismatchedSetException(null, input);
                        Recover(mse);
                        throw mse;
                    }

                    // ES3.g3:521:18: ( '+' | '-' )?
                    int alt10 = 2;
                    int LA10_0 = input.LA(1);

                    if ((LA10_0 == '+' || LA10_0 == '-'))
                    {
                        alt10 = 1;
                    }
                    switch (alt10)
                    {
                        case 1:
                            // ES3.g3:
                            {
                                if (input.LA(1) == '+' || input.LA(1) == '-')
                                {
                                    input.Consume();

                                }
                                else
                                {
                                    MismatchedSetException mse = new MismatchedSetException(null, input);
                                    Recover(mse);
                                    throw mse;
                                }


                            }
                            break;

                    }

                    // ES3.g3:521:33: ( DecimalDigit )+
                    int cnt11 = 0;
                    do
                    {
                        int alt11 = 2;
                        int LA11_0 = input.LA(1);

                        if (((LA11_0 >= '0' && LA11_0 <= '9')))
                        {
                            alt11 = 1;
                        }


                        switch (alt11)
                        {
                            case 1:
                                // ES3.g3:521:33: DecimalDigit
                                {
                                    mDecimalDigit();

                                }
                                break;

                            default:
                                if (cnt11 >= 1) goto loop11;
                                EarlyExitException eee11 =
                                    new EarlyExitException(11, input);
                                throw eee11;
                        }
                        cnt11++;
                    } while (true);

                loop11:
                    ;	// Stops C# compiler whining that label 'loop11' has no statements


                }

            }
            finally
            {
            }
        }
        // $ANTLR end "ExponentPart"

        // $ANTLR start "DecimalIntegerLiteral"
        public void mDecimalIntegerLiteral() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:525:2: ( '0' | '1' .. '9' ( DecimalDigit )* )
                int alt13 = 2;
                int LA13_0 = input.LA(1);

                if ((LA13_0 == '0'))
                {
                    alt13 = 1;
                }
                else if (((LA13_0 >= '1' && LA13_0 <= '9')))
                {
                    alt13 = 2;
                }
                else
                {
                    NoViableAltException nvae_d13s0 =
                        new NoViableAltException("", 13, 0, input);

                    throw nvae_d13s0;
                }
                switch (alt13)
                {
                    case 1:
                        // ES3.g3:525:4: '0'
                        {
                            Match('0');

                        }
                        break;
                    case 2:
                        // ES3.g3:526:4: '1' .. '9' ( DecimalDigit )*
                        {
                            MatchRange('1', '9');
                            // ES3.g3:526:13: ( DecimalDigit )*
                            do
                            {
                                int alt12 = 2;
                                int LA12_0 = input.LA(1);

                                if (((LA12_0 >= '0' && LA12_0 <= '9')))
                                {
                                    alt12 = 1;
                                }


                                switch (alt12)
                                {
                                    case 1:
                                        // ES3.g3:526:13: DecimalDigit
                                        {
                                            mDecimalDigit();

                                        }
                                        break;

                                    default:
                                        goto loop12;
                                }
                            } while (true);

                        loop12:
                            ;	// Stops C# compiler whining that label 'loop12' has no statements


                        }
                        break;

                }
            }
            finally
            {
            }
        }
        // $ANTLR end "DecimalIntegerLiteral"

        // $ANTLR start "DecimalLiteral"
        public void mDecimalLiteral() // throws RecognitionException [2]
        {
            try
            {
                int _type = DecimalLiteral;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:530:2: ( DecimalIntegerLiteral '.' ( DecimalDigit )* ( ExponentPart )? | '.' ( DecimalDigit )+ ( ExponentPart )? | DecimalIntegerLiteral ( ExponentPart )? )
                int alt19 = 3;
                alt19 = dfa19.Predict(input);
                switch (alt19)
                {
                    case 1:
                        // ES3.g3:530:4: DecimalIntegerLiteral '.' ( DecimalDigit )* ( ExponentPart )?
                        {
                            mDecimalIntegerLiteral();
                            Match('.');
                            // ES3.g3:530:30: ( DecimalDigit )*
                            do
                            {
                                int alt14 = 2;
                                int LA14_0 = input.LA(1);

                                if (((LA14_0 >= '0' && LA14_0 <= '9')))
                                {
                                    alt14 = 1;
                                }


                                switch (alt14)
                                {
                                    case 1:
                                        // ES3.g3:530:30: DecimalDigit
                                        {
                                            mDecimalDigit();

                                        }
                                        break;

                                    default:
                                        goto loop14;
                                }
                            } while (true);

                        loop14:
                            ;	// Stops C# compiler whining that label 'loop14' has no statements

                            // ES3.g3:530:44: ( ExponentPart )?
                            int alt15 = 2;
                            int LA15_0 = input.LA(1);

                            if ((LA15_0 == 'E' || LA15_0 == 'e'))
                            {
                                alt15 = 1;
                            }
                            switch (alt15)
                            {
                                case 1:
                                    // ES3.g3:530:44: ExponentPart
                                    {
                                        mExponentPart();

                                    }
                                    break;

                            }


                        }
                        break;
                    case 2:
                        // ES3.g3:531:4: '.' ( DecimalDigit )+ ( ExponentPart )?
                        {
                            Match('.');
                            // ES3.g3:531:8: ( DecimalDigit )+
                            int cnt16 = 0;
                            do
                            {
                                int alt16 = 2;
                                int LA16_0 = input.LA(1);

                                if (((LA16_0 >= '0' && LA16_0 <= '9')))
                                {
                                    alt16 = 1;
                                }


                                switch (alt16)
                                {
                                    case 1:
                                        // ES3.g3:531:8: DecimalDigit
                                        {
                                            mDecimalDigit();

                                        }
                                        break;

                                    default:
                                        if (cnt16 >= 1) goto loop16;
                                        EarlyExitException eee16 =
                                            new EarlyExitException(16, input);
                                        throw eee16;
                                }
                                cnt16++;
                            } while (true);

                        loop16:
                            ;	// Stops C# compiler whining that label 'loop16' has no statements

                            // ES3.g3:531:22: ( ExponentPart )?
                            int alt17 = 2;
                            int LA17_0 = input.LA(1);

                            if ((LA17_0 == 'E' || LA17_0 == 'e'))
                            {
                                alt17 = 1;
                            }
                            switch (alt17)
                            {
                                case 1:
                                    // ES3.g3:531:22: ExponentPart
                                    {
                                        mExponentPart();

                                    }
                                    break;

                            }


                        }
                        break;
                    case 3:
                        // ES3.g3:532:4: DecimalIntegerLiteral ( ExponentPart )?
                        {
                            mDecimalIntegerLiteral();
                            // ES3.g3:532:26: ( ExponentPart )?
                            int alt18 = 2;
                            int LA18_0 = input.LA(1);

                            if ((LA18_0 == 'E' || LA18_0 == 'e'))
                            {
                                alt18 = 1;
                            }
                            switch (alt18)
                            {
                                case 1:
                                    // ES3.g3:532:26: ExponentPart
                                    {
                                        mExponentPart();

                                    }
                                    break;

                            }


                        }
                        break;

                }
                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "DecimalLiteral"

        // $ANTLR start "OctalIntegerLiteral"
        public void mOctalIntegerLiteral() // throws RecognitionException [2]
        {
            try
            {
                int _type = OctalIntegerLiteral;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:536:2: ( '0' ( OctalDigit )+ )
                // ES3.g3:536:4: '0' ( OctalDigit )+
                {
                    Match('0');
                    // ES3.g3:536:8: ( OctalDigit )+
                    int cnt20 = 0;
                    do
                    {
                        int alt20 = 2;
                        int LA20_0 = input.LA(1);

                        if (((LA20_0 >= '0' && LA20_0 <= '7')))
                        {
                            alt20 = 1;
                        }


                        switch (alt20)
                        {
                            case 1:
                                // ES3.g3:536:8: OctalDigit
                                {
                                    mOctalDigit();

                                }
                                break;

                            default:
                                if (cnt20 >= 1) goto loop20;
                                EarlyExitException eee20 =
                                    new EarlyExitException(20, input);
                                throw eee20;
                        }
                        cnt20++;
                    } while (true);

                loop20:
                    ;	// Stops C# compiler whining that label 'loop20' has no statements


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "OctalIntegerLiteral"

        // $ANTLR start "HexIntegerLiteral"
        public void mHexIntegerLiteral() // throws RecognitionException [2]
        {
            try
            {
                int _type = HexIntegerLiteral;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:540:2: ( ( '0x' | '0X' ) ( HexDigit )+ )
                // ES3.g3:540:4: ( '0x' | '0X' ) ( HexDigit )+
                {
                    // ES3.g3:540:4: ( '0x' | '0X' )
                    int alt21 = 2;
                    int LA21_0 = input.LA(1);

                    if ((LA21_0 == '0'))
                    {
                        int LA21_1 = input.LA(2);

                        if ((LA21_1 == 'x'))
                        {
                            alt21 = 1;
                        }
                        else if ((LA21_1 == 'X'))
                        {
                            alt21 = 2;
                        }
                        else
                        {
                            NoViableAltException nvae_d21s1 =
                                new NoViableAltException("", 21, 1, input);

                            throw nvae_d21s1;
                        }
                    }
                    else
                    {
                        NoViableAltException nvae_d21s0 =
                            new NoViableAltException("", 21, 0, input);

                        throw nvae_d21s0;
                    }
                    switch (alt21)
                    {
                        case 1:
                            // ES3.g3:540:6: '0x'
                            {
                                Match("0x");


                            }
                            break;
                        case 2:
                            // ES3.g3:540:13: '0X'
                            {
                                Match("0X");


                            }
                            break;

                    }

                    // ES3.g3:540:20: ( HexDigit )+
                    int cnt22 = 0;
                    do
                    {
                        int alt22 = 2;
                        int LA22_0 = input.LA(1);

                        if (((LA22_0 >= '0' && LA22_0 <= '9') || (LA22_0 >= 'A' && LA22_0 <= 'F') || (LA22_0 >= 'a' && LA22_0 <= 'f')))
                        {
                            alt22 = 1;
                        }


                        switch (alt22)
                        {
                            case 1:
                                // ES3.g3:540:20: HexDigit
                                {
                                    mHexDigit();

                                }
                                break;

                            default:
                                if (cnt22 >= 1) goto loop22;
                                EarlyExitException eee22 =
                                    new EarlyExitException(22, input);
                                throw eee22;
                        }
                        cnt22++;
                    } while (true);

                loop22:
                    ;	// Stops C# compiler whining that label 'loop22' has no statements


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "HexIntegerLiteral"

        // $ANTLR start "CharacterEscapeSequence"
        public void mCharacterEscapeSequence() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:559:2: (~ ( DecimalDigit | 'x' | 'u' | LineTerminator ) )
                // ES3.g3:559:4: ~ ( DecimalDigit | 'x' | 'u' | LineTerminator )
                {
                    if ((input.LA(1) >= '\u0000' && input.LA(1) <= '\t') || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || (input.LA(1) >= '\u000E' && input.LA(1) <= '/') || (input.LA(1) >= ':' && input.LA(1) <= 't') || (input.LA(1) >= 'v' && input.LA(1) <= 'w') || (input.LA(1) >= 'y' && input.LA(1) <= '\u2027') || (input.LA(1) >= '\u202A' && input.LA(1) <= '\uFFFF'))
                    {
                        input.Consume();

                    }
                    else
                    {
                        MismatchedSetException mse = new MismatchedSetException(null, input);
                        Recover(mse);
                        throw mse;
                    }


                }

            }
            finally
            {
            }
        }
        // $ANTLR end "CharacterEscapeSequence"

        // $ANTLR start "ZeroToThree"
        public void mZeroToThree() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:563:2: ( '0' .. '3' )
                // ES3.g3:563:4: '0' .. '3'
                {
                    MatchRange('0', '3');

                }

            }
            finally
            {
            }
        }
        // $ANTLR end "ZeroToThree"

        // $ANTLR start "OctalEscapeSequence"
        public void mOctalEscapeSequence() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:567:2: ( OctalDigit | ZeroToThree OctalDigit | '4' .. '7' OctalDigit | ZeroToThree OctalDigit OctalDigit )
                int alt23 = 4;
                int LA23_0 = input.LA(1);

                if (((LA23_0 >= '0' && LA23_0 <= '3')))
                {
                    int LA23_1 = input.LA(2);

                    if (((LA23_1 >= '0' && LA23_1 <= '7')))
                    {
                        int LA23_4 = input.LA(3);

                        if (((LA23_4 >= '0' && LA23_4 <= '7')))
                        {
                            alt23 = 4;
                        }
                        else
                        {
                            alt23 = 2;
                        }
                    }
                    else
                    {
                        alt23 = 1;
                    }
                }
                else if (((LA23_0 >= '4' && LA23_0 <= '7')))
                {
                    int LA23_2 = input.LA(2);

                    if (((LA23_2 >= '0' && LA23_2 <= '7')))
                    {
                        alt23 = 3;
                    }
                    else
                    {
                        alt23 = 1;
                    }
                }
                else
                {
                    NoViableAltException nvae_d23s0 =
                        new NoViableAltException("", 23, 0, input);

                    throw nvae_d23s0;
                }
                switch (alt23)
                {
                    case 1:
                        // ES3.g3:567:4: OctalDigit
                        {
                            mOctalDigit();

                        }
                        break;
                    case 2:
                        // ES3.g3:568:4: ZeroToThree OctalDigit
                        {
                            mZeroToThree();
                            mOctalDigit();

                        }
                        break;
                    case 3:
                        // ES3.g3:569:4: '4' .. '7' OctalDigit
                        {
                            MatchRange('4', '7');
                            mOctalDigit();

                        }
                        break;
                    case 4:
                        // ES3.g3:570:4: ZeroToThree OctalDigit OctalDigit
                        {
                            mZeroToThree();
                            mOctalDigit();
                            mOctalDigit();

                        }
                        break;

                }
            }
            finally
            {
            }
        }
        // $ANTLR end "OctalEscapeSequence"

        // $ANTLR start "HexEscapeSequence"
        public void mHexEscapeSequence() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:574:2: ( 'x' HexDigit HexDigit )
                // ES3.g3:574:4: 'x' HexDigit HexDigit
                {
                    Match('x');
                    mHexDigit();
                    mHexDigit();

                }

            }
            finally
            {
            }
        }
        // $ANTLR end "HexEscapeSequence"

        // $ANTLR start "UnicodeEscapeSequence"
        public void mUnicodeEscapeSequence() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:578:2: ( 'u' HexDigit HexDigit HexDigit HexDigit )
                // ES3.g3:578:4: 'u' HexDigit HexDigit HexDigit HexDigit
                {
                    Match('u');
                    mHexDigit();
                    mHexDigit();
                    mHexDigit();
                    mHexDigit();

                }

            }
            finally
            {
            }
        }
        // $ANTLR end "UnicodeEscapeSequence"

        // $ANTLR start "EscapeSequence"
        public void mEscapeSequence() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:582:2: ( BSLASH ( CharacterEscapeSequence | OctalEscapeSequence | HexEscapeSequence | UnicodeEscapeSequence ) )
                // ES3.g3:583:2: BSLASH ( CharacterEscapeSequence | OctalEscapeSequence | HexEscapeSequence | UnicodeEscapeSequence )
                {
                    mBSLASH();
                    // ES3.g3:584:2: ( CharacterEscapeSequence | OctalEscapeSequence | HexEscapeSequence | UnicodeEscapeSequence )
                    int alt24 = 4;
                    int LA24_0 = input.LA(1);

                    if (((LA24_0 >= '\u0000' && LA24_0 <= '\t') || (LA24_0 >= '\u000B' && LA24_0 <= '\f') || (LA24_0 >= '\u000E' && LA24_0 <= '/') || (LA24_0 >= ':' && LA24_0 <= 't') || (LA24_0 >= 'v' && LA24_0 <= 'w') || (LA24_0 >= 'y' && LA24_0 <= '\u2027') || (LA24_0 >= '\u202A' && LA24_0 <= '\uFFFF')))
                    {
                        alt24 = 1;
                    }
                    else if (((LA24_0 >= '0' && LA24_0 <= '7')))
                    {
                        alt24 = 2;
                    }
                    else if ((LA24_0 == 'x'))
                    {
                        alt24 = 3;
                    }
                    else if ((LA24_0 == 'u'))
                    {
                        alt24 = 4;
                    }
                    else
                    {
                        NoViableAltException nvae_d24s0 =
                            new NoViableAltException("", 24, 0, input);

                        throw nvae_d24s0;
                    }
                    switch (alt24)
                    {
                        case 1:
                            // ES3.g3:585:3: CharacterEscapeSequence
                            {
                                mCharacterEscapeSequence();

                            }
                            break;
                        case 2:
                            // ES3.g3:586:5: OctalEscapeSequence
                            {
                                mOctalEscapeSequence();

                            }
                            break;
                        case 3:
                            // ES3.g3:587:5: HexEscapeSequence
                            {
                                mHexEscapeSequence();

                            }
                            break;
                        case 4:
                            // ES3.g3:588:5: UnicodeEscapeSequence
                            {
                                mUnicodeEscapeSequence();

                            }
                            break;

                    }


                }

            }
            finally
            {
            }
        }
        // $ANTLR end "EscapeSequence"

        // $ANTLR start "StringLiteral"
        public void mStringLiteral() // throws RecognitionException [2]
        {
            try
            {
                int _type = StringLiteral;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:593:2: ( SQUOTE (~ ( SQUOTE | BSLASH | LineTerminator ) | EscapeSequence )* SQUOTE | DQUOTE (~ ( DQUOTE | BSLASH | LineTerminator ) | EscapeSequence )* DQUOTE )
                int alt27 = 2;
                int LA27_0 = input.LA(1);

                if ((LA27_0 == '\''))
                {
                    alt27 = 1;
                }
                else if ((LA27_0 == '\"'))
                {
                    alt27 = 2;
                }
                else
                {
                    NoViableAltException nvae_d27s0 =
                        new NoViableAltException("", 27, 0, input);

                    throw nvae_d27s0;
                }
                switch (alt27)
                {
                    case 1:
                        // ES3.g3:593:4: SQUOTE (~ ( SQUOTE | BSLASH | LineTerminator ) | EscapeSequence )* SQUOTE
                        {
                            mSQUOTE();
                            // ES3.g3:593:11: (~ ( SQUOTE | BSLASH | LineTerminator ) | EscapeSequence )*
                            do
                            {
                                int alt25 = 3;
                                int LA25_0 = input.LA(1);

                                if (((LA25_0 >= '\u0000' && LA25_0 <= '\t') || (LA25_0 >= '\u000B' && LA25_0 <= '\f') || (LA25_0 >= '\u000E' && LA25_0 <= '&') || (LA25_0 >= '(' && LA25_0 <= '[') || (LA25_0 >= ']' && LA25_0 <= '\u2027') || (LA25_0 >= '\u202A' && LA25_0 <= '\uFFFF')))
                                {
                                    alt25 = 1;
                                }
                                else if ((LA25_0 == '\\'))
                                {
                                    alt25 = 2;
                                }


                                switch (alt25)
                                {
                                    case 1:
                                        // ES3.g3:593:13: ~ ( SQUOTE | BSLASH | LineTerminator )
                                        {
                                            if ((input.LA(1) >= '\u0000' && input.LA(1) <= '\t') || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || (input.LA(1) >= '\u000E' && input.LA(1) <= '&') || (input.LA(1) >= '(' && input.LA(1) <= '[') || (input.LA(1) >= ']' && input.LA(1) <= '\u2027') || (input.LA(1) >= '\u202A' && input.LA(1) <= '\uFFFF'))
                                            {
                                                input.Consume();

                                            }
                                            else
                                            {
                                                MismatchedSetException mse = new MismatchedSetException(null, input);
                                                Recover(mse);
                                                throw mse;
                                            }


                                        }
                                        break;
                                    case 2:
                                        // ES3.g3:593:53: EscapeSequence
                                        {
                                            mEscapeSequence();

                                        }
                                        break;

                                    default:
                                        goto loop25;
                                }
                            } while (true);

                        loop25:
                            ;	// Stops C# compiler whining that label 'loop25' has no statements

                            mSQUOTE();

                        }
                        break;
                    case 2:
                        // ES3.g3:594:4: DQUOTE (~ ( DQUOTE | BSLASH | LineTerminator ) | EscapeSequence )* DQUOTE
                        {
                            mDQUOTE();
                            // ES3.g3:594:11: (~ ( DQUOTE | BSLASH | LineTerminator ) | EscapeSequence )*
                            do
                            {
                                int alt26 = 3;
                                int LA26_0 = input.LA(1);

                                if (((LA26_0 >= '\u0000' && LA26_0 <= '\t') || (LA26_0 >= '\u000B' && LA26_0 <= '\f') || (LA26_0 >= '\u000E' && LA26_0 <= '!') || (LA26_0 >= '#' && LA26_0 <= '[') || (LA26_0 >= ']' && LA26_0 <= '\u2027') || (LA26_0 >= '\u202A' && LA26_0 <= '\uFFFF')))
                                {
                                    alt26 = 1;
                                }
                                else if ((LA26_0 == '\\'))
                                {
                                    alt26 = 2;
                                }


                                switch (alt26)
                                {
                                    case 1:
                                        // ES3.g3:594:13: ~ ( DQUOTE | BSLASH | LineTerminator )
                                        {
                                            if ((input.LA(1) >= '\u0000' && input.LA(1) <= '\t') || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || (input.LA(1) >= '\u000E' && input.LA(1) <= '!') || (input.LA(1) >= '#' && input.LA(1) <= '[') || (input.LA(1) >= ']' && input.LA(1) <= '\u2027') || (input.LA(1) >= '\u202A' && input.LA(1) <= '\uFFFF'))
                                            {
                                                input.Consume();

                                            }
                                            else
                                            {
                                                MismatchedSetException mse = new MismatchedSetException(null, input);
                                                Recover(mse);
                                                throw mse;
                                            }


                                        }
                                        break;
                                    case 2:
                                        // ES3.g3:594:53: EscapeSequence
                                        {
                                            mEscapeSequence();

                                        }
                                        break;

                                    default:
                                        goto loop26;
                                }
                            } while (true);

                        loop26:
                            ;	// Stops C# compiler whining that label 'loop26' has no statements

                            mDQUOTE();

                        }
                        break;

                }
                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "StringLiteral"

        // $ANTLR start "BackslashSequence"
        public void mBackslashSequence() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:602:2: ( BSLASH ~ ( LineTerminator ) )
                // ES3.g3:602:4: BSLASH ~ ( LineTerminator )
                {
                    mBSLASH();
                    if ((input.LA(1) >= '\u0000' && input.LA(1) <= '\t') || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || (input.LA(1) >= '\u000E' && input.LA(1) <= '\u2027') || (input.LA(1) >= '\u202A' && input.LA(1) <= '\uFFFF'))
                    {
                        input.Consume();

                    }
                    else
                    {
                        MismatchedSetException mse = new MismatchedSetException(null, input);
                        Recover(mse);
                        throw mse;
                    }


                }

            }
            finally
            {
            }
        }
        // $ANTLR end "BackslashSequence"

        // $ANTLR start "RegularExpressionFirstChar"
        public void mRegularExpressionFirstChar() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:606:2: (~ ( LineTerminator | MUL | BSLASH | DIV ) | BackslashSequence )
                int alt28 = 2;
                int LA28_0 = input.LA(1);

                if (((LA28_0 >= '\u0000' && LA28_0 <= '\t') || (LA28_0 >= '\u000B' && LA28_0 <= '\f') || (LA28_0 >= '\u000E' && LA28_0 <= ')') || (LA28_0 >= '+' && LA28_0 <= '.') || (LA28_0 >= '0' && LA28_0 <= '[') || (LA28_0 >= ']' && LA28_0 <= '\u2027') || (LA28_0 >= '\u202A' && LA28_0 <= '\uFFFF')))
                {
                    alt28 = 1;
                }
                else if ((LA28_0 == '\\'))
                {
                    alt28 = 2;
                }
                else
                {
                    NoViableAltException nvae_d28s0 =
                        new NoViableAltException("", 28, 0, input);

                    throw nvae_d28s0;
                }
                switch (alt28)
                {
                    case 1:
                        // ES3.g3:606:4: ~ ( LineTerminator | MUL | BSLASH | DIV )
                        {
                            if ((input.LA(1) >= '\u0000' && input.LA(1) <= '\t') || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || (input.LA(1) >= '\u000E' && input.LA(1) <= ')') || (input.LA(1) >= '+' && input.LA(1) <= '.') || (input.LA(1) >= '0' && input.LA(1) <= '[') || (input.LA(1) >= ']' && input.LA(1) <= '\u2027') || (input.LA(1) >= '\u202A' && input.LA(1) <= '\uFFFF'))
                            {
                                input.Consume();

                            }
                            else
                            {
                                MismatchedSetException mse = new MismatchedSetException(null, input);
                                Recover(mse);
                                throw mse;
                            }


                        }
                        break;
                    case 2:
                        // ES3.g3:607:4: BackslashSequence
                        {
                            mBackslashSequence();

                        }
                        break;

                }
            }
            finally
            {
            }
        }
        // $ANTLR end "RegularExpressionFirstChar"

        // $ANTLR start "RegularExpressionChar"
        public void mRegularExpressionChar() // throws RecognitionException [2]
        {
            try
            {
                // ES3.g3:611:2: (~ ( LineTerminator | BSLASH | DIV ) | BackslashSequence )
                int alt29 = 2;
                int LA29_0 = input.LA(1);

                if (((LA29_0 >= '\u0000' && LA29_0 <= '\t') || (LA29_0 >= '\u000B' && LA29_0 <= '\f') || (LA29_0 >= '\u000E' && LA29_0 <= '.') || (LA29_0 >= '0' && LA29_0 <= '[') || (LA29_0 >= ']' && LA29_0 <= '\u2027') || (LA29_0 >= '\u202A' && LA29_0 <= '\uFFFF')))
                {
                    alt29 = 1;
                }
                else if ((LA29_0 == '\\'))
                {
                    alt29 = 2;
                }
                else
                {
                    NoViableAltException nvae_d29s0 =
                        new NoViableAltException("", 29, 0, input);

                    throw nvae_d29s0;
                }
                switch (alt29)
                {
                    case 1:
                        // ES3.g3:611:4: ~ ( LineTerminator | BSLASH | DIV )
                        {
                            if ((input.LA(1) >= '\u0000' && input.LA(1) <= '\t') || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || (input.LA(1) >= '\u000E' && input.LA(1) <= '.') || (input.LA(1) >= '0' && input.LA(1) <= '[') || (input.LA(1) >= ']' && input.LA(1) <= '\u2027') || (input.LA(1) >= '\u202A' && input.LA(1) <= '\uFFFF'))
                            {
                                input.Consume();

                            }
                            else
                            {
                                MismatchedSetException mse = new MismatchedSetException(null, input);
                                Recover(mse);
                                throw mse;
                            }


                        }
                        break;
                    case 2:
                        // ES3.g3:612:4: BackslashSequence
                        {
                            mBackslashSequence();

                        }
                        break;

                }
            }
            finally
            {
            }
        }
        // $ANTLR end "RegularExpressionChar"

        // $ANTLR start "RegularExpressionLiteral"
        public void mRegularExpressionLiteral() // throws RecognitionException [2]
        {
            try
            {
                int _type = RegularExpressionLiteral;
                int _channel = DEFAULT_TOKEN_CHANNEL;
                // ES3.g3:616:2: ({...}? => DIV RegularExpressionFirstChar ( RegularExpressionChar )* DIV ( IdentifierPart )* )
                // ES3.g3:616:4: {...}? => DIV RegularExpressionFirstChar ( RegularExpressionChar )* DIV ( IdentifierPart )*
                {
                    if (!((AreRegularExpressionsEnabled)))
                    {
                        throw new FailedPredicateException(input, "RegularExpressionLiteral", " AreRegularExpressionsEnabled ");
                    }
                    mDIV();
                    mRegularExpressionFirstChar();
                    // ES3.g3:616:71: ( RegularExpressionChar )*
                    do
                    {
                        int alt30 = 2;
                        int LA30_0 = input.LA(1);

                        if (((LA30_0 >= '\u0000' && LA30_0 <= '\t') || (LA30_0 >= '\u000B' && LA30_0 <= '\f') || (LA30_0 >= '\u000E' && LA30_0 <= '.') || (LA30_0 >= '0' && LA30_0 <= '\u2027') || (LA30_0 >= '\u202A' && LA30_0 <= '\uFFFF')))
                        {
                            alt30 = 1;
                        }


                        switch (alt30)
                        {
                            case 1:
                                // ES3.g3:616:71: RegularExpressionChar
                                {
                                    mRegularExpressionChar();

                                }
                                break;

                            default:
                                goto loop30;
                        }
                    } while (true);

                loop30:
                    ;	// Stops C# compiler whining that label 'loop30' has no statements

                    mDIV();
                    // ES3.g3:616:98: ( IdentifierPart )*
                    do
                    {
                        int alt31 = 2;
                        int LA31_0 = input.LA(1);

                        if ((LA31_0 == '$' || (LA31_0 >= '0' && LA31_0 <= '9') || (LA31_0 >= 'A' && LA31_0 <= 'Z') || LA31_0 == '\\' || LA31_0 == '_' || (LA31_0 >= 'a' && LA31_0 <= 'z')))
                        {
                            alt31 = 1;
                        }
                        else if (((IsIdentifierPartUnicode(input.LA(1)))))
                        {
                            alt31 = 1;
                        }


                        switch (alt31)
                        {
                            case 1:
                                // ES3.g3:616:98: IdentifierPart
                                {
                                    mIdentifierPart();

                                }
                                break;

                            default:
                                goto loop31;
                        }
                    } while (true);

                loop31:
                    ;	// Stops C# compiler whining that label 'loop31' has no statements


                }

                state.type = _type;
                state.channel = _channel;
            }
            finally
            {
            }
        }
        // $ANTLR end "RegularExpressionLiteral"

        override public void mTokens() // throws RecognitionException 
        {
            // ES3.g3:1:8: ( NULL | TRUE | FALSE | BREAK | CASE | CATCH | CONTINUE | DEFAULT | DELETE | DO | ELSE | FINALLY | FOR | FUNCTION | IF | IN | INSTANCEOF | NEW | RETURN | SWITCH | THIS | THROW | TRY | TYPEOF | VAR | VOID | WHILE | WITH | ABSTRACT | BOOLEAN | BYTE | CHAR | CLASS | CONST | DEBUGGER | DOUBLE | ENUM | EXPORT | EXTENDS | FINAL | FLOAT | GOTO | IMPLEMENTS | IMPORT | INT | INTERFACE | LONG | NATIVE | PACKAGE | PRIVATE | PROTECTED | PUBLIC | SHORT | STATIC | SUPER | SYNCHRONIZED | THROWS | TRANSIENT | VOLATILE | LBRACE | RBRACE | LPAREN | RPAREN | LBRACK | RBRACK | DOT | SEMIC | COMMA | LT | GT | LTE | GTE | EQ | NEQ | SAME | NSAME | ADD | SUB | MUL | MOD | INC | DEC | SHL | SHR | SHU | AND | OR | XOR | NOT | INV | LAND | LOR | QUE | COLON | ASSIGN | ADDASS | SUBASS | MULASS | MODASS | SHLASS | SHRASS | SHUASS | ANDASS | ORASS | XORASS | DIV | DIVASS | WhiteSpace | EOL | MultiLineComment | SingleLineComment | Identifier | DecimalLiteral | OctalIntegerLiteral | HexIntegerLiteral | StringLiteral | RegularExpressionLiteral )
            int alt32 = 117;
            alt32 = dfa32.Predict(input);
            switch (alt32)
            {
                case 1:
                    // ES3.g3:1:10: NULL
                    {
                        mNULL();

                    }
                    break;
                case 2:
                    // ES3.g3:1:15: TRUE
                    {
                        mTRUE();

                    }
                    break;
                case 3:
                    // ES3.g3:1:20: FALSE
                    {
                        mFALSE();

                    }
                    break;
                case 4:
                    // ES3.g3:1:26: BREAK
                    {
                        mBREAK();

                    }
                    break;
                case 5:
                    // ES3.g3:1:32: CASE
                    {
                        mCASE();

                    }
                    break;
                case 6:
                    // ES3.g3:1:37: CATCH
                    {
                        mCATCH();

                    }
                    break;
                case 7:
                    // ES3.g3:1:43: CONTINUE
                    {
                        mCONTINUE();

                    }
                    break;
                case 8:
                    // ES3.g3:1:52: DEFAULT
                    {
                        mDEFAULT();

                    }
                    break;
                case 9:
                    // ES3.g3:1:60: DELETE
                    {
                        mDELETE();

                    }
                    break;
                case 10:
                    // ES3.g3:1:67: DO
                    {
                        mDO();

                    }
                    break;
                case 11:
                    // ES3.g3:1:70: ELSE
                    {
                        mELSE();

                    }
                    break;
                case 12:
                    // ES3.g3:1:75: FINALLY
                    {
                        mFINALLY();

                    }
                    break;
                case 13:
                    // ES3.g3:1:83: FOR
                    {
                        mFOR();

                    }
                    break;
                case 14:
                    // ES3.g3:1:87: FUNCTION
                    {
                        mFUNCTION();

                    }
                    break;
                case 15:
                    // ES3.g3:1:96: IF
                    {
                        mIF();

                    }
                    break;
                case 16:
                    // ES3.g3:1:99: IN
                    {
                        mIN();

                    }
                    break;
                case 17:
                    // ES3.g3:1:102: INSTANCEOF
                    {
                        mINSTANCEOF();

                    }
                    break;
                case 18:
                    // ES3.g3:1:113: NEW
                    {
                        mNEW();

                    }
                    break;
                case 19:
                    // ES3.g3:1:117: RETURN
                    {
                        mRETURN();

                    }
                    break;
                case 20:
                    // ES3.g3:1:124: SWITCH
                    {
                        mSWITCH();

                    }
                    break;
                case 21:
                    // ES3.g3:1:131: THIS
                    {
                        mTHIS();

                    }
                    break;
                case 22:
                    // ES3.g3:1:136: THROW
                    {
                        mTHROW();

                    }
                    break;
                case 23:
                    // ES3.g3:1:142: TRY
                    {
                        mTRY();

                    }
                    break;
                case 24:
                    // ES3.g3:1:146: TYPEOF
                    {
                        mTYPEOF();

                    }
                    break;
                case 25:
                    // ES3.g3:1:153: VAR
                    {
                        mVAR();

                    }
                    break;
                case 26:
                    // ES3.g3:1:157: VOID
                    {
                        mVOID();

                    }
                    break;
                case 27:
                    // ES3.g3:1:162: WHILE
                    {
                        mWHILE();

                    }
                    break;
                case 28:
                    // ES3.g3:1:168: WITH
                    {
                        mWITH();

                    }
                    break;
                case 29:
                    // ES3.g3:1:173: ABSTRACT
                    {
                        mABSTRACT();

                    }
                    break;
                case 30:
                    // ES3.g3:1:182: BOOLEAN
                    {
                        mBOOLEAN();

                    }
                    break;
                case 31:
                    // ES3.g3:1:190: BYTE
                    {
                        mBYTE();

                    }
                    break;
                case 32:
                    // ES3.g3:1:195: CHAR
                    {
                        mCHAR();

                    }
                    break;
                case 33:
                    // ES3.g3:1:200: CLASS
                    {
                        mCLASS();

                    }
                    break;
                case 34:
                    // ES3.g3:1:206: CONST
                    {
                        mCONST();

                    }
                    break;
                case 35:
                    // ES3.g3:1:212: DEBUGGER
                    {
                        mDEBUGGER();

                    }
                    break;
                case 36:
                    // ES3.g3:1:221: DOUBLE
                    {
                        mDOUBLE();

                    }
                    break;
                case 37:
                    // ES3.g3:1:228: ENUM
                    {
                        mENUM();

                    }
                    break;
                case 38:
                    // ES3.g3:1:233: EXPORT
                    {
                        mEXPORT();

                    }
                    break;
                case 39:
                    // ES3.g3:1:240: EXTENDS
                    {
                        mEXTENDS();

                    }
                    break;
                case 40:
                    // ES3.g3:1:248: FINAL
                    {
                        mFINAL();

                    }
                    break;
                case 41:
                    // ES3.g3:1:254: FLOAT
                    {
                        mFLOAT();

                    }
                    break;
                case 42:
                    // ES3.g3:1:260: GOTO
                    {
                        mGOTO();

                    }
                    break;
                case 43:
                    // ES3.g3:1:265: IMPLEMENTS
                    {
                        mIMPLEMENTS();

                    }
                    break;
                case 44:
                    // ES3.g3:1:276: IMPORT
                    {
                        mIMPORT();

                    }
                    break;
                case 45:
                    // ES3.g3:1:283: INT
                    {
                        mINT();

                    }
                    break;
                case 46:
                    // ES3.g3:1:287: INTERFACE
                    {
                        mINTERFACE();

                    }
                    break;
                case 47:
                    // ES3.g3:1:297: LONG
                    {
                        mLONG();

                    }
                    break;
                case 48:
                    // ES3.g3:1:302: NATIVE
                    {
                        mNATIVE();

                    }
                    break;
                case 49:
                    // ES3.g3:1:309: PACKAGE
                    {
                        mPACKAGE();

                    }
                    break;
                case 50:
                    // ES3.g3:1:317: PRIVATE
                    {
                        mPRIVATE();

                    }
                    break;
                case 51:
                    // ES3.g3:1:325: PROTECTED
                    {
                        mPROTECTED();

                    }
                    break;
                case 52:
                    // ES3.g3:1:335: PUBLIC
                    {
                        mPUBLIC();

                    }
                    break;
                case 53:
                    // ES3.g3:1:342: SHORT
                    {
                        mSHORT();

                    }
                    break;
                case 54:
                    // ES3.g3:1:348: STATIC
                    {
                        mSTATIC();

                    }
                    break;
                case 55:
                    // ES3.g3:1:355: SUPER
                    {
                        mSUPER();

                    }
                    break;
                case 56:
                    // ES3.g3:1:361: SYNCHRONIZED
                    {
                        mSYNCHRONIZED();

                    }
                    break;
                case 57:
                    // ES3.g3:1:374: THROWS
                    {
                        mTHROWS();

                    }
                    break;
                case 58:
                    // ES3.g3:1:381: TRANSIENT
                    {
                        mTRANSIENT();

                    }
                    break;
                case 59:
                    // ES3.g3:1:391: VOLATILE
                    {
                        mVOLATILE();

                    }
                    break;
                case 60:
                    // ES3.g3:1:400: LBRACE
                    {
                        mLBRACE();

                    }
                    break;
                case 61:
                    // ES3.g3:1:407: RBRACE
                    {
                        mRBRACE();

                    }
                    break;
                case 62:
                    // ES3.g3:1:414: LPAREN
                    {
                        mLPAREN();

                    }
                    break;
                case 63:
                    // ES3.g3:1:421: RPAREN
                    {
                        mRPAREN();

                    }
                    break;
                case 64:
                    // ES3.g3:1:428: LBRACK
                    {
                        mLBRACK();

                    }
                    break;
                case 65:
                    // ES3.g3:1:435: RBRACK
                    {
                        mRBRACK();

                    }
                    break;
                case 66:
                    // ES3.g3:1:442: DOT
                    {
                        mDOT();

                    }
                    break;
                case 67:
                    // ES3.g3:1:446: SEMIC
                    {
                        mSEMIC();

                    }
                    break;
                case 68:
                    // ES3.g3:1:452: COMMA
                    {
                        mCOMMA();

                    }
                    break;
                case 69:
                    // ES3.g3:1:458: LT
                    {
                        mLT();

                    }
                    break;
                case 70:
                    // ES3.g3:1:461: GT
                    {
                        mGT();

                    }
                    break;
                case 71:
                    // ES3.g3:1:464: LTE
                    {
                        mLTE();

                    }
                    break;
                case 72:
                    // ES3.g3:1:468: GTE
                    {
                        mGTE();

                    }
                    break;
                case 73:
                    // ES3.g3:1:472: EQ
                    {
                        mEQ();

                    }
                    break;
                case 74:
                    // ES3.g3:1:475: NEQ
                    {
                        mNEQ();

                    }
                    break;
                case 75:
                    // ES3.g3:1:479: SAME
                    {
                        mSAME();

                    }
                    break;
                case 76:
                    // ES3.g3:1:484: NSAME
                    {
                        mNSAME();

                    }
                    break;
                case 77:
                    // ES3.g3:1:490: ADD
                    {
                        mADD();

                    }
                    break;
                case 78:
                    // ES3.g3:1:494: SUB
                    {
                        mSUB();

                    }
                    break;
                case 79:
                    // ES3.g3:1:498: MUL
                    {
                        mMUL();

                    }
                    break;
                case 80:
                    // ES3.g3:1:502: MOD
                    {
                        mMOD();

                    }
                    break;
                case 81:
                    // ES3.g3:1:506: INC
                    {
                        mINC();

                    }
                    break;
                case 82:
                    // ES3.g3:1:510: DEC
                    {
                        mDEC();

                    }
                    break;
                case 83:
                    // ES3.g3:1:514: SHL
                    {
                        mSHL();

                    }
                    break;
                case 84:
                    // ES3.g3:1:518: SHR
                    {
                        mSHR();

                    }
                    break;
                case 85:
                    // ES3.g3:1:522: SHU
                    {
                        mSHU();

                    }
                    break;
                case 86:
                    // ES3.g3:1:526: AND
                    {
                        mAND();

                    }
                    break;
                case 87:
                    // ES3.g3:1:530: OR
                    {
                        mOR();

                    }
                    break;
                case 88:
                    // ES3.g3:1:533: XOR
                    {
                        mXOR();

                    }
                    break;
                case 89:
                    // ES3.g3:1:537: NOT
                    {
                        mNOT();

                    }
                    break;
                case 90:
                    // ES3.g3:1:541: INV
                    {
                        mINV();

                    }
                    break;
                case 91:
                    // ES3.g3:1:545: LAND
                    {
                        mLAND();

                    }
                    break;
                case 92:
                    // ES3.g3:1:550: LOR
                    {
                        mLOR();

                    }
                    break;
                case 93:
                    // ES3.g3:1:554: QUE
                    {
                        mQUE();

                    }
                    break;
                case 94:
                    // ES3.g3:1:558: COLON
                    {
                        mCOLON();

                    }
                    break;
                case 95:
                    // ES3.g3:1:564: ASSIGN
                    {
                        mASSIGN();

                    }
                    break;
                case 96:
                    // ES3.g3:1:571: ADDASS
                    {
                        mADDASS();

                    }
                    break;
                case 97:
                    // ES3.g3:1:578: SUBASS
                    {
                        mSUBASS();

                    }
                    break;
                case 98:
                    // ES3.g3:1:585: MULASS
                    {
                        mMULASS();

                    }
                    break;
                case 99:
                    // ES3.g3:1:592: MODASS
                    {
                        mMODASS();

                    }
                    break;
                case 100:
                    // ES3.g3:1:599: SHLASS
                    {
                        mSHLASS();

                    }
                    break;
                case 101:
                    // ES3.g3:1:606: SHRASS
                    {
                        mSHRASS();

                    }
                    break;
                case 102:
                    // ES3.g3:1:613: SHUASS
                    {
                        mSHUASS();

                    }
                    break;
                case 103:
                    // ES3.g3:1:620: ANDASS
                    {
                        mANDASS();

                    }
                    break;
                case 104:
                    // ES3.g3:1:627: ORASS
                    {
                        mORASS();

                    }
                    break;
                case 105:
                    // ES3.g3:1:633: XORASS
                    {
                        mXORASS();

                    }
                    break;
                case 106:
                    // ES3.g3:1:640: DIV
                    {
                        mDIV();

                    }
                    break;
                case 107:
                    // ES3.g3:1:644: DIVASS
                    {
                        mDIVASS();

                    }
                    break;
                case 108:
                    // ES3.g3:1:651: WhiteSpace
                    {
                        mWhiteSpace();

                    }
                    break;
                case 109:
                    // ES3.g3:1:662: EOL
                    {
                        mEOL();

                    }
                    break;
                case 110:
                    // ES3.g3:1:666: MultiLineComment
                    {
                        mMultiLineComment();

                    }
                    break;
                case 111:
                    // ES3.g3:1:683: SingleLineComment
                    {
                        mSingleLineComment();

                    }
                    break;
                case 112:
                    // ES3.g3:1:701: Identifier
                    {
                        mIdentifier();

                    }
                    break;
                case 113:
                    // ES3.g3:1:712: DecimalLiteral
                    {
                        mDecimalLiteral();

                    }
                    break;
                case 114:
                    // ES3.g3:1:727: OctalIntegerLiteral
                    {
                        mOctalIntegerLiteral();

                    }
                    break;
                case 115:
                    // ES3.g3:1:747: HexIntegerLiteral
                    {
                        mHexIntegerLiteral();

                    }
                    break;
                case 116:
                    // ES3.g3:1:765: StringLiteral
                    {
                        mStringLiteral();

                    }
                    break;
                case 117:
                    // ES3.g3:1:779: RegularExpressionLiteral
                    {
                        mRegularExpressionLiteral();

                    }
                    break;

            }

        }


        protected DFA19 dfa19;
        protected DFA32 dfa32;
        private void InitializeCyclicDFAs()
        {
            this.dfa19 = new DFA19(this);
            this.dfa32 = new DFA32(this);
            this.dfa32.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA32_SpecialStateTransition);
        }

        const string DFA19_eotS =
            "\x01\uffff\x02\x04\x03\uffff\x01\x04";
        const string DFA19_eofS =
            "\x07\uffff";
        const string DFA19_minS =
            "\x03\x2e\x03\uffff\x01\x2e";
        const string DFA19_maxS =
            "\x01\x39\x01\x2e\x01\x39\x03\uffff\x01\x39";
        const string DFA19_acceptS =
            "\x03\uffff\x01\x02\x01\x03\x01\x01\x01\uffff";
        const string DFA19_specialS =
            "\x07\uffff}>";
        static readonly string[] DFA19_transitionS = {
            "\x01\x03\x01\uffff\x01\x01\x09\x02",
            "\x01\x05",
            "\x01\x05\x01\uffff\x0a\x06",
            "",
            "",
            "",
            "\x01\x05\x01\uffff\x0a\x06"
    };

        static readonly short[] DFA19_eot = DFA.UnpackEncodedString(DFA19_eotS);
        static readonly short[] DFA19_eof = DFA.UnpackEncodedString(DFA19_eofS);
        static readonly char[] DFA19_min = DFA.UnpackEncodedStringToUnsignedChars(DFA19_minS);
        static readonly char[] DFA19_max = DFA.UnpackEncodedStringToUnsignedChars(DFA19_maxS);
        static readonly short[] DFA19_accept = DFA.UnpackEncodedString(DFA19_acceptS);
        static readonly short[] DFA19_special = DFA.UnpackEncodedString(DFA19_specialS);
        static readonly short[][] DFA19_transition = DFA.UnpackEncodedStringArray(DFA19_transitionS);

        protected class DFA19 : DFA
        {
            public DFA19(BaseRecognizer recognizer)
            {
                this.recognizer = recognizer;
                this.decisionNumber = 19;
                this.eot = DFA19_eot;
                this.eof = DFA19_eof;
                this.min = DFA19_min;
                this.max = DFA19_max;
                this.accept = DFA19_accept;
                this.special = DFA19_special;
                this.transition = DFA19_transition;

            }

            override public string Description
            {
                get { return "529:1: DecimalLiteral : ( DecimalIntegerLiteral '.' ( DecimalDigit )* ( ExponentPart )? | '.' ( DecimalDigit )+ ( ExponentPart )? | DecimalIntegerLiteral ( ExponentPart )? );"; }
            }

        }

        const string DFA32_eotS =
            "\x11\x2b\x06\uffff\x01\x59\x02\uffff\x01\x5c\x01\x5f\x01\x61\x01" +
            "\x63\x01\x66\x01\x69\x01\x6b\x01\x6d\x01\x70\x01\x73\x01\x75\x03" +
            "\uffff\x01\x79\x03\uffff\x01\x2d\x02\uffff\x13\x2b\x01\u0097\x03" +
            "\x2b\x01\u009c\x01\u009f\x11\x2b\x02\uffff\x01\u00b4\x02\uffff\x01" +
            "\u00b7\x01\uffff\x01\u00b9\x01\uffff\x01\u00bb\x13\uffff\x01\u00bc" +
            "\x06\uffff\x01\x2b\x01\u00be\x02\x2b\x01\u00c1\x06\x2b\x01\u00c8" +
            "\x0e\x2b\x01\uffff\x04\x2b\x01\uffff\x01\x2b\x01\u00de\x01\uffff" +
            "\x07\x2b\x01\u00e7\x0b\x2b\x02\uffff\x01\u00f4\x07\uffff\x01\u00f5" +
            "\x01\uffff\x01\x2b\x01\u00f7\x01\uffff\x01\x2b\x01\u00f9\x04\x2b" +
            "\x01\uffff\x04\x2b\x01\u0102\x01\u0103\x03\x2b\x01\u0107\x05\x2b" +
            "\x01\u010d\x01\u010e\x04\x2b\x01\uffff\x08\x2b\x01\uffff\x01\u011b" +
            "\x02\x2b\x01\u011e\x01\x2b\x01\u0120\x01\u0121\x04\x2b\x03\uffff" +
            "\x01\x2b\x01\uffff\x01\x2b\x01\uffff\x01\u0129\x01\x2b\x01\u012b" +
            "\x01\u012d\x01\x2b\x01\u012f\x01\u0130\x01\x2b\x02\uffff\x01\u0132" +
            "\x01\x2b\x01\u0134\x01\uffff\x01\u0135\x04\x2b\x02\uffff\x08\x2b" +
            "\x01\u0142\x01\x2b\x01\u0144\x01\x2b\x01\uffff\x01\x2b\x01\u0147" +
            "\x01\uffff\x01\x2b\x02\uffff\x04\x2b\x01\u014d\x01\x2b\x01\u014f" +
            "\x01\uffff\x01\u0150\x01\uffff\x01\x2b\x01\uffff\x01\x2b\x02\uffff" +
            "\x01\x2b\x01\uffff\x01\x2b\x02\uffff\x01\x2b\x01\u0156\x01\x2b\x01" +
            "\u0158\x01\u0159\x04\x2b\x01\u015e\x01\u015f\x01\u0160\x01\uffff" +
            "\x01\u0161\x01\uffff\x02\x2b\x01\uffff\x04\x2b\x01\u0168\x01\uffff" +
            "\x01\x2b\x02\uffff\x01\u016a\x01\x2b\x01\u016c\x01\x2b\x01\u016e" +
            "\x01\uffff\x01\x2b\x02\uffff\x01\u0170\x03\x2b\x04\uffff\x03\x2b" +
            "\x01\u0177\x01\u0178\x01\x2b\x01\uffff\x01\x2b\x01\uffff\x01\u017b" +
            "\x01\uffff\x01\u017c\x01\uffff\x01\u017d\x01\uffff\x04\x2b\x01\u0182" +
            "\x01\u0183\x02\uffff\x01\x2b\x01\u0185\x03\uffff\x01\x2b\x01\u0187" +
            "\x02\x2b\x02\uffff\x01\u018a\x01\uffff\x01\u018b\x01\uffff\x01\u018c" +
            "\x01\x2b\x03\uffff\x01\x2b\x01\u018f\x01\uffff";
        const string DFA32_eofS =
            "\u0190\uffff";
        const string DFA32_minS =
            "\x01\x09\x01\x61\x01\x68\x01\x61\x01\x6f\x01\x61\x01\x65\x01\x6c" +
            "\x01\x66\x01\x65\x01\x68\x01\x61\x01\x68\x01\x62\x02\x6f\x01\x61" +
            "\x06\uffff\x01\x30\x02\uffff\x01\x3c\x03\x3d\x01\x2b\x01\x2d\x02" +
            "\x3d\x01\x26\x02\x3d\x03\uffff\x01\x00\x03\uffff\x01\x30\x02\uffff" +
            "\x01\x6c\x01\x77\x01\x74\x01\x61\x01\x69\x01\x70\x01\x6c\x01\x6e" +
            "\x01\x72\x01\x6e\x01\x6f\x01\x65\x01\x6f\x01\x74\x01\x73\x01\x6e" +
            "\x02\x61\x01\x62\x01\x24\x01\x73\x01\x75\x01\x70\x02\x24\x01\x70" +
            "\x01\x74\x01\x69\x01\x6f\x01\x61\x01\x70\x01\x6e\x01\x72\x02\x69" +
            "\x01\x74\x01\x73\x01\x74\x01\x6e\x01\x63\x01\x69\x01\x62\x02\uffff" +
            "\x01\x3d\x02\uffff\x01\x3d\x01\uffff\x01\x3d\x01\uffff\x01\x3d\x13" +
            "\uffff\x01\x00\x06\uffff\x01\x6c\x01\x24\x01\x69\x01\x65\x01\x24" +
            "\x01\x6e\x01\x73\x01\x6f\x01\x65\x01\x73\x01\x61\x01\x24\x01\x63" +
            "\x02\x61\x01\x6c\x02\x65\x01\x63\x01\x73\x01\x72\x01\x73\x01\x61" +
            "\x01\x65\x01\x75\x01\x62\x01\uffff\x01\x65\x01\x6d\x01\x6f\x01\x65" +
            "\x01\uffff\x01\x74\x01\x24\x01\uffff\x01\x6c\x01\x75\x01\x74\x01" +
            "\x72\x01\x74\x01\x65\x01\x63\x01\x24\x01\x64\x01\x61\x01\x6c\x01" +
            "\x68\x01\x74\x01\x6f\x01\x67\x01\x6b\x01\x76\x01\x74\x01\x6c\x02" +
            "\uffff\x01\x3d\x07\uffff\x01\x24\x01\uffff\x01\x76\x01\x24\x01\uffff" +
            "\x01\x73\x01\x24\x01\x77\x01\x6f\x01\x65\x01\x6c\x01\uffff\x02\x74" +
            "\x01\x6b\x01\x65\x02\x24\x01\x68\x01\x69\x01\x74\x01\x24\x01\x73" +
            "\x01\x75\x01\x74\x01\x67\x01\x6c\x02\x24\x01\x72\x01\x6e\x01\x61" +
            "\x01\x72\x01\uffff\x01\x65\x02\x72\x01\x63\x01\x74\x01\x69\x01\x72" +
            "\x01\x68\x01\uffff\x01\x24\x01\x74\x01\x65\x01\x24\x01\x72\x02\x24" +
            "\x02\x61\x01\x65\x01\x69\x03\uffff\x01\x65\x01\uffff\x01\x69\x01" +
            "\uffff\x01\x24\x01\x66\x02\x24\x01\x69\x02\x24\x01\x61\x02\uffff" +
            "\x01\x24\x01\x6e\x01\x24\x01\uffff\x01\x24\x01\x6c\x01\x65\x01\x67" +
            "\x01\x65\x02\uffff\x01\x74\x01\x64\x01\x6e\x01\x66\x01\x6d\x01\x74" +
            "\x01\x6e\x01\x68\x01\x24\x01\x63\x01\x24\x01\x72\x01\uffff\x01\x69" +
            "\x01\x24\x01\uffff\x01\x61\x02\uffff\x01\x67\x01\x74\x02\x63\x01" +
            "\x24\x01\x65\x01\x24\x01\uffff\x01\x24\x01\uffff\x01\x79\x01\uffff" +
            "\x01\x6f\x02\uffff\x01\x6e\x01\uffff\x01\x75\x02\uffff\x01\x74\x01" +
            "\x24\x01\x65\x02\x24\x01\x73\x01\x63\x01\x61\x01\x65\x03\x24\x01" +
            "\uffff\x01\x24\x01\uffff\x01\x6f\x01\x6c\x01\uffff\x01\x63\x02\x65" +
            "\x01\x74\x01\x24\x01\uffff\x01\x6e\x02\uffff\x01\x24\x01\x6e\x01" +
            "\x24\x01\x65\x01\x24\x01\uffff\x01\x72\x02\uffff\x01\x24\x01\x65" +
            "\x01\x63\x01\x6e\x04\uffff\x01\x6e\x01\x65\x01\x74\x02\x24\x01\x65" +
            "\x01\uffff\x01\x74\x01\uffff\x01\x24\x01\uffff\x01\x24\x01\uffff" +
            "\x01\x24\x01\uffff\x01\x6f\x01\x65\x01\x74\x01\x69\x02\x24\x02\uffff" +
            "\x01\x64\x01\x24\x03\uffff\x01\x66\x01\x24\x01\x73\x01\x7a\x02\uffff" +
            "\x01\x24\x01\uffff\x01\x24\x01\uffff\x01\x24\x01\x65\x03\uffff\x01" +
            "\x64\x01\x24\x01\uffff";
        const string DFA32_maxS =
            "\x01\u3000\x01\x75\x01\x79\x01\x75\x01\x79\x02\x6f\x01\x78\x01" +
            "\x6e\x01\x65\x01\x79\x01\x6f\x01\x69\x01\x62\x02\x6f\x01\x75\x06" +
            "\uffff\x01\x39\x02\uffff\x01\x3d\x01\x3e\x07\x3d\x01\x7c\x01\x3d" +
            "\x03\uffff\x01\uffff\x03\uffff\x01\x78\x02\uffff\x01\x6c\x01\x77" +
            "\x01\x74\x01\x79\x01\x72\x01\x70\x01\x6c\x01\x6e\x01\x72\x01\x6e" +
            "\x01\x6f\x01\x65\x01\x6f\x02\x74\x01\x6e\x02\x61\x01\x6c\x01\x7a" +
            "\x01\x73\x01\x75\x01\x74\x02\x7a\x01\x70\x01\x74\x01\x69\x01\x6f" +
            "\x01\x61\x01\x70\x01\x6e\x01\x72\x01\x6c\x01\x69\x01\x74\x01\x73" +
            "\x01\x74\x01\x6e\x01\x63\x01\x6f\x01\x62\x02\uffff\x01\x3d\x02\uffff" +
            "\x01\x3e\x01\uffff\x01\x3d\x01\uffff\x01\x3d\x13\uffff\x01\uffff" +
            "\x06\uffff\x01\x6c\x01\x7a\x01\x69\x01\x65\x01\x7a\x01\x6e\x01\x73" +
            "\x01\x6f\x01\x65\x01\x73\x01\x61\x01\x7a\x01\x63\x02\x61\x01\x6c" +
            "\x02\x65\x01\x63\x01\x74\x01\x72\x01\x73\x01\x61\x01\x65\x01\x75" +
            "\x01\x62\x01\uffff\x01\x65\x01\x6d\x01\x6f\x01\x65\x01\uffff\x01" +
            "\x74\x01\x7a\x01\uffff\x01\x6f\x01\x75\x01\x74\x01\x72\x01\x74\x01" +
            "\x65\x01\x63\x01\x7a\x01\x64\x01\x61\x01\x6c\x01\x68\x01\x74\x01" +
            "\x6f\x01\x67\x01\x6b\x01\x76\x01\x74\x01\x6c\x02\uffff\x01\x3d\x07" +
            "\uffff\x01\x7a\x01\uffff\x01\x76\x01\x7a\x01\uffff\x01\x73\x01\x7a" +
            "\x01\x77\x01\x6f\x01\x65\x01\x6c\x01\uffff\x02\x74\x01\x6b\x01\x65" +
            "\x02\x7a\x01\x68\x01\x69\x01\x74\x01\x7a\x01\x73\x01\x75\x01\x74" +
            "\x01\x67\x01\x6c\x02\x7a\x01\x72\x01\x6e\x01\x61\x01\x72\x01\uffff" +
            "\x01\x65\x02\x72\x01\x63\x01\x74\x01\x69\x01\x72\x01\x68\x01\uffff" +
            "\x01\x7a\x01\x74\x01\x65\x01\x7a\x01\x72\x02\x7a\x02\x61\x01\x65" +
            "\x01\x69\x03\uffff\x01\x65\x01\uffff\x01\x69\x01\uffff\x01\x7a\x01" +
            "\x66\x02\x7a\x01\x69\x02\x7a\x01\x61\x02\uffff\x01\x7a\x01\x6e\x01" +
            "\x7a\x01\uffff\x01\x7a\x01\x6c\x01\x65\x01\x67\x01\x65\x02\uffff" +
            "\x01\x74\x01\x64\x01\x6e\x01\x66\x01\x6d\x01\x74\x01\x6e\x01\x68" +
            "\x01\x7a\x01\x63\x01\x7a\x01\x72\x01\uffff\x01\x69\x01\x7a\x01\uffff" +
            "\x01\x61\x02\uffff\x01\x67\x01\x74\x02\x63\x01\x7a\x01\x65\x01\x7a" +
            "\x01\uffff\x01\x7a\x01\uffff\x01\x79\x01\uffff\x01\x6f\x02\uffff" +
            "\x01\x6e\x01\uffff\x01\x75\x02\uffff\x01\x74\x01\x7a\x01\x65\x02" +
            "\x7a\x01\x73\x01\x63\x01\x61\x01\x65\x03\x7a\x01\uffff\x01\x7a\x01" +
            "\uffff\x01\x6f\x01\x6c\x01\uffff\x01\x63\x02\x65\x01\x74\x01\x7a" +
            "\x01\uffff\x01\x6e\x02\uffff\x01\x7a\x01\x6e\x01\x7a\x01\x65\x01" +
            "\x7a\x01\uffff\x01\x72\x02\uffff\x01\x7a\x01\x65\x01\x63\x01\x6e" +
            "\x04\uffff\x01\x6e\x01\x65\x01\x74\x02\x7a\x01\x65\x01\uffff\x01" +
            "\x74\x01\uffff\x01\x7a\x01\uffff\x01\x7a\x01\uffff\x01\x7a\x01\uffff" +
            "\x01\x6f\x01\x65\x01\x74\x01\x69\x02\x7a\x02\uffff\x01\x64\x01\x7a" +
            "\x03\uffff\x01\x66\x01\x7a\x01\x73\x01\x7a\x02\uffff\x01\x7a\x01" +
            "\uffff\x01\x7a\x01\uffff\x01\x7a\x01\x65\x03\uffff\x01\x64\x01\x7a" +
            "\x01\uffff";
        const string DFA32_acceptS =
            "\x11\uffff\x01\x3c\x01\x3d\x01\x3e\x01\x3f\x01\x40\x01\x41\x01" +
            "\uffff\x01\x43\x01\x44\x0b\uffff\x01\x5a\x01\x5d\x01\x5e\x01\uffff" +
            "\x01\x6c\x01\x6d\x01\x70\x01\uffff\x01\x71\x01\x74\x2a\uffff\x01" +
            "\x42\x01\x47\x01\uffff\x01\x45\x01\x48\x01\uffff\x01\x46\x01\uffff" +
            "\x01\x5f\x01\uffff\x01\x59\x01\x51\x01\x60\x01\x4d\x01\x52\x01\x61" +
            "\x01\x4e\x01\x62\x01\x4f\x01\x63\x01\x50\x01\x5b\x01\x67\x01\x56" +
            "\x01\x5c\x01\x68\x01\x57\x01\x69\x01\x58\x01\uffff\x01\x6e\x01\x6f" +
            "\x01\x6a\x01\x75\x01\x73\x01\x72\x1a\uffff\x01\x0a\x04\uffff\x01" +
            "\x0f\x02\uffff\x01\x10\x13\uffff\x01\x64\x01\x53\x01\uffff\x01\x65" +
            "\x01\x54\x01\x4b\x01\x49\x01\x4c\x01\x4a\x01\x6b\x01\uffff\x01\x12" +
            "\x02\uffff\x01\x17\x06\uffff\x01\x0d\x15\uffff\x01\x2d\x08\uffff" +
            "\x01\x19\x0b\uffff\x01\x66\x01\x55\x01\x01\x01\uffff\x01\x02\x01" +
            "\uffff\x01\x15\x08\uffff\x01\x1f\x01\x05\x03\uffff\x01\x20\x05\uffff" +
            "\x01\x0b\x01\x25\x0c\uffff\x01\x1a\x02\uffff\x01\x1c\x01\uffff\x01" +
            "\x2a\x01\x2f\x07\uffff\x01\x16\x01\uffff\x01\x03\x01\uffff\x01\x28" +
            "\x01\uffff\x01\x29\x01\x04\x01\uffff\x01\x06\x01\uffff\x01\x22\x01" +
            "\x21\x0c\uffff\x01\x35\x01\uffff\x01\x37\x02\uffff\x01\x1b\x05\uffff" +
            "\x01\x30\x01\uffff\x01\x39\x01\x18\x05\uffff\x01\x09\x01\uffff\x01" +
            "\x24\x01\x26\x04\uffff\x01\x2c\x01\x13\x01\x14\x01\x36\x06\uffff" +
            "\x01\x34\x01\uffff\x01\x0c\x01\uffff\x01\x1e\x01\uffff\x01\x08\x01" +
            "\uffff\x01\x27\x06\uffff\x01\x31\x01\x32\x02\uffff\x01\x0e\x01\x07" +
            "\x01\x23\x04\uffff\x01\x3b\x01\x1d\x01\uffff\x01\x3a\x01\uffff\x01" +
            "\x2e\x02\uffff\x01\x33\x01\x11\x01\x2b\x02\uffff\x01\x38";
        const string DFA32_specialS =
            "\x28\uffff\x01\x00\x4d\uffff\x01\x01\u0119\uffff}>";
        static readonly string[] DFA32_transitionS = {
            "\x01\x29\x01\x2a\x02\x29\x01\x2a\x12\uffff\x01\x29\x01\x1d"+
            "\x01\x2e\x02\uffff\x01\x21\x01\x22\x01\x2e\x01\x13\x01\x14\x01"+
            "\x20\x01\x1e\x01\x19\x01\x1f\x01\x17\x01\x28\x01\x2c\x09\x2d"+
            "\x01\x27\x01\x18\x01\x1a\x01\x1c\x01\x1b\x01\x26\x1b\uffff\x01"+
            "\x15\x01\uffff\x01\x16\x01\x24\x02\uffff\x01\x0d\x01\x04\x01"+
            "\x05\x01\x06\x01\x07\x01\x03\x01\x0e\x01\uffff\x01\x08\x02\uffff"+
            "\x01\x0f\x01\uffff\x01\x01\x01\uffff\x01\x10\x01\uffff\x01\x09"+
            "\x01\x0a\x01\x02\x01\uffff\x01\x0b\x01\x0c\x03\uffff\x01\x11"+
            "\x01\x23\x01\x12\x01\x25\x21\uffff\x01\x29\u15df\uffff\x01\x29"+
            "\u018d\uffff\x01\x29\u07f1\uffff\x0b\x29\x1d\uffff\x02\x2a\x05"+
            "\uffff\x01\x29\x2f\uffff\x01\x29\u0fa0\uffff\x01\x29",
            "\x01\x31\x03\uffff\x01\x30\x0f\uffff\x01\x2f",
            "\x01\x33\x09\uffff\x01\x32\x06\uffff\x01\x34",
            "\x01\x35\x07\uffff\x01\x36\x02\uffff\x01\x39\x02\uffff\x01"+
            "\x37\x05\uffff\x01\x38",
            "\x01\x3b\x02\uffff\x01\x3a\x06\uffff\x01\x3c",
            "\x01\x3d\x06\uffff\x01\x3f\x03\uffff\x01\x40\x02\uffff\x01"+
            "\x3e",
            "\x01\x41\x09\uffff\x01\x42",
            "\x01\x43\x01\uffff\x01\x44\x09\uffff\x01\x45",
            "\x01\x46\x06\uffff\x01\x48\x01\x47",
            "\x01\x49",
            "\x01\x4b\x0b\uffff\x01\x4c\x01\x4d\x01\uffff\x01\x4a\x01\uffff"+
            "\x01\x4e",
            "\x01\x4f\x0d\uffff\x01\x50",
            "\x01\x51\x01\x52",
            "\x01\x53",
            "\x01\x54",
            "\x01\x55",
            "\x01\x56\x10\uffff\x01\x57\x02\uffff\x01\x58",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x0a\x2d",
            "",
            "",
            "\x01\x5b\x01\x5a",
            "\x01\x5d\x01\x5e",
            "\x01\x60",
            "\x01\x62",
            "\x01\x64\x11\uffff\x01\x65",
            "\x01\x67\x0f\uffff\x01\x68",
            "\x01\x6a",
            "\x01\x6c",
            "\x01\x6e\x16\uffff\x01\x6f",
            "\x01\x72\x3e\uffff\x01\x71",
            "\x01\x74",
            "",
            "",
            "",
            "\x0a\x7a\x01\uffff\x02\x7a\x01\uffff\x1c\x7a\x01\x77\x04\x7a"+
            "\x01\x78\x0d\x7a\x01\x76\u1fea\x7a\x02\uffff\udfd6\x7a",
            "",
            "",
            "",
            "\x08\x7c\x20\uffff\x01\x7b\x1f\uffff\x01\x7b",
            "",
            "",
            "\x01\x7d",
            "\x01\x7e",
            "\x01\x7f",
            "\x01\u0082\x13\uffff\x01\u0080\x03\uffff\x01\u0081",
            "\x01\u0083\x08\uffff\x01\u0084",
            "\x01\u0085",
            "\x01\u0086",
            "\x01\u0087",
            "\x01\u0088",
            "\x01\u0089",
            "\x01\u008a",
            "\x01\u008b",
            "\x01\u008c",
            "\x01\u008d",
            "\x01\u008e\x01\u008f",
            "\x01\u0090",
            "\x01\u0091",
            "\x01\u0092",
            "\x01\u0095\x03\uffff\x01\u0093\x05\uffff\x01\u0094",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x14\x2b\x01\u0096\x05\x2b",
            "\x01\u0098",
            "\x01\u0099",
            "\x01\u009a\x03\uffff\x01\u009b",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x12\x2b\x01\u009d\x01\u009e"+
            "\x06\x2b",
            "\x01\u00a0",
            "\x01\u00a1",
            "\x01\u00a2",
            "\x01\u00a3",
            "\x01\u00a4",
            "\x01\u00a5",
            "\x01\u00a6",
            "\x01\u00a7",
            "\x01\u00a8\x02\uffff\x01\u00a9",
            "\x01\u00aa",
            "\x01\u00ab",
            "\x01\u00ac",
            "\x01\u00ad",
            "\x01\u00ae",
            "\x01\u00af",
            "\x01\u00b0\x05\uffff\x01\u00b1",
            "\x01\u00b2",
            "",
            "",
            "\x01\u00b3",
            "",
            "",
            "\x01\u00b6\x01\u00b5",
            "",
            "\x01\u00b8",
            "",
            "\x01\u00ba",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x0a\x7a\x01\uffff\x02\x7a\x01\uffff\u201a\x7a\x02\uffff\udfd6"+
            "\x7a",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x01\u00bd",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u00bf",
            "\x01\u00c0",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u00c2",
            "\x01\u00c3",
            "\x01\u00c4",
            "\x01\u00c5",
            "\x01\u00c6",
            "\x01\u00c7",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u00c9",
            "\x01\u00ca",
            "\x01\u00cb",
            "\x01\u00cc",
            "\x01\u00cd",
            "\x01\u00ce",
            "\x01\u00cf",
            "\x01\u00d1\x01\u00d0",
            "\x01\u00d2",
            "\x01\u00d3",
            "\x01\u00d4",
            "\x01\u00d5",
            "\x01\u00d6",
            "\x01\u00d7",
            "",
            "\x01\u00d8",
            "\x01\u00d9",
            "\x01\u00da",
            "\x01\u00db",
            "",
            "\x01\u00dc",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x04\x2b\x01\u00dd\x15\x2b",
            "",
            "\x01\u00df\x02\uffff\x01\u00e0",
            "\x01\u00e1",
            "\x01\u00e2",
            "\x01\u00e3",
            "\x01\u00e4",
            "\x01\u00e5",
            "\x01\u00e6",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u00e8",
            "\x01\u00e9",
            "\x01\u00ea",
            "\x01\u00eb",
            "\x01\u00ec",
            "\x01\u00ed",
            "\x01\u00ee",
            "\x01\u00ef",
            "\x01\u00f0",
            "\x01\u00f1",
            "\x01\u00f2",
            "",
            "",
            "\x01\u00f3",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "",
            "\x01\u00f6",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "",
            "\x01\u00f8",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u00fa",
            "\x01\u00fb",
            "\x01\u00fc",
            "\x01\u00fd",
            "",
            "\x01\u00fe",
            "\x01\u00ff",
            "\x01\u0100",
            "\x01\u0101",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u0104",
            "\x01\u0105",
            "\x01\u0106",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u0108",
            "\x01\u0109",
            "\x01\u010a",
            "\x01\u010b",
            "\x01\u010c",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u010f",
            "\x01\u0110",
            "\x01\u0111",
            "\x01\u0112",
            "",
            "\x01\u0113",
            "\x01\u0114",
            "\x01\u0115",
            "\x01\u0116",
            "\x01\u0117",
            "\x01\u0118",
            "\x01\u0119",
            "\x01\u011a",
            "",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u011c",
            "\x01\u011d",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u011f",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u0122",
            "\x01\u0123",
            "\x01\u0124",
            "\x01\u0125",
            "",
            "",
            "",
            "\x01\u0126",
            "",
            "\x01\u0127",
            "",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x12\x2b\x01\u0128\x07\x2b",
            "\x01\u012a",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x0b\x2b\x01\u012c\x0e\x2b",
            "\x01\u012e",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u0131",
            "",
            "",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u0133",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u0136",
            "\x01\u0137",
            "\x01\u0138",
            "\x01\u0139",
            "",
            "",
            "\x01\u013a",
            "\x01\u013b",
            "\x01\u013c",
            "\x01\u013d",
            "\x01\u013e",
            "\x01\u013f",
            "\x01\u0140",
            "\x01\u0141",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u0143",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u0145",
            "",
            "\x01\u0146",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "",
            "\x01\u0148",
            "",
            "",
            "\x01\u0149",
            "\x01\u014a",
            "\x01\u014b",
            "\x01\u014c",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u014e",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "",
            "\x01\u0151",
            "",
            "\x01\u0152",
            "",
            "",
            "\x01\u0153",
            "",
            "\x01\u0154",
            "",
            "",
            "\x01\u0155",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u0157",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u015a",
            "\x01\u015b",
            "\x01\u015c",
            "\x01\u015d",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "",
            "\x01\u0162",
            "\x01\u0163",
            "",
            "\x01\u0164",
            "\x01\u0165",
            "\x01\u0166",
            "\x01\u0167",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "",
            "\x01\u0169",
            "",
            "",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u016b",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u016d",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "",
            "\x01\u016f",
            "",
            "",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u0171",
            "\x01\u0172",
            "\x01\u0173",
            "",
            "",
            "",
            "",
            "\x01\u0174",
            "\x01\u0175",
            "\x01\u0176",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u0179",
            "",
            "\x01\u017a",
            "",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "",
            "\x01\u017e",
            "\x01\u017f",
            "\x01\u0180",
            "\x01\u0181",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "",
            "",
            "\x01\u0184",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "",
            "",
            "",
            "\x01\u0186",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u0188",
            "\x01\u0189",
            "",
            "",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            "\x01\u018d",
            "",
            "",
            "",
            "\x01\u018e",
            "\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01"+
            "\x2b\x02\uffff\x01\x2b\x01\uffff\x1a\x2b",
            ""
    };

        static readonly short[] DFA32_eot = DFA.UnpackEncodedString(DFA32_eotS);
        static readonly short[] DFA32_eof = DFA.UnpackEncodedString(DFA32_eofS);
        static readonly char[] DFA32_min = DFA.UnpackEncodedStringToUnsignedChars(DFA32_minS);
        static readonly char[] DFA32_max = DFA.UnpackEncodedStringToUnsignedChars(DFA32_maxS);
        static readonly short[] DFA32_accept = DFA.UnpackEncodedString(DFA32_acceptS);
        static readonly short[] DFA32_special = DFA.UnpackEncodedString(DFA32_specialS);
        static readonly short[][] DFA32_transition = DFA.UnpackEncodedStringArray(DFA32_transitionS);

        protected class DFA32 : DFA
        {
            public DFA32(BaseRecognizer recognizer)
            {
                this.recognizer = recognizer;
                this.decisionNumber = 32;
                this.eot = DFA32_eot;
                this.eof = DFA32_eof;
                this.min = DFA32_min;
                this.max = DFA32_max;
                this.accept = DFA32_accept;
                this.special = DFA32_special;
                this.transition = DFA32_transition;

            }

            override public string Description
            {
                get { return "1:1: Tokens : ( NULL | TRUE | FALSE | BREAK | CASE | CATCH | CONTINUE | DEFAULT | DELETE | DO | ELSE | FINALLY | FOR | FUNCTION | IF | IN | INSTANCEOF | NEW | RETURN | SWITCH | THIS | THROW | TRY | TYPEOF | VAR | VOID | WHILE | WITH | ABSTRACT | BOOLEAN | BYTE | CHAR | CLASS | CONST | DEBUGGER | DOUBLE | ENUM | EXPORT | EXTENDS | FINAL | FLOAT | GOTO | IMPLEMENTS | IMPORT | INT | INTERFACE | LONG | NATIVE | PACKAGE | PRIVATE | PROTECTED | PUBLIC | SHORT | STATIC | SUPER | SYNCHRONIZED | THROWS | TRANSIENT | VOLATILE | LBRACE | RBRACE | LPAREN | RPAREN | LBRACK | RBRACK | DOT | SEMIC | COMMA | LT | GT | LTE | GTE | EQ | NEQ | SAME | NSAME | ADD | SUB | MUL | MOD | INC | DEC | SHL | SHR | SHU | AND | OR | XOR | NOT | INV | LAND | LOR | QUE | COLON | ASSIGN | ADDASS | SUBASS | MULASS | MODASS | SHLASS | SHRASS | SHUASS | ANDASS | ORASS | XORASS | DIV | DIVASS | WhiteSpace | EOL | MultiLineComment | SingleLineComment | Identifier | DecimalLiteral | OctalIntegerLiteral | HexIntegerLiteral | StringLiteral | RegularExpressionLiteral );"; }
            }

        }


        protected internal int DFA32_SpecialStateTransition(DFA dfa, int s, IIntStream _input) //throws NoViableAltException
        {
            IIntStream input = _input;
            int _s = s;
            switch (s)
            {
                case 0:
                    int LA32_40 = input.LA(1);


                    int index32_40 = input.Index();
                    input.Rewind();
                    s = -1;
                    if ((LA32_40 == '=')) { s = 118; }

                    else if ((LA32_40 == '*')) { s = 119; }

                    else if ((LA32_40 == '/')) { s = 120; }

                    else if (((LA32_40 >= '\u0000' && LA32_40 <= '\t') || (LA32_40 >= '\u000B' && LA32_40 <= '\f') || (LA32_40 >= '\u000E' && LA32_40 <= ')') || (LA32_40 >= '+' && LA32_40 <= '.') || (LA32_40 >= '0' && LA32_40 <= '<') || (LA32_40 >= '>' && LA32_40 <= '\u2027') || (LA32_40 >= '\u202A' && LA32_40 <= '\uFFFF')) && ((AreRegularExpressionsEnabled))) { s = 122; }

                    else s = 121;


                    input.Seek(index32_40);
                    if (s >= 0) return s;
                    break;
                case 1:
                    int LA32_118 = input.LA(1);


                    int index32_118 = input.Index();
                    input.Rewind();
                    s = -1;
                    if (((LA32_118 >= '\u0000' && LA32_118 <= '\t') || (LA32_118 >= '\u000B' && LA32_118 <= '\f') || (LA32_118 >= '\u000E' && LA32_118 <= '\u2027') || (LA32_118 >= '\u202A' && LA32_118 <= '\uFFFF')) && ((AreRegularExpressionsEnabled))) { s = 122; }

                    else s = 188;


                    input.Seek(index32_118);
                    if (s >= 0) return s;
                    break;
            }
            NoViableAltException nvae32 =
                new NoViableAltException(dfa.Description, 32, _s, input);
            dfa.Error(nvae32);
            throw nvae32;
        }


    }
}