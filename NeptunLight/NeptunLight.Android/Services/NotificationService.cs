using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Java.IO;
using NeptunLight.Models;
using NeptunLight.Services;

namespace NeptunLight.Droid.Services
{
    public class NotificationService : IMailNotificationDispatcher
    {
        private readonly IPrimitiveStorage _primitiveStorage;

        public const string CHANNEL_ID = "messages";
        protected internal const string NOTIFICATION_ID_PREFERENCE_KEY = "NOTIFICATION_ID";

        private string ChannelName { get; } = "Üzenetek";

        private NotificationManager NotificationManager => (NotificationManager)_context.GetSystemService(Context.NotificationService);

        private Context _context;

        public NotificationService(IPrimitiveStorage primitiveStorage)
        {
            _primitiveStorage = primitiveStorage;
        }

        public void Bootstrap(Context context)
        {
            _context = context;

            CreateNotificationChannel();
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }
            NotificationChannel channel = new NotificationChannel(CHANNEL_ID, ChannelName, NotificationImportance.High);
            NotificationManager.CreateNotificationChannel(channel);
        }

        private int GetNextNotificationId()
        {
            var lastId = _primitiveStorage.ContainsKey(NOTIFICATION_ID_PREFERENCE_KEY) ? _primitiveStorage.GetInt(NOTIFICATION_ID_PREFERENCE_KEY) : 0;
            lastId++;
            _primitiveStorage.PutInt(NOTIFICATION_ID_PREFERENCE_KEY, lastId);
            return lastId;
        }


        public void NotifyNewMail(Mail mail)
        {
            ThrowIfContextNull();
            Notification notification = new NotificationCompat.Builder(_context, CHANNEL_ID)
                .SetWhen(new DateTimeOffset(mail.ReceivedTime).ToUnixTimeMilliseconds())
                .SetContentTitle(mail.Sender)
                .SetContentText(mail.Subject)
                .SetSmallIcon(Resource.Drawable.ic_stat_mail_outline)
                .SetPriority(NotificationCompat.PriorityHigh)
                .Build();

            NotificationManager.Notify(GetNextNotificationId(), notification);

        }

        public void NotifyMultipleNewMails(IEnumerable<Mail> newMails)
        {
            ThrowIfContextNull();
        }

        private void ThrowIfContextNull()
        {
            if (_context == null)
                throw new InvalidObjectException("Context is null. Call Bootstrap before this method.");
        }
    }
}