using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using NLog;

namespace WCFChat
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceChat : IServiceChat
    {
        private List<ServerUser> users = new List<ServerUser>();
        private Logger logger = LogManager.GetCurrentClassLogger();
        private int nextId = 1;
        private List<ServerFiles> files = new List<ServerFiles>();


        #region Connection
        public int Connect(string name)
        {
            if (!users.Exists(o => string.Equals(name, o.Name)))
            {
                ServerUser user = new ServerUser()
                {
                    ID = nextId,
                    Name = name,
                    operationContext = OperationContext.Current
                };
                nextId++;

                SendMsg(" - " + user.Name + " подключился к чату!", 0);
                users.Add(user);
                return user.ID;
            }
            else
            {
                return 0;
            }
        }

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(i => i.ID == id);
            if (user != null)
            {
                users.Remove(user);
                SendMsg(" - " + user.Name + " покинул чат!", 0);
            }
        }
        #endregion

        #region Online
        public string GetOnline()
        {
            string list = string.Empty;
            for (int i = 0; i < users.Count; i++)
            {
                if (i != users.Count - 1)
                    list += users[i].Name;
            }
            return list;
        }

        public void ShowOnline()
        {
            List<string> usersO = new List<string>();
            for (int i = 0; i < users.Count; i++)
            {
                usersO.Add(users[i].Name);
            }
            //string list = GetOnline();
            foreach (var item in users)
            {
                item.operationContext.GetCallbackChannel<IServerChatCallback>().ShowOnlineCallback(usersO);
            }
        }
        #endregion

        #region Messages
        public void SendMsg(string msg, int id)
        {
            foreach (var item in users)
            {
                string answer = DateTime.Now.ToShortTimeString();

                var user = users.FirstOrDefault(i => i.ID == id);
                if (user != null)
                {
                    answer += " - " + user.Name + " ";
                }
                answer += msg;
                item.operationContext.GetCallbackChannel<IServerChatCallback>().MsgCallback(answer);
            }
        }
        #endregion

        #region Files
        public void AddFile(byte[] filebyte, string filepath)
        {
            ServerFiles file = new ServerFiles()
            {
                FileBytes = (byte[])filebyte.Clone()
            };
            files.Add(file);
            foreach (var item in users)
            {
                item.operationContext.GetCallbackChannel<IServerChatCallback>().ShowFilesCallback(filepath);
            }
        }

        public void DownloadFile(int id, string path)
        {
            users[id].operationContext.GetCallbackChannel<IServerChatCallback>().DownloadFileCallback(files[files.Count - 1].FileBytes, path);
        }
        #endregion
    }
}
