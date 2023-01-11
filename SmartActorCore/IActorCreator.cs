using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartActorCore
{
    public interface IActorCreator
    {
        IActorId CreateActorId();

        IActor CreateActor();
    }
}
