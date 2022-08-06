using System;


namespace ET
{
    public static class LoginHelper
    {
        public static async ETTask<int> Login(Scene zoneScene, string address, string account, string password)
        {

            A2C_LoginAccount a2CLoginAccount = null;
            Session accountSession = null;
            try
            {
                accountSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(address));
                password = MD5Helper.StringMD5(password);
                a2CLoginAccount = (A2C_LoginAccount)await accountSession.Call(new C2A_LoginAccount() { AccountName = account, Password = password });
            }
            catch (Exception e)
            {
                accountSession?.Dispose();
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }

            if (a2CLoginAccount.Error != ErrorCode.ERR_Success)
            {
                accountSession?.Dispose();
                return a2CLoginAccount.Error;
            }

            zoneScene.AddComponent<SessionComponent>().Session = accountSession;
            
            zoneScene.GetComponent<SessionComponent>().Session.AddComponent<PingComponent>();
            
            var accountInfoComponent = zoneScene.GetComponent<AccountInfoComponent>();
            AccountInfoComponentSystem.SetToken(accountInfoComponent,a2CLoginAccount.Token);
            AccountInfoComponentSystem.SetAccountId(accountInfoComponent,a2CLoginAccount.AccountId);
            //zoneScene.GetComponent<AccountInfoComponent>().Token = a2CLoginAccount.Token; // I dont want LoginHelper to be friendclass of AccountInfoComponent, so commented out this 2 lines
            //zoneScene.GetComponent<AccountInfoComponent>().AccountId = a2CLoginAccount.AccountId;

            return ErrorCode.ERR_Success;
        }
    }
}