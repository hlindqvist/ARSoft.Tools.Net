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
	///   <para>Host information</para>
	///   <para>
	///     Defined in
	///     <a href="https://www.rfc-editor.org/rfc/rfc1035.html">RFC 1035</a>.
	///   </para>
	/// </summary>
	public class HInfoRecord : DnsRecordBase
	{
		/// <summary>
		///   Type of the CPU of the host
		/// </summary>
		public string Cpu { get; private set; }

		/// <summary>
		///   Name of the operating system of the host
		/// </summary>
		public string OperatingSystem { get; private set; }

		internal HInfoRecord(DomainName name, RecordType recordType, RecordClass recordClass, int timeToLive, IList<byte> resultData, int currentPosition, int length)
			: base(name, recordType, recordClass, timeToLive)
		{
			Cpu = DnsMessageBase.ParseText(resultData, ref currentPosition);
			OperatingSystem = DnsMessageBase.ParseText(resultData, ref currentPosition);
		}

		internal HInfoRecord(DomainName name, RecordType recordType, RecordClass recordClass, int timeToLive, DomainName origin, string[] stringRepresentation)
			: base(name, recordType, recordClass, timeToLive)
		{
			if (stringRepresentation.Length != 2)
				throw new FormatException();

			Cpu = stringRepresentation[0];
			OperatingSystem = stringRepresentation[1];
		}

		/// <summary>
		///   Creates a new instance of the HInfoRecord class
		/// </summary>
		/// <param name="name"> Name of the host </param>
		/// <param name="timeToLive"> Seconds the record should be cached at most </param>
		/// <param name="cpu"> Type of the CPU of the host </param>
		/// <param name="operatingSystem"> Name of the operating system of the host </param>
		public HInfoRecord(DomainName name, int timeToLive, string cpu, string operatingSystem)
			: base(name, RecordType.HInfo, RecordClass.INet, timeToLive)
		{
			Cpu = cpu ?? String.Empty;
			OperatingSystem = operatingSystem ?? String.Empty;
		}

		internal override string RecordDataToString()
		{
			return "\"" + Cpu.ToMasterfileLabelRepresentation() + "\""
			       + " \"" + OperatingSystem.ToMasterfileLabelRepresentation() + "\"";
		}

		protected internal override int MaximumRecordDataLength => 2 + Cpu.Length + OperatingSystem.Length;

		protected internal override void EncodeRecordData(IList<byte> messageData, ref int currentPosition, Dictionary<DomainName, ushort>? domainNames, bool useCanonical)
		{
			DnsMessageBase.EncodeTextBlock(messageData, ref currentPosition, Cpu);
			DnsMessageBase.EncodeTextBlock(messageData, ref currentPosition, OperatingSystem);
		}
	}
}