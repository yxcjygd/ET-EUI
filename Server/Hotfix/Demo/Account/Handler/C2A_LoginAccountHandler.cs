using System;
using System.Text.RegularExpressions;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;

namespace ET
{
    [MessageHandler]
    [FriendClassAttribute(typeof(ET.Account))]
    public class C2A_LoginAccountHandler : AMRpcHandler<C2A_LoginAccount, A2C_LoginAccount>
    {
        protected override async ETTask Run(Session session, C2A_LoginAccount request, A2C_LoginAccount response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Account)
            {
                Log.Error($"请求的Scene错误，当前Scene为{session.DomainScene().SceneType}");
                session?.Dispose();
                return;
            }
            session.RemoveComponent<SessionAcceptTimeoutComponent>(); // 不移除的话，连接将在5秒后断开

            if (string.IsNullOrEmpty(request.AccountName) || string.IsNullOrEmpty(request.Password))
            {
                response.Error = ErrorCode.ERR_LoginInfoError;// 可以用别的错误码！
                reply();
                session?.Dispose();
                return;
            }

            //6~15 有cap and lower case字母数字
            if (!Regex.IsMatch(request.AccountName.Trim(), @"^(?=.*[0-9].*)(?=.*[A-Z].*)(?=.*[a-z].*).{6,15}$"))
            {
                response.Error = ErrorCode.ERR_LoginInfoError;// 可以用别的错误码！
                reply();
                session?.Dispose();
                return;
            }

            if (!Regex.IsMatch(request.Password.Trim(), @"^(?=.*[0-9].*)(?=.*[A-Z].*)(?=.*[a-z].*).{6,15}$"))
            {
                response.Error = ErrorCode.ERR_LoginInfoError;// 可以用别的错误码！
                reply();
                session?.Dispose();
                return;
            }

            var accountInfoList = await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Query<Account>(d => d.AccountName.Equals(request.AccountName.Trim()));
            Account account = null;
            if (accountInfoList.Count > 0)
            {
                account = accountInfoList[0];
                session.AddChild<Account>();
                if (account.AccountType == (int)AccountType.BlackList)
                {
                    response.Error = ErrorCode.ERR_LoginInfoError;// 可以用别的错误码！
                    reply();
                    session?.Dispose();
                    return;
                }

                if (account.Password.Equals(request.Password))
                {
                    response.Error = ErrorCode.ERR_LoginInfoError;// 可以用别的错误码！
                    reply();
                    session?.Dispose();
                    return;
                }

            }
            else
            {
                account = session.AddChild<Account>();
                account.AccountName = request.AccountName.Trim();
                account.AccountName = request.Password.Trim();
                account.CreateTime = TimeHelper.ServerNow();
                account.AccountType = (int)AccountType.General;
                await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Save<Account>(account);
            }

            string Token = TimeHelper.ServerNow().ToString() + RandomHelper.RandomNumber(int.MinValue, int.MaxValue).ToString();
            session.DomainScene().GetComponent<TokenComponent>().Remove(account.Id);
            session.DomainScene().GetComponent<TokenComponent>().Add(account.Id,Token);

            response.AccountId = account.Id;
            response.Token = Token;

            reply();
        }
    }
}