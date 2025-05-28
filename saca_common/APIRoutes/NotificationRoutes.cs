using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Routes
{
    public static class NotificationRoutes
    {
        public const string INDEX = "notification";
        public static class ACTION
        {
            public const string GetById = "{id}";
            public const string GetAll = "";
            public const string Create = "";
            public const string Update = "";
            public const string Delete = "";
            public const string SendNotificationToContest = "send-notification-to-contest";
            public const string MarkAsRead = "mark-as-read/{notification_id}";
            public const string DeleteAccountNotification = "delete-account-notification/{id}";
            public const string GetAccountNotification = "account-notification";
        }
    }
}
