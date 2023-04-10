using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SmartLight.Messages
{
    public class UpdateDevice : ValueChangedMessage<string>
    {
        public UpdateDevice(string value) : base(value)
        {
        }
    }
}
 