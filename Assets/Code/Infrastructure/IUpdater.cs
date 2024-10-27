using Code.Services;
using System.Collections.Generic;

namespace Code.Infrastructure
{
    public interface IUpdater : IService
    {
        public List<IUpdatable> Updatables { get; set; }
    }
}