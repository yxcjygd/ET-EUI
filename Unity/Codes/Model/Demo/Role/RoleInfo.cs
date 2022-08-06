namespace ET
{

    public enum RoleInfoState
    {
        Normal = 0,
        Freeze = 1,
        
    }


    public class RoleInfo:Entity,IAwake
    {
        public string Name;
        public int ServerId; // 所在区服id

        public int State; // 状态

        public long AccountId;
        public long LastLoginTime; // 上次登录时间
        public long CreateTime; // 创角时间
    }
}