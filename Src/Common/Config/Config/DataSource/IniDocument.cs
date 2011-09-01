using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Avanade.Config.Impl;

namespace Avanade.Config.DataSource
{

    #region Enumerations

    /// <summary>
    /// The type of the ini line type.
    /// </summary>
    enum IniLineType
    {
        Group, KeyValue, Comment, Other, EmptyLine, None
    }

    #endregion Enumerations

    /// <summary>
    /// Interface for an IniParser, This is a parser that supports
    /// parsing multiple lines for the values.
    /// e.g. 
    /// 
    /// [post1]
    /// title: My first post
    /// description: " This is a ini parser that can handle
    /// parsing multiples lines for the value. "
    /// 
    /// </summary>
    public interface IIniParser
    {
        #region Properties

        /// <summary>
        /// List of errors
        /// </summary>
        IList<string> Errors
        {
            get;
        }

        /// <summary>
        /// Settings for the parser.
        /// </summary>
        IniParserSettings Settings
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Parse the ini content.
        /// </summary>
        /// <param name="iniContent"></param>
        /// <returns></returns>
        List<IniSection> Parse(string iniContent);

        #endregion Methods
    }

    /// <summary>
    /// IniDocument to handle loading and writing(not yet done)
    /// of ini files.
    /// 
    /// This class can load a MULTI-LINE ini file into a dictionary like data structure.
    /// 
    /// [BlogPost1]
    /// Title : Introduction to Oil painting class.
    /// Description : "Learn how to paint using
    /// oil, in this beginners class for painting enthusiats."
    /// Url : http://www.knowledgedrink.com
    /// </summary>
    public class IniDocument : ConfigSource
    {
        #region Fields

        private string iniContent = "";
        private string iniFilePath = "";
        private bool isCaseSensitive;
        private bool isFileBased;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Default initialization.
        /// </summary>
        public IniDocument()
        {
        }

        /// <summary>
        /// Initialize using IniSections.
        /// </summary>
        /// <param name="sections"></param>
        public IniDocument(List<ConfigSection> sections)
        {
            sections.ForEach(s => Add(s.Name, s));
        }

        /// <summary>
        /// Initialize using IniSections.
        /// </summary>
        /// <param name="iniContentOrFilePath"></param>
        /// <param name="isFilePath"></param>
        public IniDocument(string iniContentOrFilePath, bool isFilePath)
            : this("", iniContentOrFilePath, isFilePath, true)
        {
        }

        /// <summary>
        /// Initialize using IniSections.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="iniContentOrFilePath"></param>
        /// <param name="isFilePath"></param>
        public IniDocument(string name, string iniContentOrFilePath, bool isFilePath)
            : this(name, iniContentOrFilePath, isFilePath, true)
        {
        }

        /// <summary>
        /// Initialize the ini document with the string or file path.
        /// </summary>
        /// <param name="iniContentOrFilePath"></param>
        /// <param name="isFilePath"></param>
        /// <param name="isCaseSensitive"></param>
        public IniDocument(string iniContentOrFilePath, bool isFilePath, bool isCaseSensitive)
            : this("", iniContentOrFilePath, isFilePath, isCaseSensitive)
        {
        }

