using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using NetSpell.SpellChecker;
using NetSpell.SpellChecker.Dictionary.Affix;
using NetSpell.SpellChecker.Dictionary.Phonetic;

namespace NetSpell.SpellChecker.Dictionary
{

	/// <summary>
	/// Summary description for WordDictionary.
	/// </summary>
	public class WordDictionary : IEnumerable 
	{

		private Hashtable _BaseWords = new Hashtable();
		private string _DictionaryFile = "";
		private PhoneticRuleCollection _PhoneticRules = new PhoneticRuleCollection();
		private ArrayList _PossibleBaseWords = new ArrayList();
		private AffixRuleCollection _PrefixRules = new AffixRuleCollection();
		private ArrayList _ReplaceCharacters = new ArrayList();
		private AffixRuleCollection _SuffixRules = new AffixRuleCollection();
		private string _TryCharacters = "";
		private string _UserFile = "";
		private Hashtable _UserWords = new Hashtable();

		/// <summary>
		///     Initializes a new instance of the class
		/// </summary>
		public WordDictionary()
		{
		}

		/// <summary>
		///     Initializes the dictionary by loading and parsing the
		///     dictionary file and the user file.
		/// </summary>
		public void Initialize()
		{
			// clean up data first
			_BaseWords.Clear();
			_ReplaceCharacters.Clear();
			_PrefixRules.Clear();
			_SuffixRules.Clear();
			_PhoneticRules.Clear();
			_TryCharacters = "";
			

			// the following is used to split a line by space
			Regex _spaceRegx = new Regex(@"[^\s]+", RegexOptions.Compiled);
			
			string currentSection = "";
			AffixRule currentRule = null;

			// open dictionary file
			FileStream fs = new FileStream(_DictionaryFile, FileMode.Open, FileAccess.Read, FileShare.Read);
			StreamReader sr = new StreamReader(fs, Encoding.UTF7);
			
			// read line by line
			while (sr.Peek() >= 0) 
			{
				string tempLine = sr.ReadLine().Trim();
				if (tempLine.Length > 0)
				{
					// check for section flag
					switch (tempLine)
					{
						case "[Try]" : 
						case "[Replace]" : 
						case "[Prefix]" :
						case "[Suffix]" :
						case "[Phonetic]" :
						case "[Words]" :
							// set current section that is being parsed
							currentSection = tempLine;
							break;
						default :		
							// parse line a place in correct object
						switch (currentSection)
						{
							case "[Try]" : // ISpell try chars
								this.TryCharacters += tempLine;
								break;
							case "[Replace]" : // ISpell replace chars
								this.ReplaceCharacters.Add(tempLine);
								break;
							case "[Prefix]" : // MySpell prefix rules
							case "[Suffix]" : // MySpell suffix rules

								// split line by white space
								MatchCollection partMatches = _spaceRegx.Matches(tempLine);
									
								// if 3 parts, then new rule  
								if (partMatches.Count == 3)
								{
									currentRule = new AffixRule();
									
									// part 1 = affix key
									currentRule.Name = partMatches[0].Value;
									// part 2 = combine flag
									if (partMatches[1].Value == "Y") currentRule.AllowCombine = true;
									// part 3 = entry count, not used

									if (currentSection == "[Prefix]")
									{
										// add to prefix collection
										this.PrefixRules.Add(currentRule.Name, currentRule);
									}
									else 
									{
										// add to suffix collection
										this.SuffixRules.Add(currentRule.Name, currentRule);
									}
								}
									//if 4 parts, then entry for current rule
								else if (partMatches.Count == 4)
								{
									// part 1 = affix key
									if (currentRule.Name == partMatches[0].Value)
									{
										AffixEntry entry = new AffixEntry();

										// part 2 = strip char
										if (partMatches[1].Value != "0") entry.StripCharacters = partMatches[1].Value;
										// part 3 = add chars
										entry.AddCharacters = partMatches[2].Value;
										// part 4 = conditions
										AffixUtility.EncodeConditions(partMatches[3].Value, entry);

										currentRule.AffixEntries.Add(entry);
									}
								}	
								break;
							case "[Phonetic]" : // ASpell phonetic rules
								// TODO: parse phonectic rules
								break;
							case "[Words]" : // dictionary word list
								// splits word into its parts
								string[] parts = tempLine.Split('/');
								Word tempWord = new Word();
								// part 1 = base word
								tempWord.Value = parts[0];
								// part 2 = affix keys
								if (parts.Length >= 2) tempWord.AffixKeys = parts[1];
								// part 3 = phonetic code
								if (parts.Length >= 3) tempWord.PhoneticCode = parts[2];
								this.BaseWords.Add(tempWord.Value, tempWord);

								break;
						} // currentSection swith
							break;
					} //tempLine switch
				} // if templine
			} // read line
			// close files
			sr.Close();
			fs.Close();
		}

		/// <summary>
		///     The collection of base words for the dictionary
		/// </summary>
		public Hashtable BaseWords
		{
			get {return _BaseWords;}
			set {_BaseWords = value;}
		}

		/// <summary>
		///     The file name for this dictionary
		/// </summary>
		public string DictionaryFile
		{
			get {return _DictionaryFile;}
			set {_DictionaryFile = value;}
		}


