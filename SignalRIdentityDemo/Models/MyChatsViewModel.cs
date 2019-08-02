using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRIdentityDemo.Models
{
    public class MyChatsViewModel
    {
        public int ChatBoxID { get; set; }
        public AspNetUser Receiver { get; set; }
        public AspNetUser Sender { get; set; }
        public string LastMessage { get; set; }
        public string LastMessageDate { get; set; }
    }
}