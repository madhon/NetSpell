// Copyright (C) 2003  Paul Welter
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Drawing.Design;

namespace NetSpell.SpellChecker
{
	/// <summary>
	///		The Spelling class encapsulates the functions necessary to check
	///		the spelling of inputted text.
	/// </summary>
	[ToolboxBitmap(typeof(NetSpell.SpellChecker.Spelling), "Spelling.bmp")]
	public class Spelling : System.ComponentModel.Component
	{

		private string _CurrentWord = "";
		private DictionaryCollection _Dictionaries = new DictionaryCollection();
		
		// Regex are class scope and compiled to improve performance on reuse
		private Regex _digitRegex = new Regex("^\\d", RegexOptions.Compiled);
		private Regex _letterRegex = new Regex("\\D", RegexOptions.Compiled);
		private Regex _upperRegex = new Regex("[^A-Z]", RegexOptions.Compiled);
		private Regex _wordEx = new Regex("\\b[\\w']+\\b", RegexOptions.Compiled);
		
		private bool _IgnoreAllCapsWords = true;
		private ArrayList _IgnoreList = new ArrayList();
		private bool _IgnoreWordsWithDigits = false;
		private int _mainDictionaryId = -1;
		private int _MaxSuggestions = 25;
		private DoubleMetaphone _meta = new DoubleMetaphone();
		private Hashtable _ReplaceList = new Hashtable();
		private string _ReplacementWord = "";
		private bool _ShowDialog = true;
		private SpellingForm _spellingForm;
		private ArrayList _Suggestions = new ArrayList();
		private StringBuilder _Text = new StringBuilder();
		private int _userDictionaryId = -1;
		private int _WordCount = 0;
		private int _WordIndex = 0;

		private MatchCollection _words;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		// subsitution chars for suggetions
		private char[] tryme = new char[] {'e', 's', 'i', 'a', 'n', 'r', 't', 'o', 'l',
			'c', 'd', 'u', 'g', 'm', 'p', 'h', 'b', 'y', 'f', 'v', 'k', 'w'};
		
		// common spelling errors
		private string[] replacementChars = new string[] {"a ei", "ei a", "a ey", "ey a", 
			"ai ie", "ie ai", "are air", "are ear", "are eir", "air are", "air ere", 
			"ere air", "ere ear", "ere eir", "ear are", "ear air", "ear ere", 
			"eir are", "eir ere", "ch te", "te ch", "ch ti", "ti ch", "ch tu", 
			"tu ch", "ch s", "s ch", "ch k", "k ch", "f ph", "ph f", "gh f", 
			"f gh", "i igh", "igh i", "i uy", "uy i", "i ee", "ee i", "j di", 
			"di j", "j gg", "gg j", "j ge", "ge j", "s ti", "ti s", "s ci", 
			"ci s", "k cc", "cc k", "k qu", "qu k", "kw qu", "o eau", "eau o", 
			"o ew", "ew o", "oo ew", "ew oo", "ew ui", "ui ew", "oo ui", "ui oo", 
			"ew u", "u ew", "oo u", "u oo", "u oe", "oe u", "u ieu", "ieu u", 
			"ue ew", "ew ue", "uff ough", "oo ieu", "ieu oo", "ier ear", "ear ier", 
			"ear air", "air ear", "w qu", "qu w", "z ss", "ss z", "shun tion", 
			"shun sion", "shun cion"};


		/// <summary>
		///     This event is fired when word is detected two times in a row
		/// </summary>
		public event DoubledWordEventHandler DoubledWord;

		/// <summary>
		///     This event is fired when the spell checker reaches the end of
		///     the text in the Text property
		/// </summary>
		public event EndOfTextEventHandler EndOfText;

		/// <summary>
		///     This event is fired when the spell checker finds a word that 
		///     is not in the dictionaries
		/// </summary>
		public event MisspelledWordEventHandler MisspelledWord;

		/// <summary>
		///     This represents the delegate method prototype that
		///     event receivers must implement
		/// </summary>
		public delegate void DoubledWordEventHandler(object sender, WordEventArgs args);

		/// <summary>
		///     This represents the delegate method prototype that
		///     event receivers must implement
		/// </summary>
		public delegate void EndOfTextEventHandler(object sender, System.EventArgs args);

		/// <summary>
		///     This represents the delegate method prototype that
		///     event receivers must implement
		/// </summary>
		public delegate void MisspelledWordEventHandler(object sender, WordEventArgs args);

