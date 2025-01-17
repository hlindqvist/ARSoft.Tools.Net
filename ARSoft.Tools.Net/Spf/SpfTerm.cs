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
using System.Text.RegularExpressions;

namespace ARSoft.Tools.Net.Spf
{
	/// <summary>
	///   Represents a single term of a SPF record
	/// </summary>
	public abstract class SpfTerm
	{
		private static readonly Regex _parseMechanismRegex = new Regex(@"^(\s)*(?<qualifier>[~+?-]?)(?<type>[a-z0-9]+)(:(?<domain>[^/]+))?(/(?<prefix>[0-9]+)(/(?<prefix6>[0-9]+))?)?(\s)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static readonly Regex _parseModifierRegex = new Regex(@"^(\s)*(?<type>[a-z]+)=(?<domain>[^\s]+)(\s)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		internal static bool TryParse(string s, out SpfTerm? value)
		{
			if (String.IsNullOrEmpty(s))
			{
				value = null;
				return false;
			}

			#region Parse Mechanism
			Match match = _parseMechanismRegex.Match(s);
			if (match.Success)
			{
				SpfQualifier qualifier;
				switch (match.Groups["qualifier"].Value)
				{
					case "+":
						qualifier = SpfQualifier.Pass;
						break;
					case "-":
						qualifier = SpfQualifier.Fail;
						break;
					case "~":
						qualifier = SpfQualifier.SoftFail;
						break;
					case "?":
						qualifier = SpfQualifier.Neutral;
						break;

					default:
						qualifier = SpfQualifier.Pass;
						break;
				}

				SpfMechanismType type = EnumHelper<SpfMechanismType>.TryParse(match.Groups["type"].Value, true, out SpfMechanismType t) ? t : SpfMechanismType.Unknown;
				string? domain = match.Groups["domain"].Value;

				string tmpPrefix = match.Groups["prefix"].Value;
				int? prefix = null;
				if (!String.IsNullOrEmpty(tmpPrefix) && Int32.TryParse(tmpPrefix, out int p))
				{
					prefix = p;
				}

				tmpPrefix = match.Groups["prefix6"].Value;
				int? prefix6 = null;
				if (!String.IsNullOrEmpty(tmpPrefix) && Int32.TryParse(tmpPrefix, out int p6))
				{
					prefix6 = p6;
				}

				value = new SpfMechanism(qualifier, type, domain, prefix, prefix6);
				return true;
			}
			#endregion

			#region Parse Modifier
			match = _parseModifierRegex.Match(s);
			if (match.Success)
			{
				value = new SpfModifier(
					EnumHelper<SpfModifierType>.TryParse(match.Groups["type"].Value, true, out SpfModifierType t) ? t : SpfModifierType.Unknown,
					match.Groups["domain"].Value);
				return true;
			}
			#endregion

			value = null;
			return false;
		}
	}
}