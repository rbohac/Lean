using System;
using QuantConnect.Algorithm;
using QuantConnect.Brokerages;
using QuantConnect.Data.Consolidators;
using QuantConnect.Data.Market;
using QuantConnect.Indicators;

namespace QuantConnect.Algorithm.CSharp
{
    /*
    *   QuantConnect University: Full Basic Template:
    *
    *   The underlying QCAlgorithm class is full of helper methods which enable you to use QuantConnect.
    *   We have explained some of these here, but the full algorithm can be found at:
    *   https://github.com/QuantConnect/QCAlgorithm/blob/master/QuantConnect.Algorithm/QCAlgorithm.cs
    */
    public class SPYBollengerBands : QCAlgorithm
    {
        private BollingerBands bollingerBands;


        //Use our new consolidator class - 15 minutes / 15 bars joined.
        private decimal _price;

        private string security = "SPY";
        private int consolidateMinutes = 15;
        private int cash = 25000;

        public override void Initialize()
        {

            //Start and End Date range for the backtest:
            //SetStartDate(2013, 1, 1); 
           // SetStartDate(DateTime.Now.Date.AddDays(-365));
            SetStartDate(2016, 6, 1);
            SetEndDate(2016, 6, 30);

            //Cash allocation
            SetCash(cash);


            // IB Brokerage
            SetBrokerageModel(BrokerageName.InteractiveBrokersBrokerage);

            AddSecurity(SecurityType.Equity, security, Resolution.Second);
            //Securities["EURUSD"].SetLeverage(50.0m);
            bollingerBands = new BollingerBands(30, 3, MovingAverageType.Simple);
            var fifteenConsolidator = new TradeBarConsolidator(TimeSpan.FromMinutes(consolidateMinutes));
            fifteenConsolidator.DataConsolidated += OnDataFifteen;
            SubscriptionManager.AddConsolidator(security, fifteenConsolidator);
            RegisterIndicator(security, bollingerBands, fifteenConsolidator, x => x.Value);
            Log("DATE,Action,Qty,PRICE,CPrice,UpperBand,MiddleBand,LowerBand,HoldStock,buy_signal,sell_signal");
            SetWarmup(new TimeSpan(10, 0, 0, 0, 0));  // 10 Days

        }


        //Data Event Handler: New data arrives here. "TradeBars" type is a dictionary of strings so you can access it by symbol.
        public void OnData(TradeBars data)
        {
            if (!data.ContainsKey(security)) { return; }
            Log(String.Format("ALGORITHM {3},BB,{0},{1},{2}", bollingerBands.UpperBand, bollingerBands.MiddleBand, bollingerBands.LowerBand, Time.ToString("G")));

        }

        private void OnDataFifteen(object sender, TradeBar consolidated)
        {
            _price = consolidated.Close;
            if (!bollingerBands.IsReady) return;
            Plot("BB", "Price", _price);
            Plot("BB", bollingerBands.UpperBand, bollingerBands.MiddleBand, bollingerBands.LowerBand);

            //Log(String.Format("{3},BB,{0},{1},{2}", bollingerBands.UpperBand, bollingerBands.MiddleBand, bollingerBands.LowerBand, consolidated.EndTime.ToString("G")));



            decimal buy_signal = 0;
            decimal sell_signal = 0;

            if (!Portfolio.HoldStock)
            {

                
                var _buyPrice = bollingerBands.LowerBand;
                var _buySignal = consolidated.Close < _buyPrice;
                if (_buySignal)
                {
                    var orderSize = (CalculateOrderQuantity(security, 1.0m) / 1000) * 1000; // FXCM Required ordering in multiples of 1000 for EURUSD
                    Log(String.Format("ALGORITHM  {9},Order,{0},{1},{2},{3},{4},{5},{6},{7},{8}", orderSize, _price, consolidated.Close, bollingerBands.UpperBand, bollingerBands.MiddleBand, bollingerBands.LowerBand, Portfolio.HoldStock, buy_signal, sell_signal, consolidated.EndTime.ToString("G")));
                    Order(security, orderSize);
                    buy_signal = _price;
                }
            }
            else
            {
                var _sellPrice = bollingerBands.UpperBand;
                var _sellSignal = consolidated.Close > _sellPrice;
                if (_sellSignal)
                {
                    Liquidate(security);
                    sell_signal = _price;
                }
            }
            //Do not log prices
            // Log(String.Format(",{0},{1},{2},{3},{4},{5},{6},{7}",_price,consolidated.Close,bollingerBands.UpperBand, bollingerBands.MiddleBand, bollingerBands.LowerBand,Portfolio.HoldStock,buy_signal,sell_signal));

        }


    }
}