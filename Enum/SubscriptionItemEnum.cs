using System.ComponentModel;

namespace TGBot_TW_Stock_Webhook.Enum
{
    public enum SubscriptionItemEnum
    {
        /// <summary> 個股資訊 </summary>
        [Description("個股資訊")]
        StockInfo = 1,

        /// <summary> 個股新聞 </summary>
        [Description("個股新聞")]
        StockNews = 2,

        /// <summary> 當日市場成交資訊 </summary>
        [Description("當日市場成交資訊")]
        DailyMarketInfo = 3,

        /// <summary> 成交量前20股票 </summary>
        [Description("成交量前20股票")]
        TopVolumeItems = 4,
    }
}