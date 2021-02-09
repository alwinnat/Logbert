#region Copyright © 2015 Couchcoding

// File:    LocationInfo.cs
// Package: Logbert
// Project: Logbert
// 
// The MIT License (MIT)
// 
// Copyright (c) 2015 Couchcoding
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

namespace Couchcoding.Logbert.Logging
{
  /// <summary>
  /// Implements a simple type to represent a Log4NEt location info.
  /// </summary>
  public struct LocationInfo
  {
    #region Public Properties

    /// <summary>
    /// Gets or sets the file name of the <see cref="LocationInfo"/> type.
    /// </summary>
    public string FileName
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets or sets the class name of the <see cref="LocationInfo"/> type.
    /// </summary>
    public string ClassName
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets or sets the method name of the <see cref="LocationInfo"/> type.
    /// </summary>
    public string MethodName
    {
      get;
      private set;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the fully qualified type name of this instance.
    /// </summary>
    /// <returns>A <see cref="T:System.String"/> containing a fully qualified type name.</returns>
    public override string ToString()
    {
      string locationString = !string.IsNullOrEmpty(FileName) ? "File: " + FileName : string.Empty;
      locationString += !string.IsNullOrEmpty(ClassName) ? "Class: " + ClassName : string.Empty;
      locationString += !string.IsNullOrEmpty(MethodName) ? "Method: " + MethodName : string.Empty;

      return locationString;
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new instance of a <see cref="LocationInfo"/> type.
    /// </summary>
    /// <param name="fileName">The file name of the <see cref="LocationInfo"/> type.</param>
    /// <param name="className">The class name of the <see cref="LocationInfo"/> type.</param>
    /// <param name="methodName">The method name of the <see cref="LocationInfo"/> type.</param>
    public LocationInfo(string fileName, string className, string methodName) : this()
    {
      FileName = fileName;
      ClassName = className;
      MethodName = methodName;
    }

    #endregion
  }
}