		/// <summary>
		///     Initializes a new instance of the SpellCheck class
		/// </summary>
		public Spelling()
		{
			_spellingForm = new SpellingForm(this);
			InitializeComponent();
		}

		/// <summary>
		///     Initializes a new instance of the SpellCheck class with 
		///     the specified dictionary object. 
		/// </summary>
		/// <param name="dictionaries" type="FreeSpell.Dictionary">
		///     <para>
		///         The Dictionary object to use
		///     </para>
		/// </param>
		public Spelling(Dictionary[] dictionaries)
		{
			_Dictionaries.AddRange(dictionaries);
			_spellingForm = new SpellingForm(this);
			InitializeComponent();
		}

		/// <summary>
		///     Initializes a new instance of the SpellCheck class with 
		///     the specified dictionary object. 
		/// </summary>
		/// <param name="mainDictionary" type="FreeSpell.Dictionary">
		///     <para>
		///         The Dictionary object to use
		///     </para>
		/// </param>
		public Spelling(Dictionary mainDictionary)
		{
			_mainDictionaryId = _Dictionaries.Add(mainDictionary);
			_spellingForm = new SpellingForm(this);
			InitializeComponent();
		}

		/// <summary>
		///     Initializes a new instance of the SpellCheck class with 
		///     the specified dictionary file. 
		/// </summary>
		/// <param name="dictionaryFiles" type="string">
		///     <para>
		///         The name of the dictionary file to load
		///     </para>
		/// </param>
		public Spelling(string[] dictionaryFiles)
		{
			foreach (string file in dictionaryFiles)
			{
				_Dictionaries.Add(new Dictionary(file));
			}
			_spellingForm = new SpellingForm(this);
			InitializeComponent();
		}
		
		/// <summary>
		///     Initializes a new instance of the SpellCheck class with 
		///     the specified dictionary file. 
		/// </summary>
		/// <param name="mainDictionaryFile" type="string">
		///     <para>
		///         The main spell checker dictionary file name
		///     </para>
		/// </param>
		/// <param name="userDictionaryFile" type="string">
		///     <para>
		///         The user dictionary filename
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public Spelling(string mainDictionaryFile, string userDictionaryFile)
		{
			_mainDictionaryId = _Dictionaries.Add(new Dictionary(mainDictionaryFile));
			_userDictionaryId = _Dictionaries.Add(new Dictionary(userDictionaryFile));
			_spellingForm = new SpellingForm(this);
			InitializeComponent();
		}

		/// <summary>
		///     Initializes a new instance of the SpellCheck class with 
		///     the specified dictionary file. 
		/// </summary>
		/// <param name="mainDictionaryFile" type="string">
		///     <para>
		///          The main spell checker dictionary file name
		///     </para>
		/// </param>
		public Spelling(string mainDictionaryFile)
		{
			_mainDictionaryId = _Dictionaries.Add(new Dictionary(mainDictionaryFile));
			_spellingForm = new SpellingForm(this);
			InitializeComponent();
		}

		/// <summary>
		///     Required for Windows.Forms Class Composition Designer support
		/// </summary>
		public Spelling(System.ComponentModel.IContainer container)
		{
			container.Add(this);
			_spellingForm = new SpellingForm(this);
			InitializeComponent();
		}

		/// <summary>
		///     Calculates the words from the Text property
		/// </summary>
		private void CalculateWords()
		{
			// splits the text into words
			_words = _wordEx.Matches(_Text.ToString());
			_WordCount = _words.Count; // set word count
		}

