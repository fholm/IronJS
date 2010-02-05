// $ANTLR 3.2 Sep 23, 2009 12:02:23 ES3.g3 2009-12-30 11:53:12

#pragma warning disable 168, 219
#pragma warning disable 162
#pragma warning disable 219, 162

using System;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

using IList = System.Collections.IList;
using ArrayList = System.Collections.ArrayList;
using Stack = Antlr.Runtime.Collections.StackList;

namespace IronJS.Compiler.Parser
{
    partial class ES3Parser : Antlr.Runtime.Parser
    {
        public static readonly string[] tokenNames = new string[] 
	    {
            "<invalid>", 
    		"<EOR>", 
    		"<DOWN>", 
    		"<UP>", 
    		"NULL", 
    		"TRUE", 
    		"FALSE", 
    		"BREAK", 
    		"CASE", 
    		"CATCH", 
    		"CONTINUE", 
    		"DEFAULT", 
    		"DELETE", 
    		"DO", 
    		"ELSE", 
    		"FINALLY", 
    		"FOR", 
    		"FUNCTION", 
    		"IF", 
    		"IN", 
    		"INSTANCEOF", 
    		"NEW", 
    		"RETURN", 
    		"SWITCH", 
    		"THIS", 
    		"THROW", 
    		"TRY", 
    		"TYPEOF", 
    		"VAR", 
    		"VOID", 
    		"WHILE", 
    		"WITH", 
    		"ABSTRACT", 
    		"BOOLEAN", 
    		"BYTE", 
    		"CHAR", 
    		"CLASS", 
    		"CONST", 
    		"DEBUGGER", 
    		"DOUBLE", 
    		"ENUM", 
    		"EXPORT", 
    		"EXTENDS", 
    		"FINAL", 
    		"FLOAT", 
    		"GOTO", 
    		"IMPLEMENTS", 
    		"IMPORT", 
    		"INT", 
    		"INTERFACE", 
    		"LONG", 
    		"NATIVE", 
    		"PACKAGE", 
    		"PRIVATE", 
    		"PROTECTED", 
    		"PUBLIC", 
    		"SHORT", 
    		"STATIC", 
    		"SUPER", 
    		"SYNCHRONIZED", 
    		"THROWS", 
    		"TRANSIENT", 
    		"VOLATILE", 
    		"LBRACE", 
    		"RBRACE", 
    		"LPAREN", 
    		"RPAREN", 
    		"LBRACK", 
    		"RBRACK", 
    		"DOT", 
    		"SEMIC", 
    		"COMMA", 
    		"LT", 
    		"GT", 
    		"LTE", 
    		"GTE", 
    		"EQ", 
    		"NEQ", 
    		"SAME", 
    		"NSAME", 
    		"ADD", 
    		"SUB", 
    		"MUL", 
    		"MOD", 
    		"INC", 
    		"DEC", 
    		"SHL", 
    		"SHR", 
    		"SHU", 
    		"AND", 
    		"OR", 
    		"XOR", 
    		"NOT", 
    		"INV", 
    		"LAND", 
    		"LOR", 
    		"QUE", 
    		"COLON", 
    		"ASSIGN", 
    		"ADDASS", 
    		"SUBASS", 
    		"MULASS", 
    		"MODASS", 
    		"SHLASS", 
    		"SHRASS", 
    		"SHUASS", 
    		"ANDASS", 
    		"ORASS", 
    		"XORASS", 
    		"DIV", 
    		"DIVASS", 
    		"ARGS", 
    		"ARRAY", 
    		"BLOCK", 
    		"BYFIELD", 
    		"BYINDEX", 
    		"CALL", 
    		"CEXPR", 
    		"EXPR", 
    		"FORITER", 
    		"FORSTEP", 
    		"ITEM", 
    		"LABELLED", 
    		"NAMEDVALUE", 
    		"NEG", 
    		"OBJECT", 
    		"PAREXPR", 
    		"PDEC", 
    		"PINC", 
    		"POS", 
    		"BSLASH", 
    		"DQUOTE", 
    		"SQUOTE", 
    		"TAB", 
    		"VT", 
    		"FF", 
    		"SP", 
    		"NBSP", 
    		"USP", 
    		"WhiteSpace", 
    		"LF", 
    		"CR", 
    		"LS", 
    		"PS", 
    		"LineTerminator", 
    		"EOL", 
    		"MultiLineComment", 
    		"SingleLineComment", 
    		"Identifier", 
    		"StringLiteral", 
    		"HexDigit", 
    		"IdentifierStartASCII", 
    		"DecimalDigit", 
    		"IdentifierPart", 
    		"IdentifierNameASCIIStart", 
    		"RegularExpressionLiteral", 
    		"OctalDigit", 
    		"ExponentPart", 
    		"DecimalIntegerLiteral", 
    		"DecimalLiteral", 
    		"OctalIntegerLiteral", 
    		"HexIntegerLiteral", 
    		"CharacterEscapeSequence", 
    		"ZeroToThree", 
    		"OctalEscapeSequence", 
    		"HexEscapeSequence", 
    		"UnicodeEscapeSequence", 
    		"EscapeSequence", 
    		"BackslashSequence", 
    		"RegularExpressionFirstChar", 
    		"RegularExpressionChar"
        };

        public const int VT = 134;
        public const int LOR = 95;
        public const int FUNCTION = 17;
        public const int PACKAGE = 52;
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
        public const int DQUOTE = 131;
        public const int DO = 13;
        public const int NOT = 92;
        public const int DecimalDigit = 152;
        public const int BYFIELD = 114;
        public const int EOF = -1;
        public const int CEXPR = 117;
        public const int BREAK = 7;
        public const int Identifier = 148;
        public const int DIVASS = 110;
        public const int BYINDEX = 115;
        public const int FORSTEP = 120;
        public const int FINAL = 43;
        public const int RPAREN = 66;
        public const int INC = 84;
        public const int IMPORT = 47;
        public const int EOL = 145;
        public const int POS = 129;
        public const int OctalDigit = 156;
        public const int THIS = 24;
        public const int RETURN = 22;
        public const int ExponentPart = 157;
        public const int ARGS = 111;
        public const int DOUBLE = 39;
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
        public const int OctalIntegerLiteral = 160;
        public const int PROTECTED = 54;
        public const int DEC = 85;
        public const int CLASS = 36;
        public const int LBRACK = 67;
        public const int HexEscapeSequence = 165;
        public const int ORASS = 107;
        public const int SingleLineComment = 147;
        public const int NAMEDVALUE = 123;
        public const int LBRACE = 63;
        public const int GTE = 75;
        public const int FOR = 16;
        public const int RegularExpressionLiteral = 155;
        public const int SUB = 81;
        public const int FLOAT = 44;
        public const int ABSTRACT = 32;
        public const int AND = 89;
        public const int DecimalIntegerLiteral = 158;
        public const int HexDigit = 150;
        public const int LTE = 74;
        public const int LPAREN = 65;
        public const int IF = 18;
        public const int SUBASS = 100;
        public const int EXPR = 118;
        public const int BOOLEAN = 33;
        public const int SYNCHRONIZED = 59;
        public const int IN = 19;
        public const int IMPLEMENTS = 46;
        public const int OBJECT = 125;
        public const int CONTINUE = 10;
        public const int COMMA = 71;
        public const int FORITER = 119;
        public const int TRANSIENT = 61;
        public const int SHRASS = 104;
        public const int MODASS = 102;
        public const int PS = 143;
        public const int DOT = 69;
        public const int IdentifierPart = 153;
        public const int MultiLineComment = 146;
        public const int WITH = 31;
        public const int ADD = 80;
        public const int BYTE = 34;
        public const int XOR = 91;
        public const int ZeroToThree = 163;
        public const int ITEM = 121;
        public const int VOLATILE = 62;
        public const int UnicodeEscapeSequence = 166;
        public const int SHUASS = 105;
        public const int DEFAULT = 11;
        public const int NSAME = 79;
        public const int TAB = 133;
        public const int SHORT = 56;
        public const int INSTANCEOF = 20;
        public const int SQUOTE = 132;
        public const int DecimalLiteral = 159;
        public const int TRUE = 5;
        public const int SAME = 78;
        public const int StringLiteral = 149;
        public const int COLON = 97;
        public const int PAREXPR = 126;
        public const int NEQ = 77;
        public const int ENUM = 40;
        public const int FINALLY = 15;
        public const int HexIntegerLiteral = 161;
        public const int NBSP = 137;
        public const int SP = 136;
        public const int BLOCK = 113;
        public const int LineTerminator = 144;
        public const int NEG = 124;
        public const int ASSIGN = 98;
        public const int INTERFACE = 49;
        public const int DIV = 109;
        public const int SEMIC = 70;
        public const int CR = 141;
        public const int LONG = 50;
        public const int EXTENDS = 42;
        public const int PUBLIC = 55;
        public const int BSLASH = 130;
        public const int LF = 140;

        // delegates
        // delegators



        public ES3Parser(ITokenStream input)
            : this(input, new RecognizerSharedState())
        {
        }

        public ES3Parser(ITokenStream input, RecognizerSharedState state)
            : base(input, state)
        {
            InitializeCyclicDFAs();


        }

        protected ITreeAdaptor adaptor = new CommonTreeAdaptor();

        public ITreeAdaptor TreeAdaptor
        {
            get { return this.adaptor; }
            set
            {
                this.adaptor = value;
            }
        }

        override public string[] TokenNames
        {
            get { return ES3Parser.tokenNames; }
        }

        override public string GrammarFileName
        {
            get { return "ES3.g3"; }
        }


