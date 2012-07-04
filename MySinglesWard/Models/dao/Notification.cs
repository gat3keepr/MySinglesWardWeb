using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
	[Serializable]
    public class Notification
    {
        public double WardID { get; set; }
		public string notification { get; set; }
		public int NotificationID { get; set; }
        public bool isModerated { get; set; }

		public static Notification get(int NotificationID)
		{
			Notification notification = Cache.Get(Cache.getCacheKey<Notification>(NotificationID)) as Notification;

			if (notification == null)
			{
				notification = new Notification(NotificationID);

				Cache.Set(Cache.getCacheKey<Notification>(NotificationID), notification);
			}

			return notification;
		}

		public static void save(Notification notification)
		{
			Cache.Remove(Cache.getCacheKey<Notification>(notification.NotificationID));

			using (var db = new DBmsw())
			{
				var targetNotification = db.tNotifications.SingleOrDefault(x => x.NotificationID == notification.NotificationID);

				if (targetNotification == null)
				{
					targetNotification = new tNotification();
					targetNotification.NotificationID = notification.NotificationID;
					db.tNotifications.InsertOnSubmit(targetNotification);
				}

				targetNotification.Notification = notification.notification;
				targetNotification.WardID = notification.WardID;
				targetNotification.isModerated = notification.isModerated;

				db.SubmitChanges();

				Cache.Set(Cache.getCacheKey<Notification>(notification.NotificationID), notification);
			}
		}

		public static int create(tNotification notification)
		{
            Cache.Remove("WardNotifications:" + notification.WardID);

			using (var db = new DBmsw())
			{
				var targetNotification = new tNotification();
				db.tNotifications.InsertOnSubmit(targetNotification);

				targetNotification.Notification = notification.Notification;
				targetNotification.WardID = notification.WardID;
				targetNotification.isModerated = notification.isModerated;

				db.SubmitChanges();
				Cache.Set(Cache.getCacheKey<Notification>(targetNotification.NotificationID), new Notification(targetNotification.NotificationID));
				return targetNotification.NotificationID;
			}
		}

		public static void remove(Notification notification)
		{
			Cache.Remove(Cache.getCacheKey<Notification>(notification.NotificationID));
            Cache.Remove("WardNotifications:" + notification.WardID);

			using (var db = new DBmsw())
			{
				var targetNotification = db.tNotifications.SingleOrDefault(x => x.NotificationID == notification.NotificationID);

				db.tNotifications.DeleteOnSubmit(targetNotification);
				db.SubmitChanges();
			}
		}

        private Notification(int id)
		{
			using (var db = new DBmsw())
			{
				var dboNotification = db.tNotifications.SingleOrDefault(x => x.NotificationID == id);

				WardID = dboNotification.WardID;
				notification = dboNotification.Notification;
				this.NotificationID = dboNotification.NotificationID;
				this.isModerated = dboNotification.isModerated;
			}
		}
    }
}