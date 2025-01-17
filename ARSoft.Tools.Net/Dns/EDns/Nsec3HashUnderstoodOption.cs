﻿#region Copyright and License
// Copyright 2010..2022 Alexander Reinert
// 
// This file is part of the ARSoft.Tools.Net - C# DNS client/server and SPF Library (https://github.com/alexreinert/ARSoft.Tools.Net)
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARSoft.Tools.Net.Dns
{
	/// <summary>
	///   <para>NSEC3 Hash Unterstood option</para>
	///   <para>
	///     Defined in
	///     <a href="https://www.rfc-editor.org/rfc/rfc6975.html">RFC 6975</a>.
	///   </para>
	/// </summary>
	public class Nsec3HashUnderstoodOption : EDnsOptionBase
	{
		/// <summary>
		///   List of Algorithms
		/// </summary>
		public List<NSec3HashAlgorithm> Algorithms { get; private set; }

		internal Nsec3HashUnderstoodOption(IList<byte> resultData, int startPosition, int length)
			: base(EDnsOptionType.Nsec3HashUnderstood)
		{
			Algorithms = new List<NSec3HashAlgorithm>(length);
			for (int i = 0; i < length; i++)
			{
				Algorithms.Add((NSec3HashAlgorithm) resultData[startPosition++]);
			}
		}

		/// <summary>
		///   Creates a new instance of the Nsec3HashUnderstoodOption class
		/// </summary>
		/// <param name="algorithms">The list of algorithms</param>
		public Nsec3HashUnderstoodOption(params NSec3HashAlgorithm[] algorithms)
			: base(EDnsOptionType.Nsec3HashUnderstood)
		{
			Algorithms = algorithms.ToList();
		}

		internal override ushort DataLength => (ushort) (Algorithms?.Count ?? 0);

		internal override void EncodeData(IList<byte> messageData, ref int currentPosition)
		{
			foreach (var algorithm in Algorithms)
			{
				messageData[currentPosition++] = (byte) algorithm;
			}
		}
	}
}