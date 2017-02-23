namespace Devsense.PHP.Syntax
{
	#region User Code
	
	// Copyright(c) DEVSENSE s.r.o.
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.
using System;
using System.Collections.Generic;
#endregion
	
	
	public partial class Lexer
	{
		public enum LexicalStates
		{
			INITIAL = 0,
			ST_IN_SCRIPTING = 1,
			ST_DOUBLE_QUOTES = 2,
			ST_SINGLE_QUOTES = 3,
			ST_BACKQUOTE = 4,
			ST_HEREDOC = 5,
			ST_NEWDOC = 6,
			ST_LOOKING_FOR_PROPERTY = 7,
			ST_LOOKING_FOR_VARNAME = 8,
			ST_DOC_COMMENT = 9,
			ST_COMMENT = 10,
			ST_ONE_LINE_COMMENT = 11,
			ST_VAR_OFFSET = 12,
			ST_END_HEREDOC = 13,
			ST_NOWDOC = 14,
			ST_HALT_COMPILER1 = 15,
			ST_HALT_COMPILER2 = 16,
			ST_HALT_COMPILER3 = 17,
			ST_IN_STRING = 18,
			ST_IN_SHELL = 19,
			ST_IN_HEREDOC = 20,
		}
		
		[Flags]
		private enum AcceptConditions : byte
		{
			NotAccept = 0,
			AcceptOnStart = 1,
			AcceptOnEnd = 2,
			Accept = 4
		}
		
		public struct Position
		{
			public int Char;
			public Position(int ch)
			{
				this.Char = ch;
			}
		}
		private const int NoState = -1;
		private const char BOL = (char)128;
		private const char EOF = (char)129;
		
		private Tokens yyreturn;
		
		private System.IO.TextReader reader;
		private char[] buffer = new char[512];
		
		// whether the currently parsed token is being expanded (yymore has been called):
		private bool expanding_token;
		
		// offset in buffer where the currently parsed token starts:
		private int token_start;
		
		// offset in buffer where the currently parsed token chunk starts:
		private int token_chunk_start;
		
		// offset in buffer one char behind the currently parsed token (chunk) ending character:
		private int token_end;
		
		// offset of the lookahead character (number of characters parsed):
		private int lookahead_index;
		
		// number of characters read into the buffer:
		private int chars_read;
		
		// parsed token start position (wrt beginning of the stream):
		protected Position token_start_pos;
		
		// parsed token end position (wrt beginning of the stream):
		protected Position token_end_pos;
		
		private bool yy_at_bol = false;
		
		public LexicalStates CurrentLexicalState { get { return current_lexical_state; } set { current_lexical_state = value; } } 
		private LexicalStates current_lexical_state;
		
		protected Lexer(System.IO.TextReader reader)
		{
			Initialize(reader, LexicalStates.INITIAL);
		}
		
		public void Initialize(System.IO.TextReader reader, LexicalStates lexicalState, bool atBol)
		{
			this.expanding_token = false;
			this.token_start = 0;
			this.chars_read = 0;
			this.lookahead_index = 0;
			this.token_chunk_start = 0;
			this.token_end = 0;
			this.token_end_pos = new Position(0);
			this.reader = reader;
			this.yy_at_bol = atBol;
			this.current_lexical_state = lexicalState;
		}
		
		public void Initialize(System.IO.TextReader reader, LexicalStates lexicalState)
		{
			Initialize(reader, lexicalState, false);
		}
		
		#region Accept
		
		#pragma warning disable 162
		
		
		Tokens Accept0(int state,out bool accepted)
		{
			accepted = true;
			
			switch(state)
			{
				case 2:
					// #line 92
					{ 
						return ProcessEof(Tokens.T_INLINE_HTML);
					}
					break;
					
				case 3:
					// #line 717
					{
						yymore(); break;
					}
					break;
					
				case 4:
					// #line 704
					{
						if(ProcessPreOpenTag())
						{
							return Tokens.T_INLINE_HTML; 
						}
						if (this._allowShortTags) {
							BEGIN(LexicalStates.ST_IN_SCRIPTING);
							return (Tokens.T_OPEN_TAG);
						} else {
							yymore(); break;
						}
					}
					break;
					
				case 5:
					// #line 684
					{
						if(ProcessPreOpenTag())
						{
							return Tokens.T_INLINE_HTML; 
						}
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG_WITH_ECHO);
					}
					break;
					
				case 6:
					// #line 694
					{
						if(ProcessPreOpenTag())
						{
							return Tokens.T_INLINE_HTML; 
						}
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG);
					}
					break;
					
				case 7:
					// #line 78
					{
						return Tokens.EOF;
					}
					break;
					
				case 8:
					// #line 592
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 9:
					// #line 738
					{
						return ProcessLabel();
					}
					break;
					
				case 10:
					// #line 281
					{
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 11:
					// #line 632
					{
						return ProcessDecimalNumber();
					}
					break;
					
				case 12:
					// #line 905
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 13:
					// #line 304
					{
						return (Tokens.T_NS_SEPARATOR);
					}
					break;
					
				case 14:
					// #line 742
					{
						yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); 
						yymore(); 
						break;
					}
					break;
					
				case 15:
					// #line 597
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return (Tokens.T_LBRACE);
					}
					break;
					
				case 16:
					// #line 605
					{
						ResetDocBlock();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return (Tokens.T_RBRACE);
					}
					break;
					
				case 17:
					// #line 881
					{ BEGIN(LexicalStates.ST_BACKQUOTE); return Tokens.T_BACKQUOTE; }
					break;
					
				case 18:
					// #line 871
					{ BEGIN(LexicalStates.ST_DOUBLE_QUOTES); yymore(); break; }
					break;
					
				case 19:
					// #line 820
					{ BEGIN(LexicalStates.ST_SINGLE_QUOTES); yymore(); break; }
					break;
					
				case 20:
					// #line 312
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 21:
					// #line 758
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens.T_CLOSE_TAG);  /* implicit ';' at php-end tag */
					}
					break;
					
				case 22:
					// #line 160
					{
						return ProcessToken(Tokens.T_IF);
					}
					break;
					
				case 23:
					// #line 216
					{
						return ProcessToken(Tokens.T_AS);
					}
					break;
					
				case 24:
					// #line 572
					{
						return (Tokens.T_LOGICAL_OR);
					}
					break;
					
				case 25:
					// #line 184
					{
						return ProcessToken(Tokens.T_DO);
					}
					break;
					
				case 26:
					// #line 480
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 27:
					// #line 276
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 28:
					// #line 516
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 29:
					// #line 588
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 30:
					// #line 508
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 31:
					// #line 648
					{
						return ProcessRealNumber();
					}
					break;
					
				case 32:
					// #line 300
					{
						return (Tokens.T_DOUBLE_COLON);
					}
					break;
					
				case 33:
					// #line 536
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 34:
					// #line 748
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 35:
					// #line 532
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 36:
					// #line 524
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 37:
					// #line 520
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 38:
					// #line 460
					{
						return Tokens.T_DOUBLE_ARROW;
					}
					break;
					
				case 39:
					// #line 492
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 40:
					// #line 512
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 41:
					// #line 476
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 42:
					// #line 496
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 43:
					// #line 504
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 44:
					// #line 584
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 45:
					// #line 540
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 46:
					// #line 552
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 47:
					// #line 568
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 48:
					// #line 556
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 49:
					// #line 564
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 50:
					// #line 560
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 51:
					// #line 763
					{
						return ProcessVariable();
					}
					break;
					
				case 52:
					// #line 580
					{
						return (Tokens.T_LOGICAL_XOR);
					}
					break;
					
				case 53:
					// #line 144
					{
						return ProcessToken(Tokens.T_TRY);
					}
					break;
					
				case 54:
					// #line 576
					{
						return (Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 55:
					// #line 380
					{
						return ProcessToken(Tokens.T_USE);
					}
					break;
					
				case 56:
					// #line 120
					{
						return ProcessToken(Tokens.T_EXIT);
					}
					break;
					
				case 57:
					// #line 188
					{
						return ProcessToken(Tokens.T_FOR);
					}
					break;
					
				case 58:
					// #line 316
					{
						return ProcessToken(Tokens.T_NEW);
					}
					break;
					
				case 59:
					// #line 548
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 60:
					// #line 308
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 61:
					// #line 324
					{
						return ProcessToken(Tokens.T_VAR);
					}
					break;
					
				case 62:
					// #line 528
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 63:
					// #line 484
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 64:
					// #line 488
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 65:
					// #line 500
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 66:
					// #line 544
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 67:
					// #line 636
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 68:
					// #line 628
					{
						return ProcessBinaryNumber();
					}
					break;
					
				case 69:
					// #line 113
					{ 
						return ProcessToken(Tokens.T_EXIT);
					}
					break;
					
				case 70:
					// #line 172
					{
						return ProcessToken(Tokens.T_ELSE);
					}
					break;
					
				case 71:
					// #line 248
					{
						return ProcessToken(Tokens.T_ECHO);
					}
					break;
					
				case 72:
					// #line 356
					{
						return ProcessToken(Tokens.T_EVAL);
					}
					break;
					
				case 73:
					// #line 464
					{
						return ProcessToken(Tokens.T_LIST);
					}
					break;
					
				case 74:
					// #line 228
					{
						return ProcessToken(Tokens.T_CASE);
					}
					break;
					
				case 75:
					// #line 244
					{
						return ProcessToken(Tokens.T_GOTO);
					}
					break;
					
				case 76:
					// #line 753
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 77:
					// #line 168
					{
						return ProcessToken(Tokens.T_ENDIF);
					}
					break;
					
				case 78:
					// #line 396
					{
						return ProcessToken(Tokens.T_EMPTY);
					}
					break;
					
				case 79:
					// #line 392
					{
						return ProcessToken(Tokens.T_ISSET);
					}
					break;
					
				case 80:
					// #line 264
					{
						return ProcessToken(Tokens.T_TRAIT);
					}
					break;
					
				case 81:
					// #line 156
					{
						return ProcessToken(Tokens.T_THROW);
					}
					break;
					
				case 82:
					// #line 468
					{
						return ProcessToken(Tokens.T_ARRAY);
					}
					break;
					
				case 83:
					// #line 456
					{
						return ProcessToken(Tokens.T_UNSET);
					}
					break;
					
				case 84:
					// #line 440
					{
						return ProcessToken(Tokens.T_FINAL);
					}
					break;
					
				case 85:
					// #line 148
					{
						return ProcessToken(Tokens.T_CATCH);
					}
					break;
					
				case 86:
					// #line 128
					{
						return ProcessToken(Tokens.T_CONST);
					}
					break;
					
				case 87:
					// #line 256
					{
						return ProcessToken(Tokens.T_CLASS);
					}
					break;
					
				case 88:
					// #line 320
					{
						return ProcessToken(Tokens.T_CLONE);
					}
					break;
					
				case 89:
					// #line 140
					{
						return ProcessToken(Tokens.T_YIELD);
					}
					break;
					
				case 90:
					// #line 176
					{
						return ProcessToken(Tokens.T_WHILE);
					}
					break;
					
				case 91:
					// #line 236
					{
						return ProcessToken(Tokens.T_BREAK);
					}
					break;
					
				case 92:
					// #line 252
					{
						return ProcessToken(Tokens.T_PRINT);
					}
					break;
					
				case 93:
					// #line 328
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 94:
					// #line 767
					{
						int bprefix = (GetTokenChar(0) != '<') ? 1 : 0;
						int s = bprefix + 3;
					    int length = TokenLength - bprefix - 3 - 1 - (GetTokenChar(TokenLength-2) == '\r' ? 1 : 0);
					    string tokenString = GetTokenString();
					    while ((tokenString[s] == ' ') || (tokenString[s] == '\t')) {
							s++;
					        length--;
					    }
						if (tokenString[s] == '\'') {
							s++;
					        length -= 2;
					        BEGIN(LexicalStates.ST_NOWDOC);
						} else {
							if (tokenString[s] == '"') {
								s++;
					            length -= 2;
					        }
							BEGIN(LexicalStates.ST_HEREDOC);
						}
					    this._hereDocLabel = GetTokenSubstring(s, length);
					    return (Tokens.T_START_HEREDOC);
					}
					break;
					
				case 95:
					// #line 164
					{
						return ProcessToken(Tokens.T_ELSEIF);
					}
					break;
					
				case 96:
					// #line 192
					{
						return ProcessToken(Tokens.T_ENDFOR);
					}
					break;
					
				case 97:
					// #line 432
					{
						return ProcessToken(Tokens.T_STATIC);
					}
					break;
					
				case 98:
					// #line 220
					{
						return ProcessToken(Tokens.T_SWITCH);
					}
					break;
					
				case 99:
					// #line 132
					{
						return ProcessToken(Tokens.T_RETURN);
					}
					break;
					
				case 100:
					// #line 388
					{
						return ProcessToken(Tokens.T_GLOBAL);
					}
					break;
					
				case 101:
					// #line 452
					{
						return ProcessToken(Tokens.T_PUBLIC);
					}
					break;
					
				case 102:
					// #line 332
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 103:
					// #line 348
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 104:
					// #line 268
					{
						return ProcessToken(Tokens.T_EXTENDS);
					}
					break;
					
				case 105:
					// #line 360
					{
						return ProcessToken(Tokens.T_INCLUDE);
					}
					break;
					
				case 106:
					// #line 232
					{
						return ProcessToken(Tokens.T_DEFAULT);
					}
					break;
					
				case 107:
					// #line 204
					{
						return ProcessToken(Tokens.T_DECLARE);
					}
					break;
					
				case 108:
					// #line 152
					{
						return ProcessToken(Tokens.T_FINALLY);
					}
					break;
					
				case 109:
					// #line 196
					{
						return ProcessToken(Tokens.T_FOREACH);
					}
					break;
					
				case 110:
					// #line 368
					{
						return ProcessToken(Tokens.T_REQUIRE);
					}
					break;
					
				case 111:
					// #line 444
					{
						return ProcessToken(Tokens.T_PRIVATE);
					}
					break;
					
				case 112:
					// #line 340
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 113:
					// #line 352
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 114:
					// #line 676
					{
						return ProcessToken(Tokens.T_DIR);
					}
					break;
					
				case 115:
					// #line 180
					{
						return ProcessToken(Tokens.T_ENDWHILE);
					}
					break;
					
				case 116:
					// #line 116
					{ 
						return ProcessToken(Tokens.T_AUTOLOAD);
					}
					break;
					
				case 117:
					// #line 436
					{
						return ProcessToken(Tokens.T_ABSTRACT);
					}
					break;
					
				case 118:
					// #line 124
					{
						return ProcessToken(Tokens.T_FUNCTION);
					}
					break;
					
				case 119:
					// #line 472
					{
						return ProcessToken(Tokens.T_CALLABLE);
					}
					break;
					
				case 120:
					// #line 240
					{
						return ProcessToken(Tokens.T_CONTINUE);
					}
					break;
					
				case 121:
					// #line 344
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 122:
					// #line 336
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 123:
					// #line 668
					{
						return ProcessToken(Tokens.T_LINE);
					}
					break;
					
				case 124:
					// #line 672
					{
						return ProcessToken(Tokens.T_FILE);
					}
					break;
					
				case 125:
					// #line 224
					{
						return ProcessToken(Tokens.T_ENDSWITCH);
					}
					break;
					
				case 126:
					// #line 260
					{
						return ProcessToken(Tokens.T_INTERFACE);
					}
					break;
					
				case 127:
					// #line 384
					{
						return ProcessToken(Tokens.T_INSTEADOF);
					}
					break;
					
				case 128:
					// #line 376
					{
						return ProcessToken(Tokens.T_NAMESPACE);
					}
					break;
					
				case 129:
					// #line 448
					{
						return ProcessToken(Tokens.T_PROTECTED);
					}
					break;
					
				case 130:
					// #line 656
					{
						return ProcessToken(Tokens.T_TRAIT_C);
					}
					break;
					
				case 131:
					// #line 652
					{
						return ProcessToken(Tokens.T_CLASS_C);
					}
					break;
					
				case 132:
					// #line 208
					{
						return ProcessToken(Tokens.T_ENDDECLARE);
					}
					break;
					
				case 133:
					// #line 200
					{
						return ProcessToken(Tokens.T_ENDFOREACH);
					}
					break;
					
				case 134:
					// #line 212
					{
						return ProcessToken(Tokens.T_INSTANCEOF);
					}
					break;
					
				case 135:
					// #line 272
					{
						return ProcessToken(Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 136:
					// #line 136
					{
						return Tokens.T_YIELD_FROM;
					}
					break;
					
				case 137:
					// #line 664
					{
						return ProcessToken(Tokens.T_METHOD_C);
					}
					break;
					
				case 138:
					// #line 364
					{
						return ProcessToken(Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 139:
					// #line 372
					{
						return ProcessToken(Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 140:
					// #line 660
					{
						return ProcessToken(Tokens.T_FUNC_C);
					}
					break;
					
				case 141:
					// #line 680
					{
						return ProcessToken(Tokens.T_NS_C);
					}
					break;
					
				case 142:
					// #line 415
					{
						// IMPORTANT - Added because PHP lexer explicitly checks halt compiler syntax and reverts to initial state after semicolon
						yy_push_state(LexicalStates.ST_HALT_COMPILER1); 
						return (Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 143:
					// #line 84
					{
						if(TokenLength > 0)
						{
							return ProcessStringEOF(); 
						}
						return Tokens.EOF;
					}
					break;
					
				case 144:
					// #line 879
					{ yymore(); break; }
					break;
					
				case 145:
					// #line 877
					{ yymore(); break; }
					break;
					
				case 146:
					// #line 875
					{ Tokens token; if (ProcessString(1, out token)) return token; else break; }
					break;
					
				case 147:
					// #line 878
					{ yymore(); break; }
					break;
					
				case 148:
					// #line 874
					{ Tokens token; if (ProcessString(2, out token)) return token; else break; }
					break;
					
				case 149:
					// #line 873
					{ Tokens token; if (ProcessString(2, out token)) return token; else break; }
					break;
					
				case 150:
					// #line 872
					{ Tokens token; if (ProcessString(2, out token)) return token; else break; }
					break;
					
				case 151:
					// #line 81
					{
						return ProcessEof(Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 152:
					// #line 824
					{ yymore(); break; }
					break;
					
				case 153:
					// #line 823
					{ yymore(); break; }
					break;
					
				case 154:
					// #line 822
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 155:
					// #line 821
					{ yymore(); break; }
					break;
					
				case 156:
					// #line 889
					{ yymore(); break; }
					break;
					
				case 157:
					// #line 887
					{ yymore(); break; }
					break;
					
				case 158:
					// #line 885
					{ Tokens token; if (ProcessShell(1, out token)) return token; else break; }
					break;
					
				case 159:
					// #line 888
					{ yymore(); break; }
					break;
					
				case 160:
					// #line 884
					{ Tokens token; if (ProcessShell(2, out token)) return token; else break; }
					break;
					
				case 161:
					// #line 883
					{ Tokens token; if (ProcessShell(2, out token)) return token; else break; }
					break;
					
				case 162:
					// #line 882
					{ Tokens token; if (ProcessShell(2, out token)) return token; else break; }
					break;
					
				case 163:
					// #line 898
					{ yymore(); break; }
					break;
					
				case 164:
					// #line 897
					{ yymore(); break; }
					break;
					
				case 165:
					// #line 895
					{ yymore(); break; }
					break;
					
				case 166:
					// #line 896
					{ yymore(); break; }
					break;
					
				case 167:
					// #line 893
					{ Tokens token; if (ProcessHeredoc(2, out token)) return token; else break; }
					break;
					
				case 168:
					// #line 892
					{ Tokens token; if (ProcessHeredoc(2, out token)) return token; else break; }
					break;
					
				case 169:
					// #line 891
					{ Tokens token; if (ProcessHeredoc(2, out token)) return token; else break; }
					break;
					
				case 170:
					// #line 809
					{
					    if(!string.IsNullOrEmpty(this._hereDocLabel) && GetTokenString().Contains(this._hereDocLabel))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(s => (string)ProcessEscapedString(s, _encoding, false)) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 171:
					// #line 294
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 172:
					// #line 289
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 173:
					// #line 285
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 174:
					// #line 621
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 175:
					// #line 613
					{
						_yyless(1);
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return ProcessToken(Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 176:
					// #line 104
					{
						if(TokenLength > 0)
						{
							SetDocBlock(); 
							return Tokens.T_DOC_COMMENT; 
						}
						return Tokens.EOF;
					}
					break;
					
				case 177:
					// #line 754
					{ yymore(); break; }
					break;
					
				case 178:
					// #line 756
					{ yymore(); break; }
					break;
					
				case 179:
					// #line 755
					{ yy_pop_state(); SetDocBlock(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 180:
					// #line 96
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 181:
					// #line 749
					{ yymore(); break; }
					break;
					
				case 182:
					// #line 751
					{ yymore(); break; }
					break;
					
				case 183:
					// #line 750
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 184:
					// #line 903
					{ yymore(); break; }
					break;
					
				case 185:
					// #line 100
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 186:
					// #line 902
					{ yymore(); break; }
					break;
					
				case 187:
					// #line 900
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 188:
					// #line 901
					{ _yyless(2); yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 189:
					// #line 726
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 190:
					// #line 640
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 191:
					// #line 721
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 192:
					// #line 644
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 193:
					// #line 794
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						this._tokenSemantics.Object = this._hereDocLabel;
						return (Tokens.T_END_HEREDOC);
					}
					break;
					
				case 194:
					// #line 818
					{ yymore(); break; }
					break;
					
				case 195:
					// #line 800
					{
					    if(!string.IsNullOrEmpty(this._hereDocLabel) && GetTokenString().Contains(this._hereDocLabel))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(s => s) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 196:
					// #line 427
					{
						yy_pop_state();
						yymore(); break;
					}
					break;
					
				case 197:
					// #line 421
					{ return (Tokens.T_WHITESPACE); }
					break;
					
				case 198:
					// #line 400
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER2);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 199:
					// #line 424
					{ yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); yymore(); break; }
					break;
					
				case 200:
					// #line 425
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 201:
					// #line 423
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 202:
					// #line 405
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER3);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 203:
					// #line 410
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 204:
					// #line 865
					{ 
						_yyless(1); 
						yy_pop_state(); 
						yymore(); break; 
					}
					break;
					
				case 205:
					// #line 854
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 206:
					// #line 848
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						_yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 207:
					// #line 843
					{
						yy_pop_state();
						return ProcessVariable();
					}
					break;
					
				case 208:
					// #line 826
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 209:
					// #line 837
					{
						_yyless(1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 210:
					// #line 831
					{
						_yyless(3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 211:
					// #line 859
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 214: goto case 3;
				case 215: goto case 6;
				case 216: goto case 8;
				case 217: goto case 9;
				case 218: goto case 11;
				case 219: goto case 21;
				case 220: goto case 31;
				case 221: goto case 42;
				case 222: goto case 94;
				case 223: goto case 145;
				case 224: goto case 153;
				case 225: goto case 157;
				case 226: goto case 164;
				case 227: goto case 165;
				case 228: goto case 170;
				case 229: goto case 171;
				case 230: goto case 174;
				case 231: goto case 184;
				case 232: goto case 187;
				case 233: goto case 189;
				case 234: goto case 190;
				case 235: goto case 192;
				case 236: goto case 195;
				case 237: goto case 196;
				case 238: goto case 204;
				case 240: goto case 8;
				case 241: goto case 9;
				case 242: goto case 21;
				case 243: goto case 192;
				case 244: goto case 204;
				case 246: goto case 8;
				case 247: goto case 9;
				case 249: goto case 8;
				case 250: goto case 9;
				case 252: goto case 8;
				case 253: goto case 9;
				case 255: goto case 8;
				case 256: goto case 9;
				case 258: goto case 8;
				case 259: goto case 9;
				case 261: goto case 8;
				case 262: goto case 9;
				case 264: goto case 8;
				case 265: goto case 9;
				case 267: goto case 8;
				case 268: goto case 9;
				case 270: goto case 8;
				case 271: goto case 9;
				case 273: goto case 8;
				case 274: goto case 9;
				case 276: goto case 8;
				case 277: goto case 9;
				case 279: goto case 8;
				case 280: goto case 9;
				case 282: goto case 8;
				case 283: goto case 9;
				case 285: goto case 8;
				case 286: goto case 9;
				case 288: goto case 8;
				case 289: goto case 9;
				case 291: goto case 9;
				case 293: goto case 9;
				case 295: goto case 9;
				case 297: goto case 9;
				case 299: goto case 9;
				case 301: goto case 9;
				case 303: goto case 9;
				case 305: goto case 9;
				case 307: goto case 9;
				case 309: goto case 9;
				case 311: goto case 9;
				case 313: goto case 9;
				case 315: goto case 9;
				case 317: goto case 9;
				case 319: goto case 9;
				case 321: goto case 9;
				case 323: goto case 9;
				case 325: goto case 9;
				case 327: goto case 9;
				case 329: goto case 9;
				case 331: goto case 9;
				case 333: goto case 9;
				case 335: goto case 9;
				case 337: goto case 9;
				case 339: goto case 9;
				case 341: goto case 9;
				case 343: goto case 9;
				case 345: goto case 9;
				case 347: goto case 9;
				case 349: goto case 9;
				case 351: goto case 9;
				case 353: goto case 9;
				case 355: goto case 9;
				case 357: goto case 9;
				case 359: goto case 9;
				case 361: goto case 9;
				case 363: goto case 9;
				case 365: goto case 9;
				case 367: goto case 9;
				case 369: goto case 9;
				case 371: goto case 9;
				case 373: goto case 9;
				case 375: goto case 9;
				case 377: goto case 9;
				case 379: goto case 9;
				case 381: goto case 9;
				case 383: goto case 9;
				case 385: goto case 9;
				case 387: goto case 9;
				case 389: goto case 9;
				case 391: goto case 9;
				case 393: goto case 9;
				case 395: goto case 9;
				case 397: goto case 9;
				case 399: goto case 9;
				case 401: goto case 9;
				case 403: goto case 9;
				case 405: goto case 9;
				case 407: goto case 9;
				case 409: goto case 9;
				case 427: goto case 9;
				case 438: goto case 9;
				case 442: goto case 9;
				case 444: goto case 9;
				case 446: goto case 9;
				case 448: goto case 9;
				case 449: goto case 9;
				case 450: goto case 9;
				case 451: goto case 9;
				case 452: goto case 9;
				case 453: goto case 9;
				case 454: goto case 9;
				case 455: goto case 9;
				case 456: goto case 9;
				case 457: goto case 9;
				case 458: goto case 9;
				case 459: goto case 9;
				case 460: goto case 9;
				case 461: goto case 9;
				case 462: goto case 9;
				case 463: goto case 9;
				case 464: goto case 9;
				case 465: goto case 9;
				case 466: goto case 9;
				case 467: goto case 9;
				case 468: goto case 9;
				case 469: goto case 9;
				case 470: goto case 9;
				case 471: goto case 9;
				case 472: goto case 9;
				case 473: goto case 9;
				case 474: goto case 9;
				case 475: goto case 9;
				case 476: goto case 9;
				case 477: goto case 9;
				case 478: goto case 9;
				case 479: goto case 9;
				case 480: goto case 9;
				case 481: goto case 9;
				case 482: goto case 9;
				case 483: goto case 9;
				case 484: goto case 9;
				case 485: goto case 9;
				case 486: goto case 9;
				case 487: goto case 9;
				case 488: goto case 9;
				case 489: goto case 9;
				case 490: goto case 9;
				case 491: goto case 9;
				case 492: goto case 9;
				case 493: goto case 9;
				case 494: goto case 9;
				case 495: goto case 9;
				case 496: goto case 9;
				case 497: goto case 9;
				case 498: goto case 9;
				case 499: goto case 9;
				case 500: goto case 9;
				case 501: goto case 9;
				case 502: goto case 9;
				case 503: goto case 9;
				case 504: goto case 9;
				case 505: goto case 9;
				case 506: goto case 9;
				case 507: goto case 9;
				case 508: goto case 9;
				case 509: goto case 9;
				case 510: goto case 9;
				case 511: goto case 9;
				case 512: goto case 9;
				case 513: goto case 9;
				case 514: goto case 9;
				case 515: goto case 9;
				case 516: goto case 9;
				case 517: goto case 9;
				case 518: goto case 9;
				case 519: goto case 9;
				case 520: goto case 9;
				case 521: goto case 9;
				case 522: goto case 9;
				case 523: goto case 9;
				case 524: goto case 9;
				case 525: goto case 9;
				case 526: goto case 9;
				case 527: goto case 9;
				case 528: goto case 9;
				case 529: goto case 9;
				case 530: goto case 9;
				case 531: goto case 9;
				case 532: goto case 9;
				case 533: goto case 9;
				case 534: goto case 9;
				case 535: goto case 9;
				case 536: goto case 9;
				case 537: goto case 9;
				case 538: goto case 9;
				case 539: goto case 9;
				case 540: goto case 9;
				case 541: goto case 9;
				case 542: goto case 9;
				case 543: goto case 9;
				case 544: goto case 9;
				case 545: goto case 9;
				case 546: goto case 9;
				case 547: goto case 9;
				case 548: goto case 9;
				case 549: goto case 9;
				case 550: goto case 9;
				case 551: goto case 9;
				case 552: goto case 9;
				case 553: goto case 9;
				case 554: goto case 9;
				case 555: goto case 9;
				case 556: goto case 9;
				case 557: goto case 9;
				case 558: goto case 9;
				case 559: goto case 9;
				case 560: goto case 9;
				case 561: goto case 9;
				case 562: goto case 9;
				case 563: goto case 9;
				case 564: goto case 9;
				case 565: goto case 9;
				case 566: goto case 9;
				case 567: goto case 9;
				case 568: goto case 9;
				case 569: goto case 9;
				case 570: goto case 9;
				case 571: goto case 9;
				case 572: goto case 9;
				case 573: goto case 9;
				case 574: goto case 9;
				case 575: goto case 9;
				case 576: goto case 9;
				case 577: goto case 9;
				case 578: goto case 9;
				case 579: goto case 9;
				case 580: goto case 9;
				case 581: goto case 9;
				case 582: goto case 9;
				case 583: goto case 9;
				case 584: goto case 9;
				case 585: goto case 9;
				case 586: goto case 9;
				case 587: goto case 9;
				case 588: goto case 9;
				case 589: goto case 9;
				case 590: goto case 9;
				case 591: goto case 9;
				case 592: goto case 9;
				case 593: goto case 9;
				case 594: goto case 9;
				case 595: goto case 9;
				case 596: goto case 9;
				case 597: goto case 9;
				case 598: goto case 9;
				case 599: goto case 9;
				case 600: goto case 9;
				case 601: goto case 9;
				case 602: goto case 9;
				case 603: goto case 9;
				case 604: goto case 9;
				case 605: goto case 9;
				case 606: goto case 9;
				case 607: goto case 9;
				case 608: goto case 9;
				case 609: goto case 9;
				case 610: goto case 9;
				case 611: goto case 9;
				case 612: goto case 9;
				case 613: goto case 9;
				case 614: goto case 9;
				case 615: goto case 9;
				case 616: goto case 9;
				case 617: goto case 9;
				case 618: goto case 9;
				case 619: goto case 9;
				case 620: goto case 9;
				case 621: goto case 9;
				case 622: goto case 9;
				case 623: goto case 9;
				case 624: goto case 9;
				case 625: goto case 9;
				case 626: goto case 9;
				case 627: goto case 9;
				case 628: goto case 9;
				case 629: goto case 9;
				case 630: goto case 9;
				case 631: goto case 9;
				case 632: goto case 9;
				case 633: goto case 9;
				case 634: goto case 9;
				case 635: goto case 9;
				case 636: goto case 9;
				case 637: goto case 9;
				case 638: goto case 9;
				case 639: goto case 9;
				case 640: goto case 9;
				case 641: goto case 9;
				case 642: goto case 9;
				case 643: goto case 9;
				case 644: goto case 9;
				case 645: goto case 9;
				case 646: goto case 9;
				case 647: goto case 9;
				case 648: goto case 9;
				case 649: goto case 9;
				case 650: goto case 9;
				case 651: goto case 9;
				case 652: goto case 9;
				case 653: goto case 9;
				case 654: goto case 9;
				case 655: goto case 9;
				case 656: goto case 9;
				case 657: goto case 9;
				case 658: goto case 9;
				case 659: goto case 9;
				case 660: goto case 9;
			}
			accepted = false;
			return yyreturn;
		}
		
		#pragma warning restore 162
		
		
		#endregion
		private void BEGIN(LexicalStates state)
		{
			current_lexical_state = state;
		}
		
		private char Advance()
		{
			if (lookahead_index >= chars_read)
			{
				if (token_start > 0)
				{
					// shift buffer left:
					int length = chars_read - token_start;
					System.Buffer.BlockCopy(buffer, token_start << 1, buffer, 0, length << 1);
					token_end -= token_start;
					token_chunk_start -= token_start;
					token_start = 0;
					chars_read = lookahead_index = length;
					
					// populate the remaining bytes:
					int count = reader.Read(buffer, chars_read, buffer.Length - chars_read);
					if (count <= 0) return EOF;
					
					chars_read += count;
				}
				
				while (lookahead_index >= chars_read)
				{
					if (lookahead_index >= buffer.Length)
						buffer = ResizeBuffer(buffer);
					
					int count = reader.Read(buffer, chars_read, buffer.Length - chars_read);
					if (count <= 0) return EOF;
					chars_read += count;
				}
			}
			
			return Map(buffer[lookahead_index++]);
		}
		
		private char[] ResizeBuffer(char[] buf)
		{
			char[] result = new char[buf.Length << 1];
			System.Buffer.BlockCopy(buf, 0, result, 0, buf.Length << 1);
			return result;
		}
		
		private void AdvanceEndPosition(int from, int to)
		{
			token_end_pos.Char += to - from;
		}
		
		protected static bool IsNewLineCharacter(char ch)
		{
		    return ch == '\r' || ch == '\n' || ch == (char)0x2028 || ch == (char)0x2029;
		}
		private void TrimTokenEnd()
		{
			if (token_end > token_chunk_start && buffer[token_end - 1] == '\n')
				token_end--;
			if (token_end > token_chunk_start && buffer[token_end - 1] == '\r')
				token_end--;
			}
		
		private void MarkTokenChunkStart()
		{
			token_chunk_start = lookahead_index;
		}
		
		private void MarkTokenEnd()
		{
			token_end = lookahead_index;
		}
		
		private void MoveToTokenEnd()
		{
			lookahead_index = token_end;
			yy_at_bol = (token_end > token_chunk_start) && (buffer[token_end - 1] == '\r' || buffer[token_end - 1] == '\n');
		}
		
		public int TokenLength
		{
			get { return token_end - token_start; }
		}
		
		public int TokenChunkLength
		{
			get { return token_end - token_chunk_start; }
		}
		
		private void yymore()
		{
			if (!expanding_token)
			{
				token_start = token_chunk_start;
				expanding_token = true;
			}
		}
		
		private void yyless(int count)
		{
			lookahead_index = token_end = token_chunk_start + count;
		}
		
		private Stack<LexicalStates> stateStack = new Stack<LexicalStates>(20);
		
		private void yy_push_state(LexicalStates state)
		{
			stateStack.Push(current_lexical_state);
			current_lexical_state = state;
		}
		
		private bool yy_pop_state()
		{
			if (stateStack.Count == 0) return false;
			current_lexical_state = stateStack.Pop();
			return true;
		}
		
		private LexicalStates yy_top_state()
		{
			return stateStack.Peek();
		}
		
		#region Tables
		
		private static AcceptConditions[] acceptCondition = new AcceptConditions[]
		{
			AcceptConditions.NotAccept, // 0
			AcceptConditions.Accept, // 1
			AcceptConditions.Accept, // 2
			AcceptConditions.Accept, // 3
			AcceptConditions.Accept, // 4
			AcceptConditions.Accept, // 5
			AcceptConditions.Accept, // 6
			AcceptConditions.Accept, // 7
			AcceptConditions.Accept, // 8
			AcceptConditions.Accept, // 9
			AcceptConditions.Accept, // 10
			AcceptConditions.Accept, // 11
			AcceptConditions.Accept, // 12
			AcceptConditions.Accept, // 13
			AcceptConditions.Accept, // 14
			AcceptConditions.Accept, // 15
			AcceptConditions.Accept, // 16
			AcceptConditions.Accept, // 17
			AcceptConditions.Accept, // 18
			AcceptConditions.Accept, // 19
			AcceptConditions.Accept, // 20
			AcceptConditions.Accept, // 21
			AcceptConditions.Accept, // 22
			AcceptConditions.Accept, // 23
			AcceptConditions.Accept, // 24
			AcceptConditions.Accept, // 25
			AcceptConditions.Accept, // 26
			AcceptConditions.Accept, // 27
			AcceptConditions.Accept, // 28
			AcceptConditions.Accept, // 29
			AcceptConditions.Accept, // 30
			AcceptConditions.Accept, // 31
			AcceptConditions.Accept, // 32
			AcceptConditions.Accept, // 33
			AcceptConditions.Accept, // 34
			AcceptConditions.Accept, // 35
			AcceptConditions.Accept, // 36
			AcceptConditions.Accept, // 37
			AcceptConditions.Accept, // 38
			AcceptConditions.Accept, // 39
			AcceptConditions.Accept, // 40
			AcceptConditions.Accept, // 41
			AcceptConditions.Accept, // 42
			AcceptConditions.Accept, // 43
			AcceptConditions.Accept, // 44
			AcceptConditions.Accept, // 45
			AcceptConditions.Accept, // 46
			AcceptConditions.Accept, // 47
			AcceptConditions.Accept, // 48
			AcceptConditions.Accept, // 49
			AcceptConditions.Accept, // 50
			AcceptConditions.Accept, // 51
			AcceptConditions.Accept, // 52
			AcceptConditions.Accept, // 53
			AcceptConditions.Accept, // 54
			AcceptConditions.Accept, // 55
			AcceptConditions.Accept, // 56
			AcceptConditions.Accept, // 57
			AcceptConditions.Accept, // 58
			AcceptConditions.Accept, // 59
			AcceptConditions.Accept, // 60
			AcceptConditions.Accept, // 61
			AcceptConditions.Accept, // 62
			AcceptConditions.Accept, // 63
			AcceptConditions.Accept, // 64
			AcceptConditions.Accept, // 65
			AcceptConditions.Accept, // 66
			AcceptConditions.Accept, // 67
			AcceptConditions.Accept, // 68
			AcceptConditions.Accept, // 69
			AcceptConditions.Accept, // 70
			AcceptConditions.Accept, // 71
			AcceptConditions.Accept, // 72
			AcceptConditions.Accept, // 73
			AcceptConditions.Accept, // 74
			AcceptConditions.Accept, // 75
			AcceptConditions.Accept, // 76
			AcceptConditions.Accept, // 77
			AcceptConditions.Accept, // 78
			AcceptConditions.Accept, // 79
			AcceptConditions.Accept, // 80
			AcceptConditions.Accept, // 81
			AcceptConditions.Accept, // 82
			AcceptConditions.Accept, // 83
			AcceptConditions.Accept, // 84
			AcceptConditions.Accept, // 85
			AcceptConditions.Accept, // 86
			AcceptConditions.Accept, // 87
			AcceptConditions.Accept, // 88
			AcceptConditions.Accept, // 89
			AcceptConditions.Accept, // 90
			AcceptConditions.Accept, // 91
			AcceptConditions.Accept, // 92
			AcceptConditions.Accept, // 93
			AcceptConditions.Accept, // 94
			AcceptConditions.Accept, // 95
			AcceptConditions.Accept, // 96
			AcceptConditions.Accept, // 97
			AcceptConditions.Accept, // 98
			AcceptConditions.Accept, // 99
			AcceptConditions.Accept, // 100
			AcceptConditions.Accept, // 101
			AcceptConditions.Accept, // 102
			AcceptConditions.Accept, // 103
			AcceptConditions.Accept, // 104
			AcceptConditions.Accept, // 105
			AcceptConditions.Accept, // 106
			AcceptConditions.Accept, // 107
			AcceptConditions.Accept, // 108
			AcceptConditions.Accept, // 109
			AcceptConditions.Accept, // 110
			AcceptConditions.Accept, // 111
			AcceptConditions.Accept, // 112
			AcceptConditions.Accept, // 113
			AcceptConditions.Accept, // 114
			AcceptConditions.Accept, // 115
			AcceptConditions.Accept, // 116
			AcceptConditions.Accept, // 117
			AcceptConditions.Accept, // 118
			AcceptConditions.Accept, // 119
			AcceptConditions.Accept, // 120
			AcceptConditions.Accept, // 121
			AcceptConditions.Accept, // 122
			AcceptConditions.Accept, // 123
			AcceptConditions.Accept, // 124
			AcceptConditions.Accept, // 125
			AcceptConditions.Accept, // 126
			AcceptConditions.Accept, // 127
			AcceptConditions.Accept, // 128
			AcceptConditions.Accept, // 129
			AcceptConditions.Accept, // 130
			AcceptConditions.Accept, // 131
			AcceptConditions.Accept, // 132
			AcceptConditions.Accept, // 133
			AcceptConditions.Accept, // 134
			AcceptConditions.Accept, // 135
			AcceptConditions.Accept, // 136
			AcceptConditions.Accept, // 137
			AcceptConditions.Accept, // 138
			AcceptConditions.Accept, // 139
			AcceptConditions.Accept, // 140
			AcceptConditions.Accept, // 141
			AcceptConditions.Accept, // 142
			AcceptConditions.Accept, // 143
			AcceptConditions.Accept, // 144
			AcceptConditions.Accept, // 145
			AcceptConditions.Accept, // 146
			AcceptConditions.Accept, // 147
			AcceptConditions.Accept, // 148
			AcceptConditions.Accept, // 149
			AcceptConditions.Accept, // 150
			AcceptConditions.Accept, // 151
			AcceptConditions.Accept, // 152
			AcceptConditions.Accept, // 153
			AcceptConditions.Accept, // 154
			AcceptConditions.Accept, // 155
			AcceptConditions.Accept, // 156
			AcceptConditions.Accept, // 157
			AcceptConditions.Accept, // 158
			AcceptConditions.Accept, // 159
			AcceptConditions.Accept, // 160
			AcceptConditions.Accept, // 161
			AcceptConditions.Accept, // 162
			AcceptConditions.Accept, // 163
			AcceptConditions.Accept, // 164
			AcceptConditions.Accept, // 165
			AcceptConditions.Accept, // 166
			AcceptConditions.Accept, // 167
			AcceptConditions.Accept, // 168
			AcceptConditions.Accept, // 169
			AcceptConditions.AcceptOnStart, // 170
			AcceptConditions.Accept, // 171
			AcceptConditions.Accept, // 172
			AcceptConditions.Accept, // 173
			AcceptConditions.Accept, // 174
			AcceptConditions.Accept, // 175
			AcceptConditions.Accept, // 176
			AcceptConditions.Accept, // 177
			AcceptConditions.Accept, // 178
			AcceptConditions.Accept, // 179
			AcceptConditions.Accept, // 180
			AcceptConditions.Accept, // 181
			AcceptConditions.Accept, // 182
			AcceptConditions.Accept, // 183
			AcceptConditions.Accept, // 184
			AcceptConditions.Accept, // 185
			AcceptConditions.Accept, // 186
			AcceptConditions.Accept, // 187
			AcceptConditions.Accept, // 188
			AcceptConditions.Accept, // 189
			AcceptConditions.Accept, // 190
			AcceptConditions.Accept, // 191
			AcceptConditions.Accept, // 192
			AcceptConditions.Accept, // 193
			AcceptConditions.Accept, // 194
			AcceptConditions.AcceptOnStart, // 195
			AcceptConditions.Accept, // 196
			AcceptConditions.Accept, // 197
			AcceptConditions.Accept, // 198
			AcceptConditions.Accept, // 199
			AcceptConditions.Accept, // 200
			AcceptConditions.Accept, // 201
			AcceptConditions.Accept, // 202
			AcceptConditions.Accept, // 203
			AcceptConditions.Accept, // 204
			AcceptConditions.Accept, // 205
			AcceptConditions.Accept, // 206
			AcceptConditions.Accept, // 207
			AcceptConditions.Accept, // 208
			AcceptConditions.Accept, // 209
			AcceptConditions.Accept, // 210
			AcceptConditions.Accept, // 211
			AcceptConditions.NotAccept, // 212
			AcceptConditions.Accept, // 213
			AcceptConditions.Accept, // 214
			AcceptConditions.Accept, // 215
			AcceptConditions.Accept, // 216
			AcceptConditions.Accept, // 217
			AcceptConditions.Accept, // 218
			AcceptConditions.Accept, // 219
			AcceptConditions.Accept, // 220
			AcceptConditions.Accept, // 221
			AcceptConditions.Accept, // 222
			AcceptConditions.Accept, // 223
			AcceptConditions.Accept, // 224
			AcceptConditions.Accept, // 225
			AcceptConditions.Accept, // 226
			AcceptConditions.Accept, // 227
			AcceptConditions.AcceptOnStart, // 228
			AcceptConditions.Accept, // 229
			AcceptConditions.Accept, // 230
			AcceptConditions.Accept, // 231
			AcceptConditions.Accept, // 232
			AcceptConditions.Accept, // 233
			AcceptConditions.Accept, // 234
			AcceptConditions.Accept, // 235
			AcceptConditions.AcceptOnStart, // 236
			AcceptConditions.Accept, // 237
			AcceptConditions.Accept, // 238
			AcceptConditions.NotAccept, // 239
			AcceptConditions.Accept, // 240
			AcceptConditions.Accept, // 241
			AcceptConditions.Accept, // 242
			AcceptConditions.Accept, // 243
			AcceptConditions.Accept, // 244
			AcceptConditions.NotAccept, // 245
			AcceptConditions.Accept, // 246
			AcceptConditions.Accept, // 247
			AcceptConditions.NotAccept, // 248
			AcceptConditions.Accept, // 249
			AcceptConditions.Accept, // 250
			AcceptConditions.NotAccept, // 251
			AcceptConditions.Accept, // 252
			AcceptConditions.Accept, // 253
			AcceptConditions.NotAccept, // 254
			AcceptConditions.Accept, // 255
			AcceptConditions.Accept, // 256
			AcceptConditions.NotAccept, // 257
			AcceptConditions.Accept, // 258
			AcceptConditions.Accept, // 259
			AcceptConditions.NotAccept, // 260
			AcceptConditions.Accept, // 261
			AcceptConditions.Accept, // 262
			AcceptConditions.NotAccept, // 263
			AcceptConditions.Accept, // 264
			AcceptConditions.Accept, // 265
			AcceptConditions.NotAccept, // 266
			AcceptConditions.Accept, // 267
			AcceptConditions.Accept, // 268
			AcceptConditions.NotAccept, // 269
			AcceptConditions.Accept, // 270
			AcceptConditions.Accept, // 271
			AcceptConditions.NotAccept, // 272
			AcceptConditions.Accept, // 273
			AcceptConditions.Accept, // 274
			AcceptConditions.NotAccept, // 275
			AcceptConditions.Accept, // 276
			AcceptConditions.Accept, // 277
			AcceptConditions.NotAccept, // 278
			AcceptConditions.Accept, // 279
			AcceptConditions.Accept, // 280
			AcceptConditions.NotAccept, // 281
			AcceptConditions.Accept, // 282
			AcceptConditions.Accept, // 283
			AcceptConditions.NotAccept, // 284
			AcceptConditions.Accept, // 285
			AcceptConditions.Accept, // 286
			AcceptConditions.NotAccept, // 287
			AcceptConditions.Accept, // 288
			AcceptConditions.Accept, // 289
			AcceptConditions.NotAccept, // 290
			AcceptConditions.Accept, // 291
			AcceptConditions.NotAccept, // 292
			AcceptConditions.Accept, // 293
			AcceptConditions.NotAccept, // 294
			AcceptConditions.Accept, // 295
			AcceptConditions.NotAccept, // 296
			AcceptConditions.Accept, // 297
			AcceptConditions.NotAccept, // 298
			AcceptConditions.Accept, // 299
			AcceptConditions.NotAccept, // 300
			AcceptConditions.Accept, // 301
			AcceptConditions.NotAccept, // 302
			AcceptConditions.Accept, // 303
			AcceptConditions.NotAccept, // 304
			AcceptConditions.Accept, // 305
			AcceptConditions.NotAccept, // 306
			AcceptConditions.Accept, // 307
			AcceptConditions.NotAccept, // 308
			AcceptConditions.Accept, // 309
			AcceptConditions.NotAccept, // 310
			AcceptConditions.Accept, // 311
			AcceptConditions.NotAccept, // 312
			AcceptConditions.Accept, // 313
			AcceptConditions.NotAccept, // 314
			AcceptConditions.Accept, // 315
			AcceptConditions.NotAccept, // 316
			AcceptConditions.Accept, // 317
			AcceptConditions.NotAccept, // 318
			AcceptConditions.Accept, // 319
			AcceptConditions.NotAccept, // 320
			AcceptConditions.Accept, // 321
			AcceptConditions.NotAccept, // 322
			AcceptConditions.Accept, // 323
			AcceptConditions.NotAccept, // 324
			AcceptConditions.Accept, // 325
			AcceptConditions.NotAccept, // 326
			AcceptConditions.Accept, // 327
			AcceptConditions.NotAccept, // 328
			AcceptConditions.Accept, // 329
			AcceptConditions.NotAccept, // 330
			AcceptConditions.Accept, // 331
			AcceptConditions.NotAccept, // 332
			AcceptConditions.Accept, // 333
			AcceptConditions.NotAccept, // 334
			AcceptConditions.Accept, // 335
			AcceptConditions.NotAccept, // 336
			AcceptConditions.Accept, // 337
			AcceptConditions.NotAccept, // 338
			AcceptConditions.Accept, // 339
			AcceptConditions.NotAccept, // 340
			AcceptConditions.Accept, // 341
			AcceptConditions.NotAccept, // 342
			AcceptConditions.Accept, // 343
			AcceptConditions.NotAccept, // 344
			AcceptConditions.Accept, // 345
			AcceptConditions.NotAccept, // 346
			AcceptConditions.Accept, // 347
			AcceptConditions.NotAccept, // 348
			AcceptConditions.Accept, // 349
			AcceptConditions.NotAccept, // 350
			AcceptConditions.Accept, // 351
			AcceptConditions.NotAccept, // 352
			AcceptConditions.Accept, // 353
			AcceptConditions.NotAccept, // 354
			AcceptConditions.Accept, // 355
			AcceptConditions.NotAccept, // 356
			AcceptConditions.Accept, // 357
			AcceptConditions.NotAccept, // 358
			AcceptConditions.Accept, // 359
			AcceptConditions.NotAccept, // 360
			AcceptConditions.Accept, // 361
			AcceptConditions.NotAccept, // 362
			AcceptConditions.Accept, // 363
			AcceptConditions.NotAccept, // 364
			AcceptConditions.Accept, // 365
			AcceptConditions.NotAccept, // 366
			AcceptConditions.Accept, // 367
			AcceptConditions.NotAccept, // 368
			AcceptConditions.Accept, // 369
			AcceptConditions.NotAccept, // 370
			AcceptConditions.Accept, // 371
			AcceptConditions.NotAccept, // 372
			AcceptConditions.Accept, // 373
			AcceptConditions.NotAccept, // 374
			AcceptConditions.Accept, // 375
			AcceptConditions.NotAccept, // 376
			AcceptConditions.Accept, // 377
			AcceptConditions.NotAccept, // 378
			AcceptConditions.Accept, // 379
			AcceptConditions.NotAccept, // 380
			AcceptConditions.Accept, // 381
			AcceptConditions.NotAccept, // 382
			AcceptConditions.Accept, // 383
			AcceptConditions.NotAccept, // 384
			AcceptConditions.Accept, // 385
			AcceptConditions.NotAccept, // 386
			AcceptConditions.Accept, // 387
			AcceptConditions.NotAccept, // 388
			AcceptConditions.Accept, // 389
			AcceptConditions.NotAccept, // 390
			AcceptConditions.Accept, // 391
			AcceptConditions.NotAccept, // 392
			AcceptConditions.Accept, // 393
			AcceptConditions.NotAccept, // 394
			AcceptConditions.Accept, // 395
			AcceptConditions.NotAccept, // 396
			AcceptConditions.Accept, // 397
			AcceptConditions.NotAccept, // 398
			AcceptConditions.Accept, // 399
			AcceptConditions.NotAccept, // 400
			AcceptConditions.Accept, // 401
			AcceptConditions.NotAccept, // 402
			AcceptConditions.Accept, // 403
			AcceptConditions.NotAccept, // 404
			AcceptConditions.Accept, // 405
			AcceptConditions.NotAccept, // 406
			AcceptConditions.Accept, // 407
			AcceptConditions.NotAccept, // 408
			AcceptConditions.Accept, // 409
			AcceptConditions.NotAccept, // 410
			AcceptConditions.NotAccept, // 411
			AcceptConditions.NotAccept, // 412
			AcceptConditions.NotAccept, // 413
			AcceptConditions.NotAccept, // 414
			AcceptConditions.NotAccept, // 415
			AcceptConditions.NotAccept, // 416
			AcceptConditions.NotAccept, // 417
			AcceptConditions.NotAccept, // 418
			AcceptConditions.NotAccept, // 419
			AcceptConditions.NotAccept, // 420
			AcceptConditions.NotAccept, // 421
			AcceptConditions.NotAccept, // 422
			AcceptConditions.NotAccept, // 423
			AcceptConditions.NotAccept, // 424
			AcceptConditions.NotAccept, // 425
			AcceptConditions.NotAccept, // 426
			AcceptConditions.Accept, // 427
			AcceptConditions.Accept, // 428
			AcceptConditions.NotAccept, // 429
			AcceptConditions.NotAccept, // 430
			AcceptConditions.NotAccept, // 431
			AcceptConditions.NotAccept, // 432
			AcceptConditions.NotAccept, // 433
			AcceptConditions.NotAccept, // 434
			AcceptConditions.NotAccept, // 435
			AcceptConditions.NotAccept, // 436
			AcceptConditions.NotAccept, // 437
			AcceptConditions.Accept, // 438
			AcceptConditions.NotAccept, // 439
			AcceptConditions.NotAccept, // 440
			AcceptConditions.NotAccept, // 441
			AcceptConditions.Accept, // 442
			AcceptConditions.NotAccept, // 443
			AcceptConditions.Accept, // 444
			AcceptConditions.NotAccept, // 445
			AcceptConditions.Accept, // 446
			AcceptConditions.NotAccept, // 447
			AcceptConditions.Accept, // 448
			AcceptConditions.Accept, // 449
			AcceptConditions.Accept, // 450
			AcceptConditions.Accept, // 451
			AcceptConditions.Accept, // 452
			AcceptConditions.Accept, // 453
			AcceptConditions.Accept, // 454
			AcceptConditions.Accept, // 455
			AcceptConditions.Accept, // 456
			AcceptConditions.Accept, // 457
			AcceptConditions.Accept, // 458
			AcceptConditions.Accept, // 459
			AcceptConditions.Accept, // 460
			AcceptConditions.Accept, // 461
			AcceptConditions.Accept, // 462
			AcceptConditions.Accept, // 463
			AcceptConditions.Accept, // 464
			AcceptConditions.Accept, // 465
			AcceptConditions.Accept, // 466
			AcceptConditions.Accept, // 467
			AcceptConditions.Accept, // 468
			AcceptConditions.Accept, // 469
			AcceptConditions.Accept, // 470
			AcceptConditions.Accept, // 471
			AcceptConditions.Accept, // 472
			AcceptConditions.Accept, // 473
			AcceptConditions.Accept, // 474
			AcceptConditions.Accept, // 475
			AcceptConditions.Accept, // 476
			AcceptConditions.Accept, // 477
			AcceptConditions.Accept, // 478
			AcceptConditions.Accept, // 479
			AcceptConditions.Accept, // 480
			AcceptConditions.Accept, // 481
			AcceptConditions.Accept, // 482
			AcceptConditions.Accept, // 483
			AcceptConditions.Accept, // 484
			AcceptConditions.Accept, // 485
			AcceptConditions.Accept, // 486
			AcceptConditions.Accept, // 487
			AcceptConditions.Accept, // 488
			AcceptConditions.Accept, // 489
			AcceptConditions.Accept, // 490
			AcceptConditions.Accept, // 491
			AcceptConditions.Accept, // 492
			AcceptConditions.Accept, // 493
			AcceptConditions.Accept, // 494
			AcceptConditions.Accept, // 495
			AcceptConditions.Accept, // 496
			AcceptConditions.Accept, // 497
			AcceptConditions.Accept, // 498
			AcceptConditions.Accept, // 499
			AcceptConditions.Accept, // 500
			AcceptConditions.Accept, // 501
			AcceptConditions.Accept, // 502
			AcceptConditions.Accept, // 503
			AcceptConditions.Accept, // 504
			AcceptConditions.Accept, // 505
			AcceptConditions.Accept, // 506
			AcceptConditions.Accept, // 507
			AcceptConditions.Accept, // 508
			AcceptConditions.Accept, // 509
			AcceptConditions.Accept, // 510
			AcceptConditions.Accept, // 511
			AcceptConditions.Accept, // 512
			AcceptConditions.Accept, // 513
			AcceptConditions.Accept, // 514
			AcceptConditions.Accept, // 515
			AcceptConditions.Accept, // 516
			AcceptConditions.Accept, // 517
			AcceptConditions.Accept, // 518
			AcceptConditions.Accept, // 519
			AcceptConditions.Accept, // 520
			AcceptConditions.Accept, // 521
			AcceptConditions.Accept, // 522
			AcceptConditions.Accept, // 523
			AcceptConditions.Accept, // 524
			AcceptConditions.Accept, // 525
			AcceptConditions.Accept, // 526
			AcceptConditions.Accept, // 527
			AcceptConditions.Accept, // 528
			AcceptConditions.Accept, // 529
			AcceptConditions.Accept, // 530
			AcceptConditions.Accept, // 531
			AcceptConditions.Accept, // 532
			AcceptConditions.Accept, // 533
			AcceptConditions.Accept, // 534
			AcceptConditions.Accept, // 535
			AcceptConditions.Accept, // 536
			AcceptConditions.Accept, // 537
			AcceptConditions.Accept, // 538
			AcceptConditions.Accept, // 539
			AcceptConditions.Accept, // 540
			AcceptConditions.Accept, // 541
			AcceptConditions.Accept, // 542
			AcceptConditions.Accept, // 543
			AcceptConditions.Accept, // 544
			AcceptConditions.Accept, // 545
			AcceptConditions.Accept, // 546
			AcceptConditions.Accept, // 547
			AcceptConditions.Accept, // 548
			AcceptConditions.Accept, // 549
			AcceptConditions.Accept, // 550
			AcceptConditions.Accept, // 551
			AcceptConditions.Accept, // 552
			AcceptConditions.Accept, // 553
			AcceptConditions.Accept, // 554
			AcceptConditions.Accept, // 555
			AcceptConditions.Accept, // 556
			AcceptConditions.Accept, // 557
			AcceptConditions.Accept, // 558
			AcceptConditions.Accept, // 559
			AcceptConditions.Accept, // 560
			AcceptConditions.Accept, // 561
			AcceptConditions.Accept, // 562
			AcceptConditions.Accept, // 563
			AcceptConditions.Accept, // 564
			AcceptConditions.Accept, // 565
			AcceptConditions.Accept, // 566
			AcceptConditions.Accept, // 567
			AcceptConditions.Accept, // 568
			AcceptConditions.Accept, // 569
			AcceptConditions.Accept, // 570
			AcceptConditions.Accept, // 571
			AcceptConditions.Accept, // 572
			AcceptConditions.Accept, // 573
			AcceptConditions.Accept, // 574
			AcceptConditions.Accept, // 575
			AcceptConditions.Accept, // 576
			AcceptConditions.Accept, // 577
			AcceptConditions.Accept, // 578
			AcceptConditions.Accept, // 579
			AcceptConditions.Accept, // 580
			AcceptConditions.Accept, // 581
			AcceptConditions.Accept, // 582
			AcceptConditions.Accept, // 583
			AcceptConditions.Accept, // 584
			AcceptConditions.Accept, // 585
			AcceptConditions.Accept, // 586
			AcceptConditions.Accept, // 587
			AcceptConditions.Accept, // 588
			AcceptConditions.Accept, // 589
			AcceptConditions.Accept, // 590
			AcceptConditions.Accept, // 591
			AcceptConditions.Accept, // 592
			AcceptConditions.Accept, // 593
			AcceptConditions.Accept, // 594
			AcceptConditions.Accept, // 595
			AcceptConditions.Accept, // 596
			AcceptConditions.Accept, // 597
			AcceptConditions.Accept, // 598
			AcceptConditions.Accept, // 599
			AcceptConditions.Accept, // 600
			AcceptConditions.Accept, // 601
			AcceptConditions.Accept, // 602
			AcceptConditions.Accept, // 603
			AcceptConditions.Accept, // 604
			AcceptConditions.Accept, // 605
			AcceptConditions.Accept, // 606
			AcceptConditions.Accept, // 607
			AcceptConditions.Accept, // 608
			AcceptConditions.Accept, // 609
			AcceptConditions.Accept, // 610
			AcceptConditions.Accept, // 611
			AcceptConditions.Accept, // 612
			AcceptConditions.Accept, // 613
			AcceptConditions.Accept, // 614
			AcceptConditions.Accept, // 615
			AcceptConditions.Accept, // 616
			AcceptConditions.Accept, // 617
			AcceptConditions.Accept, // 618
			AcceptConditions.Accept, // 619
			AcceptConditions.Accept, // 620
			AcceptConditions.Accept, // 621
			AcceptConditions.Accept, // 622
			AcceptConditions.Accept, // 623
			AcceptConditions.Accept, // 624
			AcceptConditions.Accept, // 625
			AcceptConditions.Accept, // 626
			AcceptConditions.Accept, // 627
			AcceptConditions.Accept, // 628
			AcceptConditions.Accept, // 629
			AcceptConditions.Accept, // 630
			AcceptConditions.Accept, // 631
			AcceptConditions.Accept, // 632
			AcceptConditions.Accept, // 633
			AcceptConditions.Accept, // 634
			AcceptConditions.Accept, // 635
			AcceptConditions.Accept, // 636
			AcceptConditions.Accept, // 637
			AcceptConditions.Accept, // 638
			AcceptConditions.Accept, // 639
			AcceptConditions.Accept, // 640
			AcceptConditions.Accept, // 641
			AcceptConditions.Accept, // 642
			AcceptConditions.Accept, // 643
			AcceptConditions.Accept, // 644
			AcceptConditions.Accept, // 645
			AcceptConditions.Accept, // 646
			AcceptConditions.Accept, // 647
			AcceptConditions.Accept, // 648
			AcceptConditions.Accept, // 649
			AcceptConditions.Accept, // 650
			AcceptConditions.Accept, // 651
			AcceptConditions.Accept, // 652
			AcceptConditions.Accept, // 653
			AcceptConditions.Accept, // 654
			AcceptConditions.Accept, // 655
			AcceptConditions.Accept, // 656
			AcceptConditions.Accept, // 657
			AcceptConditions.Accept, // 658
			AcceptConditions.Accept, // 659
			AcceptConditions.Accept, // 660
		};
		
		private static int[] colMap = new int[]
		{
			30, 30, 30, 30, 30, 30, 30, 30, 30, 36, 18, 30, 30, 59, 30, 30, 
			30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 
			36, 47, 62, 44, 63, 49, 50, 64, 35, 37, 43, 46, 53, 26, 33, 42, 
			57, 58, 29, 29, 29, 29, 29, 29, 29, 29, 31, 41, 48, 45, 27, 2, 
			53, 7, 22, 14, 11, 3, 12, 24, 20, 5, 38, 23, 10, 19, 13, 9, 
			25, 40, 16, 15, 6, 8, 34, 21, 4, 17, 28, 56, 32, 60, 52, 39, 
			61, 7, 22, 14, 11, 3, 12, 24, 20, 5, 38, 23, 10, 19, 13, 9, 
			25, 40, 16, 15, 6, 8, 34, 21, 4, 17, 28, 54, 51, 55, 53, 30, 
			0, 1
		};
		
		private static int[] rowMap = new int[]
		{
			0, 1, 1, 1, 2, 1, 1, 1, 3, 4, 5, 6, 1, 1, 1, 1, 
			1, 1, 1, 1, 1, 7, 8, 8, 8, 8, 1, 1, 1, 9, 1, 10, 
			1, 1, 11, 1, 12, 1, 1, 13, 1, 1, 14, 15, 16, 1, 1, 1, 
			1, 1, 1, 17, 8, 8, 8, 8, 8, 18, 8, 1, 1, 8, 1, 1, 
			1, 1, 1, 19, 20, 8, 21, 8, 8, 8, 8, 8, 22, 8, 8, 8, 
			8, 8, 8, 8, 23, 8, 8, 8, 8, 24, 8, 8, 8, 1, 1, 8, 
			25, 8, 8, 8, 8, 8, 1, 1, 8, 26, 8, 8, 8, 8, 27, 8, 
			1, 1, 8, 8, 8, 8, 8, 8, 8, 1, 1, 8, 8, 8, 8, 8, 
			8, 8, 8, 8, 8, 8, 8, 8, 1, 8, 8, 8, 8, 8, 8, 1, 
			28, 29, 1, 1, 1, 1, 1, 1, 30, 30, 1, 1, 31, 32, 1, 1, 
			1, 1, 1, 33, 1, 34, 1, 1, 1, 1, 1, 1, 35, 1, 1, 1, 
			1, 36, 37, 1, 1, 38, 39, 1, 40, 1, 41, 1, 1, 1, 42, 1, 
			43, 1, 1, 1, 1, 44, 1, 1, 45, 46, 1, 1, 1, 1, 1, 47, 
			1, 1, 1, 1, 48, 49, 50, 51, 52, 53, 54, 1, 55, 1, 56, 57, 
			58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 
			74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 1, 
			89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 
			105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 86, 117, 118, 19, 
			67, 119, 20, 120, 55, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 
			132, 133, 134, 135, 22, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 
			147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 
			163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 
			179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 
			195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 
			211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 
			227, 228, 229, 230, 231, 232, 64, 233, 234, 235, 236, 237, 69, 77, 238, 239, 
			240, 241, 242, 46, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 
			255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 
			271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 
			287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 
			303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 
			319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 
			335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 
			351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 
			367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 
			383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 
			399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 
			415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 
			431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 
			447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 
			463, 464, 465, 466, 467, 468, 469, 470, 471, 8, 472, 473, 474, 475, 476, 477, 
			478, 479, 480, 481, 482
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 214, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 212, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 20, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 21, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 449, 649, 649, 649, 649, 649, 450, 649, 649, 451, 452, 649, 649, 649, -1, 453, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 454, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 254, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 219, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 242, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 59, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 254, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, 31, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 308, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 63, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 65, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, 310, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, -1, 51, 51, 51, 51, 51, 51, 51, -1, -1, 51, 51, -1, -1, -1, -1, 51, -1, -1, -1, 51, 51, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 51, 51, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 637, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 67, -1, -1, -1, 67, -1, -1, -1, 67, 67, -1, 67, -1, -1, -1, -1, -1, -1, -1, 67, -1, -1, -1, -1, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, 67, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 68, 68, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 329, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 351, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 348, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, 348, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, 348, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 563, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 657, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, -1, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, -1, 144, 144, 144, 144, 144, 144, 144, -1, -1, 144 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 148, -1 },
			{ -1, -1, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, -1, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, -1 },
			{ -1, -1, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, -1, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, -1, 156, 156, 156, 156, 156, 156, -1, 156, -1, 156 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 160, -1 },
			{ -1, -1, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, -1, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, -1, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, -1, 163, 163, 163, 163, -1, 163, 163, 163, -1, 163 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 167, -1 },
			{ -1, -1, -1, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, -1, 172, 172, 172, 172, 172, 172, 172, -1, -1, 172, 172, -1, -1, -1, -1, 172, -1, -1, -1, 172, 172, 172, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, 172, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, -1, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 179, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, -1, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 183, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 185, 186, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 187, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 232, 231, 231, 231, 231, 231 },
			{ -1, 185, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 188, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 190, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 190, 190, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 192, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 192, 192, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 419, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, -1, 207, 207, 207, 207, 207, 207, 207, 423, -1, 207, 207, -1, -1, -1, -1, 207, -1, -1, -1, 207, 207, 207, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 209, 207, 207, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 239, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, -1, 396, 396, 396, 396, 396, 396, 396, -1, -1, 396, -1, -1, -1, -1, -1, 396, -1, -1, -1, 396, 396, 396, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 22, 455, 649, 456, 649, 649, -1, 581, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 254, 287, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 290, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 220, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 220, 220, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 94, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, -1, 149, 149, 149, 149, 149, 149, 149, -1, -1, 149, -1, -1, -1, -1, -1, 149, -1, -1, -1, 149, 149, 149, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 150, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 153, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, -1, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, -1 },
			{ -1, -1, -1, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, -1, 161, 161, 161, 161, 161, 161, 161, -1, -1, 161, -1, -1, -1, -1, -1, 161, -1, -1, -1, 161, 161, 161, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 162, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 164, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, -1, 168, 168, 168, 168, 168, 168, 168, -1, -1, 168, -1, -1, -1, -1, -1, 168, -1, -1, -1, 168, 168, 168, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 169, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 170, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 173, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 406, 406, 406, 406, 406, 406, 406, 406, 406, 406, 406, 406, 406, 406, 406, -1, 406, 406, 406, 406, 406, 406, 406, -1, -1, 406, 406, -1, -1, -1, -1, 406, -1, -1, -1, 406, 406, 406, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 175, 175, 406, 406, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, -1, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, -1, 231, 231, 231, 231, 231 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 187, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, -1, 51, 51, 51, 51, 51, 51, 51, -1, -1, 51, -1, -1, -1, -1, -1, 51, -1, -1, -1, 51, 51, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 412, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 413, -1, -1, -1, -1, -1, -1, 192, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 192, 192, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 235, -1, -1, -1, 235, -1, -1, -1, 235, 235, -1, 235, -1, -1, -1, -1, -1, -1, -1, 235, -1, -1, -1, -1, -1, -1, 235, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 235, 235, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 195, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 199, 200, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 206, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 245, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 458, 649, 649, 649, 649, 262, 649, 23, 583, 649, -1, 649, 649, 649, 582, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 219, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 243, 243, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, -1, 207, 207, 207, 207, 207, 207, 207, -1, -1, 207, -1, -1, -1, -1, -1, 207, -1, -1, -1, 207, 207, 207, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 208, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 215, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 24, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 8, 9, 427, 217, 438, 241, 442, 247, 580, 250, 444, 446, 611, 631, 642, 647, 10, 649, 649, 651, 253, 649, 652, 653, 216, 240, 649, 11, 12, 246, 13, 249, 448, 252, 10, 255, 649, 654, 649, 255, 258, 261, 14, 264, 267, 270, 273, 276, 279, 282, 285, 255, 15, 16, 255, 218, 11, 10, 255, 17, 18, 288, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, 257, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, 31, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 460, 649, 268, 649, 649, 649, 25, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 429, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 260, -1, 263, 430, 266, -1, 269, 272, -1, -1, 275, 278, -1, -1, -1, -1, -1, 281, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 284, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 588, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, 251, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, 18, -1, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 292, -1, -1, 220, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 292, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 220, 220, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 52, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 60, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 14, 34, -1, 35, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 478, 649, 649, 649, 649, 649, 649, 649, 649, 649, 53, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 294, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 36, -1, 37, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 54, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 431, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 38, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 39, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 55, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 298, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 40, 41, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 56, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 300, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 42, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 57, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 302, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 221, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 43, -1, -1, 44, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 58, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 439, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 61, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 304, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 46, -1, -1, -1, -1, 47, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 69, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 306, -1, -1, -1, 432, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 70, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 50, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 71, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 72, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 73, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 74, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 312, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 75, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 314, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 77, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 434, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 78, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 316, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 79, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 440, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 80, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, 320, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 81, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 322, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 82, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 83, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, -1, 324, 324, 324, 324, 324, 324, 324, -1, -1, 324, -1, -1, -1, -1, -1, 324, -1, 310, -1, 324, 324, 324, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 326, -1, 436 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 84, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 328, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 330, 93, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 85, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 334, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 86, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 441, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 87, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 338, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 88, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 340, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 89, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, 443, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 90, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 94, 324, 324, 324, 324, 324, 324, 324, -1, -1, 324, 324, -1, -1, -1, -1, 324, -1, -1, -1, 324, 324, 324, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 324, 324, 222, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 91, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, -1, 344, 344, 344, 344, 344, 344, 344, -1, -1, 344, -1, -1, -1, -1, -1, 344, -1, -1, -1, 344, 344, 344, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 92, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 350, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 95, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 330, 93, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 96, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 352, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 97, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 354, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 98, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 356, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 99, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 360, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 100, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 340, 102, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 101, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 364, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 366, 103, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 104, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, -1, 344, 344, 344, 344, 344, 344, 344, -1, -1, 344, 344, -1, -1, -1, -1, 344, -1, -1, -1, 344, 344, 344, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 344, 344, -1, -1, -1, 368, -1, -1 },
			{ -1, -1, -1, 105, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, -1, 346, 346, 346, 346, 346, 346, 346, -1, -1, 346, 346, -1, -1, -1, -1, 346, -1, -1, -1, 346, 346, 346, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 346, 346, -1, -1, -1, -1, -1, 368 },
			{ -1, -1, -1, 649, 649, 649, 106, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 445, -1, -1, -1, -1, -1, 348, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 348, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 348, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 107, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 447, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 108, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 352, 112, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 109, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 354, 113, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 110, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 370, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 111, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 340, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 114, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 372, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 115, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 372, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 116, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, 374, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 117, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 366, 103, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 118, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 94, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 119, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 370, 121, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 120, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 372, 122, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 123, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 366, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 124, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 378, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 125, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 136, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 126, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ 1, 143, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 382, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 145, 144, 144, 144, 144, 144, 144, 144, 146, 223, 144 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 127, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147 },
			{ -1, -1, -1, 128, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ 1, 151, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 153, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 386, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 224, 152, 152, 152, 152, 154 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 129, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 130, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ 1, 151, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 390, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 157, 156, 156, 156, 156, 156, 156, 158, 156, 225, 156 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 131, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159 },
			{ -1, -1, -1, 132, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ 213, 151, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 164, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 394, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 165, 163, 163, 163, 163, 226, 163, 163, 163, 227, 163 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 133, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 134, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 170, 396, 396, 396, 396, 396, 396, 396, -1, -1, 396, 396, -1, -1, -1, -1, 396, -1, -1, -1, 396, 396, 396, 398, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 396, 396, 228, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 135, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 170, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 228, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 137, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 138, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 171, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 10, 172, 172, 172, 172, 172, 172, 172, 229, 171, 172, 171, 171, 171, 171, 171, 172, 171, 10, 171, 172, 172, 172, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 10, 171, 171, 171, 171, 171 },
			{ -1, -1, -1, 139, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 174, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 174, 230, 230, 230, 230, 230, 230, 230, 174, 174, 230, 174, 174, 174, 174, 174, 230, 174, 174, 174, 230, 230, 230, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 140, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 141, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ 1, 176, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 178, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 142, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ 1, 180, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 182, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181 },
			{ 1, 7, 189, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 12, 649, 649, 649, 649, 649, 649, 649, 189, 189, 649, 190, 12, 189, 12, 189, 649, 189, 12, 189, 649, 649, 649, 189, 189, 189, 12, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 234, 190, 12, 191, 189, 189, 233, 12 },
			{ 1, 7, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193 },
			{ 428, 151, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194 },
			{ -1, -1, -1, 416, 416, 416, 416, 416, 416, 416, 416, 416, 416, 416, 416, 416, 416, 416, 195, 416, 416, 416, 416, 416, 416, 416, -1, -1, 416, 416, -1, -1, -1, -1, 416, -1, -1, -1, 416, 416, 416, 417, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 416, 416, 236, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 195, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 236, -1, -1, -1, -1, -1 },
			{ 1, 7, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 197, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 198, 197, 196, 196, 196, 196, 196, 237, 196, 199, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 197, 196, 196, 196, 196, 196 },
			{ 1, 7, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 197, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 197, 202, 196, 196, 196, 196, 237, 196, 199, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 197, 196, 196, 196, 196, 196 },
			{ 1, 7, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 197, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 197, 196, 196, 196, 196, 203, 237, 196, 199, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 197, 196, 196, 196, 196, 196 },
			{ 1, 7, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 238, 204, 204, 204, 204, 204, 204, 204, 205, 244, 204 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 424, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 210, 210, -1, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, -1, 210, 210, 210, 210, 210, 210, 210, -1, -1, 210, -1, -1, -1, -1, -1, 210, -1, -1, -1, 210, 210, 210, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 1, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 238, 204, 204, 204, 204, 204, 204, 211, 204, 244, 204 },
			{ 1, 1, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 238, 204, 204, 204, 204, 204, 204, 204, 204, 244, 204 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 256, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 416, 416, 416, 416, 416, 416, 416, 416, 416, 416, 416, 416, 416, 416, 416, -1, 416, 416, 416, 416, 416, 416, 416, -1, -1, 416, -1, -1, -1, -1, -1, 416, -1, -1, -1, 416, 416, 416, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 310, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 296, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 433, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 435, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, 332, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 336, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 342, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, -1, 346, 346, 346, 346, 346, 346, 346, -1, -1, 346, -1, -1, -1, -1, -1, 346, -1, -1, -1, 346, 346, 346, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 340, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 259, 649, -1, 649, 457, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 318, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, 437, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 358, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 612, 649, 265, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 362, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 461, 649, 649, 585, 271, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 376, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 274, 649, 649, 649, 462, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 330, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 277, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 280, 614, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 283, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 474, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 286, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 475, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 289, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 632, 649, 649, 649, 649, 649, 649, 649, 476, 584, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 477, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 479, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 589, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 291, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 586, 649, 615, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 483, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 643, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 485, 649, 649, 649, 659, 649, 649, 649, 649, 293, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 487, 649, 488, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 592, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 616, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 489, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 593, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 490, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 295, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 590, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 633, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 493, 649, 649, 649, 634, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 494, 649, 649, 649, 495, 595, 496, 497, 644, 649, 649, 649, -1, 498, 596, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 297, 649, 649, 649, 649, 649, 597, 500, 649, 649, 501, 649, 649, -1, 649, 649, 502, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 299, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 617, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 301, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 303, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 305, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 307, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 618, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 309, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 311, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 505, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 313, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 619, 649, 649, 649, 649, 649, 649, 649, 649, 315, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 317, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 319, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 509, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 321, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 323, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 325, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 327, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 648, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 650, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 598, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 599, 649, 649, 622, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 513, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 600, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 515, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 331, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 517, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 603, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 601, 649, 649, 649, 520, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 521, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 625, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 525, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 333, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 335, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 337, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 339, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 341, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 530, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 626, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 531, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 343, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 534, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 607, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 536, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 345, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 538, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 539, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 347, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 349, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 353, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 608, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 542, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 355, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 357, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 544, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 359, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 627, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 546, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 547, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 629, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 361, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 549, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 550, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 551, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 363, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 365, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 367, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 369, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 371, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 373, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 375, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 559, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 560, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 377, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 379, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 381, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 564, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 565, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 383, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 385, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 387, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 660, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 566, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 389, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 567, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 609, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 391, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 393, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 568, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 395, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 397, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 569, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 399, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 571, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 574, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 575, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 401, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 403, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 405, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 576, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 577, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 407, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 578, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 579, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 409, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 459, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 591, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 481, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 480, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 503, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 484, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 635, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 491, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 492, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 504, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 510, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 621, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 507, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 636, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 518, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 512, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 623, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 516, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 529, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 605, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 532, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 537, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 628, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 535, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 543, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 545, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 557, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 548, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 553, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 570, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 572, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 463, 649, 613, 464, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 482, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 486, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 499, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 620, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 508, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 519, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 624, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 604, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 523, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 655, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 658, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 533, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 540, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 541, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 606, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 558, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 554, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 561, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 573, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 465, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 466, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 594, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 511, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 638, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 522, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 527, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 524, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 602, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 552, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 555, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 562, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 467, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 506, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 514, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 526, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 556, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 468, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 528, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 656, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 587, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 469, 470, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 471, 649, 649, 649, 649, 649, 649, 649, 472, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 473, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 639, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 640, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 610, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 649, 646, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 649, 645, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 649, 649, 649, 649, 649, 649, 630, 649, 649, 649, 649, 649, 649, 649, 649, -1, 649, 649, 649, 649, 649, 649, 649, -1, -1, 649, 649, -1, -1, -1, -1, 649, -1, -1, -1, 649, 649, 649, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 649, 649, -1, -1, -1, -1, -1, -1 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  248,
			  380,
			  384,
			  388,
			  392,
			  400,
			  402,
			  404,
			  408,
			  410,
			  184,
			  411,
			  414,
			  415,
			  418,
			  420,
			  421,
			  422,
			  425,
			  426
		};
		
		#endregion
		
		private Tokens NextToken()
		{
			int current_state = yy_state_dtrans[(int)current_lexical_state];
			int last_accept_state = NoState;
			bool is_initial_state = true;
			
			MarkTokenChunkStart();
			token_start = token_chunk_start;
			expanding_token = false;
			AdvanceEndPosition((token_end > 0) ? token_end - 1 : 0, token_start);
			
			// capture token start position:
			token_start_pos.Char = token_end_pos.Char;
			
			if (acceptCondition[current_state] != AcceptConditions.NotAccept)
			{
				last_accept_state = current_state;
				MarkTokenEnd();
			}
			
			while (true)
			{
				char lookahead = (is_initial_state && yy_at_bol) ? BOL : Advance();
				int next_state = nextState[rowMap[current_state], colMap[lookahead]];
				
				if (next_state != -1)
				{
					current_state = next_state;
					is_initial_state = false;
					
					if (acceptCondition[current_state] != AcceptConditions.NotAccept)
					{
						last_accept_state = current_state;
						MarkTokenEnd();
					}
				}
				else
				{
					if (last_accept_state == NoState)
					{
						return Tokens.T_ERROR;
					}
					else
					{
						if ((acceptCondition[last_accept_state] & AcceptConditions.AcceptOnEnd) != 0)
							TrimTokenEnd();
						MoveToTokenEnd();
						
						if (last_accept_state < 0)
						{
							System.Diagnostics.Debug.Assert(last_accept_state >= 661);
						}
						else
						{
							bool accepted = false;
							yyreturn = Accept0(last_accept_state, out accepted);
							if (accepted)
							{
								AdvanceEndPosition(token_start, token_end - 1);
								return yyreturn;
							}
						}
						
						// token ignored:
						is_initial_state = true;
						current_state = yy_state_dtrans[(int)current_lexical_state];
						last_accept_state = NoState;
						MarkTokenChunkStart();
						if (acceptCondition[current_state] != AcceptConditions.NotAccept)
						{
							last_accept_state = current_state;
							MarkTokenEnd();
						}
					}
				}
			}
		} // end of NextToken
	}
}

