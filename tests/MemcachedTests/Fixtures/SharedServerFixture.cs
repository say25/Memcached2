﻿using System;
using System.Diagnostics;
using System.Net;

namespace Enyim.Caching.Memcached
{
	public class SharedServerFixture : IServerFixture
	{
		private static readonly object serverLock = new Object();
		private static MemcachedServerGroup sharedServers;
		private static IPEndPoint[] endpoints;

		private static int sharedRefCount;

		public IPEndPoint[] Run()
		{
			lock (serverLock)
			{
				if (sharedServers == null)
				{
					sharedServers = new MemcachedServerGroup(3);
					endpoints = sharedServers.Run();
				}

				sharedRefCount++;
			}

			return endpoints;
		}

		void IDisposable.Dispose()
		{
			lock (serverLock)
			{
				Debug.Assert(sharedRefCount > 0);
				Debug.Assert(sharedServers != null);

				sharedRefCount--;

				if (sharedRefCount == 0)
				{
					sharedServers.Dispose();
					sharedServers = null;
				}
			}
		}
	}
}

#region [ License information          ]

/*

Copyright (c) Attila Kiskó, enyim.com

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

*/

#endregion