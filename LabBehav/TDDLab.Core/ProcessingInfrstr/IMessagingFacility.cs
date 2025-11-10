namespace TDDLab.Core.Infrastructure
{
    public interface IMessagingFacility<InputType, OutputType>
    {
        void InitializeInputChannel(string channelId);
        void InitializeOutputChannel(string channelId);
        Message<InputType> ReadMessage();
        void WriteMessage(Message<OutputType> message);
        void Dispose();
    }
}