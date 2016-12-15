using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOvF.Models
{
    public class User
    {
        public string name { get; set; }
        public string title { get; set; }
        public string percent { get; set; }
        public string currentPosition { get; set; }
        public string twitterUrl { get; set; }
        public string githubUrl { get; set; }
        public string siteUrl { get; set; }
        public string imageUrl { get; set; }
        public string languages { get; set; }
        public Flair flair { get; set; }
        internal string reputationText { get; set; }
        public int reputation { get; set; }
        public string address { get; set; }
        public string stackOverFlowAge { get; set; }
        public string bio { get; set; }

        public class Flair
        {
            public string gold { get; set; }
            public string silver { get; set; }
            public string bronze { get; set; }
        }
    }
}
