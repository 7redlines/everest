using System;
using GalaSoft.MvvmLight.Messaging;

namespace Se7enRedLines
{
    public static class MessengerExtensions
    {
        public static void Publish<T>(this IMessenger messenger, object token, T content)
        {
            messenger.Send(content, token);
        }

        public static void Publish(this IMessenger messenger, object token)
        {
            messenger.Send(new NotificationMessage(token.ToString()), token);
        }

        public static void Subscribe<T>(this IMessenger messenger, object recipient, object token, Action<T> action)
        {
            messenger.Register(recipient, token, action);
        }

        public static void Subscribe(this IMessenger messenger, object recipient, object token,
            Action<NotificationMessage> action)
        {
            messenger.Register(recipient, token, action);
        }
    }
}