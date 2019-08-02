using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRIdentityDemo.Models
{
    public class MyChatOtherUsersViewModel
    {
        public List<MyChatsViewModel> myChatsViewModels { get; set; }
        public List<AspNetUser> otherusers { get; set; }
    }
}