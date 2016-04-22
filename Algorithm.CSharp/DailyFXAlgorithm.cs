﻿/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
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

/*
Examples:
# today’s events in json, minified.
https://content.dailyfx.com/getData?contenttype=calendarEvent
 
# parameter format
# json: minified
# json_pretty: human readable
https://content.dailyfx.com/getData?contenttype=calendarEvent&description=true&format=json_pretty
 
# date interval
# startdate: format YYYYMMDD or YYYYMMDDHHmmss. Look calendar events starting from date/time (inclusive). Defaults to today
# enddate: format YYYYMMDD or YYYYMMDDHHmmss. Look calendar events up to date/time (exclusive). Defaults to tomorrow
# If enddate < startdate and range greater than 3 months, values will be defaulted. Date inputs are in UTC.
https://content.dailyfx.com/getData?contenttype=calendarEvent&description=true&format=json_pretty&startdate=20160410&enddate=201604172300
*/

using System;
using Newtonsoft.Json;
using QuantConnect.Data.Custom;
using QuantConnect.Indicators;

namespace QuantConnect.Algorithm.Examples
{
    /// <summary>
    /// QuantConnect University: Generic Quandl Data Importer
    /// Using the underlying dynamic data class "Quandl" we take care of the data 
    /// importing and definition for you. Simply point QuantConnect to the Quandl Short Code.
    /// 
    /// The Quandl object has properties which match the spreadsheet headers.
    /// If you have multiple quandl streams look at data.Symbol to distinguish them.
    /// </summary>
    public class DailyFXAlgorithm : QCAlgorithm
    {
        private SimpleMovingAverage sma;
        string security = "EURUSD";

        /// Initialize the data and resolution you require for your strategy:
        public override void Initialize()
        {
            //Start and End Date range for the backtest:
            SetStartDate(2013, 1, 1);
            SetEndDate(DateTime.Now.Date.AddDays(-1));

            //Cash allocation
            SetCash(25000);

            //Add Generic Quandl Data:
            AddData<DailyFX>(security, Resolution.Second);

            //sma = SMA(security, 14);
        }

        /// Data Event Handler: New data arrives here. "TradeBars" type is a dictionary of strings so you can access it by symbol
        public void OnData(DailyFX data)
        {
       
            Debug("On Data: " + JsonConvert.SerializeObject(data).ToString());
            if (!Portfolio.HoldStock)
            {
                //Order function places trades: enter the string symbol and the quantity you want:
                SetHoldings(security, 1);

                //Debug sends messages to the user console: "Time" is the algorithm time keeper object 
               // Debug("Purchased " + _quandlCode + " >> " + Time.ToShortDateString());
            }

            Plot("SPY", sma);
        }
    }
}