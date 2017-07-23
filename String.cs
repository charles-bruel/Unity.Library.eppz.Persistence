//
// Copyright (c) 2017 Geri Borbás http://www.twitter.com/_eppz
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Text;
using System.IO.Compression;


namespace EPPZ.Persistence
{


	public static class String
	{


	#region Serializer

		/// <summary>
		/// An alias to `Serializer.GetFilePathWithExtension()`.
		/// </summary>
		/// <param name="serializer">Serializer to use.</param>
		public static string WithExtension(this string this_, Serializer serializer = null)
		{ return Serializer.SerializerOrDefault(serializer).GetFilePathWithExtension(this_); }

		/// <summary>
		/// An alias to `Serializer.StringToObject()`.
		/// </summary>
		public static T ToObject<T>(this string this_, Serializer serializer = null)
		{ return Serializer.SerializerOrDefault(serializer).StringToObject<T>(this_); }

	#endregion


	#region Bytes

		/// <summary>
		/// Get UTF8 bytes from string.
		/// </summary>
		public static byte[] Bytes(this string this_)
		{ return Encoding.UTF8.GetBytes(this_); }

		/// <summary>
		/// Get Base64 bytes from string.
		/// </summary>
		public static byte[] Base64Bytes(this string this_)
		{
			byte[] outputBytes = new byte[0];
			try
			{ outputBytes = Convert.FromBase64String(this_); }
			catch (Exception exception)
			{ Debug.LogWarning("String.Base64Bytes() exception: "+exception); }
			return outputBytes;
		}

	#endregion


	#region Zip

		public static string Zip(this string this_)
		{ return CompressBytes(this_.Bytes()).Base64String(); }

		public static string Unzip(this string this_)
		{ return DecompressBytes(this_.Base64Bytes()).String(); }

		/// <summary>
		/// A fallback for `Stream.CopyTo()` (only introduced in .NET 4).
		/// </summary>
		public static void _CopyTo(this Stream this_, Stream outputStream, int bufferSize = 4096)
        {
            byte[] bytes = new byte[bufferSize];
            int bytesRead;
            while ((bytesRead = this_.Read(bytes, 0, bytes.Length)) != 0)
            { outputStream.Write(bytes, 0, bytesRead); }
        }

		static byte[] CompressBytes(byte[] inputBytes)
		{
			if (inputBytes.Length == 0) return inputBytes; // Only if any

			using (MemoryStream inputStream = new MemoryStream(inputBytes))
				using (MemoryStream outputStream = new MemoryStream())
				{
					using (GZipStream zipStream = new GZipStream(outputStream, CompressionMode.Compress))
					{ inputStream._CopyTo(zipStream); }
					return outputStream.ToArray();
				}
		}

		static byte[] DecompressBytes(byte[] inputBytes)
		{
			if (inputBytes.Length == 0) return inputBytes; // Only if any

			using (MemoryStream inputStream = new MemoryStream(inputBytes))
    			using (MemoryStream outputStream = new MemoryStream())
				{
					using (GZipStream zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
					{ zipStream._CopyTo(outputStream); }
					return outputStream.ToArray();
				}
		}

	#endregion


	}
}