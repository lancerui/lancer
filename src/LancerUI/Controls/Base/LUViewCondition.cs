using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LancerUI.Controls.Base
{
    public enum LUViewCondition
    {
        ListMinCount,
        ListMaxCount,
        EmptyList,
        NotEmptyList,
        /// <summary>
        /// 等于
        /// </summary>
        Equal,
        /// <summary>
        /// 不等于
        /// </summary>
        //NotEqual,
        /// <summary>
        /// 小于
        /// </summary>
        //LessThan,
        /// <summary>
        /// 小于或等于
        /// </summary>
        //LessThanOrEqual,
        /// <summary>
        /// 为NULL
        /// </summary>
        Null,
        /// <summary>
        /// 不为NULL
        /// </summary>
        //NotNull,
        /// <summary>
        /// 为TRUE
        /// </summary>
        True,
        /// <summary>
        /// 为FALSE
        /// </summary>
        False,
        /// <summary>
        /// 范围
        /// </summary>
        //Between,
    }
}
