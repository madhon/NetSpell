// Copyright (c) 2003, Paul Welter
// All rights reserved.

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Diagnostics;

using NUnit.Framework;
using NetSpell.SpellChecker;
using NetSpell.SpellChecker.Dictionary;
using NetSpell.SpellChecker.Dictionary.Phonetic;
using NetSpell.SpellChecker.Dictionary.Affix;

namespace NetSpell.Tests
{
	/// <summary>
	/// Summary description for DictionaryTest.
	/// </summary>
	[TestFixture]
	public class DictionaryTest
	{
		WordDictionary _WordDictionary = new WordDictionary();
		PerformanceTimer _timer = new PerformanceTimer();


		public DictionaryTest()
		{
		}

		[SetUp]
		public void SetUp()
		{
			_WordDictionary.DictionaryFolder = @"..\..\..\..\dic";
			_WordDictionary.Initialize();
		}

		[Test]
		public void Contains() 
		{
			string validFile = @"..\..\Data\ValidWords.txt";
			string invalidFile = @"..\..\Data\SuggestionTest.txt";
			
			// open file
			FileStream fs = new FileStream(validFile, FileMode.Open, FileAccess.Read, FileShare.Read);
			StreamReader sr = new StreamReader(fs, Encoding.UTF7);
			
			_timer.StartTimer();

			// read line by line
			while (sr.Peek() >= 0) 
			{
				string tempLine = sr.ReadLine().Trim();
				if (tempLine.Length > 0)
				{
					if (tempLine.IndexOf(' ') > 0)
					{
						tempLine = tempLine.Substring(0, tempLine.IndexOf(' '));
					}

					if (!_WordDictionary.Contains(tempLine)) 
					{
						Assert.Fail(string.Format("Did not find word: {0}" , tempLine));
					}
				}
			}
			float checkTime = _timer.StopTimer();
			Console.WriteLine("Valid words check time:" + checkTime.ToString());

			sr.Close();
			fs.Close();

			
			// open file
			fs = new FileStream(invalidFile, FileMode.Open, FileAccess.Read, FileShare.Read);
			sr = new StreamReader(fs, Encoding.UTF7);
			
			_timer.StartTimer();
			// read line by line
			while (sr.Peek() >= 0) 
			{
				string tempLine = sr.ReadLine().Trim();
				if (tempLine.Length > 0)
				{
					if (tempLine.IndexOf(' ') > 0)
					{
						tempLine = tempLine.Substring(0, tempLine.IndexOf(' '));
					}

					if (_WordDictionary.Contains(tempLine)) 
					{
						Assert.Fail(string.Format("Word found that should not be: {0}" , tempLine));
					}
				}

			}
			float invalidTime = _timer.StopTimer();
			Console.WriteLine("Invalid words check time:" + invalidTime.ToString());
			
			sr.Close();
			fs.Close();
		}

		[Test]
		public void PhoneticCode()
		{
			string code = _WordDictionary.PhoneticCode("test");

		    Assert.AreEqual("Incorrect Phonitic Code", "*BRFTT", _WordDictionary.PhoneticCode("abbreviated"));
			Assert.AreEqual("Incorrect Phonitic Code", "*BLT", _WordDictionary.PhoneticCode("ability"));
			Assert.AreEqual("Incorrect Phonitic Code", "NMNT", _WordDictionary.PhoneticCode("nominate"));
			Assert.AreEqual("Incorrect Phonitic Code", "NN", _WordDictionary.PhoneticCode("noun"));
			Assert.AreEqual("Incorrect Phonitic Code", "*BKKT", _WordDictionary.PhoneticCode("object"));
			Assert.AreEqual("Incorrect Phonitic Code", "*TKR", _WordDictionary.PhoneticCode("outgrow"));
			Assert.AreEqual("Incorrect Phonitic Code", "*TLNTX", _WordDictionary.PhoneticCode("outlandish"));
			Assert.AreEqual("Incorrect Phonitic Code", "PBLX", _WordDictionary.PhoneticCode("publish"));
			Assert.AreEqual("Incorrect Phonitic Code", "STL", _WordDictionary.PhoneticCode("sightly"));
			Assert.AreEqual("Incorrect Phonitic Code", "SPL", _WordDictionary.PhoneticCode("supple"));
			Assert.AreEqual("Incorrect Phonitic Code", "TRTNS", _WordDictionary.PhoneticCode("triteness"));

		}

		[Test]
		public void ExpandWord()
		{
			
			ArrayList words = new ArrayList();

			words = _WordDictionary.ExpandWord(new Word("abbreviated", "UA"));
			Assert.AreEqual("Incorrect Number of expanded words", 3, words.Count.ToString());

			words = _WordDictionary.ExpandWord(new Word("ability", "IMES"));
			Assert.AreEqual("Incorrect Number of expanded words", 9, words.Count.ToString());

			words = _WordDictionary.ExpandWord(new Word("nominate", "CDSAXNG"));
			Assert.AreEqual("Incorrect Number of expanded words", 18, words.Count.ToString());

			words = _WordDictionary.ExpandWord(new Word("noun", "SMK"));
			Assert.AreEqual("Incorrect Number of expanded words", 6, words.Count.ToString());

			words = _WordDictionary.ExpandWord(new Word("object", "SGVMD"));
			Assert.AreEqual("Incorrect Number of expanded words", 6, words.Count.ToString());

			words = _WordDictionary.ExpandWord(new Word("outgrow", "GSH"));
			Assert.AreEqual("Incorrect Number of expanded words", 4, words.Count.ToString());

			words = _WordDictionary.ExpandWord(new Word("outlandish", "PY"));
			Assert.AreEqual("Incorrect Number of expanded words", 3, words.Count.ToString());

			words = _WordDictionary.ExpandWord(new Word("publish", "JDRSBZG"));
			Assert.AreEqual("Incorrect Number of expanded words", 8, words.Count.ToString());

			words = _WordDictionary.ExpandWord(new Word("sightly", "TURP"));
			Assert.AreEqual("Incorrect Number of expanded words", 7, words.Count.ToString());

			words = _WordDictionary.ExpandWord(new Word("supple", "SPLY"));
			Assert.AreEqual("Incorrect Number of expanded words", 5, words.Count.ToString());

			words = _WordDictionary.ExpandWord(new Word("triteness", "SF"));
			Assert.AreEqual("Incorrect Number of expanded words", 4, words.Count.ToString());

		}

	}
}
