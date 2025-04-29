using System;

[Serializable]
public class Message
{
    public string sender;
    public string message;
    public string name;

    // Add constructor
    public Message(string sender, string message, string name)
    {
        this.sender = sender;
        this.message = message;
        this.name = name;
    }

    // Need empty constructor for JSON deserialization
    public Message() { }
}

[Serializable]
public class MessageList
{
    public Message[] messages;
}
