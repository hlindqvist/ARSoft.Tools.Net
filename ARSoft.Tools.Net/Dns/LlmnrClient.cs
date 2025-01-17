#region Copyright and License
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
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace ARSoft.Tools.Net.Dns
{
	/// <summary>
	///   Provides a client for querying LLMNR (link-local multicast name resolution) as defined in
	///   <a href="https://www.rfc-editor.org/rfc/rfc4795.html">RFC 4795</a>.
	/// </summary>
	public sealed class LlmnrClient : DnsClientBase
	{
		public const int DEFAULT_PORT = 5355;

		private static readonly List<IPAddress> _addresses = new List<IPAddress> { IPAddress.Parse("FF02::1:3"), IPAddress.Parse("224.0.0.252") };

		/// <summary>
		///   Provides a new instance with a timeout of 1 second
		/// </summary>
		public LlmnrClient()
			: this(1000) { }

		/// <summary>
		///   Provides a new instance with a custom timeout
		/// </summary>
		/// <param name="queryTimeout"> Query timeout in milliseconds </param>
		public LlmnrClient(int queryTimeout)
			: base(_addresses, queryTimeout, new IClientTransport[] { new MulticastClientTransport(DEFAULT_PORT), new TcpClientTransport(DEFAULT_PORT) }, true) { }

		/// <summary>
		///   Queries for specified records.
		/// </summary>
		/// <param name="name"> Name, that should be queried </param>
		/// <param name="recordType"> Type the should be queried </param>
		/// <returns> All available responses on the local network </returns>
		public List<LlmnrMessage> Resolve(DomainName name, RecordType recordType = RecordType.A)
		{
			_ = name ?? throw new ArgumentNullException(nameof(name), "Name must be provided");

			LlmnrMessage message = new LlmnrMessage { IsQuery = true, OperationCode = OperationCode.Query };
			message.Questions.Add(new DnsQuestion(name, recordType, RecordClass.INet));

			return SendMessageParallel(message);
		}

		/// <summary>
		///   Queries for specified records as an asynchronous operation.
		/// </summary>
		/// <param name="name"> Name, that should be queried </param>
		/// <param name="recordType"> Type the should be queried </param>
		/// <param name="token"> The token to monitor cancellation requests </param>
		/// <returns> All available responses on the local network </returns>
		public Task<List<LlmnrMessage>> ResolveAsync(DomainName name, RecordType recordType = RecordType.A, CancellationToken token = default(CancellationToken))
		{
			_ = name ?? throw new ArgumentNullException(nameof(name), "Name must be provided");

			LlmnrMessage message = new LlmnrMessage { IsQuery = true, OperationCode = OperationCode.Query };
			message.Questions.Add(new DnsQuestion(name, recordType, RecordClass.INet));

			return SendMessageParallelAsync(message, token);
		}
	}
}