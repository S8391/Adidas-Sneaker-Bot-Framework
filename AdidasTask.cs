using HtmlAgilityPack;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using FreeSBAIO.Data.Common;
using FreeSBAIO.Data.Misc;
using FreeSBAIO.Helpers.Address;
using FreeSBAIO.Helpers.Captcha;
using FreeSBAIO.Helpers.Misc;
using FreeSBAIO.Helpers.Utilities;
using FreeSBAIO.Models.Billing;
using FreeSBAIO.Models.Misc;
using FreeSBAIO.Models.Proxies;
using FreeSBAIO.Models.Settings;
using FreeSBAIO.Models.Tasks;
using FreeSBAIO.Networking.Common;
using FreeSBAIO.Services.Devices;
using FreeSBAIO.Services.Logging;
using FreeSBAIO.Services.Notifications;
using FreeSBAIO.ViewModels.ContextMenu;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FreeSBAIO.Tasks.Adidas
{
	internal class AdidasTask : FreeSBAIO.Models.Tasks.Task
	{
		private string _region;

		private string _url;

		private AdidasSplashMode? _splashMode;

		private AdidasSession _session;

		private string _productUrl;

		private string _sizeCode;

		private bool _passedSplash;

		private bool _carted;

		private bool _basketsApi;

		private bool _oppwaCheckout;

		private bool _recaptchaEnabled;

		private string _recaptchaSitekey;

		private string _recaptchaAction;

		private string _captchaCookieName;

		private string _pollUrl;

		private int _pollDelay;

		private string _basketId;

		private string _authHeader;

		private string _akamaiUrl;

		private string _adyenPublicKey;

		private const int _maxNumberGenerations = 50;

		private const int _maxNumberFailedCarts = 50;

		private const int _maxNumberFailedCheckouts = 10;

		public static List<string> BasketsApiRegions;

		public static List<string> OppwaCheckoutRegions;

		public static List<string> NormalSizes;

		public static List<string> MxSizes;

		public static List<string> EuSizes;

		public static List<string> RuSizes;

		public static List<string> BrSizes;

		public static List<string> AsianSizes;

		[JsonIgnore]
		public List<string> AvailableSizes
		{
			get
			{
				return this.GetAvailableSizes();
			}
		}

		[JsonIgnore]
		public string DwUrl
		{
			get;
			set;
		}

		[JsonIgnore]
		public string HomeUrl
		{
			get;
			set;
		}

		public string Region
		{
			get
			{
				return this._region;
			}
			set
			{
				if (this._region != value)
				{
					this._region = value;
					base.OnPropertyChanged("Region");
					base.OnPropertyChanged("AvailableSizes");
				}
			}
		}

		public static List<string> Regions
		{
			get;
		}

		public AdidasSplashMode? SplashMode
		{
			get
			{
				return this._splashMode;
			}
			set
			{
				AdidasSplashMode? nullable = this._splashMode;
				AdidasSplashMode? nullable1 = value;
				if (nullable.GetValueOrDefault() != nullable1.GetValueOrDefault() | nullable.HasValue != nullable1.HasValue)
				{
					this._splashMode = value;
					base.OnPropertyChanged("SplashMode");
				}
			}
		}

		public string Url
		{
			get
			{
				return this._url;
			}
			set
			{
				if (this._url != value)
				{
					this._url = value;
					base.OnPropertyChanged("Url");
				}
			}
		}

		static AdidasTask()
		{
			AdidasTask.Regions = new List<string>()
			{
				"UK",
				"US",
				"CA",
				"RU",
				"AU",
				"DE",
				"IT",
				"FR",
				"PL",
				"NL",
				"NO",
				"SK",
				"FI",
				"CH",
				"BE",
				"BR",
				"IE",
				"ES",
				"SE",
				"MX",
				"NZ",
				"GR",
				"PT",
				"AT",
				"MY",
				"SG",
				"PH",
				"DK",
				"CZ",
				"CO",
				"TH",
				"TR"
			};
			AdidasTask.BasketsApiRegions = new List<string>()
			{
				"UK",
				"US",
				"CA",
				"DE",
				"IT",
				"FR",
				"PL",
				"NL",
				"NO",
				"SK",
				"FI",
				"CH",
				"BE",
				"IE",
				"ES",
				"SE",
				"GR",
				"PT",
				"AT",
				"DK",
				"CZ",
				"RU",
				"MX"
			};
			AdidasTask.OppwaCheckoutRegions = new List<string>()
			{
				"UK",
				"US",
				"DE",
				"CH",
				"BE"
			};
			AdidasTask.NormalSizes = new List<string>()
			{
				"Random",
				"3",
				"3.5",
				"4",
				"4.5",
				"5",
				"5.5",
				"6",
				"6.5",
				"7",
				"7.5",
				"8",
				"8.5",
				"9",
				"9.5",
				"10",
				"10.5",
				"11",
				"11.5",
				"12",
				"12.5",
				"13",
				"13.5",
				"14",
				"14.5",
				"15",
				"15.5",
				"16",
				"1k",
				"2k",
				"3k",
				"4k",
				"5k",
				"5.5k",
				"6k",
				"6.5k",
				"7k",
				"7.5k",
				"8k",
				"8.5k",
				"9k",
				"9.5k",
				"10k",
				"XS",
				"S",
				"M",
				"L",
				"XL",
				"XXL"
			};
			AdidasTask.MxSizes = new List<string>()
			{
				"Random",
				"MX 2",
				"MX 2.5",
				"MX 3",
				"MX 3.5",
				"MX 4",
				"MX 4.5",
				"MX 5",
				"MX 5.5",
				"MX 6",
				"MX 6.5",
				"MX 7",
				"MX 7.5",
				"MX 8",
				"MX 8.5",
				"MX 9",
				"MX 9.5",
				"MX 10",
				"MX 10.5",
				"MX 11",
				"MX 11.5",
				"MX 12",
				"MX 12.5",
				"MX 13",
				"MX 13.5",
				"MX 14"
			};
			AdidasTask.EuSizes = new List<string>()
			{
				"Random",
				"36",
				"36 2/3",
				"37 1/3",
				"38",
				"38 2/3",
				"38.5",
				"39 1/3",
				"40",
				"40 2/3",
				"41 1/3",
				"42",
				"42 2/3",
				"43 1/3",
				"44",
				"44 2/3",
				"45 1/3",
				"46",
				"46 2/3",
				"47 1/3",
				"48",
				"48 2/3",
				"49 1/3",
				"50",
				"50 2/3",
				"51 1/3",
				"XS",
				"S",
				"M",
				"L",
				"XL",
				"XXL"
			};
			AdidasTask.RuSizes = new List<string>()
			{
				"Random",
				"36 RU",
				"36.5 RU",
				"37 RU",
				"37.5 RU",
				"38 RU",
				"38.5 RU",
				"39 RU",
				"40 RU",
				"40.5 RU",
				"41 RU",
				"42 RU",
				"42.5 RU",
				"43 RU",
				"44 RU",
				"44.5 RU",
				"45 RU",
				"46 RU",
				"46.5 RU",
				"47 RU",
				"48 RU",
				"49 RU",
				"49.5 RU",
				"50 RU",
				"52 RU",
				"XS",
				"S",
				"M",
				"L",
				"XL",
				"XXL"
			};
			AdidasTask.BrSizes = new List<string>()
			{
				"Random",
				"34",
				"35",
				"36",
				"37",
				"38",
				"39",
				"40",
				"41",
				"42",
				"43",
				"44",
				"45",
				"46",
				"47",
				"48",
				"XS",
				"S",
				"M",
				"L",
				"XL",
				"XXL"
			};
			AdidasTask.AsianSizes = new List<string>()
			{
				"Random",
				"3 UK",
				"3.5 UK",
				"4 UK",
				"4.5 UK",
				"5 UK",
				"5.5 UK",
				"6 UK",
				"6.5 UK",
				"7 UK",
				"7.5 UK",
				"8 UK",
				"8.5 UK",
				"9 UK",
				"9.5 UK",
				"10 UK",
				"10.5 UK",
				"11 UK",
				"11.5 UK",
				"12 UK",
				"12.5 UK",
				"13 UK",
				"13.5 UK",
				"14 UK",
				"14.5 UK",
				"15 UK",
				"15.5 UK",
				"16 UK"
			};
		}

		[JsonConstructor]
		public AdidasTask(int id, string region, string productId, string url, List<string> sizes, AdidasSplashMode? splashMode, string accountUuid, string proxyListUuid, string profileUuid, bool useTimer, FreeSBAIO.Models.Misc.Schedule schedule) : base(id, string.Format("Adidas {0}", region), productId, sizes, accountUuid, proxyListUuid, profileUuid, useTimer, schedule)
		{
			this.Region = region;
			this.Url = url;
			this.SplashMode = splashMode;
		}

		public AdidasTask(FreeSBAIO.Models.Tasks.TaskGroup taskGroup, bool isMassEdit) : base(taskGroup, isMassEdit)
		{
			AdidasSplashMode? nullable;
			if (isMassEdit)
			{
				nullable = null;
			}
			else
			{
				nullable = new AdidasSplashMode?(AdidasSplashMode.Splash);
			}
			this.SplashMode = nullable;
		}

		private string BuildAdyen(string key)
		{
			string script;
			try
			{
				string str = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
				string str1 = FreeSBAIO.Models.Tasks.Task._dataHandler.ReadAllText("Assets/Adyen/adyen_1_21.js").Replace("[KEY]", key).Replace("[GENTIME]", str).Replace("[NAME]", string.Concat((base.Profile.BillingSameAsShipping ? base.Profile.ShippingAddress.FirstName : base.Profile.BillingAddress.FirstName), " ", (base.Profile.BillingSameAsShipping ? base.Profile.ShippingAddress.LastName : base.Profile.BillingAddress.LastName))).Replace("[CVV]", base.Profile.Card.Cvv).Replace("[CARDNUM]", base.Profile.Card.Number).Replace("[MONTH]", string.Format("{0:00}", int.Parse(base.Profile.Card.ExpiryMonth))).Replace("[YEAR]", base.Profile.Card.ExpiryYear);
				using (V8ScriptEngine v8ScriptEngine = new V8ScriptEngine())
				{
					v8ScriptEngine.Execute(str1);
					script = (string)((dynamic)v8ScriptEngine.get_Script()).result.ToString();
				}
			}
			catch
			{
				script = null;
			}
			return script;
		}

		public override FreeSBAIO.Models.Tasks.Task Clone()
		{
			return new AdidasTask(base.Id, this.Region, base.Product, this.Url, new List<string>(base.Sizes), this.SplashMode, base.AccountUuid, base.ProxyListUuid, base.ProfileUuid, base.UseTimer, base.Schedule.Clone())
			{
				TaskGroup = base.TaskGroup
			};
		}

		protected override void CloseSession()
		{
			if (this._session != null)
			{
				this._session.Close();
			}
			this._session = null;
		}

		private async System.Threading.Tasks.Task Complete3DS(string uuid, JObject paymentResponse)
		{
			var variable = new AdidasTask.Complete3DSHelper();
            variable._this = this;
            variable.uuid = uuid;
            variable.paymentResponse = paymentResponse;
            variable.tcs = new TaskCompletionSource<bool>();
            await variable.Process();
		}

		private async System.Threading.Tasks.Task CompleteOppwa3DS(string uuid, string checkoutId, string action, string termUrl, string paReq, string md)
		{
			var variable = new AdidasTask.CompleteOppwa3DSHelper();
			variable._this = this;
			variable.uuid = uuid;
			variable.checkoutId = checkoutId;
			variable.action = action;
			variable.termUrl = termUrl;
			variable.paReq = paReq;
			variable.md = md;
			variable.tcs = new TaskCompletionSource<bool>();
			await variable.Process();
		}

		public override async System.Threading.Tasks.Task CopyNonNullValues(FreeSBAIO.Models.Tasks.Task t, bool editReleaseTimer)
		{
			AdidasTask region = t as AdidasTask;
			if (region != null)
			{
				if (!string.IsNullOrEmpty(this.Region))
				{
					region.Region = this.Region;
				}
				if (!string.IsNullOrEmpty(this.Url))
				{
					region.Url = this.Url;
				}
				if (this.SplashMode.HasValue)
				{
					region.SplashMode = this.SplashMode;
				}
				await this.<>n__1(region, editReleaseTimer);
			}
		}

		public override async System.Threading.Tasks.Task CopyValues(FreeSBAIO.Models.Tasks.Task t)
		{
			AdidasTask region = t as AdidasTask;
			if (region != null)
			{
				region.Region = this.Region;
				region.Url = this.Url;
				region.SplashMode = this.SplashMode;
				await this.<>n__0(region);
			}
		}

		protected override async System.Threading.Tasks.Task CreateSession()
		{
			this._session = new AdidasSession(this);
			await this._session.Start();
		}

		private async Task<bool> GenerateCookie(bool useEvents)
		{
			bool flag;
			try
			{
				string empty = string.Empty;
				foreach (Cookie cooky in await this._session.Client.GetCookies())
				{
					if (cooky.Name != "_abck")
					{
						continue;
					}
					empty = cooky.Value;
				}
				object[] apiKey = new object[] { BotData.ApiKey, BotData.Settings.LicenseKey, useEvents, base.UserAgent, this._productUrl, empty };
				string str = string.Format("removed api keyy);
				Response Api = await this._session.GetApi(str, null);
				if (!soleApi.Error)
				{
					JObject jObject = JObject.Parse(Api.Body);
					if (jObject.ContainsKey("status") && jObject.get_Item("status").ToObject<string>() == "success")
					{
						string obj = jObject.get_Item("sensor_data").ToObject<string>();
						string str1 = string.Concat("{\"sensor_data\":\"", obj, "\"}");
						Response response = await this._session.PostSensor(this._akamaiUrl, str1, this._productUrl);
						foreach (Cookie cookie in await this._session.Client.GetCookies())
						{
							if (cookie.Name != "_abck")
							{
								continue;
							}
							string value = cookie.Value;
						}
						base.Log(string.Concat("Generated cookie ", empty));
						flag = response.Body.Contains("\"success\": true");
						return flag;
					}
				}
				empty = null;
			}
			catch
			{
			}
			flag = false;
			return flag;
		}

		private void GetAdyenPublicKey(string html)
		{
			try
			{
				this._adyenPublicKey = html.Split(new string[] { "\"adyenPublicKey\":\"" }, StringSplitOptions.None)[1].Split(new char[] { '\"' })[0];
				this._adyenPublicKey = this._adyenPublicKey.Trim(new char[] { '\\' });
			}
			catch
			{
			}
			if (string.IsNullOrEmpty(this._adyenPublicKey))
			{
				try
				{
					this._adyenPublicKey = html.Split(new string[] { "\\\"adyenPublicKey\\\":\\\"" }, StringSplitOptions.None)[1].Split(new string[] { "\"" }, StringSplitOptions.None)[0];
					this._adyenPublicKey = this._adyenPublicKey.Trim(new char[] { '\\' });
				}
				catch
				{
				}
			}
		}

		private void GetAkamaiData(string html)
		{
			try
			{
				HtmlDocument htmlDocument = new HtmlDocument();
				htmlDocument.LoadHtml(html);
				foreach (HtmlNode htmlNode in htmlDocument.get_DocumentNode().SelectNodes("//script"))
				{
					try
					{
						if (htmlNode.get_InnerText().Contains("'_setAu', '"))
						{
							string str = htmlNode.get_InnerText().Split(new string[] { "'_setAu', '" }, StringSplitOptions.None)[1].Split(new char[] { '\'' })[0];
							this._akamaiUrl = string.Format("{0}{1}", this.HomeUrl, str);
						}
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
		}

		private List<string> GetAvailableSizes()
		{
			string region = this.Region;
			if (!(region == "NZ") && !(region == "TH") && !(region == "IE") && !(region == "AU") && !(region == "CA") && !(region == "US") && !(region == "CO") && !(region == "UK"))
			{
				while (region != null)
				{
				}
			}
			return AdidasTask.NormalSizes;
		}

		private string GetBackgroundScript(string url, List<Cookie> cookies)
		{
			bool proxy = base.Proxy != null;
			string str = FreeSBAIO.Models.Tasks.Task._dataHandler.ReadAllText("Assets/Extensions/Adidas/background.js").Replace("var useProxy = false;", string.Format("var useProxy = {0};", proxy.ToString().ToLower())).Replace("[URL]", url);
			if (base.Proxy != null)
			{
				str = str.Replace("[PROXY_IP]", base.Proxy.Address).Replace("[PROXY_PORT]", base.Proxy.Port);
				if (base.Proxy.IsUserPass)
				{
					str = str.Replace("var userPass = false;", "var userPass = true;").Replace("[PROXY_USERNAME]", base.Proxy.Username).Replace("[PROXY_PASSWORD]", base.Proxy.Password);
				}
			}
			string str1 = "var cookies = [  ";
			foreach (Cookie cooky in cookies)
			{
				object[] name = new object[] { cooky.Name, cooky.Value, cooky.Domain, cooky.Path, null, null };
				proxy = cooky.Secure;
				name[4] = proxy.ToString().ToLower();
				proxy = cooky.HttpOnly;
				name[5] = proxy.ToString().ToLower();
				str1 = string.Concat(str1, "{ ", string.Format("\"name\": \"{0}\", \"value\": \"{1}\", \"domain\": \"{2}\", \"path\": \"{3}\", \"secure\": {4}, \"httpOnly\": {5}", name), " }, ");
			}
			str1 = string.Concat(str1.Remove(str1.Length - 2), "];");
			return str.Replace("var cookies;", str1);
		}

		private int GetCardMonth()
		{
			try
			{
				if (base.Profile.Card.ExpiryMonth.StartsWith("0"))
				{
					int num = int.Parse(base.Profile.Card.ExpiryMonth.Split(new char[] { '0' })[1]);
					return num;
				}
			}
			catch
			{
			}
			return 1;
		}

		private string GetCardType()
		{
			string type = base.Profile.Card.Type;
			if (type == "Mastercard")
			{
				return "MASTER";
			}
			if (type == "Visa")
			{
				return "VISA";
			}
			if (type == "American Express")
			{
				return "AMEX";
			}
			if (type == "Solo")
			{
				return "SOLO";
			}
			if (type == "Maestro")
			{
				return "MAESTRO";
			}
			if (type != "Delta")
			{
				return "MASTER";
			}
			return "DELTA";
		}

		private string GetCartUrl()
		{
			if (!this._basketsApi)
			{
				return string.Format("{0}Cart-Show", this.DwUrl);
			}
			return string.Format("{0}{1}/cart", this.HomeUrl, (this.Region.ToLower() == "us" ? "/us" : string.Empty));
		}

		private void GetCheckoutDetails(Response response)
		{
			try
			{
				JObject jObject = JObject.Parse(response.Body);
				this._basketId = jObject.get_Item("basketId").ToObject<string>();
				if (response.Headers.ContainsKey("authorization"))
				{
					this._authHeader = response.Headers["authorization"];
				}
			}
			catch
			{
			}
		}

		private string GetContentScript(string url, Dictionary<string, string> localStorage)
		{
			string str = FreeSBAIO.Models.Tasks.Task._dataHandler.ReadAllText("Assets/Extensions/Adidas/content.js").Replace("[URL]", url);
			string str1 = "var localStorageItems = [  ";
			foreach (KeyValuePair<string, string> keyValuePair in localStorage)
			{
				str1 = string.Concat(str1, "{ ", string.Format("\"name\": \"{0}\", \"value\": \"{1}\"", keyValuePair.Key, keyValuePair.Value), " }, ");
			}
			str1 = string.Concat(str1.Remove(str1.Length - 2), "];");
			return str.Replace("var localStorageItems;", str1);
		}

		private async Task<bool> GetCookie(string uuid)
		{
			bool flag;
			int num = 0;
			while (!base.IsFinished(uuid))
			{
				if (string.IsNullOrEmpty(this._akamaiUrl) || num >= 50)
				{
					flag = false;
					return flag;
				}
				else
				{
					base.SetStatus(uuid, string.Format("Generating cookie ({0})", num + 1), FreeSBAIO.Helpers.Misc.Status.StatusColor.Default, false);
					if (!await this.GenerateCookie(num > 0))
					{
						num++;
						await base.Delay(100);
					}
					else
					{
						flag = true;
						return flag;
					}
				}
			}
			flag = true;
			return flag;
		}

		private string GetDeliveryData(JObject shippingMethod)
		{
			string obj;
			JObject jObject = new JObject();
			jObject.set_Item("customer", new JObject());
			jObject.get_Item("customer").set_Item("email", base.Profile.Email);
			jObject.get_Item("customer").set_Item("receiveSmsUpdates", false);
			JObject jObject1 = new JObject();
			jObject1.set_Item("country", (this._region.ToLower() == "uk" ? "GB" : this._region.ToUpper()));
			jObject1.set_Item("firstName", base.Profile.ShippingAddress.FirstName);
			jObject1.set_Item("lastName", base.Profile.ShippingAddress.LastName);
			jObject1.set_Item("address1", base.Profile.ShippingAddress.Address1);
			jObject1.set_Item((this._region.ToLower() == "ru" ? "houseNumber" : "address2"), base.Profile.ShippingAddress.Address2);
			jObject1.set_Item("city", base.Profile.ShippingAddress.City);
			if (this._region.ToLower() == "us")
			{
				jObject1.set_Item("stateCode", this.GetStateCode(base.Profile.ShippingAddress.State));
			}
			else if (this._region.ToLower() == "ca")
			{
				jObject1.set_Item("countyProvince", this.GetStateCode(base.Profile.ShippingAddress.State));
			}
			jObject1.set_Item("zipcode", base.Profile.ShippingAddress.Zip);
			jObject1.set_Item("phoneNumber", base.Profile.PhoneNumber);
			jObject.set_Item("shippingAddress", jObject1);
			if (base.Profile.BillingSameAsShipping)
			{
				jObject.set_Item("billingAddress", jObject1);
			}
			else
			{
				JObject jObject2 = new JObject();
				jObject2.set_Item("country", (this._region.ToLower() == "uk" ? "GB" : this._region.ToUpper()));
				jObject2.set_Item("firstName", base.Profile.BillingAddress.FirstName);
				jObject2.set_Item("lastName", base.Profile.BillingAddress.LastName);
				jObject2.set_Item("address1", base.Profile.BillingAddress.Address1);
				jObject2.set_Item((this._region.ToLower() == "ru" ? "houseNumber" : "address2"), base.Profile.BillingAddress.Address2);
				jObject2.set_Item("city", base.Profile.BillingAddress.City);
				if (this._region.ToLower() == "us")
				{
					jObject2.set_Item("stateCode", this.GetStateCode(base.Profile.BillingAddress.State));
				}
				else if (this._region.ToLower() == "ca")
				{
					jObject2.set_Item("countyProvince", this.GetStateCode(base.Profile.BillingAddress.State));
				}
				jObject2.set_Item("zipcode", base.Profile.BillingAddress.Zip);
				jObject2.set_Item("phoneNumber", base.Profile.PhoneNumber);
				jObject.set_Item("billingAddress", jObject2);
			}
			JArray jArray = new JArray();
			JObject jObject3 = new JObject();
			jObject3.set_Item("id", (this._region.ToLower() == "ru" ? "Standard" : shippingMethod.get_Item("id").ToObject<string>()));
			jObject3.set_Item("shipmentId", shippingMethod.get_Item("shipmentId").ToObject<string>());
			if (shippingMethod.ContainsKey("carrierCode"))
			{
				jObject3.set_Item("carrierCode", shippingMethod.get_Item("carrierCode").ToObject<string>());
			}
			if (shippingMethod.ContainsKey("carrierServiceCode"))
			{
				jObject3.set_Item("carrierServiceCode", shippingMethod.get_Item("carrierServiceCode").ToObject<string>());
			}
			if (shippingMethod.ContainsKey("locationProviderId"))
			{
				JObject jObject4 = jObject3;
				if (shippingMethod.get_Item("locationProviderId").ToObject<string>() == "null")
				{
					obj = null;
				}
				else
				{
					obj = shippingMethod.get_Item("locationProviderId").ToObject<string>();
				}
				jObject4.set_Item("locationProviderId", obj);
			}
			if (shippingMethod.ContainsKey("shipNode"))
			{
				jObject3.set_Item("shipNode", shippingMethod.get_Item("shipNode").ToObject<string>());
			}
			if (shippingMethod.ContainsKey("collection"))
			{
				JObject jObject5 = JObject.Parse(shippingMethod.get_Item("collection").ToString());
				if (jObject5.ContainsKey("from") && jObject5.ContainsKey("to"))
				{
					double num = jObject5.get_Item("from").ToObject<double>();
					double obj1 = jObject5.get_Item("to").ToObject<double>();
					string str = string.Format("{0},{1}", this.GetFormattedTimestamp(num), this.GetFormattedTimestamp(obj1));
					jObject3.set_Item("collectionPeriod", str);
				}
			}
			if (shippingMethod.ContainsKey("delivery"))
			{
				JObject jObject6 = JObject.Parse(shippingMethod.get_Item("delivery").ToString());
				if (jObject6.ContainsKey("from") && jObject6.ContainsKey("to"))
				{
					double num1 = jObject6.get_Item("from").ToObject<double>();
					double obj2 = jObject6.get_Item("to").ToObject<double>();
					string str1 = string.Format("{0},{1}", this.GetFormattedTimestamp(num1), this.GetFormattedTimestamp(obj2));
					jObject3.set_Item("deliveryPeriod", str1);
				}
			}
			jArray.Add(jObject3);
			jObject.set_Item("methodList", jArray);
			jObject.set_Item("newsletterSubscription", true);
			return jObject.ToString(0, Array.Empty<JsonConverter>());
		}

		private string GetDwUrl()
		{
			Tuple<string, string> regionData = this.GetRegionData();
			string lower = this.Region.ToLower();
			if (lower == "uk")
			{
				return string.Format("https://www.adidas{0}/on/demandware.store/Sites-adidas-GB-Site/{1}/", regionData.Item1, regionData.Item2);
			}
			if (lower == "gr" || lower == "pt")
			{
				return string.Format("https://www.adidas{0}/on/demandware.store/Sites-adidas-MLT-Site/{1}/", regionData.Item1, regionData.Item2);
			}
			return string.Format("https://www.adidas{0}/on/demandware.store/Sites-adidas-{1}-Site/{2}/", regionData.Item1, this.Region.ToUpper(), regionData.Item2);
		}

		private string GetFormattedTimestamp(double timestamp)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			DateTime universalTime = dateTime.AddMilliseconds(timestamp);
			dateTime = universalTime.ToLocalTime();
			universalTime = dateTime.ToUniversalTime();
			return universalTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
		}

		public string GetHomeUrl()
		{
			Tuple<string, string> regionData = this.GetRegionData();
			if (regionData == null)
			{
				return null;
			}
			return string.Format("https://www.adidas{0}", regionData.Item1);
		}

		private void GetImage(JObject response)
		{
			try
			{
				this._image = response.get_Item("shipmentList").get_Item(0).get_Item("productLineItemList").get_Item(0).get_Item("productImage").ToObject<string>();
			}
			catch
			{
			}
		}

		private string GetLastFourCardDigits()
		{
			string str;
			try
			{
				str = base.Profile.Card.Number.Replace(" ", "").Substring(base.Profile.Card.Number.Replace(" ", "").Length - 4);
			}
			catch
			{
				return string.Empty;
			}
			return str;
		}

		private string GetOppwaPaymentData()
		{
			JObject jObject = new JObject();
			jObject.set_Item("basketId", this._basketId);
			JObject jObject1 = new JObject();
			jObject1.set_Item("paymentMethodId", "CREDIT_CARD");
			jObject.set_Item("paymentInstrument", jObject1);
			return jObject.ToString();
		}

		private string GetPaymentData(string encryptedInstrument)
		{
			JObject jObject = new JObject();
			jObject.set_Item("basketId", this._basketId);
			jObject.set_Item("encryptedInstrument", encryptedInstrument);
			JObject jObject1 = new JObject();
			jObject1.set_Item("holder", base.Profile.Card.HolderName);
			jObject1.set_Item("expirationMonth", this.GetCardMonth());
			jObject1.set_Item("expirationYear", int.Parse(base.Profile.Card.ExpiryYear));
			jObject1.set_Item("lastFour", this.GetLastFourCardDigits());
			jObject1.set_Item("paymentMethodId", "CREDIT_CARD");
			jObject1.set_Item("cardType", this.GetCardType());
			jObject.set_Item("paymentInstrument", jObject1);
			jObject.set_Item("fingerprint", "ryEGX8eZpJ0030000000000000BTWDfYZVR30089146776cVB94iKzBGDXVH6pEuPO5S16Goh5Mk0045zgp4q8JSa00000qZkTE00000PRbZ1HbvOQLlxFnNgZ6L:40");
			return jObject.ToString(0, Array.Empty<JsonConverter>());
		}

		private string GetStartPaymentData()
		{
			JObject jObject = new JObject();
			jObject.set_Item("basketId", this._basketId);
			jObject.set_Item("isAsync", false);
			jObject.set_Item("paymentMethodId", "CREDIT_CARD");
			jObject.set_Item("token", this._authHeader);
			jObject.set_Item("cardType", this.GetCardType());
			return jObject.ToString();
		}

		private string GetStateCode(string state)
		{
			if (string.IsNullOrEmpty(state))
			{
				return string.Empty;
			}
			if (this._region.ToLower() == "us")
			{
				if (!States.UsStateCodes.ContainsKey(state))
				{
					return string.Empty;
				}
				return States.UsStateCodes[state];
			}
			if (this._region.ToLower() != "ca")
			{
				return string.Empty;
			}
			if (!States.CaStateCodes.ContainsKey(state))
			{
				return string.Empty;
			}
			return States.CaStateCodes[state];
		}

		private bool IsBanned(string page)
		{
			if (string.IsNullOrEmpty(page))
			{
				return false;
			}
			return page.ToLower().Contains("unable to give you access");
		}

		public override void LoadData()
		{
			string clientHello;
			base.LoadData();
			Device device = Devices.Get();
			base.UserAgent = (device == null ? BotData.UserAgent : device.UserAgent);
			if (device == null)
			{
				clientHello = null;
			}
			else
			{
				clientHello = device.ClientHello;
			}
			base.ClientHello = clientHello;
			this.HomeUrl = this.GetHomeUrl();
			this.DwUrl = this.GetDwUrl();
			this._productUrl = null;
			this._sizeCode = null;
			this._passedSplash = false;
			this._carted = false;
			this._basketsApi = AdidasTask.BasketsApiRegions.Contains(this._region);
			this._oppwaCheckout = AdidasTask.OppwaCheckoutRegions.Contains(this._region);
			this._recaptchaEnabled = false;
			this._recaptchaSitekey = null;
			this._recaptchaAction = null;
			this._captchaCookieName = null;
			this._pollUrl = null;
			this._pollDelay = 0;
			this._basketId = null;
			this._authHeader = null;
			this._akamaiUrl = null;
			this._adyenPublicKey = null;
		}

		public override ObservableCollection<MenuItemViewModel> MenuItems()
		{
			ObservableCollection<MenuItemViewModel> observableCollection = new ObservableCollection<MenuItemViewModel>();
			if (this._carted)
			{
				observableCollection.Add(new MenuItemViewModel("Open Cart", (object o) => this.OpenInChome()));
			}
			else if (this._passedSplash)
			{
				observableCollection.Add(new MenuItemViewModel("Open Splash Pass", (object o) => this.OpenInChome()));
			}
			return new ObservableCollection<MenuItemViewModel>(observableCollection.Concat<MenuItemViewModel>(base.MenuItems()).ToList<MenuItemViewModel>());
		}

		private async void OpenInChome()
		{
			List<Cookie> list;
			string str;
			str = (this._carted ? this.GetCartUrl() : this._productUrl);
			string str1 = str;
			if (this._session != null)
			{
				List<Cookie> cookies = await this._session.Client.GetCookies();
				list = cookies.Where<Cookie>((Cookie c) => {
					if (c.Name.StartsWith("_ab") || c.Name.StartsWith("ak_"))
					{
						return false;
					}
					return !c.Name.StartsWith("bm_");
				}).ToList<Cookie>();
			}
			else
			{
				list = new List<Cookie>();
			}
			List<Cookie> cookies1 = list;
			Dictionary<string, string> strs = new Dictionary<string, string>();
			if (this._carted)
			{
				if (!string.IsNullOrEmpty(this._basketId))
				{
					strs.Add("basketId", this._basketId);
				}
				if (!string.IsNullOrEmpty(this._authHeader))
				{
					strs.Add("jwtToken", this._authHeader);
				}
			}
			string str2 = string.Format("{0}/{1}", BotData.TempDirectory, Guid.NewGuid());
			Directory.CreateDirectory(string.Format("{0}/Extension", str2));
			FreeSBAIO.Models.Tasks.Task._dataHandler.WriteAllText(string.Format("{0}/Extension/manifest.json", str2), FreeSBAIO.Models.Tasks.Task._dataHandler.ReadAllText("Assets/Extensions/Adidas/manifest.json"));
			FreeSBAIO.Models.Tasks.Task._dataHandler.WriteAllText(string.Format("{0}/Extension/background.js", str2), this.GetBackgroundScript(str1, cookies1));
			FreeSBAIO.Models.Tasks.Task._dataHandler.WriteAllText(string.Format("{0}/Extension/content.js", str2), this.GetContentScript(str1, strs));
			string str3 = string.Format("\"{0}\" --user-data-dir=\"{1}\" --user-agent=\"{2}\" --load-extension=\"{1}\\Extension\"", BotData.Settings.ChromePath, str2, base.UserAgent);
			Process process = new Process();
			ProcessStartInfo processStartInfo = new ProcessStartInfo()
			{
				FileName = "cmd.exe",
				WindowStyle = ProcessWindowStyle.Hidden,
				RedirectStandardInput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};
			process.StartInfo = processStartInfo;
			Process process1 = process;
			process1.Start();
			using (StreamWriter standardInput = process1.StandardInput)
			{
				standardInput.WriteLine(str3);
			}
			process1.Close();
			str1 = null;
		}

		private void ParseStock(string uuid, string html)
		{
			dynamic lower;
			try
			{
				ExpandoObjectConverter expandoObjectConverter = new ExpandoObjectConverter();
				dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(html, new JsonConverter[] { expandoObjectConverter });
				try
				{
					lower = obj.message == "not found";
					if (!lower)
					{
						if ((lower | obj.availability_status == "PREVIEW") == 0)
						{
							goto Label1;
						}
					}
					base.SetStatus(uuid, "Product stock not loaded", FreeSBAIO.Helpers.Misc.Status.StatusColor.Default, false);
					return;
				}
				catch
				{
				}
				List<object> objs = (List<object>)obj.variation_list;
				List<object> objs1 = new List<object>();
				foreach (dynamic obj2 in objs)
				{
					if (obj2.availability_status != "IN_STOCK")
					{
						continue;
					}
					objs1.Add(obj2);
				}
				if (objs1.Count == 0)
				{
					base.SetStatus(uuid, "No suitable size found", FreeSBAIO.Helpers.Misc.Status.StatusColor.Default, false);
				}
				else if (!base.Sizes.ConvertAll<string>((string s) => s.ToLower()).Contains("random"))
				{
					foreach (string size in base.Sizes)
					{
						foreach (dynamic obj3 in objs)
						{
							lower = obj3.size.ToLower() == size.Trim().ToLower();
							if ((!lower ? lower == null : (lower & obj3.availability_status == "IN_STOCK") == 0))
							{
								continue;
							}
							this._selectedSize = (string)obj3.size;
							this._sizeCode = (string)obj3.sku.Split('\u005F')[1];
							base.Log(string.Concat("Found desired size ", this._selectedSize));
							return;
						}
					}
					if (string.IsNullOrEmpty(this._selectedSize))
					{
						base.SetStatus(uuid, "No suitable size found", FreeSBAIO.Helpers.Misc.Status.StatusColor.Default, false);
					}
				}
				else if (base.Sizes.Count != 1)
				{
					List<object> list = (
						from x in objs1
						where (bool)base.Sizes.Contains(x.size)
						select x).ToList<object>();
					dynamic item = list[Utilities.RandomInt(list.Count)];
					this._selectedSize = (string)item.size;
					this._sizeCode = (string)item.sku.Split('\u005F')[1];
					base.Log(string.Concat("Randomly selected size ", this._selectedSize));
				}
				else
				{
					dynamic item1 = objs1[Utilities.RandomInt(objs1.Count)];
					this._selectedSize = (string)item1.size;
					this._sizeCode = (string)item1.sku.Split('\u005F')[1];
					base.Log(string.Concat("Found desired size ", this._selectedSize));
				}
			}
			catch (Exception exception)
			{
				base.SetStatus(uuid, "Error parsing stock", FreeSBAIO.Helpers.Misc.Status.StatusColor.Default, false);
			}
		}

		protected override async System.Threading.Tasks.Task Run(string uuid)
		{
			var variable = new RunHelper
			{
				This = this,
				Uuid = uuid,
				Builder = AsyncTaskMethodBuilder.Create(),
				State = -1
			};
				variable.Builder.Start(ref variable);
				await variable.Builder.Task;
		}

		protected override async System.Threading.Tasks.Task UpdateProxy()
		{
			string str;
			if (base.IsRunning)
			{
				List<Cookie> cookies = await this._session.Client.GetCookies();
				this._session = new AdidasSession(this);
				await this._session.Start();
				foreach (Cookie cooky in cookies)
				{
					str = (string.IsNullOrEmpty(cooky.Domain) ? this.HomeUrl : cooky.Domain);
					string str1 = str;
					if (str1.StartsWith("."))
					{
						str1 = str1.Substring(1, str1.Length - 1);
					}
					str1 = string.Format("https://{0}", str1);
					await this._session.Client.SetCookie(str1, cooky);
				}
				cookies = null;
			}
		}
	}
}
