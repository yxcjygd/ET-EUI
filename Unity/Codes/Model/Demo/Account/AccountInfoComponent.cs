namespace ET
{
    [ComponentOf(typeof(Scene))]
    //[ChildType(typeof(LoginHelper))]
    public class AccountInfoComponent:Entity,IAwake,IDestroy
    {
        public string Token;
        public long AccountId;
    }
    
    
}