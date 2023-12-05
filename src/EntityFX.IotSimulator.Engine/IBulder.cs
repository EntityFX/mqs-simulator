using System.Collections.Generic;
using System.Text;

namespace EntityFX.IotSimulator.Engine
{
    public interface IBuilder<TType>
    {
        TType Build();

    }
}
