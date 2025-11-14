

using System.Data;

namespace ForUser.Domains.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class UnitOfWorkAttribute:Attribute
    {
        public bool IsTransactional { get; set; } = true;

        public bool IsDisabled { get; set; } = false;

        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;
    }

    // 用于禁用工作单元的特性
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class DisableUnitOfWorkAttribute : Attribute { }
}
