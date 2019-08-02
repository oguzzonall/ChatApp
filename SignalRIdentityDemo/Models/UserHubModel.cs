using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRIdentityDemo.Models
{
    public class UserHubModel
    {
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
    }
}