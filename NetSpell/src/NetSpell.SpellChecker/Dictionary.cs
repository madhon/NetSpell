// Copyright (c) 2003, Paul Welter
// All rights reserved.

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;

namespace NetSpell.SpellChecker
{

	#region Dictionary Object

	/// <summary>
	///		The Dictionary class encapsulates all functions associated 
	///		with the word dictionary.
	/// </summary>
	/// <remarks>
	///		When adding or removing words, you must call save before
	///		using the dictionary because the word list must be sorted.
	/// </remarks>
	public class Dictionary
	{
		private string _FileName;
		private ArrayList _WordList = new ArrayList();

		/// <summary>
		///     Dictionary default constructor
		/// </summary>
		public Dictionary()
		{
		}

		/// <summary>
		///     Dictionary constructor that takes in a file name
		///     to automatically load the dictionary from
		/// </summary>
		/// <param name="fileName" type="string">
		///     <para>
		///         The name of the dictionary file to load
		///     </para>
		/// </param>
		public Dictionary(string fileName)
		{
			this.FileName = fileName;
			this.Load();
		}

		/// <summary>
		///     Adds a word to the dictionary
		/// </summary>
		/// <remarks>
		///		You must call save before using the dictionary
		///		because the word list must be sorted.
		/// </remarks>
		/// <param name="word" type="string">
		///     <para>
		///         The word to add to the dictionary
		///     </para>
		/// </param>
		public void AddWord(string word)
		{
			DoubleMetaphone meta = new DoubleMetaphone(word);
			_WordList.Add(word + "|" + meta.PrimaryCode + "|" + meta.SecondaryCode + "|");
		}

		/// <summary>
		///     Loads the Dictionary Word List from the FileName
		/// </summary>
		public void Load()
		{
			if (_FileName.Length > 0) 
			{
				FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
				StreamReader sr = new StreamReader(fs, Encoding.UTF8);

				_WordList.AddRange(sr.ReadToEnd().Split());
			
				sr.Close();
				fs.Close();
			}
		}

		/// <summary>
		///     Loads the Dictionary Word List from the FileName
		/// </summary>
		/// <param name="fileName" type="string">
		///     <para>
		///         The name of the dictionary file to load
		///     </para>
		/// </param>
		public void Load(string fileName)
		{
			this.FileName = fileName;
			this.Load();
		}

		/// <summary>
		///     Saves the Dictionary Word List to a file
		/// </summary>
		public void Save()
		{
			//sorting the word list for binary search
			_WordList.Sort();

			FileStream fs = new FileStream(_FileName, FileMode.Create, FileAccess.Write); 
			StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
				
			sw.Write(string.Join("\n", (string[])_WordList.ToArray(typeof(string))));

			sw.Close();
			fs.Close();
		}

		/// <summary>
		///     Saves the Dictionary Word List to a file
		/// </summary>
		/// <param name="fileName" type="string">
		///     <para>
		///         The name of the dictionary file to save
		///     </para>
		/// </param>
		public void Save(string fileName)
		{
			this.FileName = fileName;
			this.Save();
		}
		
		/// <summary>
		///     The file name and path of the dictionary file
		/// </summary>
		public string FileName
		{
			get {return _FileName;}
			set {_FileName = value;}
		}

		/// <summary>
		///     The list of words in this dictionary
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ArrayList WordList
		{
			get {return _WordList;}
		}

	}

	#endregion //Dictionary Object

}
