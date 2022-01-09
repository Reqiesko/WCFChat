using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFChat
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IServiceChat" в коде и файле конфигурации.
    [ServiceContract(CallbackContract = typeof(IServerChatCallback))]
    public interface IServiceChat
    {
        [OperationContract]
        int Connect(string name);

        [OperationContract]
        void Disconnect(int id);

        [OperationContract]
        string GetOnline();

        [OperationContract(IsOneWay = true)]
        void SaveMsg(string msg);

        [OperationContract(IsOneWay = true)]
        void ShowMessages(int id);

        [OperationContract(IsOneWay = true)]
        void SendMsg(string msg, int id);

        [OperationContract(IsOneWay = true)]
        void ShowOnline();

        [OperationContract(IsOneWay = true)]
        void AddFile(byte[] file, string filepath);

        [OperationContract(IsOneWay = true)]
        void DownloadFile(int id, string path);

    }

    public interface IServerChatCallback
    {
        [OperationContract(IsOneWay = true)]
        void MsgCallback(string msg);

        [OperationContract(IsOneWay = true)]
        void ShowMsgCallback(string text);

        [OperationContract(IsOneWay = true)]
        void ShowOnlineCallback(List<string> users);

        [OperationContract(IsOneWay = true)]
        void ShowFilesCallback(string filepath);

        [OperationContract(IsOneWay = true)]
        void DownloadFileCallback(byte[] file, string path);
    }
}
