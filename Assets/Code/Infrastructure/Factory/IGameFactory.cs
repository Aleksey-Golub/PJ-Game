using System.Collections.Generic;
using Code.Services;
using UnityEngine;

namespace Code.Infrastructure
{
  public interface IGameFactory : IService
  {
    List<ISavedProgressReader> ProgressReaders { get; }
    List<ISavedProgress> ProgressWriters { get; }
    GameObject CreateHero(GameObject at);
    Hud CreateHud();
    void Cleanup();
  }
}