using System.Collections.Generic;
using System.Text;

namespace EntityFX.IotSimulator.Engine.Builder
{
    public interface IBuilder<TType>
    {
        TType Build();

    }
}
