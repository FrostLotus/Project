namespace ClassLibrary.SharedComponent.Log
{
    public interface IAppLogProcess
    {
        void InsertDebugLog(string xMsg, AOI_LOG_TYPE xType);
        void StartLogServer();
        void StopLogServer();
    }
}