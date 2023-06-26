using FreeSBAIO.Models.Tasks;
using FreeSBAIO.Networking.Common;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FreeSBAIO.Tasks.Adidas
{
	internal class AdidasSession : Session
	{
		private AdidasTask _task;

		public AdidasSession(AdidasTask task) : base(task)
		{
			this._task = task;
		}

		public async Task<Response> GetApi(string url, string referer = null)
		{
			OrderedDictionary orderedDictionaries = new OrderedDictionary()
			{
				{ "content-type", "application/json" },
				{ "user-agent", this._task.UserAgent },
				{ "accept", "*/*" },
				{ "sec-fetch-site", "same-origin" },
				{ "sec-fetch-mode", "cors" },
				{ "sec-fetch-dest", "empty" },
				{ "accept-encoding", "gzip, deflate, br" },
				{ "accept-language", "en-GB,en-US;q=0.9,en;q=0.8" }
			};
			OrderedDictionary orderedDictionaries1 = orderedDictionaries;
			if (!string.IsNullOrEmpty(referer))
			{
				orderedDictionaries1.Insert(6, "referer", referer);
			}
			Response response = await base.Client.MakeRequest(url, "GET", orderedDictionaries1, null, true, false, false);
			return response;
		}

		public async Task<Response> GetOppwa(string url)
		{
			OrderedDictionary orderedDictionaries = new OrderedDictionary()
			{
				{ "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
				{ "Referer", string.Format("{0}/payment", this._task.HomeUrl) },
				{ "Accept-Encoding", "gzip, deflate, br" },
				{ "Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8" }
			};
			OrderedDictionary orderedDictionaries1 = orderedDictionaries;
			Response response = await base.Client.MakeRequest(url, "GET", orderedDictionaries1, null, true, true, false);
			return response;
		}

		public async Task<Response> GetPaymentMethods(string url, string authorization)
		{
			object obj;
			OrderedDictionary orderedDictionaries = new OrderedDictionary();
			obj = (string.IsNullOrEmpty(authorization) ? "null" : authorization);
			orderedDictionaries.Add("checkout-authorization", obj);
			orderedDictionaries.Add("content-type", "application/json");
			orderedDictionaries.Add("user-agent", this._task.UserAgent);
			orderedDictionaries.Add("accept", "*/*");
			orderedDictionaries.Add("sec-fetch-site", "same-origin");
			orderedDictionaries.Add("sec-fetch-mode", "cors");
			orderedDictionaries.Add("sec-fetch-dest", "empty");
			orderedDictionaries.Add("referer", string.Format("{0}/payment", this._task.HomeUrl));
			orderedDictionaries.Add("accept-encoding", "gzip, deflate, br");
			orderedDictionaries.Add("accept-language", "en-GB,en-US;q=0.9,en;q=0.8");
			OrderedDictionary orderedDictionaries1 = orderedDictionaries;
			Response response = await base.Client.MakeRequest(url, "GET", orderedDictionaries1, null, true, true, false);
			return response;
		}

		public async Task<Response> GetProductPage(string url)
		{
			OrderedDictionary orderedDictionaries = new OrderedDictionary()
			{
				{ "upgrade-insecure-requests", "1" },
				{ "user-agent", this._task.UserAgent },
				{ "accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
				{ "sec-fetch-site", "none" },
				{ "sec-fetch-mode", "navigate" },
				{ "sec-fetch-user", "?1" },
				{ "sec-fetch-dest", "document" },
				{ "accept-encoding", "gzip, deflate, br" },
				{ "accept-language", "en-GB,en-US;q=0.9,en;q=0.8" }
			};
			OrderedDictionary orderedDictionaries1 = orderedDictionaries;
			Response response = await base.Client.MakeRequest(url, "GET", orderedDictionaries1, null, true, true, false);
			return response;
		}

		public async Task<Response> GetQueue(string url, string referer)
		{
			OrderedDictionary orderedDictionaries = new OrderedDictionary()
			{
				{ "user-agent", this._task.UserAgent },
				{ "accept", "application/json, text/plain, */*" },
				{ "sec-fetch-site", "same-origin" },
				{ "sec-fetch-mode", "cors" },
				{ "sec-fetch-dest", "empty" },
				{ "referer", referer },
				{ "accept-encoding", "gzip, deflate, br" },
				{ "accept-language", "en-GB,en-US;q=0.9,en;q=0.8" }
			};
			OrderedDictionary orderedDictionaries1 = orderedDictionaries;
			Response response = await base.Client.MakeRequest(url, "GET", orderedDictionaries1, null, true, true, false);
			return response;
		}

		public async Task<Response> GetShippingMethods(string url, string authorization)
		{
			object obj;
			OrderedDictionary orderedDictionaries = new OrderedDictionary();
			obj = (string.IsNullOrEmpty(authorization) ? "null" : authorization);
			orderedDictionaries.Add("checkout-authorization", obj);
			orderedDictionaries.Add("content-type", "application/json");
			orderedDictionaries.Add("user-agent", this._task.UserAgent);
			orderedDictionaries.Add("accept", "*/*");
			orderedDictionaries.Add("sec-fetch-site", "same-origin");
			orderedDictionaries.Add("sec-fetch-mode", "cors");
			orderedDictionaries.Add("sec-fetch-dest", "empty");
			orderedDictionaries.Add("referer", string.Format("{0}/delivery", this._task.HomeUrl));
			orderedDictionaries.Add("accept-encoding", "gzip, deflate, br");
			orderedDictionaries.Add("accept-language", "en-GB,en-US;q=0.9,en;q=0.8");
			OrderedDictionary orderedDictionaries1 = orderedDictionaries;
			Response response = await base.Client.MakeRequest(url, "GET", orderedDictionaries1, null, true, true, false);
			return response;
		}

		public async Task<Response> PatchDetails(string url, string data, string authorization)
		{
			object obj;
			OrderedDictionary orderedDictionaries = new OrderedDictionary()
			{
				{ "content-length", "x" }
			};
			obj = (string.IsNullOrEmpty(authorization) ? "null" : authorization);
			orderedDictionaries.Add("checkout-authorization", obj);
			orderedDictionaries.Add("content-type", "application/json");
			orderedDictionaries.Add("user-agent", this._task.UserAgent);
			orderedDictionaries.Add("accept", "*/*");
			orderedDictionaries.Add("origin", this._task.HomeUrl);
			orderedDictionaries.Add("sec-fetch-site", "same-origin");
			orderedDictionaries.Add("sec-fetch-mode", "cors");
			orderedDictionaries.Add("sec-fetch-dest", "empty");
			orderedDictionaries.Add("referer", string.Format("{0}/delivery", this._task.HomeUrl));
			orderedDictionaries.Add("accept-encoding", "gzip, deflate, br");
			orderedDictionaries.Add("accept-language", "en-GB,en-US;q=0.9,en;q=0.8");
			OrderedDictionary orderedDictionaries1 = orderedDictionaries;
			Response response = await base.Client.MakeRequest(url, "PATCH", orderedDictionaries1, data, true, true, false);
			return response;
		}

		public async Task<Response> PostAuth(string url, string data, string origin, string referer = null)
		{
			OrderedDictionary orderedDictionaries = new OrderedDictionary()
			{
				{ "cache-control", "max-age=0" },
				{ "upgrade-insecure-requests", "1" },
				{ "origin", this._task.HomeUrl },
				{ "content-type", "application/x-www-form-urlencoded" },
				{ "accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
				{ "accept-encoding", "gzip, deflate, br" },
				{ "accept-language", "en-GB,en-US;q=0.9,en;q=0.8" }
			};
			OrderedDictionary orderedDictionaries1 = orderedDictionaries;
			if (!string.IsNullOrEmpty(referer))
			{
				orderedDictionaries1.Insert(5, "referer", referer);
			}
			Response response = await base.Client.MakeRequest(url, "POST", orderedDictionaries1, data, true, true, false);
			return response;
		}

		public async Task<Response> PostCardinalAuth(string url, string data, string origin, string referer)
		{
			OrderedDictionary orderedDictionaries = new OrderedDictionary()
			{
				{ "Cache-Control", "max-age=0" },
				{ "Upgrade-Insecure-Requests", "1" },
				{ "Origin", this._task.HomeUrl },
				{ "Content-Type", "application/x-www-form-urlencoded" },
				{ "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
				{ "Referer", referer },
				{ "Accept-Encoding", "gzip, deflate, br" },
				{ "Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8" }
			};
			OrderedDictionary orderedDictionaries1 = orderedDictionaries;
			Response response = await base.Client.MakeRequest(url, "POST", orderedDictionaries1, data, true, true, false);
			return response;
		}

		public async Task<Response> PostCart(string url, string data, string referer, string authorization = null)
		{
			OrderedDictionary orderedDictionaries;
			object obj;
			if (!url.Contains("/baskets/"))
			{
				OrderedDictionary orderedDictionaries1 = new OrderedDictionary()
				{
					{ "content-length", "x" },
					{ "content-type", "application/json" },
					{ "user-agent", this._task.UserAgent },
					{ "accept", "*/*" },
					{ "origin", this._task.HomeUrl },
					{ "sec-fetch-site", "same-origin" },
					{ "sec-fetch-mode", "cors" },
					{ "sec-fetch-dest", "empty" },
					{ "referer", referer },
					{ "accept-encoding", "gzip, deflate, br" },
					{ "accept-language", "en-GB,en-US;q=0.9,en;q=0.8" }
				};
				orderedDictionaries = orderedDictionaries1;
			}
			else
			{
				OrderedDictionary orderedDictionaries2 = new OrderedDictionary()
				{
					{ "content-length", "x" }
				};
				obj = (string.IsNullOrEmpty(authorization) ? "null" : authorization);
				orderedDictionaries2.Add("checkout-authorization", obj);
				orderedDictionaries2.Add("content-type", "application/json");
				orderedDictionaries2.Add("user-agent", this._task.UserAgent);
				orderedDictionaries2.Add("accept", "*/*");
				orderedDictionaries2.Add("origin", this._task.HomeUrl);
				orderedDictionaries2.Add("sec-fetch-site", "same-origin");
				orderedDictionaries2.Add("sec-fetch-mode", "cors");
				orderedDictionaries2.Add("sec-fetch-dest", "empty");
				orderedDictionaries2.Add("referer", referer);
				orderedDictionaries2.Add("accept-encoding", "gzip, deflate, br");
				orderedDictionaries2.Add("accept-language", "en-GB,en-US;q=0.9,en;q=0.8");
				orderedDictionaries = orderedDictionaries2;
			}
			Response response = await base.Client.MakeRequest(url, "POST", orderedDictionaries, data, true, true, true);
			return response;
		}

		public async Task<Response> PostOppwa(string url, string data, string referer)
		{
			OrderedDictionary orderedDictionaries = new OrderedDictionary()
			{
				{ "Accept", "*/*" },
				{ "X-Requested-With", "XMLHttpRequest" },
				{ "Content-Type", "application/x-www-form-urlencoded; charset=UTF-8" },
				{ "Origin", "https://oppwa.com" },
				{ "Referer", referer },
				{ "Accept-Encoding", "gzip, deflate, br" },
				{ "Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8" }
			};
			OrderedDictionary orderedDictionaries1 = orderedDictionaries;
			Response response = await base.Client.MakeRequest(url, "POST", orderedDictionaries1, data, true, true, false);
			return response;
		}

		public async Task<Response> PostOppwaAuth(string url, string data, string referer)
		{
			OrderedDictionary orderedDictionaries = new OrderedDictionary()
			{
				{ "cache-control", "max-age=0" },
				{ "upgrade-insecure-requests", "1" },
				{ "origin", "https://oppwa.com" },
				{ "content-type", "application/x-www-form-urlencoded" },
				{ "accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
				{ "referer", referer },
				{ "accept-encoding", "gzip, deflate, br" },
				{ "accept-language", "en-GB,en-US;q=0.9,en;q=0.8" }
			};
			OrderedDictionary orderedDictionaries1 = orderedDictionaries;
			Response response = await base.Client.MakeRequest(url, "POST", orderedDictionaries1, data, true, true, false);
			return response;
		}

		public async Task<Response> PostOppwaPayment(string url, string data)
		{
			OrderedDictionary orderedDictionaries = new OrderedDictionary()
			{
				{ "Upgrade-Insecure-Requests", "1" },
				{ "Origin", this._task.HomeUrl },
				{ "Content-Type", "application/x-www-form-urlencoded" },
				{ "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
				{ "Referer", string.Format("{0}/payment", this._task.HomeUrl) },
				{ "Accept-Encoding", "gzip, deflate, br" },
				{ "Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8" }
			};
			OrderedDictionary orderedDictionaries1 = orderedDictionaries;
			Response response = await base.Client.MakeRequest(url, "POST", orderedDictionaries1, data, true, true, false);
			return response;
		}

		public async Task<Response> PostOppwaRedirect(string url, string data, string origin, string referer)
		{
			OrderedDictionary orderedDictionaries = new OrderedDictionary()
			{
				{ "Upgrade-Insecure-Requests", "1" },
				{ "Origin", this._task.HomeUrl },
				{ "Content-Type", "application/x-www-form-urlencoded" },
				{ "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
				{ "Referer", referer },
				{ "Accept-Encoding", "gzip, deflate, br" },
				{ "Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8" }
			};
			OrderedDictionary orderedDictionaries1 = orderedDictionaries;
			Response response = await base.Client.MakeRequest(url, "POST", orderedDictionaries1, data, true, false, false);
			return response;
		}

		public async Task<Response> PostPayment(string url, string data, string authorization, string referer)
		{
			object obj;
			OrderedDictionary orderedDictionaries = new OrderedDictionary()
			{
				{ "content-length", "x" }
			};
			obj = (string.IsNullOrEmpty(authorization) ? "null" : authorization);
			orderedDictionaries.Add("checkout-authorization", obj);
			orderedDictionaries.Add("content-type", "application/json");
			orderedDictionaries.Add("user-agent", this._task.UserAgent);
			orderedDictionaries.Add("accept", "*/*");
			orderedDictionaries.Add("origin", this._task.HomeUrl);
			orderedDictionaries.Add("sec-fetch-site", "same-origin");
			orderedDictionaries.Add("sec-fetch-mode", "cors");
			orderedDictionaries.Add("sec-fetch-dest", "empty");
			orderedDictionaries.Add("referer", referer);
			orderedDictionaries.Add("accept-encoding", "gzip, deflate, br");
			orderedDictionaries.Add("accept-language", "en-GB,en-US;q=0.9,en;q=0.8");
			OrderedDictionary orderedDictionaries1 = orderedDictionaries;
			Response response = await base.Client.MakeRequest(url, "POST", orderedDictionaries1, data, true, true, false);
			return response;
		}

		public async Task<Response> PostPoll(string url, string data, string origin, string referer)
		{
			OrderedDictionary orderedDictionaries = new OrderedDictionary()
			{
				{ "content-type", "application/json" },
				{ "accept", "*/*" },
				{ "origin", this._task.HomeUrl },
				{ "referer", referer },
				{ "accept-encoding", "gzip, deflate, br" },
				{ "accept-language", "en-GB,en-US;q=0.9,en;q=0.8" }
			};
			OrderedDictionary orderedDictionaries1 = orderedDictionaries;
			Response response = await base.Client.MakeRequest(url, "POST", orderedDictionaries1, data, true, true, false);
			return response;
		}

		public async Task<Response> PostRisk(string url, string data, string origin, string referer)
		{
			OrderedDictionary orderedDictionaries = new OrderedDictionary()
			{
				{ "Accept", "*/*" },
				{ "X-Requested-With", "XMLHttpRequest" },
				{ "Content-Type", "application/x-www-form-urlencoded" },
				{ "Origin", this._task.HomeUrl },
				{ "Referer", referer },
				{ "Accept-Encoding", "gzip, deflate, br" },
				{ "Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8" }
			};
			OrderedDictionary orderedDictionaries1 = orderedDictionaries;
			Response response = await base.Client.MakeRequest(url, "POST", orderedDictionaries1, data, true, true, false);
			return response;
		}

		public async Task<Response> PostSensor(string url, string data, string referer)
		{
			OrderedDictionary orderedDictionaries = new OrderedDictionary()
			{
				{ "content-length", "x" },
				{ "content-type", "text/plain;charset=UTF-8" },
				{ "user-agent", this._task.UserAgent },
				{ "accept", "*/*" },
				{ "origin", this._task.HomeUrl },
				{ "sec-fetch-site", "same-origin" },
				{ "sec-fetch-mode", "cors" },
				{ "sec-fetch-dest", "empty" },
				{ "referer", referer },
				{ "accept-encoding", "gzip, deflate, br" },
				{ "accept-language", "en-GB,en-US;q=0.9,en;q=0.8" }
			};
			OrderedDictionary orderedDictionaries1 = orderedDictionaries;
			Response response = await base.Client.MakeRequest(url, "POST", orderedDictionaries1, data, true, true, false);
			return response;
		}
	}
}
