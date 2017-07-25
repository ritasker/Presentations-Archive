using System;
using System.Collections.Generic;

namespace MediatorDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var chatRoom = new ChatRoom();

            var chatUser1 = new HumanUser("Rich", chatRoom);
            var chatUser2 = new ChatBot("Bot 1", chatRoom);
            var chatUser3 = new ChatBot("Bot 2", chatRoom);

            chatRoom.AddUser(chatUser1);
            chatRoom.AddUser(chatUser2);
            chatRoom.AddUser(chatUser3);

            while (true)
            {
                Console.WriteLine(string.Empty);
                Console.Write(">");
                var message = Console.ReadLine();

                if(message.ToLower() == "exit")
                    break;

                chatUser1.Send(message);
            }
        }
    }

    public interface IChatRoom
    {
        void Send(string message);
        void AddUser(IChatUser chatUser);
    }

    public interface IChatUser
    {
        void Send(string message);
        void Notify(string message);

        string DisplayName { get; }
    }

    public class ChatRoom : IChatRoom
    {
        private readonly List<IChatUser> _chatUsers;

        public ChatRoom()
        {
            _chatUsers = new List<IChatUser>();
        }

        public void Send(string message)
        {
            _chatUsers.ForEach(c => c.Notify(message));
        }

        public void AddUser(IChatUser chatUser)
        {
            _chatUsers.Add(chatUser);
            Console.WriteLine(string.Format("{0} joined the chatroom.", chatUser.DisplayName));
        }
    }

    public class HumanUser : IChatUser
    {
        public string DisplayName { get; }
        private readonly ChatRoom _mediator;

        public HumanUser(string displayName, ChatRoom mediator)
        {
            DisplayName = displayName;
            _mediator = mediator;
        }

        public void Send(string message)
        {
            _mediator.Send(message);
        }

        public void Notify(string message)
        {
            Console.WriteLine("{0} sent: {1}", DisplayName, message);
        }
    }

    public class ChatBot : IChatUser
    {
        public string DisplayName { get; }
        private readonly ChatRoom _mediator;

        public ChatBot(string displayName, ChatRoom mediator)
        {
            DisplayName = displayName;
            _mediator = mediator;
        }

        public void Send(string message)
        {
            _mediator.Send(message);
        }

        public void Notify(string message)
        {
            Console.WriteLine("{0} received: {1}", DisplayName, message);
        }
    }
}
