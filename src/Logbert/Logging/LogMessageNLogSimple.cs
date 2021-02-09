#region Copyright © 2021 alwinnat

// File:    LogMessageNLogSimple.cs
// Package: Logbert
// Project: Logbert
// 
// The MIT License (MIT)
// 
// Copyright (c) 2021 alwinnat
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using Couchcoding.Logbert.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Couchcoding.Logbert.Helper;

using MoonSharp.Interpreter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Couchcoding.Logbert.Logging
{
  /// <summary>
  /// Implements a <see cref="LogMessage"/> class for NLog simple (plain text) logger messages.
  /// </summary>
  public sealed class LogMessageNLogSimple : LogMessage
  {
    #region Private Fields

    /// <summary>
    /// Regular expression to parse an NLog text line in a specific format.
    /// </summary>
    private static readonly Regex mParserRegex = new Regex(
          "(?<timestamp>[0-9]{4}\\-[0-1][0-9]\\-[0-3][0-9] [0-2][0-9]:[0-5][0-9]:[0-5][0-9]\\.[0-9]+) " +
          "(?<level>[a-z]+) Thrd:(?<thread>[0-9]+) (?<logger>.*) -> (?<message>.*)",
          RegexOptions.IgnoreCase);

    /// <summary>
    /// Holds the <see cref="DateTime"/> the <see cref="LogMessage"/> is received. 
    /// </summary>
    private DateTime mTimestamp;

    /// <summary>
    /// Holds the message of the <see cref="LogMessage"/>.
    /// </summary>
    private string mMessage = string.Empty;

    /// <summary>
    /// Holds the <see cref="LogLevel"/> of the <see cref="LogMessage"/>.
    /// </summary>
    private LogLevel mLevel;

    /// <summary>
    /// Holds the thread name of the <see cref="LogMessage"/>.
    /// </summary>
    private string mThread;

    /// <summary>
    /// Holds the <see cref="LocationInfo"/> of the <see cref="LogMessage"/>.
    /// </summary>
    private LocationInfo mLocation;

    /// <summary>
    /// Holds a <see cref="Dictionary{V, T}"/> of custom properties.
    /// </summary>
    private readonly Dictionary<string, string> mCustomProperties = new Dictionary<string, string>();

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the <see cref="DateTime"/> the <see cref="LogMessage"/> is received.
    /// </summary>
    public override DateTime Timestamp
    {
      get
      {
        return mTimestamp;
      }
    }

    /// <summary>
    /// Gets the message of the <see cref="LogMessage"/>.
    /// </summary>
    public override string Message
    {
      get
      {
        return mMessage;
      }
    }

    /// <summary>
    /// Gets the <see cref="LogLevel"/> of the <see cref="LogMessage"/>.
    /// </summary>
    public override LogLevel Level
    {
      get
      {
        return mLevel;
      }
    }

    /// <summary>
    /// Gets the thread name of the <see cref="LogMessage"/>.
    /// </summary>
    public string Thread
    {
      get
      {
        return mThread;
      }
    }

    /// <summary>
    /// Gets the <see cref="LocationInfo"/> of the <see cref="LogMessage"/>.
    /// </summary>
    public LocationInfo Location
    {
      get
      {
        return mLocation;
      }
    }

    /// <summary>
    /// Gets a <see cref="Dictionary{V, T}"/> of custom properties.
    /// </summary>
    public Dictionary<string, string> CustomProperties
    {
      get
      {
        return mCustomProperties;
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Parses the given <paramref name="data"/> for possible log message parts.
    /// </summary>
    /// <param name="data">The data string to parse.</param>
    /// <returns><c>True</c> on success, otherwise <c>false</c>.</returns>
    private bool ParseData(string data)
    {
      if (!string.IsNullOrEmpty(data))
      {
        var match = mParserRegex.Match(data);

        if (match.Success)
        {
          mTimestamp = DateTime.Parse(match.Groups["timestamp"].Value);
          mLevel = MapLevelType(match.Groups["level"].Value);
          mLogger = match.Groups["logger"].Value.Replace("..", ".").Trim();
          mThread = match.Groups["thread"].Value;
          mMessage = match.Groups["message"].Value;
          return true;
        }
      }
      return false;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the value to display within a <see cref="DataGridView"/> at the fiven <paramref name="columnIndex"/>.
    /// </summary>
    /// <param name="columnIndex">The index of the column at the <see cref="DataGridView"/>.</param>
    /// <returns>The value to display at the given <paramref name="columnIndex"/>, or <c>null</c> if nothing to display.</returns>
    public override object GetValueForColumn(int columnIndex)
    {
      switch (columnIndex)
      {
        case 1:
          return mIndex;
        case 2:
          return mLevel.ToString();
        case 3:
          return mTimestamp.Add(mTimeShiftOffset).ToString(
              Settings.Default.TimestampFormat
            , CultureInfo.InvariantCulture);
        case 4:
          return mLogger;
        case 5:
          return mThread;
        case 6:
          return mMessage;
      }

      return null;
    }

    /// <summary>
    /// Exports the <see cref="LogMessage"/> with its data into a comma seperated line.
    /// </summary>
    /// <returns>The <see cref="LogMessage"/> with its data as a comma seperated line.</returns>
    public override string GetCsvLine()
    {
      string messageValue = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\""
        , mIndex.ToCsv()
        , mLevel.ToCsv()
        , mTimestamp.ToString(Settings.Default.TimestampFormat)
        , mLogger.ToCsv()
        , mThread.ToCsv()
        , mMessage.ToCsv()
        , mLocation.ToString().ToCsv());

      foreach (KeyValuePair<string, string> customProperty in mCustomProperties)
      {
        messageValue += string.Format("{0}: {1},"
          , customProperty.Key.ToCsv()
          , customProperty.Value.ToCsv());
      }

      if (messageValue.Length > 0)
      {
        // Remove the last comma from the custom properties string.
        messageValue = messageValue.Remove(
            messageValue.Length - 1
          , 1);
      }

      return messageValue + "\"";
    }

    /// <summary>
    /// Returns a <see cref="Table"/> that represents the current <see cref="LogMessage"/>.
    /// </summary>
    /// <param name="owner">The owner <see cref="Script"/> instance this <see cref="Table"/> is for.</param>
    /// <returns>A new <see cref="Table"/> object that represents the current <see cref="LogMessage"/>, or <c>null</c> on error.</returns>
    public override Table ToLuaTable(Script owner)
    {
      Table msgData = base.ToLuaTable(owner);

      if (msgData == null)
      {
        return null;
      }

      msgData["Thread"] = Thread;
      msgData["Location"] = Location.ToString();

      Table customProperties = new Table(owner);

      foreach (KeyValuePair<string, string> customProperty in mCustomProperties)
      {
        customProperties[customProperty.Key] = customProperty.Value;
      }

      msgData["CustomProperties"] = customProperties;

      return msgData;
    }

    /// <summary>
    /// Attach text to <see cref="Message"/> from a multiline NLog (for instance exceptions).
    /// </summary>
    /// <param name="message">Text to attach <see cref="Message"/>.</param>
    public void AppendMessage(string message)
    {
      mMessage += message + Environment.NewLine;
    }

    /// <summary>
    /// Try to parse a data string into a <see cref="LogMessageNLogSimple"/> object.
    /// </summary>
    /// <param name="data">The data string to parse.</param>
    /// <param name="index">The index of the <see cref="LogMessage"/>.</param>
    /// <param name="message">Parsed <see cref="LogMessageNLogSimple"/> object.</param>
    /// <returns><c>True</c> on success, otherwise <c>false</c>.</returns>
    public static bool TryParse(string data, int index, out LogMessageNLogSimple message)
    {
      if (!string.IsNullOrEmpty(data))
      {
        var match = mParserRegex.Match(data);

        if (match.Success)
        {
          message = new LogMessageNLogSimple(index)
          {
            mTimestamp = DateTime.Parse(match.Groups["timestamp"].Value),
            mLevel = MapLevelType(match.Groups["level"].Value),
            mLogger = match.Groups["logger"].Value.Replace("..", ".").Trim(),
            mThread = match.Groups["thread"].Value,
            mMessage = match.Groups["message"].Value
          };
          return true;
        }
      }
      message = null;
      return false;
    }

    #endregion

    #region Constructor

    public LogMessageNLogSimple(int index) : base(null, index)
    {

    }

    /// <summary>
    /// Creates a new instance of the <see cref="LogMessageNLogSimple"/> object.
    /// </summary>
    /// <param name="rawData">The data <see cref="Array"/> the new <see cref="LogMessageNLogSimple"/> represents.</param>
    /// <param name="index">The index of the <see cref="LogMessage"/>.</param>
    public LogMessageNLogSimple(string rawData, int index) : base(rawData, index)
    {
      if (!ParseData(rawData))
      {
        throw new ApplicationException("Unable to parse the logger data.");
      }
    }

    #endregion
  }
}