        /// <summary>
        /// Initialize the ini document with the string or file path.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="iniContentOrFilePath"></param>
        /// <param name="isFilePath"></param>
        /// <param name="isCaseSensitive"></param>
        public IniDocument(string name, string iniContentOrFilePath, bool isFilePath, bool isCaseSensitive)
        {
            Init(name, iniContentOrFilePath, isFilePath, isCaseSensitive);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Source path of this file.
        /// </summary>
        public override string SourcePath
        {
            get
            {
                if (isFileBased) return iniFilePath;

                return string.Empty;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="iniContentOrFilePath"></param>
        /// <param name="isFilePath"></param>
        /// <param name="caseSensitive"></param>
        public void Init(string name, string iniContentOrFilePath, bool isFilePath, bool caseSensitive)
        {
            Name = name;
            iniContent = iniContentOrFilePath;
            isCaseSensitive = caseSensitive;
            if (isFilePath)
            {
                iniFilePath = iniContentOrFilePath;
                iniContent = File.ReadAllText(iniContentOrFilePath);
                isFileBased = true;
            }
            Load();
        }

        /// <summary>
        /// Load settings.
        /// </summary>
        public override void Load()
        {
            // Handle empty ini file.
            // This is possible and very possible for inherited configs.
            if (string.IsNullOrEmpty(iniContent))
                return;

            IIniParser parser = new IniParser();
            parser.Settings.IsCaseSensitive = isCaseSensitive;
            var sections = parser.Parse(iniContent);
            sections.ForEach(s => AddMulti(s.Name, s, false));
        }

        /// <summary>
        /// Save the document to file.
        /// </summary>
        public override void Save()
        {
            Save(iniFilePath);
        }

        /// <summary>
        /// Save the document to the filepath specified.
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            var buffer = new StringBuilder();
            List<string> sections = Sections;
            foreach (var sectionName in sections)
            {
                var section = GetSection(sectionName) as IniSection;
                if (section != null)
                {
                    var sectionContent = section.ToString();
                    buffer.Append(sectionContent);
                }
                buffer.Append(Environment.NewLine);
            }
            string fullContent = buffer.ToString();
            File.WriteAllText(filePath, fullContent);
        }

        #endregion Methods
    }

    /// <summary>
    /// Parser.
    /// Terms:
    /// 1. Char - a single char whether it's space, doublequote, singlequote, etc.
    /// 2. Token - a collection of chars that make up a valid word/word-boundary.
    ///     e.g.
    ///     1. abc, 
    ///     2. -format:csv
    ///     3. -file:"c:/my files/file1.txt"
    ///     4. loc:'c:/my files/file1.txt'
    ///     5. -format:csv
    /// </summary>
    public class IniParser : IIniParser
    {
        #region Fields

        IniSection currentSection = new IniSection();

        //private IniLineType _lastLineType = IniLineType.None;
        private readonly List<string> _errors = new List<string>();
        private string inputText = string.Empty;
        private Scanner reader = null;
        private readonly List<IniSection> sections = new List<IniSection>();
        private IniParserSettings settings;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Create new instance with default settings
        /// </summary>
        public IniParser()
        {
            Settings = new IniParserSettings();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// List of errors.
        /// </summary>
        public IList<string> Errors
        {
            get { return _errors; }
        }

        /// <summary>
        /// The settings for the parser.
        /// </summary>
        public IniParserSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Parse the text and convert into list of params.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public List<IniSection> Parse(string text)
        {
            // Initialize for parsing.
            Init(text);

            // Validate the input text.
            if (!ValidateText().Success) return null;

            // Parse the text.
            Process();

            return sections;
        }

        /// <summary>
        /// Store the current group
        /// </summary>
        protected virtual void StoreGroup()
        {
            // Push the last one into the list.
            if (currentSection.Count > 0)
                sections.Add(currentSection);

            string group = reader.ReadToken(']', '\\', false, true, true, true);
            if (!settings.IsCaseSensitive)
                group = group.ToLower();

            // Create a new section using the name of the group.
            currentSection = new IniSection() { Name = group };
            //_lastLineType = IniLineType.Group;
        }

        /// <summary>
        /// Store the current key value.
        /// </summary>
        protected virtual void StoreKeyValue()
        {
            string key = reader.ReadToken(':', '\\', false, false, true, true);
            string val;

            reader.ConsumeWhiteSpace();
            // If starting with " then possibly multi-line.
            if (reader.CurrentChar == '"')
                val = reader.ReadToken('"', '\\', false, true, true, true);
            else
                val = reader.ReadToEol();

            if (!settings.IsCaseSensitive)
                key = key.ToLower();

            // This allow multiple values for the same key.
            // Multiple values are stored using List<object>.
            currentSection.AddMulti(key, val, false);
            //_lastLineType = IniLineType.KeyValue;
        }

        /// <summary>
        /// Initialize the reader and the stack.
        /// </summary>
        /// <param name="text"></param>
        private void Init(string text)
        {
            inputText = text;
            reader = new Scanner(text, new[] { '[', ']', ':' });
        }

        private void Process()
        {
            // Move to first char.
            reader.ReadChar();

            // Consume End Of Line.
            reader.ConsumeNewLines();
            bool readNextChar = true;
            // While not end of content.
            while (!reader.IsEnd())
            {
                // Get the current char.
                char currentChar = reader.CurrentChar;

                if (currentChar == '[')
                {
                    StoreGroup();
                    readNextChar = false;
                }
                else if (currentChar == ';')
                {
                    string comment = reader.ReadToEol();
                }
                else if (reader.IsEol())
                {
                    reader.ConsumeNewLines();
                    readNextChar = false;
                }
                else
                {
                    StoreKeyValue();
                    reader.ConsumeNewLines();
                    readNextChar = false;
                }

                // Read the next char.
                if (readNextChar) reader.ReadChar();
            }

            // Add the last section.
            // Handle null/emtpy sections.
            if (currentSection.Count > 0 && !string.IsNullOrEmpty(currentSection.Name))
                sections.Add(currentSection);
        }

        /// <summary>
        /// Confirm that the input text is valid text
        /// </summary>
        /// <returns></returns>
        private BoolMessage ValidateText()
        {
            if (string.IsNullOrEmpty(inputText))
                return new BoolMessage(false, "Empty text");

            return BoolMessage.True;
        }

        #endregion Methods
    }

    /// <summary>
    /// Settings for the parser.
    /// </summary>
    public class IniParserSettings
    {
        #region Fields

        /// <summary>
        /// Whether or not the groups/keys are case-sensitive
        /// </summary>
        public bool IsCaseSensitive = false;

        /// <summary>
        /// The maximum length of a comment.
        /// </summary>
        public int MaxLenghtOfComment = 500;

        /// <summary>
        /// The maximum length of a value in a single line.
        /// </summary>
        public int MaxLenghtOfValueSingleLine = 500;

        /// <summary>
        /// The maximum length of a group name [group]
        /// </summary>
        public int MaxLengthOfGroup = 40;

        /// <summary>
        /// The maximum length of a key key:.
        /// </summary>
        public int MaxLengthOfKey = 40;

        #endregion Fields
    }

    /// <summary>
    /// Class to represent an IniSection/Group and which also stores the entries
    /// associated under the section/group.
    /// e.g. 
    /// [group1]
    /// key1 = value1
    /// key2 = value2
    /// </summary>
    public class IniSection : ConfigSection
    {
        #region Methods

        /// <summary>
        /// Create shallow copy.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Return the ini format of the contents.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            string name = string.IsNullOrEmpty(Name) ? "section" : Name;

            buffer.Append("[" + name + "]" + Environment.NewLine);
            foreach (DictionaryEntry pair in this)
            {
                buffer.Append(pair.Key.ToString() + " : ");
                string val = pair.Value as string;
                if (!val.Contains("\""))
                {
                    buffer.Append(val);
                }
                else
                {
                    string encoded = val.Replace("\"", "\\\"");
                    buffer.Append(encoded);
                }
                buffer.Append(Environment.NewLine);
            }
            return buffer.ToString();
        }

        #endregion Methods
    }

    /// <summary>
    /// This class implements a token reader.
    /// </summary>
    public class Scanner
    {
        #region Fields

        private char END_CHAR = ' ';
        private int LAST_POSITION;
        private char _currentChar;
        private List<string> _errors = new List<string>();
        private char _escapeChar;
        private char _nextChar;
        private int _pos;
        private Dictionary<char, char> _readonlyWhiteSpaceMap;
        private string _text;
        private IDictionary<char, char> _tokens;
        private IDictionary<char, char> _whiteSpaceChars;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialize this instance with defaults.
        /// </summary>
        public Scanner()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initialize with text to parse.
        /// </summary>
        /// <param name="text"></param>
        public Scanner(string text)
        {
            Init(text, '\\', new char[] { '"', '\'' }, new char[] { ' ', '\t' });
        }

        /// <summary>
        /// Initialize with text to parse.
        /// </summary>
        /// <param name="text">The text to scan</param>
        /// <param name="reservedChars">Reserved chars</param>
        public Scanner(string text, char[] reservedChars)
        {
            Init(text, '\\', reservedChars, new char[] { ' ', '\t' });
        }

        /// <summary>
        /// Initialize this instance with supplied parameters.
        /// </summary>
        /// <param name="text">Text to use.</param>
        /// <param name="escapeChar">Escape character.</param>
        /// <param name="tokenChars">Special characters</param>
        /// <param name="whiteSpaceTokens">Array with whitespace tokens.</param>
        public Scanner(string text, char escapeChar, char[] tokenChars, char[] whiteSpaceTokens)
        {
            Init(text, escapeChar, tokenChars, whiteSpaceTokens);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Current char.
        /// </summary>
        /// <returns>Current character.</returns>
        public char CurrentChar
        {
            get { return _currentChar; }
        }

        /// <summary>
        /// The current position.
        /// </summary>
        public int Position
        {
            get { return _pos; }
        }

        /// <summary>
        /// Get the previous char that was read in.
        /// </summary>
        public char PreviousChar
        {
            get
            {
                // Check.
                if (_pos <= 0)
                    return char.MinValue;

                // Get the last char from the back buffer.
                // This is the last valid char that is not escaped.
                return _text[_pos - 1];
            }
        }

        /// <summary>
        /// Get the previous char that is part of the input and which may be an escape char.
        /// </summary>
        public string PreviousCharAny
        {
            get
            {
                // Check.
                if (_pos <= 0)
                    return string.Empty;

                // Get the last char from the back buffer.
                // This is the last valid char that is not escaped.
                return _text[_pos - 1].ToString();
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Check if all of the items in the collection satisfied by the condition.
        /// </summary>
        /// <typeparam name="T">Type of items.</typeparam>
        /// <param name="items">List of items.</param>
        /// <returns>Dictionary of items.</returns>
        public static IDictionary<T, T> ToDictionary<T>(IList<T> items)
        {
            IDictionary<T, T> dict = new Dictionary<T, T>();
            foreach (T item in items)
            {
                dict[item] = item;
            }
            return dict;
        }

        /// <summary>
        /// Advance and consume the current current char without storing 
        /// the char in the additional buffer for undo.
        /// </summary>
        public void ConsumeChar()
        {
            _pos++;
        }

        /// <summary>
        /// Consume the next <paramref name="count"/> chars without
        /// storing them in the additional buffer for undo.
        /// </summary>
        /// <param name="count">Number of characters to consume.</param>
        public void ConsumeChars(int count)
        {
            _pos += count;
        }

        /// <summary>
        /// Consume new line.
        /// </summary>
        public void ConsumeNewLine()
        {
            // Check
            if (_currentChar == '\r' && PeekChar() == '\n')
            {
                // Move to \n in \r\n
                ReadChar();
                ReadChar();
                return;
            }

            // Just \n
            if (_currentChar == '\n')
                ReadChar();
        }

        /// <summary>
        /// Consume New Lines.
        /// </summary>
        public void ConsumeNewLines()
        {
            string combinedNewLine = _currentChar.ToString() + PeekChar();
            while (_currentChar == '\n' || combinedNewLine == "\r\n" && _pos != LAST_POSITION)
            {
                ConsumeNewLine();
                combinedNewLine = _currentChar.ToString() + PeekChar();
            }
        }

        /// <summary>
        /// Read text up to the eol.
        /// </summary>
        /// <returns>String read.</returns>
        public void ConsumeToNewLine(bool includeNewLine = false)
        {
            // Read until ":" colon and while not end of string.
            while (!IsEol() && _pos <= LAST_POSITION)
            {
                MoveChars(1);
            }
            if (includeNewLine) ConsumeNewLine();
        }

        /// <summary>
        /// Consume until the chars found.
        /// </summary>
        /// <param name="pattern">The pattern to consume chars to.</param>
        /// <param name="includePatternInConsumption">Wether or not to consume the pattern as well.</param>
        /// <param name="movePastEndOfPattern">Whether or not to move to the ending position of the pattern</param>
        /// <param name="moveToStartOfPattern">Whether or not to move to the starting position of the pattern</param>
        public bool ConsumeUntil(string pattern, bool includePatternInConsumption, bool moveToStartOfPattern, bool movePastEndOfPattern)
        {
            int ndx = _text.IndexOf(pattern, _pos);
            if (ndx == -1) return false;
            int newCharPos = 0;

            if (!includePatternInConsumption)
                newCharPos = moveToStartOfPattern ? ndx : ndx - 1;
            else
                newCharPos = movePastEndOfPattern ? ndx + pattern.Length : (ndx + pattern.Length) - 1;

            MoveChars(newCharPos - _pos);
            return true;
        }

        /// <summary>
        /// Consume the whitespace without reading anything.
        /// </summary>
        public void ConsumeWhiteSpace()
        {
            ConsumeWhiteSpace(false);
        }

        /// <summary>
        /// Consume all white space.
        /// This works by checking the next char against
        /// the chars in the dictionary of chars supplied during initialization.
        /// </summary>
        /// <param name="readFirst">True to read a character
        /// before consuming the whitespace.</param>
        public void ConsumeWhiteSpace(bool readFirst)
        {
            char currentChar = readFirst ? ReadChar() : _currentChar;

            while (!IsEnd() && _whiteSpaceChars.ContainsKey(currentChar))
            {
                // Advance reader and next char.
                ReadChar();
                currentChar = _currentChar;
            }
        }

        /// <summary>
        /// Consume all white space.
        /// This works by checking the next char against
        /// the chars in the dictionary of chars supplied during initialization.
        /// </summary>
        /// <param name="readFirst">True to read a character
        /// before consuming the whitepsace.</param>
        /// <param name="setPosAfterWhiteSpace">True to move position to after whitespace</param>
        public void ConsumeWhiteSpace(bool readFirst, bool setPosAfterWhiteSpace = true)
        {
            if (readFirst) ReadChar();

            bool matched = false;
            while (_pos <= LAST_POSITION)
            {
                if (!_whiteSpaceChars.ContainsKey(_currentChar))
                {
                    matched = true;
                    break;
                }
                ReadChar();
            }

            // At this point the pos is already after token.
            // If matched and need to set at end of token, move back 1 char
            if (matched && !setPosAfterWhiteSpace) MoveChars(-1);
        }

        /// <summary>
        /// Current position in text.
        /// </summary>
        /// <returns>Current character index.</returns>
        public int CurrentCharIndex()
        {
            return _pos;
        }

        /// <summary>
        /// Determines whether the current character is the expected one.
        /// </summary>
        /// <param name="charToExpect">Character to expect.</param>
        /// <returns>True if the current character is the expected one.</returns>
        public bool Expect(char charToExpect)
        {
            bool isMatch = _currentChar == charToExpect;
            if (!isMatch)
                _errors.Add("Expected " + charToExpect + " at : " + _pos);
            return isMatch;
        }

        /// <summary>
        /// Initialize using settings.
        /// </summary>
        /// <param name="text">Text to use.</param>
        /// <param name="settings">Instance with token reader settings.</param>
        public void Init(string text, ScannerSettings settings)
        {
            Init(text, settings.EscapeChar, settings.Tokens, settings.WhiteSpaceTokens);
        }

        /// <summary>
        /// Initialize using the supplied parameters.
        /// </summary>
        /// <param name="text">Text to read.</param>
        /// <param name="escapeChar">Escape character.</param>
        /// <param name="tokens">Array with tokens.</param>
        /// <param name="whiteSpaceTokens">Array with whitespace tokens.</param>
        public void Init(string text, char escapeChar, char[] tokens, char[] whiteSpaceTokens)
        {
            Reset();
            _text = text;
            LAST_POSITION = _text.Length - 1;
            _escapeChar = escapeChar;
            _tokens = ToDictionary(tokens);
            _whiteSpaceChars = ToDictionary(whiteSpaceTokens);
            _readonlyWhiteSpaceMap = new Dictionary<char, char>(_whiteSpaceChars);
        }

        /// <summary>
        /// Determine if at last char.
        /// </summary>
        /// <returns>True if the last character is the current character.</returns>
        public bool IsAtEnd()
        {
            return _pos == _text.Length - 1;
        }

        /// <summary>
        /// Determine if the end of the text input has been reached.
        /// </summary>
        /// <returns>True if the end of the stream has been reached.</returns>
        public bool IsEnd()
        {
            return _pos >= _text.Length;
        }

        /// <summary>
        /// Determine if current char is EOL.
        /// </summary>
        /// <returns>True if the current character is an eol.</returns>
        public bool IsEol()
        {
            // Check for "\r\n"
            if (_currentChar == '\r' && PeekChar() == '\n')
                return true;

            return false;
        }

        /// <summary>
        /// Determine if current char is escape char.
        /// </summary>
        /// <returns>True if the current char is an escape char.</returns>
        public bool IsEscape()
        {
            return _currentChar == _escapeChar;
        }

        /// <summary>
        /// Determine if current char is token.
        /// </summary>
        /// <returns>True if the current char is a token.</returns>
        public bool IsToken()
        {
            return _tokens.ContainsKey(_currentChar);
        }

        /// <summary>
        /// Whether or not the current sequence of chars matches the token supplied.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ignoreCase">Whether or not to check against case.</param>
        /// <returns></returns>
        public bool IsToken(string token, bool ignoreCase = false)
        {
            return string.Compare(_text, _pos, token, 0, token.Length, ignoreCase) == 0;
        }

        /// <summary>
        /// Determine if current char is whitespace.
        /// </summary>
        /// <param name="whitespaceChars">Dictionary with whitespace chars.</param>
        /// <returns>True if the current character is a whitespace.</returns>
        public bool IsWhiteSpace(IDictionary whitespaceChars)
        {
            return whitespaceChars.Contains(_currentChar);
        }

        /// <summary>
        /// Determine if current char is whitespace.
        /// </summary>
        /// <returns>True if the current character is a whitespace.</returns>
        public bool IsWhiteSpace()
        {
            return this._whiteSpaceChars.ContainsKey(_currentChar);
        }

        /// <summary>
        /// Moves forward by count chars.
        /// </summary>
        /// <param name="count"></param>
        public void MoveChars(int count)
        {
            // Pos can never be more than 1 + last index position.
            // e.g. "common"
            // 1. length = 6
            // 2. LAST_POSITION = 5;
            // 3. _pos can not be more than 6. 6 indicating that it's past end
            // 4. _pos == 5 Indicating it's at end.
            if (_pos > LAST_POSITION && count > 0) return;

            // Move past end? Move it just 1 position more than last index.
            if (_pos + count > LAST_POSITION)
            {
                _pos = LAST_POSITION + 1;
                _currentChar = END_CHAR;
                return;
            }

            // Can move forward count chars
            _pos += count;
            _currentChar = _text[_pos];
        }

        /// <summary>
        /// Returns the char at current position + 1.
        /// </summary>
        /// <returns>Next char or string.empty if end of text.</returns>
        public char PeekChar()
        {
            // Validate.
            // a b c d e
            // 0 1 2 3 4
            // Lenght = 5
            // 4 >= 5 - 1
            if (_pos >= _text.Length - 1)
                return char.MinValue;

            _nextChar = _text[_pos + 1];
            return _nextChar;
        }

        /// <summary>
        /// Returns the nth char from the current char index
        /// </summary>
        /// <param name="countFromCurrentCharIndex">Number of characters from the current char index</param>
        /// <returns>Single char as string</returns>
        public char PeekChar(int countFromCurrentCharIndex)
        {
            // Validate.
            if (_pos + countFromCurrentCharIndex > _text.Length)
                return END_CHAR;

            return _text[_pos + countFromCurrentCharIndex];
        }

        /// <summary>
        /// Returns the chars starting at current position + 1 and
        /// including the <paramref name="count"/> number of characters.
        /// </summary>
        /// <param name="count">Number of characters.</param>
        /// <returns>Range of chars as string or string.empty if end of text.</returns>
        public string PeekChars(int count)
        {
            // Validate.
            if (_pos + count > _text.Length)
                return string.Empty;

            return _text.Substring(_pos + 1, count);
        }

        /// <summary>
        /// Peeks at the string all the way until the end of line.
        /// </summary>
        /// <returns>Current line.</returns>
        public string PeekLine()
        {
            int ndxEol = _text.IndexOf(Environment.NewLine, _pos + 1);
            if (ndxEol == -1)
                return _text.Substring(_pos + 1);

            return _text.Substring(_pos + 1, (ndxEol - _pos));
        }

        /// <summary>
        /// Read back the last char and reset
        /// </summary>
        public void ReadBackChar()
        {
            _pos--;
            //_backBuffer.Remove(_backBuffer.Length - 1, 1);
            _currentChar = _text[_pos];
        }

        /// <summary>
        /// Unwinds the reader by <paramref name="count"/> chars.
        /// </summary>
        /// <param name="count">Number of characters to read.</param>
        public void ReadBackChar(int count)
        {
            // Unwind
            _pos -= count;
            //_backBuffer.Remove(_backBuffer.Length - count, count);
            _currentChar = _text[_pos];
        }

        /// <summary>
        /// Read the next char.
        /// </summary>
        /// <returns>Character read.</returns>
        public char ReadChar()
        {
            // NEVER GO PAST 1 INDEX POSITION AFTER CHAR
            if (_pos > LAST_POSITION) return END_CHAR;

            _pos++;

            // Still valid?
            if (_pos <= LAST_POSITION)
            {
                _currentChar = _text[_pos];
                return _currentChar;
            }
            _currentChar = END_CHAR;
            return END_CHAR;
        }

        /// <summary>
        /// Read the next <paramref name="count"/> number of chars.
        /// </summary>
        /// <param name="count">Number of characters to read.</param>
        /// <returns>Characters read.</returns>
        public string ReadChars(int count)
        {
            string text = _text.Substring(_pos + 1, count);
            _pos += count;
            _currentChar = _text[_pos];
            return text;
        }

        /// <summary>
        /// Reads a word which must not have space in it and must have space/tab before and after
        /// </summary>
        /// <param name="continueReadCheck">Callback function to determine whether or not to continue reading</param>
        /// <param name="advanceFirst">Whether or not to advance position first</param>
        /// <param name="setPosAfterToken">True to move position to end space, otherwise past end space.</param>
        /// <returns>Contents of token read.</returns>
        public ScanTokenResult ReadChars(Func<bool> continueReadCheck, bool advanceFirst, bool setPosAfterToken = true)
        {
            // while for function
            var buffer = new StringBuilder();
            if (advanceFirst) ReadChar();

            bool matched = false;
            bool valid = true;
            while (_pos <= LAST_POSITION)
            {
                if (continueReadCheck())
                    buffer.Append(_currentChar);
                else
                {
                    matched = true;
                    valid = false;
                    break;
                }
                ReadChar();
                if (_pos < LAST_POSITION)
                    _nextChar = _text[_pos + 1];
            }
            // Either
            // 1. Matched the token
            // 2. Did not match but valid && end_of_file
            bool success = matched || (valid && _pos > LAST_POSITION);

            // At this point the pos is already after token.
            // If matched and need to set at end of token, move back 1 char
            if (success && !setPosAfterToken) MoveChars(-1);

            return new ScanTokenResult(success, buffer.ToString());
        }

        /// <summary>
        /// Reads a word which must not have space in it and must have space/tab before and after
        /// </summary>
        /// <param name="validChars">Dictionary to check against valid chars.</param>
        /// <param name="advanceFirst">Whether or not to advance position first</param>
        /// <param name="setPosAfterToken">True to move position to end space, otherwise past end space.</param>
        /// <returns>Contents of token read.</returns>
        public ScanTokenResult ReadChars(IDictionary<char, bool> validChars, bool advanceFirst, bool setPosAfterToken = true)
        {
            // while for function
            var buffer = new StringBuilder();
            if (advanceFirst) ReadChar();

            bool matched = false;
            bool valid = true;
            while (_pos <= LAST_POSITION)
            {
                if (validChars.ContainsKey(_currentChar))
                    buffer.Append(_currentChar);
                else
                {
                    matched = true;
                    valid = false;
                    break;
                }
                ReadChar();
                if (_pos < LAST_POSITION)
                    _nextChar = _text[_pos + 1];
            }

            // At this point the pos is already after token.
            // If matched and need to set at end of token, move back 1 char
            if (matched && !setPosAfterToken) MoveChars(-1);

            // Either
            // 1. Matched the token
            // 2. Did not match but valid && end_of_file
            bool success = matched || (valid && _pos > LAST_POSITION);
            return new ScanTokenResult(success, buffer.ToString());
        }

        /// <summary>
        /// Reads an identifier where legal chars for the identifier are [$ . _ a-z A-Z 0-9]
        /// </summary>
        /// <param name="advanceFirst"></param>
        /// <param name="setPosAfterToken">True to move position to after id, otherwise 2 chars past</param>
        /// <returns></returns>
        public ScanTokenResult ReadId(bool advanceFirst, bool setPosAfterToken = true)
        {
            return ReadChars(() =>
            {
                return ('a' <= _currentChar && _currentChar <= 'z')
                    || ('A' <= _currentChar && _currentChar <= 'Z')
                    || _currentChar == '$' || _currentChar == '_' || _currentChar == '.'
                    || ('0' <= _currentChar && _currentChar <= '9');
            }, advanceFirst, setPosAfterToken);
        }

        /// <summary>
        /// Reads entire line from curr position
        /// </summary>
        /// <param name="advanceFirst">Whether or not to advance curr position first </param>
        /// <param name="setPosAfterToken">Whether or not to move curr position to starting of new line or after</param>
        /// <returns>String read.</returns>
        public ScanTokenResult ReadLine(bool advanceFirst, bool setPosAfterToken = true)
        {
            var result = ReadChars(() => !(_currentChar == '\r' && _nextChar == '\n'), advanceFirst, setPosAfterToken);
            if (setPosAfterToken)
                MoveChars(2);
            return result;
        }

        /// <summary>
        /// Reads until the 2 chars are reached.
        /// </summary>
        /// <param name="advanceFirst">Whether or not to advance curr position first </param>
        /// <param name="first">The first char expected</param>
        /// <param name="second">The second char expected</param>
        /// <param name="moveToEndChar">Whether or not to advance to last end char ( second char ) or move past it</param>
        /// <returns>String read.</returns>
        public ScanTokenResult ReadLinesUntilChars(bool advanceFirst, char first, char second, bool moveToEndChar = true)
        {
            int lineCount = 0;
            return ReadChars(() =>
            {
                // Keep track of lines meet
                if (_currentChar == '\r' && _nextChar == '\n') lineCount++;

                return !(_currentChar == first && _nextChar == second);
            }, advanceFirst, moveToEndChar);
        }

        /// <summary>
        /// Reads a number +/-?[0-9]*.?[0-9]*
        /// </summary>
        /// <param name="advanceFirst">Whether or not to advance position first</param>
        /// <param name="setPosAfterToken">True to move position to end space, otherwise past end space.</param>
        /// <returns>Contents of token read.</returns>
        public ScanTokenResult ReadNumber(bool advanceFirst, bool setPosAfterToken = true)
        {
            return ReadChars(() =>
            {
                return ('0' <= _currentChar && _currentChar <= '9' || _currentChar == '.'
                            || _currentChar == '-' || _currentChar == '+');
            },
            advanceFirst, setPosAfterToken);
        }

        /// <summary>
        /// Read token until endchar
        /// </summary>
        /// <param name="quoteChar">char representing quote ' or "</param>
        /// <param name="escapeChar">Escape character for quote within string.</param>
        /// <param name="advanceFirst">True to advance position first before reading string.</param>
        /// <param name="setPosAfterToken">True to move position to end quote, otherwise past end quote.</param>
        /// <returns>Contents of token read.</returns>
        public ScanTokenResult ReadString(char quoteChar, char escapeChar = '\\', bool advanceFirst = true, bool setPosAfterToken = true)
        {
            // "name" 'name' "name\"s" 'name\'"
            var buffer = new StringBuilder();
            var curr = advanceFirst ? ReadChar() : _currentChar;
            var next = PeekChar();
            bool matched = false;
            while (_pos <= LAST_POSITION)
            {
                // Escape char
                if (curr == escapeChar)
                {
                    curr = ReadChar();
                    buffer.Append(curr);
                }
                else if (curr == quoteChar)
                {
                    matched = true;
                    MoveChars(1);
                    break;
                }
                else buffer.Append(curr);

                curr = ReadChar(); next = PeekChar();
            }
            // At this point the pos is already after token.
            // If matched and need to set at end of token, move back 1 char
            if (matched && !setPosAfterToken) MoveChars(-1);

            return new ScanTokenResult(matched, buffer.ToString());
        }

        /// <summary>
        /// Read text up to the eol.
        /// </summary>
        /// <returns>String read.</returns>
        public string ReadToEol()
        {
            StringBuilder buffer = new StringBuilder();

            // Read until ":" colon and while not end of string.
            while (!IsEol() && _pos <= LAST_POSITION)
            {
                buffer.Append(_currentChar);
                ReadChar();
            }
            return buffer.ToString();
        }

        /// <summary>
        /// Read a token.
        /// </summary>
        /// <param name="endChar">Ending character.</param>
        /// <param name="escapeChar">Escape character.</param>
        /// <param name="includeEndChar">True to include end character.</param>
        /// <param name="advanceFirst">True to advance before reading.</param>
        /// <param name="expectEndChar">True to expect an end charachter.</param>
        /// <param name="readPastEndChar">True to read past the end character.</param>
        /// <returns>Contens of token read.</returns>
        public string ReadToken(char endChar, char escapeChar, bool includeEndChar, bool advanceFirst, bool expectEndChar, bool readPastEndChar)
        {
            StringBuilder buffer = new StringBuilder();
            char currentChar = advanceFirst ? ReadChar() : _currentChar;

            // Read until ":" colon and while not end of string.
            while (currentChar != endChar && _pos <= LAST_POSITION)
            {
                // Escape char
                if (currentChar == escapeChar)
                {
                    currentChar = ReadChar();
                    buffer.Append(currentChar);
                }
                else
                    buffer.Append(currentChar);

                currentChar = ReadChar();
            }
            bool matchedEndChar = true;

            // Error out if current char is not ":".
            if (expectEndChar && _currentChar != endChar)
            {
                _errors.Add("Expected " + endChar + " at : " + _pos);
                matchedEndChar = false;
            }

            // Read past char.
            if (matchedEndChar && readPastEndChar)
                ReadChar();

            return buffer.ToString();
        }

        /// <summary>
        /// ReadToken until one of the endchars is found
        /// </summary>
        /// <param name="endChars">List of possible end chars which halts reading further.</param>
        /// <param name="includeEndChar">True to include end character.</param>
        /// <param name="advanceFirst">True to advance before reading.</param>
        /// <param name="readPastEndChar">True to read past the end character.</param>
        /// <returns></returns>
        public string ReadTokenUntil(char[] endChars, bool includeEndChar = false, bool advanceFirst = false, bool readPastEndChar = false)
        {
            var buffer = new StringBuilder();
            bool found = false;
            if (advanceFirst) ReadChar();

            while (_pos < LAST_POSITION && !found)
            {
                for (int ndx = 0; ndx < endChars.Length; ndx++)
                {
                    if (_currentChar == endChars[ndx])
                    {
                        found = true;
                        break;
                    }
                }
                if (!found || (found && includeEndChar))
                    buffer.Append(_currentChar);

                if (!found || (found && readPastEndChar))
                    ReadChar();
            }
            string token = buffer.ToString();
            return token;
        }

        /// <summary>
        /// Read token until endchar
        /// </summary>
        /// <param name="endChar">Ending character.</param>
        /// <param name="escapeChar">Escape character.</param>
        /// <param name="advanceFirst">True to advance before reading.</param>
        /// <param name="expectEndChar">True to expect an end charachter.</param>
        /// <param name="includeEndChar">True to include an end character.</param>
        /// <param name="moveToEndChar">True to move to the end character.</param>
        /// <param name="readPastEndChar">True to read past the end character.</param>
        /// <returns>Contents of token read.</returns>
        public string ReadTokenUntil(char endChar, char escapeChar, bool advanceFirst, bool expectEndChar, bool includeEndChar, bool moveToEndChar, bool readPastEndChar)
        {
            // abcd <div>
            var buffer = new StringBuilder();
            var currentChar = advanceFirst ? ReadChar() : _currentChar;
            var nextChar = PeekChar();
            while (nextChar != endChar && _pos <= LAST_POSITION)
            {
                // Escape char
                if (currentChar == escapeChar)
                {
                    currentChar = ReadChar();
                    buffer.Append(currentChar);
                }
                else
                    buffer.Append(currentChar);

                currentChar = ReadChar();
                nextChar = PeekChar();
            }
            bool matchedEndChar = nextChar == endChar;
            if (expectEndChar && !matchedEndChar)
                _errors.Add("Expected " + endChar + " at : " + _pos);

            if (matchedEndChar)
            {
                buffer.Append(currentChar);
                if (includeEndChar)
                    buffer.Append(nextChar);

                if (moveToEndChar)
                    ReadChar();

                else if (readPastEndChar && !IsAtEnd())
                    ReadChars(2);
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Reads until the 2 chars are reached.
        /// </summary>
        /// <param name="advanceFirst">Whether or not to advance curr position first </param>
        /// <param name="first">The first char expected</param>
        /// <param name="second">The second char expected</param>
        /// <param name="setPosAfterToken">Whether or not to advance to position after chars</param>
        /// <returns>String read.</returns>
        public ScanTokenResult ReadUntilChars(bool advanceFirst, char first, char second, bool setPosAfterToken = true)
        {
            var result = ReadChars(() => !(_currentChar == first && _nextChar == second), advanceFirst, setPosAfterToken);
            if (setPosAfterToken)
                MoveChars(2);
            return result;
        }

        /// <summary>
        /// Reads a word which must not have space in it and must have space/tab before and after
        /// </summary>
        /// <param name="advanceFirst">Whether or not to advance position first</param>
        /// <param name="setPosAfterToken">True to move position to end space, otherwise past end space.</param>
        /// <returns>Contents of token read.</returns>
        public ScanTokenResult ReadWord(bool advanceFirst, bool setPosAfterToken = true)
        {
            return ReadChars(() => _currentChar != ' ' && _currentChar != '\t', advanceFirst, setPosAfterToken);
        }

        /// <summary>
        /// Store the white space chars.
        /// </summary>
        /// <param name="whitespaceChars">Dictionary with whitespace characters.</param>
        public void RegisterWhiteSpace(IDictionary<char, char> whitespaceChars)
        {
            _whiteSpaceChars = whitespaceChars;
        }

        /// <summary>
        /// Reset reader for parsing again.
        /// </summary>
        public void Reset()
        {
            _pos = -1;
            _text = string.Empty;
            //_backBuffer = new StringBuilder();
            _whiteSpaceChars = new Dictionary<char, char>();
            _tokens = new Dictionary<char, char>();
            _escapeChar = '\\';
        }

        private void Init(IDictionary<string, bool> tokens, string[] tokenList)
        {
            foreach (string token in tokenList)
            {
                tokens[token] = true;
            }
        }

        #endregion Methods
    }

    /// <summary>
    /// Settings for the token reader class.
    /// </summary>
    public class ScannerSettings
    {
        #region Fields

        /// <summary>
        /// Char used to escape.
        /// </summary>
        public char EscapeChar;

        /// <summary>
        /// Tokens
        /// </summary>
        public char[] Tokens;

        /// <summary>
        /// White space tokens.
        /// </summary>
        public char[] WhiteSpaceTokens;

        #endregion Fields
    }

    /// <summary>
    /// The result of a scan for a specific token
    /// </summary>
    public class ScanTokenResult
    {
        #region Fields

        /// <summary>
        /// Whether or not the token was properly present
        /// </summary>
        public readonly bool Success;

        /// <summary>
        /// The text of the token.
        /// </summary>
        public readonly string Text;

        /// <summary>
        /// Number of lines parsed.
        /// </summary>
        public int Lines;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="success"></param>
        /// <param name="text"></param>
        public ScanTokenResult(bool success, string text)
        {
            Success = success;
            Text = text;
        }

        #endregion Constructors
    }

    /// <summary>
    /// Parser constants.
    /// </summary>
    class IniParserConstants
    {
        #region Fields

        public const string BracketLeft = "[";
        public const string BracketRight = "]";
        public const string Colon = ":";
        public const string CommentChar = ";";
        public const string DoubleQuote = "\"";
        public const string Escape = "\\";
        public const string SingleQuote = "'";
        public const string WhiteSpace = " ";

        public static string Eol = Environment.NewLine;

        #endregion Fields
    }
}