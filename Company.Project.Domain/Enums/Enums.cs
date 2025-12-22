namespace Company.Project.Domain.Enums;

public class Enums
{
    public enum ValidationStatus
    {
        pending ,
        validated ,
        expired 
    }
    public enum GenderType
    {
        Male,
        Female
    }

    public enum TransactionStatus
    {
        Pending,
        Success,
        Failed,
        Refunded
    }
    public enum MessageSender
    {
        User,
        Bot
    }
    
}