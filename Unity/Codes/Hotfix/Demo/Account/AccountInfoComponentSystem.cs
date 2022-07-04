namespace ET
{

    public class AccountInfoComponentDestroySystem: DestroySystem<AccountInfoComponent>
    {
        public override void Destroy(AccountInfoComponent self)
        {
            self.Token = string.Empty;
            self.AccountId = 0;
        }
    }
    [FriendClassAttribute(typeof(ET.AccountInfoComponent))]
    public static class AccountInfoComponentSystem
    {
        public static void SetToken(AccountInfoComponent accountInfoComponent, string Token)
        {
            accountInfoComponent.Token = Token;
        }

        public static void SetAccountId(AccountInfoComponent accountInfoComponent, long AccountId)
        {
            accountInfoComponent.AccountId = AccountId;
        }


    }
}