using System.IO.Pipes;

namespace PubSub
{
    public class Publisher
    {
        public void Publish(string topic, object obj)
        {
            // wait for connection
            var stream = new NamedPipeServerStream(topic, PipeDirection.Out, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            stream.WaitForConnection();

            // serialize object
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            byte[] body = System.Text.Encoding.UTF8.GetBytes(jsonString);

            // write to stream
            stream.Write(body, 0, body.Length);
            stream.Flush();

            // close connection
            stream.Close();
        }
    }
}


