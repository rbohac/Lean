/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2016 QuantConnect Corporation.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QuantConnect.Data.Custom
{
    public class DailyFX : BaseData
    {
        private List<String> Subscriptions = null; 
        /// <summary>
        /// Default DailyFX constructor 
        /// </summary>
        public DailyFX()
        {
            Subscriptions = new List<string>();
        }


        /// <summary>
        /// Generic Reader Implementation for Quandl Data.
        /// </summary>
        /// <param name="config">Subscription configuration</param>
        /// <param name="line">CSV line of data from the souce</param>
        /// <param name="date">Date of the requested line</param>
        /// <param name="isLiveMode">true if we're in live mode, false for backtesting mode</param>
        /// <returns></returns>
        public override BaseData Reader(SubscriptionDataConfig config, string line, DateTime date, bool isLiveMode)
        {
            DailyFXItem[] dailyFXarray = null;
            //var liveBTC = JsonConvert.DeserializeObject<DailyFXItem>(line);
            try
            {
                // var des = (DailyFXItem) Newtonsoft.Json.JsonConvert.DeserializeObject(line, typeof (DailyFXItem));

                dailyFXarray = JsonConvert.DeserializeObject<DailyFXItem[]>(line);

            }
            catch (Exception ex)
            {
                
            }

            foreach (DailyFXItem item in dailyFXarray)
            {
                
            }

            var data = (DailyFX)Activator.CreateInstance(GetType());
            data.Symbol = config.Symbol;
       
            return data;
        }

        /// <summary>
        /// Quandl Source Locator: Using the Quandl V1 API automatically set the URL for the dataset.
        /// </summary>
        /// <param name="config">Subscription configuration object</param>
        /// <param name="date">Date of the data file we're looking for</param>
        /// <param name="isLiveMode">true if we're in live mode, false for backtesting mode</param>
        /// <returns>STRING API Url for Quandl.</returns>
        public override SubscriptionDataSource GetSource(SubscriptionDataConfig config, DateTime date, bool isLiveMode)
        {
          QuantConnect.Logging.Log.Debug("Called GetSource");
            var source = @"https://content.dailyfx.com/getData?contenttype=calendarEvent&description=true&startdate=20160410&enddate=201604172300";
            //https://www.quandl.com/api/v1/datasets/" + config.Symbol.Value + ".csv?sort_order=asc&exclude_headers=false&auth_token=" + _authCode;
            return new SubscriptionDataSource(source, SubscriptionTransportMedium.RemoteFile, FileFormat.JSON);
        }
    }

    class DailyFXItem
    {

        [JsonProperty("displayDate")]
        public DateTime Date;
        [JsonProperty("displayTime")]
        public DateTime Time;
        [JsonProperty("currency")]
        public string Currency;
        [JsonProperty("title")]
        public string Title;
        [JsonProperty("title2")]
        public string Title2;
        [JsonProperty("importance")]
        public string Importance;
        [JsonProperty("actual")]
        public string Actual;
        [JsonProperty("forecast")]
        public string Forecast;
        [JsonProperty("previous")]
        public string Previous;
        [JsonProperty("daily")]
        public bool Daily;
        [JsonProperty("live")]
        public bool Live;
        [JsonProperty("better")]
        public string Better;
        [JsonProperty("revised")]
        public string Revised;
        [JsonProperty("show")]
        public bool Show;
        [JsonProperty("sortingOrder")]
        public int SortingOrder;
        [JsonProperty("autoupdate")]
        public bool Autoupdate;
        [JsonProperty("difference")]
        public int Difference;
        [JsonProperty("language")]
        public string Language;
        [JsonProperty("commentary")]
        public string Commentary;
  



        /*
        "displayDate" : "2016-04-22T00:00:00.000+0000",
  "displayTime" : "1970-01-01T12:30:00.000+0000",
  "currency" : "cad",
  "title" : "CAD Retail Sales Less Autos (MoM)",
  "title2" : "(FEB)",
  "importance" : "low",
  "actual" : "",
  "forecast" : "-0.8%",
  "previous" : "1.2%",
  "daily" : false,
  "live" : false,
  "better" : "NONE",
  "revised" : "NONE",
  "show" : true,
  "sortingOrder" : 20,
  "autoupdate" : false,
  "difference" : 0,
  "language" : "english",
  "commentary"
  */
    }
}