		/// <summary>
		///     Collection of phonetic rules for this dictionary
		/// </summary>
		public PhoneticRuleCollection PhoneticRules
		{
			get {return _PhoneticRules;}
			set {_PhoneticRules = value;}
		}


		/// <summary>
		///     Collection of affix prefixes for the base words in this dictionary
		/// </summary>
		public AffixRuleCollection PrefixRules
		{
			get {return _PrefixRules;}
			set {_PrefixRules = value;}
		}

		/// <summary>
		///     List of characters to use when generating suggestions using the near miss stratigy
		/// </summary>
		public ArrayList ReplaceCharacters
		{
			get {return _ReplaceCharacters;}
			set {_ReplaceCharacters = value;}
		}


		/// <summary>
		///     Collection of affix suffixes for the base words in this dictionary
		/// </summary>
		public AffixRuleCollection SuffixRules
		{
			get {return _SuffixRules;}
			set {_SuffixRules = value;}
		}

		/// <summary>
		///     List of characters to try when generating suggestions using the near miss stratigy
		/// </summary>
		public string TryCharacters
		{
			get {return _TryCharacters;}
			set {_TryCharacters = value;}
		}

		/// <summary>
		///     file name for the user word list for this dictionary
		/// </summary>
		public string UserFile
		{
			get {return _UserFile;}
			set {_UserFile = value;}
		}

		
		/// <summary>
		///     List of user entered words in this dictionary
		/// </summary>
		public Hashtable UserWords
		{
			get {return _UserWords;}
			set {_UserWords = value;}
		}

		internal ArrayList PossibleBaseWords
		{
			get {return _PossibleBaseWords;}
		}

		#region Public IDictionary Members

		/// <summary>
		///     Adds a word to the user list
		/// </summary>
		/// <param name="word" type="string">
		///     <para>
		///         The word to add
		///     </para>
		/// </param>
		/// <param name="value" type="NetSpell.SpellChecker.Dictionary.Word">
		///     <para>
		///         The word object to add
		///     </para>
		/// </param>
		/// <remarks>
		///		This method is only affects the user word list
		/// </remarks>
		public void Add(string word, Word value)
		{
			_UserWords.Add(word, value);
		}

		/// <summary>
		///     Clears the user list of words
		/// </summary>
		/// <remarks>
		///		This method is only affects the user word list
		/// </remarks>
		public void Clear()
		{
			_UserWords.Clear();
		}

		/// <summary>
		///     Searches all contained word lists for word
		/// </summary>
		/// <param name="word" type="string">
		///     <para>
		///         The word to search for
		///     </para>
		/// </param>
		/// <returns>
		///     Returns true if word is found
		/// </returns>
		public bool Contains(string word)
		{
			// clean up possible base word list
			_PossibleBaseWords.Clear();

			// Step 1 Search UserWords
			if (_UserWords.Contains(word)) 
			{
				return true;  // word found
			}

			// Step 2 Search BaseWords
			if (_BaseWords.Contains(word)) 
			{
				return true; // word found
			}

			// Step 3 Remove suffix, Search BaseWords

			// save suffixed words for use when removing prefix
			ArrayList suffixWords = new ArrayList();
			// Add word to suffix word list
			suffixWords.Add(word);

			foreach(AffixRule rule in SuffixRules.Values)
			{	
				foreach(AffixEntry entry in rule.AffixEntries)
				{
					string tempWord = AffixUtility.RemoveSuffix(word, entry);
					if(tempWord != word)
					{
						if (_BaseWords.Contains(tempWord))
						{
							return true; // word found
						}
						else if(rule.AllowCombine)
						{
							// saving word to check if it is a word after prefix is removed
							suffixWords.Add(tempWord);
						}
					}
				}
			}
			// saving possible base words for use in generating suggestions
			_PossibleBaseWords.AddRange(suffixWords);

			// Step 4 Remove Prefix, Search BaseWords
			foreach(AffixRule rule in PrefixRules.Values)
			{
				foreach(AffixEntry entry in rule.AffixEntries)
				{
					foreach(string suffixWord in suffixWords)
					{
						string tempWord = AffixUtility.RemovePrefix(suffixWord, entry);
						if (tempWord != suffixWord)
						{
							if (_BaseWords.Contains(tempWord))
							{
								return true; // word found
							}
							else
							{
								// saving possible base words for use in generating suggestions
								_PossibleBaseWords.Add(tempWord);
							}
						}
					} // suffix word
				} // prefix rule entry
			} // prefix rule
			// word not found 
			return false;
		}

		/// <summary>
		///     Removes a word from the user list
		/// </summary>
		/// <param name="word" type="string">
		///     <para>
		///         The word to remove
		///     </para>
		/// </param>
		/// <remarks>
		///		This method is only affects the user word list
		/// </remarks>
		public void Remove(string word)
		{
			_UserWords.Remove(word);
		}


		#endregion


		#region IEnumerable Members

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			// TODO:  Add WordDictionary.System.Collections.IEnumerable.GetEnumerator implementation
			return null;
		}

		#endregion

	}

}