        public class token_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "token"
        // ES3.g3:302:1: token : ( reservedWord | Identifier | punctuator | numericLiteral | StringLiteral );
        public ES3Parser.token_return token() // throws RecognitionException [1]
        {
            ES3Parser.token_return retval = new ES3Parser.token_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken Identifier2 = null;
            IToken StringLiteral5 = null;
            ES3Parser.reservedWord_return reservedWord1 = default(ES3Parser.reservedWord_return);

            ES3Parser.punctuator_return punctuator3 = default(ES3Parser.punctuator_return);

            ES3Parser.numericLiteral_return numericLiteral4 = default(ES3Parser.numericLiteral_return);


            object Identifier2_tree = null;
            object StringLiteral5_tree = null;

            try
            {
                // ES3.g3:303:2: ( reservedWord | Identifier | punctuator | numericLiteral | StringLiteral )
                int alt1 = 5;
                switch (input.LA(1))
                {
                    case NULL:
                    case TRUE:
                    case FALSE:
                    case BREAK:
                    case CASE:
                    case CATCH:
                    case CONTINUE:
                    case DEFAULT:
                    case DELETE:
                    case DO:
                    case ELSE:
                    case FINALLY:
                    case FOR:
                    case FUNCTION:
                    case IF:
                    case IN:
                    case INSTANCEOF:
                    case NEW:
                    case RETURN:
                    case SWITCH:
                    case THIS:
                    case THROW:
                    case TRY:
                    case TYPEOF:
                    case VAR:
                    case VOID:
                    case WHILE:
                    case WITH:
                    case ABSTRACT:
                    case BOOLEAN:
                    case BYTE:
                    case CHAR:
                    case CLASS:
                    case CONST:
                    case DEBUGGER:
                    case DOUBLE:
                    case ENUM:
                    case EXPORT:
                    case EXTENDS:
                    case FINAL:
                    case FLOAT:
                    case GOTO:
                    case IMPLEMENTS:
                    case IMPORT:
                    case INT:
                    case INTERFACE:
                    case LONG:
                    case NATIVE:
                    case PACKAGE:
                    case PRIVATE:
                    case PROTECTED:
                    case PUBLIC:
                    case SHORT:
                    case STATIC:
                    case SUPER:
                    case SYNCHRONIZED:
                    case THROWS:
                    case TRANSIENT:
                    case VOLATILE:
                        {
                            alt1 = 1;
                        }
                        break;
                    case Identifier:
                        {
                            alt1 = 2;
                        }
                        break;
                    case LBRACE:
                    case RBRACE:
                    case LPAREN:
                    case RPAREN:
                    case LBRACK:
                    case RBRACK:
                    case DOT:
                    case SEMIC:
                    case COMMA:
                    case LT:
                    case GT:
                    case LTE:
                    case GTE:
                    case EQ:
                    case NEQ:
                    case SAME:
                    case NSAME:
                    case ADD:
                    case SUB:
                    case MUL:
                    case MOD:
                    case INC:
                    case DEC:
                    case SHL:
                    case SHR:
                    case SHU:
                    case AND:
                    case OR:
                    case XOR:
                    case NOT:
                    case INV:
                    case LAND:
                    case LOR:
                    case QUE:
                    case COLON:
                    case ASSIGN:
                    case ADDASS:
                    case SUBASS:
                    case MULASS:
                    case MODASS:
                    case SHLASS:
                    case SHRASS:
                    case SHUASS:
                    case ANDASS:
                    case ORASS:
                    case XORASS:
                    case DIV:
                    case DIVASS:
                        {
                            alt1 = 3;
                        }
                        break;
                    case DecimalLiteral:
                    case OctalIntegerLiteral:
                    case HexIntegerLiteral:
                        {
                            alt1 = 4;
                        }
                        break;
                    case StringLiteral:
                        {
                            alt1 = 5;
                        }
                        break;
                    default:
                        NoViableAltException nvae_d1s0 =
                            new NoViableAltException("", 1, 0, input);

                        throw nvae_d1s0;
                }

                switch (alt1)
                {
                    case 1:
                        // ES3.g3:303:4: reservedWord
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_reservedWord_in_token1753);
                            reservedWord1 = reservedWord();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, reservedWord1.Tree);

                        }
                        break;
                    case 2:
                        // ES3.g3:304:4: Identifier
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            Identifier2 = (IToken)Match(input, Identifier, FOLLOW_Identifier_in_token1758);
                            Identifier2_tree = (object)adaptor.Create(Identifier2);
                            adaptor.AddChild(root_0, Identifier2_tree);


                        }
                        break;
                    case 3:
                        // ES3.g3:305:4: punctuator
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_punctuator_in_token1763);
                            punctuator3 = punctuator();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, punctuator3.Tree);

                        }
                        break;
                    case 4:
                        // ES3.g3:306:4: numericLiteral
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_numericLiteral_in_token1768);
                            numericLiteral4 = numericLiteral();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, numericLiteral4.Tree);

                        }
                        break;
                    case 5:
                        // ES3.g3:307:4: StringLiteral
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            StringLiteral5 = (IToken)Match(input, StringLiteral, FOLLOW_StringLiteral_in_token1773);
                            StringLiteral5_tree = (object)adaptor.Create(StringLiteral5);
                            adaptor.AddChild(root_0, StringLiteral5_tree);


                        }
                        break;

                }
                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "token"

        public class reservedWord_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "reservedWord"
        // ES3.g3:312:1: reservedWord : ( keyword | futureReservedWord | NULL | booleanLiteral );
        public ES3Parser.reservedWord_return reservedWord() // throws RecognitionException [1]
        {
            ES3Parser.reservedWord_return retval = new ES3Parser.reservedWord_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken NULL8 = null;
            ES3Parser.keyword_return keyword6 = default(ES3Parser.keyword_return);

            ES3Parser.futureReservedWord_return futureReservedWord7 = default(ES3Parser.futureReservedWord_return);

            ES3Parser.booleanLiteral_return booleanLiteral9 = default(ES3Parser.booleanLiteral_return);


            object NULL8_tree = null;

            try
            {
                // ES3.g3:313:2: ( keyword | futureReservedWord | NULL | booleanLiteral )
                int alt2 = 4;
                switch (input.LA(1))
                {
                    case BREAK:
                    case CASE:
                    case CATCH:
                    case CONTINUE:
                    case DEFAULT:
                    case DELETE:
                    case DO:
                    case ELSE:
                    case FINALLY:
                    case FOR:
                    case FUNCTION:
                    case IF:
                    case IN:
                    case INSTANCEOF:
                    case NEW:
                    case RETURN:
                    case SWITCH:
                    case THIS:
                    case THROW:
                    case TRY:
                    case TYPEOF:
                    case VAR:
                    case VOID:
                    case WHILE:
                    case WITH:
                        {
                            alt2 = 1;
                        }
                        break;
                    case ABSTRACT:
                    case BOOLEAN:
                    case BYTE:
                    case CHAR:
                    case CLASS:
                    case CONST:
                    case DEBUGGER:
                    case DOUBLE:
                    case ENUM:
                    case EXPORT:
                    case EXTENDS:
                    case FINAL:
                    case FLOAT:
                    case GOTO:
                    case IMPLEMENTS:
                    case IMPORT:
                    case INT:
                    case INTERFACE:
                    case LONG:
                    case NATIVE:
                    case PACKAGE:
                    case PRIVATE:
                    case PROTECTED:
                    case PUBLIC:
                    case SHORT:
                    case STATIC:
                    case SUPER:
                    case SYNCHRONIZED:
                    case THROWS:
                    case TRANSIENT:
                    case VOLATILE:
                        {
                            alt2 = 2;
                        }
                        break;
                    case NULL:
                        {
                            alt2 = 3;
                        }
                        break;
                    case TRUE:
                    case FALSE:
                        {
                            alt2 = 4;
                        }
                        break;
                    default:
                        NoViableAltException nvae_d2s0 =
                            new NoViableAltException("", 2, 0, input);

                        throw nvae_d2s0;
                }

                switch (alt2)
                {
                    case 1:
                        // ES3.g3:313:4: keyword
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_keyword_in_reservedWord1786);
                            keyword6 = keyword();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, keyword6.Tree);

                        }
                        break;
                    case 2:
                        // ES3.g3:314:4: futureReservedWord
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_futureReservedWord_in_reservedWord1791);
                            futureReservedWord7 = futureReservedWord();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, futureReservedWord7.Tree);

                        }
                        break;
                    case 3:
                        // ES3.g3:315:4: NULL
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            NULL8 = (IToken)Match(input, NULL, FOLLOW_NULL_in_reservedWord1796);
                            NULL8_tree = (object)adaptor.Create(NULL8);
                            adaptor.AddChild(root_0, NULL8_tree);


                        }
                        break;
                    case 4:
                        // ES3.g3:316:4: booleanLiteral
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_booleanLiteral_in_reservedWord1801);
                            booleanLiteral9 = booleanLiteral();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, booleanLiteral9.Tree);

                        }
                        break;

                }
                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "reservedWord"

        public class keyword_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "keyword"
        // ES3.g3:323:1: keyword : ( BREAK | CASE | CATCH | CONTINUE | DEFAULT | DELETE | DO | ELSE | FINALLY | FOR | FUNCTION | IF | IN | INSTANCEOF | NEW | RETURN | SWITCH | THIS | THROW | TRY | TYPEOF | VAR | VOID | WHILE | WITH );
        public ES3Parser.keyword_return keyword() // throws RecognitionException [1]
        {
            ES3Parser.keyword_return retval = new ES3Parser.keyword_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken set10 = null;

            object set10_tree = null;

            try
            {
                // ES3.g3:324:2: ( BREAK | CASE | CATCH | CONTINUE | DEFAULT | DELETE | DO | ELSE | FINALLY | FOR | FUNCTION | IF | IN | INSTANCEOF | NEW | RETURN | SWITCH | THIS | THROW | TRY | TYPEOF | VAR | VOID | WHILE | WITH )
                // ES3.g3:
                {
                    root_0 = (object)adaptor.GetNilNode();

                    set10 = (IToken)input.LT(1);
                    if ((input.LA(1) >= BREAK && input.LA(1) <= WITH))
                    {
                        input.Consume();
                        adaptor.AddChild(root_0, (object)adaptor.Create(set10));
                        state.errorRecovery = false;
                    }
                    else
                    {
                        MismatchedSetException mse = new MismatchedSetException(null, input);
                        throw mse;
                    }


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "keyword"

        public class futureReservedWord_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "futureReservedWord"
        // ES3.g3:355:1: futureReservedWord : ( ABSTRACT | BOOLEAN | BYTE | CHAR | CLASS | CONST | DEBUGGER | DOUBLE | ENUM | EXPORT | EXTENDS | FINAL | FLOAT | GOTO | IMPLEMENTS | IMPORT | INT | INTERFACE | LONG | NATIVE | PACKAGE | PRIVATE | PROTECTED | PUBLIC | SHORT | STATIC | SUPER | SYNCHRONIZED | THROWS | TRANSIENT | VOLATILE );
        public ES3Parser.futureReservedWord_return futureReservedWord() // throws RecognitionException [1]
        {
            ES3Parser.futureReservedWord_return retval = new ES3Parser.futureReservedWord_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken set11 = null;

            object set11_tree = null;

            try
            {
                // ES3.g3:356:2: ( ABSTRACT | BOOLEAN | BYTE | CHAR | CLASS | CONST | DEBUGGER | DOUBLE | ENUM | EXPORT | EXTENDS | FINAL | FLOAT | GOTO | IMPLEMENTS | IMPORT | INT | INTERFACE | LONG | NATIVE | PACKAGE | PRIVATE | PROTECTED | PUBLIC | SHORT | STATIC | SUPER | SYNCHRONIZED | THROWS | TRANSIENT | VOLATILE )
                // ES3.g3:
                {
                    root_0 = (object)adaptor.GetNilNode();

                    set11 = (IToken)input.LT(1);
                    if ((input.LA(1) >= ABSTRACT && input.LA(1) <= VOLATILE))
                    {
                        input.Consume();
                        adaptor.AddChild(root_0, (object)adaptor.Create(set11));
                        state.errorRecovery = false;
                    }
                    else
                    {
                        MismatchedSetException mse = new MismatchedSetException(null, input);
                        throw mse;
                    }


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "futureReservedWord"

        public class punctuator_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "punctuator"
        // ES3.g3:433:1: punctuator : ( LBRACE | RBRACE | LPAREN | RPAREN | LBRACK | RBRACK | DOT | SEMIC | COMMA | LT | GT | LTE | GTE | EQ | NEQ | SAME | NSAME | ADD | SUB | MUL | MOD | INC | DEC | SHL | SHR | SHU | AND | OR | XOR | NOT | INV | LAND | LOR | QUE | COLON | ASSIGN | ADDASS | SUBASS | MULASS | MODASS | SHLASS | SHRASS | SHUASS | ANDASS | ORASS | XORASS | DIV | DIVASS );
        public ES3Parser.punctuator_return punctuator() // throws RecognitionException [1]
        {
            ES3Parser.punctuator_return retval = new ES3Parser.punctuator_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken set12 = null;

            object set12_tree = null;

            try
            {
                // ES3.g3:434:2: ( LBRACE | RBRACE | LPAREN | RPAREN | LBRACK | RBRACK | DOT | SEMIC | COMMA | LT | GT | LTE | GTE | EQ | NEQ | SAME | NSAME | ADD | SUB | MUL | MOD | INC | DEC | SHL | SHR | SHU | AND | OR | XOR | NOT | INV | LAND | LOR | QUE | COLON | ASSIGN | ADDASS | SUBASS | MULASS | MODASS | SHLASS | SHRASS | SHUASS | ANDASS | ORASS | XORASS | DIV | DIVASS )
                // ES3.g3:
                {
                    root_0 = (object)adaptor.GetNilNode();

                    set12 = (IToken)input.LT(1);
                    if ((input.LA(1) >= LBRACE && input.LA(1) <= DIVASS))
                    {
                        input.Consume();
                        adaptor.AddChild(root_0, (object)adaptor.Create(set12));
                        state.errorRecovery = false;
                    }
                    else
                    {
                        MismatchedSetException mse = new MismatchedSetException(null, input);
                        throw mse;
                    }


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "punctuator"

        public class literal_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "literal"
        // ES3.g3:488:1: literal : ( NULL | booleanLiteral | numericLiteral | StringLiteral | RegularExpressionLiteral );
        public ES3Parser.literal_return literal() // throws RecognitionException [1]
        {
            ES3Parser.literal_return retval = new ES3Parser.literal_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken NULL13 = null;
            IToken StringLiteral16 = null;
            IToken RegularExpressionLiteral17 = null;
            ES3Parser.booleanLiteral_return booleanLiteral14 = default(ES3Parser.booleanLiteral_return);

            ES3Parser.numericLiteral_return numericLiteral15 = default(ES3Parser.numericLiteral_return);


            object NULL13_tree = null;
            object StringLiteral16_tree = null;
            object RegularExpressionLiteral17_tree = null;

            try
            {
                // ES3.g3:489:2: ( NULL | booleanLiteral | numericLiteral | StringLiteral | RegularExpressionLiteral )
                int alt3 = 5;
                switch (input.LA(1))
                {
                    case NULL:
                        {
                            alt3 = 1;
                        }
                        break;
                    case TRUE:
                    case FALSE:
                        {
                            alt3 = 2;
                        }
                        break;
                    case DecimalLiteral:
                    case OctalIntegerLiteral:
                    case HexIntegerLiteral:
                        {
                            alt3 = 3;
                        }
                        break;
                    case StringLiteral:
                        {
                            alt3 = 4;
                        }
                        break;
                    case RegularExpressionLiteral:
                        {
                            alt3 = 5;
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
                        // ES3.g3:489:4: NULL
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            NULL13 = (IToken)Match(input, NULL, FOLLOW_NULL_in_literal2482);
                            NULL13_tree = (object)adaptor.Create(NULL13);
                            adaptor.AddChild(root_0, NULL13_tree);


                        }
                        break;
                    case 2:
                        // ES3.g3:490:4: booleanLiteral
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_booleanLiteral_in_literal2487);
                            booleanLiteral14 = booleanLiteral();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, booleanLiteral14.Tree);

                        }
                        break;
                    case 3:
                        // ES3.g3:491:4: numericLiteral
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_numericLiteral_in_literal2492);
                            numericLiteral15 = numericLiteral();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, numericLiteral15.Tree);

                        }
                        break;
                    case 4:
                        // ES3.g3:492:4: StringLiteral
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            StringLiteral16 = (IToken)Match(input, StringLiteral, FOLLOW_StringLiteral_in_literal2497);
                            StringLiteral16_tree = (object)adaptor.Create(StringLiteral16);
                            adaptor.AddChild(root_0, StringLiteral16_tree);


                        }
                        break;
                    case 5:
                        // ES3.g3:493:4: RegularExpressionLiteral
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            RegularExpressionLiteral17 = (IToken)Match(input, RegularExpressionLiteral, FOLLOW_RegularExpressionLiteral_in_literal2502);
                            RegularExpressionLiteral17_tree = (object)adaptor.Create(RegularExpressionLiteral17);
                            adaptor.AddChild(root_0, RegularExpressionLiteral17_tree);


                        }
                        break;

                }
                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "literal"

        public class booleanLiteral_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "booleanLiteral"
        // ES3.g3:496:1: booleanLiteral : ( TRUE | FALSE );
        public ES3Parser.booleanLiteral_return booleanLiteral() // throws RecognitionException [1]
        {
            ES3Parser.booleanLiteral_return retval = new ES3Parser.booleanLiteral_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken set18 = null;

            object set18_tree = null;

            try
            {
                // ES3.g3:497:2: ( TRUE | FALSE )
                // ES3.g3:
                {
                    root_0 = (object)adaptor.GetNilNode();

                    set18 = (IToken)input.LT(1);
                    if ((input.LA(1) >= TRUE && input.LA(1) <= FALSE))
                    {
                        input.Consume();
                        adaptor.AddChild(root_0, (object)adaptor.Create(set18));
                        state.errorRecovery = false;
                    }
                    else
                    {
                        MismatchedSetException mse = new MismatchedSetException(null, input);
                        throw mse;
                    }


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "booleanLiteral"

        public class numericLiteral_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "numericLiteral"
        // ES3.g3:543:1: numericLiteral : ( DecimalLiteral | OctalIntegerLiteral | HexIntegerLiteral );
        public ES3Parser.numericLiteral_return numericLiteral() // throws RecognitionException [1]
        {
            ES3Parser.numericLiteral_return retval = new ES3Parser.numericLiteral_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken set19 = null;

            object set19_tree = null;

            try
            {
                // ES3.g3:544:2: ( DecimalLiteral | OctalIntegerLiteral | HexIntegerLiteral )
                // ES3.g3:
                {
                    root_0 = (object)adaptor.GetNilNode();

                    set19 = (IToken)input.LT(1);
                    if ((input.LA(1) >= DecimalLiteral && input.LA(1) <= HexIntegerLiteral))
                    {
                        input.Consume();
                        adaptor.AddChild(root_0, (object)adaptor.Create(set19));
                        state.errorRecovery = false;
                    }
                    else
                    {
                        MismatchedSetException mse = new MismatchedSetException(null, input);
                        throw mse;
                    }


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "numericLiteral"

        public class primaryExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "primaryExpression"
        // ES3.g3:631:1: primaryExpression : ( THIS | Identifier | literal | arrayLiteral | objectLiteral | lpar= LPAREN expression RPAREN -> ^( PAREXPR[$lpar, \"PAREXPR\"] expression ) );
        public ES3Parser.primaryExpression_return primaryExpression() // throws RecognitionException [1]
        {
            ES3Parser.primaryExpression_return retval = new ES3Parser.primaryExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken lpar = null;
            IToken THIS20 = null;
            IToken Identifier21 = null;
            IToken RPAREN26 = null;
            ES3Parser.literal_return literal22 = default(ES3Parser.literal_return);

            ES3Parser.arrayLiteral_return arrayLiteral23 = default(ES3Parser.arrayLiteral_return);

            ES3Parser.objectLiteral_return objectLiteral24 = default(ES3Parser.objectLiteral_return);

            ES3Parser.expression_return expression25 = default(ES3Parser.expression_return);


            object lpar_tree = null;
            object THIS20_tree = null;
            object Identifier21_tree = null;
            object RPAREN26_tree = null;
            RewriteRuleTokenStream stream_RPAREN = new RewriteRuleTokenStream(adaptor, "token RPAREN");
            RewriteRuleTokenStream stream_LPAREN = new RewriteRuleTokenStream(adaptor, "token LPAREN");
            RewriteRuleSubtreeStream stream_expression = new RewriteRuleSubtreeStream(adaptor, "rule expression");
            try
            {
                // ES3.g3:632:2: ( THIS | Identifier | literal | arrayLiteral | objectLiteral | lpar= LPAREN expression RPAREN -> ^( PAREXPR[$lpar, \"PAREXPR\"] expression ) )
                int alt4 = 6;
                switch (input.LA(1))
                {
                    case THIS:
                        {
                            alt4 = 1;
                        }
                        break;
                    case Identifier:
                        {
                            alt4 = 2;
                        }
                        break;
                    case NULL:
                    case TRUE:
                    case FALSE:
                    case StringLiteral:
                    case RegularExpressionLiteral:
                    case DecimalLiteral:
                    case OctalIntegerLiteral:
                    case HexIntegerLiteral:
                        {
                            alt4 = 3;
                        }
                        break;
                    case LBRACK:
                        {
                            alt4 = 4;
                        }
                        break;
                    case LBRACE:
                        {
                            alt4 = 5;
                        }
                        break;
                    case LPAREN:
                        {
                            alt4 = 6;
                        }
                        break;
                    default:
                        NoViableAltException nvae_d4s0 =
                            new NoViableAltException("", 4, 0, input);

                        throw nvae_d4s0;
                }

                switch (alt4)
                {
                    case 1:
                        // ES3.g3:632:4: THIS
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            THIS20 = (IToken)Match(input, THIS, FOLLOW_THIS_in_primaryExpression3115);
                            THIS20_tree = (object)adaptor.Create(THIS20);
                            adaptor.AddChild(root_0, THIS20_tree);


                        }
                        break;
                    case 2:
                        // ES3.g3:633:4: Identifier
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            Identifier21 = (IToken)Match(input, Identifier, FOLLOW_Identifier_in_primaryExpression3120);
                            Identifier21_tree = (object)adaptor.Create(Identifier21);
                            adaptor.AddChild(root_0, Identifier21_tree);


                        }
                        break;
                    case 3:
                        // ES3.g3:634:4: literal
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_literal_in_primaryExpression3125);
                            literal22 = literal();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, literal22.Tree);

                        }
                        break;
                    case 4:
                        // ES3.g3:635:4: arrayLiteral
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_arrayLiteral_in_primaryExpression3130);
                            arrayLiteral23 = arrayLiteral();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, arrayLiteral23.Tree);

                        }
                        break;
                    case 5:
                        // ES3.g3:636:4: objectLiteral
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_objectLiteral_in_primaryExpression3135);
                            objectLiteral24 = objectLiteral();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, objectLiteral24.Tree);

                        }
                        break;
                    case 6:
                        // ES3.g3:637:4: lpar= LPAREN expression RPAREN
                        {
                            lpar = (IToken)Match(input, LPAREN, FOLLOW_LPAREN_in_primaryExpression3142);
                            stream_LPAREN.Add(lpar);

                            PushFollow(FOLLOW_expression_in_primaryExpression3144);
                            expression25 = expression();
                            state.followingStackPointer--;

                            stream_expression.Add(expression25.Tree);
                            RPAREN26 = (IToken)Match(input, RPAREN, FOLLOW_RPAREN_in_primaryExpression3146);
                            stream_RPAREN.Add(RPAREN26);



                            // AST REWRITE
                            // elements:          expression
                            // token labels:      
                            // rule labels:       retval
                            // token list labels: 
                            // rule list labels:  
                            // wildcard labels: 
                            retval.Tree = root_0;
                            RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                            root_0 = (object)adaptor.GetNilNode();
                            // 637:34: -> ^( PAREXPR[$lpar, \"PAREXPR\"] expression )
                            {
                                // ES3.g3:637:37: ^( PAREXPR[$lpar, \"PAREXPR\"] expression )
                                {
                                    object root_1 = (object)adaptor.GetNilNode();
                                    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(PAREXPR, lpar, "PAREXPR"), root_1);

                                    adaptor.AddChild(root_1, stream_expression.NextTree());

                                    adaptor.AddChild(root_0, root_1);
                                }

                            }

                            retval.Tree = root_0; retval.Tree = root_0;
                        }
                        break;

                }
                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "primaryExpression"

        public class arrayLiteral_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "arrayLiteral"
        // ES3.g3:640:1: arrayLiteral : lb= LBRACK ( arrayItem ( COMMA arrayItem )* )? RBRACK -> ^( ARRAY[$lb, \"ARRAY\"] ( arrayItem )* ) ;
        public ES3Parser.arrayLiteral_return arrayLiteral() // throws RecognitionException [1]
        {
            ES3Parser.arrayLiteral_return retval = new ES3Parser.arrayLiteral_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken lb = null;
            IToken COMMA28 = null;
            IToken RBRACK30 = null;
            ES3Parser.arrayItem_return arrayItem27 = default(ES3Parser.arrayItem_return);

            ES3Parser.arrayItem_return arrayItem29 = default(ES3Parser.arrayItem_return);


            object lb_tree = null;
            object COMMA28_tree = null;
            object RBRACK30_tree = null;
            RewriteRuleTokenStream stream_RBRACK = new RewriteRuleTokenStream(adaptor, "token RBRACK");
            RewriteRuleTokenStream stream_LBRACK = new RewriteRuleTokenStream(adaptor, "token LBRACK");
            RewriteRuleTokenStream stream_COMMA = new RewriteRuleTokenStream(adaptor, "token COMMA");
            RewriteRuleSubtreeStream stream_arrayItem = new RewriteRuleSubtreeStream(adaptor, "rule arrayItem");
            try
            {
                // ES3.g3:641:2: (lb= LBRACK ( arrayItem ( COMMA arrayItem )* )? RBRACK -> ^( ARRAY[$lb, \"ARRAY\"] ( arrayItem )* ) )
                // ES3.g3:641:4: lb= LBRACK ( arrayItem ( COMMA arrayItem )* )? RBRACK
                {
                    lb = (IToken)Match(input, LBRACK, FOLLOW_LBRACK_in_arrayLiteral3170);
                    stream_LBRACK.Add(lb);

                    // ES3.g3:641:14: ( arrayItem ( COMMA arrayItem )* )?
                    int alt6 = 2;
                    int LA6_0 = input.LA(1);

                    if (((LA6_0 >= NULL && LA6_0 <= FALSE) || LA6_0 == DELETE || LA6_0 == FUNCTION || LA6_0 == NEW || LA6_0 == THIS || LA6_0 == TYPEOF || LA6_0 == VOID || LA6_0 == LBRACE || LA6_0 == LPAREN || LA6_0 == LBRACK || LA6_0 == COMMA || (LA6_0 >= ADD && LA6_0 <= SUB) || (LA6_0 >= INC && LA6_0 <= DEC) || (LA6_0 >= NOT && LA6_0 <= INV) || (LA6_0 >= Identifier && LA6_0 <= StringLiteral) || LA6_0 == RegularExpressionLiteral || (LA6_0 >= DecimalLiteral && LA6_0 <= HexIntegerLiteral)))
                    {
                        alt6 = 1;
                    }
                    else if ((LA6_0 == RBRACK))
                    {
                        int LA6_2 = input.LA(2);

                        if (((input.LA(1) == COMMA)))
                        {
                            alt6 = 1;
                        }
                    }
                    switch (alt6)
                    {
                        case 1:
                            // ES3.g3:641:16: arrayItem ( COMMA arrayItem )*
                            {
                                PushFollow(FOLLOW_arrayItem_in_arrayLiteral3174);
                                arrayItem27 = arrayItem();
                                state.followingStackPointer--;

                                stream_arrayItem.Add(arrayItem27.Tree);
                                // ES3.g3:641:26: ( COMMA arrayItem )*
                                do
                                {
                                    int alt5 = 2;
                                    int LA5_0 = input.LA(1);

                                    if ((LA5_0 == COMMA))
                                    {
                                        alt5 = 1;
                                    }


                                    switch (alt5)
                                    {
                                        case 1:
                                            // ES3.g3:641:28: COMMA arrayItem
                                            {
                                                COMMA28 = (IToken)Match(input, COMMA, FOLLOW_COMMA_in_arrayLiteral3178);
                                                stream_COMMA.Add(COMMA28);

                                                PushFollow(FOLLOW_arrayItem_in_arrayLiteral3180);
                                                arrayItem29 = arrayItem();
                                                state.followingStackPointer--;

                                                stream_arrayItem.Add(arrayItem29.Tree);

                                            }
                                            break;

                                        default:
                                            goto loop5;
                                    }
                                } while (true);

                            loop5:
                                ;	// Stops C# compiler whining that label 'loop5' has no statements


                            }
                            break;

                    }

                    RBRACK30 = (IToken)Match(input, RBRACK, FOLLOW_RBRACK_in_arrayLiteral3188);
                    stream_RBRACK.Add(RBRACK30);



                    // AST REWRITE
                    // elements:          arrayItem
                    // token labels:      
                    // rule labels:       retval
                    // token list labels: 
                    // rule list labels:  
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                    root_0 = (object)adaptor.GetNilNode();
                    // 642:2: -> ^( ARRAY[$lb, \"ARRAY\"] ( arrayItem )* )
                    {
                        // ES3.g3:642:5: ^( ARRAY[$lb, \"ARRAY\"] ( arrayItem )* )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(ARRAY, lb, "ARRAY"), root_1);

                            // ES3.g3:642:28: ( arrayItem )*
                            while (stream_arrayItem.HasNext())
                            {
                                adaptor.AddChild(root_1, stream_arrayItem.NextTree());

                            }
                            stream_arrayItem.Reset();

                            adaptor.AddChild(root_0, root_1);
                        }

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "arrayLiteral"

        public class arrayItem_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "arrayItem"
        // ES3.g3:645:1: arrayItem : (expr= assignmentExpression | {...}?) -> ^( ITEM ( $expr)? ) ;
        public ES3Parser.arrayItem_return arrayItem() // throws RecognitionException [1]
        {
            ES3Parser.arrayItem_return retval = new ES3Parser.arrayItem_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            ES3Parser.assignmentExpression_return expr = default(ES3Parser.assignmentExpression_return);


            RewriteRuleSubtreeStream stream_assignmentExpression = new RewriteRuleSubtreeStream(adaptor, "rule assignmentExpression");
            try
            {
                // ES3.g3:646:2: ( (expr= assignmentExpression | {...}?) -> ^( ITEM ( $expr)? ) )
                // ES3.g3:646:4: (expr= assignmentExpression | {...}?)
                {
                    // ES3.g3:646:4: (expr= assignmentExpression | {...}?)
                    int alt7 = 2;
                    int LA7_0 = input.LA(1);

                    if (((LA7_0 >= NULL && LA7_0 <= FALSE) || LA7_0 == DELETE || LA7_0 == FUNCTION || LA7_0 == NEW || LA7_0 == THIS || LA7_0 == TYPEOF || LA7_0 == VOID || LA7_0 == LBRACE || LA7_0 == LPAREN || LA7_0 == LBRACK || (LA7_0 >= ADD && LA7_0 <= SUB) || (LA7_0 >= INC && LA7_0 <= DEC) || (LA7_0 >= NOT && LA7_0 <= INV) || (LA7_0 >= Identifier && LA7_0 <= StringLiteral) || LA7_0 == RegularExpressionLiteral || (LA7_0 >= DecimalLiteral && LA7_0 <= HexIntegerLiteral)))
                    {
                        alt7 = 1;
                    }
                    else if ((LA7_0 == RBRACK || LA7_0 == COMMA))
                    {
                        alt7 = 2;
                    }
                    else
                    {
                        NoViableAltException nvae_d7s0 =
                            new NoViableAltException("", 7, 0, input);

                        throw nvae_d7s0;
                    }
                    switch (alt7)
                    {
                        case 1:
                            // ES3.g3:646:6: expr= assignmentExpression
                            {
                                PushFollow(FOLLOW_assignmentExpression_in_arrayItem3216);
                                expr = assignmentExpression();
                                state.followingStackPointer--;

                                stream_assignmentExpression.Add(expr.Tree);

                            }
                            break;
                        case 2:
                            // ES3.g3:646:34: {...}?
                            {
                                if (!((input.LA(1) == COMMA)))
                                {
                                    throw new FailedPredicateException(input, "arrayItem", " input.LA(1) == COMMA ");
                                }

                            }
                            break;

                    }



                    // AST REWRITE
                    // elements:          expr
                    // token labels:      
                    // rule labels:       retval, expr
                    // token list labels: 
                    // rule list labels:  
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);
                    RewriteRuleSubtreeStream stream_expr = new RewriteRuleSubtreeStream(adaptor, "rule expr", expr != null ? expr.Tree : null);

                    root_0 = (object)adaptor.GetNilNode();
                    // 647:2: -> ^( ITEM ( $expr)? )
                    {
                        // ES3.g3:647:5: ^( ITEM ( $expr)? )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(ITEM, "ITEM"), root_1);

                            // ES3.g3:647:13: ( $expr)?
                            if (stream_expr.HasNext())
                            {
                                adaptor.AddChild(root_1, stream_expr.NextTree());

                            }
                            stream_expr.Reset();

                            adaptor.AddChild(root_0, root_1);
                        }

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "arrayItem"

        public class objectLiteral_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "objectLiteral"
        // ES3.g3:650:1: objectLiteral : lb= LBRACE ( nameValuePair ( COMMA nameValuePair )* )? RBRACE -> ^( OBJECT[$lb, \"OBJECT\"] ( nameValuePair )* ) ;
        public ES3Parser.objectLiteral_return objectLiteral() // throws RecognitionException [1]
        {
            ES3Parser.objectLiteral_return retval = new ES3Parser.objectLiteral_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken lb = null;
            IToken COMMA32 = null;
            IToken RBRACE34 = null;
            ES3Parser.nameValuePair_return nameValuePair31 = default(ES3Parser.nameValuePair_return);

            ES3Parser.nameValuePair_return nameValuePair33 = default(ES3Parser.nameValuePair_return);


            object lb_tree = null;
            object COMMA32_tree = null;
            object RBRACE34_tree = null;
            RewriteRuleTokenStream stream_RBRACE = new RewriteRuleTokenStream(adaptor, "token RBRACE");
            RewriteRuleTokenStream stream_COMMA = new RewriteRuleTokenStream(adaptor, "token COMMA");
            RewriteRuleTokenStream stream_LBRACE = new RewriteRuleTokenStream(adaptor, "token LBRACE");
            RewriteRuleSubtreeStream stream_nameValuePair = new RewriteRuleSubtreeStream(adaptor, "rule nameValuePair");
            try
            {
                // ES3.g3:651:2: (lb= LBRACE ( nameValuePair ( COMMA nameValuePair )* )? RBRACE -> ^( OBJECT[$lb, \"OBJECT\"] ( nameValuePair )* ) )
                // ES3.g3:651:4: lb= LBRACE ( nameValuePair ( COMMA nameValuePair )* )? RBRACE
                {
                    lb = (IToken)Match(input, LBRACE, FOLLOW_LBRACE_in_objectLiteral3248);
                    stream_LBRACE.Add(lb);

                    // ES3.g3:651:14: ( nameValuePair ( COMMA nameValuePair )* )?
                    int alt9 = 2;
                    int LA9_0 = input.LA(1);

                    if (((LA9_0 >= Identifier && LA9_0 <= StringLiteral) || (LA9_0 >= DecimalLiteral && LA9_0 <= HexIntegerLiteral)))
                    {
                        alt9 = 1;
                    }
                    switch (alt9)
                    {
                        case 1:
                            // ES3.g3:651:16: nameValuePair ( COMMA nameValuePair )*
                            {
                                PushFollow(FOLLOW_nameValuePair_in_objectLiteral3252);
                                nameValuePair31 = nameValuePair();
                                state.followingStackPointer--;

                                stream_nameValuePair.Add(nameValuePair31.Tree);
                                // ES3.g3:651:30: ( COMMA nameValuePair )*
                                do
                                {
                                    int alt8 = 2;
                                    int LA8_0 = input.LA(1);

                                    if ((LA8_0 == COMMA))
                                    {
                                        alt8 = 1;
                                    }


                                    switch (alt8)
                                    {
                                        case 1:
                                            // ES3.g3:651:32: COMMA nameValuePair
                                            {
                                                COMMA32 = (IToken)Match(input, COMMA, FOLLOW_COMMA_in_objectLiteral3256);
                                                stream_COMMA.Add(COMMA32);

                                                PushFollow(FOLLOW_nameValuePair_in_objectLiteral3258);
                                                nameValuePair33 = nameValuePair();
                                                state.followingStackPointer--;

                                                stream_nameValuePair.Add(nameValuePair33.Tree);

                                            }
                                            break;

                                        default:
                                            goto loop8;
                                    }
                                } while (true);

                            loop8:
                                ;	// Stops C# compiler whining that label 'loop8' has no statements


                            }
                            break;

                    }

                    RBRACE34 = (IToken)Match(input, RBRACE, FOLLOW_RBRACE_in_objectLiteral3266);
                    stream_RBRACE.Add(RBRACE34);



                    // AST REWRITE
                    // elements:          nameValuePair
                    // token labels:      
                    // rule labels:       retval
                    // token list labels: 
                    // rule list labels:  
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                    root_0 = (object)adaptor.GetNilNode();
                    // 652:2: -> ^( OBJECT[$lb, \"OBJECT\"] ( nameValuePair )* )
                    {
                        // ES3.g3:652:5: ^( OBJECT[$lb, \"OBJECT\"] ( nameValuePair )* )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(OBJECT, lb, "OBJECT"), root_1);

                            // ES3.g3:652:30: ( nameValuePair )*
                            while (stream_nameValuePair.HasNext())
                            {
                                adaptor.AddChild(root_1, stream_nameValuePair.NextTree());

                            }
                            stream_nameValuePair.Reset();

                            adaptor.AddChild(root_0, root_1);
                        }

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "objectLiteral"

        public class nameValuePair_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "nameValuePair"
        // ES3.g3:655:1: nameValuePair : propertyName COLON assignmentExpression -> ^( NAMEDVALUE propertyName assignmentExpression ) ;
        public ES3Parser.nameValuePair_return nameValuePair() // throws RecognitionException [1]
        {
            ES3Parser.nameValuePair_return retval = new ES3Parser.nameValuePair_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken COLON36 = null;
            ES3Parser.propertyName_return propertyName35 = default(ES3Parser.propertyName_return);

            ES3Parser.assignmentExpression_return assignmentExpression37 = default(ES3Parser.assignmentExpression_return);


            object COLON36_tree = null;
            RewriteRuleTokenStream stream_COLON = new RewriteRuleTokenStream(adaptor, "token COLON");
            RewriteRuleSubtreeStream stream_propertyName = new RewriteRuleSubtreeStream(adaptor, "rule propertyName");
            RewriteRuleSubtreeStream stream_assignmentExpression = new RewriteRuleSubtreeStream(adaptor, "rule assignmentExpression");
            try
            {
                // ES3.g3:656:2: ( propertyName COLON assignmentExpression -> ^( NAMEDVALUE propertyName assignmentExpression ) )
                // ES3.g3:656:4: propertyName COLON assignmentExpression
                {
                    PushFollow(FOLLOW_propertyName_in_nameValuePair3291);
                    propertyName35 = propertyName();
                    state.followingStackPointer--;

                    stream_propertyName.Add(propertyName35.Tree);
                    COLON36 = (IToken)Match(input, COLON, FOLLOW_COLON_in_nameValuePair3293);
                    stream_COLON.Add(COLON36);

                    PushFollow(FOLLOW_assignmentExpression_in_nameValuePair3295);
                    assignmentExpression37 = assignmentExpression();
                    state.followingStackPointer--;

                    stream_assignmentExpression.Add(assignmentExpression37.Tree);


                    // AST REWRITE
                    // elements:          assignmentExpression, propertyName
                    // token labels:      
                    // rule labels:       retval
                    // token list labels: 
                    // rule list labels:  
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                    root_0 = (object)adaptor.GetNilNode();
                    // 657:2: -> ^( NAMEDVALUE propertyName assignmentExpression )
                    {
                        // ES3.g3:657:5: ^( NAMEDVALUE propertyName assignmentExpression )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(NAMEDVALUE, "NAMEDVALUE"), root_1);

                            adaptor.AddChild(root_1, stream_propertyName.NextTree());
                            adaptor.AddChild(root_1, stream_assignmentExpression.NextTree());

                            adaptor.AddChild(root_0, root_1);
                        }

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "nameValuePair"

        public class propertyName_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "propertyName"
        // ES3.g3:660:1: propertyName : ( Identifier | StringLiteral | numericLiteral );
        public ES3Parser.propertyName_return propertyName() // throws RecognitionException [1]
        {
            ES3Parser.propertyName_return retval = new ES3Parser.propertyName_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken Identifier38 = null;
            IToken StringLiteral39 = null;
            ES3Parser.numericLiteral_return numericLiteral40 = default(ES3Parser.numericLiteral_return);


            object Identifier38_tree = null;
            object StringLiteral39_tree = null;

            try
            {
                // ES3.g3:661:2: ( Identifier | StringLiteral | numericLiteral )
                int alt10 = 3;
                switch (input.LA(1))
                {
                    case Identifier:
                        {
                            alt10 = 1;
                        }
                        break;
                    case StringLiteral:
                        {
                            alt10 = 2;
                        }
                        break;
                    case DecimalLiteral:
                    case OctalIntegerLiteral:
                    case HexIntegerLiteral:
                        {
                            alt10 = 3;
                        }
                        break;
                    default:
                        NoViableAltException nvae_d10s0 =
                            new NoViableAltException("", 10, 0, input);

                        throw nvae_d10s0;
                }

                switch (alt10)
                {
                    case 1:
                        // ES3.g3:661:4: Identifier
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            Identifier38 = (IToken)Match(input, Identifier, FOLLOW_Identifier_in_propertyName3319);
                            Identifier38_tree = (object)adaptor.Create(Identifier38);
                            adaptor.AddChild(root_0, Identifier38_tree);


                        }
                        break;
                    case 2:
                        // ES3.g3:662:4: StringLiteral
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            StringLiteral39 = (IToken)Match(input, StringLiteral, FOLLOW_StringLiteral_in_propertyName3324);
                            StringLiteral39_tree = (object)adaptor.Create(StringLiteral39);
                            adaptor.AddChild(root_0, StringLiteral39_tree);


                        }
                        break;
                    case 3:
                        // ES3.g3:663:4: numericLiteral
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_numericLiteral_in_propertyName3329);
                            numericLiteral40 = numericLiteral();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, numericLiteral40.Tree);

                        }
                        break;

                }
                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "propertyName"

        public class memberExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "memberExpression"
        // ES3.g3:675:1: memberExpression : ( primaryExpression | functionExpression | newExpression );
        public ES3Parser.memberExpression_return memberExpression() // throws RecognitionException [1]
        {
            ES3Parser.memberExpression_return retval = new ES3Parser.memberExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            ES3Parser.primaryExpression_return primaryExpression41 = default(ES3Parser.primaryExpression_return);

            ES3Parser.functionExpression_return functionExpression42 = default(ES3Parser.functionExpression_return);

            ES3Parser.newExpression_return newExpression43 = default(ES3Parser.newExpression_return);



            try
            {
                // ES3.g3:676:2: ( primaryExpression | functionExpression | newExpression )
                int alt11 = 3;
                switch (input.LA(1))
                {
                    case NULL:
                    case TRUE:
                    case FALSE:
                    case THIS:
                    case LBRACE:
                    case LPAREN:
                    case LBRACK:
                    case Identifier:
                    case StringLiteral:
                    case RegularExpressionLiteral:
                    case DecimalLiteral:
                    case OctalIntegerLiteral:
                    case HexIntegerLiteral:
                        {
                            alt11 = 1;
                        }
                        break;
                    case FUNCTION:
                        {
                            alt11 = 2;
                        }
                        break;
                    case NEW:
                        {
                            alt11 = 3;
                        }
                        break;
                    default:
                        NoViableAltException nvae_d11s0 =
                            new NoViableAltException("", 11, 0, input);

                        throw nvae_d11s0;
                }

                switch (alt11)
                {
                    case 1:
                        // ES3.g3:676:4: primaryExpression
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_primaryExpression_in_memberExpression3347);
                            primaryExpression41 = primaryExpression();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, primaryExpression41.Tree);

                        }
                        break;
                    case 2:
                        // ES3.g3:677:4: functionExpression
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_functionExpression_in_memberExpression3352);
                            functionExpression42 = functionExpression();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, functionExpression42.Tree);

                        }
                        break;
                    case 3:
                        // ES3.g3:678:4: newExpression
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_newExpression_in_memberExpression3357);
                            newExpression43 = newExpression();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, newExpression43.Tree);

                        }
                        break;

                }
                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "memberExpression"

        public class newExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "newExpression"
        // ES3.g3:681:1: newExpression : NEW primaryExpression ;
        public ES3Parser.newExpression_return newExpression() // throws RecognitionException [1]
        {
            ES3Parser.newExpression_return retval = new ES3Parser.newExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken NEW44 = null;
            ES3Parser.primaryExpression_return primaryExpression45 = default(ES3Parser.primaryExpression_return);


            object NEW44_tree = null;

            try
            {
                // ES3.g3:682:2: ( NEW primaryExpression )
                // ES3.g3:682:4: NEW primaryExpression
                {
                    root_0 = (object)adaptor.GetNilNode();

                    NEW44 = (IToken)Match(input, NEW, FOLLOW_NEW_in_newExpression3368);
                    NEW44_tree = (object)adaptor.Create(NEW44);
                    root_0 = (object)adaptor.BecomeRoot(NEW44_tree, root_0);

                    PushFollow(FOLLOW_primaryExpression_in_newExpression3371);
                    primaryExpression45 = primaryExpression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, primaryExpression45.Tree);

                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "newExpression"

        public class arguments_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "arguments"
        // ES3.g3:686:1: arguments : LPAREN ( assignmentExpression ( COMMA assignmentExpression )* )? RPAREN -> ^( ARGS ( assignmentExpression )* ) ;
        public ES3Parser.arguments_return arguments() // throws RecognitionException [1]
        {
            ES3Parser.arguments_return retval = new ES3Parser.arguments_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken LPAREN46 = null;
            IToken COMMA48 = null;
            IToken RPAREN50 = null;
            ES3Parser.assignmentExpression_return assignmentExpression47 = default(ES3Parser.assignmentExpression_return);

            ES3Parser.assignmentExpression_return assignmentExpression49 = default(ES3Parser.assignmentExpression_return);


            object LPAREN46_tree = null;
            object COMMA48_tree = null;
            object RPAREN50_tree = null;
            RewriteRuleTokenStream stream_RPAREN = new RewriteRuleTokenStream(adaptor, "token RPAREN");
            RewriteRuleTokenStream stream_COMMA = new RewriteRuleTokenStream(adaptor, "token COMMA");
            RewriteRuleTokenStream stream_LPAREN = new RewriteRuleTokenStream(adaptor, "token LPAREN");
            RewriteRuleSubtreeStream stream_assignmentExpression = new RewriteRuleSubtreeStream(adaptor, "rule assignmentExpression");
            try
            {
                // ES3.g3:687:2: ( LPAREN ( assignmentExpression ( COMMA assignmentExpression )* )? RPAREN -> ^( ARGS ( assignmentExpression )* ) )
                // ES3.g3:687:4: LPAREN ( assignmentExpression ( COMMA assignmentExpression )* )? RPAREN
                {
                    LPAREN46 = (IToken)Match(input, LPAREN, FOLLOW_LPAREN_in_arguments3384);
                    stream_LPAREN.Add(LPAREN46);

                    // ES3.g3:687:11: ( assignmentExpression ( COMMA assignmentExpression )* )?
                    int alt13 = 2;
                    int LA13_0 = input.LA(1);

                    if (((LA13_0 >= NULL && LA13_0 <= FALSE) || LA13_0 == DELETE || LA13_0 == FUNCTION || LA13_0 == NEW || LA13_0 == THIS || LA13_0 == TYPEOF || LA13_0 == VOID || LA13_0 == LBRACE || LA13_0 == LPAREN || LA13_0 == LBRACK || (LA13_0 >= ADD && LA13_0 <= SUB) || (LA13_0 >= INC && LA13_0 <= DEC) || (LA13_0 >= NOT && LA13_0 <= INV) || (LA13_0 >= Identifier && LA13_0 <= StringLiteral) || LA13_0 == RegularExpressionLiteral || (LA13_0 >= DecimalLiteral && LA13_0 <= HexIntegerLiteral)))
                    {
                        alt13 = 1;
                    }
                    switch (alt13)
                    {
                        case 1:
                            // ES3.g3:687:13: assignmentExpression ( COMMA assignmentExpression )*
                            {
                                PushFollow(FOLLOW_assignmentExpression_in_arguments3388);
                                assignmentExpression47 = assignmentExpression();
                                state.followingStackPointer--;

                                stream_assignmentExpression.Add(assignmentExpression47.Tree);
                                // ES3.g3:687:34: ( COMMA assignmentExpression )*
                                do
                                {
                                    int alt12 = 2;
                                    int LA12_0 = input.LA(1);

                                    if ((LA12_0 == COMMA))
                                    {
                                        alt12 = 1;
                                    }


                                    switch (alt12)
                                    {
                                        case 1:
                                            // ES3.g3:687:36: COMMA assignmentExpression
                                            {
                                                COMMA48 = (IToken)Match(input, COMMA, FOLLOW_COMMA_in_arguments3392);
                                                stream_COMMA.Add(COMMA48);

                                                PushFollow(FOLLOW_assignmentExpression_in_arguments3394);
                                                assignmentExpression49 = assignmentExpression();
                                                state.followingStackPointer--;

                                                stream_assignmentExpression.Add(assignmentExpression49.Tree);

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

                    RPAREN50 = (IToken)Match(input, RPAREN, FOLLOW_RPAREN_in_arguments3402);
                    stream_RPAREN.Add(RPAREN50);



                    // AST REWRITE
                    // elements:          assignmentExpression
                    // token labels:      
                    // rule labels:       retval
                    // token list labels: 
                    // rule list labels:  
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                    root_0 = (object)adaptor.GetNilNode();
                    // 688:2: -> ^( ARGS ( assignmentExpression )* )
                    {
                        // ES3.g3:688:5: ^( ARGS ( assignmentExpression )* )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(ARGS, "ARGS"), root_1);

                            // ES3.g3:688:13: ( assignmentExpression )*
                            while (stream_assignmentExpression.HasNext())
                            {
                                adaptor.AddChild(root_1, stream_assignmentExpression.NextTree());

                            }
                            stream_assignmentExpression.Reset();

                            adaptor.AddChild(root_0, root_1);
                        }

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "arguments"

        public class leftHandSideExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "leftHandSideExpression"
        // ES3.g3:691:1: leftHandSideExpression : ( memberExpression -> memberExpression ) ( arguments -> ^( CALL $leftHandSideExpression arguments ) | LBRACK expression RBRACK -> ^( BYINDEX $leftHandSideExpression expression ) | DOT Identifier -> ^( BYFIELD $leftHandSideExpression Identifier ) )* ;
        public ES3Parser.leftHandSideExpression_return leftHandSideExpression() // throws RecognitionException [1]
        {
            ES3Parser.leftHandSideExpression_return retval = new ES3Parser.leftHandSideExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken LBRACK53 = null;
            IToken RBRACK55 = null;
            IToken DOT56 = null;
            IToken Identifier57 = null;
            ES3Parser.memberExpression_return memberExpression51 = default(ES3Parser.memberExpression_return);

            ES3Parser.arguments_return arguments52 = default(ES3Parser.arguments_return);

            ES3Parser.expression_return expression54 = default(ES3Parser.expression_return);


            object LBRACK53_tree = null;
            object RBRACK55_tree = null;
            object DOT56_tree = null;
            object Identifier57_tree = null;
            RewriteRuleTokenStream stream_RBRACK = new RewriteRuleTokenStream(adaptor, "token RBRACK");
            RewriteRuleTokenStream stream_LBRACK = new RewriteRuleTokenStream(adaptor, "token LBRACK");
            RewriteRuleTokenStream stream_DOT = new RewriteRuleTokenStream(adaptor, "token DOT");
            RewriteRuleTokenStream stream_Identifier = new RewriteRuleTokenStream(adaptor, "token Identifier");
            RewriteRuleSubtreeStream stream_memberExpression = new RewriteRuleSubtreeStream(adaptor, "rule memberExpression");
            RewriteRuleSubtreeStream stream_expression = new RewriteRuleSubtreeStream(adaptor, "rule expression");
            RewriteRuleSubtreeStream stream_arguments = new RewriteRuleSubtreeStream(adaptor, "rule arguments");
            try
            {
                // ES3.g3:692:2: ( ( memberExpression -> memberExpression ) ( arguments -> ^( CALL $leftHandSideExpression arguments ) | LBRACK expression RBRACK -> ^( BYINDEX $leftHandSideExpression expression ) | DOT Identifier -> ^( BYFIELD $leftHandSideExpression Identifier ) )* )
                // ES3.g3:693:2: ( memberExpression -> memberExpression ) ( arguments -> ^( CALL $leftHandSideExpression arguments ) | LBRACK expression RBRACK -> ^( BYINDEX $leftHandSideExpression expression ) | DOT Identifier -> ^( BYFIELD $leftHandSideExpression Identifier ) )*
                {
                    // ES3.g3:693:2: ( memberExpression -> memberExpression )
                    // ES3.g3:694:3: memberExpression
                    {
                        PushFollow(FOLLOW_memberExpression_in_leftHandSideExpression3431);
                        memberExpression51 = memberExpression();
                        state.followingStackPointer--;

                        stream_memberExpression.Add(memberExpression51.Tree);


                        // AST REWRITE
                        // elements:          memberExpression
                        // token labels:      
                        // rule labels:       retval
                        // token list labels: 
                        // rule list labels:  
                        // wildcard labels: 
                        retval.Tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                        root_0 = (object)adaptor.GetNilNode();
                        // 694:22: -> memberExpression
                        {
                            adaptor.AddChild(root_0, stream_memberExpression.NextTree());

                        }

                        retval.Tree = root_0; retval.Tree = root_0;
                    }

                    // ES3.g3:696:2: ( arguments -> ^( CALL $leftHandSideExpression arguments ) | LBRACK expression RBRACK -> ^( BYINDEX $leftHandSideExpression expression ) | DOT Identifier -> ^( BYFIELD $leftHandSideExpression Identifier ) )*
                    do
                    {
                        int alt14 = 4;
                        switch (input.LA(1))
                        {
                            case LPAREN:
                                {
                                    alt14 = 1;
                                }
                                break;
                            case LBRACK:
                                {
                                    alt14 = 2;
                                }
                                break;
                            case DOT:
                                {
                                    alt14 = 3;
                                }
                                break;

                        }

                        switch (alt14)
                        {
                            case 1:
                                // ES3.g3:697:3: arguments
                                {
                                    PushFollow(FOLLOW_arguments_in_leftHandSideExpression3447);
                                    arguments52 = arguments();
                                    state.followingStackPointer--;

                                    stream_arguments.Add(arguments52.Tree);


                                    // AST REWRITE
                                    // elements:          leftHandSideExpression, arguments
                                    // token labels:      
                                    // rule labels:       retval
                                    // token list labels: 
                                    // rule list labels:  
                                    // wildcard labels: 
                                    retval.Tree = root_0;
                                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                                    root_0 = (object)adaptor.GetNilNode();
                                    // 697:15: -> ^( CALL $leftHandSideExpression arguments )
                                    {
                                        // ES3.g3:697:18: ^( CALL $leftHandSideExpression arguments )
                                        {
                                            object root_1 = (object)adaptor.GetNilNode();
                                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(CALL, "CALL"), root_1);

                                            adaptor.AddChild(root_1, stream_retval.NextTree());
                                            adaptor.AddChild(root_1, stream_arguments.NextTree());

                                            adaptor.AddChild(root_0, root_1);
                                        }

                                    }

                                    retval.Tree = root_0; retval.Tree = root_0;
                                }
                                break;
                            case 2:
                                // ES3.g3:698:5: LBRACK expression RBRACK
                                {
                                    LBRACK53 = (IToken)Match(input, LBRACK, FOLLOW_LBRACK_in_leftHandSideExpression3468);
                                    stream_LBRACK.Add(LBRACK53);

                                    PushFollow(FOLLOW_expression_in_leftHandSideExpression3470);
                                    expression54 = expression();
                                    state.followingStackPointer--;

                                    stream_expression.Add(expression54.Tree);
                                    RBRACK55 = (IToken)Match(input, RBRACK, FOLLOW_RBRACK_in_leftHandSideExpression3472);
                                    stream_RBRACK.Add(RBRACK55);



                                    // AST REWRITE
                                    // elements:          leftHandSideExpression, expression
                                    // token labels:      
                                    // rule labels:       retval
                                    // token list labels: 
                                    // rule list labels:  
                                    // wildcard labels: 
                                    retval.Tree = root_0;
                                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                                    root_0 = (object)adaptor.GetNilNode();
                                    // 698:30: -> ^( BYINDEX $leftHandSideExpression expression )
                                    {
                                        // ES3.g3:698:33: ^( BYINDEX $leftHandSideExpression expression )
                                        {
                                            object root_1 = (object)adaptor.GetNilNode();
                                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(BYINDEX, "BYINDEX"), root_1);

                                            adaptor.AddChild(root_1, stream_retval.NextTree());
                                            adaptor.AddChild(root_1, stream_expression.NextTree());

                                            adaptor.AddChild(root_0, root_1);
                                        }

                                    }

                                    retval.Tree = root_0; retval.Tree = root_0;
                                }
                                break;
                            case 3:
                                // ES3.g3:699:5: DOT Identifier
                                {
                                    DOT56 = (IToken)Match(input, DOT, FOLLOW_DOT_in_leftHandSideExpression3491);
                                    stream_DOT.Add(DOT56);

                                    Identifier57 = (IToken)Match(input, Identifier, FOLLOW_Identifier_in_leftHandSideExpression3493);
                                    stream_Identifier.Add(Identifier57);



                                    // AST REWRITE
                                    // elements:          Identifier, leftHandSideExpression
                                    // token labels:      
                                    // rule labels:       retval
                                    // token list labels: 
                                    // rule list labels:  
                                    // wildcard labels: 
                                    retval.Tree = root_0;
                                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                                    root_0 = (object)adaptor.GetNilNode();
                                    // 699:21: -> ^( BYFIELD $leftHandSideExpression Identifier )
                                    {
                                        // ES3.g3:699:24: ^( BYFIELD $leftHandSideExpression Identifier )
                                        {
                                            object root_1 = (object)adaptor.GetNilNode();
                                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(BYFIELD, "BYFIELD"), root_1);

                                            adaptor.AddChild(root_1, stream_retval.NextTree());
                                            adaptor.AddChild(root_1, stream_Identifier.NextNode());

                                            adaptor.AddChild(root_0, root_1);
                                        }

                                    }

                                    retval.Tree = root_0; retval.Tree = root_0;
                                }
                                break;

                            default:
                                goto loop14;
                        }
                    } while (true);

                loop14:
                    ;	// Stops C# compiler whining that label 'loop14' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "leftHandSideExpression"

        public class postfixExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "postfixExpression"
        // ES3.g3:713:1: postfixExpression : leftHandSideExpression ( postfixOperator )? ;
        public ES3Parser.postfixExpression_return postfixExpression() // throws RecognitionException [1]
        {
            ES3Parser.postfixExpression_return retval = new ES3Parser.postfixExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            ES3Parser.leftHandSideExpression_return leftHandSideExpression58 = default(ES3Parser.leftHandSideExpression_return);

            ES3Parser.postfixOperator_return postfixOperator59 = default(ES3Parser.postfixOperator_return);



            try
            {
                // ES3.g3:714:2: ( leftHandSideExpression ( postfixOperator )? )
                // ES3.g3:714:4: leftHandSideExpression ( postfixOperator )?
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_leftHandSideExpression_in_postfixExpression3528);
                    leftHandSideExpression58 = leftHandSideExpression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, leftHandSideExpression58.Tree);
                    if (input.LA(1) == INC || input.LA(1) == DEC) PromoteEOL(null);
                    // ES3.g3:714:95: ( postfixOperator )?
                    int alt15 = 2;
                    int LA15_0 = input.LA(1);

                    if (((LA15_0 >= INC && LA15_0 <= DEC)))
                    {
                        alt15 = 1;
                    }
                    switch (alt15)
                    {
                        case 1:
                            // ES3.g3:714:97: postfixOperator
                            {
                                PushFollow(FOLLOW_postfixOperator_in_postfixExpression3534);
                                postfixOperator59 = postfixOperator();
                                state.followingStackPointer--;

                                root_0 = (object)adaptor.BecomeRoot(postfixOperator59.Tree, root_0);

                            }
                            break;

                    }


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "postfixExpression"

        public class postfixOperator_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "postfixOperator"
        // ES3.g3:717:1: postfixOperator : (op= INC | op= DEC );
        public ES3Parser.postfixOperator_return postfixOperator() // throws RecognitionException [1]
        {
            ES3Parser.postfixOperator_return retval = new ES3Parser.postfixOperator_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken op = null;

            object op_tree = null;

            try
            {
                // ES3.g3:718:2: (op= INC | op= DEC )
                int alt16 = 2;
                int LA16_0 = input.LA(1);

                if ((LA16_0 == INC))
                {
                    alt16 = 1;
                }
                else if ((LA16_0 == DEC))
                {
                    alt16 = 2;
                }
                else
                {
                    NoViableAltException nvae_d16s0 =
                        new NoViableAltException("", 16, 0, input);

                    throw nvae_d16s0;
                }
                switch (alt16)
                {
                    case 1:
                        // ES3.g3:718:4: op= INC
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            op = (IToken)Match(input, INC, FOLLOW_INC_in_postfixOperator3552);
                            op_tree = (object)adaptor.Create(op);
                            adaptor.AddChild(root_0, op_tree);

                            op.Type = PINC;

                        }
                        break;
                    case 2:
                        // ES3.g3:719:4: op= DEC
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            op = (IToken)Match(input, DEC, FOLLOW_DEC_in_postfixOperator3561);
                            op_tree = (object)adaptor.Create(op);
                            adaptor.AddChild(root_0, op_tree);

                            op.Type = PDEC;

                        }
                        break;

                }
                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "postfixOperator"

        public class unaryExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "unaryExpression"
        // ES3.g3:726:1: unaryExpression : ( postfixExpression | unaryOperator unaryExpression );
        public ES3Parser.unaryExpression_return unaryExpression() // throws RecognitionException [1]
        {
            ES3Parser.unaryExpression_return retval = new ES3Parser.unaryExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            ES3Parser.postfixExpression_return postfixExpression60 = default(ES3Parser.postfixExpression_return);

            ES3Parser.unaryOperator_return unaryOperator61 = default(ES3Parser.unaryOperator_return);

            ES3Parser.unaryExpression_return unaryExpression62 = default(ES3Parser.unaryExpression_return);



            try
            {
                // ES3.g3:727:2: ( postfixExpression | unaryOperator unaryExpression )
                int alt17 = 2;
                int LA17_0 = input.LA(1);

                if (((LA17_0 >= NULL && LA17_0 <= FALSE) || LA17_0 == FUNCTION || LA17_0 == NEW || LA17_0 == THIS || LA17_0 == LBRACE || LA17_0 == LPAREN || LA17_0 == LBRACK || (LA17_0 >= Identifier && LA17_0 <= StringLiteral) || LA17_0 == RegularExpressionLiteral || (LA17_0 >= DecimalLiteral && LA17_0 <= HexIntegerLiteral)))
                {
                    alt17 = 1;
                }
                else if ((LA17_0 == DELETE || LA17_0 == TYPEOF || LA17_0 == VOID || (LA17_0 >= ADD && LA17_0 <= SUB) || (LA17_0 >= INC && LA17_0 <= DEC) || (LA17_0 >= NOT && LA17_0 <= INV)))
                {
                    alt17 = 2;
                }
                else
                {
                    NoViableAltException nvae_d17s0 =
                        new NoViableAltException("", 17, 0, input);

                    throw nvae_d17s0;
                }
                switch (alt17)
                {
                    case 1:
                        // ES3.g3:727:4: postfixExpression
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_postfixExpression_in_unaryExpression3578);
                            postfixExpression60 = postfixExpression();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, postfixExpression60.Tree);

                        }
                        break;
                    case 2:
                        // ES3.g3:728:4: unaryOperator unaryExpression
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_unaryOperator_in_unaryExpression3583);
                            unaryOperator61 = unaryOperator();
                            state.followingStackPointer--;

                            root_0 = (object)adaptor.BecomeRoot(unaryOperator61.Tree, root_0);
                            PushFollow(FOLLOW_unaryExpression_in_unaryExpression3586);
                            unaryExpression62 = unaryExpression();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, unaryExpression62.Tree);

                        }
                        break;

                }
                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "unaryExpression"

        public class unaryOperator_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "unaryOperator"
        // ES3.g3:731:1: unaryOperator : ( DELETE | VOID | TYPEOF | INC | DEC | op= ADD | op= SUB | INV | NOT );
        public ES3Parser.unaryOperator_return unaryOperator() // throws RecognitionException [1]
        {
            ES3Parser.unaryOperator_return retval = new ES3Parser.unaryOperator_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken op = null;
            IToken DELETE63 = null;
            IToken VOID64 = null;
            IToken TYPEOF65 = null;
            IToken INC66 = null;
            IToken DEC67 = null;
            IToken INV68 = null;
            IToken NOT69 = null;

            object op_tree = null;
            object DELETE63_tree = null;
            object VOID64_tree = null;
            object TYPEOF65_tree = null;
            object INC66_tree = null;
            object DEC67_tree = null;
            object INV68_tree = null;
            object NOT69_tree = null;

            try
            {
                // ES3.g3:732:2: ( DELETE | VOID | TYPEOF | INC | DEC | op= ADD | op= SUB | INV | NOT )
                int alt18 = 9;
                switch (input.LA(1))
                {
                    case DELETE:
                        {
                            alt18 = 1;
                        }
                        break;
                    case VOID:
                        {
                            alt18 = 2;
                        }
                        break;
                    case TYPEOF:
                        {
                            alt18 = 3;
                        }
                        break;
                    case INC:
                        {
                            alt18 = 4;
                        }
                        break;
                    case DEC:
                        {
                            alt18 = 5;
                        }
                        break;
                    case ADD:
                        {
                            alt18 = 6;
                        }
                        break;
                    case SUB:
                        {
                            alt18 = 7;
                        }
                        break;
                    case INV:
                        {
                            alt18 = 8;
                        }
                        break;
                    case NOT:
                        {
                            alt18 = 9;
                        }
                        break;
                    default:
                        NoViableAltException nvae_d18s0 =
                            new NoViableAltException("", 18, 0, input);

                        throw nvae_d18s0;
                }

                switch (alt18)
                {
                    case 1:
                        // ES3.g3:732:4: DELETE
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            DELETE63 = (IToken)Match(input, DELETE, FOLLOW_DELETE_in_unaryOperator3598);
                            DELETE63_tree = (object)adaptor.Create(DELETE63);
                            adaptor.AddChild(root_0, DELETE63_tree);


                        }
                        break;
                    case 2:
                        // ES3.g3:733:4: VOID
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            VOID64 = (IToken)Match(input, VOID, FOLLOW_VOID_in_unaryOperator3603);
                            VOID64_tree = (object)adaptor.Create(VOID64);
                            adaptor.AddChild(root_0, VOID64_tree);


                        }
                        break;
                    case 3:
                        // ES3.g3:734:4: TYPEOF
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            TYPEOF65 = (IToken)Match(input, TYPEOF, FOLLOW_TYPEOF_in_unaryOperator3608);
                            TYPEOF65_tree = (object)adaptor.Create(TYPEOF65);
                            adaptor.AddChild(root_0, TYPEOF65_tree);


                        }
                        break;
                    case 4:
                        // ES3.g3:735:4: INC
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            INC66 = (IToken)Match(input, INC, FOLLOW_INC_in_unaryOperator3613);
                            INC66_tree = (object)adaptor.Create(INC66);
                            adaptor.AddChild(root_0, INC66_tree);


                        }
                        break;
                    case 5:
                        // ES3.g3:736:4: DEC
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            DEC67 = (IToken)Match(input, DEC, FOLLOW_DEC_in_unaryOperator3618);
                            DEC67_tree = (object)adaptor.Create(DEC67);
                            adaptor.AddChild(root_0, DEC67_tree);


                        }
                        break;
                    case 6:
                        // ES3.g3:737:4: op= ADD
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            op = (IToken)Match(input, ADD, FOLLOW_ADD_in_unaryOperator3625);
                            op_tree = (object)adaptor.Create(op);
                            adaptor.AddChild(root_0, op_tree);

                            op.Type = POS;

                        }
                        break;
                    case 7:
                        // ES3.g3:738:4: op= SUB
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            op = (IToken)Match(input, SUB, FOLLOW_SUB_in_unaryOperator3634);
                            op_tree = (object)adaptor.Create(op);
                            adaptor.AddChild(root_0, op_tree);

                            op.Type = NEG;

                        }
                        break;
                    case 8:
                        // ES3.g3:739:4: INV
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            INV68 = (IToken)Match(input, INV, FOLLOW_INV_in_unaryOperator3641);
                            INV68_tree = (object)adaptor.Create(INV68);
                            adaptor.AddChild(root_0, INV68_tree);


                        }
                        break;
                    case 9:
                        // ES3.g3:740:4: NOT
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            NOT69 = (IToken)Match(input, NOT, FOLLOW_NOT_in_unaryOperator3646);
                            NOT69_tree = (object)adaptor.Create(NOT69);
                            adaptor.AddChild(root_0, NOT69_tree);


                        }
                        break;

                }
                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "unaryOperator"

        public class multiplicativeExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "multiplicativeExpression"
        // ES3.g3:747:1: multiplicativeExpression : unaryExpression ( ( MUL | DIV | MOD ) unaryExpression )* ;
        public ES3Parser.multiplicativeExpression_return multiplicativeExpression() // throws RecognitionException [1]
        {
            ES3Parser.multiplicativeExpression_return retval = new ES3Parser.multiplicativeExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken set71 = null;
            ES3Parser.unaryExpression_return unaryExpression70 = default(ES3Parser.unaryExpression_return);

            ES3Parser.unaryExpression_return unaryExpression72 = default(ES3Parser.unaryExpression_return);


            object set71_tree = null;

            try
            {
                // ES3.g3:748:2: ( unaryExpression ( ( MUL | DIV | MOD ) unaryExpression )* )
                // ES3.g3:748:4: unaryExpression ( ( MUL | DIV | MOD ) unaryExpression )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_unaryExpression_in_multiplicativeExpression3661);
                    unaryExpression70 = unaryExpression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, unaryExpression70.Tree);
                    // ES3.g3:748:20: ( ( MUL | DIV | MOD ) unaryExpression )*
                    do
                    {
                        int alt19 = 2;
                        int LA19_0 = input.LA(1);

                        if (((LA19_0 >= MUL && LA19_0 <= MOD) || LA19_0 == DIV))
                        {
                            alt19 = 1;
                        }


                        switch (alt19)
                        {
                            case 1:
                                // ES3.g3:748:22: ( MUL | DIV | MOD ) unaryExpression
                                {
                                    set71 = (IToken)input.LT(1);
                                    set71 = (IToken)input.LT(1);
                                    if ((input.LA(1) >= MUL && input.LA(1) <= MOD) || input.LA(1) == DIV)
                                    {
                                        input.Consume();
                                        root_0 = (object)adaptor.BecomeRoot((object)adaptor.Create(set71), root_0);
                                        state.errorRecovery = false;
                                    }
                                    else
                                    {
                                        MismatchedSetException mse = new MismatchedSetException(null, input);
                                        throw mse;
                                    }

                                    PushFollow(FOLLOW_unaryExpression_in_multiplicativeExpression3680);
                                    unaryExpression72 = unaryExpression();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, unaryExpression72.Tree);

                                }
                                break;

                            default:
                                goto loop19;
                        }
                    } while (true);

                loop19:
                    ;	// Stops C# compiler whining that label 'loop19' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "multiplicativeExpression"

        public class additiveExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "additiveExpression"
        // ES3.g3:755:1: additiveExpression : multiplicativeExpression ( ( ADD | SUB ) multiplicativeExpression )* ;
        public ES3Parser.additiveExpression_return additiveExpression() // throws RecognitionException [1]
        {
            ES3Parser.additiveExpression_return retval = new ES3Parser.additiveExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken set74 = null;
            ES3Parser.multiplicativeExpression_return multiplicativeExpression73 = default(ES3Parser.multiplicativeExpression_return);

            ES3Parser.multiplicativeExpression_return multiplicativeExpression75 = default(ES3Parser.multiplicativeExpression_return);


            object set74_tree = null;

            try
            {
                // ES3.g3:756:2: ( multiplicativeExpression ( ( ADD | SUB ) multiplicativeExpression )* )
                // ES3.g3:756:4: multiplicativeExpression ( ( ADD | SUB ) multiplicativeExpression )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_multiplicativeExpression_in_additiveExpression3698);
                    multiplicativeExpression73 = multiplicativeExpression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, multiplicativeExpression73.Tree);
                    // ES3.g3:756:29: ( ( ADD | SUB ) multiplicativeExpression )*
                    do
                    {
                        int alt20 = 2;
                        int LA20_0 = input.LA(1);

                        if (((LA20_0 >= ADD && LA20_0 <= SUB)))
                        {
                            alt20 = 1;
                        }


                        switch (alt20)
                        {
                            case 1:
                                // ES3.g3:756:31: ( ADD | SUB ) multiplicativeExpression
                                {
                                    set74 = (IToken)input.LT(1);
                                    set74 = (IToken)input.LT(1);
                                    if ((input.LA(1) >= ADD && input.LA(1) <= SUB))
                                    {
                                        input.Consume();
                                        root_0 = (object)adaptor.BecomeRoot((object)adaptor.Create(set74), root_0);
                                        state.errorRecovery = false;
                                    }
                                    else
                                    {
                                        MismatchedSetException mse = new MismatchedSetException(null, input);
                                        throw mse;
                                    }

                                    PushFollow(FOLLOW_multiplicativeExpression_in_additiveExpression3713);
                                    multiplicativeExpression75 = multiplicativeExpression();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, multiplicativeExpression75.Tree);

                                }
                                break;

                            default:
                                goto loop20;
                        }
                    } while (true);

                loop20:
                    ;	// Stops C# compiler whining that label 'loop20' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "additiveExpression"

        public class shiftExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "shiftExpression"
        // ES3.g3:763:1: shiftExpression : additiveExpression ( ( SHL | SHR | SHU ) additiveExpression )* ;
        public ES3Parser.shiftExpression_return shiftExpression() // throws RecognitionException [1]
        {
            ES3Parser.shiftExpression_return retval = new ES3Parser.shiftExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken set77 = null;
            ES3Parser.additiveExpression_return additiveExpression76 = default(ES3Parser.additiveExpression_return);

            ES3Parser.additiveExpression_return additiveExpression78 = default(ES3Parser.additiveExpression_return);


            object set77_tree = null;

            try
            {
                // ES3.g3:764:2: ( additiveExpression ( ( SHL | SHR | SHU ) additiveExpression )* )
                // ES3.g3:764:4: additiveExpression ( ( SHL | SHR | SHU ) additiveExpression )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_additiveExpression_in_shiftExpression3732);
                    additiveExpression76 = additiveExpression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, additiveExpression76.Tree);
                    // ES3.g3:764:23: ( ( SHL | SHR | SHU ) additiveExpression )*
                    do
                    {
                        int alt21 = 2;
                        int LA21_0 = input.LA(1);

                        if (((LA21_0 >= SHL && LA21_0 <= SHU)))
                        {
                            alt21 = 1;
                        }


                        switch (alt21)
                        {
                            case 1:
                                // ES3.g3:764:25: ( SHL | SHR | SHU ) additiveExpression
                                {
                                    set77 = (IToken)input.LT(1);
                                    set77 = (IToken)input.LT(1);
                                    if ((input.LA(1) >= SHL && input.LA(1) <= SHU))
                                    {
                                        input.Consume();
                                        root_0 = (object)adaptor.BecomeRoot((object)adaptor.Create(set77), root_0);
                                        state.errorRecovery = false;
                                    }
                                    else
                                    {
                                        MismatchedSetException mse = new MismatchedSetException(null, input);
                                        throw mse;
                                    }

                                    PushFollow(FOLLOW_additiveExpression_in_shiftExpression3751);
                                    additiveExpression78 = additiveExpression();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, additiveExpression78.Tree);

                                }
                                break;

                            default:
                                goto loop21;
                        }
                    } while (true);

                loop21:
                    ;	// Stops C# compiler whining that label 'loop21' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "shiftExpression"

        public class relationalExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "relationalExpression"
        // ES3.g3:771:1: relationalExpression : shiftExpression ( ( LT | GT | LTE | GTE | INSTANCEOF | IN ) shiftExpression )* ;
        public ES3Parser.relationalExpression_return relationalExpression() // throws RecognitionException [1]
        {
            ES3Parser.relationalExpression_return retval = new ES3Parser.relationalExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken set80 = null;
            ES3Parser.shiftExpression_return shiftExpression79 = default(ES3Parser.shiftExpression_return);

            ES3Parser.shiftExpression_return shiftExpression81 = default(ES3Parser.shiftExpression_return);


            object set80_tree = null;

            try
            {
                // ES3.g3:772:2: ( shiftExpression ( ( LT | GT | LTE | GTE | INSTANCEOF | IN ) shiftExpression )* )
                // ES3.g3:772:4: shiftExpression ( ( LT | GT | LTE | GTE | INSTANCEOF | IN ) shiftExpression )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_shiftExpression_in_relationalExpression3770);
                    shiftExpression79 = shiftExpression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, shiftExpression79.Tree);
                    // ES3.g3:772:20: ( ( LT | GT | LTE | GTE | INSTANCEOF | IN ) shiftExpression )*
                    do
                    {
                        int alt22 = 2;
                        int LA22_0 = input.LA(1);

                        if (((LA22_0 >= IN && LA22_0 <= INSTANCEOF) || (LA22_0 >= LT && LA22_0 <= GTE)))
                        {
                            alt22 = 1;
                        }


                        switch (alt22)
                        {
                            case 1:
                                // ES3.g3:772:22: ( LT | GT | LTE | GTE | INSTANCEOF | IN ) shiftExpression
                                {
                                    set80 = (IToken)input.LT(1);
                                    set80 = (IToken)input.LT(1);
                                    if ((input.LA(1) >= IN && input.LA(1) <= INSTANCEOF) || (input.LA(1) >= LT && input.LA(1) <= GTE))
                                    {
                                        input.Consume();
                                        root_0 = (object)adaptor.BecomeRoot((object)adaptor.Create(set80), root_0);
                                        state.errorRecovery = false;
                                    }
                                    else
                                    {
                                        MismatchedSetException mse = new MismatchedSetException(null, input);
                                        throw mse;
                                    }

                                    PushFollow(FOLLOW_shiftExpression_in_relationalExpression3801);
                                    shiftExpression81 = shiftExpression();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, shiftExpression81.Tree);

                                }
                                break;

                            default:
                                goto loop22;
                        }
                    } while (true);

                loop22:
                    ;	// Stops C# compiler whining that label 'loop22' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "relationalExpression"

        public class relationalExpressionNoIn_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "relationalExpressionNoIn"
        // ES3.g3:775:1: relationalExpressionNoIn : shiftExpression ( ( LT | GT | LTE | GTE | INSTANCEOF ) shiftExpression )* ;
        public ES3Parser.relationalExpressionNoIn_return relationalExpressionNoIn() // throws RecognitionException [1]
        {
            ES3Parser.relationalExpressionNoIn_return retval = new ES3Parser.relationalExpressionNoIn_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken set83 = null;
            ES3Parser.shiftExpression_return shiftExpression82 = default(ES3Parser.shiftExpression_return);

            ES3Parser.shiftExpression_return shiftExpression84 = default(ES3Parser.shiftExpression_return);


            object set83_tree = null;

            try
            {
                // ES3.g3:776:2: ( shiftExpression ( ( LT | GT | LTE | GTE | INSTANCEOF ) shiftExpression )* )
                // ES3.g3:776:4: shiftExpression ( ( LT | GT | LTE | GTE | INSTANCEOF ) shiftExpression )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_shiftExpression_in_relationalExpressionNoIn3815);
                    shiftExpression82 = shiftExpression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, shiftExpression82.Tree);
                    // ES3.g3:776:20: ( ( LT | GT | LTE | GTE | INSTANCEOF ) shiftExpression )*
                    do
                    {
                        int alt23 = 2;
                        int LA23_0 = input.LA(1);

                        if ((LA23_0 == INSTANCEOF || (LA23_0 >= LT && LA23_0 <= GTE)))
                        {
                            alt23 = 1;
                        }


                        switch (alt23)
                        {
                            case 1:
                                // ES3.g3:776:22: ( LT | GT | LTE | GTE | INSTANCEOF ) shiftExpression
                                {
                                    set83 = (IToken)input.LT(1);
                                    set83 = (IToken)input.LT(1);
                                    if (input.LA(1) == INSTANCEOF || (input.LA(1) >= LT && input.LA(1) <= GTE))
                                    {
                                        input.Consume();
                                        root_0 = (object)adaptor.BecomeRoot((object)adaptor.Create(set83), root_0);
                                        state.errorRecovery = false;
                                    }
                                    else
                                    {
                                        MismatchedSetException mse = new MismatchedSetException(null, input);
                                        throw mse;
                                    }

                                    PushFollow(FOLLOW_shiftExpression_in_relationalExpressionNoIn3842);
                                    shiftExpression84 = shiftExpression();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, shiftExpression84.Tree);

                                }
                                break;

                            default:
                                goto loop23;
                        }
                    } while (true);

                loop23:
                    ;	// Stops C# compiler whining that label 'loop23' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "relationalExpressionNoIn"

        public class equalityExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "equalityExpression"
        // ES3.g3:783:1: equalityExpression : relationalExpression ( ( EQ | NEQ | SAME | NSAME ) relationalExpression )* ;
        public ES3Parser.equalityExpression_return equalityExpression() // throws RecognitionException [1]
        {
            ES3Parser.equalityExpression_return retval = new ES3Parser.equalityExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken set86 = null;
            ES3Parser.relationalExpression_return relationalExpression85 = default(ES3Parser.relationalExpression_return);

            ES3Parser.relationalExpression_return relationalExpression87 = default(ES3Parser.relationalExpression_return);


            object set86_tree = null;

            try
            {
                // ES3.g3:784:2: ( relationalExpression ( ( EQ | NEQ | SAME | NSAME ) relationalExpression )* )
                // ES3.g3:784:4: relationalExpression ( ( EQ | NEQ | SAME | NSAME ) relationalExpression )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_relationalExpression_in_equalityExpression3861);
                    relationalExpression85 = relationalExpression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, relationalExpression85.Tree);
                    // ES3.g3:784:25: ( ( EQ | NEQ | SAME | NSAME ) relationalExpression )*
                    do
                    {
                        int alt24 = 2;
                        int LA24_0 = input.LA(1);

                        if (((LA24_0 >= EQ && LA24_0 <= NSAME)))
                        {
                            alt24 = 1;
                        }


                        switch (alt24)
                        {
                            case 1:
                                // ES3.g3:784:27: ( EQ | NEQ | SAME | NSAME ) relationalExpression
                                {
                                    set86 = (IToken)input.LT(1);
                                    set86 = (IToken)input.LT(1);
                                    if ((input.LA(1) >= EQ && input.LA(1) <= NSAME))
                                    {
                                        input.Consume();
                                        root_0 = (object)adaptor.BecomeRoot((object)adaptor.Create(set86), root_0);
                                        state.errorRecovery = false;
                                    }
                                    else
                                    {
                                        MismatchedSetException mse = new MismatchedSetException(null, input);
                                        throw mse;
                                    }

                                    PushFollow(FOLLOW_relationalExpression_in_equalityExpression3884);
                                    relationalExpression87 = relationalExpression();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, relationalExpression87.Tree);

                                }
                                break;

                            default:
                                goto loop24;
                        }
                    } while (true);

                loop24:
                    ;	// Stops C# compiler whining that label 'loop24' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "equalityExpression"

        public class equalityExpressionNoIn_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "equalityExpressionNoIn"
        // ES3.g3:787:1: equalityExpressionNoIn : relationalExpressionNoIn ( ( EQ | NEQ | SAME | NSAME ) relationalExpressionNoIn )* ;
        public ES3Parser.equalityExpressionNoIn_return equalityExpressionNoIn() // throws RecognitionException [1]
        {
            ES3Parser.equalityExpressionNoIn_return retval = new ES3Parser.equalityExpressionNoIn_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken set89 = null;
            ES3Parser.relationalExpressionNoIn_return relationalExpressionNoIn88 = default(ES3Parser.relationalExpressionNoIn_return);

            ES3Parser.relationalExpressionNoIn_return relationalExpressionNoIn90 = default(ES3Parser.relationalExpressionNoIn_return);


            object set89_tree = null;

            try
            {
                // ES3.g3:788:2: ( relationalExpressionNoIn ( ( EQ | NEQ | SAME | NSAME ) relationalExpressionNoIn )* )
                // ES3.g3:788:4: relationalExpressionNoIn ( ( EQ | NEQ | SAME | NSAME ) relationalExpressionNoIn )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_relationalExpressionNoIn_in_equalityExpressionNoIn3898);
                    relationalExpressionNoIn88 = relationalExpressionNoIn();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, relationalExpressionNoIn88.Tree);
                    // ES3.g3:788:29: ( ( EQ | NEQ | SAME | NSAME ) relationalExpressionNoIn )*
                    do
                    {
                        int alt25 = 2;
                        int LA25_0 = input.LA(1);

                        if (((LA25_0 >= EQ && LA25_0 <= NSAME)))
                        {
                            alt25 = 1;
                        }


                        switch (alt25)
                        {
                            case 1:
                                // ES3.g3:788:31: ( EQ | NEQ | SAME | NSAME ) relationalExpressionNoIn
                                {
                                    set89 = (IToken)input.LT(1);
                                    set89 = (IToken)input.LT(1);
                                    if ((input.LA(1) >= EQ && input.LA(1) <= NSAME))
                                    {
                                        input.Consume();
                                        root_0 = (object)adaptor.BecomeRoot((object)adaptor.Create(set89), root_0);
                                        state.errorRecovery = false;
                                    }
                                    else
                                    {
                                        MismatchedSetException mse = new MismatchedSetException(null, input);
                                        throw mse;
                                    }

                                    PushFollow(FOLLOW_relationalExpressionNoIn_in_equalityExpressionNoIn3921);
                                    relationalExpressionNoIn90 = relationalExpressionNoIn();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, relationalExpressionNoIn90.Tree);

                                }
                                break;

                            default:
                                goto loop25;
                        }
                    } while (true);

                loop25:
                    ;	// Stops C# compiler whining that label 'loop25' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "equalityExpressionNoIn"

        public class bitwiseANDExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "bitwiseANDExpression"
        // ES3.g3:795:1: bitwiseANDExpression : equalityExpression ( AND equalityExpression )* ;
        public ES3Parser.bitwiseANDExpression_return bitwiseANDExpression() // throws RecognitionException [1]
        {
            ES3Parser.bitwiseANDExpression_return retval = new ES3Parser.bitwiseANDExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken AND92 = null;
            ES3Parser.equalityExpression_return equalityExpression91 = default(ES3Parser.equalityExpression_return);

            ES3Parser.equalityExpression_return equalityExpression93 = default(ES3Parser.equalityExpression_return);


            object AND92_tree = null;

            try
            {
                // ES3.g3:796:2: ( equalityExpression ( AND equalityExpression )* )
                // ES3.g3:796:4: equalityExpression ( AND equalityExpression )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_equalityExpression_in_bitwiseANDExpression3941);
                    equalityExpression91 = equalityExpression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, equalityExpression91.Tree);
                    // ES3.g3:796:23: ( AND equalityExpression )*
                    do
                    {
                        int alt26 = 2;
                        int LA26_0 = input.LA(1);

                        if ((LA26_0 == AND))
                        {
                            alt26 = 1;
                        }


                        switch (alt26)
                        {
                            case 1:
                                // ES3.g3:796:25: AND equalityExpression
                                {
                                    AND92 = (IToken)Match(input, AND, FOLLOW_AND_in_bitwiseANDExpression3945);
                                    AND92_tree = (object)adaptor.Create(AND92);
                                    root_0 = (object)adaptor.BecomeRoot(AND92_tree, root_0);

                                    PushFollow(FOLLOW_equalityExpression_in_bitwiseANDExpression3948);
                                    equalityExpression93 = equalityExpression();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, equalityExpression93.Tree);

                                }
                                break;

                            default:
                                goto loop26;
                        }
                    } while (true);

                loop26:
                    ;	// Stops C# compiler whining that label 'loop26' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "bitwiseANDExpression"

        public class bitwiseANDExpressionNoIn_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "bitwiseANDExpressionNoIn"
        // ES3.g3:799:1: bitwiseANDExpressionNoIn : equalityExpressionNoIn ( AND equalityExpressionNoIn )* ;
        public ES3Parser.bitwiseANDExpressionNoIn_return bitwiseANDExpressionNoIn() // throws RecognitionException [1]
        {
            ES3Parser.bitwiseANDExpressionNoIn_return retval = new ES3Parser.bitwiseANDExpressionNoIn_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken AND95 = null;
            ES3Parser.equalityExpressionNoIn_return equalityExpressionNoIn94 = default(ES3Parser.equalityExpressionNoIn_return);

            ES3Parser.equalityExpressionNoIn_return equalityExpressionNoIn96 = default(ES3Parser.equalityExpressionNoIn_return);


            object AND95_tree = null;

            try
            {
                // ES3.g3:800:2: ( equalityExpressionNoIn ( AND equalityExpressionNoIn )* )
                // ES3.g3:800:4: equalityExpressionNoIn ( AND equalityExpressionNoIn )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_equalityExpressionNoIn_in_bitwiseANDExpressionNoIn3962);
                    equalityExpressionNoIn94 = equalityExpressionNoIn();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, equalityExpressionNoIn94.Tree);
                    // ES3.g3:800:27: ( AND equalityExpressionNoIn )*
                    do
                    {
                        int alt27 = 2;
                        int LA27_0 = input.LA(1);

                        if ((LA27_0 == AND))
                        {
                            alt27 = 1;
                        }


                        switch (alt27)
                        {
                            case 1:
                                // ES3.g3:800:29: AND equalityExpressionNoIn
                                {
                                    AND95 = (IToken)Match(input, AND, FOLLOW_AND_in_bitwiseANDExpressionNoIn3966);
                                    AND95_tree = (object)adaptor.Create(AND95);
                                    root_0 = (object)adaptor.BecomeRoot(AND95_tree, root_0);

                                    PushFollow(FOLLOW_equalityExpressionNoIn_in_bitwiseANDExpressionNoIn3969);
                                    equalityExpressionNoIn96 = equalityExpressionNoIn();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, equalityExpressionNoIn96.Tree);

                                }
                                break;

                            default:
                                goto loop27;
                        }
                    } while (true);

                loop27:
                    ;	// Stops C# compiler whining that label 'loop27' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "bitwiseANDExpressionNoIn"

        public class bitwiseXORExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "bitwiseXORExpression"
        // ES3.g3:803:1: bitwiseXORExpression : bitwiseANDExpression ( XOR bitwiseANDExpression )* ;
        public ES3Parser.bitwiseXORExpression_return bitwiseXORExpression() // throws RecognitionException [1]
        {
            ES3Parser.bitwiseXORExpression_return retval = new ES3Parser.bitwiseXORExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken XOR98 = null;
            ES3Parser.bitwiseANDExpression_return bitwiseANDExpression97 = default(ES3Parser.bitwiseANDExpression_return);

            ES3Parser.bitwiseANDExpression_return bitwiseANDExpression99 = default(ES3Parser.bitwiseANDExpression_return);


            object XOR98_tree = null;

            try
            {
                // ES3.g3:804:2: ( bitwiseANDExpression ( XOR bitwiseANDExpression )* )
                // ES3.g3:804:4: bitwiseANDExpression ( XOR bitwiseANDExpression )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_bitwiseANDExpression_in_bitwiseXORExpression3985);
                    bitwiseANDExpression97 = bitwiseANDExpression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, bitwiseANDExpression97.Tree);
                    // ES3.g3:804:25: ( XOR bitwiseANDExpression )*
                    do
                    {
                        int alt28 = 2;
                        int LA28_0 = input.LA(1);

                        if ((LA28_0 == XOR))
                        {
                            alt28 = 1;
                        }


                        switch (alt28)
                        {
                            case 1:
                                // ES3.g3:804:27: XOR bitwiseANDExpression
                                {
                                    XOR98 = (IToken)Match(input, XOR, FOLLOW_XOR_in_bitwiseXORExpression3989);
                                    XOR98_tree = (object)adaptor.Create(XOR98);
                                    root_0 = (object)adaptor.BecomeRoot(XOR98_tree, root_0);

                                    PushFollow(FOLLOW_bitwiseANDExpression_in_bitwiseXORExpression3992);
                                    bitwiseANDExpression99 = bitwiseANDExpression();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, bitwiseANDExpression99.Tree);

                                }
                                break;

                            default:
                                goto loop28;
                        }
                    } while (true);

                loop28:
                    ;	// Stops C# compiler whining that label 'loop28' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "bitwiseXORExpression"

        public class bitwiseXORExpressionNoIn_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "bitwiseXORExpressionNoIn"
        // ES3.g3:807:1: bitwiseXORExpressionNoIn : bitwiseANDExpressionNoIn ( XOR bitwiseANDExpressionNoIn )* ;
        public ES3Parser.bitwiseXORExpressionNoIn_return bitwiseXORExpressionNoIn() // throws RecognitionException [1]
        {
            ES3Parser.bitwiseXORExpressionNoIn_return retval = new ES3Parser.bitwiseXORExpressionNoIn_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken XOR101 = null;
            ES3Parser.bitwiseANDExpressionNoIn_return bitwiseANDExpressionNoIn100 = default(ES3Parser.bitwiseANDExpressionNoIn_return);

            ES3Parser.bitwiseANDExpressionNoIn_return bitwiseANDExpressionNoIn102 = default(ES3Parser.bitwiseANDExpressionNoIn_return);


            object XOR101_tree = null;

            try
            {
                // ES3.g3:808:2: ( bitwiseANDExpressionNoIn ( XOR bitwiseANDExpressionNoIn )* )
                // ES3.g3:808:4: bitwiseANDExpressionNoIn ( XOR bitwiseANDExpressionNoIn )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_bitwiseANDExpressionNoIn_in_bitwiseXORExpressionNoIn4008);
                    bitwiseANDExpressionNoIn100 = bitwiseANDExpressionNoIn();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, bitwiseANDExpressionNoIn100.Tree);
                    // ES3.g3:808:29: ( XOR bitwiseANDExpressionNoIn )*
                    do
                    {
                        int alt29 = 2;
                        int LA29_0 = input.LA(1);

                        if ((LA29_0 == XOR))
                        {
                            alt29 = 1;
                        }


                        switch (alt29)
                        {
                            case 1:
                                // ES3.g3:808:31: XOR bitwiseANDExpressionNoIn
                                {
                                    XOR101 = (IToken)Match(input, XOR, FOLLOW_XOR_in_bitwiseXORExpressionNoIn4012);
                                    XOR101_tree = (object)adaptor.Create(XOR101);
                                    root_0 = (object)adaptor.BecomeRoot(XOR101_tree, root_0);

                                    PushFollow(FOLLOW_bitwiseANDExpressionNoIn_in_bitwiseXORExpressionNoIn4015);
                                    bitwiseANDExpressionNoIn102 = bitwiseANDExpressionNoIn();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, bitwiseANDExpressionNoIn102.Tree);

                                }
                                break;

                            default:
                                goto loop29;
                        }
                    } while (true);

                loop29:
                    ;	// Stops C# compiler whining that label 'loop29' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "bitwiseXORExpressionNoIn"

        public class bitwiseORExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "bitwiseORExpression"
        // ES3.g3:811:1: bitwiseORExpression : bitwiseXORExpression ( OR bitwiseXORExpression )* ;
        public ES3Parser.bitwiseORExpression_return bitwiseORExpression() // throws RecognitionException [1]
        {
            ES3Parser.bitwiseORExpression_return retval = new ES3Parser.bitwiseORExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken OR104 = null;
            ES3Parser.bitwiseXORExpression_return bitwiseXORExpression103 = default(ES3Parser.bitwiseXORExpression_return);

            ES3Parser.bitwiseXORExpression_return bitwiseXORExpression105 = default(ES3Parser.bitwiseXORExpression_return);


            object OR104_tree = null;

            try
            {
                // ES3.g3:812:2: ( bitwiseXORExpression ( OR bitwiseXORExpression )* )
                // ES3.g3:812:4: bitwiseXORExpression ( OR bitwiseXORExpression )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_bitwiseXORExpression_in_bitwiseORExpression4030);
                    bitwiseXORExpression103 = bitwiseXORExpression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, bitwiseXORExpression103.Tree);
                    // ES3.g3:812:25: ( OR bitwiseXORExpression )*
                    do
                    {
                        int alt30 = 2;
                        int LA30_0 = input.LA(1);

                        if ((LA30_0 == OR))
                        {
                            alt30 = 1;
                        }


                        switch (alt30)
                        {
                            case 1:
                                // ES3.g3:812:27: OR bitwiseXORExpression
                                {
                                    OR104 = (IToken)Match(input, OR, FOLLOW_OR_in_bitwiseORExpression4034);
                                    OR104_tree = (object)adaptor.Create(OR104);
                                    root_0 = (object)adaptor.BecomeRoot(OR104_tree, root_0);

                                    PushFollow(FOLLOW_bitwiseXORExpression_in_bitwiseORExpression4037);
                                    bitwiseXORExpression105 = bitwiseXORExpression();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, bitwiseXORExpression105.Tree);

                                }
                                break;

                            default:
                                goto loop30;
                        }
                    } while (true);

                loop30:
                    ;	// Stops C# compiler whining that label 'loop30' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "bitwiseORExpression"

        public class bitwiseORExpressionNoIn_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "bitwiseORExpressionNoIn"
        // ES3.g3:815:1: bitwiseORExpressionNoIn : bitwiseXORExpressionNoIn ( OR bitwiseXORExpressionNoIn )* ;
        public ES3Parser.bitwiseORExpressionNoIn_return bitwiseORExpressionNoIn() // throws RecognitionException [1]
        {
            ES3Parser.bitwiseORExpressionNoIn_return retval = new ES3Parser.bitwiseORExpressionNoIn_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken OR107 = null;
            ES3Parser.bitwiseXORExpressionNoIn_return bitwiseXORExpressionNoIn106 = default(ES3Parser.bitwiseXORExpressionNoIn_return);

            ES3Parser.bitwiseXORExpressionNoIn_return bitwiseXORExpressionNoIn108 = default(ES3Parser.bitwiseXORExpressionNoIn_return);


            object OR107_tree = null;

            try
            {
                // ES3.g3:816:2: ( bitwiseXORExpressionNoIn ( OR bitwiseXORExpressionNoIn )* )
                // ES3.g3:816:4: bitwiseXORExpressionNoIn ( OR bitwiseXORExpressionNoIn )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_bitwiseXORExpressionNoIn_in_bitwiseORExpressionNoIn4052);
                    bitwiseXORExpressionNoIn106 = bitwiseXORExpressionNoIn();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, bitwiseXORExpressionNoIn106.Tree);
                    // ES3.g3:816:29: ( OR bitwiseXORExpressionNoIn )*
                    do
                    {
                        int alt31 = 2;
                        int LA31_0 = input.LA(1);

                        if ((LA31_0 == OR))
                        {
                            alt31 = 1;
                        }


                        switch (alt31)
                        {
                            case 1:
                                // ES3.g3:816:31: OR bitwiseXORExpressionNoIn
                                {
                                    OR107 = (IToken)Match(input, OR, FOLLOW_OR_in_bitwiseORExpressionNoIn4056);
                                    OR107_tree = (object)adaptor.Create(OR107);
                                    root_0 = (object)adaptor.BecomeRoot(OR107_tree, root_0);

                                    PushFollow(FOLLOW_bitwiseXORExpressionNoIn_in_bitwiseORExpressionNoIn4059);
                                    bitwiseXORExpressionNoIn108 = bitwiseXORExpressionNoIn();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, bitwiseXORExpressionNoIn108.Tree);

                                }
                                break;

                            default:
                                goto loop31;
                        }
                    } while (true);

                loop31:
                    ;	// Stops C# compiler whining that label 'loop31' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "bitwiseORExpressionNoIn"

        public class logicalANDExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "logicalANDExpression"
        // ES3.g3:823:1: logicalANDExpression : bitwiseORExpression ( LAND bitwiseORExpression )* ;
        public ES3Parser.logicalANDExpression_return logicalANDExpression() // throws RecognitionException [1]
        {
            ES3Parser.logicalANDExpression_return retval = new ES3Parser.logicalANDExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken LAND110 = null;
            ES3Parser.bitwiseORExpression_return bitwiseORExpression109 = default(ES3Parser.bitwiseORExpression_return);

            ES3Parser.bitwiseORExpression_return bitwiseORExpression111 = default(ES3Parser.bitwiseORExpression_return);


            object LAND110_tree = null;

            try
            {
                // ES3.g3:824:2: ( bitwiseORExpression ( LAND bitwiseORExpression )* )
                // ES3.g3:824:4: bitwiseORExpression ( LAND bitwiseORExpression )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_bitwiseORExpression_in_logicalANDExpression4078);
                    bitwiseORExpression109 = bitwiseORExpression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, bitwiseORExpression109.Tree);
                    // ES3.g3:824:24: ( LAND bitwiseORExpression )*
                    do
                    {
                        int alt32 = 2;
                        int LA32_0 = input.LA(1);

                        if ((LA32_0 == LAND))
                        {
                            alt32 = 1;
                        }


                        switch (alt32)
                        {
                            case 1:
                                // ES3.g3:824:26: LAND bitwiseORExpression
                                {
                                    LAND110 = (IToken)Match(input, LAND, FOLLOW_LAND_in_logicalANDExpression4082);
                                    LAND110_tree = (object)adaptor.Create(LAND110);
                                    root_0 = (object)adaptor.BecomeRoot(LAND110_tree, root_0);

                                    PushFollow(FOLLOW_bitwiseORExpression_in_logicalANDExpression4085);
                                    bitwiseORExpression111 = bitwiseORExpression();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, bitwiseORExpression111.Tree);

                                }
                                break;

                            default:
                                goto loop32;
                        }
                    } while (true);

                loop32:
                    ;	// Stops C# compiler whining that label 'loop32' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "logicalANDExpression"

        public class logicalANDExpressionNoIn_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "logicalANDExpressionNoIn"
        // ES3.g3:827:1: logicalANDExpressionNoIn : bitwiseORExpressionNoIn ( LAND bitwiseORExpressionNoIn )* ;
        public ES3Parser.logicalANDExpressionNoIn_return logicalANDExpressionNoIn() // throws RecognitionException [1]
        {
            ES3Parser.logicalANDExpressionNoIn_return retval = new ES3Parser.logicalANDExpressionNoIn_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken LAND113 = null;
            ES3Parser.bitwiseORExpressionNoIn_return bitwiseORExpressionNoIn112 = default(ES3Parser.bitwiseORExpressionNoIn_return);

            ES3Parser.bitwiseORExpressionNoIn_return bitwiseORExpressionNoIn114 = default(ES3Parser.bitwiseORExpressionNoIn_return);


            object LAND113_tree = null;

            try
            {
                // ES3.g3:828:2: ( bitwiseORExpressionNoIn ( LAND bitwiseORExpressionNoIn )* )
                // ES3.g3:828:4: bitwiseORExpressionNoIn ( LAND bitwiseORExpressionNoIn )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_bitwiseORExpressionNoIn_in_logicalANDExpressionNoIn4099);
                    bitwiseORExpressionNoIn112 = bitwiseORExpressionNoIn();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, bitwiseORExpressionNoIn112.Tree);
                    // ES3.g3:828:28: ( LAND bitwiseORExpressionNoIn )*
                    do
                    {
                        int alt33 = 2;
                        int LA33_0 = input.LA(1);

                        if ((LA33_0 == LAND))
                        {
                            alt33 = 1;
                        }


                        switch (alt33)
                        {
                            case 1:
                                // ES3.g3:828:30: LAND bitwiseORExpressionNoIn
                                {
                                    LAND113 = (IToken)Match(input, LAND, FOLLOW_LAND_in_logicalANDExpressionNoIn4103);
                                    LAND113_tree = (object)adaptor.Create(LAND113);
                                    root_0 = (object)adaptor.BecomeRoot(LAND113_tree, root_0);

                                    PushFollow(FOLLOW_bitwiseORExpressionNoIn_in_logicalANDExpressionNoIn4106);
                                    bitwiseORExpressionNoIn114 = bitwiseORExpressionNoIn();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, bitwiseORExpressionNoIn114.Tree);

                                }
                                break;

                            default:
                                goto loop33;
                        }
                    } while (true);

                loop33:
                    ;	// Stops C# compiler whining that label 'loop33' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "logicalANDExpressionNoIn"

        public class logicalORExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "logicalORExpression"
        // ES3.g3:831:1: logicalORExpression : logicalANDExpression ( LOR logicalANDExpression )* ;
        public ES3Parser.logicalORExpression_return logicalORExpression() // throws RecognitionException [1]
        {
            ES3Parser.logicalORExpression_return retval = new ES3Parser.logicalORExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken LOR116 = null;
            ES3Parser.logicalANDExpression_return logicalANDExpression115 = default(ES3Parser.logicalANDExpression_return);

            ES3Parser.logicalANDExpression_return logicalANDExpression117 = default(ES3Parser.logicalANDExpression_return);


            object LOR116_tree = null;

            try
            {
                // ES3.g3:832:2: ( logicalANDExpression ( LOR logicalANDExpression )* )
                // ES3.g3:832:4: logicalANDExpression ( LOR logicalANDExpression )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_logicalANDExpression_in_logicalORExpression4121);
                    logicalANDExpression115 = logicalANDExpression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, logicalANDExpression115.Tree);
                    // ES3.g3:832:25: ( LOR logicalANDExpression )*
                    do
                    {
                        int alt34 = 2;
                        int LA34_0 = input.LA(1);

                        if ((LA34_0 == LOR))
                        {
                            alt34 = 1;
                        }


                        switch (alt34)
                        {
                            case 1:
                                // ES3.g3:832:27: LOR logicalANDExpression
                                {
                                    LOR116 = (IToken)Match(input, LOR, FOLLOW_LOR_in_logicalORExpression4125);
                                    LOR116_tree = (object)adaptor.Create(LOR116);
                                    root_0 = (object)adaptor.BecomeRoot(LOR116_tree, root_0);

                                    PushFollow(FOLLOW_logicalANDExpression_in_logicalORExpression4128);
                                    logicalANDExpression117 = logicalANDExpression();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, logicalANDExpression117.Tree);

                                }
                                break;

                            default:
                                goto loop34;
                        }
                    } while (true);

                loop34:
                    ;	// Stops C# compiler whining that label 'loop34' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "logicalORExpression"

        public class logicalORExpressionNoIn_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "logicalORExpressionNoIn"
        // ES3.g3:835:1: logicalORExpressionNoIn : logicalANDExpressionNoIn ( LOR logicalANDExpressionNoIn )* ;
        public ES3Parser.logicalORExpressionNoIn_return logicalORExpressionNoIn() // throws RecognitionException [1]
        {
            ES3Parser.logicalORExpressionNoIn_return retval = new ES3Parser.logicalORExpressionNoIn_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken LOR119 = null;
            ES3Parser.logicalANDExpressionNoIn_return logicalANDExpressionNoIn118 = default(ES3Parser.logicalANDExpressionNoIn_return);

            ES3Parser.logicalANDExpressionNoIn_return logicalANDExpressionNoIn120 = default(ES3Parser.logicalANDExpressionNoIn_return);


            object LOR119_tree = null;

            try
            {
                // ES3.g3:836:2: ( logicalANDExpressionNoIn ( LOR logicalANDExpressionNoIn )* )
                // ES3.g3:836:4: logicalANDExpressionNoIn ( LOR logicalANDExpressionNoIn )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_logicalANDExpressionNoIn_in_logicalORExpressionNoIn4143);
                    logicalANDExpressionNoIn118 = logicalANDExpressionNoIn();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, logicalANDExpressionNoIn118.Tree);
                    // ES3.g3:836:29: ( LOR logicalANDExpressionNoIn )*
                    do
                    {
                        int alt35 = 2;
                        int LA35_0 = input.LA(1);

                        if ((LA35_0 == LOR))
                        {
                            alt35 = 1;
                        }


                        switch (alt35)
                        {
                            case 1:
                                // ES3.g3:836:31: LOR logicalANDExpressionNoIn
                                {
                                    LOR119 = (IToken)Match(input, LOR, FOLLOW_LOR_in_logicalORExpressionNoIn4147);
                                    LOR119_tree = (object)adaptor.Create(LOR119);
                                    root_0 = (object)adaptor.BecomeRoot(LOR119_tree, root_0);

                                    PushFollow(FOLLOW_logicalANDExpressionNoIn_in_logicalORExpressionNoIn4150);
                                    logicalANDExpressionNoIn120 = logicalANDExpressionNoIn();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, logicalANDExpressionNoIn120.Tree);

                                }
                                break;

                            default:
                                goto loop35;
                        }
                    } while (true);

                loop35:
                    ;	// Stops C# compiler whining that label 'loop35' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "logicalORExpressionNoIn"

        public class conditionalExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "conditionalExpression"
        // ES3.g3:843:1: conditionalExpression : logicalORExpression ( QUE assignmentExpression COLON assignmentExpression )? ;
        public ES3Parser.conditionalExpression_return conditionalExpression() // throws RecognitionException [1]
        {
            ES3Parser.conditionalExpression_return retval = new ES3Parser.conditionalExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken QUE122 = null;
            IToken COLON124 = null;
            ES3Parser.logicalORExpression_return logicalORExpression121 = default(ES3Parser.logicalORExpression_return);

            ES3Parser.assignmentExpression_return assignmentExpression123 = default(ES3Parser.assignmentExpression_return);

            ES3Parser.assignmentExpression_return assignmentExpression125 = default(ES3Parser.assignmentExpression_return);


            object QUE122_tree = null;
            object COLON124_tree = null;

            try
            {
                // ES3.g3:844:2: ( logicalORExpression ( QUE assignmentExpression COLON assignmentExpression )? )
                // ES3.g3:844:4: logicalORExpression ( QUE assignmentExpression COLON assignmentExpression )?
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_logicalORExpression_in_conditionalExpression4169);
                    logicalORExpression121 = logicalORExpression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, logicalORExpression121.Tree);
                    // ES3.g3:844:24: ( QUE assignmentExpression COLON assignmentExpression )?
                    int alt36 = 2;
                    int LA36_0 = input.LA(1);

                    if ((LA36_0 == QUE))
                    {
                        alt36 = 1;
                    }
                    switch (alt36)
                    {
                        case 1:
                            // ES3.g3:844:26: QUE assignmentExpression COLON assignmentExpression
                            {
                                QUE122 = (IToken)Match(input, QUE, FOLLOW_QUE_in_conditionalExpression4173);
                                QUE122_tree = (object)adaptor.Create(QUE122);
                                root_0 = (object)adaptor.BecomeRoot(QUE122_tree, root_0);

                                PushFollow(FOLLOW_assignmentExpression_in_conditionalExpression4176);
                                assignmentExpression123 = assignmentExpression();
                                state.followingStackPointer--;

                                adaptor.AddChild(root_0, assignmentExpression123.Tree);
                                COLON124 = (IToken)Match(input, COLON, FOLLOW_COLON_in_conditionalExpression4178);
                                PushFollow(FOLLOW_assignmentExpression_in_conditionalExpression4181);
                                assignmentExpression125 = assignmentExpression();
                                state.followingStackPointer--;

                                adaptor.AddChild(root_0, assignmentExpression125.Tree);

                            }
                            break;

                    }


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "conditionalExpression"

        public class conditionalExpressionNoIn_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "conditionalExpressionNoIn"
        // ES3.g3:847:1: conditionalExpressionNoIn : logicalORExpressionNoIn ( QUE assignmentExpressionNoIn COLON assignmentExpressionNoIn )? ;
        public ES3Parser.conditionalExpressionNoIn_return conditionalExpressionNoIn() // throws RecognitionException [1]
        {
            ES3Parser.conditionalExpressionNoIn_return retval = new ES3Parser.conditionalExpressionNoIn_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken QUE127 = null;
            IToken COLON129 = null;
            ES3Parser.logicalORExpressionNoIn_return logicalORExpressionNoIn126 = default(ES3Parser.logicalORExpressionNoIn_return);

            ES3Parser.assignmentExpressionNoIn_return assignmentExpressionNoIn128 = default(ES3Parser.assignmentExpressionNoIn_return);

            ES3Parser.assignmentExpressionNoIn_return assignmentExpressionNoIn130 = default(ES3Parser.assignmentExpressionNoIn_return);


            object QUE127_tree = null;
            object COLON129_tree = null;

            try
            {
                // ES3.g3:848:2: ( logicalORExpressionNoIn ( QUE assignmentExpressionNoIn COLON assignmentExpressionNoIn )? )
                // ES3.g3:848:4: logicalORExpressionNoIn ( QUE assignmentExpressionNoIn COLON assignmentExpressionNoIn )?
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_logicalORExpressionNoIn_in_conditionalExpressionNoIn4195);
                    logicalORExpressionNoIn126 = logicalORExpressionNoIn();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, logicalORExpressionNoIn126.Tree);
                    // ES3.g3:848:28: ( QUE assignmentExpressionNoIn COLON assignmentExpressionNoIn )?
                    int alt37 = 2;
                    int LA37_0 = input.LA(1);

                    if ((LA37_0 == QUE))
                    {
                        alt37 = 1;
                    }
                    switch (alt37)
                    {
                        case 1:
                            // ES3.g3:848:30: QUE assignmentExpressionNoIn COLON assignmentExpressionNoIn
                            {
                                QUE127 = (IToken)Match(input, QUE, FOLLOW_QUE_in_conditionalExpressionNoIn4199);
                                QUE127_tree = (object)adaptor.Create(QUE127);
                                root_0 = (object)adaptor.BecomeRoot(QUE127_tree, root_0);

                                PushFollow(FOLLOW_assignmentExpressionNoIn_in_conditionalExpressionNoIn4202);
                                assignmentExpressionNoIn128 = assignmentExpressionNoIn();
                                state.followingStackPointer--;

                                adaptor.AddChild(root_0, assignmentExpressionNoIn128.Tree);
                                COLON129 = (IToken)Match(input, COLON, FOLLOW_COLON_in_conditionalExpressionNoIn4204);
                                PushFollow(FOLLOW_assignmentExpressionNoIn_in_conditionalExpressionNoIn4207);
                                assignmentExpressionNoIn130 = assignmentExpressionNoIn();
                                state.followingStackPointer--;

                                adaptor.AddChild(root_0, assignmentExpressionNoIn130.Tree);

                            }
                            break;

                    }


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "conditionalExpressionNoIn"

        public class assignmentExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "assignmentExpression"
        // ES3.g3:877:1: assignmentExpression : lhs= conditionalExpression ({...}? assignmentOperator assignmentExpression )? ;
        public ES3Parser.assignmentExpression_return assignmentExpression() // throws RecognitionException [1]
        {
            ES3Parser.assignmentExpression_return retval = new ES3Parser.assignmentExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            ES3Parser.conditionalExpression_return lhs = default(ES3Parser.conditionalExpression_return);

            ES3Parser.assignmentOperator_return assignmentOperator131 = default(ES3Parser.assignmentOperator_return);

            ES3Parser.assignmentExpression_return assignmentExpression132 = default(ES3Parser.assignmentExpression_return);




            bool? isLhs = null;

            try
            {
                // ES3.g3:882:2: (lhs= conditionalExpression ({...}? assignmentOperator assignmentExpression )? )
                // ES3.g3:882:4: lhs= conditionalExpression ({...}? assignmentOperator assignmentExpression )?
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_conditionalExpression_in_assignmentExpression4235);
                    lhs = conditionalExpression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, lhs.Tree);
                    // ES3.g3:883:2: ({...}? assignmentOperator assignmentExpression )?
                    int alt38 = 2;
                    int LA38_0 = input.LA(1);

                    if (((LA38_0 >= ASSIGN && LA38_0 <= XORASS) || LA38_0 == DIVASS))
                    {
                        int LA38_1 = input.LA(2);

                        if (((IsLeftHandSideAssign(lhs, ref isLhs))))
                        {
                            alt38 = 1;
                        }
                    }
                    switch (alt38)
                    {
                        case 1:
                            // ES3.g3:883:4: {...}? assignmentOperator assignmentExpression
                            {
                                if (!((IsLeftHandSideAssign(lhs, ref isLhs))))
                                {
                                    throw new FailedPredicateException(input, "assignmentExpression", " IsLeftHandSideAssign(lhs, ref isLhs) ");
                                }
                                PushFollow(FOLLOW_assignmentOperator_in_assignmentExpression4242);
                                assignmentOperator131 = assignmentOperator();
                                state.followingStackPointer--;

                                root_0 = (object)adaptor.BecomeRoot(assignmentOperator131.Tree, root_0);
                                PushFollow(FOLLOW_assignmentExpression_in_assignmentExpression4245);
                                assignmentExpression132 = assignmentExpression();
                                state.followingStackPointer--;

                                adaptor.AddChild(root_0, assignmentExpression132.Tree);

                            }
                            break;

                    }


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "assignmentExpression"

        public class assignmentOperator_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "assignmentOperator"
        // ES3.g3:886:1: assignmentOperator : ( ASSIGN | MULASS | DIVASS | MODASS | ADDASS | SUBASS | SHLASS | SHRASS | SHUASS | ANDASS | XORASS | ORASS );
        public ES3Parser.assignmentOperator_return assignmentOperator() // throws RecognitionException [1]
        {
            ES3Parser.assignmentOperator_return retval = new ES3Parser.assignmentOperator_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken set133 = null;

            object set133_tree = null;

            try
            {
                // ES3.g3:887:2: ( ASSIGN | MULASS | DIVASS | MODASS | ADDASS | SUBASS | SHLASS | SHRASS | SHUASS | ANDASS | XORASS | ORASS )
                // ES3.g3:
                {
                    root_0 = (object)adaptor.GetNilNode();

                    set133 = (IToken)input.LT(1);
                    if ((input.LA(1) >= ASSIGN && input.LA(1) <= XORASS) || input.LA(1) == DIVASS)
                    {
                        input.Consume();
                        adaptor.AddChild(root_0, (object)adaptor.Create(set133));
                        state.errorRecovery = false;
                    }
                    else
                    {
                        MismatchedSetException mse = new MismatchedSetException(null, input);
                        throw mse;
                    }


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "assignmentOperator"

        public class assignmentExpressionNoIn_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "assignmentExpressionNoIn"
        // ES3.g3:890:1: assignmentExpressionNoIn : lhs= conditionalExpressionNoIn ({...}? assignmentOperator assignmentExpressionNoIn )? ;
        public ES3Parser.assignmentExpressionNoIn_return assignmentExpressionNoIn() // throws RecognitionException [1]
        {
            ES3Parser.assignmentExpressionNoIn_return retval = new ES3Parser.assignmentExpressionNoIn_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            ES3Parser.conditionalExpressionNoIn_return lhs = default(ES3Parser.conditionalExpressionNoIn_return);

            ES3Parser.assignmentOperator_return assignmentOperator134 = default(ES3Parser.assignmentOperator_return);

            ES3Parser.assignmentExpressionNoIn_return assignmentExpressionNoIn135 = default(ES3Parser.assignmentExpressionNoIn_return);




            bool? isLhs = null;

            try
            {
                // ES3.g3:895:2: (lhs= conditionalExpressionNoIn ({...}? assignmentOperator assignmentExpressionNoIn )? )
                // ES3.g3:895:4: lhs= conditionalExpressionNoIn ({...}? assignmentOperator assignmentExpressionNoIn )?
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_conditionalExpressionNoIn_in_assignmentExpressionNoIn4322);
                    lhs = conditionalExpressionNoIn();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, lhs.Tree);
                    // ES3.g3:896:2: ({...}? assignmentOperator assignmentExpressionNoIn )?
                    int alt39 = 2;
                    int LA39_0 = input.LA(1);

                    if (((LA39_0 >= ASSIGN && LA39_0 <= XORASS) || LA39_0 == DIVASS))
                    {
                        int LA39_1 = input.LA(2);

                        if (((IsLeftHandSideAssign(lhs, ref isLhs))))
                        {
                            alt39 = 1;
                        }
                    }
                    switch (alt39)
                    {
                        case 1:
                            // ES3.g3:896:4: {...}? assignmentOperator assignmentExpressionNoIn
                            {
                                if (!((IsLeftHandSideAssign(lhs, ref isLhs))))
                                {
                                    throw new FailedPredicateException(input, "assignmentExpressionNoIn", " IsLeftHandSideAssign(lhs, ref isLhs) ");
                                }
                                PushFollow(FOLLOW_assignmentOperator_in_assignmentExpressionNoIn4329);
                                assignmentOperator134 = assignmentOperator();
                                state.followingStackPointer--;

                                root_0 = (object)adaptor.BecomeRoot(assignmentOperator134.Tree, root_0);
                                PushFollow(FOLLOW_assignmentExpressionNoIn_in_assignmentExpressionNoIn4332);
                                assignmentExpressionNoIn135 = assignmentExpressionNoIn();
                                state.followingStackPointer--;

                                adaptor.AddChild(root_0, assignmentExpressionNoIn135.Tree);

                            }
                            break;

                    }


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "assignmentExpressionNoIn"

        public class expression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "expression"
        // ES3.g3:903:1: expression : exprs+= assignmentExpression ( COMMA exprs+= assignmentExpression )* -> { $exprs.Count > 1 }? ^( CEXPR ( $exprs)+ ) -> $exprs;
        public ES3Parser.expression_return expression() // throws RecognitionException [1]
        {
            ES3Parser.expression_return retval = new ES3Parser.expression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken COMMA136 = null;
            IList list_exprs = null;
            ES3Parser.assignmentExpression_return exprs = default(ES3Parser.assignmentExpression_return);
            exprs = null;
            object COMMA136_tree = null;
            RewriteRuleTokenStream stream_COMMA = new RewriteRuleTokenStream(adaptor, "token COMMA");
            RewriteRuleSubtreeStream stream_assignmentExpression = new RewriteRuleSubtreeStream(adaptor, "rule assignmentExpression");
            try
            {
                // ES3.g3:904:2: (exprs+= assignmentExpression ( COMMA exprs+= assignmentExpression )* -> { $exprs.Count > 1 }? ^( CEXPR ( $exprs)+ ) -> $exprs)
                // ES3.g3:904:4: exprs+= assignmentExpression ( COMMA exprs+= assignmentExpression )*
                {
                    PushFollow(FOLLOW_assignmentExpression_in_expression4354);
                    exprs = assignmentExpression();
                    state.followingStackPointer--;

                    stream_assignmentExpression.Add(exprs.Tree);
                    if (list_exprs == null) list_exprs = new ArrayList();
                    list_exprs.Add(exprs.Tree);

                    // ES3.g3:904:32: ( COMMA exprs+= assignmentExpression )*
                    do
                    {
                        int alt40 = 2;
                        int LA40_0 = input.LA(1);

                        if ((LA40_0 == COMMA))
                        {
                            alt40 = 1;
                        }


                        switch (alt40)
                        {
                            case 1:
                                // ES3.g3:904:34: COMMA exprs+= assignmentExpression
                                {
                                    COMMA136 = (IToken)Match(input, COMMA, FOLLOW_COMMA_in_expression4358);
                                    stream_COMMA.Add(COMMA136);

                                    PushFollow(FOLLOW_assignmentExpression_in_expression4362);
                                    exprs = assignmentExpression();
                                    state.followingStackPointer--;

                                    stream_assignmentExpression.Add(exprs.Tree);
                                    if (list_exprs == null) list_exprs = new ArrayList();
                                    list_exprs.Add(exprs.Tree);


                                }
                                break;

                            default:
                                goto loop40;
                        }
                    } while (true);

                loop40:
                    ;	// Stops C# compiler whining that label 'loop40' has no statements



                    // AST REWRITE
                    // elements:          exprs, exprs
                    // token labels:      
                    // rule labels:       retval
                    // token list labels: 
                    // rule list labels:  exprs
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);
                    RewriteRuleSubtreeStream stream_exprs = new RewriteRuleSubtreeStream(adaptor, "token exprs", list_exprs);
                    root_0 = (object)adaptor.GetNilNode();
                    // 905:2: -> { $exprs.Count > 1 }? ^( CEXPR ( $exprs)+ )
                    if (list_exprs.Count > 1)
                    {
                        // ES3.g3:905:27: ^( CEXPR ( $exprs)+ )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(CEXPR, "CEXPR"), root_1);

                            if (!(stream_exprs.HasNext()))
                            {
                                throw new RewriteEarlyExitException();
                            }
                            while (stream_exprs.HasNext())
                            {
                                adaptor.AddChild(root_1, stream_exprs.NextTree());

                            }
                            stream_exprs.Reset();

                            adaptor.AddChild(root_0, root_1);
                        }

                    }
                    else // 906:2: -> $exprs
                    {
                        adaptor.AddChild(root_0, stream_exprs.NextTree());

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "expression"

        public class expressionNoIn_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "expressionNoIn"
        // ES3.g3:909:1: expressionNoIn : exprs+= assignmentExpressionNoIn ( COMMA exprs+= assignmentExpressionNoIn )* -> { $exprs.Count > 1 }? ^( CEXPR ( $exprs)+ ) -> $exprs;
        public ES3Parser.expressionNoIn_return expressionNoIn() // throws RecognitionException [1]
        {
            ES3Parser.expressionNoIn_return retval = new ES3Parser.expressionNoIn_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken COMMA137 = null;
            IList list_exprs = null;
            ES3Parser.assignmentExpressionNoIn_return exprs = default(ES3Parser.assignmentExpressionNoIn_return);
            exprs = null;
            object COMMA137_tree = null;
            RewriteRuleTokenStream stream_COMMA = new RewriteRuleTokenStream(adaptor, "token COMMA");
            RewriteRuleSubtreeStream stream_assignmentExpressionNoIn = new RewriteRuleSubtreeStream(adaptor, "rule assignmentExpressionNoIn");
            try
            {
                // ES3.g3:910:2: (exprs+= assignmentExpressionNoIn ( COMMA exprs+= assignmentExpressionNoIn )* -> { $exprs.Count > 1 }? ^( CEXPR ( $exprs)+ ) -> $exprs)
                // ES3.g3:910:4: exprs+= assignmentExpressionNoIn ( COMMA exprs+= assignmentExpressionNoIn )*
                {
                    PushFollow(FOLLOW_assignmentExpressionNoIn_in_expressionNoIn4399);
                    exprs = assignmentExpressionNoIn();
                    state.followingStackPointer--;

                    stream_assignmentExpressionNoIn.Add(exprs.Tree);
                    if (list_exprs == null) list_exprs = new ArrayList();
                    list_exprs.Add(exprs.Tree);

                    // ES3.g3:910:36: ( COMMA exprs+= assignmentExpressionNoIn )*
                    do
                    {
                        int alt41 = 2;
                        int LA41_0 = input.LA(1);

                        if ((LA41_0 == COMMA))
                        {
                            alt41 = 1;
                        }


                        switch (alt41)
                        {
                            case 1:
                                // ES3.g3:910:38: COMMA exprs+= assignmentExpressionNoIn
                                {
                                    COMMA137 = (IToken)Match(input, COMMA, FOLLOW_COMMA_in_expressionNoIn4403);
                                    stream_COMMA.Add(COMMA137);

                                    PushFollow(FOLLOW_assignmentExpressionNoIn_in_expressionNoIn4407);
                                    exprs = assignmentExpressionNoIn();
                                    state.followingStackPointer--;

                                    stream_assignmentExpressionNoIn.Add(exprs.Tree);
                                    if (list_exprs == null) list_exprs = new ArrayList();
                                    list_exprs.Add(exprs.Tree);


                                }
                                break;

                            default:
                                goto loop41;
                        }
                    } while (true);

                loop41:
                    ;	// Stops C# compiler whining that label 'loop41' has no statements



                    // AST REWRITE
                    // elements:          exprs, exprs
                    // token labels:      
                    // rule labels:       retval
                    // token list labels: 
                    // rule list labels:  exprs
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);
                    RewriteRuleSubtreeStream stream_exprs = new RewriteRuleSubtreeStream(adaptor, "token exprs", list_exprs);
                    root_0 = (object)adaptor.GetNilNode();
                    // 911:2: -> { $exprs.Count > 1 }? ^( CEXPR ( $exprs)+ )
                    if (list_exprs.Count > 1)
                    {
                        // ES3.g3:911:27: ^( CEXPR ( $exprs)+ )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(CEXPR, "CEXPR"), root_1);

                            if (!(stream_exprs.HasNext()))
                            {
                                throw new RewriteEarlyExitException();
                            }
                            while (stream_exprs.HasNext())
                            {
                                adaptor.AddChild(root_1, stream_exprs.NextTree());

                            }
                            stream_exprs.Reset();

                            adaptor.AddChild(root_0, root_1);
                        }

                    }
                    else // 912:2: -> $exprs
                    {
                        adaptor.AddChild(root_0, stream_exprs.NextTree());

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "expressionNoIn"

        public class semic_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "semic"
        // ES3.g3:937:1: semic : ( SEMIC | EOF | RBRACE | EOL | MultiLineComment );
        public ES3Parser.semic_return semic() // throws RecognitionException [1]
        {
            ES3Parser.semic_return retval = new ES3Parser.semic_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken SEMIC138 = null;
            IToken EOF139 = null;
            IToken RBRACE140 = null;
            IToken EOL141 = null;
            IToken MultiLineComment142 = null;

            object SEMIC138_tree = null;
            object EOF139_tree = null;
            object RBRACE140_tree = null;
            object EOL141_tree = null;
            object MultiLineComment142_tree = null;


            // Mark current position so we can unconsume a RBRACE.
            int marker = input.Mark();
            // Promote EOL if appropriate	
            PromoteEOL(retval);

            try
            {
                // ES3.g3:945:2: ( SEMIC | EOF | RBRACE | EOL | MultiLineComment )
                int alt42 = 5;
                switch (input.LA(1))
                {
                    case SEMIC:
                        {
                            alt42 = 1;
                        }
                        break;
                    case EOF:
                        {
                            alt42 = 2;
                        }
                        break;
                    case RBRACE:
                        {
                            alt42 = 3;
                        }
                        break;
                    case EOL:
                        {
                            alt42 = 4;
                        }
                        break;
                    case MultiLineComment:
                        {
                            alt42 = 5;
                        }
                        break;
                    default:
                        NoViableAltException nvae_d42s0 =
                            new NoViableAltException("", 42, 0, input);

                        throw nvae_d42s0;
                }

                switch (alt42)
                {
                    case 1:
                        // ES3.g3:945:4: SEMIC
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            SEMIC138 = (IToken)Match(input, SEMIC, FOLLOW_SEMIC_in_semic4458);
                            SEMIC138_tree = (object)adaptor.Create(SEMIC138);
                            adaptor.AddChild(root_0, SEMIC138_tree);


                        }
                        break;
                    case 2:
                        // ES3.g3:946:4: EOF
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            EOF139 = (IToken)Match(input, EOF, FOLLOW_EOF_in_semic4463);
                            EOF139_tree = (object)adaptor.Create(EOF139);
                            adaptor.AddChild(root_0, EOF139_tree);


                        }
                        break;
                    case 3:
                        // ES3.g3:947:4: RBRACE
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            RBRACE140 = (IToken)Match(input, RBRACE, FOLLOW_RBRACE_in_semic4468);
                            RBRACE140_tree = (object)adaptor.Create(RBRACE140);
                            adaptor.AddChild(root_0, RBRACE140_tree);

                            input.Rewind(marker);

                        }
                        break;
                    case 4:
                        // ES3.g3:948:4: EOL
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            EOL141 = (IToken)Match(input, EOL, FOLLOW_EOL_in_semic4475);
                            EOL141_tree = (object)adaptor.Create(EOL141);
                            adaptor.AddChild(root_0, EOL141_tree);


                        }
                        break;
                    case 5:
                        // ES3.g3:948:10: MultiLineComment
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            MultiLineComment142 = (IToken)Match(input, MultiLineComment, FOLLOW_MultiLineComment_in_semic4479);
                            MultiLineComment142_tree = (object)adaptor.Create(MultiLineComment142);
                            adaptor.AddChild(root_0, MultiLineComment142_tree);


                        }
                        break;

                }
                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "semic"

        public class statement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "statement"
        // ES3.g3:956:1: statement options {k=1; } : ({...}? block | statementTail );
        public ES3Parser.statement_return statement() // throws RecognitionException [1]
        {
            ES3Parser.statement_return retval = new ES3Parser.statement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            ES3Parser.block_return block143 = default(ES3Parser.block_return);

            ES3Parser.statementTail_return statementTail144 = default(ES3Parser.statementTail_return);



            try
            {
                // ES3.g3:961:2: ({...}? block | statementTail )
                int alt43 = 2;
                alt43 = dfa43.Predict(input);
                switch (alt43)
                {
                    case 1:
                        // ES3.g3:961:4: {...}? block
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            if (!((input.LA(1) == LBRACE)))
                            {
                                throw new FailedPredicateException(input, "statement", " input.LA(1) == LBRACE ");
                            }
                            PushFollow(FOLLOW_block_in_statement4508);
                            block143 = block();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, block143.Tree);

                        }
                        break;
                    case 2:
                        // ES3.g3:962:4: statementTail
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_statementTail_in_statement4513);
                            statementTail144 = statementTail();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, statementTail144.Tree);

                        }
                        break;

                }
                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "statement"

        public class statementTail_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "statementTail"
        // ES3.g3:965:1: statementTail : ( variableStatement | emptyStatement | expressionStatement | ifStatement | iterationStatement | continueStatement | breakStatement | returnStatement | withStatement | labelledStatement | switchStatement | throwStatement | tryStatement );
        public ES3Parser.statementTail_return statementTail() // throws RecognitionException [1]
        {
            ES3Parser.statementTail_return retval = new ES3Parser.statementTail_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            ES3Parser.variableStatement_return variableStatement145 = default(ES3Parser.variableStatement_return);

            ES3Parser.emptyStatement_return emptyStatement146 = default(ES3Parser.emptyStatement_return);

            ES3Parser.expressionStatement_return expressionStatement147 = default(ES3Parser.expressionStatement_return);

            ES3Parser.ifStatement_return ifStatement148 = default(ES3Parser.ifStatement_return);

            ES3Parser.iterationStatement_return iterationStatement149 = default(ES3Parser.iterationStatement_return);

            ES3Parser.continueStatement_return continueStatement150 = default(ES3Parser.continueStatement_return);

            ES3Parser.breakStatement_return breakStatement151 = default(ES3Parser.breakStatement_return);

            ES3Parser.returnStatement_return returnStatement152 = default(ES3Parser.returnStatement_return);

            ES3Parser.withStatement_return withStatement153 = default(ES3Parser.withStatement_return);

            ES3Parser.labelledStatement_return labelledStatement154 = default(ES3Parser.labelledStatement_return);

            ES3Parser.switchStatement_return switchStatement155 = default(ES3Parser.switchStatement_return);

            ES3Parser.throwStatement_return throwStatement156 = default(ES3Parser.throwStatement_return);

            ES3Parser.tryStatement_return tryStatement157 = default(ES3Parser.tryStatement_return);



            try
            {
                // ES3.g3:966:2: ( variableStatement | emptyStatement | expressionStatement | ifStatement | iterationStatement | continueStatement | breakStatement | returnStatement | withStatement | labelledStatement | switchStatement | throwStatement | tryStatement )
                int alt44 = 13;
                alt44 = dfa44.Predict(input);
                switch (alt44)
                {
                    case 1:
                        // ES3.g3:966:4: variableStatement
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_variableStatement_in_statementTail4525);
                            variableStatement145 = variableStatement();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, variableStatement145.Tree);

                        }
                        break;
                    case 2:
                        // ES3.g3:967:4: emptyStatement
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_emptyStatement_in_statementTail4530);
                            emptyStatement146 = emptyStatement();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, emptyStatement146.Tree);

                        }
                        break;
                    case 3:
                        // ES3.g3:968:4: expressionStatement
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_expressionStatement_in_statementTail4535);
                            expressionStatement147 = expressionStatement();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, expressionStatement147.Tree);

                        }
                        break;
                    case 4:
                        // ES3.g3:969:4: ifStatement
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_ifStatement_in_statementTail4540);
                            ifStatement148 = ifStatement();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, ifStatement148.Tree);

                        }
                        break;
                    case 5:
                        // ES3.g3:970:4: iterationStatement
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_iterationStatement_in_statementTail4545);
                            iterationStatement149 = iterationStatement();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, iterationStatement149.Tree);

                        }
                        break;
                    case 6:
                        // ES3.g3:971:4: continueStatement
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_continueStatement_in_statementTail4550);
                            continueStatement150 = continueStatement();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, continueStatement150.Tree);

                        }
                        break;
                    case 7:
                        // ES3.g3:972:4: breakStatement
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_breakStatement_in_statementTail4555);
                            breakStatement151 = breakStatement();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, breakStatement151.Tree);

                        }
                        break;
                    case 8:
                        // ES3.g3:973:4: returnStatement
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_returnStatement_in_statementTail4560);
                            returnStatement152 = returnStatement();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, returnStatement152.Tree);

                        }
                        break;
                    case 9:
                        // ES3.g3:974:4: withStatement
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_withStatement_in_statementTail4565);
                            withStatement153 = withStatement();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, withStatement153.Tree);

                        }
                        break;
                    case 10:
                        // ES3.g3:975:4: labelledStatement
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_labelledStatement_in_statementTail4570);
                            labelledStatement154 = labelledStatement();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, labelledStatement154.Tree);

                        }
                        break;
                    case 11:
                        // ES3.g3:976:4: switchStatement
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_switchStatement_in_statementTail4575);
                            switchStatement155 = switchStatement();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, switchStatement155.Tree);

                        }
                        break;
                    case 12:
                        // ES3.g3:977:4: throwStatement
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_throwStatement_in_statementTail4580);
                            throwStatement156 = throwStatement();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, throwStatement156.Tree);

                        }
                        break;
                    case 13:
                        // ES3.g3:978:4: tryStatement
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_tryStatement_in_statementTail4585);
                            tryStatement157 = tryStatement();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, tryStatement157.Tree);

                        }
                        break;

                }
                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "statementTail"

        public class block_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "block"
        // ES3.g3:983:1: block : lb= LBRACE ( statement )* RBRACE -> ^( BLOCK[$lb, \"BLOCK\"] ( statement )* ) ;
        public ES3Parser.block_return block() // throws RecognitionException [1]
        {
            ES3Parser.block_return retval = new ES3Parser.block_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken lb = null;
            IToken RBRACE159 = null;
            ES3Parser.statement_return statement158 = default(ES3Parser.statement_return);


            object lb_tree = null;
            object RBRACE159_tree = null;
            RewriteRuleTokenStream stream_RBRACE = new RewriteRuleTokenStream(adaptor, "token RBRACE");
            RewriteRuleTokenStream stream_LBRACE = new RewriteRuleTokenStream(adaptor, "token LBRACE");
            RewriteRuleSubtreeStream stream_statement = new RewriteRuleSubtreeStream(adaptor, "rule statement");
            try
            {
                // ES3.g3:984:2: (lb= LBRACE ( statement )* RBRACE -> ^( BLOCK[$lb, \"BLOCK\"] ( statement )* ) )
                // ES3.g3:984:4: lb= LBRACE ( statement )* RBRACE
                {
                    lb = (IToken)Match(input, LBRACE, FOLLOW_LBRACE_in_block4600);
                    stream_LBRACE.Add(lb);

                    // ES3.g3:984:14: ( statement )*
                    do
                    {
                        int alt45 = 2;
                        int LA45_0 = input.LA(1);

                        if (((LA45_0 >= NULL && LA45_0 <= BREAK) || LA45_0 == CONTINUE || (LA45_0 >= DELETE && LA45_0 <= DO) || (LA45_0 >= FOR && LA45_0 <= IF) || (LA45_0 >= NEW && LA45_0 <= WITH) || LA45_0 == LBRACE || LA45_0 == LPAREN || LA45_0 == LBRACK || LA45_0 == SEMIC || (LA45_0 >= ADD && LA45_0 <= SUB) || (LA45_0 >= INC && LA45_0 <= DEC) || (LA45_0 >= NOT && LA45_0 <= INV) || (LA45_0 >= Identifier && LA45_0 <= StringLiteral) || LA45_0 == RegularExpressionLiteral || (LA45_0 >= DecimalLiteral && LA45_0 <= HexIntegerLiteral)))
                        {
                            alt45 = 1;
                        }


                        switch (alt45)
                        {
                            case 1:
                                // ES3.g3:984:14: statement
                                {
                                    PushFollow(FOLLOW_statement_in_block4602);
                                    statement158 = statement();
                                    state.followingStackPointer--;

                                    stream_statement.Add(statement158.Tree);

                                }
                                break;

                            default:
                                goto loop45;
                        }
                    } while (true);

                loop45:
                    ;	// Stops C# compiler whining that label 'loop45' has no statements

                    RBRACE159 = (IToken)Match(input, RBRACE, FOLLOW_RBRACE_in_block4605);
                    stream_RBRACE.Add(RBRACE159);



                    // AST REWRITE
                    // elements:          statement
                    // token labels:      
                    // rule labels:       retval
                    // token list labels: 
                    // rule list labels:  
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                    root_0 = (object)adaptor.GetNilNode();
                    // 985:2: -> ^( BLOCK[$lb, \"BLOCK\"] ( statement )* )
                    {
                        // ES3.g3:985:5: ^( BLOCK[$lb, \"BLOCK\"] ( statement )* )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(BLOCK, lb, "BLOCK"), root_1);

                            // ES3.g3:985:28: ( statement )*
                            while (stream_statement.HasNext())
                            {
                                adaptor.AddChild(root_1, stream_statement.NextTree());

                            }
                            stream_statement.Reset();

                            adaptor.AddChild(root_0, root_1);
                        }

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "block"

        public class variableStatement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "variableStatement"
        // ES3.g3:992:1: variableStatement : VAR variableDeclaration ( COMMA variableDeclaration )* semic -> ^( VAR ( variableDeclaration )+ ) ;
        public ES3Parser.variableStatement_return variableStatement() // throws RecognitionException [1]
        {
            ES3Parser.variableStatement_return retval = new ES3Parser.variableStatement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken VAR160 = null;
            IToken COMMA162 = null;
            ES3Parser.variableDeclaration_return variableDeclaration161 = default(ES3Parser.variableDeclaration_return);

            ES3Parser.variableDeclaration_return variableDeclaration163 = default(ES3Parser.variableDeclaration_return);

            ES3Parser.semic_return semic164 = default(ES3Parser.semic_return);


            object VAR160_tree = null;
            object COMMA162_tree = null;
            RewriteRuleTokenStream stream_VAR = new RewriteRuleTokenStream(adaptor, "token VAR");
            RewriteRuleTokenStream stream_COMMA = new RewriteRuleTokenStream(adaptor, "token COMMA");
            RewriteRuleSubtreeStream stream_variableDeclaration = new RewriteRuleSubtreeStream(adaptor, "rule variableDeclaration");
            RewriteRuleSubtreeStream stream_semic = new RewriteRuleSubtreeStream(adaptor, "rule semic");
            try
            {
                // ES3.g3:993:2: ( VAR variableDeclaration ( COMMA variableDeclaration )* semic -> ^( VAR ( variableDeclaration )+ ) )
                // ES3.g3:993:4: VAR variableDeclaration ( COMMA variableDeclaration )* semic
                {
                    VAR160 = (IToken)Match(input, VAR, FOLLOW_VAR_in_variableStatement4634);
                    stream_VAR.Add(VAR160);

                    PushFollow(FOLLOW_variableDeclaration_in_variableStatement4636);
                    variableDeclaration161 = variableDeclaration();
                    state.followingStackPointer--;

                    stream_variableDeclaration.Add(variableDeclaration161.Tree);
                    // ES3.g3:993:28: ( COMMA variableDeclaration )*
                    do
                    {
                        int alt46 = 2;
                        int LA46_0 = input.LA(1);

                        if ((LA46_0 == COMMA))
                        {
                            alt46 = 1;
                        }


                        switch (alt46)
                        {
                            case 1:
                                // ES3.g3:993:30: COMMA variableDeclaration
                                {
                                    COMMA162 = (IToken)Match(input, COMMA, FOLLOW_COMMA_in_variableStatement4640);
                                    stream_COMMA.Add(COMMA162);

                                    PushFollow(FOLLOW_variableDeclaration_in_variableStatement4642);
                                    variableDeclaration163 = variableDeclaration();
                                    state.followingStackPointer--;

                                    stream_variableDeclaration.Add(variableDeclaration163.Tree);

                                }
                                break;

                            default:
                                goto loop46;
                        }
                    } while (true);

                loop46:
                    ;	// Stops C# compiler whining that label 'loop46' has no statements

                    PushFollow(FOLLOW_semic_in_variableStatement4647);
                    semic164 = semic();
                    state.followingStackPointer--;

                    stream_semic.Add(semic164.Tree);


                    // AST REWRITE
                    // elements:          variableDeclaration, VAR
                    // token labels:      
                    // rule labels:       retval
                    // token list labels: 
                    // rule list labels:  
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                    root_0 = (object)adaptor.GetNilNode();
                    // 994:2: -> ^( VAR ( variableDeclaration )+ )
                    {
                        // ES3.g3:994:5: ^( VAR ( variableDeclaration )+ )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot(stream_VAR.NextNode(), root_1);

                            if (!(stream_variableDeclaration.HasNext()))
                            {
                                throw new RewriteEarlyExitException();
                            }
                            while (stream_variableDeclaration.HasNext())
                            {
                                adaptor.AddChild(root_1, stream_variableDeclaration.NextTree());

                            }
                            stream_variableDeclaration.Reset();

                            adaptor.AddChild(root_0, root_1);
                        }

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "variableStatement"

        public class variableDeclaration_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "variableDeclaration"
        // ES3.g3:997:1: variableDeclaration : Identifier ( ASSIGN assignmentExpression )? ;
        public ES3Parser.variableDeclaration_return variableDeclaration() // throws RecognitionException [1]
        {
            ES3Parser.variableDeclaration_return retval = new ES3Parser.variableDeclaration_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken Identifier165 = null;
            IToken ASSIGN166 = null;
            ES3Parser.assignmentExpression_return assignmentExpression167 = default(ES3Parser.assignmentExpression_return);


            object Identifier165_tree = null;
            object ASSIGN166_tree = null;

            try
            {
                // ES3.g3:998:2: ( Identifier ( ASSIGN assignmentExpression )? )
                // ES3.g3:998:4: Identifier ( ASSIGN assignmentExpression )?
                {
                    root_0 = (object)adaptor.GetNilNode();

                    Identifier165 = (IToken)Match(input, Identifier, FOLLOW_Identifier_in_variableDeclaration4670);
                    Identifier165_tree = (object)adaptor.Create(Identifier165);
                    adaptor.AddChild(root_0, Identifier165_tree);

                    // ES3.g3:998:15: ( ASSIGN assignmentExpression )?
                    int alt47 = 2;
                    int LA47_0 = input.LA(1);

                    if ((LA47_0 == ASSIGN))
                    {
                        alt47 = 1;
                    }
                    switch (alt47)
                    {
                        case 1:
                            // ES3.g3:998:17: ASSIGN assignmentExpression
                            {
                                ASSIGN166 = (IToken)Match(input, ASSIGN, FOLLOW_ASSIGN_in_variableDeclaration4674);
                                ASSIGN166_tree = (object)adaptor.Create(ASSIGN166);
                                root_0 = (object)adaptor.BecomeRoot(ASSIGN166_tree, root_0);

                                PushFollow(FOLLOW_assignmentExpression_in_variableDeclaration4677);
                                assignmentExpression167 = assignmentExpression();
                                state.followingStackPointer--;

                                adaptor.AddChild(root_0, assignmentExpression167.Tree);

                            }
                            break;

                    }


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "variableDeclaration"

        public class variableDeclarationNoIn_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "variableDeclarationNoIn"
        // ES3.g3:1001:1: variableDeclarationNoIn : Identifier ( ASSIGN assignmentExpressionNoIn )? ;
        public ES3Parser.variableDeclarationNoIn_return variableDeclarationNoIn() // throws RecognitionException [1]
        {
            ES3Parser.variableDeclarationNoIn_return retval = new ES3Parser.variableDeclarationNoIn_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken Identifier168 = null;
            IToken ASSIGN169 = null;
            ES3Parser.assignmentExpressionNoIn_return assignmentExpressionNoIn170 = default(ES3Parser.assignmentExpressionNoIn_return);


            object Identifier168_tree = null;
            object ASSIGN169_tree = null;

            try
            {
                // ES3.g3:1002:2: ( Identifier ( ASSIGN assignmentExpressionNoIn )? )
                // ES3.g3:1002:4: Identifier ( ASSIGN assignmentExpressionNoIn )?
                {
                    root_0 = (object)adaptor.GetNilNode();

                    Identifier168 = (IToken)Match(input, Identifier, FOLLOW_Identifier_in_variableDeclarationNoIn4692);
                    Identifier168_tree = (object)adaptor.Create(Identifier168);
                    adaptor.AddChild(root_0, Identifier168_tree);

                    // ES3.g3:1002:15: ( ASSIGN assignmentExpressionNoIn )?
                    int alt48 = 2;
                    int LA48_0 = input.LA(1);

                    if ((LA48_0 == ASSIGN))
                    {
                        alt48 = 1;
                    }
                    switch (alt48)
                    {
                        case 1:
                            // ES3.g3:1002:17: ASSIGN assignmentExpressionNoIn
                            {
                                ASSIGN169 = (IToken)Match(input, ASSIGN, FOLLOW_ASSIGN_in_variableDeclarationNoIn4696);
                                ASSIGN169_tree = (object)adaptor.Create(ASSIGN169);
                                root_0 = (object)adaptor.BecomeRoot(ASSIGN169_tree, root_0);

                                PushFollow(FOLLOW_assignmentExpressionNoIn_in_variableDeclarationNoIn4699);
                                assignmentExpressionNoIn170 = assignmentExpressionNoIn();
                                state.followingStackPointer--;

                                adaptor.AddChild(root_0, assignmentExpressionNoIn170.Tree);

                            }
                            break;

                    }


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "variableDeclarationNoIn"

        public class emptyStatement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "emptyStatement"
        // ES3.g3:1009:1: emptyStatement : SEMIC ;
        public ES3Parser.emptyStatement_return emptyStatement() // throws RecognitionException [1]
        {
            ES3Parser.emptyStatement_return retval = new ES3Parser.emptyStatement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken SEMIC171 = null;

            object SEMIC171_tree = null;

            try
            {
                // ES3.g3:1010:2: ( SEMIC )
                // ES3.g3:1010:4: SEMIC
                {
                    root_0 = (object)adaptor.GetNilNode();

                    SEMIC171 = (IToken)Match(input, SEMIC, FOLLOW_SEMIC_in_emptyStatement4718);

                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "emptyStatement"

        public class expressionStatement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "expressionStatement"
        // ES3.g3:1023:1: expressionStatement : expression semic ;
        public ES3Parser.expressionStatement_return expressionStatement() // throws RecognitionException [1]
        {
            ES3Parser.expressionStatement_return retval = new ES3Parser.expressionStatement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            ES3Parser.expression_return expression172 = default(ES3Parser.expression_return);

            ES3Parser.semic_return semic173 = default(ES3Parser.semic_return);



            try
            {
                // ES3.g3:1024:2: ( expression semic )
                // ES3.g3:1024:4: expression semic
                {
                    root_0 = (object)adaptor.GetNilNode();

                    PushFollow(FOLLOW_expression_in_expressionStatement4737);
                    expression172 = expression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, expression172.Tree);
                    PushFollow(FOLLOW_semic_in_expressionStatement4739);
                    semic173 = semic();
                    state.followingStackPointer--;


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "expressionStatement"

        public class ifStatement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "ifStatement"
        // ES3.g3:1031:1: ifStatement : IF LPAREN expression RPAREN statement ({...}? ELSE statement )? -> ^( IF expression ( statement )+ ) ;
        public ES3Parser.ifStatement_return ifStatement() // throws RecognitionException [1]
        {
            ES3Parser.ifStatement_return retval = new ES3Parser.ifStatement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken IF174 = null;
            IToken LPAREN175 = null;
            IToken RPAREN177 = null;
            IToken ELSE179 = null;
            ES3Parser.expression_return expression176 = default(ES3Parser.expression_return);

            ES3Parser.statement_return statement178 = default(ES3Parser.statement_return);

            ES3Parser.statement_return statement180 = default(ES3Parser.statement_return);


            object IF174_tree = null;
            object LPAREN175_tree = null;
            object RPAREN177_tree = null;
            object ELSE179_tree = null;
            RewriteRuleTokenStream stream_RPAREN = new RewriteRuleTokenStream(adaptor, "token RPAREN");
            RewriteRuleTokenStream stream_LPAREN = new RewriteRuleTokenStream(adaptor, "token LPAREN");
            RewriteRuleTokenStream stream_IF = new RewriteRuleTokenStream(adaptor, "token IF");
            RewriteRuleTokenStream stream_ELSE = new RewriteRuleTokenStream(adaptor, "token ELSE");
            RewriteRuleSubtreeStream stream_expression = new RewriteRuleSubtreeStream(adaptor, "rule expression");
            RewriteRuleSubtreeStream stream_statement = new RewriteRuleSubtreeStream(adaptor, "rule statement");
            try
            {
                // ES3.g3:1033:2: ( IF LPAREN expression RPAREN statement ({...}? ELSE statement )? -> ^( IF expression ( statement )+ ) )
                // ES3.g3:1033:4: IF LPAREN expression RPAREN statement ({...}? ELSE statement )?
                {
                    IF174 = (IToken)Match(input, IF, FOLLOW_IF_in_ifStatement4757);
                    stream_IF.Add(IF174);

                    LPAREN175 = (IToken)Match(input, LPAREN, FOLLOW_LPAREN_in_ifStatement4759);
                    stream_LPAREN.Add(LPAREN175);

                    PushFollow(FOLLOW_expression_in_ifStatement4761);
                    expression176 = expression();
                    state.followingStackPointer--;

                    stream_expression.Add(expression176.Tree);
                    RPAREN177 = (IToken)Match(input, RPAREN, FOLLOW_RPAREN_in_ifStatement4763);
                    stream_RPAREN.Add(RPAREN177);

                    PushFollow(FOLLOW_statement_in_ifStatement4765);
                    statement178 = statement();
                    state.followingStackPointer--;

                    stream_statement.Add(statement178.Tree);
                    // ES3.g3:1033:42: ({...}? ELSE statement )?
                    int alt49 = 2;
                    int LA49_0 = input.LA(1);

                    if ((LA49_0 == ELSE))
                    {
                        int LA49_1 = input.LA(2);

                        if (((input.LA(1) == ELSE)))
                        {
                            alt49 = 1;
                        }
                    }
                    switch (alt49)
                    {
                        case 1:
                            // ES3.g3:1033:44: {...}? ELSE statement
                            {
                                if (!((input.LA(1) == ELSE)))
                                {
                                    throw new FailedPredicateException(input, "ifStatement", " input.LA(1) == ELSE ");
                                }
                                ELSE179 = (IToken)Match(input, ELSE, FOLLOW_ELSE_in_ifStatement4771);
                                stream_ELSE.Add(ELSE179);

                                PushFollow(FOLLOW_statement_in_ifStatement4773);
                                statement180 = statement();
                                state.followingStackPointer--;

                                stream_statement.Add(statement180.Tree);

                            }
                            break;

                    }



                    // AST REWRITE
                    // elements:          expression, statement, IF
                    // token labels:      
                    // rule labels:       retval
                    // token list labels: 
                    // rule list labels:  
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                    root_0 = (object)adaptor.GetNilNode();
                    // 1034:2: -> ^( IF expression ( statement )+ )
                    {
                        // ES3.g3:1034:5: ^( IF expression ( statement )+ )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot(stream_IF.NextNode(), root_1);

                            adaptor.AddChild(root_1, stream_expression.NextTree());
                            if (!(stream_statement.HasNext()))
                            {
                                throw new RewriteEarlyExitException();
                            }
                            while (stream_statement.HasNext())
                            {
                                adaptor.AddChild(root_1, stream_statement.NextTree());

                            }
                            stream_statement.Reset();

                            adaptor.AddChild(root_0, root_1);
                        }

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "ifStatement"

        public class iterationStatement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "iterationStatement"
        // ES3.g3:1041:1: iterationStatement : ( doStatement | whileStatement | forStatement );
        public ES3Parser.iterationStatement_return iterationStatement() // throws RecognitionException [1]
        {
            ES3Parser.iterationStatement_return retval = new ES3Parser.iterationStatement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            ES3Parser.doStatement_return doStatement181 = default(ES3Parser.doStatement_return);

            ES3Parser.whileStatement_return whileStatement182 = default(ES3Parser.whileStatement_return);

            ES3Parser.forStatement_return forStatement183 = default(ES3Parser.forStatement_return);



            try
            {
                // ES3.g3:1042:2: ( doStatement | whileStatement | forStatement )
                int alt50 = 3;
                switch (input.LA(1))
                {
                    case DO:
                        {
                            alt50 = 1;
                        }
                        break;
                    case WHILE:
                        {
                            alt50 = 2;
                        }
                        break;
                    case FOR:
                        {
                            alt50 = 3;
                        }
                        break;
                    default:
                        NoViableAltException nvae_d50s0 =
                            new NoViableAltException("", 50, 0, input);

                        throw nvae_d50s0;
                }

                switch (alt50)
                {
                    case 1:
                        // ES3.g3:1042:4: doStatement
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_doStatement_in_iterationStatement4806);
                            doStatement181 = doStatement();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, doStatement181.Tree);

                        }
                        break;
                    case 2:
                        // ES3.g3:1043:4: whileStatement
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_whileStatement_in_iterationStatement4811);
                            whileStatement182 = whileStatement();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, whileStatement182.Tree);

                        }
                        break;
                    case 3:
                        // ES3.g3:1044:4: forStatement
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_forStatement_in_iterationStatement4816);
                            forStatement183 = forStatement();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, forStatement183.Tree);

                        }
                        break;

                }
                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "iterationStatement"

        public class doStatement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "doStatement"
        // ES3.g3:1047:1: doStatement : DO statement WHILE LPAREN expression RPAREN semic -> ^( DO statement expression ) ;
        public ES3Parser.doStatement_return doStatement() // throws RecognitionException [1]
        {
            ES3Parser.doStatement_return retval = new ES3Parser.doStatement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken DO184 = null;
            IToken WHILE186 = null;
            IToken LPAREN187 = null;
            IToken RPAREN189 = null;
            ES3Parser.statement_return statement185 = default(ES3Parser.statement_return);

            ES3Parser.expression_return expression188 = default(ES3Parser.expression_return);

            ES3Parser.semic_return semic190 = default(ES3Parser.semic_return);


            object DO184_tree = null;
            object WHILE186_tree = null;
            object LPAREN187_tree = null;
            object RPAREN189_tree = null;
            RewriteRuleTokenStream stream_DO = new RewriteRuleTokenStream(adaptor, "token DO");
            RewriteRuleTokenStream stream_RPAREN = new RewriteRuleTokenStream(adaptor, "token RPAREN");
            RewriteRuleTokenStream stream_WHILE = new RewriteRuleTokenStream(adaptor, "token WHILE");
            RewriteRuleTokenStream stream_LPAREN = new RewriteRuleTokenStream(adaptor, "token LPAREN");
            RewriteRuleSubtreeStream stream_statement = new RewriteRuleSubtreeStream(adaptor, "rule statement");
            RewriteRuleSubtreeStream stream_expression = new RewriteRuleSubtreeStream(adaptor, "rule expression");
            RewriteRuleSubtreeStream stream_semic = new RewriteRuleSubtreeStream(adaptor, "rule semic");
            try
            {
                // ES3.g3:1048:2: ( DO statement WHILE LPAREN expression RPAREN semic -> ^( DO statement expression ) )
                // ES3.g3:1048:4: DO statement WHILE LPAREN expression RPAREN semic
                {
                    DO184 = (IToken)Match(input, DO, FOLLOW_DO_in_doStatement4828);
                    stream_DO.Add(DO184);

                    PushFollow(FOLLOW_statement_in_doStatement4830);
                    statement185 = statement();
                    state.followingStackPointer--;

                    stream_statement.Add(statement185.Tree);
                    WHILE186 = (IToken)Match(input, WHILE, FOLLOW_WHILE_in_doStatement4832);
                    stream_WHILE.Add(WHILE186);

                    LPAREN187 = (IToken)Match(input, LPAREN, FOLLOW_LPAREN_in_doStatement4834);
                    stream_LPAREN.Add(LPAREN187);

                    PushFollow(FOLLOW_expression_in_doStatement4836);
                    expression188 = expression();
                    state.followingStackPointer--;

                    stream_expression.Add(expression188.Tree);
                    RPAREN189 = (IToken)Match(input, RPAREN, FOLLOW_RPAREN_in_doStatement4838);
                    stream_RPAREN.Add(RPAREN189);

                    PushFollow(FOLLOW_semic_in_doStatement4840);
                    semic190 = semic();
                    state.followingStackPointer--;

                    stream_semic.Add(semic190.Tree);


                    // AST REWRITE
                    // elements:          expression, statement, DO
                    // token labels:      
                    // rule labels:       retval
                    // token list labels: 
                    // rule list labels:  
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                    root_0 = (object)adaptor.GetNilNode();
                    // 1049:2: -> ^( DO statement expression )
                    {
                        // ES3.g3:1049:5: ^( DO statement expression )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot(stream_DO.NextNode(), root_1);

                            adaptor.AddChild(root_1, stream_statement.NextTree());
                            adaptor.AddChild(root_1, stream_expression.NextTree());

                            adaptor.AddChild(root_0, root_1);
                        }

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "doStatement"

        public class whileStatement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "whileStatement"
        // ES3.g3:1052:1: whileStatement : WHILE LPAREN expression RPAREN statement ;
        public ES3Parser.whileStatement_return whileStatement() // throws RecognitionException [1]
        {
            ES3Parser.whileStatement_return retval = new ES3Parser.whileStatement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken WHILE191 = null;
            IToken LPAREN192 = null;
            IToken RPAREN194 = null;
            ES3Parser.expression_return expression193 = default(ES3Parser.expression_return);

            ES3Parser.statement_return statement195 = default(ES3Parser.statement_return);


            object WHILE191_tree = null;
            object LPAREN192_tree = null;
            object RPAREN194_tree = null;

            try
            {
                // ES3.g3:1053:2: ( WHILE LPAREN expression RPAREN statement )
                // ES3.g3:1053:4: WHILE LPAREN expression RPAREN statement
                {
                    root_0 = (object)adaptor.GetNilNode();

                    WHILE191 = (IToken)Match(input, WHILE, FOLLOW_WHILE_in_whileStatement4865);
                    WHILE191_tree = (object)adaptor.Create(WHILE191);
                    root_0 = (object)adaptor.BecomeRoot(WHILE191_tree, root_0);

                    LPAREN192 = (IToken)Match(input, LPAREN, FOLLOW_LPAREN_in_whileStatement4868);
                    PushFollow(FOLLOW_expression_in_whileStatement4871);
                    expression193 = expression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, expression193.Tree);
                    RPAREN194 = (IToken)Match(input, RPAREN, FOLLOW_RPAREN_in_whileStatement4873);
                    PushFollow(FOLLOW_statement_in_whileStatement4876);
                    statement195 = statement();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, statement195.Tree);

                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "whileStatement"

        public class forStatement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "forStatement"
        // ES3.g3:1097:1: forStatement : FOR LPAREN forControl RPAREN statement ;
        public ES3Parser.forStatement_return forStatement() // throws RecognitionException [1]
        {
            ES3Parser.forStatement_return retval = new ES3Parser.forStatement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken FOR196 = null;
            IToken LPAREN197 = null;
            IToken RPAREN199 = null;
            ES3Parser.forControl_return forControl198 = default(ES3Parser.forControl_return);

            ES3Parser.statement_return statement200 = default(ES3Parser.statement_return);


            object FOR196_tree = null;
            object LPAREN197_tree = null;
            object RPAREN199_tree = null;

            try
            {
                // ES3.g3:1098:2: ( FOR LPAREN forControl RPAREN statement )
                // ES3.g3:1098:4: FOR LPAREN forControl RPAREN statement
                {
                    root_0 = (object)adaptor.GetNilNode();

                    FOR196 = (IToken)Match(input, FOR, FOLLOW_FOR_in_forStatement4889);
                    FOR196_tree = (object)adaptor.Create(FOR196);
                    root_0 = (object)adaptor.BecomeRoot(FOR196_tree, root_0);

                    LPAREN197 = (IToken)Match(input, LPAREN, FOLLOW_LPAREN_in_forStatement4892);
                    PushFollow(FOLLOW_forControl_in_forStatement4895);
                    forControl198 = forControl();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, forControl198.Tree);
                    RPAREN199 = (IToken)Match(input, RPAREN, FOLLOW_RPAREN_in_forStatement4897);
                    PushFollow(FOLLOW_statement_in_forStatement4900);
                    statement200 = statement();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, statement200.Tree);

                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "forStatement"

        public class forControl_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "forControl"
        // ES3.g3:1101:1: forControl : ( forControlVar | forControlExpression | forControlSemic );
        public ES3Parser.forControl_return forControl() // throws RecognitionException [1]
        {
            ES3Parser.forControl_return retval = new ES3Parser.forControl_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            ES3Parser.forControlVar_return forControlVar201 = default(ES3Parser.forControlVar_return);

            ES3Parser.forControlExpression_return forControlExpression202 = default(ES3Parser.forControlExpression_return);

            ES3Parser.forControlSemic_return forControlSemic203 = default(ES3Parser.forControlSemic_return);



            try
            {
                // ES3.g3:1102:2: ( forControlVar | forControlExpression | forControlSemic )
                int alt51 = 3;
                switch (input.LA(1))
                {
                    case VAR:
                        {
                            alt51 = 1;
                        }
                        break;
                    case NULL:
                    case TRUE:
                    case FALSE:
                    case DELETE:
                    case FUNCTION:
                    case NEW:
                    case THIS:
                    case TYPEOF:
                    case VOID:
                    case LBRACE:
                    case LPAREN:
                    case LBRACK:
                    case ADD:
                    case SUB:
                    case INC:
                    case DEC:
                    case NOT:
                    case INV:
                    case Identifier:
                    case StringLiteral:
                    case RegularExpressionLiteral:
                    case DecimalLiteral:
                    case OctalIntegerLiteral:
                    case HexIntegerLiteral:
                        {
                            alt51 = 2;
                        }
                        break;
                    case SEMIC:
                        {
                            alt51 = 3;
                        }
                        break;
                    default:
                        NoViableAltException nvae_d51s0 =
                            new NoViableAltException("", 51, 0, input);

                        throw nvae_d51s0;
                }

                switch (alt51)
                {
                    case 1:
                        // ES3.g3:1102:4: forControlVar
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_forControlVar_in_forControl4911);
                            forControlVar201 = forControlVar();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, forControlVar201.Tree);

                        }
                        break;
                    case 2:
                        // ES3.g3:1103:4: forControlExpression
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_forControlExpression_in_forControl4916);
                            forControlExpression202 = forControlExpression();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, forControlExpression202.Tree);

                        }
                        break;
                    case 3:
                        // ES3.g3:1104:4: forControlSemic
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_forControlSemic_in_forControl4921);
                            forControlSemic203 = forControlSemic();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, forControlSemic203.Tree);

                        }
                        break;

                }
                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "forControl"

        public class forControlVar_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "forControlVar"
        // ES3.g3:1107:1: forControlVar : VAR variableDeclarationNoIn ( ( IN expression -> ^( FORITER ^( VAR variableDeclarationNoIn ) ^( EXPR expression ) ) ) | ( ( COMMA variableDeclarationNoIn )* SEMIC (ex1= expression )? SEMIC (ex2= expression )? -> ^( FORSTEP ^( VAR ( variableDeclarationNoIn )+ ) ^( EXPR ( $ex1)? ) ^( EXPR ( $ex2)? ) ) ) ) ;
        public ES3Parser.forControlVar_return forControlVar() // throws RecognitionException [1]
        {
            ES3Parser.forControlVar_return retval = new ES3Parser.forControlVar_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken VAR204 = null;
            IToken IN206 = null;
            IToken COMMA208 = null;
            IToken SEMIC210 = null;
            IToken SEMIC211 = null;
            ES3Parser.expression_return ex1 = default(ES3Parser.expression_return);

            ES3Parser.expression_return ex2 = default(ES3Parser.expression_return);

            ES3Parser.variableDeclarationNoIn_return variableDeclarationNoIn205 = default(ES3Parser.variableDeclarationNoIn_return);

            ES3Parser.expression_return expression207 = default(ES3Parser.expression_return);

            ES3Parser.variableDeclarationNoIn_return variableDeclarationNoIn209 = default(ES3Parser.variableDeclarationNoIn_return);


            object VAR204_tree = null;
            object IN206_tree = null;
            object COMMA208_tree = null;
            object SEMIC210_tree = null;
            object SEMIC211_tree = null;
            RewriteRuleTokenStream stream_VAR = new RewriteRuleTokenStream(adaptor, "token VAR");
            RewriteRuleTokenStream stream_IN = new RewriteRuleTokenStream(adaptor, "token IN");
            RewriteRuleTokenStream stream_SEMIC = new RewriteRuleTokenStream(adaptor, "token SEMIC");
            RewriteRuleTokenStream stream_COMMA = new RewriteRuleTokenStream(adaptor, "token COMMA");
            RewriteRuleSubtreeStream stream_expression = new RewriteRuleSubtreeStream(adaptor, "rule expression");
            RewriteRuleSubtreeStream stream_variableDeclarationNoIn = new RewriteRuleSubtreeStream(adaptor, "rule variableDeclarationNoIn");
            try
            {
                // ES3.g3:1108:2: ( VAR variableDeclarationNoIn ( ( IN expression -> ^( FORITER ^( VAR variableDeclarationNoIn ) ^( EXPR expression ) ) ) | ( ( COMMA variableDeclarationNoIn )* SEMIC (ex1= expression )? SEMIC (ex2= expression )? -> ^( FORSTEP ^( VAR ( variableDeclarationNoIn )+ ) ^( EXPR ( $ex1)? ) ^( EXPR ( $ex2)? ) ) ) ) )
                // ES3.g3:1108:4: VAR variableDeclarationNoIn ( ( IN expression -> ^( FORITER ^( VAR variableDeclarationNoIn ) ^( EXPR expression ) ) ) | ( ( COMMA variableDeclarationNoIn )* SEMIC (ex1= expression )? SEMIC (ex2= expression )? -> ^( FORSTEP ^( VAR ( variableDeclarationNoIn )+ ) ^( EXPR ( $ex1)? ) ^( EXPR ( $ex2)? ) ) ) )
                {
                    VAR204 = (IToken)Match(input, VAR, FOLLOW_VAR_in_forControlVar4932);
                    stream_VAR.Add(VAR204);

                    PushFollow(FOLLOW_variableDeclarationNoIn_in_forControlVar4934);
                    variableDeclarationNoIn205 = variableDeclarationNoIn();
                    state.followingStackPointer--;

                    stream_variableDeclarationNoIn.Add(variableDeclarationNoIn205.Tree);
                    // ES3.g3:1109:2: ( ( IN expression -> ^( FORITER ^( VAR variableDeclarationNoIn ) ^( EXPR expression ) ) ) | ( ( COMMA variableDeclarationNoIn )* SEMIC (ex1= expression )? SEMIC (ex2= expression )? -> ^( FORSTEP ^( VAR ( variableDeclarationNoIn )+ ) ^( EXPR ( $ex1)? ) ^( EXPR ( $ex2)? ) ) ) )
                    int alt55 = 2;
                    int LA55_0 = input.LA(1);

                    if ((LA55_0 == IN))
                    {
                        alt55 = 1;
                    }
                    else if (((LA55_0 >= SEMIC && LA55_0 <= COMMA)))
                    {
                        alt55 = 2;
                    }
                    else
                    {
                        NoViableAltException nvae_d55s0 =
                            new NoViableAltException("", 55, 0, input);

                        throw nvae_d55s0;
                    }
                    switch (alt55)
                    {
                        case 1:
                            // ES3.g3:1110:3: ( IN expression -> ^( FORITER ^( VAR variableDeclarationNoIn ) ^( EXPR expression ) ) )
                            {
                                // ES3.g3:1110:3: ( IN expression -> ^( FORITER ^( VAR variableDeclarationNoIn ) ^( EXPR expression ) ) )
                                // ES3.g3:1111:4: IN expression
                                {
                                    IN206 = (IToken)Match(input, IN, FOLLOW_IN_in_forControlVar4946);
                                    stream_IN.Add(IN206);

                                    PushFollow(FOLLOW_expression_in_forControlVar4948);
                                    expression207 = expression();
                                    state.followingStackPointer--;

                                    stream_expression.Add(expression207.Tree);


                                    // AST REWRITE
                                    // elements:          expression, variableDeclarationNoIn, VAR
                                    // token labels:      
                                    // rule labels:       retval
                                    // token list labels: 
                                    // rule list labels:  
                                    // wildcard labels: 
                                    retval.Tree = root_0;
                                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                                    root_0 = (object)adaptor.GetNilNode();
                                    // 1112:4: -> ^( FORITER ^( VAR variableDeclarationNoIn ) ^( EXPR expression ) )
                                    {
                                        // ES3.g3:1112:7: ^( FORITER ^( VAR variableDeclarationNoIn ) ^( EXPR expression ) )
                                        {
                                            object root_1 = (object)adaptor.GetNilNode();
                                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(FORITER, "FORITER"), root_1);

                                            // ES3.g3:1112:18: ^( VAR variableDeclarationNoIn )
                                            {
                                                object root_2 = (object)adaptor.GetNilNode();
                                                root_2 = (object)adaptor.BecomeRoot(stream_VAR.NextNode(), root_2);

                                                adaptor.AddChild(root_2, stream_variableDeclarationNoIn.NextTree());

                                                adaptor.AddChild(root_1, root_2);
                                            }
                                            // ES3.g3:1112:51: ^( EXPR expression )
                                            {
                                                object root_2 = (object)adaptor.GetNilNode();
                                                root_2 = (object)adaptor.BecomeRoot((object)adaptor.Create(EXPR, "EXPR"), root_2);

                                                adaptor.AddChild(root_2, stream_expression.NextTree());

                                                adaptor.AddChild(root_1, root_2);
                                            }

                                            adaptor.AddChild(root_0, root_1);
                                        }

                                    }

                                    retval.Tree = root_0; retval.Tree = root_0;
                                }


                            }
                            break;
                        case 2:
                            // ES3.g3:1115:3: ( ( COMMA variableDeclarationNoIn )* SEMIC (ex1= expression )? SEMIC (ex2= expression )? -> ^( FORSTEP ^( VAR ( variableDeclarationNoIn )+ ) ^( EXPR ( $ex1)? ) ^( EXPR ( $ex2)? ) ) )
                            {
                                // ES3.g3:1115:3: ( ( COMMA variableDeclarationNoIn )* SEMIC (ex1= expression )? SEMIC (ex2= expression )? -> ^( FORSTEP ^( VAR ( variableDeclarationNoIn )+ ) ^( EXPR ( $ex1)? ) ^( EXPR ( $ex2)? ) ) )
                                // ES3.g3:1116:4: ( COMMA variableDeclarationNoIn )* SEMIC (ex1= expression )? SEMIC (ex2= expression )?
                                {
                                    // ES3.g3:1116:4: ( COMMA variableDeclarationNoIn )*
                                    do
                                    {
                                        int alt52 = 2;
                                        int LA52_0 = input.LA(1);

                                        if ((LA52_0 == COMMA))
                                        {
                                            alt52 = 1;
                                        }


                                        switch (alt52)
                                        {
                                            case 1:
                                                // ES3.g3:1116:6: COMMA variableDeclarationNoIn
                                                {
                                                    COMMA208 = (IToken)Match(input, COMMA, FOLLOW_COMMA_in_forControlVar4994);
                                                    stream_COMMA.Add(COMMA208);

                                                    PushFollow(FOLLOW_variableDeclarationNoIn_in_forControlVar4996);
                                                    variableDeclarationNoIn209 = variableDeclarationNoIn();
                                                    state.followingStackPointer--;

                                                    stream_variableDeclarationNoIn.Add(variableDeclarationNoIn209.Tree);

                                                }
                                                break;

                                            default:
                                                goto loop52;
                                        }
                                    } while (true);

                                loop52:
                                    ;	// Stops C# compiler whining that label 'loop52' has no statements

                                    SEMIC210 = (IToken)Match(input, SEMIC, FOLLOW_SEMIC_in_forControlVar5001);
                                    stream_SEMIC.Add(SEMIC210);

                                    // ES3.g3:1116:48: (ex1= expression )?
                                    int alt53 = 2;
                                    int LA53_0 = input.LA(1);

                                    if (((LA53_0 >= NULL && LA53_0 <= FALSE) || LA53_0 == DELETE || LA53_0 == FUNCTION || LA53_0 == NEW || LA53_0 == THIS || LA53_0 == TYPEOF || LA53_0 == VOID || LA53_0 == LBRACE || LA53_0 == LPAREN || LA53_0 == LBRACK || (LA53_0 >= ADD && LA53_0 <= SUB) || (LA53_0 >= INC && LA53_0 <= DEC) || (LA53_0 >= NOT && LA53_0 <= INV) || (LA53_0 >= Identifier && LA53_0 <= StringLiteral) || LA53_0 == RegularExpressionLiteral || (LA53_0 >= DecimalLiteral && LA53_0 <= HexIntegerLiteral)))
                                    {
                                        alt53 = 1;
                                    }
                                    switch (alt53)
                                    {
                                        case 1:
                                            // ES3.g3:1116:48: ex1= expression
                                            {
                                                PushFollow(FOLLOW_expression_in_forControlVar5005);
                                                ex1 = expression();
                                                state.followingStackPointer--;

                                                stream_expression.Add(ex1.Tree);

                                            }
                                            break;

                                    }

                                    SEMIC211 = (IToken)Match(input, SEMIC, FOLLOW_SEMIC_in_forControlVar5008);
                                    stream_SEMIC.Add(SEMIC211);

                                    // ES3.g3:1116:70: (ex2= expression )?
                                    int alt54 = 2;
                                    int LA54_0 = input.LA(1);

                                    if (((LA54_0 >= NULL && LA54_0 <= FALSE) || LA54_0 == DELETE || LA54_0 == FUNCTION || LA54_0 == NEW || LA54_0 == THIS || LA54_0 == TYPEOF || LA54_0 == VOID || LA54_0 == LBRACE || LA54_0 == LPAREN || LA54_0 == LBRACK || (LA54_0 >= ADD && LA54_0 <= SUB) || (LA54_0 >= INC && LA54_0 <= DEC) || (LA54_0 >= NOT && LA54_0 <= INV) || (LA54_0 >= Identifier && LA54_0 <= StringLiteral) || LA54_0 == RegularExpressionLiteral || (LA54_0 >= DecimalLiteral && LA54_0 <= HexIntegerLiteral)))
                                    {
                                        alt54 = 1;
                                    }
                                    switch (alt54)
                                    {
                                        case 1:
                                            // ES3.g3:1116:70: ex2= expression
                                            {
                                                PushFollow(FOLLOW_expression_in_forControlVar5012);
                                                ex2 = expression();
                                                state.followingStackPointer--;

                                                stream_expression.Add(ex2.Tree);

                                            }
                                            break;

                                    }



                                    // AST REWRITE
                                    // elements:          ex2, variableDeclarationNoIn, VAR, ex1
                                    // token labels:      
                                    // rule labels:       retval, ex2, ex1
                                    // token list labels: 
                                    // rule list labels:  
                                    // wildcard labels: 
                                    retval.Tree = root_0;
                                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);
                                    RewriteRuleSubtreeStream stream_ex2 = new RewriteRuleSubtreeStream(adaptor, "rule ex2", ex2 != null ? ex2.Tree : null);
                                    RewriteRuleSubtreeStream stream_ex1 = new RewriteRuleSubtreeStream(adaptor, "rule ex1", ex1 != null ? ex1.Tree : null);

                                    root_0 = (object)adaptor.GetNilNode();
                                    // 1117:4: -> ^( FORSTEP ^( VAR ( variableDeclarationNoIn )+ ) ^( EXPR ( $ex1)? ) ^( EXPR ( $ex2)? ) )
                                    {
                                        // ES3.g3:1117:7: ^( FORSTEP ^( VAR ( variableDeclarationNoIn )+ ) ^( EXPR ( $ex1)? ) ^( EXPR ( $ex2)? ) )
                                        {
                                            object root_1 = (object)adaptor.GetNilNode();
                                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(FORSTEP, "FORSTEP"), root_1);

                                            // ES3.g3:1117:18: ^( VAR ( variableDeclarationNoIn )+ )
                                            {
                                                object root_2 = (object)adaptor.GetNilNode();
                                                root_2 = (object)adaptor.BecomeRoot(stream_VAR.NextNode(), root_2);

                                                if (!(stream_variableDeclarationNoIn.HasNext()))
                                                {
                                                    throw new RewriteEarlyExitException();
                                                }
                                                while (stream_variableDeclarationNoIn.HasNext())
                                                {
                                                    adaptor.AddChild(root_2, stream_variableDeclarationNoIn.NextTree());

                                                }
                                                stream_variableDeclarationNoIn.Reset();

                                                adaptor.AddChild(root_1, root_2);
                                            }
                                            // ES3.g3:1117:52: ^( EXPR ( $ex1)? )
                                            {
                                                object root_2 = (object)adaptor.GetNilNode();
                                                root_2 = (object)adaptor.BecomeRoot((object)adaptor.Create(EXPR, "EXPR"), root_2);

                                                // ES3.g3:1117:60: ( $ex1)?
                                                if (stream_ex1.HasNext())
                                                {
                                                    adaptor.AddChild(root_2, stream_ex1.NextTree());

                                                }
                                                stream_ex1.Reset();

                                                adaptor.AddChild(root_1, root_2);
                                            }
                                            // ES3.g3:1117:68: ^( EXPR ( $ex2)? )
                                            {
                                                object root_2 = (object)adaptor.GetNilNode();
                                                root_2 = (object)adaptor.BecomeRoot((object)adaptor.Create(EXPR, "EXPR"), root_2);

                                                // ES3.g3:1117:76: ( $ex2)?
                                                if (stream_ex2.HasNext())
                                                {
                                                    adaptor.AddChild(root_2, stream_ex2.NextTree());

                                                }
                                                stream_ex2.Reset();

                                                adaptor.AddChild(root_1, root_2);
                                            }

                                            adaptor.AddChild(root_0, root_1);
                                        }

                                    }

                                    retval.Tree = root_0; retval.Tree = root_0;
                                }


                            }
                            break;

                    }


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "forControlVar"

        public class forControlExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "forControlExpression"
        // ES3.g3:1122:1: forControlExpression : ex1= expressionNoIn ({...}? ( IN ex2= expression -> ^( FORITER ^( EXPR $ex1) ^( EXPR $ex2) ) ) | ( SEMIC (ex2= expression )? SEMIC (ex3= expression )? -> ^( FORSTEP ^( EXPR $ex1) ^( EXPR ( $ex2)? ) ^( EXPR ( $ex3)? ) ) ) ) ;
        public ES3Parser.forControlExpression_return forControlExpression() // throws RecognitionException [1]
        {
            ES3Parser.forControlExpression_return retval = new ES3Parser.forControlExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken IN212 = null;
            IToken SEMIC213 = null;
            IToken SEMIC214 = null;
            ES3Parser.expressionNoIn_return ex1 = default(ES3Parser.expressionNoIn_return);

            ES3Parser.expression_return ex2 = default(ES3Parser.expression_return);

            ES3Parser.expression_return ex3 = default(ES3Parser.expression_return);


            object IN212_tree = null;
            object SEMIC213_tree = null;
            object SEMIC214_tree = null;
            RewriteRuleTokenStream stream_IN = new RewriteRuleTokenStream(adaptor, "token IN");
            RewriteRuleTokenStream stream_SEMIC = new RewriteRuleTokenStream(adaptor, "token SEMIC");
            RewriteRuleSubtreeStream stream_expression = new RewriteRuleSubtreeStream(adaptor, "rule expression");
            RewriteRuleSubtreeStream stream_expressionNoIn = new RewriteRuleSubtreeStream(adaptor, "rule expressionNoIn");

            bool? isLhs = null;

            try
            {
                // ES3.g3:1127:2: (ex1= expressionNoIn ({...}? ( IN ex2= expression -> ^( FORITER ^( EXPR $ex1) ^( EXPR $ex2) ) ) | ( SEMIC (ex2= expression )? SEMIC (ex3= expression )? -> ^( FORSTEP ^( EXPR $ex1) ^( EXPR ( $ex2)? ) ^( EXPR ( $ex3)? ) ) ) ) )
                // ES3.g3:1127:4: ex1= expressionNoIn ({...}? ( IN ex2= expression -> ^( FORITER ^( EXPR $ex1) ^( EXPR $ex2) ) ) | ( SEMIC (ex2= expression )? SEMIC (ex3= expression )? -> ^( FORSTEP ^( EXPR $ex1) ^( EXPR ( $ex2)? ) ^( EXPR ( $ex3)? ) ) ) )
                {
                    PushFollow(FOLLOW_expressionNoIn_in_forControlExpression5078);
                    ex1 = expressionNoIn();
                    state.followingStackPointer--;

                    stream_expressionNoIn.Add(ex1.Tree);
                    // ES3.g3:1128:2: ({...}? ( IN ex2= expression -> ^( FORITER ^( EXPR $ex1) ^( EXPR $ex2) ) ) | ( SEMIC (ex2= expression )? SEMIC (ex3= expression )? -> ^( FORSTEP ^( EXPR $ex1) ^( EXPR ( $ex2)? ) ^( EXPR ( $ex3)? ) ) ) )
                    int alt58 = 2;
                    int LA58_0 = input.LA(1);

                    if ((LA58_0 == IN))
                    {
                        alt58 = 1;
                    }
                    else if ((LA58_0 == SEMIC))
                    {
                        alt58 = 2;
                    }
                    else
                    {
                        NoViableAltException nvae_d58s0 =
                            new NoViableAltException("", 58, 0, input);

                        throw nvae_d58s0;
                    }
                    switch (alt58)
                    {
                        case 1:
                            // ES3.g3:1129:3: {...}? ( IN ex2= expression -> ^( FORITER ^( EXPR $ex1) ^( EXPR $ex2) ) )
                            {
                                if (!((IsLeftHandSideIn(ex1, ref isLhs))))
                                {
                                    throw new FailedPredicateException(input, "forControlExpression", " IsLeftHandSideIn(ex1, ref isLhs) ");
                                }
                                // ES3.g3:1129:41: ( IN ex2= expression -> ^( FORITER ^( EXPR $ex1) ^( EXPR $ex2) ) )
                                // ES3.g3:1130:4: IN ex2= expression
                                {
                                    IN212 = (IToken)Match(input, IN, FOLLOW_IN_in_forControlExpression5093);
                                    stream_IN.Add(IN212);

                                    PushFollow(FOLLOW_expression_in_forControlExpression5097);
                                    ex2 = expression();
                                    state.followingStackPointer--;

                                    stream_expression.Add(ex2.Tree);


                                    // AST REWRITE
                                    // elements:          ex1, ex2
                                    // token labels:      
                                    // rule labels:       retval, ex2, ex1
                                    // token list labels: 
                                    // rule list labels:  
                                    // wildcard labels: 
                                    retval.Tree = root_0;
                                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);
                                    RewriteRuleSubtreeStream stream_ex2 = new RewriteRuleSubtreeStream(adaptor, "rule ex2", ex2 != null ? ex2.Tree : null);
                                    RewriteRuleSubtreeStream stream_ex1 = new RewriteRuleSubtreeStream(adaptor, "rule ex1", ex1 != null ? ex1.Tree : null);

                                    root_0 = (object)adaptor.GetNilNode();
                                    // 1131:4: -> ^( FORITER ^( EXPR $ex1) ^( EXPR $ex2) )
                                    {
                                        // ES3.g3:1131:7: ^( FORITER ^( EXPR $ex1) ^( EXPR $ex2) )
                                        {
                                            object root_1 = (object)adaptor.GetNilNode();
                                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(FORITER, "FORITER"), root_1);

                                            // ES3.g3:1131:18: ^( EXPR $ex1)
                                            {
                                                object root_2 = (object)adaptor.GetNilNode();
                                                root_2 = (object)adaptor.BecomeRoot((object)adaptor.Create(EXPR, "EXPR"), root_2);

                                                adaptor.AddChild(root_2, stream_ex1.NextTree());

                                                adaptor.AddChild(root_1, root_2);
                                            }
                                            // ES3.g3:1131:33: ^( EXPR $ex2)
                                            {
                                                object root_2 = (object)adaptor.GetNilNode();
                                                root_2 = (object)adaptor.BecomeRoot((object)adaptor.Create(EXPR, "EXPR"), root_2);

                                                adaptor.AddChild(root_2, stream_ex2.NextTree());

                                                adaptor.AddChild(root_1, root_2);
                                            }

                                            adaptor.AddChild(root_0, root_1);
                                        }

                                    }

                                    retval.Tree = root_0; retval.Tree = root_0;
                                }


                            }
                            break;
                        case 2:
                            // ES3.g3:1134:3: ( SEMIC (ex2= expression )? SEMIC (ex3= expression )? -> ^( FORSTEP ^( EXPR $ex1) ^( EXPR ( $ex2)? ) ^( EXPR ( $ex3)? ) ) )
                            {
                                // ES3.g3:1134:3: ( SEMIC (ex2= expression )? SEMIC (ex3= expression )? -> ^( FORSTEP ^( EXPR $ex1) ^( EXPR ( $ex2)? ) ^( EXPR ( $ex3)? ) ) )
                                // ES3.g3:1135:4: SEMIC (ex2= expression )? SEMIC (ex3= expression )?
                                {
                                    SEMIC213 = (IToken)Match(input, SEMIC, FOLLOW_SEMIC_in_forControlExpression5143);
                                    stream_SEMIC.Add(SEMIC213);

                                    // ES3.g3:1135:13: (ex2= expression )?
                                    int alt56 = 2;
                                    int LA56_0 = input.LA(1);

                                    if (((LA56_0 >= NULL && LA56_0 <= FALSE) || LA56_0 == DELETE || LA56_0 == FUNCTION || LA56_0 == NEW || LA56_0 == THIS || LA56_0 == TYPEOF || LA56_0 == VOID || LA56_0 == LBRACE || LA56_0 == LPAREN || LA56_0 == LBRACK || (LA56_0 >= ADD && LA56_0 <= SUB) || (LA56_0 >= INC && LA56_0 <= DEC) || (LA56_0 >= NOT && LA56_0 <= INV) || (LA56_0 >= Identifier && LA56_0 <= StringLiteral) || LA56_0 == RegularExpressionLiteral || (LA56_0 >= DecimalLiteral && LA56_0 <= HexIntegerLiteral)))
                                    {
                                        alt56 = 1;
                                    }
                                    switch (alt56)
                                    {
                                        case 1:
                                            // ES3.g3:1135:13: ex2= expression
                                            {
                                                PushFollow(FOLLOW_expression_in_forControlExpression5147);
                                                ex2 = expression();
                                                state.followingStackPointer--;

                                                stream_expression.Add(ex2.Tree);

                                            }
                                            break;

                                    }

                                    SEMIC214 = (IToken)Match(input, SEMIC, FOLLOW_SEMIC_in_forControlExpression5150);
                                    stream_SEMIC.Add(SEMIC214);

                                    // ES3.g3:1135:35: (ex3= expression )?
                                    int alt57 = 2;
                                    int LA57_0 = input.LA(1);

                                    if (((LA57_0 >= NULL && LA57_0 <= FALSE) || LA57_0 == DELETE || LA57_0 == FUNCTION || LA57_0 == NEW || LA57_0 == THIS || LA57_0 == TYPEOF || LA57_0 == VOID || LA57_0 == LBRACE || LA57_0 == LPAREN || LA57_0 == LBRACK || (LA57_0 >= ADD && LA57_0 <= SUB) || (LA57_0 >= INC && LA57_0 <= DEC) || (LA57_0 >= NOT && LA57_0 <= INV) || (LA57_0 >= Identifier && LA57_0 <= StringLiteral) || LA57_0 == RegularExpressionLiteral || (LA57_0 >= DecimalLiteral && LA57_0 <= HexIntegerLiteral)))
                                    {
                                        alt57 = 1;
                                    }
                                    switch (alt57)
                                    {
                                        case 1:
                                            // ES3.g3:1135:35: ex3= expression
                                            {
                                                PushFollow(FOLLOW_expression_in_forControlExpression5154);
                                                ex3 = expression();
                                                state.followingStackPointer--;

                                                stream_expression.Add(ex3.Tree);

                                            }
                                            break;

                                    }



                                    // AST REWRITE
                                    // elements:          ex2, ex1, ex3
                                    // token labels:      
                                    // rule labels:       retval, ex3, ex2, ex1
                                    // token list labels: 
                                    // rule list labels:  
                                    // wildcard labels: 
                                    retval.Tree = root_0;
                                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);
                                    RewriteRuleSubtreeStream stream_ex3 = new RewriteRuleSubtreeStream(adaptor, "rule ex3", ex3 != null ? ex3.Tree : null);
                                    RewriteRuleSubtreeStream stream_ex2 = new RewriteRuleSubtreeStream(adaptor, "rule ex2", ex2 != null ? ex2.Tree : null);
                                    RewriteRuleSubtreeStream stream_ex1 = new RewriteRuleSubtreeStream(adaptor, "rule ex1", ex1 != null ? ex1.Tree : null);

                                    root_0 = (object)adaptor.GetNilNode();
                                    // 1136:4: -> ^( FORSTEP ^( EXPR $ex1) ^( EXPR ( $ex2)? ) ^( EXPR ( $ex3)? ) )
                                    {
                                        // ES3.g3:1136:7: ^( FORSTEP ^( EXPR $ex1) ^( EXPR ( $ex2)? ) ^( EXPR ( $ex3)? ) )
                                        {
                                            object root_1 = (object)adaptor.GetNilNode();
                                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(FORSTEP, "FORSTEP"), root_1);

                                            // ES3.g3:1136:18: ^( EXPR $ex1)
                                            {
                                                object root_2 = (object)adaptor.GetNilNode();
                                                root_2 = (object)adaptor.BecomeRoot((object)adaptor.Create(EXPR, "EXPR"), root_2);

                                                adaptor.AddChild(root_2, stream_ex1.NextTree());

                                                adaptor.AddChild(root_1, root_2);
                                            }
                                            // ES3.g3:1136:33: ^( EXPR ( $ex2)? )
                                            {
                                                object root_2 = (object)adaptor.GetNilNode();
                                                root_2 = (object)adaptor.BecomeRoot((object)adaptor.Create(EXPR, "EXPR"), root_2);

                                                // ES3.g3:1136:41: ( $ex2)?
                                                if (stream_ex2.HasNext())
                                                {
                                                    adaptor.AddChild(root_2, stream_ex2.NextTree());

                                                }
                                                stream_ex2.Reset();

                                                adaptor.AddChild(root_1, root_2);
                                            }
                                            // ES3.g3:1136:49: ^( EXPR ( $ex3)? )
                                            {
                                                object root_2 = (object)adaptor.GetNilNode();
                                                root_2 = (object)adaptor.BecomeRoot((object)adaptor.Create(EXPR, "EXPR"), root_2);

                                                // ES3.g3:1136:57: ( $ex3)?
                                                if (stream_ex3.HasNext())
                                                {
                                                    adaptor.AddChild(root_2, stream_ex3.NextTree());

                                                }
                                                stream_ex3.Reset();

                                                adaptor.AddChild(root_1, root_2);
                                            }

                                            adaptor.AddChild(root_0, root_1);
                                        }

                                    }

                                    retval.Tree = root_0; retval.Tree = root_0;
                                }


                            }
                            break;

                    }


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "forControlExpression"

        public class forControlSemic_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "forControlSemic"
        // ES3.g3:1141:1: forControlSemic : SEMIC (ex1= expression )? SEMIC (ex2= expression )? -> ^( FORSTEP ^( EXPR ) ^( EXPR ( $ex1)? ) ^( EXPR ( $ex2)? ) ) ;
        public ES3Parser.forControlSemic_return forControlSemic() // throws RecognitionException [1]
        {
            ES3Parser.forControlSemic_return retval = new ES3Parser.forControlSemic_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken SEMIC215 = null;
            IToken SEMIC216 = null;
            ES3Parser.expression_return ex1 = default(ES3Parser.expression_return);

            ES3Parser.expression_return ex2 = default(ES3Parser.expression_return);


            object SEMIC215_tree = null;
            object SEMIC216_tree = null;
            RewriteRuleTokenStream stream_SEMIC = new RewriteRuleTokenStream(adaptor, "token SEMIC");
            RewriteRuleSubtreeStream stream_expression = new RewriteRuleSubtreeStream(adaptor, "rule expression");
            try
            {
                // ES3.g3:1142:2: ( SEMIC (ex1= expression )? SEMIC (ex2= expression )? -> ^( FORSTEP ^( EXPR ) ^( EXPR ( $ex1)? ) ^( EXPR ( $ex2)? ) ) )
                // ES3.g3:1142:4: SEMIC (ex1= expression )? SEMIC (ex2= expression )?
                {
                    SEMIC215 = (IToken)Match(input, SEMIC, FOLLOW_SEMIC_in_forControlSemic5213);
                    stream_SEMIC.Add(SEMIC215);

                    // ES3.g3:1142:13: (ex1= expression )?
                    int alt59 = 2;
                    int LA59_0 = input.LA(1);

                    if (((LA59_0 >= NULL && LA59_0 <= FALSE) || LA59_0 == DELETE || LA59_0 == FUNCTION || LA59_0 == NEW || LA59_0 == THIS || LA59_0 == TYPEOF || LA59_0 == VOID || LA59_0 == LBRACE || LA59_0 == LPAREN || LA59_0 == LBRACK || (LA59_0 >= ADD && LA59_0 <= SUB) || (LA59_0 >= INC && LA59_0 <= DEC) || (LA59_0 >= NOT && LA59_0 <= INV) || (LA59_0 >= Identifier && LA59_0 <= StringLiteral) || LA59_0 == RegularExpressionLiteral || (LA59_0 >= DecimalLiteral && LA59_0 <= HexIntegerLiteral)))
                    {
                        alt59 = 1;
                    }
                    switch (alt59)
                    {
                        case 1:
                            // ES3.g3:1142:13: ex1= expression
                            {
                                PushFollow(FOLLOW_expression_in_forControlSemic5217);
                                ex1 = expression();
                                state.followingStackPointer--;

                                stream_expression.Add(ex1.Tree);

                            }
                            break;

                    }

                    SEMIC216 = (IToken)Match(input, SEMIC, FOLLOW_SEMIC_in_forControlSemic5220);
                    stream_SEMIC.Add(SEMIC216);

                    // ES3.g3:1142:35: (ex2= expression )?
                    int alt60 = 2;
                    int LA60_0 = input.LA(1);

                    if (((LA60_0 >= NULL && LA60_0 <= FALSE) || LA60_0 == DELETE || LA60_0 == FUNCTION || LA60_0 == NEW || LA60_0 == THIS || LA60_0 == TYPEOF || LA60_0 == VOID || LA60_0 == LBRACE || LA60_0 == LPAREN || LA60_0 == LBRACK || (LA60_0 >= ADD && LA60_0 <= SUB) || (LA60_0 >= INC && LA60_0 <= DEC) || (LA60_0 >= NOT && LA60_0 <= INV) || (LA60_0 >= Identifier && LA60_0 <= StringLiteral) || LA60_0 == RegularExpressionLiteral || (LA60_0 >= DecimalLiteral && LA60_0 <= HexIntegerLiteral)))
                    {
                        alt60 = 1;
                    }
                    switch (alt60)
                    {
                        case 1:
                            // ES3.g3:1142:35: ex2= expression
                            {
                                PushFollow(FOLLOW_expression_in_forControlSemic5224);
                                ex2 = expression();
                                state.followingStackPointer--;

                                stream_expression.Add(ex2.Tree);

                            }
                            break;

                    }



                    // AST REWRITE
                    // elements:          ex1, ex2
                    // token labels:      
                    // rule labels:       retval, ex2, ex1
                    // token list labels: 
                    // rule list labels:  
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);
                    RewriteRuleSubtreeStream stream_ex2 = new RewriteRuleSubtreeStream(adaptor, "rule ex2", ex2 != null ? ex2.Tree : null);
                    RewriteRuleSubtreeStream stream_ex1 = new RewriteRuleSubtreeStream(adaptor, "rule ex1", ex1 != null ? ex1.Tree : null);

                    root_0 = (object)adaptor.GetNilNode();
                    // 1143:2: -> ^( FORSTEP ^( EXPR ) ^( EXPR ( $ex1)? ) ^( EXPR ( $ex2)? ) )
                    {
                        // ES3.g3:1143:5: ^( FORSTEP ^( EXPR ) ^( EXPR ( $ex1)? ) ^( EXPR ( $ex2)? ) )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(FORSTEP, "FORSTEP"), root_1);

                            // ES3.g3:1143:16: ^( EXPR )
                            {
                                object root_2 = (object)adaptor.GetNilNode();
                                root_2 = (object)adaptor.BecomeRoot((object)adaptor.Create(EXPR, "EXPR"), root_2);

                                adaptor.AddChild(root_1, root_2);
                            }
                            // ES3.g3:1143:26: ^( EXPR ( $ex1)? )
                            {
                                object root_2 = (object)adaptor.GetNilNode();
                                root_2 = (object)adaptor.BecomeRoot((object)adaptor.Create(EXPR, "EXPR"), root_2);

                                // ES3.g3:1143:34: ( $ex1)?
                                if (stream_ex1.HasNext())
                                {
                                    adaptor.AddChild(root_2, stream_ex1.NextTree());

                                }
                                stream_ex1.Reset();

                                adaptor.AddChild(root_1, root_2);
                            }
                            // ES3.g3:1143:42: ^( EXPR ( $ex2)? )
                            {
                                object root_2 = (object)adaptor.GetNilNode();
                                root_2 = (object)adaptor.BecomeRoot((object)adaptor.Create(EXPR, "EXPR"), root_2);

                                // ES3.g3:1143:50: ( $ex2)?
                                if (stream_ex2.HasNext())
                                {
                                    adaptor.AddChild(root_2, stream_ex2.NextTree());

                                }
                                stream_ex2.Reset();

                                adaptor.AddChild(root_1, root_2);
                            }

                            adaptor.AddChild(root_0, root_1);
                        }

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "forControlSemic"

        public class continueStatement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "continueStatement"
        // ES3.g3:1155:1: continueStatement : CONTINUE ( Identifier )? semic ;
        public ES3Parser.continueStatement_return continueStatement() // throws RecognitionException [1]
        {
            ES3Parser.continueStatement_return retval = new ES3Parser.continueStatement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken CONTINUE217 = null;
            IToken Identifier218 = null;
            ES3Parser.semic_return semic219 = default(ES3Parser.semic_return);


            object CONTINUE217_tree = null;
            object Identifier218_tree = null;

            try
            {
                // ES3.g3:1156:2: ( CONTINUE ( Identifier )? semic )
                // ES3.g3:1156:4: CONTINUE ( Identifier )? semic
                {
                    root_0 = (object)adaptor.GetNilNode();

                    CONTINUE217 = (IToken)Match(input, CONTINUE, FOLLOW_CONTINUE_in_continueStatement5278);
                    CONTINUE217_tree = (object)adaptor.Create(CONTINUE217);
                    root_0 = (object)adaptor.BecomeRoot(CONTINUE217_tree, root_0);

                    if (input.LA(1) == Identifier) PromoteEOL(null);
                    // ES3.g3:1156:67: ( Identifier )?
                    int alt61 = 2;
                    int LA61_0 = input.LA(1);

                    if ((LA61_0 == Identifier))
                    {
                        alt61 = 1;
                    }
                    switch (alt61)
                    {
                        case 1:
                            // ES3.g3:1156:67: Identifier
                            {
                                Identifier218 = (IToken)Match(input, Identifier, FOLLOW_Identifier_in_continueStatement5283);
                                Identifier218_tree = (object)adaptor.Create(Identifier218);
                                adaptor.AddChild(root_0, Identifier218_tree);


                            }
                            break;

                    }

                    PushFollow(FOLLOW_semic_in_continueStatement5286);
                    semic219 = semic();
                    state.followingStackPointer--;


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "continueStatement"

        public class breakStatement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "breakStatement"
        // ES3.g3:1168:1: breakStatement : BREAK ( Identifier )? semic ;
        public ES3Parser.breakStatement_return breakStatement() // throws RecognitionException [1]
        {
            ES3Parser.breakStatement_return retval = new ES3Parser.breakStatement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken BREAK220 = null;
            IToken Identifier221 = null;
            ES3Parser.semic_return semic222 = default(ES3Parser.semic_return);


            object BREAK220_tree = null;
            object Identifier221_tree = null;

            try
            {
                // ES3.g3:1169:2: ( BREAK ( Identifier )? semic )
                // ES3.g3:1169:4: BREAK ( Identifier )? semic
                {
                    root_0 = (object)adaptor.GetNilNode();

                    BREAK220 = (IToken)Match(input, BREAK, FOLLOW_BREAK_in_breakStatement5305);
                    BREAK220_tree = (object)adaptor.Create(BREAK220);
                    root_0 = (object)adaptor.BecomeRoot(BREAK220_tree, root_0);

                    if (input.LA(1) == Identifier) PromoteEOL(null);
                    // ES3.g3:1169:64: ( Identifier )?
                    int alt62 = 2;
                    int LA62_0 = input.LA(1);

                    if ((LA62_0 == Identifier))
                    {
                        alt62 = 1;
                    }
                    switch (alt62)
                    {
                        case 1:
                            // ES3.g3:1169:64: Identifier
                            {
                                Identifier221 = (IToken)Match(input, Identifier, FOLLOW_Identifier_in_breakStatement5310);
                                Identifier221_tree = (object)adaptor.Create(Identifier221);
                                adaptor.AddChild(root_0, Identifier221_tree);


                            }
                            break;

                    }

                    PushFollow(FOLLOW_semic_in_breakStatement5313);
                    semic222 = semic();
                    state.followingStackPointer--;


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "breakStatement"

        public class returnStatement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "returnStatement"
        // ES3.g3:1189:1: returnStatement : RETURN ( expression )? semic ;
        public ES3Parser.returnStatement_return returnStatement() // throws RecognitionException [1]
        {
            ES3Parser.returnStatement_return retval = new ES3Parser.returnStatement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken RETURN223 = null;
            ES3Parser.expression_return expression224 = default(ES3Parser.expression_return);

            ES3Parser.semic_return semic225 = default(ES3Parser.semic_return);


            object RETURN223_tree = null;

            try
            {
                // ES3.g3:1190:2: ( RETURN ( expression )? semic )
                // ES3.g3:1190:4: RETURN ( expression )? semic
                {
                    root_0 = (object)adaptor.GetNilNode();

                    RETURN223 = (IToken)Match(input, RETURN, FOLLOW_RETURN_in_returnStatement5332);
                    RETURN223_tree = (object)adaptor.Create(RETURN223);
                    root_0 = (object)adaptor.BecomeRoot(RETURN223_tree, root_0);

                    PromoteEOL(null);
                    // ES3.g3:1190:34: ( expression )?
                    int alt63 = 2;
                    int LA63_0 = input.LA(1);

                    if (((LA63_0 >= NULL && LA63_0 <= FALSE) || LA63_0 == DELETE || LA63_0 == FUNCTION || LA63_0 == NEW || LA63_0 == THIS || LA63_0 == TYPEOF || LA63_0 == VOID || LA63_0 == LBRACE || LA63_0 == LPAREN || LA63_0 == LBRACK || (LA63_0 >= ADD && LA63_0 <= SUB) || (LA63_0 >= INC && LA63_0 <= DEC) || (LA63_0 >= NOT && LA63_0 <= INV) || (LA63_0 >= Identifier && LA63_0 <= StringLiteral) || LA63_0 == RegularExpressionLiteral || (LA63_0 >= DecimalLiteral && LA63_0 <= HexIntegerLiteral)))
                    {
                        alt63 = 1;
                    }
                    switch (alt63)
                    {
                        case 1:
                            // ES3.g3:1190:34: expression
                            {
                                PushFollow(FOLLOW_expression_in_returnStatement5337);
                                expression224 = expression();
                                state.followingStackPointer--;

                                adaptor.AddChild(root_0, expression224.Tree);

                            }
                            break;

                    }

                    PushFollow(FOLLOW_semic_in_returnStatement5340);
                    semic225 = semic();
                    state.followingStackPointer--;


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "returnStatement"

        public class withStatement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "withStatement"
        // ES3.g3:1197:1: withStatement : WITH LPAREN expression RPAREN statement ;
        public ES3Parser.withStatement_return withStatement() // throws RecognitionException [1]
        {
            ES3Parser.withStatement_return retval = new ES3Parser.withStatement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken WITH226 = null;
            IToken LPAREN227 = null;
            IToken RPAREN229 = null;
            ES3Parser.expression_return expression228 = default(ES3Parser.expression_return);

            ES3Parser.statement_return statement230 = default(ES3Parser.statement_return);


            object WITH226_tree = null;
            object LPAREN227_tree = null;
            object RPAREN229_tree = null;

            try
            {
                // ES3.g3:1198:2: ( WITH LPAREN expression RPAREN statement )
                // ES3.g3:1198:4: WITH LPAREN expression RPAREN statement
                {
                    root_0 = (object)adaptor.GetNilNode();

                    WITH226 = (IToken)Match(input, WITH, FOLLOW_WITH_in_withStatement5357);
                    WITH226_tree = (object)adaptor.Create(WITH226);
                    root_0 = (object)adaptor.BecomeRoot(WITH226_tree, root_0);

                    LPAREN227 = (IToken)Match(input, LPAREN, FOLLOW_LPAREN_in_withStatement5360);
                    PushFollow(FOLLOW_expression_in_withStatement5363);
                    expression228 = expression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, expression228.Tree);
                    RPAREN229 = (IToken)Match(input, RPAREN, FOLLOW_RPAREN_in_withStatement5365);
                    PushFollow(FOLLOW_statement_in_withStatement5368);
                    statement230 = statement();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, statement230.Tree);

                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "withStatement"

        public class switchStatement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "switchStatement"
        // ES3.g3:1205:1: switchStatement : SWITCH LPAREN expression RPAREN LBRACE ({...}? => defaultClause | caseClause )* RBRACE -> ^( SWITCH expression ( defaultClause )? ( caseClause )* ) ;
        public ES3Parser.switchStatement_return switchStatement() // throws RecognitionException [1]
        {
            ES3Parser.switchStatement_return retval = new ES3Parser.switchStatement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken SWITCH231 = null;
            IToken LPAREN232 = null;
            IToken RPAREN234 = null;
            IToken LBRACE235 = null;
            IToken RBRACE238 = null;
            ES3Parser.expression_return expression233 = default(ES3Parser.expression_return);

            ES3Parser.defaultClause_return defaultClause236 = default(ES3Parser.defaultClause_return);

            ES3Parser.caseClause_return caseClause237 = default(ES3Parser.caseClause_return);


            object SWITCH231_tree = null;
            object LPAREN232_tree = null;
            object RPAREN234_tree = null;
            object LBRACE235_tree = null;
            object RBRACE238_tree = null;
            RewriteRuleTokenStream stream_RPAREN = new RewriteRuleTokenStream(adaptor, "token RPAREN");
            RewriteRuleTokenStream stream_RBRACE = new RewriteRuleTokenStream(adaptor, "token RBRACE");
            RewriteRuleTokenStream stream_SWITCH = new RewriteRuleTokenStream(adaptor, "token SWITCH");
            RewriteRuleTokenStream stream_LPAREN = new RewriteRuleTokenStream(adaptor, "token LPAREN");
            RewriteRuleTokenStream stream_LBRACE = new RewriteRuleTokenStream(adaptor, "token LBRACE");
            RewriteRuleSubtreeStream stream_expression = new RewriteRuleSubtreeStream(adaptor, "rule expression");
            RewriteRuleSubtreeStream stream_caseClause = new RewriteRuleSubtreeStream(adaptor, "rule caseClause");
            RewriteRuleSubtreeStream stream_defaultClause = new RewriteRuleSubtreeStream(adaptor, "rule defaultClause");

            int defaultClauseCount = 0;

            try
            {
                // ES3.g3:1210:2: ( SWITCH LPAREN expression RPAREN LBRACE ({...}? => defaultClause | caseClause )* RBRACE -> ^( SWITCH expression ( defaultClause )? ( caseClause )* ) )
                // ES3.g3:1210:4: SWITCH LPAREN expression RPAREN LBRACE ({...}? => defaultClause | caseClause )* RBRACE
                {
                    SWITCH231 = (IToken)Match(input, SWITCH, FOLLOW_SWITCH_in_switchStatement5389);
                    stream_SWITCH.Add(SWITCH231);

                    LPAREN232 = (IToken)Match(input, LPAREN, FOLLOW_LPAREN_in_switchStatement5391);
                    stream_LPAREN.Add(LPAREN232);

                    PushFollow(FOLLOW_expression_in_switchStatement5393);
                    expression233 = expression();
                    state.followingStackPointer--;

                    stream_expression.Add(expression233.Tree);
                    RPAREN234 = (IToken)Match(input, RPAREN, FOLLOW_RPAREN_in_switchStatement5395);
                    stream_RPAREN.Add(RPAREN234);

                    LBRACE235 = (IToken)Match(input, LBRACE, FOLLOW_LBRACE_in_switchStatement5397);
                    stream_LBRACE.Add(LBRACE235);

                    // ES3.g3:1210:43: ({...}? => defaultClause | caseClause )*
                    do
                    {
                        int alt64 = 3;
                        int LA64_0 = input.LA(1);

                        if ((LA64_0 == DEFAULT) && ((defaultClauseCount == 0)))
                        {
                            alt64 = 1;
                        }
                        else if ((LA64_0 == CASE))
                        {
                            alt64 = 2;
                        }


                        switch (alt64)
                        {
                            case 1:
                                // ES3.g3:1210:45: {...}? => defaultClause
                                {
                                    if (!((defaultClauseCount == 0)))
                                    {
                                        throw new FailedPredicateException(input, "switchStatement", " defaultClauseCount == 0 ");
                                    }
                                    PushFollow(FOLLOW_defaultClause_in_switchStatement5404);
                                    defaultClause236 = defaultClause();
                                    state.followingStackPointer--;

                                    stream_defaultClause.Add(defaultClause236.Tree);
                                    defaultClauseCount++;

                                }
                                break;
                            case 2:
                                // ES3.g3:1210:118: caseClause
                                {
                                    PushFollow(FOLLOW_caseClause_in_switchStatement5410);
                                    caseClause237 = caseClause();
                                    state.followingStackPointer--;

                                    stream_caseClause.Add(caseClause237.Tree);

                                }
                                break;

                            default:
                                goto loop64;
                        }
                    } while (true);

                loop64:
                    ;	// Stops C# compiler whining that label 'loop64' has no statements

                    RBRACE238 = (IToken)Match(input, RBRACE, FOLLOW_RBRACE_in_switchStatement5415);
                    stream_RBRACE.Add(RBRACE238);



                    // AST REWRITE
                    // elements:          expression, SWITCH, defaultClause, caseClause
                    // token labels:      
                    // rule labels:       retval
                    // token list labels: 
                    // rule list labels:  
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                    root_0 = (object)adaptor.GetNilNode();
                    // 1211:2: -> ^( SWITCH expression ( defaultClause )? ( caseClause )* )
                    {
                        // ES3.g3:1211:5: ^( SWITCH expression ( defaultClause )? ( caseClause )* )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot(stream_SWITCH.NextNode(), root_1);

                            adaptor.AddChild(root_1, stream_expression.NextTree());
                            // ES3.g3:1211:26: ( defaultClause )?
                            if (stream_defaultClause.HasNext())
                            {
                                adaptor.AddChild(root_1, stream_defaultClause.NextTree());

                            }
                            stream_defaultClause.Reset();
                            // ES3.g3:1211:41: ( caseClause )*
                            while (stream_caseClause.HasNext())
                            {
                                adaptor.AddChild(root_1, stream_caseClause.NextTree());

                            }
                            stream_caseClause.Reset();

                            adaptor.AddChild(root_0, root_1);
                        }

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "switchStatement"

        public class caseClause_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "caseClause"
        // ES3.g3:1214:1: caseClause : CASE expression COLON ( statement )* ;
        public ES3Parser.caseClause_return caseClause() // throws RecognitionException [1]
        {
            ES3Parser.caseClause_return retval = new ES3Parser.caseClause_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken CASE239 = null;
            IToken COLON241 = null;
            ES3Parser.expression_return expression240 = default(ES3Parser.expression_return);

            ES3Parser.statement_return statement242 = default(ES3Parser.statement_return);


            object CASE239_tree = null;
            object COLON241_tree = null;

            try
            {
                // ES3.g3:1215:2: ( CASE expression COLON ( statement )* )
                // ES3.g3:1215:4: CASE expression COLON ( statement )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    CASE239 = (IToken)Match(input, CASE, FOLLOW_CASE_in_caseClause5443);
                    CASE239_tree = (object)adaptor.Create(CASE239);
                    root_0 = (object)adaptor.BecomeRoot(CASE239_tree, root_0);

                    PushFollow(FOLLOW_expression_in_caseClause5446);
                    expression240 = expression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, expression240.Tree);
                    COLON241 = (IToken)Match(input, COLON, FOLLOW_COLON_in_caseClause5448);
                    // ES3.g3:1215:28: ( statement )*
                    do
                    {
                        int alt65 = 2;
                        int LA65_0 = input.LA(1);

                        if (((LA65_0 >= NULL && LA65_0 <= BREAK) || LA65_0 == CONTINUE || (LA65_0 >= DELETE && LA65_0 <= DO) || (LA65_0 >= FOR && LA65_0 <= IF) || (LA65_0 >= NEW && LA65_0 <= WITH) || LA65_0 == LBRACE || LA65_0 == LPAREN || LA65_0 == LBRACK || LA65_0 == SEMIC || (LA65_0 >= ADD && LA65_0 <= SUB) || (LA65_0 >= INC && LA65_0 <= DEC) || (LA65_0 >= NOT && LA65_0 <= INV) || (LA65_0 >= Identifier && LA65_0 <= StringLiteral) || LA65_0 == RegularExpressionLiteral || (LA65_0 >= DecimalLiteral && LA65_0 <= HexIntegerLiteral)))
                        {
                            alt65 = 1;
                        }


                        switch (alt65)
                        {
                            case 1:
                                // ES3.g3:1215:28: statement
                                {
                                    PushFollow(FOLLOW_statement_in_caseClause5451);
                                    statement242 = statement();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, statement242.Tree);

                                }
                                break;

                            default:
                                goto loop65;
                        }
                    } while (true);

                loop65:
                    ;	// Stops C# compiler whining that label 'loop65' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "caseClause"

        public class defaultClause_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "defaultClause"
        // ES3.g3:1218:1: defaultClause : DEFAULT COLON ( statement )* ;
        public ES3Parser.defaultClause_return defaultClause() // throws RecognitionException [1]
        {
            ES3Parser.defaultClause_return retval = new ES3Parser.defaultClause_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken DEFAULT243 = null;
            IToken COLON244 = null;
            ES3Parser.statement_return statement245 = default(ES3Parser.statement_return);


            object DEFAULT243_tree = null;
            object COLON244_tree = null;

            try
            {
                // ES3.g3:1219:2: ( DEFAULT COLON ( statement )* )
                // ES3.g3:1219:4: DEFAULT COLON ( statement )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    DEFAULT243 = (IToken)Match(input, DEFAULT, FOLLOW_DEFAULT_in_defaultClause5464);
                    DEFAULT243_tree = (object)adaptor.Create(DEFAULT243);
                    root_0 = (object)adaptor.BecomeRoot(DEFAULT243_tree, root_0);

                    COLON244 = (IToken)Match(input, COLON, FOLLOW_COLON_in_defaultClause5467);
                    // ES3.g3:1219:20: ( statement )*
                    do
                    {
                        int alt66 = 2;
                        int LA66_0 = input.LA(1);

                        if (((LA66_0 >= NULL && LA66_0 <= BREAK) || LA66_0 == CONTINUE || (LA66_0 >= DELETE && LA66_0 <= DO) || (LA66_0 >= FOR && LA66_0 <= IF) || (LA66_0 >= NEW && LA66_0 <= WITH) || LA66_0 == LBRACE || LA66_0 == LPAREN || LA66_0 == LBRACK || LA66_0 == SEMIC || (LA66_0 >= ADD && LA66_0 <= SUB) || (LA66_0 >= INC && LA66_0 <= DEC) || (LA66_0 >= NOT && LA66_0 <= INV) || (LA66_0 >= Identifier && LA66_0 <= StringLiteral) || LA66_0 == RegularExpressionLiteral || (LA66_0 >= DecimalLiteral && LA66_0 <= HexIntegerLiteral)))
                        {
                            alt66 = 1;
                        }


                        switch (alt66)
                        {
                            case 1:
                                // ES3.g3:1219:20: statement
                                {
                                    PushFollow(FOLLOW_statement_in_defaultClause5470);
                                    statement245 = statement();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, statement245.Tree);

                                }
                                break;

                            default:
                                goto loop66;
                        }
                    } while (true);

                loop66:
                    ;	// Stops C# compiler whining that label 'loop66' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "defaultClause"

        public class labelledStatement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "labelledStatement"
        // ES3.g3:1226:1: labelledStatement : Identifier COLON statement -> ^( LABELLED Identifier statement ) ;
        public ES3Parser.labelledStatement_return labelledStatement() // throws RecognitionException [1]
        {
            ES3Parser.labelledStatement_return retval = new ES3Parser.labelledStatement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken Identifier246 = null;
            IToken COLON247 = null;
            ES3Parser.statement_return statement248 = default(ES3Parser.statement_return);


            object Identifier246_tree = null;
            object COLON247_tree = null;
            RewriteRuleTokenStream stream_COLON = new RewriteRuleTokenStream(adaptor, "token COLON");
            RewriteRuleTokenStream stream_Identifier = new RewriteRuleTokenStream(adaptor, "token Identifier");
            RewriteRuleSubtreeStream stream_statement = new RewriteRuleSubtreeStream(adaptor, "rule statement");
            try
            {
                // ES3.g3:1227:2: ( Identifier COLON statement -> ^( LABELLED Identifier statement ) )
                // ES3.g3:1227:4: Identifier COLON statement
                {
                    Identifier246 = (IToken)Match(input, Identifier, FOLLOW_Identifier_in_labelledStatement5487);
                    stream_Identifier.Add(Identifier246);

                    COLON247 = (IToken)Match(input, COLON, FOLLOW_COLON_in_labelledStatement5489);
                    stream_COLON.Add(COLON247);

                    PushFollow(FOLLOW_statement_in_labelledStatement5491);
                    statement248 = statement();
                    state.followingStackPointer--;

                    stream_statement.Add(statement248.Tree);


                    // AST REWRITE
                    // elements:          statement, Identifier
                    // token labels:      
                    // rule labels:       retval
                    // token list labels: 
                    // rule list labels:  
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                    root_0 = (object)adaptor.GetNilNode();
                    // 1228:2: -> ^( LABELLED Identifier statement )
                    {
                        // ES3.g3:1228:5: ^( LABELLED Identifier statement )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(LABELLED, "LABELLED"), root_1);

                            adaptor.AddChild(root_1, stream_Identifier.NextNode());
                            adaptor.AddChild(root_1, stream_statement.NextTree());

                            adaptor.AddChild(root_0, root_1);
                        }

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "labelledStatement"

        public class throwStatement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "throwStatement"
        // ES3.g3:1250:1: throwStatement : THROW expression semic ;
        public ES3Parser.throwStatement_return throwStatement() // throws RecognitionException [1]
        {
            ES3Parser.throwStatement_return retval = new ES3Parser.throwStatement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken THROW249 = null;
            ES3Parser.expression_return expression250 = default(ES3Parser.expression_return);

            ES3Parser.semic_return semic251 = default(ES3Parser.semic_return);


            object THROW249_tree = null;

            try
            {
                // ES3.g3:1251:2: ( THROW expression semic )
                // ES3.g3:1251:4: THROW expression semic
                {
                    root_0 = (object)adaptor.GetNilNode();

                    THROW249 = (IToken)Match(input, THROW, FOLLOW_THROW_in_throwStatement5522);
                    THROW249_tree = (object)adaptor.Create(THROW249);
                    root_0 = (object)adaptor.BecomeRoot(THROW249_tree, root_0);

                    PromoteEOL(null);
                    PushFollow(FOLLOW_expression_in_throwStatement5527);
                    expression250 = expression();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, expression250.Tree);
                    PushFollow(FOLLOW_semic_in_throwStatement5529);
                    semic251 = semic();
                    state.followingStackPointer--;


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "throwStatement"

        public class tryStatement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "tryStatement"
        // ES3.g3:1258:1: tryStatement : TRY block ( catchClause ( finallyClause )? | finallyClause ) ;
        public ES3Parser.tryStatement_return tryStatement() // throws RecognitionException [1]
        {
            ES3Parser.tryStatement_return retval = new ES3Parser.tryStatement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken TRY252 = null;
            ES3Parser.block_return block253 = default(ES3Parser.block_return);

            ES3Parser.catchClause_return catchClause254 = default(ES3Parser.catchClause_return);

            ES3Parser.finallyClause_return finallyClause255 = default(ES3Parser.finallyClause_return);

            ES3Parser.finallyClause_return finallyClause256 = default(ES3Parser.finallyClause_return);


            object TRY252_tree = null;

            try
            {
                // ES3.g3:1259:2: ( TRY block ( catchClause ( finallyClause )? | finallyClause ) )
                // ES3.g3:1259:4: TRY block ( catchClause ( finallyClause )? | finallyClause )
                {
                    root_0 = (object)adaptor.GetNilNode();

                    TRY252 = (IToken)Match(input, TRY, FOLLOW_TRY_in_tryStatement5546);
                    TRY252_tree = (object)adaptor.Create(TRY252);
                    root_0 = (object)adaptor.BecomeRoot(TRY252_tree, root_0);

                    PushFollow(FOLLOW_block_in_tryStatement5549);
                    block253 = block();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, block253.Tree);
                    // ES3.g3:1259:15: ( catchClause ( finallyClause )? | finallyClause )
                    int alt68 = 2;
                    int LA68_0 = input.LA(1);

                    if ((LA68_0 == CATCH))
                    {
                        alt68 = 1;
                    }
                    else if ((LA68_0 == FINALLY))
                    {
                        alt68 = 2;
                    }
                    else
                    {
                        NoViableAltException nvae_d68s0 =
                            new NoViableAltException("", 68, 0, input);

                        throw nvae_d68s0;
                    }
                    switch (alt68)
                    {
                        case 1:
                            // ES3.g3:1259:17: catchClause ( finallyClause )?
                            {
                                PushFollow(FOLLOW_catchClause_in_tryStatement5553);
                                catchClause254 = catchClause();
                                state.followingStackPointer--;

                                adaptor.AddChild(root_0, catchClause254.Tree);
                                // ES3.g3:1259:29: ( finallyClause )?
                                int alt67 = 2;
                                int LA67_0 = input.LA(1);

                                if ((LA67_0 == FINALLY))
                                {
                                    alt67 = 1;
                                }
                                switch (alt67)
                                {
                                    case 1:
                                        // ES3.g3:1259:29: finallyClause
                                        {
                                            PushFollow(FOLLOW_finallyClause_in_tryStatement5555);
                                            finallyClause255 = finallyClause();
                                            state.followingStackPointer--;

                                            adaptor.AddChild(root_0, finallyClause255.Tree);

                                        }
                                        break;

                                }


                            }
                            break;
                        case 2:
                            // ES3.g3:1259:46: finallyClause
                            {
                                PushFollow(FOLLOW_finallyClause_in_tryStatement5560);
                                finallyClause256 = finallyClause();
                                state.followingStackPointer--;

                                adaptor.AddChild(root_0, finallyClause256.Tree);

                            }
                            break;

                    }


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "tryStatement"

        public class catchClause_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "catchClause"
        // ES3.g3:1262:1: catchClause : CATCH LPAREN Identifier RPAREN block ;
        public ES3Parser.catchClause_return catchClause() // throws RecognitionException [1]
        {
            ES3Parser.catchClause_return retval = new ES3Parser.catchClause_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken CATCH257 = null;
            IToken LPAREN258 = null;
            IToken Identifier259 = null;
            IToken RPAREN260 = null;
            ES3Parser.block_return block261 = default(ES3Parser.block_return);


            object CATCH257_tree = null;
            object LPAREN258_tree = null;
            object Identifier259_tree = null;
            object RPAREN260_tree = null;

            try
            {
                // ES3.g3:1263:2: ( CATCH LPAREN Identifier RPAREN block )
                // ES3.g3:1263:4: CATCH LPAREN Identifier RPAREN block
                {
                    root_0 = (object)adaptor.GetNilNode();

                    CATCH257 = (IToken)Match(input, CATCH, FOLLOW_CATCH_in_catchClause5574);
                    CATCH257_tree = (object)adaptor.Create(CATCH257);
                    root_0 = (object)adaptor.BecomeRoot(CATCH257_tree, root_0);

                    LPAREN258 = (IToken)Match(input, LPAREN, FOLLOW_LPAREN_in_catchClause5577);
                    Identifier259 = (IToken)Match(input, Identifier, FOLLOW_Identifier_in_catchClause5580);
                    Identifier259_tree = (object)adaptor.Create(Identifier259);
                    adaptor.AddChild(root_0, Identifier259_tree);

                    RPAREN260 = (IToken)Match(input, RPAREN, FOLLOW_RPAREN_in_catchClause5582);
                    PushFollow(FOLLOW_block_in_catchClause5585);
                    block261 = block();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, block261.Tree);

                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "catchClause"

        public class finallyClause_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "finallyClause"
        // ES3.g3:1266:1: finallyClause : FINALLY block ;
        public ES3Parser.finallyClause_return finallyClause() // throws RecognitionException [1]
        {
            ES3Parser.finallyClause_return retval = new ES3Parser.finallyClause_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken FINALLY262 = null;
            ES3Parser.block_return block263 = default(ES3Parser.block_return);


            object FINALLY262_tree = null;

            try
            {
                // ES3.g3:1267:2: ( FINALLY block )
                // ES3.g3:1267:4: FINALLY block
                {
                    root_0 = (object)adaptor.GetNilNode();

                    FINALLY262 = (IToken)Match(input, FINALLY, FOLLOW_FINALLY_in_finallyClause5597);
                    FINALLY262_tree = (object)adaptor.Create(FINALLY262);
                    root_0 = (object)adaptor.BecomeRoot(FINALLY262_tree, root_0);

                    PushFollow(FOLLOW_block_in_finallyClause5600);
                    block263 = block();
                    state.followingStackPointer--;

                    adaptor.AddChild(root_0, block263.Tree);

                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "finallyClause"

        public class functionDeclaration_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "functionDeclaration"
        // ES3.g3:1280:1: functionDeclaration : FUNCTION name= Identifier formalParameterList functionBody -> ^( FUNCTION $name formalParameterList functionBody ) ;
        public ES3Parser.functionDeclaration_return functionDeclaration() // throws RecognitionException [1]
        {
            ES3Parser.functionDeclaration_return retval = new ES3Parser.functionDeclaration_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken name = null;
            IToken FUNCTION264 = null;
            ES3Parser.formalParameterList_return formalParameterList265 = default(ES3Parser.formalParameterList_return);

            ES3Parser.functionBody_return functionBody266 = default(ES3Parser.functionBody_return);


            object name_tree = null;
            object FUNCTION264_tree = null;
            RewriteRuleTokenStream stream_FUNCTION = new RewriteRuleTokenStream(adaptor, "token FUNCTION");
            RewriteRuleTokenStream stream_Identifier = new RewriteRuleTokenStream(adaptor, "token Identifier");
            RewriteRuleSubtreeStream stream_functionBody = new RewriteRuleSubtreeStream(adaptor, "rule functionBody");
            RewriteRuleSubtreeStream stream_formalParameterList = new RewriteRuleSubtreeStream(adaptor, "rule formalParameterList");
            try
            {
                // ES3.g3:1281:2: ( FUNCTION name= Identifier formalParameterList functionBody -> ^( FUNCTION $name formalParameterList functionBody ) )
                // ES3.g3:1281:4: FUNCTION name= Identifier formalParameterList functionBody
                {
                    FUNCTION264 = (IToken)Match(input, FUNCTION, FOLLOW_FUNCTION_in_functionDeclaration5621);
                    stream_FUNCTION.Add(FUNCTION264);

                    name = (IToken)Match(input, Identifier, FOLLOW_Identifier_in_functionDeclaration5625);
                    stream_Identifier.Add(name);

                    PushFollow(FOLLOW_formalParameterList_in_functionDeclaration5627);
                    formalParameterList265 = formalParameterList();
                    state.followingStackPointer--;

                    stream_formalParameterList.Add(formalParameterList265.Tree);
                    PushFollow(FOLLOW_functionBody_in_functionDeclaration5629);
                    functionBody266 = functionBody();
                    state.followingStackPointer--;

                    stream_functionBody.Add(functionBody266.Tree);


                    // AST REWRITE
                    // elements:          functionBody, name, FUNCTION, formalParameterList
                    // token labels:      name
                    // rule labels:       retval
                    // token list labels: 
                    // rule list labels:  
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleTokenStream stream_name = new RewriteRuleTokenStream(adaptor, "token name", name);
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                    root_0 = (object)adaptor.GetNilNode();
                    // 1282:2: -> ^( FUNCTION $name formalParameterList functionBody )
                    {
                        // ES3.g3:1282:5: ^( FUNCTION $name formalParameterList functionBody )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot(stream_FUNCTION.NextNode(), root_1);

                            adaptor.AddChild(root_1, stream_name.NextNode());
                            adaptor.AddChild(root_1, stream_formalParameterList.NextTree());
                            adaptor.AddChild(root_1, stream_functionBody.NextTree());

                            adaptor.AddChild(root_0, root_1);
                        }

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "functionDeclaration"

        public class functionExpression_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "functionExpression"
        // ES3.g3:1285:1: functionExpression : FUNCTION (name= Identifier )? formalParameterList functionBody -> ^( FUNCTION ( $name)? formalParameterList functionBody ) ;
        public ES3Parser.functionExpression_return functionExpression() // throws RecognitionException [1]
        {
            ES3Parser.functionExpression_return retval = new ES3Parser.functionExpression_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken name = null;
            IToken FUNCTION267 = null;
            ES3Parser.formalParameterList_return formalParameterList268 = default(ES3Parser.formalParameterList_return);

            ES3Parser.functionBody_return functionBody269 = default(ES3Parser.functionBody_return);


            object name_tree = null;
            object FUNCTION267_tree = null;
            RewriteRuleTokenStream stream_FUNCTION = new RewriteRuleTokenStream(adaptor, "token FUNCTION");
            RewriteRuleTokenStream stream_Identifier = new RewriteRuleTokenStream(adaptor, "token Identifier");
            RewriteRuleSubtreeStream stream_functionBody = new RewriteRuleSubtreeStream(adaptor, "rule functionBody");
            RewriteRuleSubtreeStream stream_formalParameterList = new RewriteRuleSubtreeStream(adaptor, "rule formalParameterList");
            try
            {
                // ES3.g3:1286:2: ( FUNCTION (name= Identifier )? formalParameterList functionBody -> ^( FUNCTION ( $name)? formalParameterList functionBody ) )
                // ES3.g3:1286:4: FUNCTION (name= Identifier )? formalParameterList functionBody
                {
                    FUNCTION267 = (IToken)Match(input, FUNCTION, FOLLOW_FUNCTION_in_functionExpression5656);
                    stream_FUNCTION.Add(FUNCTION267);

                    // ES3.g3:1286:17: (name= Identifier )?
                    int alt69 = 2;
                    int LA69_0 = input.LA(1);

                    if ((LA69_0 == Identifier))
                    {
                        alt69 = 1;
                    }
                    switch (alt69)
                    {
                        case 1:
                            // ES3.g3:1286:17: name= Identifier
                            {
                                name = (IToken)Match(input, Identifier, FOLLOW_Identifier_in_functionExpression5660);
                                stream_Identifier.Add(name);


                            }
                            break;

                    }

                    PushFollow(FOLLOW_formalParameterList_in_functionExpression5663);
                    formalParameterList268 = formalParameterList();
                    state.followingStackPointer--;

                    stream_formalParameterList.Add(formalParameterList268.Tree);
                    PushFollow(FOLLOW_functionBody_in_functionExpression5665);
                    functionBody269 = functionBody();
                    state.followingStackPointer--;

                    stream_functionBody.Add(functionBody269.Tree);


                    // AST REWRITE
                    // elements:          formalParameterList, functionBody, FUNCTION, name
                    // token labels:      name
                    // rule labels:       retval
                    // token list labels: 
                    // rule list labels:  
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleTokenStream stream_name = new RewriteRuleTokenStream(adaptor, "token name", name);
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                    root_0 = (object)adaptor.GetNilNode();
                    // 1287:2: -> ^( FUNCTION ( $name)? formalParameterList functionBody )
                    {
                        // ES3.g3:1287:5: ^( FUNCTION ( $name)? formalParameterList functionBody )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot(stream_FUNCTION.NextNode(), root_1);

                            // ES3.g3:1287:17: ( $name)?
                            if (stream_name.HasNext())
                            {
                                adaptor.AddChild(root_1, stream_name.NextNode());

                            }
                            stream_name.Reset();
                            adaptor.AddChild(root_1, stream_formalParameterList.NextTree());
                            adaptor.AddChild(root_1, stream_functionBody.NextTree());

                            adaptor.AddChild(root_0, root_1);
                        }

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "functionExpression"

        public class formalParameterList_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "formalParameterList"
        // ES3.g3:1290:1: formalParameterList : LPAREN ( Identifier ( COMMA Identifier )* )? RPAREN -> ^( ARGS ( Identifier )* ) ;
        public ES3Parser.formalParameterList_return formalParameterList() // throws RecognitionException [1]
        {
            ES3Parser.formalParameterList_return retval = new ES3Parser.formalParameterList_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken LPAREN270 = null;
            IToken Identifier271 = null;
            IToken COMMA272 = null;
            IToken Identifier273 = null;
            IToken RPAREN274 = null;

            object LPAREN270_tree = null;
            object Identifier271_tree = null;
            object COMMA272_tree = null;
            object Identifier273_tree = null;
            object RPAREN274_tree = null;
            RewriteRuleTokenStream stream_RPAREN = new RewriteRuleTokenStream(adaptor, "token RPAREN");
            RewriteRuleTokenStream stream_COMMA = new RewriteRuleTokenStream(adaptor, "token COMMA");
            RewriteRuleTokenStream stream_Identifier = new RewriteRuleTokenStream(adaptor, "token Identifier");
            RewriteRuleTokenStream stream_LPAREN = new RewriteRuleTokenStream(adaptor, "token LPAREN");

            try
            {
                // ES3.g3:1291:2: ( LPAREN ( Identifier ( COMMA Identifier )* )? RPAREN -> ^( ARGS ( Identifier )* ) )
                // ES3.g3:1291:4: LPAREN ( Identifier ( COMMA Identifier )* )? RPAREN
                {
                    LPAREN270 = (IToken)Match(input, LPAREN, FOLLOW_LPAREN_in_formalParameterList5693);
                    stream_LPAREN.Add(LPAREN270);

                    // ES3.g3:1291:11: ( Identifier ( COMMA Identifier )* )?
                    int alt71 = 2;
                    int LA71_0 = input.LA(1);

                    if ((LA71_0 == Identifier))
                    {
                        alt71 = 1;
                    }
                    switch (alt71)
                    {
                        case 1:
                            // ES3.g3:1291:13: Identifier ( COMMA Identifier )*
                            {
                                Identifier271 = (IToken)Match(input, Identifier, FOLLOW_Identifier_in_formalParameterList5697);
                                stream_Identifier.Add(Identifier271);

                                // ES3.g3:1291:24: ( COMMA Identifier )*
                                do
                                {
                                    int alt70 = 2;
                                    int LA70_0 = input.LA(1);

                                    if ((LA70_0 == COMMA))
                                    {
                                        alt70 = 1;
                                    }


                                    switch (alt70)
                                    {
                                        case 1:
                                            // ES3.g3:1291:26: COMMA Identifier
                                            {
                                                COMMA272 = (IToken)Match(input, COMMA, FOLLOW_COMMA_in_formalParameterList5701);
                                                stream_COMMA.Add(COMMA272);

                                                Identifier273 = (IToken)Match(input, Identifier, FOLLOW_Identifier_in_formalParameterList5703);
                                                stream_Identifier.Add(Identifier273);


                                            }
                                            break;

                                        default:
                                            goto loop70;
                                    }
                                } while (true);

                            loop70:
                                ;	// Stops C# compiler whining that label 'loop70' has no statements


                            }
                            break;

                    }

                    RPAREN274 = (IToken)Match(input, RPAREN, FOLLOW_RPAREN_in_formalParameterList5711);
                    stream_RPAREN.Add(RPAREN274);



                    // AST REWRITE
                    // elements:          Identifier
                    // token labels:      
                    // rule labels:       retval
                    // token list labels: 
                    // rule list labels:  
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                    root_0 = (object)adaptor.GetNilNode();
                    // 1292:2: -> ^( ARGS ( Identifier )* )
                    {
                        // ES3.g3:1292:5: ^( ARGS ( Identifier )* )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(ARGS, "ARGS"), root_1);

                            // ES3.g3:1292:13: ( Identifier )*
                            while (stream_Identifier.HasNext())
                            {
                                adaptor.AddChild(root_1, stream_Identifier.NextNode());

                            }
                            stream_Identifier.Reset();

                            adaptor.AddChild(root_0, root_1);
                        }

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "formalParameterList"

        public class functionBody_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "functionBody"
        // ES3.g3:1295:1: functionBody : lb= LBRACE ( sourceElement )* RBRACE -> ^( BLOCK[$lb, \"BLOCK\"] ( sourceElement )* ) ;
        public ES3Parser.functionBody_return functionBody() // throws RecognitionException [1]
        {
            ES3Parser.functionBody_return retval = new ES3Parser.functionBody_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            IToken lb = null;
            IToken RBRACE276 = null;
            ES3Parser.sourceElement_return sourceElement275 = default(ES3Parser.sourceElement_return);


            object lb_tree = null;
            object RBRACE276_tree = null;
            RewriteRuleTokenStream stream_RBRACE = new RewriteRuleTokenStream(adaptor, "token RBRACE");
            RewriteRuleTokenStream stream_LBRACE = new RewriteRuleTokenStream(adaptor, "token LBRACE");
            RewriteRuleSubtreeStream stream_sourceElement = new RewriteRuleSubtreeStream(adaptor, "rule sourceElement");
            try
            {
                // ES3.g3:1296:2: (lb= LBRACE ( sourceElement )* RBRACE -> ^( BLOCK[$lb, \"BLOCK\"] ( sourceElement )* ) )
                // ES3.g3:1296:4: lb= LBRACE ( sourceElement )* RBRACE
                {
                    lb = (IToken)Match(input, LBRACE, FOLLOW_LBRACE_in_functionBody5736);
                    stream_LBRACE.Add(lb);

                    // ES3.g3:1296:14: ( sourceElement )*
                    do
                    {
                        int alt72 = 2;
                        int LA72_0 = input.LA(1);

                        if (((LA72_0 >= NULL && LA72_0 <= BREAK) || LA72_0 == CONTINUE || (LA72_0 >= DELETE && LA72_0 <= DO) || (LA72_0 >= FOR && LA72_0 <= IF) || (LA72_0 >= NEW && LA72_0 <= WITH) || LA72_0 == LBRACE || LA72_0 == LPAREN || LA72_0 == LBRACK || LA72_0 == SEMIC || (LA72_0 >= ADD && LA72_0 <= SUB) || (LA72_0 >= INC && LA72_0 <= DEC) || (LA72_0 >= NOT && LA72_0 <= INV) || (LA72_0 >= Identifier && LA72_0 <= StringLiteral) || LA72_0 == RegularExpressionLiteral || (LA72_0 >= DecimalLiteral && LA72_0 <= HexIntegerLiteral)))
                        {
                            alt72 = 1;
                        }


                        switch (alt72)
                        {
                            case 1:
                                // ES3.g3:1296:14: sourceElement
                                {
                                    PushFollow(FOLLOW_sourceElement_in_functionBody5738);
                                    sourceElement275 = sourceElement();
                                    state.followingStackPointer--;

                                    stream_sourceElement.Add(sourceElement275.Tree);

                                }
                                break;

                            default:
                                goto loop72;
                        }
                    } while (true);

                loop72:
                    ;	// Stops C# compiler whining that label 'loop72' has no statements

                    RBRACE276 = (IToken)Match(input, RBRACE, FOLLOW_RBRACE_in_functionBody5741);
                    stream_RBRACE.Add(RBRACE276);



                    // AST REWRITE
                    // elements:          sourceElement
                    // token labels:      
                    // rule labels:       retval
                    // token list labels: 
                    // rule list labels:  
                    // wildcard labels: 
                    retval.Tree = root_0;
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.Tree : null);

                    root_0 = (object)adaptor.GetNilNode();
                    // 1297:2: -> ^( BLOCK[$lb, \"BLOCK\"] ( sourceElement )* )
                    {
                        // ES3.g3:1297:5: ^( BLOCK[$lb, \"BLOCK\"] ( sourceElement )* )
                        {
                            object root_1 = (object)adaptor.GetNilNode();
                            root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(BLOCK, lb, "BLOCK"), root_1);

                            // ES3.g3:1297:28: ( sourceElement )*
                            while (stream_sourceElement.HasNext())
                            {
                                adaptor.AddChild(root_1, stream_sourceElement.NextTree());

                            }
                            stream_sourceElement.Reset();

                            adaptor.AddChild(root_0, root_1);
                        }

                    }

                    retval.Tree = root_0; retval.Tree = root_0;
                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "functionBody"

        public class program_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "program"
        // ES3.g3:1304:1: program : ( sourceElement )* ;
        public ES3Parser.program_return program() // throws RecognitionException [1]
        {
            ES3Parser.program_return retval = new ES3Parser.program_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            ES3Parser.sourceElement_return sourceElement277 = default(ES3Parser.sourceElement_return);



            try
            {
                // ES3.g3:1305:2: ( ( sourceElement )* )
                // ES3.g3:1305:4: ( sourceElement )*
                {
                    root_0 = (object)adaptor.GetNilNode();

                    // ES3.g3:1305:4: ( sourceElement )*
                    do
                    {
                        int alt73 = 2;
                        int LA73_0 = input.LA(1);

                        if (((LA73_0 >= NULL && LA73_0 <= BREAK) || LA73_0 == CONTINUE || (LA73_0 >= DELETE && LA73_0 <= DO) || (LA73_0 >= FOR && LA73_0 <= IF) || (LA73_0 >= NEW && LA73_0 <= WITH) || LA73_0 == LBRACE || LA73_0 == LPAREN || LA73_0 == LBRACK || LA73_0 == SEMIC || (LA73_0 >= ADD && LA73_0 <= SUB) || (LA73_0 >= INC && LA73_0 <= DEC) || (LA73_0 >= NOT && LA73_0 <= INV) || (LA73_0 >= Identifier && LA73_0 <= StringLiteral) || LA73_0 == RegularExpressionLiteral || (LA73_0 >= DecimalLiteral && LA73_0 <= HexIntegerLiteral)))
                        {
                            alt73 = 1;
                        }


                        switch (alt73)
                        {
                            case 1:
                                // ES3.g3:1305:4: sourceElement
                                {
                                    PushFollow(FOLLOW_sourceElement_in_program5770);
                                    sourceElement277 = sourceElement();
                                    state.followingStackPointer--;

                                    adaptor.AddChild(root_0, sourceElement277.Tree);

                                }
                                break;

                            default:
                                goto loop73;
                        }
                    } while (true);

                loop73:
                    ;	// Stops C# compiler whining that label 'loop73' has no statements


                }

                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "program"

        public class sourceElement_return : ParserRuleReturnScope
        {
            private object tree;
            override public object Tree
            {
                get { return tree; }
                set { tree = (object)value; }
            }
        };

        // $ANTLR start "sourceElement"
        // ES3.g3:1313:1: sourceElement options {k=1; } : ({...}? functionDeclaration | statement );
        public ES3Parser.sourceElement_return sourceElement() // throws RecognitionException [1]
        {
            ES3Parser.sourceElement_return retval = new ES3Parser.sourceElement_return();
            retval.Start = input.LT(1);

            object root_0 = null;

            ES3Parser.functionDeclaration_return functionDeclaration278 = default(ES3Parser.functionDeclaration_return);

            ES3Parser.statement_return statement279 = default(ES3Parser.statement_return);



            try
            {
                // ES3.g3:1318:2: ({...}? functionDeclaration | statement )
                int alt74 = 2;
                alt74 = dfa74.Predict(input);
                switch (alt74)
                {
                    case 1:
                        // ES3.g3:1318:4: {...}? functionDeclaration
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            if (!((input.LA(1) == FUNCTION)))
                            {
                                throw new FailedPredicateException(input, "sourceElement", " input.LA(1) == FUNCTION ");
                            }
                            PushFollow(FOLLOW_functionDeclaration_in_sourceElement5799);
                            functionDeclaration278 = functionDeclaration();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, functionDeclaration278.Tree);

                        }
                        break;
                    case 2:
                        // ES3.g3:1319:4: statement
                        {
                            root_0 = (object)adaptor.GetNilNode();

                            PushFollow(FOLLOW_statement_in_sourceElement5804);
                            statement279 = statement();
                            state.followingStackPointer--;

                            adaptor.AddChild(root_0, statement279.Tree);

                        }
                        break;

                }
                retval.Stop = input.LT(-1);

                retval.Tree = (object)adaptor.RulePostProcessing(root_0);
                adaptor.SetTokenBoundaries(retval.Tree, (IToken)retval.Start, (IToken)retval.Stop);
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(input, re);
                // Conversion of the second argument necessary, but harmless
                retval.Tree = (object)adaptor.ErrorNode(input, (IToken)retval.Start, input.LT(-1), re);

            }
            finally
            {
            }
            return retval;
        }
        // $ANTLR end "sourceElement"

        // Delegated rules


        protected DFA43 dfa43;
        protected DFA44 dfa44;
        protected DFA74 dfa74;
        private void InitializeCyclicDFAs()
        {
            this.dfa43 = new DFA43(this);
            this.dfa44 = new DFA44(this);
            this.dfa74 = new DFA74(this);
            this.dfa43.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA43_SpecialStateTransition);
            this.dfa74.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA74_SpecialStateTransition);
        }

        const string DFA43_eotS =
            "\x24\uffff";
        const string DFA43_eofS =
            "\x24\uffff";
        const string DFA43_minS =
            "\x01\x04\x01\x00\x22\uffff";
        const string DFA43_maxS =
            "\x01\u00a1\x01\x00\x22\uffff";
        const string DFA43_acceptS =
            "\x02\uffff\x01\x02\x20\uffff\x01\x01";
        const string DFA43_specialS =
            "\x01\uffff\x01\x00\x22\uffff}>";
        static readonly string[] DFA43_transitionS = {
            "\x04\x02\x02\uffff\x01\x02\x01\uffff\x02\x02\x02\uffff\x03"+
            "\x02\x02\uffff\x0b\x02\x1f\uffff\x01\x01\x01\uffff\x01\x02\x01"+
            "\uffff\x01\x02\x02\uffff\x01\x02\x09\uffff\x02\x02\x02\uffff"+
            "\x02\x02\x06\uffff\x02\x02\x36\uffff\x02\x02\x05\uffff\x01\x02"+
            "\x03\uffff\x03\x02",
            "\x01\uffff",
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
            ""
    };

        static readonly short[] DFA43_eot = DFA.UnpackEncodedString(DFA43_eotS);
        static readonly short[] DFA43_eof = DFA.UnpackEncodedString(DFA43_eofS);
        static readonly char[] DFA43_min = DFA.UnpackEncodedStringToUnsignedChars(DFA43_minS);
        static readonly char[] DFA43_max = DFA.UnpackEncodedStringToUnsignedChars(DFA43_maxS);
        static readonly short[] DFA43_accept = DFA.UnpackEncodedString(DFA43_acceptS);
        static readonly short[] DFA43_special = DFA.UnpackEncodedString(DFA43_specialS);
        static readonly short[][] DFA43_transition = DFA.UnpackEncodedStringArray(DFA43_transitionS);

        protected class DFA43 : DFA
        {
            public DFA43(BaseRecognizer recognizer)
            {
                this.recognizer = recognizer;
                this.decisionNumber = 43;
                this.eot = DFA43_eot;
                this.eof = DFA43_eof;
                this.min = DFA43_min;
                this.max = DFA43_max;
                this.accept = DFA43_accept;
                this.special = DFA43_special;
                this.transition = DFA43_transition;

            }

            override public string Description
            {
                get { return "956:1: statement options {k=1; } : ({...}? block | statementTail );"; }
            }

        }


        protected internal int DFA43_SpecialStateTransition(DFA dfa, int s, IIntStream _input) //throws NoViableAltException
        {
            ITokenStream input = (ITokenStream)_input;
            int _s = s;
            switch (s)
            {
                case 0:
                    int LA43_1 = input.LA(1);


                    int index43_1 = input.Index();
                    input.Rewind();
                    s = -1;
                    if (((input.LA(1) == LBRACE))) { s = 35; }

                    else if ((true)) { s = 2; }


                    input.Seek(index43_1);
                    if (s >= 0) return s;
                    break;
            }
            NoViableAltException nvae43 =
                new NoViableAltException(dfa.Description, 43, _s, input);
            dfa.Error(nvae43);
            throw nvae43;
        }
        const string DFA44_eotS =
            "\x0f\uffff";
        const string DFA44_eofS =
            "\x04\uffff\x01\x03\x0a\uffff";
        const string DFA44_minS =
            "\x01\x04\x03\uffff\x01\x13\x0a\uffff";
        const string DFA44_maxS =
            "\x01\u00a1\x03\uffff\x01\u0092\x0a\uffff";
        const string DFA44_acceptS =
            "\x01\uffff\x01\x01\x01\x02\x01\x03\x01\uffff\x01\x04\x01\x05\x01" +
            "\x06\x01\x07\x01\x08\x01\x09\x01\x0b\x01\x0c\x01\x0d\x01\x0a";
        const string DFA44_specialS =
            "\x0f\uffff}>";
        static readonly string[] DFA44_transitionS = {
            "\x03\x03\x01\x08\x02\uffff\x01\x07\x01\uffff\x01\x03\x01\x06"+
            "\x02\uffff\x01\x06\x01\x03\x01\x05\x02\uffff\x01\x03\x01\x09"+
            "\x01\x0b\x01\x03\x01\x0c\x01\x0d\x01\x03\x01\x01\x01\x03\x01"+
            "\x06\x01\x0a\x1f\uffff\x01\x03\x01\uffff\x01\x03\x01\uffff\x01"+
            "\x03\x02\uffff\x01\x02\x09\uffff\x02\x03\x02\uffff\x02\x03\x06"+
            "\uffff\x02\x03\x36\uffff\x01\x04\x01\x03\x05\uffff\x01\x03\x03"+
            "\uffff\x03\x03",
            "",
            "",
            "",
            "\x02\x03\x2b\uffff\x02\x03\x01\uffff\x01\x03\x01\uffff\x17"+
            "\x03\x02\uffff\x03\x03\x01\x0e\x0d\x03\x22\uffff\x02\x03",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
    };

        static readonly short[] DFA44_eot = DFA.UnpackEncodedString(DFA44_eotS);
        static readonly short[] DFA44_eof = DFA.UnpackEncodedString(DFA44_eofS);
        static readonly char[] DFA44_min = DFA.UnpackEncodedStringToUnsignedChars(DFA44_minS);
        static readonly char[] DFA44_max = DFA.UnpackEncodedStringToUnsignedChars(DFA44_maxS);
        static readonly short[] DFA44_accept = DFA.UnpackEncodedString(DFA44_acceptS);
        static readonly short[] DFA44_special = DFA.UnpackEncodedString(DFA44_specialS);
        static readonly short[][] DFA44_transition = DFA.UnpackEncodedStringArray(DFA44_transitionS);

        protected class DFA44 : DFA
        {
            public DFA44(BaseRecognizer recognizer)
            {
                this.recognizer = recognizer;
                this.decisionNumber = 44;
                this.eot = DFA44_eot;
                this.eof = DFA44_eof;
                this.min = DFA44_min;
                this.max = DFA44_max;
                this.accept = DFA44_accept;
                this.special = DFA44_special;
                this.transition = DFA44_transition;

            }

            override public string Description
            {
                get { return "965:1: statementTail : ( variableStatement | emptyStatement | expressionStatement | ifStatement | iterationStatement | continueStatement | breakStatement | returnStatement | withStatement | labelledStatement | switchStatement | throwStatement | tryStatement );"; }
            }

        }

        const string DFA74_eotS =
            "\x24\uffff";
        const string DFA74_eofS =
            "\x24\uffff";
        const string DFA74_minS =
            "\x01\x04\x01\x00\x22\uffff";
        const string DFA74_maxS =
            "\x01\u00a1\x01\x00\x22\uffff";
        const string DFA74_acceptS =
            "\x02\uffff\x01\x02\x20\uffff\x01\x01";
        const string DFA74_specialS =
            "\x01\uffff\x01\x00\x22\uffff}>";
        static readonly string[] DFA74_transitionS = {
            "\x04\x02\x02\uffff\x01\x02\x01\uffff\x02\x02\x02\uffff\x01"+
            "\x02\x01\x01\x01\x02\x02\uffff\x0b\x02\x1f\uffff\x01\x02\x01"+
            "\uffff\x01\x02\x01\uffff\x01\x02\x02\uffff\x01\x02\x09\uffff"+
            "\x02\x02\x02\uffff\x02\x02\x06\uffff\x02\x02\x36\uffff\x02\x02"+
            "\x05\uffff\x01\x02\x03\uffff\x03\x02",
            "\x01\uffff",
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
            ""
    };

        static readonly short[] DFA74_eot = DFA.UnpackEncodedString(DFA74_eotS);
        static readonly short[] DFA74_eof = DFA.UnpackEncodedString(DFA74_eofS);
        static readonly char[] DFA74_min = DFA.UnpackEncodedStringToUnsignedChars(DFA74_minS);
        static readonly char[] DFA74_max = DFA.UnpackEncodedStringToUnsignedChars(DFA74_maxS);
        static readonly short[] DFA74_accept = DFA.UnpackEncodedString(DFA74_acceptS);
        static readonly short[] DFA74_special = DFA.UnpackEncodedString(DFA74_specialS);
        static readonly short[][] DFA74_transition = DFA.UnpackEncodedStringArray(DFA74_transitionS);

        protected class DFA74 : DFA
        {
            public DFA74(BaseRecognizer recognizer)
            {
                this.recognizer = recognizer;
                this.decisionNumber = 74;
                this.eot = DFA74_eot;
                this.eof = DFA74_eof;
                this.min = DFA74_min;
                this.max = DFA74_max;
                this.accept = DFA74_accept;
                this.special = DFA74_special;
                this.transition = DFA74_transition;

            }

            override public string Description
            {
                get { return "1313:1: sourceElement options {k=1; } : ({...}? functionDeclaration | statement );"; }
            }

        }


        protected internal int DFA74_SpecialStateTransition(DFA dfa, int s, IIntStream _input) //throws NoViableAltException
        {
            ITokenStream input = (ITokenStream)_input;
            int _s = s;
            switch (s)
            {
                case 0:
                    int LA74_1 = input.LA(1);


                    int index74_1 = input.Index();
                    input.Rewind();
                    s = -1;
                    if (((input.LA(1) == FUNCTION))) { s = 35; }

                    else if ((true)) { s = 2; }


                    input.Seek(index74_1);
                    if (s >= 0) return s;
                    break;
            }
            NoViableAltException nvae74 =
                new NoViableAltException(dfa.Description, 74, _s, input);
            dfa.Error(nvae74);
            throw nvae74;
        }


        public static readonly BitSet FOLLOW_reservedWord_in_token1753 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_Identifier_in_token1758 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_punctuator_in_token1763 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_numericLiteral_in_token1768 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_StringLiteral_in_token1773 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_keyword_in_reservedWord1786 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_futureReservedWord_in_reservedWord1791 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_NULL_in_reservedWord1796 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_booleanLiteral_in_reservedWord1801 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_set_in_keyword0 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_set_in_futureReservedWord0 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_set_in_punctuator0 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_NULL_in_literal2482 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_booleanLiteral_in_literal2487 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_numericLiteral_in_literal2492 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_StringLiteral_in_literal2497 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_RegularExpressionLiteral_in_literal2502 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_set_in_booleanLiteral0 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_set_in_numericLiteral0 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_THIS_in_primaryExpression3115 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_Identifier_in_primaryExpression3120 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_literal_in_primaryExpression3125 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_arrayLiteral_in_primaryExpression3130 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_objectLiteral_in_primaryExpression3135 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_LPAREN_in_primaryExpression3142 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_expression_in_primaryExpression3144 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000004UL });
        public static readonly BitSet FOLLOW_RPAREN_in_primaryExpression3146 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_LBRACK_in_arrayLiteral3170 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033009AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_arrayItem_in_arrayLiteral3174 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000090UL });
        public static readonly BitSet FOLLOW_COMMA_in_arrayLiteral3178 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033009AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_arrayItem_in_arrayLiteral3180 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000090UL });
        public static readonly BitSet FOLLOW_RBRACK_in_arrayLiteral3188 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_assignmentExpression_in_arrayItem3216 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_LBRACE_in_objectLiteral3248 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000001UL, 0x0000000380300000UL });
        public static readonly BitSet FOLLOW_nameValuePair_in_objectLiteral3252 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000081UL });
        public static readonly BitSet FOLLOW_COMMA_in_objectLiteral3256 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000000UL, 0x0000000380300000UL });
        public static readonly BitSet FOLLOW_nameValuePair_in_objectLiteral3258 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000081UL });
        public static readonly BitSet FOLLOW_RBRACE_in_objectLiteral3266 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_propertyName_in_nameValuePair3291 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000200000000UL });
        public static readonly BitSet FOLLOW_COLON_in_nameValuePair3293 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_assignmentExpression_in_nameValuePair3295 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_Identifier_in_propertyName3319 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_StringLiteral_in_propertyName3324 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_numericLiteral_in_propertyName3329 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_primaryExpression_in_memberExpression3347 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_functionExpression_in_memberExpression3352 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_newExpression_in_memberExpression3357 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_NEW_in_newExpression3368 = new BitSet(new ulong[] { 0x8000000001000070UL, 0x000000000000000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_primaryExpression_in_newExpression3371 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_LPAREN_in_arguments3384 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000EUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_assignmentExpression_in_arguments3388 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000084UL });
        public static readonly BitSet FOLLOW_COMMA_in_arguments3392 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_assignmentExpression_in_arguments3394 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000084UL });
        public static readonly BitSet FOLLOW_RPAREN_in_arguments3402 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_memberExpression_in_leftHandSideExpression3431 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x000000000000002AUL });
        public static readonly BitSet FOLLOW_arguments_in_leftHandSideExpression3447 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x000000000000002AUL });
        public static readonly BitSet FOLLOW_LBRACK_in_leftHandSideExpression3468 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_expression_in_leftHandSideExpression3470 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000010UL });
        public static readonly BitSet FOLLOW_RBRACK_in_leftHandSideExpression3472 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x000000000000002AUL });
        public static readonly BitSet FOLLOW_DOT_in_leftHandSideExpression3491 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000000UL, 0x0000000000100000UL });
        public static readonly BitSet FOLLOW_Identifier_in_leftHandSideExpression3493 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x000000000000002AUL });
        public static readonly BitSet FOLLOW_leftHandSideExpression_in_postfixExpression3528 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000000300000UL });
        public static readonly BitSet FOLLOW_postfixOperator_in_postfixExpression3534 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_INC_in_postfixOperator3552 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_DEC_in_postfixOperator3561 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_postfixExpression_in_unaryExpression3578 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_unaryOperator_in_unaryExpression3583 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_unaryExpression_in_unaryExpression3586 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_DELETE_in_unaryOperator3598 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_VOID_in_unaryOperator3603 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_TYPEOF_in_unaryOperator3608 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_INC_in_unaryOperator3613 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_DEC_in_unaryOperator3618 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_ADD_in_unaryOperator3625 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_SUB_in_unaryOperator3634 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_INV_in_unaryOperator3641 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_NOT_in_unaryOperator3646 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_unaryExpression_in_multiplicativeExpression3661 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x00002000000C0000UL });
        public static readonly BitSet FOLLOW_set_in_multiplicativeExpression3665 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_unaryExpression_in_multiplicativeExpression3680 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x00002000000C0000UL });
        public static readonly BitSet FOLLOW_multiplicativeExpression_in_additiveExpression3698 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000000030000UL });
        public static readonly BitSet FOLLOW_set_in_additiveExpression3702 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_multiplicativeExpression_in_additiveExpression3713 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000000030000UL });
        public static readonly BitSet FOLLOW_additiveExpression_in_shiftExpression3732 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000001C00000UL });
        public static readonly BitSet FOLLOW_set_in_shiftExpression3736 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_additiveExpression_in_shiftExpression3751 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000001C00000UL });
        public static readonly BitSet FOLLOW_shiftExpression_in_relationalExpression3770 = new BitSet(new ulong[] { 0x0000000000180002UL, 0x0000000000000F00UL });
        public static readonly BitSet FOLLOW_set_in_relationalExpression3774 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_shiftExpression_in_relationalExpression3801 = new BitSet(new ulong[] { 0x0000000000180002UL, 0x0000000000000F00UL });
        public static readonly BitSet FOLLOW_shiftExpression_in_relationalExpressionNoIn3815 = new BitSet(new ulong[] { 0x0000000000100002UL, 0x0000000000000F00UL });
        public static readonly BitSet FOLLOW_set_in_relationalExpressionNoIn3819 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_shiftExpression_in_relationalExpressionNoIn3842 = new BitSet(new ulong[] { 0x0000000000100002UL, 0x0000000000000F00UL });
        public static readonly BitSet FOLLOW_relationalExpression_in_equalityExpression3861 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x000000000000F000UL });
        public static readonly BitSet FOLLOW_set_in_equalityExpression3865 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_relationalExpression_in_equalityExpression3884 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x000000000000F000UL });
        public static readonly BitSet FOLLOW_relationalExpressionNoIn_in_equalityExpressionNoIn3898 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x000000000000F000UL });
        public static readonly BitSet FOLLOW_set_in_equalityExpressionNoIn3902 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_relationalExpressionNoIn_in_equalityExpressionNoIn3921 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x000000000000F000UL });
        public static readonly BitSet FOLLOW_equalityExpression_in_bitwiseANDExpression3941 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000002000000UL });
        public static readonly BitSet FOLLOW_AND_in_bitwiseANDExpression3945 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_equalityExpression_in_bitwiseANDExpression3948 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000002000000UL });
        public static readonly BitSet FOLLOW_equalityExpressionNoIn_in_bitwiseANDExpressionNoIn3962 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000002000000UL });
        public static readonly BitSet FOLLOW_AND_in_bitwiseANDExpressionNoIn3966 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_equalityExpressionNoIn_in_bitwiseANDExpressionNoIn3969 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000002000000UL });
        public static readonly BitSet FOLLOW_bitwiseANDExpression_in_bitwiseXORExpression3985 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000008000000UL });
        public static readonly BitSet FOLLOW_XOR_in_bitwiseXORExpression3989 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_bitwiseANDExpression_in_bitwiseXORExpression3992 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000008000000UL });
        public static readonly BitSet FOLLOW_bitwiseANDExpressionNoIn_in_bitwiseXORExpressionNoIn4008 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000008000000UL });
        public static readonly BitSet FOLLOW_XOR_in_bitwiseXORExpressionNoIn4012 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_bitwiseANDExpressionNoIn_in_bitwiseXORExpressionNoIn4015 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000008000000UL });
        public static readonly BitSet FOLLOW_bitwiseXORExpression_in_bitwiseORExpression4030 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000004000000UL });
        public static readonly BitSet FOLLOW_OR_in_bitwiseORExpression4034 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_bitwiseXORExpression_in_bitwiseORExpression4037 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000004000000UL });
        public static readonly BitSet FOLLOW_bitwiseXORExpressionNoIn_in_bitwiseORExpressionNoIn4052 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000004000000UL });
        public static readonly BitSet FOLLOW_OR_in_bitwiseORExpressionNoIn4056 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_bitwiseXORExpressionNoIn_in_bitwiseORExpressionNoIn4059 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000004000000UL });
        public static readonly BitSet FOLLOW_bitwiseORExpression_in_logicalANDExpression4078 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000040000000UL });
        public static readonly BitSet FOLLOW_LAND_in_logicalANDExpression4082 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_bitwiseORExpression_in_logicalANDExpression4085 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000040000000UL });
        public static readonly BitSet FOLLOW_bitwiseORExpressionNoIn_in_logicalANDExpressionNoIn4099 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000040000000UL });
        public static readonly BitSet FOLLOW_LAND_in_logicalANDExpressionNoIn4103 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_bitwiseORExpressionNoIn_in_logicalANDExpressionNoIn4106 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000040000000UL });
        public static readonly BitSet FOLLOW_logicalANDExpression_in_logicalORExpression4121 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000080000000UL });
        public static readonly BitSet FOLLOW_LOR_in_logicalORExpression4125 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_logicalANDExpression_in_logicalORExpression4128 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000080000000UL });
        public static readonly BitSet FOLLOW_logicalANDExpressionNoIn_in_logicalORExpressionNoIn4143 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000080000000UL });
        public static readonly BitSet FOLLOW_LOR_in_logicalORExpressionNoIn4147 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_logicalANDExpressionNoIn_in_logicalORExpressionNoIn4150 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000080000000UL });
        public static readonly BitSet FOLLOW_logicalORExpression_in_conditionalExpression4169 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000100000000UL });
        public static readonly BitSet FOLLOW_QUE_in_conditionalExpression4173 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_assignmentExpression_in_conditionalExpression4176 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000200000000UL });
        public static readonly BitSet FOLLOW_COLON_in_conditionalExpression4178 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_assignmentExpression_in_conditionalExpression4181 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_logicalORExpressionNoIn_in_conditionalExpressionNoIn4195 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000100000000UL });
        public static readonly BitSet FOLLOW_QUE_in_conditionalExpressionNoIn4199 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_assignmentExpressionNoIn_in_conditionalExpressionNoIn4202 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000200000000UL });
        public static readonly BitSet FOLLOW_COLON_in_conditionalExpressionNoIn4204 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_assignmentExpressionNoIn_in_conditionalExpressionNoIn4207 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_conditionalExpression_in_assignmentExpression4235 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x00005FFC00000000UL });
        public static readonly BitSet FOLLOW_assignmentOperator_in_assignmentExpression4242 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_assignmentExpression_in_assignmentExpression4245 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_set_in_assignmentOperator0 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_conditionalExpressionNoIn_in_assignmentExpressionNoIn4322 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x00005FFC00000000UL });
        public static readonly BitSet FOLLOW_assignmentOperator_in_assignmentExpressionNoIn4329 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_assignmentExpressionNoIn_in_assignmentExpressionNoIn4332 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_assignmentExpression_in_expression4354 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000000000080UL });
        public static readonly BitSet FOLLOW_COMMA_in_expression4358 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_assignmentExpression_in_expression4362 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000000000080UL });
        public static readonly BitSet FOLLOW_assignmentExpressionNoIn_in_expressionNoIn4399 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000000000080UL });
        public static readonly BitSet FOLLOW_COMMA_in_expressionNoIn4403 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_assignmentExpressionNoIn_in_expressionNoIn4407 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000000000080UL });
        public static readonly BitSet FOLLOW_SEMIC_in_semic4458 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_EOF_in_semic4463 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_RBRACE_in_semic4468 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_EOL_in_semic4475 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_MultiLineComment_in_semic4479 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_block_in_statement4508 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_statementTail_in_statement4513 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_variableStatement_in_statementTail4525 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_emptyStatement_in_statementTail4530 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_expressionStatement_in_statementTail4535 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_ifStatement_in_statementTail4540 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_iterationStatement_in_statementTail4545 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_continueStatement_in_statementTail4550 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_breakStatement_in_statementTail4555 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_returnStatement_in_statementTail4560 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_withStatement_in_statementTail4565 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_labelledStatement_in_statementTail4570 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_switchStatement_in_statementTail4575 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_throwStatement_in_statementTail4580 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_tryStatement_in_statementTail4585 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_LBRACE_in_block4600 = new BitSet(new ulong[] { 0x80000000FFE734F0UL, 0x000000003033004BUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_statement_in_block4602 = new BitSet(new ulong[] { 0x80000000FFE734F0UL, 0x000000003033004BUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_RBRACE_in_block4605 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_VAR_in_variableStatement4634 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000000UL, 0x0000000000100000UL });
        public static readonly BitSet FOLLOW_variableDeclaration_in_variableStatement4636 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x00000000000000C1UL, 0x0000000000060000UL });
        public static readonly BitSet FOLLOW_COMMA_in_variableStatement4640 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000000UL, 0x0000000000100000UL });
        public static readonly BitSet FOLLOW_variableDeclaration_in_variableStatement4642 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x00000000000000C1UL, 0x0000000000060000UL });
        public static readonly BitSet FOLLOW_semic_in_variableStatement4647 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_Identifier_in_variableDeclaration4670 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000400000000UL });
        public static readonly BitSet FOLLOW_ASSIGN_in_variableDeclaration4674 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_assignmentExpression_in_variableDeclaration4677 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_Identifier_in_variableDeclarationNoIn4692 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000400000000UL });
        public static readonly BitSet FOLLOW_ASSIGN_in_variableDeclarationNoIn4696 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_assignmentExpressionNoIn_in_variableDeclarationNoIn4699 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_SEMIC_in_emptyStatement4718 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_expression_in_expressionStatement4737 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x00000000000000C1UL, 0x0000000000060000UL });
        public static readonly BitSet FOLLOW_semic_in_expressionStatement4739 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_IF_in_ifStatement4757 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_LPAREN_in_ifStatement4759 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_expression_in_ifStatement4761 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000004UL });
        public static readonly BitSet FOLLOW_RPAREN_in_ifStatement4763 = new BitSet(new ulong[] { 0x80000000FFE734F0UL, 0x000000003033004AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_statement_in_ifStatement4765 = new BitSet(new ulong[] { 0x0000000000004002UL });
        public static readonly BitSet FOLLOW_ELSE_in_ifStatement4771 = new BitSet(new ulong[] { 0x80000000FFE734F0UL, 0x000000003033004AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_statement_in_ifStatement4773 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_doStatement_in_iterationStatement4806 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_whileStatement_in_iterationStatement4811 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_forStatement_in_iterationStatement4816 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_DO_in_doStatement4828 = new BitSet(new ulong[] { 0x80000000FFE734F0UL, 0x000000003033004AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_statement_in_doStatement4830 = new BitSet(new ulong[] { 0x0000000040000000UL });
        public static readonly BitSet FOLLOW_WHILE_in_doStatement4832 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_LPAREN_in_doStatement4834 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_expression_in_doStatement4836 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000004UL });
        public static readonly BitSet FOLLOW_RPAREN_in_doStatement4838 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x00000000000000C1UL, 0x0000000000060000UL });
        public static readonly BitSet FOLLOW_semic_in_doStatement4840 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_WHILE_in_whileStatement4865 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_LPAREN_in_whileStatement4868 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_expression_in_whileStatement4871 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000004UL });
        public static readonly BitSet FOLLOW_RPAREN_in_whileStatement4873 = new BitSet(new ulong[] { 0x80000000FFE734F0UL, 0x000000003033004AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_statement_in_whileStatement4876 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_FOR_in_forStatement4889 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_LPAREN_in_forStatement4892 = new BitSet(new ulong[] { 0x8000000039221070UL, 0x000000003033004AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_forControl_in_forStatement4895 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000004UL });
        public static readonly BitSet FOLLOW_RPAREN_in_forStatement4897 = new BitSet(new ulong[] { 0x80000000FFE734F0UL, 0x000000003033004AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_statement_in_forStatement4900 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_forControlVar_in_forControl4911 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_forControlExpression_in_forControl4916 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_forControlSemic_in_forControl4921 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_VAR_in_forControlVar4932 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000000UL, 0x0000000000100000UL });
        public static readonly BitSet FOLLOW_variableDeclarationNoIn_in_forControlVar4934 = new BitSet(new ulong[] { 0x0000000000080000UL, 0x00000000000000C0UL });
        public static readonly BitSet FOLLOW_IN_in_forControlVar4946 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_expression_in_forControlVar4948 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_COMMA_in_forControlVar4994 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000000UL, 0x0000000000100000UL });
        public static readonly BitSet FOLLOW_variableDeclarationNoIn_in_forControlVar4996 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x00000000000000C0UL });
        public static readonly BitSet FOLLOW_SEMIC_in_forControlVar5001 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033004AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_expression_in_forControlVar5005 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000040UL });
        public static readonly BitSet FOLLOW_SEMIC_in_forControlVar5008 = new BitSet(new ulong[] { 0x8000000029221072UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_expression_in_forControlVar5012 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_expressionNoIn_in_forControlExpression5078 = new BitSet(new ulong[] { 0x0000000000080000UL, 0x0000000000000040UL });
        public static readonly BitSet FOLLOW_IN_in_forControlExpression5093 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_expression_in_forControlExpression5097 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_SEMIC_in_forControlExpression5143 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033004AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_expression_in_forControlExpression5147 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000040UL });
        public static readonly BitSet FOLLOW_SEMIC_in_forControlExpression5150 = new BitSet(new ulong[] { 0x8000000029221072UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_expression_in_forControlExpression5154 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_SEMIC_in_forControlSemic5213 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033004AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_expression_in_forControlSemic5217 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000040UL });
        public static readonly BitSet FOLLOW_SEMIC_in_forControlSemic5220 = new BitSet(new ulong[] { 0x8000000029221072UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_expression_in_forControlSemic5224 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_CONTINUE_in_continueStatement5278 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x00000000000000C1UL, 0x0000000000160000UL });
        public static readonly BitSet FOLLOW_Identifier_in_continueStatement5283 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x00000000000000C1UL, 0x0000000000060000UL });
        public static readonly BitSet FOLLOW_semic_in_continueStatement5286 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_BREAK_in_breakStatement5305 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x00000000000000C1UL, 0x0000000000160000UL });
        public static readonly BitSet FOLLOW_Identifier_in_breakStatement5310 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x00000000000000C1UL, 0x0000000000060000UL });
        public static readonly BitSet FOLLOW_semic_in_breakStatement5313 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_RETURN_in_returnStatement5332 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x00000000303300CBUL, 0x0000000388360000UL });
        public static readonly BitSet FOLLOW_expression_in_returnStatement5337 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x00000000000000C1UL, 0x0000000000060000UL });
        public static readonly BitSet FOLLOW_semic_in_returnStatement5340 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_WITH_in_withStatement5357 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_LPAREN_in_withStatement5360 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_expression_in_withStatement5363 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000004UL });
        public static readonly BitSet FOLLOW_RPAREN_in_withStatement5365 = new BitSet(new ulong[] { 0x80000000FFE734F0UL, 0x000000003033004AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_statement_in_withStatement5368 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_SWITCH_in_switchStatement5389 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_LPAREN_in_switchStatement5391 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_expression_in_switchStatement5393 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000004UL });
        public static readonly BitSet FOLLOW_RPAREN_in_switchStatement5395 = new BitSet(new ulong[] { 0x8000000000000000UL });
        public static readonly BitSet FOLLOW_LBRACE_in_switchStatement5397 = new BitSet(new ulong[] { 0x0000000000000900UL, 0x0000000000000001UL });
        public static readonly BitSet FOLLOW_defaultClause_in_switchStatement5404 = new BitSet(new ulong[] { 0x0000000000000900UL, 0x0000000000000001UL });
        public static readonly BitSet FOLLOW_caseClause_in_switchStatement5410 = new BitSet(new ulong[] { 0x0000000000000900UL, 0x0000000000000001UL });
        public static readonly BitSet FOLLOW_RBRACE_in_switchStatement5415 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_CASE_in_caseClause5443 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_expression_in_caseClause5446 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000200000000UL });
        public static readonly BitSet FOLLOW_COLON_in_caseClause5448 = new BitSet(new ulong[] { 0x80000000FFE734F2UL, 0x000000003033004AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_statement_in_caseClause5451 = new BitSet(new ulong[] { 0x80000000FFE734F2UL, 0x000000003033004AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_DEFAULT_in_defaultClause5464 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000200000000UL });
        public static readonly BitSet FOLLOW_COLON_in_defaultClause5467 = new BitSet(new ulong[] { 0x80000000FFE734F2UL, 0x000000003033004AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_statement_in_defaultClause5470 = new BitSet(new ulong[] { 0x80000000FFE734F2UL, 0x000000003033004AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_Identifier_in_labelledStatement5487 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000200000000UL });
        public static readonly BitSet FOLLOW_COLON_in_labelledStatement5489 = new BitSet(new ulong[] { 0x80000000FFE734F0UL, 0x000000003033004AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_statement_in_labelledStatement5491 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_THROW_in_throwStatement5522 = new BitSet(new ulong[] { 0x8000000029221070UL, 0x000000003033000AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_expression_in_throwStatement5527 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x00000000000000C1UL, 0x0000000000060000UL });
        public static readonly BitSet FOLLOW_semic_in_throwStatement5529 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_TRY_in_tryStatement5546 = new BitSet(new ulong[] { 0x8000000000000000UL });
        public static readonly BitSet FOLLOW_block_in_tryStatement5549 = new BitSet(new ulong[] { 0x0000000000008200UL });
        public static readonly BitSet FOLLOW_catchClause_in_tryStatement5553 = new BitSet(new ulong[] { 0x0000000000008202UL });
        public static readonly BitSet FOLLOW_finallyClause_in_tryStatement5555 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_finallyClause_in_tryStatement5560 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_CATCH_in_catchClause5574 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_LPAREN_in_catchClause5577 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000000UL, 0x0000000000100000UL });
        public static readonly BitSet FOLLOW_Identifier_in_catchClause5580 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000004UL });
        public static readonly BitSet FOLLOW_RPAREN_in_catchClause5582 = new BitSet(new ulong[] { 0x8000000000000000UL });
        public static readonly BitSet FOLLOW_block_in_catchClause5585 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_FINALLY_in_finallyClause5597 = new BitSet(new ulong[] { 0x8000000000000000UL });
        public static readonly BitSet FOLLOW_block_in_finallyClause5600 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_FUNCTION_in_functionDeclaration5621 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000000UL, 0x0000000000100000UL });
        public static readonly BitSet FOLLOW_Identifier_in_functionDeclaration5625 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_formalParameterList_in_functionDeclaration5627 = new BitSet(new ulong[] { 0x8000000000000000UL });
        public static readonly BitSet FOLLOW_functionBody_in_functionDeclaration5629 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_FUNCTION_in_functionExpression5656 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000002UL, 0x0000000000100000UL });
        public static readonly BitSet FOLLOW_Identifier_in_functionExpression5660 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_formalParameterList_in_functionExpression5663 = new BitSet(new ulong[] { 0x8000000000000000UL });
        public static readonly BitSet FOLLOW_functionBody_in_functionExpression5665 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_LPAREN_in_formalParameterList5693 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000004UL, 0x0000000000100000UL });
        public static readonly BitSet FOLLOW_Identifier_in_formalParameterList5697 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000084UL });
        public static readonly BitSet FOLLOW_COMMA_in_formalParameterList5701 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000000UL, 0x0000000000100000UL });
        public static readonly BitSet FOLLOW_Identifier_in_formalParameterList5703 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000000084UL });
        public static readonly BitSet FOLLOW_RPAREN_in_formalParameterList5711 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_LBRACE_in_functionBody5736 = new BitSet(new ulong[] { 0x80000000FFE734F0UL, 0x000000003033004BUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_sourceElement_in_functionBody5738 = new BitSet(new ulong[] { 0x80000000FFE734F0UL, 0x000000003033004BUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_RBRACE_in_functionBody5741 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_sourceElement_in_program5770 = new BitSet(new ulong[] { 0x80000000FFE734F2UL, 0x000000003033004AUL, 0x0000000388300000UL });
        public static readonly BitSet FOLLOW_functionDeclaration_in_sourceElement5799 = new BitSet(new ulong[] { 0x0000000000000002UL });
        public static readonly BitSet FOLLOW_statement_in_sourceElement5804 = new BitSet(new ulong[] { 0x0000000000000002UL });

    }
}