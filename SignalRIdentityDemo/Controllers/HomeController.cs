using SignalRIdentityDemo.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalRIdentityDemo.Controllers
{
    public class HomeController : Controller
    {
        ChatRoomEntities context = new ChatRoomEntities();
        [Authorize]
        public ActionResult Index()
        {
            var messages = context.ChatMessages.Where(x => x.ReceiverId == null).ToList();
            return View(messages);
        }

        [Authorize]
        public ActionResult Chat()
        {
            string userId = User.Identity.GetUserId();
            var usertouserchat = context.ChatMessages.Where(x => x.UserId == userId).ToList();
            return View(usertouserchat);
        }

        [Authorize]
        public ActionResult MyChats()
        {
            string userId = User.Identity.GetUserId();
            var c = context.ChatBoxes.FirstOrDefault(x => x.Id == 1);
            List<AspNetUser> listuser = new List<AspNetUser>();
            List<MyChatsViewModel> usertouserchat = (from ctbx in context.ChatBoxes.Where(x => x.SenderID == userId || x.ReceiverID == userId).ToList()
                                                     join u in context.AspNetUsers.ToList() on ctbx.SenderID equals u.Id
                                                     join r in context.AspNetUsers.ToList() on ctbx.ReceiverID equals r.Id
                                                     select new MyChatsViewModel
                                                     {
                                                         ChatBoxID = ctbx.Id,
                                                         Receiver = r,
                                                         Sender = u,
                                                         LastMessage = ctbx.ChatMessages == null || ctbx.ChatMessages.Count() == 0 ? "" :
                                                         ctbx.ChatMessages.OrderByDescending(x => x.Date).Select(x => x.Message).Take(1).FirstOrDefault().ToString(),

                                                         LastMessageDate = ctbx.ChatMessages == null || ctbx.ChatMessages.Count() == 0 ? "" :
                                                         ctbx.ChatMessages.OrderByDescending(x => x.Date).Select(x => x.Date).Take(1).FirstOrDefault().ToShortDateString()
                                                     }).ToList();
            foreach (var item in usertouserchat)
            {
                if (!listuser.Contains(item.Receiver))
                {
                    listuser.Add(item.Receiver);
                }
                if (!listuser.Contains(item.Sender))
                {
                    listuser.Add(item.Sender);
                }
            }

            var otherUsers = context.AspNetUsers.ToList().Where(p => !listuser.Select(i => i.Id).Contains(p.Id)).ToList();

            MyChatOtherUsersViewModel myChatOtherUsersViewModel = new MyChatOtherUsersViewModel
            {
                myChatsViewModels = usertouserchat,
                otherusers = otherUsers
            };

            return View(myChatOtherUsersViewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [HttpGet]
        public PartialViewResult GetChatsByUserId(string id)
        {
            string userId = User.Identity.GetUserId();
            var messages = context.ChatMessages.Where(x => x.UserId == userId &&
              x.ReceiverId == id || x.UserId==id && x.ReceiverId==userId).OrderBy(x=>x.Date).ToList();
            return PartialView("_GetChatsByUserId", messages);
        }
    }
}