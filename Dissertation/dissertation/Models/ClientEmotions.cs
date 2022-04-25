using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dissertation.Models
{
    /// <summary>
    /// client emotion model storing child's emotional behaviour and being retrieved to be displayed on website in pie chart graph.
    /// </summary>
    /// <returns></returns>
    public class ClientEmotions
    {
        public int EmotionID { set; get; }
        public string ToneName { set; get; }
        public decimal ToneScore { set; get; }
        public int ClientID { set; get; }
        public int UserID { set; get; }
    }
}