		/// <summary>
		///     Determines if the string should be spell checked
		/// </summary>
		/// <param name="characters" type="string">
		///     <para>
		///         The Characters to check
		///     </para>
		/// </param>
		/// <returns>
		///     Returns true if the string should be spell checked
		/// </returns>
		private bool CheckString(string characters)
		{
			if(!_upperRegex.IsMatch(characters) && _IgnoreAllCapsWords)
			{
				return false;
			}
			if(_digitRegex.IsMatch(characters) && _IgnoreWordsWithDigits)
			{
				return false;
			}
			if(!_letterRegex.IsMatch(characters))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		///     Resets the public properties
		/// </summary>
		private void Reset()
		{
			_WordIndex = 0; // reset word index
			_CurrentWord = "";
			_ReplacementWord = "";
			_IgnoreList.Clear();
			_ReplaceList.Clear();
			_Suggestions.Clear();
		}

		/// <summary>
		///     Deletes the CurrentWord from the Text Property
		/// </summary>
		public void DeleteWord()
		{
			int index = _words[_WordIndex].Index;
			int length = _words[_WordIndex].Length;

			_Text.Remove(index, length);
			this.CalculateWords();
		}

		/// <summary>
		///     Ignores all instances of the CurrentWord in the Text Property
		/// </summary>
		public void IgnoreAllWord()
		{
			// Add current word to ignore list
			_IgnoreList.Add(_CurrentWord);
			this.IgnoreWord();
		}

		/// <summary>
		///     Ignores the instances of the CurrentWord in the Text Property
		/// </summary>
		/// <remarks>
		///		Must call SpellCheck after call this method to resume
		///		spell checking
		/// </remarks>
		public void IgnoreWord()
		{
			// increment Word Index to skip over this word
			_WordIndex++;
		}

		/// <summary>
		///     Replaces all instances of the CurrentWord in the Text Property
		/// </summary>
		public void ReplaceAllWord()
		{
			if(!_ReplaceList.ContainsKey(_CurrentWord)) 
			{
				_ReplaceList.Add(_CurrentWord, _ReplacementWord);
			}
			this.ReplaceWord();
		}

		/// <summary>
		///     Replaces all instances of the CurrentWord in the Text Property
		/// </summary>
		/// <param name="replacementWord" type="string">
		///     <para>
		///         The word to replace the CurrentWord with
		///     </para>
		/// </param>
		public void ReplaceAllWord(string replacementWord)
		{
			this.ReplacementWord = replacementWord;
			this.ReplaceAllWord();
		}

		/// <summary>
		///     Replaces the instances of the CurrentWord in the Text Property
		/// </summary>
		public void ReplaceWord()
		{
			int index = _words[_WordIndex].Index;
			int length = _words[_WordIndex].Length;
			
			_Text.Remove(index, length);
			if (_ReplacementWord.Length > 0) 
			{
				// if first letter upper case, match case for replacement word
				if (char.IsUpper(_words[_WordIndex].ToString(), 0))
				{
					_ReplacementWord = _ReplacementWord.Substring(0,1).ToUpper() 
						+ _ReplacementWord.Substring(1);
				}
				_Text.Insert(index, _ReplacementWord);
			}
			else if (index > 0) 
			{
				if (_Text.ToString().Substring(index-1, 2) == "  ")
				{
					//removing double space
					_Text.Remove(index, 1);
				}
			}
			this.CalculateWords();
		}

		/// <summary>
		///     Replaces the instances of the CurrentWord in the Text Property
		/// </summary>
		/// <param name="replacementWord" type="string">
		///     <para>
		///         The word to replace the CurrentWord with
		///     </para>
		/// </param>
		public void ReplaceWord(string replacementWord)
		{
			this.ReplacementWord = replacementWord;
			this.ReplaceWord();
		}

		/// <summary>
		///     A code that represents how the word sounds phonetically
		///     based on the way that it's spelled
		/// </summary>
		/// <param name="word" type="string">
		///     <para>
		///         The word to generate soundex code on
		///     </para>
		/// </param>
		/// <remarks>
		///		* Made obsolete by DoubleMetaphone class. This method is
		///		no longer used.
		///		
		///		This function is based off the Poor Man's Spell Checker
		///		by Sam Kirchmeier.  
		///		
		///		http://www.kirchmeier.org/code/pmsc/
		/// </remarks>
		/// <returns>
		///     The soundex code for the word
		/// </returns>
		public string Soundex(string word)
		{
			StringBuilder sb = new StringBuilder();
			//add first letter
			sb.Append(word.Substring(0,1).ToUpper()); 
			// loop through chars assigning code
			for (int i = 1; i < word.Length; i++) 
			{
				switch (word.Substring(i,1).ToUpper())
				{
					case "B": 
					case "P":
						sb.Append("1");
						break;
					case "F": 
					case "V":
						sb.Append("2");
						break;
					case "C": 
					case "K": 
					case "S":
						sb.Append("3");
						break;
					case "G": 
					case "J":
						sb.Append("4");
						break;
					case "Q": 
					case "X": 
					case "Z":
						sb.Append("5");
						break;
					case "D": 
					case "T":
						sb.Append("6");
						break;
					case "L":
						sb.Append("7");
						break;
					case "M": 
					case "N":
						sb.Append("8");
						break;
					case "R":
						sb.Append("9");
						break;
				}
			}
			return sb.ToString();
		}

		
		/// <summary>
		///     Spell checks the words in the <see cref="Text"/> property starting
		///     at the <see cref="WordIndex"/> position
		/// </summary>
		/// <returns>
		///     Returns true if there is a word found in the text 
		///     that is not in the dictionaries
		/// </returns>
		/// <seealso cref="CurrentWord"/>
		/// <seealso cref="WordIndex"/>
		/// <seealso cref="Dictionaries"/>
		public bool SpellCheck()
		{
			string currentWord = "";
			bool misspelledWord = false;
            
			if(_words != null) 
			{
				for (int i = _WordIndex; i < _words.Count; i++) 
				{
					currentWord = _words[i].Value.ToString();
					_CurrentWord = currentWord;		// setting the current word
					_WordIndex = i;					// saving the current word index 

					if(CheckString(currentWord)) 
					{
						if(!TestWord(currentWord)) 
						{
							if(_ReplaceList.ContainsKey(currentWord)) 
							{
								this.ReplacementWord = _ReplaceList[currentWord].ToString();
								this.ReplaceWord();
							}
							else if(!_IgnoreList.Contains(currentWord))
							{
								misspelledWord = true;
								this.OnMisspelledWord(new WordEventArgs(currentWord, i, _words[i].Index));		//raise event
								break;
							}
						}
						else if(i > 0) 
						{
							if(_words[i-1].Value.ToString() == currentWord) 
							{
								misspelledWord = true;
								this.OnDoubledWord(new WordEventArgs(currentWord, i, _words[i].Index));		//raise event
								break;
							}
						}
					}
				} // for

				if(_WordIndex >= _words.Count-1 && !misspelledWord) 
				{
					OnEndOfText(System.EventArgs.Empty);	//raise event
				}
			} // not words null

			return misspelledWord;

		} // SpellCheck

		/// <summary>
		///     Spell checks the words in the <see cref="Text"/> property starting
		///     at the <see cref="WordIndex"/> position. This overload takes in the
		///     WordIndex to start checking from.
		/// </summary>
		/// <param name="startWordIndex" type="int">
		///     <para>
		///         The index of the word to start checking from. 
		///     </para>
		/// </param>
		/// <returns>
		///     Returns true if there is a word found in the text 
		///     that is not in the dictionaries
		/// </returns>
		/// <seealso cref="CurrentWord"/>
		/// <seealso cref="WordIndex"/>
		/// <seealso cref="Dictionaries"/>
		public bool SpellCheck(int startWordIndex)
		{
			_WordIndex = startWordIndex;
			return SpellCheck();
		}
		
		/// <summary>
		///     Spell checks the words in the <see cref="Text"/> property starting
		///     at the <see cref="WordIndex"/> position. This overload takes in the 
		///     text to spell check
		/// </summary>
		/// <param name="text" type="string">
		///     <para>
		///         The text to spell check
		///     </para>
		/// </param>
		/// <returns>
		///     Returns true if there is a word found in the text 
		///     that is not in the dictionaries
		/// </returns>
		/// <seealso cref="CurrentWord"/>
		/// <seealso cref="WordIndex"/>
		/// <seealso cref="Dictionaries"/>
		public bool SpellCheck(string text)
		{
			this.Text = text;
			return SpellCheck();
		}

		/// <summary>
		///     Spell checks the words in the <see cref="Text"/> property starting
		///     at the <see cref="WordIndex"/> position. This overload takes in 
		///     the text to check and the WordIndex to start checking from.
		/// </summary>
		/// <param name="text" type="string">
		///     <para>
		///         The text to spell check
		///     </para>
		/// </param>
		/// <param name="startWordIndex" type="int">
		///     <para>
		///         The index of the word to start checking from
		///     </para>
		/// </param>
		/// <returns>
		///     Returns true if there is a word found in the text 
		///     that is not in the dictionaries
		/// </returns>
		/// <seealso cref="CurrentWord"/>
		/// <seealso cref="WordIndex"/>
		/// <seealso cref="Dictionaries"/>
		public bool SpellCheck(string text, int startWordIndex)
		{
			this.WordIndex = startWordIndex;
			this.Text = text;
			return SpellCheck();
		}

		/// <summary>
		///     Populates the <see cref="Suggestions"/> property with word suggestions
		///     for the <see cref="CurrentWord"/>
		/// </summary>
		/// <seealso cref="CurrentWord"/>
		/// <seealso cref="Suggestions"/>
		public void Suggest()
		{
			ArrayList tempSuggestion = new ArrayList();

			_meta.GenerateMetaphone(_CurrentWord);

			string priSearch = string.Concat("|", _meta.PrimaryCode, "|");
			string sndSearch = string.Concat("|", _meta.SecondaryCode, "|");

			// get suggestions by sound
			foreach (Dictionary dic in _Dictionaries)
			{
				foreach (string word in dic.WordList)
				{
					if(word.IndexOf(priSearch) > -1 || word.IndexOf(sndSearch) > -1)
					{
						string tempWord = word.Substring(0, word.IndexOf("|"));
						tempSuggestion.Add(new WordSuggestion(tempWord, 
							WordSimilarity(_CurrentWord, tempWord)));
					}
				} 
			} 
			
			// suggestions for a typical fault of spelling, that
			// differs with more, than 1 letter from the right form.
			this.ReplaceChars(ref tempSuggestion);

			// swap out each char one by one and try all the tryme
			// chars in its place to see if that makes a good word
			this.BadChar(ref tempSuggestion);

			// try omitting one char of word at a time
			this.ExtraChar(ref tempSuggestion);

			// try inserting a tryme character before every letter
			this.ForgotChar(ref tempSuggestion);

			// split the string into two pieces after every char
			// if both pieces are good words make them a suggestion
			this.TwoWords(ref tempSuggestion);

			// try swapping adjacent chars one by one
			this.SwapChar(ref tempSuggestion);

			tempSuggestion.Sort();  // sorts by word sim score
			_Suggestions.Clear(); 

			string lastWord = "";

			for (int i = 0; i < tempSuggestion.Count && _Suggestions.Count < _MaxSuggestions; i++)
			{
				if (((WordSuggestion)tempSuggestion[i]).Word != lastWord)
				{
					_Suggestions.Add(((WordSuggestion)tempSuggestion[i]).Word);
				}
				lastWord = ((WordSuggestion)tempSuggestion[i]).Word;
			}

		} // suggest

		/// <summary>
		///     Checks to see if the word is in the dictionary
		/// </summary>
		/// <param name="word" type="string">
		///     <para>
		///         The word to check
		///     </para>
		/// </param>
		/// <returns>
		///     Returns true if word is found in dictionary
		/// </returns>
		public bool TestWord(string word)
		{
			// dictionary stores words in 'word|PrimaryCode|SecondaryCode|' format
			_meta.GenerateMetaphone(word);
			string tempWord = string.Format("{0}|{1}|{2}|", 
				word, _meta.PrimaryCode, _meta.SecondaryCode);

			// search lower case 
			string lowerWord = string.Format("{0}|{1}|{2}|", 
				word.ToLower(), _meta.PrimaryCode, _meta.SecondaryCode);
				

			foreach (Dictionary dict in _Dictionaries)
			{
				if (dict.WordList.BinarySearch(tempWord) >= 0) return true;
				if (dict.WordList.BinarySearch(lowerWord) >= 0) return true;
			}
			return false;
		}

		/// <summary>
		///     Rates the words similarity based on two criteria: length, and letter content. 
		///     SimilarWord is awarded some points if it is about the same length as the word. 
		///     It is also awarded one point for each letter that they share. 
		/// </summary>
		/// <param name="word" type="string">
		///     <para>
		///         The base word to compare SimilarWord to 
		///     </para>
		/// </param>
		/// <param name="similarWord" type="string">
		///     <para>
		///         The word to rate similarity with
		///     </para>
		/// </param>
		/// <remarks>
		///		This function is based off the Poor Man's Spell Checker
		///		by Sam Kirchmeier.  
		///		
		///		http://www.kirchmeier.org/code/pmsc/
		/// </remarks>
		/// <returns>
		///     The score of how similar the words are
		/// </returns>
		public float WordSimilarity(string word, string similarWord)
		{
			
			float maxScore = 3F;
			float perfectScore = word.Length + word.Length + maxScore;
			// score for length
			float simScore = maxScore - Math.Abs(word.Length - similarWord.Length);

			for (int i = 0; i < word.Length; i++)
			{
				if(i < similarWord.Length) 
				{
					// score for same letter starting from front
					if(word.Substring(i,1).ToLower() 
						== similarWord.Substring(i,1).ToLower()) 
					{
						simScore++;
					}
					// score for same letter starting from back
					if(word.Substring(word.Length-1-i,1).ToLower() 
						== similarWord.Substring(similarWord.Length-1-i,1).ToLower()) 
					{
						simScore++;
					}
				}
			}
			return simScore/perfectScore;
		}

		/// <summary>
		///     This is the method that is responsible for notifying
		///     receivers that the event occurred
		/// </summary>
		protected virtual void OnDoubledWord(WordEventArgs e)
		{
			if (DoubledWord != null)
			{
				DoubledWord(this, e);
			}
		}

		/// <summary>
		///     This is the method that is responsible for notifying
		///     receivers that the event occurred
		/// </summary>
		protected virtual void OnEndOfText(System.EventArgs e)
		{
			if (EndOfText != null)
			{
				EndOfText(this, e);
			}
		}

		/// <summary>
		///     This is the method that is responsible for notifying
		///     receivers that the event occurred
		/// </summary>
		protected virtual void OnMisspelledWord(WordEventArgs e)
		{
			if (MisspelledWord != null)
			{
				MisspelledWord(this, e);
			}
		}

		/// <summary>
		///     The current word being spell checked from the text property
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string CurrentWord
		{
			get	{return _CurrentWord;}
		}

		/// <summary>
		///     A collection of dictionaries to use when spell checking
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DictionaryCollection Dictionaries
		{
			get {return _Dictionaries;}
		}

		/// <summary>
		///     Ignore words with all capital letters when spell checking
		/// </summary>
		[DefaultValue(true)]
		[CategoryAttribute("Options")]
		[Description("Ignore words with all capital letters when spell checking")]
		public bool IgnoreAllCapsWords
		{
			get {return _IgnoreAllCapsWords;}
			set {_IgnoreAllCapsWords = value;}
		}

		/// <summary>
		///     List of words to automatically ignore
		/// </summary>
		/// <remarks>
		///		When <see cref="IgnoreAllWord"/> is clicked, the <see cref="CurrentWord"/> is added to this list
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ArrayList IgnoreList
		{
			get {return _IgnoreList;}
			set {_IgnoreList = value;}
		}

		/// <summary>
		///     Ignore words with digits when spell checking
		/// </summary>
		[DefaultValue(false)]
		[CategoryAttribute("Options")]
		[Description("Ignore words with digits when spell checking")]
		public bool IgnoreWordsWithDigits
		{
			get {return _IgnoreWordsWithDigits;}
			set {_IgnoreWordsWithDigits = value;}
		}


		/// <summary>
		///		The file name for the main dictionary
		/// </summary>
		[DefaultValue("")]
		[CategoryAttribute("Dictionary")]
		[Description("The file name for the main dictionary")]
		[Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
		public string MainDictionary
		{
			get 
			{
				if (_mainDictionaryId != -1)
					return _Dictionaries[_mainDictionaryId].FileName;
				else
					return "";
			}
			set 
			{
				if (_mainDictionaryId == -1)
					_mainDictionaryId = _Dictionaries.Add(new Dictionary(value));
				else
					_Dictionaries[_mainDictionaryId].Load(value);
			}
		}

		/// <summary>
		///     The maximum number of suggestions to generate
		/// </summary>
		[DefaultValue(25)]
		[CategoryAttribute("Options")]
		[Description("The maximum number of suggestions to generate")]
		public int MaxSuggestions
		{
			get {return _MaxSuggestions;}
			set {_MaxSuggestions = value;}
		}

		/// <summary>
		///     List of words and replacement values to automatically replace
		/// </summary>
		/// <remarks>
		///		When <see cref="ReplaceAllWord"/> is clicked, the <see cref="CurrentWord"/> is added to this list
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Hashtable ReplaceList
		{
			get {return _ReplaceList;}
			set {_ReplaceList = value;}
		}

		/// <summary>
		///     The word to used when replacing the misspelled word
		/// </summary>
		/// <seealso cref="ReplaceAllWord"/>
		/// <seealso cref="ReplaceWord"/>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ReplacementWord
		{
			get {return _ReplacementWord;}
			set {_ReplacementWord = value;}
		}

		/// <summary>
		///     Determines if the spell checker should use its internal suggestions
		///     and options dialogs.
		/// </summary>
		[DefaultValue(true)]
		[CategoryAttribute("Options")]
		[Description("Determines if the spell checker should use its internal dialogs")]
		public bool ShowDialog
		{
			get {return _ShowDialog;}
			set 
			{
				_ShowDialog = value;
				if (_ShowDialog) _spellingForm.AttachEvents();
				else _spellingForm.DetachEvents();
			}
		}


		/// <summary>
		///     The internal spelling suggestions dialog form
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SpellingForm SpellingForm
		{
			get {return _spellingForm;}
		}

		/// <summary>
		///     An array of word suggestions for the correct spelling of the misspelled word
		/// </summary>
		/// <seealso cref="Suggest"/>
		/// <seealso cref="SpellCheck"/>
		/// <seealso cref="MaxSuggestions"/>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ArrayList Suggestions
		{
			get {return _Suggestions;}
		}

		/// <summary>
		///     The text to spell check
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Text
		{
			get {return _Text.ToString();}
			set 
			{
				_Text = new StringBuilder(value);
				this.CalculateWords();
				this.Reset();
			}
		}

		/// <summary>
		///		The file name of the current user dictionary
		/// </summary>
		/// <remarks>
		///		The user dictionary is where words get added by the user
		/// </remarks>
		[DefaultValue("")]
		[CategoryAttribute("Dictionary")]
		[Description("The file name for the user dictionary")]
		[Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
		public string UserDictionary
		{
			get 
			{
				if (_userDictionaryId != -1)
					return _Dictionaries[_userDictionaryId].FileName;
				else
					return "";
			}
			set 
			{
				if (_userDictionaryId == -1)
					_userDictionaryId = _Dictionaries.Add(new Dictionary(value));
				else
					_Dictionaries[_userDictionaryId].Load(value);
			}
		}

		/// <summary>
		///     The number of words being spell checked
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int WordCount
		{
			get {return _WordCount;}
		}

		/// <summary>
		///     WordIndex is the index of the current word being spell checked
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int WordIndex
		{
			get {return _WordIndex;}
			set {_WordIndex = value;}
		}

		/// <summary>
		///     This class is used to sort suggestions
		/// </summary>
		private class WordSuggestion : IComparable 
		{
			private float _SimilarScore;
			private string _Word;

			public WordSuggestion(string word, float similarScore)
			{
				_Word = word;
				_SimilarScore = similarScore;
			}

			/// <summary>
			///     Method inherited from the IComparable interface
			/// </summary>
			/// <remarks>
			///		Note: the compare sorts in desc order, largest score first
			/// </remarks>
			public int CompareTo(object obj)
			{
				int result = this.SimilarScore.CompareTo(((WordSuggestion)obj).SimilarScore);
				return result * -1; // sorts desc order
			}
			public float SimilarScore
			{
				get {return _SimilarScore;}
				set {_SimilarScore = value;}
			}
			public string Word
			{
				get {return _Word;}
				set {_Word = value;}
			}

		}
#region ISpell Suggetion Helper functions

		/// <summary>
		///		swap out each char one by one and try all the tryme
		///		chars in its place to see if that makes a good word
		/// </summary>
		public void BadChar(ref ArrayList tempSuggestion)
		{
			for (int i = 0; i < _CurrentWord.Length; i++)
			{
				StringBuilder tempWord = new StringBuilder(_CurrentWord);
				for (int x = 0; x < tryme.Length; x++)
				{
					tempWord[i] = tryme[x];
					if (this.TestWord(tempWord.ToString())) 
					{
						WordSuggestion ws = new WordSuggestion(tempWord.ToString().ToLower(), 
							WordSimilarity(_CurrentWord, tempWord.ToString()));
					
						tempSuggestion.Add(ws);
					}
				}				 
			}
		}

		/// <summary>
		///     try omitting one char of word at a time
		/// </summary>
		private void ExtraChar(ref ArrayList tempSuggestion)
		{
			if (_CurrentWord.Length > 1) 
			{
				for (int i = 0; i < _CurrentWord.Length; i++)
				{
					StringBuilder tempWord = new StringBuilder(_CurrentWord);
					tempWord.Remove(i, 1);

					if (this.TestWord(tempWord.ToString())) 
					{
						WordSuggestion ws = new WordSuggestion(tempWord.ToString().ToLower(), 
							WordSimilarity(_CurrentWord, tempWord.ToString()));
				
						tempSuggestion.Add(ws);
					}
								 
				}
			}
		}

		/// <summary>
		///     try inserting a tryme character before every letter
		/// </summary>
		private void ForgotChar(ref ArrayList tempSuggestion)
		{
			for (int i = 0; i < _CurrentWord.Length; i++)
			{
				for (int x = 0; x < tryme.Length; x++)
				{
					StringBuilder tempWord = new StringBuilder(_CurrentWord);
				
					tempWord.Insert(i, tryme[x]);
					if (this.TestWord(tempWord.ToString())) 
					{
						WordSuggestion ws = new WordSuggestion(tempWord.ToString().ToLower(), 
							WordSimilarity(_CurrentWord, tempWord.ToString()));
					
						tempSuggestion.Add(ws);
					}
				}				 
			}
		}

		/// <summary>
		///     suggestions for a typical fault of spelling, that
		///		differs with more, than 1 letter from the right form.
		/// </summary>
		private void ReplaceChars(ref ArrayList tempSuggestion)
		{
			for (int i = 0; i < replacementChars.Length; i++)
			{
				int split = replacementChars[i].IndexOf(' ');
				string key = replacementChars[i].Substring(0, split);
				string replacement = replacementChars[i].Substring(split+1);

				int pos = _CurrentWord.IndexOf(key);
				while (pos > -1)
				{
					string tempWord = _CurrentWord.Substring(0, pos);
					tempWord += replacement;
					tempWord += _CurrentWord.Substring(pos + key.Length);

					if (this.TestWord(tempWord))
					{
						WordSuggestion ws = new WordSuggestion(tempWord.ToLower(), 
							WordSimilarity(_CurrentWord, tempWord));
					
						tempSuggestion.Add(ws);
					}
					pos = _CurrentWord.IndexOf(key, pos+1);
				}
			}
		}

		/// <summary>
		///     try swapping adjacent chars one by one
		/// </summary>
		private void SwapChar(ref ArrayList tempSuggestion)
		{
			for (int i = 0; i < _CurrentWord.Length - 1; i++)
			{
				StringBuilder tempWord = new StringBuilder(_CurrentWord);
				
				char swap = tempWord[i];
				tempWord[i] = tempWord[i+1];
				tempWord[i+1] = swap;

				if (this.TestWord(tempWord.ToString())) 
				{
					WordSuggestion ws = new WordSuggestion(tempWord.ToString().ToLower(), 
						WordSimilarity(_CurrentWord, tempWord.ToString()));
				
					tempSuggestion.Add(ws);
				}	 
			}
		}
		
		/// <summary>
		///     split the string into two pieces after every char
		///		if both pieces are good words make them a suggestion
		/// </summary>
		private void TwoWords(ref ArrayList tempSuggestion)
		{
			for (int i = 1; i < _CurrentWord.Length - 1; i++)
			{
				string firstWord = _CurrentWord.Substring(0,i);
				string secondWord = _CurrentWord.Substring(i);
				
				if (this.TestWord(firstWord) && this.TestWord(secondWord)) 
				{
					string tempWord = firstWord + " " + secondWord;
					WordSuggestion ws = new WordSuggestion(tempWord.ToLower(), 
						WordSimilarity(_CurrentWord, tempWord));
				
					tempSuggestion.Add(ws);
				}	 
			}
		}

#endregion

#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
#endregion

	} // Class SpellChecker

	/// <summary>
	///     Class sent to the event handler when the DoubleWord or 
	///     MisspelledWord event occurs
	/// </summary>
	public class WordEventArgs : EventArgs 
	{
		private int _TextIndex;
		private string _Word;
		private int _WordIndex;

		/// <summary>
		///     Constructor used to pass in properties
		/// </summary>
		public WordEventArgs(string word, int wordIndex, int textIndex)
		{
			_Word = word;
			_WordIndex = wordIndex;
			_TextIndex = textIndex;
		}

		/// <summary>
		///     Text index of the WordEvent
		/// </summary>
		public int TextIndex
		{
			get {return _TextIndex;}
		}

		/// <summary>
		///     Word that caused the WordEvent
		/// </summary>
		public string Word
		{
			get {return _Word;}
		}

		/// <summary>
		///     Word index of the WordEvent
		/// </summary>
		public int WordIndex
		{
			get {return _WordIndex;}
		}

	} // Class WordEventArgs


}
