// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using A2v10.Data.Interfaces;

namespace A2v10.Data.Providers.Csv
{
	public class CsvReader :  IExternalDataReader
	{
		private readonly DataFile _file;

		public CsvReader(DataFile file)
		{
			_file = file;
		}

		public IExternalDataFile Read(Stream stream)
		{
			// FindEncoding & delimiter
			FindEncoding(stream);
			using (StreamReader rdr = new StreamReader(stream, _file.Encoding))
			{
				ReadHeader(rdr);
				Read(rdr);
			}
			return _file; 
		}

		void FindEncoding(Stream stream)
		{
			using (var br = new BinaryReader(stream, Encoding.Default, true))
			{
				var bytes = br.ReadBytes(2048);
				_file.Encoding = _file.FindDecoding(bytes);
			}
			stream.Seek(0, SeekOrigin.Begin);
		}

		void ReadHeader (TextReader rdr)
		{
			String header = rdr.ReadLine();
			ParseHeader(header);
		}

		void Read(StreamReader rdr)
		{
			while (!rdr.EndOfStream)
			{
				String line = rdr.ReadLine();
				var items = ParseLine(line);
				var r = _file.CreateRecord();
				for (var i = 0; i < items.Count; i++)
					r.DataFields.Add(new FieldData() { StringValue = items[i] });
			}
		}

		void ParseHeader(String header)
		{
			// find delimiters
			var delims = new Dictionary<Char, Int32>()
			{
				{ ';',  0 },
				{ ',',  0 },
				{ '\t', 0 },
				{ '|',  0 },
			};
			for (Int32 i=0; i<header.Length; i++)
			{
				Char ch = header[i];
				if (delims.TryGetValue(ch, out Int32 cnt))
					delims[ch] = cnt + 1;
			}
			var list = delims.ToList();
			list.Sort((v1, v2) => v2.Value.CompareTo(v1.Value)); // desc
			_file.Delimiter = list[0].Key;
			var fields = ParseLine(header);
			for (var i = 0; i < fields.Count; i++) {
				var f = _file.CreateField();
				f.Name = fields[i];
			}
			_file.MapFields();
		}

		IList<String> ParseLine(String line)
		{
			// very simple tokenizer
			Int32 ix = 0;
			Int32 len = line.Length;
			StringBuilder token = new StringBuilder();
			Char ch;
			var retval = new List<String>();

			Char _nextChar()
			{
				if (ix >= len)
					return '\0';
				Char currChar = line[ix];
				ix++;
				return currChar;
			}

			void _addToken()
			{
				retval.Add(token.ToString());
				token.Clear();
			}

			void _readString()
			{
				Char sch;
				token.Clear();
				while (true)
				{
					sch = _nextChar();
					if (sch == '"')
					{
						var nextStrChar = _nextChar();
						if (nextStrChar == '"')
							token.Append(nextStrChar);
						else
						{
							_addToken();
							break;
						}
					} else
					{
						token.Append(sch);
					}
				}
			}

			while (true)
			{
				ch = _nextChar();
				if (ch == '\0')
				{
					if (token.Length > 0)
						retval.Add(token.ToString());
					return retval;
				}
				if (ch == _file.Delimiter)
				{
					_addToken();
				}
				else if (ch == '"')
				{
					if (token.Length == 0)
						_readString();
					else
						token.Append(ch); // inside string
				}
				else
				{
					token.Append(ch);
				}
			}
		}
	}
}
