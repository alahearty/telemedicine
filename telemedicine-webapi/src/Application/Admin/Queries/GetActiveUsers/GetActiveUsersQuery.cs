//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Web;
//using MediatR;
//using telemedicine_webapi.Application.Common.Models;

//namespace telemedicine_webapi.Application.Admin.Queries.GetActiveUsers;
//public record GetActiveUsersQuery : IRequest<BaseResponse>;

//public class GetActiveUsersQueryHandler : IRequestHandler<GetActiveUsersQuery, BaseResponse>
//{
//    public async Task<BaseResponse> Handle(GetActiveUsersQuery request, CancellationToken cancellationToken)
//    {
//        throw new NotImplementedException();
//    }
//    private List<String> getOnlineUsers()
//    {
//        List<String> activeSessions = new List<String>();
//        object obj = typeof(HttpRuntime).GetProperty("CacheInternal", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null, null);
//        object[] obj2 = (object[])obj.GetType().GetField("_caches", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
//        for (int i = 0; i < obj2.Length; i++)
//        {
//            Hashtable c2 = (Hashtable)obj2[i].GetType().GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj2[i]);
//            foreach (DictionaryEntry entry in c2)
//            {
//                object o1 = entry.Value.GetType().GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(entry.Value, null);
//                if (o1.GetType().ToString() == "System.Web.SessionState.InProcSessionState")
//                {
//                    SessionStateItemCollection sess = (SessionStateItemCollection)o1.GetType().GetField("_sessionItems", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(o1);
//                    if (sess != null)
//                    {
//                        if (sess["loggedInUserId"] != null)
//                        {
//                            activeSessions.Add(sess["loggedInUserId"].ToString());
//                        }
//                    }
//                }
//            }
//        }
//        return activeSessions;
//    }
//    public static List<LoggedInUsers> OnlineUsers
//    {
//        get
//        {
//            if (System.Web.HttpContext.Current.Cache["OnlineUsers"] != null)
//                return (List<LoggedInUsers>)System.Web.HttpContext.Current.Cache["OnlineUsers"];
//            else
//                List<LoggedInUsers> list = My_MSI.Net.Controllers.OnlineUsers.List;
//            System.Web.HttpContext.Current.Cache.Add("OnlineUsers", list, null, DateTime.MaxValue, new TimeSpan(), CacheItemPriority.NotRemovable, null);
//            return list;
//        }
//    }


//}
//public class LoggedInUsers
//{

//}
